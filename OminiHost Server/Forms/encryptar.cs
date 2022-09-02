using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OminiHost_Server.Forms
{
    public partial class encryptar : Form
    {
        public encryptar()
        {
            InitializeComponent();    
        }
       
      
        private void textBox1_Click(object sender, EventArgs e)
        {
            if (senha.Text.Length > 0 && saidaEncrypt.Text.Length > 0)
            {
                Clipboard.SetText(saidaEncrypt.Text);
                label2.Show();
            }
        }
        private void converter()
        {
            label2.Hide();
            if (senha.Text.Length > 0)
            {
                saidaEncrypt.Text = Omini.Base64Encode(senha.Text);
            }
            else
                Omini.ShowERRO("ERRO", "Você precisa digitar algo para encryptar");
        }
        private void desconverter()
        {
            label2.Hide();
            if (senha.Text.Length > 0)
            {
                saidaEncrypt.Text = Omini.Base64Decode(senha.Text);
            }
            else
                Omini.ShowERRO("ERRO", "Você precisa digitar algo para desencryptar");
        }

        private void convert_Click(object sender, EventArgs e)
        {
            this.converter();
        }

        private void desconvert_Click(object sender, EventArgs e)
        {
            this.desconverter();
        }

        private void encryptar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if((Keys)e.KeyChar == Keys.Enter)
            {
                this.converter();
            }
        }

        private void senha_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                this.converter();
            }
        }
    }
}
