using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using VMware.Vim;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace OminiHost_Server
{
    public partial class Inicio : Form
    {
        private readonly bool onTesting = false;
        
        delegate void SetTextCallback(string text);
        delegate void SetAddProxiesCount();
        Http.main servidorHTTP = null;

        TelaLoad Form1Program = null;
        public Inicio(TelaLoad criadoPor)
        {
            Form1Program = criadoPor;
            InitializeComponent();
            /*XJOmini.debug("Criando objeto servidor!");
            Class.servidor servidor = new Class.servidor(this.Form1Program, Form1Program.config.socket_ip, Form1Program.config.socket_port, Form1Program.config.socket_pass);
            */attNameServer();

            if (onTesting)
                this.menuStrip1.BackColor = System.Drawing.Color.Red;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            //form_Form1.Close();
            Global.proxies.ForEach(proxy =>
            {
                proxy.Kill();
            });

            Environment.Exit(0);
            //System.Threading.Thread.CurrentThread.Abort();
        }
        public void MaisAcao()
        {
            /*if (this.Form1Program.InvokeRequired)
            {
                SetCallback d = new SetCallback(MaisAcao);
                this.Invoke(d);
            }
            else
            {
                this.Form1Program.acoesEfetuadas++;
                this.acoes.Text = "Ações efetuadas " + this.Form1Program.acoesEfetuadas;

            }*/
        }
        public void proxiesCountUpdate()
        {
            if (this.consolePanel.InvokeRequired)
            {
                SetAddProxiesCount d = new SetAddProxiesCount(proxiesCountUpdate);
                this.Invoke(d, new object[] { });
            }
            else
            {
                this.proxiesCount.Text = "Proxies abertos " + Global.proxies.Count;
            }
        }
        public void WriteConsole(string texto)
        {
            if (this.consolePanel.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(WriteConsole);
                this.Invoke(d, new object[] { texto });
            }
            else
            {
                this.Write("[" + DateTime.Now + "] " + texto);
            }            
        }
        public void Write(string texto)
        {
            if (this.consolePanel.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(Write);
                this.Invoke(d, new object[] { texto });
            }
            else
            {
                var lines = this.consolePanel.Lines;

                if (lines.Length >= 100)
                {
                    var newLines = lines.Skip(1);
                    this.consolePanel.Lines = newLines.ToArray();
                }

                this.consolePanel.Text += texto + Environment.NewLine;
                this.consolePanel.Select(this.consolePanel.Text.Length, 0);
            }
        }
        private void inicio_Load(object sender, EventArgs e)
        {
            Thread updateThread = new Thread(new ThreadStart(upatePoxiesCount));
            updateThread.Start();

        }
        private void upatePoxiesCount()
        {
            while (true)
            {
                proxiesCountUpdate();
                Thread.Sleep(1000);
            }
        }

        private void encryptadorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Forms.encryptar().ShowDialog();
        }

        private void iPsPermitidosToolStripMenuItem_Click(object sender, EventArgs e)
        {
           new Forms.edit_ipsPermitidos(this.Form1Program).ShowDialog();
        }

        private void configuraçõesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Forms.edit_confs(this.Form1Program).ShowDialog();
        }

        private void reconnectarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WriteConsole("Reiniciando a conexão com o servidor vCenter");
            this.Form1Program.vmware.DisconnectServer(this.Form1Program.clientTasks);
            this.Form1Program.clientTasks = this.Form1Program.vmware.ConnectServer(this.Form1Program.config.vcenter_ip, new NetworkCredential(this.Form1Program.config.vcenter_user, this.Form1Program.config.vcenter_pass));
            if (this.Form1Program.clientTasks != null)
            {
                this.WriteConsole("Conexão efetuada com sucesso!" + Environment.NewLine);
                this.lb_vcenter.Text = this.Form1Program.clientTasks.ServiceContent.About.FullName;
            }
            else
                this.WriteConsole("Falha ao tentar reconectar" + Environment.NewLine);
                
        }

        private void limparConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.consolePanel.Text = "";
        }

        private void tarefasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Forms.viewTarefas(this.Form1Program).ShowDialog();
        }

        private void consolePanel_TextChanged(object sender, EventArgs e)
        {
            this.consolePanel.Select(this.consolePanel.Text.Length, 0);
            this.consolePanel.ScrollToCaret();
        }
        public void attNameServer()
        {
            this.servename.Text = this.Form1Program.config.name;
            this.servename.Location = new System.Drawing.Point((this.Width / 2) - (this.servename.Width / 2), this.servename.Location.Y);
        }        
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void startNewHTTPServer()
        {
            Http.main HTTP = new Http.main();
            this.servidorHTTP = HTTP;
            servidorHTTP.startServer(this.Form1Program);
        }

        private void Inicio_Shown(object sender, EventArgs e)
        {
            this.startNewHTTPServer();
        }

        private void reiniciarServidorHTTPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!servidorHTTP.Equals(null))
            {
                servidorHTTP.stopServer();
                this.Write("Desligando servidor HTTP");
            }
            this.startNewHTTPServer();
        }

        private void testesToolStripMenuItem_Click(object sender, EventArgs e)
        {

            vmwareConnect vmware = Form1Program.vmware;
            VimClient vimClient = Form1Program.clientTasks;

            /*GuestOperationsManager guestOperation = vmware.getObject<GuestOperationsManager>(vimClient, vimClient.ServiceContent.GuestOperationsManager);
            GuestProcessManager processManager = vmware.getObject<GuestProcessManager>(vimClient, guestOperation.ProcessManager);
            VirtualMachine vm = vmware.getMachine(vimClient, "198.50.237.86");
            if(vm != null)
            {
                //  c:\windows\system32\notepad.exe
                NamePasswordAuthentication auth = new NamePasswordAuthentication
                {
                    InteractiveSession = false,
                    Username = "Administrador",
                    Password = "eduardo123*",
                };

                GuestProgramSpec programSpec = new GuestProgramSpec
                {
                    ProgramPath = "c://Windows/System32/notepad.exe",
                    Arguments = ""
                };

                long pid = processManager.StartProgramInGuest(vm.MoRef, auth, programSpec);
                this.Write("Agora inicio com o pid: "+ pid);
            }
            else
            {
                this.Write("Vm não encontrada");
            }*/

            /*vmwareConnect vmware = Form1Program.vmware;
            VimClient vimClient = Form1Program.clientTasks;
            var teste = vmware.getHostsInDataCenter(vimClient, "OVH");

            this.Write("Total de dedicado no datacenter OVH: " + teste.Count);*/

            /* Regex regex = new Regex(@"\d{1,3}(\.\d{1,3}){3}");
             Match match = regex.Match("[NVME] 142.44.146.15 Micael (tjgamernas@outlook.com)");
             if (match.Success)
             {
                 string nameFind = match.Value;
                 this.Write("Nome encontrado: " + nameFind);
             }*/

            //Class.VncProxy teste = new Class.VncProxy(554 + this.cont);
            /* int port = Global.GetFreePort();
             Global.proxies.Add(new Class.VncProxy(port, "localhost", 50011));           
              this.Write(" port " + port);*/

            /*vmwareConnect vmware = Form1Program.vmware;
            VimClient vimClient = Form1Program.clientTasks;

            VirtualMachine vm = vmware.getMachine(vimClient, "158.69.3.121");
            if(vm != null)
            {
                VirtualMachineConfigSpec newSpec = new VirtualMachineConfigSpec();
                newSpec.ExtraConfig = new OptionValue[vm.Config.ExtraConfig.Length];
                newSpec.ExtraConfig = vm.Config.ExtraConfig;
                newSpec.ExtraConfig = vmware.addExtraConfig(newSpec.ExtraConfig, "RemoteDisplay.vnc.enabled", "false");
                newSpec.ExtraConfig = vmware.addExtraConfig(newSpec.ExtraConfig, "RemoteDisplay.vnc.port", 50013);
                newSpec.ExtraConfig = vmware.addExtraConfig(newSpec.ExtraConfig, "RemoteDisplay.vnc.password", "vamola");
                vm.ReconfigVM(newSpec);     
                //Debug.WriteLine(vmware.getVncPort(vm) + "< ");
            }*/

            VirtualMachine vm = vmware.getMachine(vimClient, "54.39.196.30");
            if (vm != null)
            {

                VirtualMachineTicket aaaaaa = vm.AcquireTicket("webmks");
                this.Write("host " + aaaaaa.Host);
                this.Write("port " + aaaaaa.Port.ToString());
                this.Write("cfg " + aaaaaa.CfgFile);
                this.Write("ssl " + aaaaaa.SslThumbprint);
                this.Write("ticket " + aaaaaa.Ticket);                
            }
        }
    }
}
