using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Net;
using System.Windows.Forms;
using System.IO;
using VMware.Vim;
using OminiHost_Server.Class;


namespace OminiHost_Server
{
    public partial class TelaLoad : Form
    {
        public Inicio TelaInicial;
        Thread timeVcenterUpdates;        

        System.Windows.Forms.Timer timeHide = new System.Windows.Forms.Timer();
        public vmwareConnect vmware = new vmwareConnect();
        public VimClient clientTasks;

        public Data.Config config;
        public List<string> IPS_PERMITIDOS = new List<string>();

        public Dictionary<string, Class.Tarefa> ip_tarefas = new Dictionary<string, Class.Tarefa>();

        public TelaLoad()
        {
            InitializeComponent();

            Global.telaLoad = this;

            Omini.debug("Aguardando 2s para iniciar a tela inicial");
            InitializeComponent();
            timeHide.Interval = (2000);
            timeHide.Tick += new EventHandler(fimTimerStart);
            timeHide.Start();

        }
        private void fimTimerStart(object sender, EventArgs e)
        {
            timeHide.Stop();

            try
            {
                if (!Directory.Exists(@"configs"))
                {
                    Directory.CreateDirectory(@"configs");
                }
                if (!Directory.Exists(@"logs"))
                {
                    Directory.CreateDirectory(@"logs");
                }
                Omini.debug("Carregando arquivo de configuração");
                if (File.Exists(@"configs\conf.json"))
                {
                    config = JsonConvert.DeserializeObject<Data.Config>(System.IO.File.ReadAllText(@"configs\conf.json"));
                }
                else
                {
                    Data.Config conf = new Data.Config();
                    conf.httpLink = @"http://localhost:8080";
                    conf.password = "123321";
                    conf.name = "OminiHost";
                    conf.vcenter_ip = "127.0.0.1";
                    conf.vcenter_pass = "123321";
                    conf.vcenter_user = "Administrator@vsphere.local";

                    StreamWriter file = new StreamWriter(@"configs\conf.json");
                    file.Write(JsonConvert.SerializeObject(conf));
                    file.Close();
                    //XJOmini.ShowERRO("AVISO", "Parece que é a primeira vez que você inicia o programa\nÉ preciso configurar o arquivo de configuração\nVá até a pasta configs e faça as alterações necessárias no arquivo conf.json", true);
                    this.config = conf;

                    Forms.edit_confs editConf = new Forms.edit_confs(this);
                    editConf.ShowDialog();
                }
                Omini.debug("Carregando ips permitidos");
                if (File.Exists(@"configs\ipsPermitidos.json"))
                {
                    IPS_PERMITIDOS = JsonConvert.DeserializeObject<List<string>>(System.IO.File.ReadAllText(@"configs\ipsPermitidos.json"));
                }
                else
                {
                    StreamWriter file = new StreamWriter(@"configs\ipsPermitidos.json");
                    file.Write("[]");
                    file.Close();
                }
                this.TelaInicial = new Inicio(this);

                Omini.debug("Tentando fazer conexão com o vCenter");
                try
                {
                    this.clientTasks = vmware.ConnectServer(this.config.vcenter_ip, new NetworkCredential(this.config.vcenter_user, this.config.vcenter_pass));
                    this.TelaInicial.lb_vcenter.Text = this.clientTasks.ServiceContent.About.FullName;
                    this.TelaInicial.Write("Conectado com sucesso no vcenter");
                    timeVcenterUpdates = new Thread(new ThreadStart(timevCenter));
                    timeVcenterUpdates.Start();
                }
                catch (Exception exp)
                {
                    Omini.ShowERRO("AVISO", "Falha ao conectar no VCenter\nVerifique as configurações.");
                    Forms.edit_confs editConf = new Forms.edit_confs(this);
                    editConf.ShowDialog();
                    Omini.ShowERRO("AVISO vCenter", "Abra o servidor novamente\nERRO: " + exp.Message, true);

                }
            }
            catch (Exception ex)
            {
                Omini.ShowERRO("ERRO", "Falha ao carregar o arquivo de configuração :\n" + ex.Message, true);
            }

            Omini.debug("Abrindo tela de incio!");

            this.TelaInicial.Show();
            this.Hide();
        }         
        private void timevCenter()
        {
            while (true)
            {
                try
                {
                    if (this.vmware.sessionAlive(this.clientTasks))
                    {
                        Thread.Sleep(60000);
                    }
                    else
                    {
                        this.clientTasks = vmware.ConnectServer(this.config.vcenter_ip, new NetworkCredential(this.config.vcenter_user, this.config.vcenter_pass));
                        this.TelaInicial.WriteConsole("[" + DateTime.Now + "] " + "Uma nova conexão foi refeita com o vcenter pois a sessão anterior tinha esgotado");
                        Thread.Sleep(20000);
                    }
                }
                catch (Exception ex)
                {
                    Omini.debug(ex.Message);
                    this.TelaInicial.WriteConsole("[" + DateTime.Now + "] " + "Tentando fazer reconexão com o vCenter");
                    this.clientTasks = vmware.ConnectServer(this.config.vcenter_ip, new NetworkCredential(this.config.vcenter_user, this.config.vcenter_pass));
                    if (this.clientTasks == null)
                    {
                        Thread.Sleep(10000);
                    }
                    else
                    {
                        this.TelaInicial.WriteConsole("[" + DateTime.Now + "] " + "Conexão feita com sucesso");
                        this.TelaInicial.lb_vcenter.Text = this.clientTasks.ServiceContent.About.FullName;
                        Thread.Sleep(60000);
                    }
                }
            }
        }

        private void carg_Click(object sender, EventArgs e)
        {

        }
    }
}
