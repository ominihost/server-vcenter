using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using OminiHost_Server.Http.Data;
using VMware.Vim;
using OminiHost_Server.Class;
using System.Diagnostics;

namespace OminiHost_Server.Http
{
    class ServerListen
    {
        TelaLoad telaLoad;
        
        public ServerListen(TelaLoad tela)
        {
            this.telaLoad = tela;
        }
        public void startListen(Object l)
        {
            HttpListener listener = (HttpListener)l;
            while (listener.IsListening)
            {                                
                var context = listener.GetContext();
                string IP_Client = IPAddress.Parse((context.Request.RemoteEndPoint).Address.ToString()).ToString();

                bool naoTemAcesso = true;
                foreach (string ip in this.telaLoad.IPS_PERMITIDOS)
                {
                    if (ip.Equals(IP_Client) == true)
                    {
                        Thread cliente = new Thread(clientListen);
                        cliente.Start(context);
                        naoTemAcesso = false;
                        break;
                    }
                }
                if (naoTemAcesso)
                {
                    SendString(context, "You don't have permission");
                    this.telaLoad.TelaInicial.WriteConsole(IP_Client + " Tentou fazer uma conexão mas não conseguiu pois não tem permissão");
                }
            }
        }
        public void clientListen(Object ct)
        {
            HttpListenerContext context = (HttpListenerContext)ct;
            string IP_Client = IPAddress.Parse((context.Request.RemoteEndPoint).Address.ToString()).ToString();

            var body = new StreamReader(context.Request.InputStream).ReadToEnd();

            try
            {             
                Base data = JsonConvert.DeserializeObject<Base>(body);

                if(!data.sPPk.Equals(this.telaLoad.config.password))
                {
                    this.telaLoad.TelaInicial.WriteConsole(IP_Client + " Tentou executar um cmd mas a senha era inválida(Senha usada: "+data.sPPk+")");
                    SendString(context, "Invalid password");
                    return;
                }
                Response saida = new Response();                    

                switch(data.cmd)
                {
                    case "isAlive":
                        saida.Result = true;
                        SendString(context, JsonConvert.SerializeObject(saida) );
                        break;

                   case "vcenterIsAlive":
                        {
                            vmwareConnect vmware = this.telaLoad.vmware;
                            VimClient vimClient = this.telaLoad.clientTasks;
                            try
                            {
                                if (vimClient.ServiceTimeout > 0)
                                    saida.Result = (bool)true;
                                else
                                    saida.Result = (bool)false;
                                SendString(context, JsonConvert.SerializeObject(saida));
                            }
                            catch (Exception ex)
                            {
                                Omini.debug("ERRO vcenterIsAlive " + ex.Message);
                                saida.Result = (bool)false;
                                SendString(context, JsonConvert.SerializeObject(saida));
                            }
                            break;
                        }

                    case "criarVM":
                        {
                            vmwareConnect vmware = this.telaLoad.vmware;
                            VimClient vimClient = this.telaLoad.clientTasks;
                            try
                            {
                                OminiHost_Server.Data.VMEspec vmEspec = JsonConvert.DeserializeObject<OminiHost_Server.Data.VMEspec>( Omini.Base64Decode(data.otherData) );

                                Debug.WriteLine("(criarVM) " + Omini.Base64Decode(data.otherData) + data.sPPk + data.cmd);

                                if (this.telaLoad.ip_tarefas.ContainsKey(vmEspec.ipVM))
                                {
                                    Tarefa tarefaExistente = this.telaLoad.ip_tarefas[vmEspec.ipVM];
                                    if (tarefaExistente.estadoAtual == "terminada" || tarefaExistente.estadoAtual == "cancelada")
                                    {
                                        goto novaTarefa;
                                    }
                                    else
                                    {
                                        saida.Result = "ja existe uma tarefa";
                                        goto PularNovaTarefaCreate;
                                    }
                                }
                                novaTarefa:
                                VirtualMachine vm = vmware.getMachine(vimClient, vmEspec.ipVM);
                                if (vm == null)
                                {
                                    Tarefa tf = new Tarefa(this.telaLoad, vmEspec);
                                    tf.setTipoTarefa(Tarefa.Tipos.criacao);

                                    switch (tf.clonarTemplate(vmEspec.nameDC))
                                    {
                                        case Tarefa.Erros_Criacao.Sucesso:
                                            saida.Result = "clonando";
                                            this.telaLoad.TelaInicial.WriteConsole(" Criando nova VM IP: " + vmEspec.ipVM);                                            
                                            break;
                                        case Tarefa.Erros_Criacao.DedicadoNaoExiste:
                                            saida.Result = "DedicaoFail";
                                            this.telaLoad.TelaInicial.WriteConsole(" Falha ao criar VM IP: " + vmEspec.ipVM + " Dedicado " + vmEspec.dedicado + " não encontrado");
                                            break;
                                        case Tarefa.Erros_Criacao.DedicadoNaoTemHosts:
                                            saida.Result = "DedicadoVazio";
                                            this.telaLoad.TelaInicial.WriteConsole(" Falha ao criar VM IP: " + vmEspec.ipVM + " Dedicado " + vmEspec.dedicado + " está vazio, não possui hosts ESXI");
                                            break;
                                        case Tarefa.Erros_Criacao.TemplateNaoEncontrado:
                                            this.telaLoad.TelaInicial.WriteConsole(" Falha ao criar VM IP: " + vmEspec.ipVM + " Template " + vmEspec.templateSystemName + " não encontrado");
                                            saida.Result = "TemplateFail";
                                            break;

                                    }
                                    if (this.telaLoad.ip_tarefas.ContainsKey(vmEspec.ipVM))
                                        this.telaLoad.ip_tarefas[vmEspec.ipVM] = tf;
                                    else
                                        this.telaLoad.ip_tarefas.Add(vmEspec.ipVM, tf);
                                }
                                else
                                {
                                    saida.Result = "ip em uso";
                                    goto PularNovaTarefaCreate;
                                }

                                PularNovaTarefaCreate:
                                SendString(context, JsonConvert.SerializeObject(saida));
                            }
                            catch (Exception ex)
                            {
                                this.telaLoad.TelaInicial.WriteConsole("Client(" + IP_Client + ")(Comando:" + data.cmd + ") ERRO: " + ex.Message);
                                Debug.WriteLine("(CriarVM ERROR) " + ex.StackTrace);                                
                                saida.Result = "erro";
                                SendString(context, JsonConvert.SerializeObject(saida));
                            }
                            break;
                        }
                    case "formatarVM":
                        {
                            vmwareConnect vmware = this.telaLoad.vmware;
                            VimClient vimClient = this.telaLoad.clientTasks;
                            Debug.WriteLine("(formatarVM) " + Omini.Base64Decode(data.otherData) + data.sPPk + data.cmd);
                            try
                            {
                                OminiHost_Server.Data.VMEspec vmEspec = JsonConvert.DeserializeObject<OminiHost_Server.Data.VMEspec>( Omini.Base64Decode(data.otherData) );
                                this.telaLoad.TelaInicial.WriteConsole("Formatando Network Name: " + vmEspec.NetworkName);
                                if (this.telaLoad.ip_tarefas.ContainsKey(vmEspec.ipVM))
                                {
                                    Tarefa tarefaExistente = this.telaLoad.ip_tarefas[vmEspec.ipVM];
                                    if (tarefaExistente.estadoAtual == "terminada" || tarefaExistente.estadoAtual == "cancelada")
                                    {
                                        goto novaTarefa;
                                    }
                                    else
                                    {
                                        saida.Result = "ja existe uma tarefa";
                                        goto PularNovaTarefaFormat;
                                    }
                                }
                                novaTarefa:
                                VirtualMachine vm = vmware.getMachine(vimClient, vmEspec.ipVM);
                                if (vm == null)
                                {
                                    saida.Result = "vm nao encontrada";
                                    goto PularNovaTarefaFormat;
                                }
                                else
                                {
                                    Tarefa tf = new Tarefa(this.telaLoad, vmEspec);
                                    tf.setTipoTarefa(Tarefa.Tipos.formatacao);
                                    Tarefa.Erros_Formatacao reste = tf.deletarVM(vmEspec.ipVM);
                                    switch (reste)
                                    {
                                        case Tarefa.Erros_Formatacao.Sucesso:                                            
                                            saida.Result = "formatando";
                                            this.telaLoad.TelaInicial.WriteConsole("Formatando a VM IP: " + vmEspec.ipVM);
                                            break;
                                        case Tarefa.Erros_Formatacao.VMNaoEncontrada:
                                            saida.Result = "vm nao encontrada";
                                            this.telaLoad.TelaInicial.WriteConsole("Falha ao formatar VM IP: " + vmEspec.ipVM + " a maquina não foi encontrada");
                                            break;

                                    }
                                    if (this.telaLoad.ip_tarefas.ContainsKey(vmEspec.ipVM))
                                        this.telaLoad.ip_tarefas[vmEspec.ipVM] = tf;
                                    else
                                        this.telaLoad.ip_tarefas.Add(vmEspec.ipVM, tf);
                                }
                                PularNovaTarefaFormat:

                                SendString(context, JsonConvert.SerializeObject(saida));
                            }
                            catch (Exception ex)
                            {
                                this.telaLoad.TelaInicial.WriteConsole("Client(" + IP_Client + ")(Comando:" + data.cmd + ") ERRO: " + ex.Message);
                                saida.Result = "erro";
                                SendString(context, JsonConvert.SerializeObject(saida));
                            }
                            break;
                        }
                    case "powerOnVM":
                        {
                            vmwareConnect vmware = this.telaLoad.vmware;
                            VimClient vimClient = this.telaLoad.clientTasks;
                            Debug.WriteLine("(powerOnVM) " + Omini.Base64Decode(data.otherData) + data.sPPk + data.cmd);
                            try
                            {
                                OminiHost_Server.Data.powerOnOffSuspend vmForChange = JsonConvert.DeserializeObject<OminiHost_Server.Data.powerOnOffSuspend>( Omini.Base64Decode(data.otherData) );

                                VirtualMachine vm = vmware.getMachine(vimClient, vmForChange.vmip);
                                if (vm.Runtime.PowerState == VirtualMachinePowerState.poweredOn)
                                {
                                    saida.Result = "ja on";
                                }
                                else
                                {                                    
                                    vm.PowerOnVM_Task(vm.Runtime.Host);
                                    saida.Result = "poweredOn";
                                }

                                SendString(context, JsonConvert.SerializeObject(saida) );

                            }
                            catch (Exception ex)
                            {
                                this.telaLoad.TelaInicial.WriteConsole("Client(" + IP_Client + ")(Comando:" + data.cmd + ") ERRO: " + ex.Message);
                                saida.Result = "erro";
                                SendString(context, JsonConvert.SerializeObject(saida));
                            }
                            break;
                        }
                    case "powerOffVM":
                        {
                            vmwareConnect vmware = this.telaLoad.vmware;
                            VimClient vimClient = this.telaLoad.clientTasks;
                            Debug.WriteLine("(powerOffVM) " + Omini.Base64Decode(data.otherData) + data.sPPk + data.cmd);
                            try
                            {
                                OminiHost_Server.Data.powerOnOffSuspend vmForChange = JsonConvert.DeserializeObject<OminiHost_Server.Data.powerOnOffSuspend>(Omini.Base64Decode(data.otherData));
                                VirtualMachine vm = vmware.getMachine(vimClient, vmForChange.vmip);
                                if (vm.Runtime.PowerState == VirtualMachinePowerState.poweredOff)
                                {
                                    saida.Result = "ja off";
                                }
                                else
                                {                                    
                                    vm.PowerOffVM_Task();
                                    saida.Result = "poweredOff";
                                }

                                SendString(context, JsonConvert.SerializeObject(saida));

                            }
                            catch (Exception ex)
                            {
                                this.telaLoad.TelaInicial.WriteConsole("Client(" + IP_Client + ")(Comando:" + data.cmd + ") ERRO: " + ex.Message);
                                saida.Result = "erro";
                                SendString(context, JsonConvert.SerializeObject(saida));
                            }
                            break;
                        }
                    case "suspendVM":
                        {
                            vmwareConnect vmware = this.telaLoad.vmware;
                            VimClient vimClient = this.telaLoad.clientTasks;

                            Debug.WriteLine("(suspendVM) " + Omini.Base64Decode(data.otherData) + data.sPPk + data.cmd);
                            try
                            {
                                OminiHost_Server.Data.powerOnOffSuspend vmForChange = JsonConvert.DeserializeObject<OminiHost_Server.Data.powerOnOffSuspend>(Omini.Base64Decode(data.otherData));
                                VirtualMachine vm = vmware.getMachine(vimClient, vmForChange.vmip);
                                if (vm.Runtime.PowerState == VirtualMachinePowerState.suspended)
                                {
                                    saida.Result = "ja suspenso";
                                }
                                else
                                {                                  
                                    vm.SuspendVM_Task();
                                    saida.Result = "suspended";
                                }

                                SendString(context, JsonConvert.SerializeObject(saida));

                            }
                            catch (Exception ex)
                            {
                                this.telaLoad.TelaInicial.WriteConsole("Client(" + IP_Client + ")(Comando:" + data.cmd + ") ERRO: " + ex.Message);
                                saida.Result = "erro";
                                SendString(context, JsonConvert.SerializeObject(saida));
                            }
                            break;
                        }
                    case "restartVM":
                        {
                            vmwareConnect vmware = this.telaLoad.vmware;
                            VimClient vimClient = this.telaLoad.clientTasks;
                            Debug.WriteLine("(restartVM) " + Omini.Base64Decode(data.otherData) + data.sPPk + data.cmd);
                            try
                            {
                                OminiHost_Server.Data.powerOnOffSuspend vmForChange = JsonConvert.DeserializeObject<OminiHost_Server.Data.powerOnOffSuspend>(Omini.Base64Decode(data.otherData));
                                VirtualMachine vm = vmware.getMachine(vimClient, vmForChange.vmip);
                              
                                vm.ResetVM_Task();
                                saida.Result = "restarting";

                                SendString(context, JsonConvert.SerializeObject(saida));

                            }
                            catch (Exception ex)
                            {
                                this.telaLoad.TelaInicial.WriteConsole("Client(" + IP_Client + ")(Comando:" + data.cmd + ") ERRO: " + ex.Message);
                                saida.Result = "erro";
                                SendString(context, JsonConvert.SerializeObject(saida));
                            }
                            break;
                        }
                    case "guestRestartVM":
                        {
                            vmwareConnect vmware = this.telaLoad.vmware;
                            VimClient vimClient = this.telaLoad.clientTasks;
                            Debug.WriteLine("(guestRestartVM) " + Omini.Base64Decode(data.otherData) + data.sPPk + data.cmd);
                            try
                            {
                                OminiHost_Server.Data.powerOnOffSuspend vmForChange = JsonConvert.DeserializeObject<OminiHost_Server.Data.powerOnOffSuspend>(Omini.Base64Decode(data.otherData));
                                VirtualMachine vm = vmware.getMachine(vimClient, vmForChange.vmip);
                                
                                vm.RebootGuest();
                                saida.Result = "restarting";

                                SendString(context, JsonConvert.SerializeObject(saida));

                            }
                            catch (Exception ex)
                            {
                                this.telaLoad.TelaInicial.WriteConsole("Client(" + IP_Client + ")(Comando:" + data.cmd + ") ERRO: " + ex.Message);
                                saida.Result = "erro";
                                SendString(context, JsonConvert.SerializeObject(saida));
                            }
                            break;
                        }
                    case "guestShutdownVM":
                        {
                            vmwareConnect vmware = this.telaLoad.vmware;
                            VimClient vimClient = this.telaLoad.clientTasks;
                            Debug.WriteLine("(guestShutdownVM) " + Omini.Base64Decode(data.otherData) + data.sPPk + data.cmd);
                            try
                            {
                                OminiHost_Server.Data.powerOnOffSuspend vmForChange = JsonConvert.DeserializeObject<OminiHost_Server.Data.powerOnOffSuspend>(Omini.Base64Decode(data.otherData));
                                VirtualMachine vm = vmware.getMachine(vimClient, vmForChange.vmip);
                                if (vm.Runtime.PowerState == VirtualMachinePowerState.poweredOff)
                                {
                                    saida.Result = "ja off";
                                }
                                else
                                {
                                    vm.PowerOffVM_Task();
                                    saida.Result = "desligando";
                                }
                                SendString(context, JsonConvert.SerializeObject(saida));

                            }
                            catch (Exception ex)
                            {
                                this.telaLoad.TelaInicial.WriteConsole("Client(" + IP_Client + ")(Comando:" + data.cmd + ") ERRO: " + ex.Message);
                                saida.Result = "erro";
                                SendString(context, JsonConvert.SerializeObject(saida));
                            }
                            break;
                        }
                    case "deleteVM":
                        {
                            vmwareConnect vmware = this.telaLoad.vmware;
                            VimClient vimClient = this.telaLoad.clientTasks;
                            Debug.WriteLine("(deleteVM) " + Omini.Base64Decode(data.otherData) + data.sPPk + data.cmd);
                            try
                            {
                                OminiHost_Server.Data.powerOnOffSuspend vmForChange = JsonConvert.DeserializeObject<OminiHost_Server.Data.powerOnOffSuspend>(Omini.Base64Decode(data.otherData));
                                VirtualMachine vm = vmware.getMachine(vimClient, vmForChange.vmip);
                                if (vm == null)
                                {
                                    saida.Result = "vm not found";
                                }
                                else
                                {                                
                                    vm.Destroy_Task();
                                    saida.Result = "deletado";
                                }
                                SendString(context, JsonConvert.SerializeObject(saida));
                            }
                            catch (Exception ex)
                            {
                                this.telaLoad.TelaInicial.WriteConsole("Client(" + IP_Client + ")(Comando:" + data.cmd + ") ERRO: " + ex.Message);
                                saida.Result = "erro";
                                SendString(context, JsonConvert.SerializeObject(saida));
                            }
                            break;
                        }
                    case "statePowerVM":
                        {
                            vmwareConnect vmware = this.telaLoad.vmware;
                            VimClient vimClient = this.telaLoad.clientTasks;
                            Debug.WriteLine("(statePowerVM) " + Omini.Base64Decode(data.otherData) + data.sPPk + data.cmd);
                            try
                            {
                                OminiHost_Server.Data.powerOnOffSuspend vmForChange = JsonConvert.DeserializeObject<OminiHost_Server.Data.powerOnOffSuspend>(Omini.Base64Decode(data.otherData));
                                VirtualMachine vm = vmware.getMachine(vimClient, vmForChange.vmip);
                                if (vm == null)
                                {
                                    saida.Result = "vm not found";
                                }
                                else
                                {
                                    switch (vm.Runtime.PowerState)
                                    {
                                        case VirtualMachinePowerState.poweredOff: saida.Result = "poweredOff"; break;
                                        case VirtualMachinePowerState.poweredOn: saida.Result = "poweredOn"; break;
                                        case VirtualMachinePowerState.suspended: saida.Result = "suspenso"; break;
                                        default: saida.Result = "what?"; break;
                                    }

                                }
                                SendString(context, JsonConvert.SerializeObject(saida));
                            }
                            catch (Exception ex)
                            {
                                this.telaLoad.TelaInicial.WriteConsole("Client(" + IP_Client + ")(Comando:" + data.cmd + ") ERRO: " + ex.Message);
                                saida.Result = "erro";
                                SendString(context, JsonConvert.SerializeObject(saida));
                            }
                            break;
                        }
                    case "getVMInfo":
                        {
                            vmwareConnect vmware = this.telaLoad.vmware;
                            VimClient vimClient = this.telaLoad.clientTasks;
                            Debug.WriteLine("(getVMInfo) " + Omini.Base64Decode(data.otherData) + data.sPPk + data.cmd);
                            try
                            {
                                OminiHost_Server.Data.vmGetInfos vmForGet = JsonConvert.DeserializeObject<OminiHost_Server.Data.vmGetInfos>( Omini.Base64Decode(data.otherData) );
                                VirtualMachine vm = vmware.getMachine(vimClient, vmForGet.vmip);
                                if (vm == null)
                                {
                                    saida.Result = "vm not found";
                                }
                                else
                                {                                    
                                    Dictionary<string, object> respostas = new Dictionary<string, object>();
                                    foreach (string item in vmForGet.infos)
                                    {
                                        switch (item)
                                        {
                                            case "NumeroCPU":
                                                respostas.Add("NumeroCPU", vm.Summary.Config.NumCpu);
                                                break;

                                            case "MaxMemoria":
                                                respostas.Add("MaxMemoria", vm.Summary.Runtime.MaxMemoryUsage);
                                                break;

                                            case "MaxCPU":
                                                respostas.Add("MaxCPU", vm.Config.CpuAllocation.Limit);
                                                break;

                                            case "BootTime":
                                                respostas.Add("BootTime", vm.Summary.Runtime.BootTime);
                                                break;

                                            case "MemoriaEmUso":
                                                respostas.Add("MemoriaEmUso", vm.Summary.QuickStats.GuestMemoryUsage);
                                                break;
                                            case "CPUEmUso":
                                                respostas.Add("CPUEmUso", vm.Summary.QuickStats.OverallCpuUsage);
                                                break;
                                            case "HD":
                                                long espacoTotal = 0;

                                                List<VirtualDevice> hds = vmware.getAllDiskVM(vimClient, vm);
                                                foreach (VirtualDevice hd in hds)
                                                {
                                                    espacoTotal += ((VirtualDisk)hd).CapacityInKB;
                                                }
                                                espacoTotal -= 2000000;
                                                decimal kB = (decimal)Math.Pow(10, -3);
                                                decimal espcoInB = vm.Summary.Storage.Uncommitted;
                                                int espacoLivreKB = (int)decimal.Multiply(espcoInB, kB);
                                                respostas.Add("HD", new OminiHost_Server.Data.HDInfo { Capacidade = espacoTotal, EspacoLivre = espacoLivreKB });
                                                break;
                                        }
                                    }
                                    saida.Result = JsonConvert.SerializeObject(respostas);

                                }

                                SendString(context, JsonConvert.SerializeObject(saida));
                            }
                            catch (Exception ex)
                            {
                                this.telaLoad.TelaInicial.WriteConsole("Client(" + IP_Client + ")(Comando:" + data.cmd + ") ERRO: " + ex.Message);
                                saida.Result = "erro";
                                SendString(context, JsonConvert.SerializeObject(saida));
                            }
                            break;
                        }
                    case "getTarefaStatus":
                        {
                            OminiHost_Server.Data.powerOnOffSuspend phpInfo = JsonConvert.DeserializeObject<OminiHost_Server.Data.powerOnOffSuspend>(Omini.Base64Decode(data.otherData));
                            Debug.WriteLine("(getTarefaStatus) " + Omini.Base64Decode(data.otherData) + data.sPPk + data.cmd);
                            if (this.telaLoad.ip_tarefas.ContainsKey(phpInfo.vmip))
                            {
                                OminiHost_Server.Data.infoTarefa resposta = new OminiHost_Server.Data.infoTarefa();
                                resposta.statusAtual = this.telaLoad.ip_tarefas[phpInfo.vmip].estadoAtual;
                                resposta.msg = this.telaLoad.ip_tarefas[phpInfo.vmip].msg;
                                resposta.tipoTarefa = this.telaLoad.ip_tarefas[phpInfo.vmip].getTarefaType();
                                resposta.progresso = this.telaLoad.ip_tarefas[phpInfo.vmip].progresso;
                                saida.Result = JsonConvert.SerializeObject(resposta);
                            }
                            else
                            {
                                saida.Result = "sem tarefa";
                            }

                            SendString(context, JsonConvert.SerializeObject(saida));
                            break;
                        }
                    case "getVncProxy":
                        {
                            vmwareConnect vmware = this.telaLoad.vmware;
                            VimClient vimClient = this.telaLoad.clientTasks;
                            OminiHost_Server.Data.getVncProxy phpInfo = JsonConvert.DeserializeObject<OminiHost_Server.Data.getVncProxy>(Omini.Base64Decode(data.otherData));

                            VirtualMachine vm = vmware.getMachine(vimClient, phpInfo.vmip);
                            if (vm == null)
                            {
                                saida.Result = "vm not found";
                            }
                            else
                            {                                
                                HostSystem host = vmware.getHost(vimClient, vm.Runtime.Host);
                                if(host == null)
                                {
                                    saida.Result = "host not found";
                                }
                                else
                                {
                                    Dictionary<string, string> valoresSaida = new Dictionary<string, string>();
                                    int portProxy = Global.GetFreePort();
                                    string randomPassword = Global.GenerateRandomCode(16);

                                    #region randomPortVNC                            
                                    string[] dedicadoIpSlipt = host.Name.Split('.');
                                    string[] ipVmSplit = phpInfo.vmip.Split('.');

                                    int dedicadoFim = int.Parse(dedicadoIpSlipt[3]);
                                    int vmFim = int.Parse(ipVmSplit[3]);
                                    Random rd = new Random();
                                    int randomNumber = rd.Next(1000, 7000);

                                    int portVnc = randomNumber + dedicadoFim + (vmFim - 2);
                                    #endregion

                                    VirtualMachineConfigSpec newSpec = new VirtualMachineConfigSpec();
                                    newSpec.ExtraConfig = new OptionValue[vm.Config.ExtraConfig.Length];
                                    newSpec.ExtraConfig = vm.Config.ExtraConfig;
                                    newSpec.ExtraConfig = vmware.addExtraConfig(newSpec.ExtraConfig, "RemoteDisplay.vnc.enabled", "true");
                                    newSpec.ExtraConfig = vmware.addExtraConfig(newSpec.ExtraConfig, "RemoteDisplay.vnc.port", portVnc);
                                    newSpec.ExtraConfig = vmware.addExtraConfig(newSpec.ExtraConfig, "RemoteDisplay.vnc.password", randomPassword);
                                    // é necessário a VM estar desligada para que configuração da porta funcione!
                                    // Do contrário será como se a config nunca tivesse sido efetuada
                                    //newSpec.ExtraConfig = vmware.addExtraConfig(newSpec.ExtraConfig, "vnc_disable_password", "true");
                                    vm.ReconfigVM(newSpec);

                                    Global.proxies.Add(new VncProxy(portProxy, host.Name, portVnc));
                                    telaLoad.TelaInicial.Write("[ Proxy ] Iniciado porta: " + portProxy + " para " + host.Name + ":" + portVnc + " " + randomPassword);

                                    valoresSaida.Add("host", Global.proxyHostIp);
                                    valoresSaida.Add("port", portProxy.ToString());
                                    valoresSaida.Add("password", randomPassword);
                                    saida.Result = JsonConvert.SerializeObject(valoresSaida);
                                }
                            }
                            SendString(context, JsonConvert.SerializeObject(saida));
                            break;
                        }
                    case "setVpsVncConfig":
                        {
                            vmwareConnect vmware = this.telaLoad.vmware;
                            VimClient vimClient = this.telaLoad.clientTasks;
                            OminiHost_Server.Data.setVpsVncConfig phpInfo = JsonConvert.DeserializeObject<OminiHost_Server.Data.setVpsVncConfig>(Omini.Base64Decode(data.otherData));

                            VirtualMachine vm = vmware.getMachine(vimClient, phpInfo.vmip);
                            if (vm == null)
                            {
                                saida.Result = "vm not found";
                            }
                            else
                            {
                                try
                                {
                                    VirtualMachineConfigSpec newSpec = new VirtualMachineConfigSpec();
                                    newSpec.ExtraConfig = new OptionValue[vm.Config.ExtraConfig.Length];
                                    newSpec.ExtraConfig = vm.Config.ExtraConfig;                                    
                                    newSpec.ExtraConfig = vmware.addExtraConfig(newSpec.ExtraConfig, "RemoteDisplay.vnc.enabled", "true");
                                    newSpec.ExtraConfig = vmware.addExtraConfig(newSpec.ExtraConfig, "RemoteDisplay.vnc.port", phpInfo.port);
                                    newSpec.ExtraConfig = vmware.addExtraConfig(newSpec.ExtraConfig, "RemoteDisplay.vnc.password", phpInfo.password);
                                    newSpec.ExtraConfig = vmware.addExtraConfig(newSpec.ExtraConfig, "vnc_disable_password", "true");
                                    Debug.WriteLine("SetVNCConfig");
                                    vm.ReconfigVM(newSpec);
                                    saida.Result = "ok";
                                }
                                catch(Exception ex)
                                {
                                    saida.Result = "error";
                                    this.telaLoad.TelaInicial.WriteConsole("setVpsVncConfig error " + ex.Message + ex.StackTrace + ex.Data);
                                }                                
                            }
                            SendString(context, JsonConvert.SerializeObject(saida));
                            break;
                        }
                    case "getHostsInDatacenter":
                        {
                            vmwareConnect vmware = this.telaLoad.vmware;
                            VimClient vimClient = this.telaLoad.clientTasks;
                            OminiHost_Server.Data.hostsInDC phpInfo = JsonConvert.DeserializeObject<OminiHost_Server.Data.hostsInDC>( Omini.Base64Decode(data.otherData) );
                            Dictionary<string, string>[] valoresSaida = vmware.getHostsInDataCenter(vimClient, phpInfo.DCName).ToArray();
                            Debug.WriteLine("(getHostsInDatacenter) " + Omini.Base64Decode(data.otherData) + data.sPPk + data.cmd);
                            if (valoresSaida == null)
                            {
                                saida.Result = "dc or host null";
                            }
                            else
                            {
                                saida.Result = JsonConvert.SerializeObject(valoresSaida);
                            }
                            System.Diagnostics.Debug.WriteLine("(getHostsInDatacenter) = " + JsonConvert.SerializeObject(saida));
                            SendString(context, JsonConvert.SerializeObject(saida));
                            break;
                        }
                    default:
                        {
                            saida.hasError = true;
                            saida.Error = "Invalid CMD";
                            SendString(context, JsonConvert.SerializeObject(saida));
                            break;
                        }

                }               
            }
            catch(Exception err)
            {
                this.telaLoad.TelaInicial.WriteConsole(IP_Client + " enviou um pacote inválido! Dados do pacote: " + body + "ERRO "+err.Message);
                SendString(context, "Invalid data");
            }                       
        }
        public void SendString(HttpListenerContext context,string response)
        {
            byte[] b = Encoding.UTF8.GetBytes(response);
            context.Response.StatusCode = 200;
            context.Response.KeepAlive = false;
            context.Response.ContentLength64 = b.Length;

            var output = context.Response.OutputStream;
            output.Write(b, 0, b.Length);
            context.Response.Close();
        }
    }
}
