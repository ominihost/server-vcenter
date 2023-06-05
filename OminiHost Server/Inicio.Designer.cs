namespace OminiHost_Server
{ 
    partial class Inicio
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Inicio));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.configuraçõesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iPsPermitidosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tarefasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.encryptadorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.limparConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vCenterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reconnectarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aPIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reiniciarServidorHTTPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.consolePanel = new System.Windows.Forms.TextBox();
            this.proxiesCount = new System.Windows.Forms.Label();
            this.lb_vcenter = new System.Windows.Forms.Label();
            this.servename = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.ForestGreen;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.vCenterToolStripMenuItem,
            this.aPIToolStripMenuItem,
            this.testesToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(753, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configuraçõesToolStripMenuItem,
            this.iPsPermitidosToolStripMenuItem,
            this.tarefasToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(49, 20);
            this.toolStripMenuItem1.Text = "Editar";
            // 
            // configuraçõesToolStripMenuItem
            // 
            this.configuraçõesToolStripMenuItem.Name = "configuraçõesToolStripMenuItem";
            this.configuraçõesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.configuraçõesToolStripMenuItem.Text = "Configurações";
            this.configuraçõesToolStripMenuItem.Click += new System.EventHandler(this.configuraçõesToolStripMenuItem_Click);
            // 
            // iPsPermitidosToolStripMenuItem
            // 
            this.iPsPermitidosToolStripMenuItem.Name = "iPsPermitidosToolStripMenuItem";
            this.iPsPermitidosToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.iPsPermitidosToolStripMenuItem.Text = "IPs permitidos";
            this.iPsPermitidosToolStripMenuItem.Click += new System.EventHandler(this.iPsPermitidosToolStripMenuItem_Click);
            // 
            // tarefasToolStripMenuItem
            // 
            this.tarefasToolStripMenuItem.Name = "tarefasToolStripMenuItem";
            this.tarefasToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.tarefasToolStripMenuItem.Text = "Tarefas";
            this.tarefasToolStripMenuItem.Click += new System.EventHandler(this.tarefasToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.encryptadorToolStripMenuItem,
            this.limparConsoleToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(84, 20);
            this.toolStripMenuItem2.Text = "Ferramentas";
            // 
            // encryptadorToolStripMenuItem
            // 
            this.encryptadorToolStripMenuItem.Name = "encryptadorToolStripMenuItem";
            this.encryptadorToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.encryptadorToolStripMenuItem.Text = "Encryptador";
            this.encryptadorToolStripMenuItem.Click += new System.EventHandler(this.encryptadorToolStripMenuItem_Click);
            // 
            // limparConsoleToolStripMenuItem
            // 
            this.limparConsoleToolStripMenuItem.Name = "limparConsoleToolStripMenuItem";
            this.limparConsoleToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.limparConsoleToolStripMenuItem.Text = "Limpar console";
            this.limparConsoleToolStripMenuItem.Click += new System.EventHandler(this.limparConsoleToolStripMenuItem_Click);
            // 
            // vCenterToolStripMenuItem
            // 
            this.vCenterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reconnectarToolStripMenuItem});
            this.vCenterToolStripMenuItem.Name = "vCenterToolStripMenuItem";
            this.vCenterToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.vCenterToolStripMenuItem.Text = "vCenter";
            // 
            // reconnectarToolStripMenuItem
            // 
            this.reconnectarToolStripMenuItem.Name = "reconnectarToolStripMenuItem";
            this.reconnectarToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.reconnectarToolStripMenuItem.Text = "Reconnectar";
            this.reconnectarToolStripMenuItem.Click += new System.EventHandler(this.reconnectarToolStripMenuItem_Click);
            // 
            // aPIToolStripMenuItem
            // 
            this.aPIToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reiniciarServidorHTTPToolStripMenuItem});
            this.aPIToolStripMenuItem.Name = "aPIToolStripMenuItem";
            this.aPIToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.aPIToolStripMenuItem.Text = "API";
            // 
            // reiniciarServidorHTTPToolStripMenuItem
            // 
            this.reiniciarServidorHTTPToolStripMenuItem.Name = "reiniciarServidorHTTPToolStripMenuItem";
            this.reiniciarServidorHTTPToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.reiniciarServidorHTTPToolStripMenuItem.Text = "Reiniciar servidor HTTP";
            this.reiniciarServidorHTTPToolStripMenuItem.Click += new System.EventHandler(this.reiniciarServidorHTTPToolStripMenuItem_Click);
            // 
            // testesToolStripMenuItem
            // 
            this.testesToolStripMenuItem.Name = "testesToolStripMenuItem";
            this.testesToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.testesToolStripMenuItem.Text = "testes";
            this.testesToolStripMenuItem.Click += new System.EventHandler(this.testesToolStripMenuItem_Click);
            // 
            // consolePanel
            // 
            this.consolePanel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.consolePanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.consolePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.consolePanel.Font = new System.Drawing.Font("Arial", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.consolePanel.Location = new System.Drawing.Point(0, 96);
            this.consolePanel.Multiline = true;
            this.consolePanel.Name = "consolePanel";
            this.consolePanel.ReadOnly = true;
            this.consolePanel.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.consolePanel.Size = new System.Drawing.Size(753, 354);
            this.consolePanel.TabIndex = 2;
            this.consolePanel.TextChanged += new System.EventHandler(this.consolePanel_TextChanged);
            // 
            // proxiesCount
            // 
            this.proxiesCount.AutoSize = true;
            this.proxiesCount.Location = new System.Drawing.Point(12, 28);
            this.proxiesCount.Name = "proxiesCount";
            this.proxiesCount.Size = new System.Drawing.Size(88, 13);
            this.proxiesCount.TabIndex = 3;
            this.proxiesCount.Text = "Proxies abertos 0";
            // 
            // lb_vcenter
            // 
            this.lb_vcenter.AutoSize = true;
            this.lb_vcenter.Dock = System.Windows.Forms.DockStyle.Right;
            this.lb_vcenter.Font = new System.Drawing.Font("Palatino Linotype", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_vcenter.Location = new System.Drawing.Point(642, 24);
            this.lb_vcenter.Name = "lb_vcenter";
            this.lb_vcenter.Size = new System.Drawing.Size(111, 19);
            this.lb_vcenter.TabIndex = 4;
            this.lb_vcenter.Text = "VCenter Version";
            this.lb_vcenter.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // servename
            // 
            this.servename.AutoSize = true;
            this.servename.Font = new System.Drawing.Font("Arial Black", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.servename.ForeColor = System.Drawing.Color.Green;
            this.servename.Location = new System.Drawing.Point(46, 50);
            this.servename.Name = "servename";
            this.servename.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.servename.Size = new System.Drawing.Size(61, 24);
            this.servename.TabIndex = 5;
            this.servename.Text = "Basic";
            this.servename.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.servename.UseMnemonic = false;
            this.servename.Click += new System.EventHandler(this.label1_Click);
            // 
            // Inicio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(753, 450);
            this.Controls.Add(this.servename);
            this.Controls.Add(this.lb_vcenter);
            this.Controls.Add(this.proxiesCount);
            this.Controls.Add(this.consolePanel);
            this.Controls.Add(this.menuStrip1);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Inicio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OminiHost";
            this.Load += new System.EventHandler(this.inicio_Load);
            this.Shown += new System.EventHandler(this.Inicio_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.TextBox consolePanel;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem encryptadorToolStripMenuItem;
        private System.Windows.Forms.Label proxiesCount;
        private System.Windows.Forms.ToolStripMenuItem configuraçõesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iPsPermitidosToolStripMenuItem;
        public System.Windows.Forms.Label lb_vcenter;
        private System.Windows.Forms.ToolStripMenuItem vCenterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reconnectarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem limparConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tarefasToolStripMenuItem;
        private System.Windows.Forms.Label servename;
        private System.Windows.Forms.ToolStripMenuItem aPIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reiniciarServidorHTTPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testesToolStripMenuItem;
    }
}