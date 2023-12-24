using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using VMware.Vim;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace OminiHost_Server.Class
{
    public class Tarefa
    {
        public enum Tipos {
            criacao,
            formatacao
        };
        public enum Erros_Criacao
        {            
            DedicadoNaoExiste,
            TemplateNaoEncontrado,
            DedicadoNaoTemHosts,
            ProblemasNoTemplate,
            Sucesso
        }
        public enum Erros_Formatacao
        {
            Sucesso,
            VMNaoEncontrada
        }
        public Tipos tipo;
        public string TimeCriacao;
        public string estadoAtual;
        public int? progresso;
        public string msg = "Iniciando";

        public HostSystem host;        
        public ManagedObjectReference taskMoref;

        public TelaLoad processo;        

        private VimClient client;
        private vmwareConnect vmware;
        private Data.VMEspec espec;
        private Inicio tela_inicio;

        public Tarefa(TelaLoad p,Data.VMEspec spc)
        {            
            this.espec = spc;                                   
            this.processo = p;
            Omini.debug("Nova tarefa criada");
            this.TimeCriacao = DateTime.Now.ToString();
            this.client = p.clientTasks;
            this.vmware = p.vmware;
            this.tela_inicio = this.processo.TelaInicial;
        }
        private void CancelarTerafa(string menssage)
        {
            Omini.debug("Tarefa cancelada, msg "+menssage);
            this.estadoAtual = "cancelada";
            this.msg = menssage;
        }       
        public string getTarefaType()
        {
            switch (this.tipo) {
                case Tipos.criacao: return "Criação";
                case Tipos.formatacao: return "Formatação";
                default: return "desconhecido";
            }
        }
        public void setTipoTarefa(Tipos ti)
        {
            this.tipo = ti;
            this.estadoAtual = "em andamento";
            this.msg = "Iniciando";
        }        
        public Erros_Criacao clonarTemplate(string nameDC = "nada")
        {             
            // espec.dedicado na verdade quer dizer o nome do datacenter!! Sim, sou idiota.
            Datacenter datacenter = vmware.findDatacerterByName(this.client, espec.dedicado);            
            if (datacenter != null)
            {
                HostSystem host = null;
                if (nameDC == "nada")
                {
                    if(espec.nameDC == "nada")
                        host = vmware.getHostAnyLessInDatacenter(this.client, datacenter);
                    else
                        host = vmware.findDedicadoByName(this.client, espec.nameDC);
                    Debug.WriteLine(espec.nameDC);
                }                
                else
                {
                    host = vmware.findDedicadoByName(this.client, nameDC);
                }
                if (host != null)
                {
                    this.host = host;
                    Datastore datastore = vmware.getDatastoreAnyLessInHost(this.client, host, this.espec.HDType);

                    if (datastore == null)
                    {
                        tela_inicio.WriteConsole("Maquina(" + this.espec.ipVM + ") tarefa(" + getTarefaType() + ") ERRO: dedicado " + this.espec.dedicado + " não possui nenhum HD tipo: " + this.espec.HDType + " com mais de 30GB livre");
                        CancelarTerafa("Dedicado não possui hosts ESXI com espaço livre no HD");
                        Omini.debug("Dedicado não possui hosts ESXI com espaço livre no HD!");
                        return Erros_Criacao.DedicadoNaoTemHosts;
                    }

                    string ipv6 = espec.IPv6;
                    string ipv4 = espec.IPv4;
                    string sistemaOperacional = espec.sistemaOperacional;
                    string protocolv = "";
                    if (sistemaOperacional == "windows")
                    {
                        if (ipv6 != "null" && ipv4 != "null")
                        {
                            protocolv = "";
                        }

                        if (ipv6 == "null" && ipv4 != "null")
                        {
                            protocolv = "-ipv4";
                        }

                        if (ipv6 != "null" && ipv4 == "null")
                        {
                            protocolv = "";
                        }
                    }
                        

                    // Tentar pegar template pelo ip dedicado (host name)
                    string nameOfTempalte = espec.templateSystemName + protocolv + " " +host.Name;                    
                    VirtualMachine template = vmware.findTemplateInDatacenter(this.client, datacenter, nameOfTempalte);

                    if(template == null)
                    {                        
                        // Tentar pegar o template pelo nome do datacenter 
                        nameOfTempalte = espec.templateSystemName + protocolv + " " + espec.dedicado;
                        template = vmware.findTemplateInDatacenter(this.client, datacenter, nameOfTempalte);
                    }

                    if (template != null)
                    {
                        VMware.Vim.Task tf = vmware.CloneVM(this.client, template, datastore, host, datacenter.VmFolder, this.espec);
                        if (tf == null)
                        {                            
                            CancelarTerafa("problemas no template");
                            this.tela_inicio.WriteConsole("Falha ao clonar, verifique o hardware de rede"+Environment.NewLine+ "Eles precisa ser (VirtualE1000)");
                            return Erros_Criacao.ProblemasNoTemplate;
                        }
                        else
                        {
                            this.taskMoref = tf.MoRef;
                            Thread th = new Thread(new ThreadStart(this.start));
                            th.Start();
                            this.msg = "clonando";
                            Omini.debug("tarefa de clone iniciada");
                            return Erros_Criacao.Sucesso;
                        }
                    }
                    else
                    {
                        tela_inicio.WriteConsole("Maquina("+this.espec.ipVM+") tarefa("+ getTarefaType()+ ") ERRO: template nao encontrado");                        
                        CancelarTerafa("Template não encontrado");
                        Omini.debug("Falha de tarefa nao encontrada!");
                        return Erros_Criacao.TemplateNaoEncontrado;
                    }
                }
                else
                {
                    tela_inicio.WriteConsole("Maquina(" + this.espec.ipVM + ") tarefa(" + getTarefaType() + ") ERRO: dedicado "+this.espec.dedicado+" não possui Hosts ESXI");                    
                    CancelarTerafa("Dedicado não possui hosts ESXI");
                    Omini.debug("Erro pois o dedico nao possui hosts!");
                    return Erros_Criacao.DedicadoNaoTemHosts;
                }

            }
            else
            {
                tela_inicio.WriteConsole("Maquina(" + this.espec.ipVM + ") tarefa(" + getTarefaType() + ") ERRO: dedicado "+this.espec.dedicado+" não encontrado");                
                CancelarTerafa("Dedico não encontrado");
                Omini.debug("Dedicado nao encontradp");
                return Erros_Criacao.DedicadoNaoExiste;
            }            
        } 
        public Erros_Formatacao deletarVM(string vmname)
        {
            NameValueCollection filtro = new NameValueCollection();
            filtro.Add("name", vmname);

            List<VirtualMachine> vmList = this.vmware.getEntities<VirtualMachine>(this.client, null, filtro);
            foreach(VirtualMachine vm in vmList)
            {
                if (vm == null)
                {
                    CancelarTerafa("Maquina não encontrada");
                    Omini.debug("VM que ia ser deletada nao econtrada");
                    return Erros_Formatacao.VMNaoEncontrada;
                }

                Regex regexipv4 = new Regex(@"\d{1,3}(\.\d{1,3}){3}");
                //Regex regexipv6 = new Regex(@"\d([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}");
                Regex regexipv6 = new Regex(@"\d(([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:((:[0-9a-fA-F]{1,4}){1,6})|:((:[0-9a-fA-F]{1,4}){1,7}|:)|fe80:(:[0-9a-fA-F]{0,4}){0,4}%[0-9a-zA-Z]{1,}|::(ffff(:0{1,4}){0,1}:){0,1}((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])|([0-9a-fA-F]{1,4}:){1,4}:((25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9])\.){3,3}(25[0-5]|(2[0-4]|1{0,1}[0-9]){0,1}[0-9]))");

                Match matchipv6 = regexipv6.Match(vm.Name);
                Match matchipv4 = regexipv4.Match(vm.Name);

                bool matchresult = matchipv6.Success == true ? matchipv6.Success : matchipv4.Success;

                if (matchresult)
                {
                    string nameFind = matchipv6.Success == true ? matchipv6.Value.ToLower() : matchipv4.Value.ToLower();
                    if (!nameFind.Equals(vmname.ToLower()))
                    {
                        continue;
                    }
                }
                else
                    continue;

                if (vm.Runtime.PowerState != VirtualMachinePowerState.poweredOff)
                {
                    vm.PowerOffVM();
                    Thread.Sleep(1000);
                }
                this.msg = "deletando";
                Omini.debug("Tarefa vmware de delete criada!");
                ManagedObjectReference DeleteTaskMoRef = new ManagedObjectReference();

                DeleteTaskMoRef = vm.Destroy_Task();
                VMware.Vim.Task tf = new VMware.Vim.Task(this.client, DeleteTaskMoRef);

                this.taskMoref = tf.MoRef;
                Thread th = new Thread(new ThreadStart(this.startDeleteCheck));
                th.Start();

                return Erros_Formatacao.Sucesso;
            }
            return Erros_Formatacao.VMNaoEncontrada;
        }
        ///configurar IP em sistemas linux
        public void ConfigurarIP(VMware.Vim.Task tf)
        {
            Omini.debug("Funcao de configuracao iniciada");
            //VirtualDevice disk = null;
            try
            {
                Omini.debug("Pegando o disk da vm");
                //disk = this.vmware.getDeviceDiskVM(this.client, (ManagedObjectReference)tf.Info.Result);
            }
            catch (Exception Ex)
            {
                Omini.debug("Falha ao tentar pegar o local do disco da VM: "+Ex.Message);
            }
            Omini.debug("Disco pego, decodificando a senha");
            //string senha = Omini.Base64Decode(vmware.getCustomFieldInHost(this.host, espec.customField)); Omini.debug("Senha pega");
            string ipDedicado = this.host.Name; Omini.debug("IP do dedicado "+ipDedicado);
            string ipMaquina = espec.ipVM; Omini.debug("IP da VM "+ipMaquina);
            string novaSenha = espec.novaSenha; Omini.debug("Nova senha "+novaSenha);
            string ipv6 = espec.IPv6;
            string IPv6Gateway = espec.IPv6Gateway;
            int IPv6Mask = espec.IPv6Mask;
            string IPv6DNS1 = espec.IPv6DNS1;
            string IPv6DNS2 = espec.IPv6DNS2;
            string IPv6DNS3 = espec.IPv6DNS3;
            string IPv6DNS4 = espec.IPv6DNS4;
            string ipv4 = espec.IPv4;
            string IPv4Gateway = espec.IPv4Gateway;
            string IPv4Mask = espec.IPv4Mask;
            string IPv4DNS1 = espec.IPv4DNS1;
            string IPv4DNS2 = espec.IPv4DNS2;
            string IPv4DNS3 = espec.IPv4DNS3;
            string IPv4DNS4 = espec.IPv4DNS4;
            string gateway = host.Config.Network.IpRouteConfig.DefaultGateway; Omini.debug("Gateway "+gateway);
            
            try
            {
                /*Omini.debug("Abrindo programa de configura de ip linux!");
                ProcessStartInfo programInfo = new ProcessStartInfo();
                //programInfo.FileName = @"vmrun\" + espec.setIpFileName;
                programInfo.FileName = @"vmrun\vmrun";
                programInfo.Arguments = "-T ominihost1 -h https://" + ipDedicado + "/sdk -u root -p " + senha + " -gu root -gp omini123* runProgramInGuest " + '"' + ((VirtualDiskFlatVer2BackingInfo)disk.Backing).FileName.Replace("vmdk", "vmx") + '"' + " /usr/bin/ipconf " + ipMaquina + " " + gateway + " " + novaSenha;                
                programInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Omini.debug("Arquivo de configuracao: " + programInfo.FileName);
                Omini.debug("Argumentos de configuracao: "+programInfo.Arguments);
                Process infoprocess = Process.Start(programInfo);
                Omini.debug("Programa iniciado");*/
                
                VirtualMachine vm = vmware.getMachine(this.client, ipMaquina);                
                if(vm != null)
                {

                    //long processPid = vmware.startProcessInVm(this.client, vm, "root", "omini123*", "/usr/bin/ipconf", $"{ipMaquina} {gateway} {novaSenha}");

                    long processPid = default;

                    if (ipv6 != "null" && ipv4 != "null")
                    {
                        processPid = vmware.startProcessInVm(this.client, vm, "root", "omini123*", "/usr/bin/ip46conf", $"{ipv4} {IPv4Mask} {IPv4Gateway} {IPv4DNS1} {IPv4DNS2} {IPv4DNS3} {IPv4DNS4} {ipv6} {IPv6Mask} {IPv6Gateway} {IPv6DNS1} {IPv6DNS2} {IPv6DNS3} {IPv6DNS4} {novaSenha}");
                    }

                    if (ipv6 == "null" && ipv4 != "null")
                    {
                        processPid = vmware.startProcessInVm(this.client, vm, "root", "omini123*", "/usr/bin/ip4conf", $"{ipv4} {IPv4Mask} {IPv4Gateway} {IPv4DNS1} {IPv4DNS2} {IPv4DNS3} {IPv4DNS4} {novaSenha}");
                    }

                    if (ipv6 != "null" && ipv4 == "null")
                    {
                        processPid = vmware.startProcessInVm(this.client, vm, "root", "omini123*", "/usr/bin/ip6conf", $"{ipv6} {IPv6Mask} {IPv6Gateway} {IPv6DNS1} {IPv6DNS2} {IPv6DNS3} {IPv6DNS4} {novaSenha}");
                    }
                    //this.tela_inicio.WriteConsole("Configurando ip em Linux(" + processPid + ")");
                    //this.tela_inicio.WriteConsole("" +
                    //    "Configurando ip em Linux(" + 
                    //    this.espec.IPv4 + " " +
                    //    this.espec.IPv4Mask + " " +
                    //    this.espec.IPv4Gateway + " " +
                    //    this.espec.IPv4DNS1 + " " +
                    //    this.espec.IPv4DNS2 + " " +
                    //    this.espec.IPv4DNS3 + " " +
                    //    this.espec.IPv6 + " " +
                    //    this.espec.IPv6Mask + " " +
                    //    this.espec.IPv6Gateway + " " +
                    //    this.espec.IPv6DNS1 + " " +
                    //    this.espec.IPv6DNS2 + " " +
                    //    this.espec.IPv6DNS3 + " " +
                    //    this.espec.novaSenha + " " +
                    //    ")");
                    if (processPid != default)
                    {
                        this.tela_inicio.WriteConsole("Configurando ip em Linux(" + this.espec.templateSystemName + ")");
                        this.msg = "configurando ip";
                    }
                    else
                    {
                        this.tela_inicio.WriteConsole("IP não configurado, não foi possível iniciar o processo na VM");
                    }

                }
                else
                {
                    this.tela_inicio.WriteConsole("VM não encontrada, não foi possível configurar.");
                }
            }
            catch (Exception e)
            {                             
                CancelarTerafa(e.Message);
                this.tela_inicio.WriteConsole("Erro configurar tentar configurar IP. Maquina IP ("+espec.ipVM+") "+Environment.NewLine+"ERRO: "+e.Message);
            }
        }
        public void startDeleteCheck()
        {
            while (true)
            {
                VMware.Vim.Task tf = vmware.getObject<VMware.Vim.Task>(this.client, this.taskMoref);
                switch (tf.Info.State)
                {
                    case TaskInfoState.running:                        
                        break;
                    case TaskInfoState.success:
                        this.clonarTemplate();
                        return;
                    //Tarefa terminada                        
                    case TaskInfoState.error:                        
                        CancelarTerafa(tf.Info.Description.Message);
                        tela_inicio.WriteConsole("Maquina(" + this.espec.ipVM + ") tarefa(" + getTarefaType() + ") estado da tarefa(" + this.estadoAtual + ") ERRO: " + tf.Info.Description.Message+Environment.NewLine+"A tarefa em andamento no vcenter era de deletar");
                        //erro na tarefa
                        return;
                }
                Thread.Sleep(3000);
            }
        }
        public void start()
        {                                                  
            while (true)
            {               
                VMware.Vim.Task tf = vmware.getObject<VMware.Vim.Task>(this.client, this.taskMoref);
                switch (tf.Info.State)
                {
                    case TaskInfoState.running:
                        this.progresso = tf.Info.Progress;
                        break;
                    case TaskInfoState.success:
                        Omini.debug("Tarefa terminada, s.o =  "+this.espec.sistemaOperacional);
                        if (this.tipo == Tipos.criacao || this.tipo == Tipos.formatacao)
                        {
                            if (this.espec.sistemaOperacional.Equals("linux") == true)
                            {
                                Omini.debug("Esperando pra configurar o ip");
                                this.msg = "aguardando para configurar o ip";
                                Thread.Sleep(30000);
                                Omini.debug("Tempo acabou iniciando configuracao");
                                ConfigurarIP(tf);
                                this.progresso = 96;
                                Thread.Sleep(18750);
                                this.progresso = 98;
                                Thread.Sleep(37500);
                                this.progresso = 99;
                                Thread.Sleep(18750);
                                this.progresso = 100;
                            }
                            else
                            {
                                Thread.Sleep(30000);
                                Omini.debug("Configurando IP em Windows");
                                this.msg = "aguardando para configurar o ip";
                                this.tela_inicio.WriteConsole("Configurando IP em Windows " + espec.ipVM);
                                this.progresso = 94;
                                Thread.Sleep(50000);
                                this.progresso = 95;
                                Thread.Sleep(50000);
                                this.progresso = 96;
                                Thread.Sleep(50000);
                                this.progresso = 97;
                                Thread.Sleep(50000);
                                this.progresso = 98;
                                Thread.Sleep(50000);
                                this.progresso = 99;
                                Thread.Sleep(50000);
                                this.progresso = 100;
                            }
                            Omini.debug("Esperando pra marcar como terminado");
                            Thread.Sleep(30000);
                            this.estadoAtual = "terminada";
                            this.msg = "concluido";
                            Thread.Sleep(30000);
                            this.msg = "";
                            this.tela_inicio.WriteConsole("Tarefa " + getTarefaType() + " da maquina " + espec.ipVM + " foi termanada" + Environment.NewLine);

                        }
                        return;
                    //Tarefa terminada                        
                    case TaskInfoState.error:                                                
                        CancelarTerafa(tf.Info.Description.Message);
                        tela_inicio.WriteConsole("Maquina(" + this.espec.ipVM + ") tarefa(" + getTarefaType() + ") estado da tarefa("+this.estadoAtual+") ERRO: "+tf.Info.Description.Message);
                        //erro na tarefa
                        return;
                }              
                Thread.Sleep(3000);
            }
        }
    }
}
