using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OminiHost_Server.Forms
{
    public partial class edit_confs : Form
    {
        TelaLoad Form1Program;
        public edit_confs(TelaLoad f1p)
        {
            Form1Program = f1p;
            InitializeComponent();
            box_ipvc.Text = Form1Program.config.vcenter_ip;
            box_senhavc.Text = Form1Program.config.vcenter_pass;
            box_uservc.Text = Form1Program.config.vcenter_user;
            box_ipsock.Text = Form1Program.config.httpLink;                      
            box_senhasock.Text = Form1Program.config.password;
            nameServer.Text = Form1Program.config.name;
        }

        private void btn_salvar_Click(object sender, EventArgs e)
        {
            Form1Program.config.vcenter_ip = box_ipvc.Text;
            Form1Program.config.vcenter_user = box_uservc.Text;
            Form1Program.config.vcenter_pass = box_senhavc.Text;
            Form1Program.config.httpLink = box_ipsock.Text;            
            Form1Program.config.password = box_senhasock.Text;
            Form1Program.config.name = nameServer.Text;

            StreamWriter file = new StreamWriter(@"configs\conf.json");
            file.Write(JsonConvert.SerializeObject(Form1Program.config));
            file.Close();
            this.Close();
            Form1Program.TelaInicial.attNameServer();
        }
    }
}
