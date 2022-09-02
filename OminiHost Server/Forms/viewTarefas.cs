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
    public partial class viewTarefas : Form
    {
        TelaLoad Form1_Program;
        public viewTarefas(TelaLoad f1)
        {
            this.Form1_Program = f1;
            InitializeComponent();
            foreach (KeyValuePair<string,Class.Tarefa> ip in Form1_Program.ip_tarefas)
            {                             
                string[] item = new string[] { (ip.Key), (ip.Value).TimeCriacao, (ip.Value).getTarefaType(), (ip.Value).estadoAtual, (ip.Value).msg};
                tarefaViewer.Rows.Add(item);
            }            
        }

        private void tarefaViewer_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string ipLinha = tarefaViewer.Rows[e.RowIndex].Cells[0].Value.ToString();
            string novoValor = (string)tarefaViewer.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            if (tarefaViewer.Columns[e.ColumnIndex].Name == "colum_tipo")
            {                
                if(novoValor.Equals("Criação"))
                    Form1_Program.ip_tarefas[ipLinha].setTipoTarefa(Class.Tarefa.Tipos.criacao);                
                else
                    Form1_Program.ip_tarefas[ipLinha].setTipoTarefa(Class.Tarefa.Tipos.formatacao);
            }
            else if (tarefaViewer.Columns[e.ColumnIndex].Name == "colum_estado")
            {
                Form1_Program.ip_tarefas[ipLinha].estadoAtual = novoValor;
            }
            else if (tarefaViewer.Columns[e.ColumnIndex].Name == "colum_msg")
            {
                Form1_Program.ip_tarefas[ipLinha].msg = novoValor;
            }               
        }
    }
}
