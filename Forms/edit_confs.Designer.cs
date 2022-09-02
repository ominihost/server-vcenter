namespace OminiHost_Server.Forms
{
    partial class edit_confs
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
            this.lb_vcconf = new System.Windows.Forms.Label();
            this.lb_ipvc = new System.Windows.Forms.Label();
            this.box_ipvc = new System.Windows.Forms.TextBox();
            this.lb_senhavc = new System.Windows.Forms.Label();
            this.box_senhavc = new System.Windows.Forms.TextBox();
            this.lb_confSocket = new System.Windows.Forms.Label();
            this.lb_ipsock = new System.Windows.Forms.Label();
            this.lb_senhaSock = new System.Windows.Forms.Label();
            this.box_ipsock = new System.Windows.Forms.TextBox();
            this.box_senhasock = new System.Windows.Forms.TextBox();
            this.btn_salvar = new System.Windows.Forms.Button();
            this.lb_uservc = new System.Windows.Forms.Label();
            this.box_uservc = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nameServer = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lb_vcconf
            // 
            this.lb_vcconf.AutoSize = true;
            this.lb_vcconf.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_vcconf.Location = new System.Drawing.Point(12, 9);
            this.lb_vcconf.Name = "lb_vcconf";
            this.lb_vcconf.Size = new System.Drawing.Size(168, 25);
            this.lb_vcconf.TabIndex = 0;
            this.lb_vcconf.Text = "vCenter configs:";
            // 
            // lb_ipvc
            // 
            this.lb_ipvc.AutoSize = true;
            this.lb_ipvc.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_ipvc.Location = new System.Drawing.Point(12, 48);
            this.lb_ipvc.Name = "lb_ipvc";
            this.lb_ipvc.Size = new System.Drawing.Size(23, 17);
            this.lb_ipvc.TabIndex = 1;
            this.lb_ipvc.Text = "ip:";
            // 
            // box_ipvc
            // 
            this.box_ipvc.Location = new System.Drawing.Point(69, 48);
            this.box_ipvc.Name = "box_ipvc";
            this.box_ipvc.Size = new System.Drawing.Size(171, 20);
            this.box_ipvc.TabIndex = 2;
            // 
            // lb_senhavc
            // 
            this.lb_senhavc.AutoSize = true;
            this.lb_senhavc.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_senhavc.Location = new System.Drawing.Point(8, 104);
            this.lb_senhavc.Name = "lb_senhavc";
            this.lb_senhavc.Size = new System.Drawing.Size(55, 17);
            this.lb_senhavc.TabIndex = 3;
            this.lb_senhavc.Text = "senha: ";
            // 
            // box_senhavc
            // 
            this.box_senhavc.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.box_senhavc.Location = new System.Drawing.Point(69, 101);
            this.box_senhavc.Name = "box_senhavc";
            this.box_senhavc.PasswordChar = '*';
            this.box_senhavc.Size = new System.Drawing.Size(171, 23);
            this.box_senhavc.TabIndex = 4;
            // 
            // lb_confSocket
            // 
            this.lb_confSocket.AutoSize = true;
            this.lb_confSocket.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_confSocket.Location = new System.Drawing.Point(10, 151);
            this.lb_confSocket.Name = "lb_confSocket";
            this.lb_confSocket.Size = new System.Drawing.Size(156, 25);
            this.lb_confSocket.TabIndex = 5;
            this.lb_confSocket.Text = "Server configs:";
            // 
            // lb_ipsock
            // 
            this.lb_ipsock.AutoSize = true;
            this.lb_ipsock.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_ipsock.Location = new System.Drawing.Point(14, 187);
            this.lb_ipsock.Name = "lb_ipsock";
            this.lb_ipsock.Size = new System.Drawing.Size(38, 17);
            this.lb_ipsock.TabIndex = 6;
            this.lb_ipsock.Text = "Link:";
            // 
            // lb_senhaSock
            // 
            this.lb_senhaSock.AutoSize = true;
            this.lb_senhaSock.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_senhaSock.Location = new System.Drawing.Point(12, 216);
            this.lb_senhaSock.Name = "lb_senhaSock";
            this.lb_senhaSock.Size = new System.Drawing.Size(55, 17);
            this.lb_senhaSock.TabIndex = 7;
            this.lb_senhaSock.Text = "senha: ";
            // 
            // box_ipsock
            // 
            this.box_ipsock.Location = new System.Drawing.Point(69, 187);
            this.box_ipsock.Name = "box_ipsock";
            this.box_ipsock.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.box_ipsock.Size = new System.Drawing.Size(171, 20);
            this.box_ipsock.TabIndex = 10;
            // 
            // box_senhasock
            // 
            this.box_senhasock.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.box_senhasock.Location = new System.Drawing.Point(69, 214);
            this.box_senhasock.Name = "box_senhasock";
            this.box_senhasock.PasswordChar = '*';
            this.box_senhasock.Size = new System.Drawing.Size(171, 23);
            this.box_senhasock.TabIndex = 11;
            // 
            // btn_salvar
            // 
            this.btn_salvar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_salvar.Location = new System.Drawing.Point(53, 297);
            this.btn_salvar.Name = "btn_salvar";
            this.btn_salvar.Size = new System.Drawing.Size(151, 61);
            this.btn_salvar.TabIndex = 12;
            this.btn_salvar.Text = "Salvar";
            this.btn_salvar.UseVisualStyleBackColor = true;
            this.btn_salvar.Click += new System.EventHandler(this.btn_salvar_Click);
            // 
            // lb_uservc
            // 
            this.lb_uservc.AutoSize = true;
            this.lb_uservc.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_uservc.Location = new System.Drawing.Point(8, 74);
            this.lb_uservc.Name = "lb_uservc";
            this.lb_uservc.Size = new System.Drawing.Size(40, 17);
            this.lb_uservc.TabIndex = 13;
            this.lb_uservc.Text = "user:";
            // 
            // box_uservc
            // 
            this.box_uservc.Location = new System.Drawing.Point(69, 74);
            this.box_uservc.Name = "box_uservc";
            this.box_uservc.Size = new System.Drawing.Size(171, 20);
            this.box_uservc.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 252);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 17);
            this.label1.TabIndex = 15;
            this.label1.Text = "Nome server: ";
            // 
            // nameServer
            // 
            this.nameServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameServer.Location = new System.Drawing.Point(101, 252);
            this.nameServer.MaxLength = 50;
            this.nameServer.Name = "nameServer";
            this.nameServer.Size = new System.Drawing.Size(155, 23);
            this.nameServer.TabIndex = 16;
            // 
            // edit_confs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 370);
            this.Controls.Add(this.nameServer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.box_uservc);
            this.Controls.Add(this.lb_uservc);
            this.Controls.Add(this.btn_salvar);
            this.Controls.Add(this.box_senhasock);
            this.Controls.Add(this.box_ipsock);
            this.Controls.Add(this.lb_senhaSock);
            this.Controls.Add(this.lb_ipsock);
            this.Controls.Add(this.lb_confSocket);
            this.Controls.Add(this.box_senhavc);
            this.Controls.Add(this.lb_senhavc);
            this.Controls.Add(this.box_ipvc);
            this.Controls.Add(this.lb_ipvc);
            this.Controls.Add(this.lb_vcconf);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "edit_confs";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Editar configurações";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_vcconf;
        private System.Windows.Forms.Label lb_ipvc;
        private System.Windows.Forms.TextBox box_ipvc;
        private System.Windows.Forms.Label lb_senhavc;
        private System.Windows.Forms.TextBox box_senhavc;
        private System.Windows.Forms.Label lb_confSocket;
        private System.Windows.Forms.Label lb_ipsock;
        private System.Windows.Forms.Label lb_senhaSock;
        private System.Windows.Forms.TextBox box_ipsock;
        private System.Windows.Forms.TextBox box_senhasock;
        private System.Windows.Forms.Button btn_salvar;
        private System.Windows.Forms.Label lb_uservc;
        private System.Windows.Forms.TextBox box_uservc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox nameServer;
    }
}