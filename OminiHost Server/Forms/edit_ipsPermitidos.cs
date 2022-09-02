using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;

namespace OminiHost_Server.Forms
{
    public partial class edit_ipsPermitidos : Form
    {
        public TelaLoad form1Program;
        public edit_ipsPermitidos(TelaLoad f1)
        {
            form1Program = f1;
            InitializeComponent();
            listaDeIPs.Items.Clear();
            listaDeIPs.Items.AddRange(form1Program.IPS_PERMITIDOS.ToArray());           
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAdd_click(object sender, EventArgs e)
        {
            if(ipTextBox.Text.Length < 6)
            {
                Omini.ShowERRO("ERRO", "Você precisa digitar o ip que quer adicionar");
                return;
            }
            if (form1Program.IPS_PERMITIDOS.Contains(ipTextBox.Text))                
            {
                Omini.ShowERRO("ERRO", "Esse ip já está permitido para fazer conexões");
            }
            else
            {
                form1Program.IPS_PERMITIDOS.Add(ipTextBox.Text);
                listaDeIPs.Items.Add(ipTextBox.Text);
                StreamWriter file = new StreamWriter(@"configs\ipsPermitidos.json");
                file.Write(JsonConvert.SerializeObject(form1Program.IPS_PERMITIDOS));
                file.Close();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            for(int x=0; x < listaDeIPs.Items.Count; x++)
            {
                if (listaDeIPs.GetSelected(x))
                {
                    form1Program.IPS_PERMITIDOS.Remove((string)listaDeIPs.Items[x]);
                    listaDeIPs.Items.RemoveAt(x);
                }
            }
            StreamWriter file = new StreamWriter(@"configs\ipsPermitidos.json");
            file.Write(JsonConvert.SerializeObject(form1Program.IPS_PERMITIDOS));
            file.Close();
        }
    }
}
