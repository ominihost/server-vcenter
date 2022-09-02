namespace OminiHost_Server.Forms
{
    partial class viewTarefas
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tarefaViewer = new System.Windows.Forms.DataGridView();
            this.colum_ip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colum_criacao = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colum_tipo = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colum_estado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colum_msg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.tarefaViewer)).BeginInit();
            this.SuspendLayout();
            // 
            // tarefaViewer
            // 
            this.tarefaViewer.AllowUserToAddRows = false;
            this.tarefaViewer.AllowUserToDeleteRows = false;
            this.tarefaViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tarefaViewer.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.tarefaViewer.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.tarefaViewer.ColumnHeadersHeight = 35;
            this.tarefaViewer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colum_ip,
            this.colum_criacao,
            this.colum_tipo,
            this.colum_estado,
            this.colum_msg});
            this.tarefaViewer.Location = new System.Drawing.Point(-1, -1);
            this.tarefaViewer.MultiSelect = false;
            this.tarefaViewer.Name = "tarefaViewer";
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tarefaViewer.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.tarefaViewer.Size = new System.Drawing.Size(763, 386);
            this.tarefaViewer.TabIndex = 0;
            this.tarefaViewer.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.tarefaViewer_CellEndEdit);
            // 
            // colum_ip
            // 
            this.colum_ip.HeaderText = "IP";
            this.colum_ip.MaxInputLength = 10;
            this.colum_ip.Name = "colum_ip";
            this.colum_ip.ReadOnly = true;
            this.colum_ip.Width = 80;
            // 
            // colum_criacao
            // 
            this.colum_criacao.HeaderText = "Instânciada";
            this.colum_criacao.Name = "colum_criacao";
            this.colum_criacao.ReadOnly = true;
            // 
            // colum_tipo
            // 
            this.colum_tipo.HeaderText = "Tipo da tarefa";
            this.colum_tipo.Items.AddRange(new object[] {
            "Formatação",
            "Criação",
            "desconhecido"});
            this.colum_tipo.Name = "colum_tipo";
            this.colum_tipo.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colum_tipo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colum_tipo.Width = 120;
            // 
            // colum_estado
            // 
            this.colum_estado.HeaderText = "Estado atual";
            this.colum_estado.MaxInputLength = 50;
            this.colum_estado.Name = "colum_estado";
            this.colum_estado.Width = 120;
            // 
            // colum_msg
            // 
            this.colum_msg.HeaderText = "Menssagem";
            this.colum_msg.Name = "colum_msg";
            this.colum_msg.Width = 300;
            // 
            // viewTarefas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 382);
            this.Controls.Add(this.tarefaViewer);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "viewTarefas";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tarefas";
            ((System.ComponentModel.ISupportInitialize)(this.tarefaViewer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView tarefaViewer;
        private System.Windows.Forms.DataGridViewTextBoxColumn colum_ip;
        private System.Windows.Forms.DataGridViewTextBoxColumn colum_criacao;
        private System.Windows.Forms.DataGridViewComboBoxColumn colum_tipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colum_estado;
        private System.Windows.Forms.DataGridViewTextBoxColumn colum_msg;
    }
}