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

                    // Tentar pegar template pelo ip dedicado (host name)
                    string nameOfTempalte = espec.templateSystemName + " "+host.Name;                    
                    VirtualMachine template = vmware.findTemplateInDatacenter(this.client, datacenter, nameOfTempalte);
                    if(template == null)
                    {                        
                        // Tentar pegar o template pelo nome do datacenter 
                        nameOfTempalte = espec.templateSystemName + " " + espec.dedicado;
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

                Regex regex = new Regex(@"\d{1,3}(\.\d{1,3}){3}");
                Match match = regex.Match(vm.Name);
                if (match.Success)
                {
                    string nameFind = match.Value.ToLower();
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
                    long processPid = vmware.startProcessInVm(this.client, vm, "root", "omini123*", "/usr/bin/ipconf", $"{ipMaquina} {gateway} {novaSenha}");
                    if(processPid != default)
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
                                
                            }
                            else
                            {
                                Thread.Sleep(30000);
                                Omini.debug("Configurando IP em Windows");
                                this.msg = "aguardando para configurar o ip";
                                this.tela_inicio.WriteConsole("Configurando IP em Windows " + espec.ipVM);
                                this.progresso = 94;
                                Thread.Sleep(150000);
                                this.progresso = 95;
                                Thread.Sleep(150000);
                                this.progresso = 96;
                                Thread.Sleep(150000);
                                this.progresso = 97;
                                Thread.Sleep(150000);
                                this.progresso = 98;
                                Thread.Sleep(30000);
                                this.progresso = 99;
                                Thread.Sleep(30000);
                                this.progresso = 100;
                            }
                            Omini.debug("Esperando pra marcar como terminado");
                            Thread.Sleep(30000);
                            this.estadoAtual = "terminada";
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
