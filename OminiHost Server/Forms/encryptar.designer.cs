namespace OminiHost_Server.Forms
{
    partial class encryptar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(encryptar));
            this.convert = new System.Windows.Forms.Button();
            this.senha = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.saidaEncrypt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.desconvert = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // convert
            // 
            this.convert.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.convert.BackColor = System.Drawing.Color.WhiteSmoke;
            this.convert.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.convert.FlatAppearance.BorderSize = 0;
            this.convert.Font = new System.Drawing.Font("Lucida Bright", 12.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.convert.ForeColor = System.Drawing.SystemColors.ControlText;
            this.convert.Location = new System.Drawing.Point(12, 108);
            this.convert.Name = "convert";
            this.convert.Size = new System.Drawing.Size(141, 43);
            this.convert.TabIndex = 0;
            this.convert.Text = "Encriptar";
            this.convert.UseVisualStyleBackColor = false;
            this.convert.Click += new System.EventHandler(this.convert_Click);
            // 
            // senha
            // 
            this.senha.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.senha.Location = new System.Drawing.Point(72, 12);
            this.senha.Name = "senha";
            this.senha.Size = new System.Drawing.Size(318, 20);
            this.senha.TabIndex = 1;
            this.senha.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.senha_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Digite:";
            // 
            // saidaEncrypt
            // 
            this.saidaEncrypt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.saidaEncrypt.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saidaEncrypt.Location = new System.Drawing.Point(16, 47);
            this.saidaEncrypt.Name = "saidaEncrypt";
            this.saidaEncrypt.ReadOnly = true;
            this.saidaEncrypt.Size = new System.Drawing.Size(378, 19);
            this.saidaEncrypt.TabIndex = 3;
            this.saidaEncrypt.Click += new System.EventHandler(this.textBox1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(168, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Copiado!";
            this.label2.Visible = false;
            // 
            // desconvert
            // 
            this.desconvert.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.desconvert.BackColor = System.Drawing.Color.WhiteSmoke;
            this.desconvert.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.desconvert.FlatAppearance.BorderSize = 0;
            this.desconvert.Font = new System.Drawing.Font("Lucida Bright", 12.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.desconvert.Location = new System.Drawing.Point(249, 108);
            this.desconvert.Name = "desconvert";
            this.desconvert.Size = new System.Drawing.Size(141, 43);
            this.desconvert.TabIndex = 5;
            this.desconvert.Text = "Desencryptar";
            this.desconvert.UseVisualStyleBackColor = false;
            this.desconvert.Click += new System.EventHandler(this.desconvert_Click);
            // 
            // encryptar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(406, 163);
            this.Controls.Add(this.desconvert);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.saidaEncrypt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.senha);
            this.Controls.Add(this.convert);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;            
            this.Name = "encryptar";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Encryptador";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.encryptar_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button convert;
        private System.Windows.Forms.TextBox senha;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox saidaEncrypt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button desconvert;
    }
}