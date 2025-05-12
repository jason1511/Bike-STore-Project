namespace WinFormsApp1
{
    partial class ServiceForm
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
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            navigateToolStripMenuItem = new ToolStripMenuItem();
            inventoryToolStripMenuItem = new ToolStripMenuItem();
            salesToolStripMenuItem = new ToolStripMenuItem();
            serviceLogToolStripMenuItem = new ToolStripMenuItem();
            reportsToolStripMenuItem = new ToolStripMenuItem();
            label1 = new Label();
            txtBikeSerial = new TextBox();
            label2 = new Label();
            txtService = new TextBox();
            label3 = new Label();
            label4 = new Label();
            txtCost = new TextBox();
            btnAddService = new Button();
            cmbPartUsed = new ComboBox();
            txtPartQty = new TextBox();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, navigateToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 24);
            fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(116, 26);
            exitToolStripMenuItem.Text = "Exit";
            // 
            // navigateToolStripMenuItem
            // 
            navigateToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { inventoryToolStripMenuItem, salesToolStripMenuItem, serviceLogToolStripMenuItem, reportsToolStripMenuItem });
            navigateToolStripMenuItem.Name = "navigateToolStripMenuItem";
            navigateToolStripMenuItem.Size = new Size(83, 24);
            navigateToolStripMenuItem.Text = "Navigate";
            // 
            // inventoryToolStripMenuItem
            // 
            inventoryToolStripMenuItem.Name = "inventoryToolStripMenuItem";
            inventoryToolStripMenuItem.Size = new Size(168, 26);
            inventoryToolStripMenuItem.Text = "Inventory";
            // 
            // salesToolStripMenuItem
            // 
            salesToolStripMenuItem.Name = "salesToolStripMenuItem";
            salesToolStripMenuItem.Size = new Size(168, 26);
            salesToolStripMenuItem.Text = "Sales";
            // 
            // serviceLogToolStripMenuItem
            // 
            serviceLogToolStripMenuItem.Name = "serviceLogToolStripMenuItem";
            serviceLogToolStripMenuItem.Size = new Size(168, 26);
            serviceLogToolStripMenuItem.Text = "Service Log";
            // 
            // reportsToolStripMenuItem
            // 
            reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            reportsToolStripMenuItem.Size = new Size(168, 26);
            reportsToolStripMenuItem.Text = "Report";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 41);
            label1.Name = "label1";
            label1.Size = new Size(78, 20);
            label1.TabIndex = 1;
            label1.Text = "Bike Serial";
            // 
            // txtBikeSerial
            // 
            txtBikeSerial.Location = new Point(199, 38);
            txtBikeSerial.Name = "txtBikeSerial";
            txtBikeSerial.Size = new Size(437, 27);
            txtBikeSerial.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 76);
            label2.Name = "label2";
            label2.Size = new Size(129, 20);
            label2.TabIndex = 3;
            label2.Text = "Service Performed";
            // 
            // txtService
            // 
            txtService.Location = new Point(199, 76);
            txtService.Name = "txtService";
            txtService.Size = new Size(437, 27);
            txtService.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 115);
            label3.Name = "label3";
            label3.Size = new Size(100, 20);
            label3.TabIndex = 5;
            label3.Text = "Part Replaced";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 153);
            label4.Name = "label4";
            label4.Size = new Size(89, 20);
            label4.TabIndex = 7;
            label4.Text = "Service Cost";
            // 
            // txtCost
            // 
            txtCost.Location = new Point(199, 153);
            txtCost.Name = "txtCost";
            txtCost.Size = new Size(437, 27);
            txtCost.TabIndex = 8;
            // 
            // btnAddService
            // 
            btnAddService.Location = new Point(18, 210);
            btnAddService.Name = "btnAddService";
            btnAddService.Size = new Size(94, 29);
            btnAddService.TabIndex = 9;
            btnAddService.Text = "Submit";
            btnAddService.UseVisualStyleBackColor = true;
            btnAddService.Click += btnAddService_Click;
            // 
            // cmbPartUsed
            // 
            cmbPartUsed.FormattingEnabled = true;
            cmbPartUsed.Location = new Point(199, 115);
            cmbPartUsed.Name = "cmbPartUsed";
            cmbPartUsed.Size = new Size(437, 28);
            cmbPartUsed.TabIndex = 10;
            // 
            // txtPartQty
            // 
            txtPartQty.Location = new Point(642, 116);
            txtPartQty.Name = "txtPartQty";
            txtPartQty.PlaceholderText = "0";
            txtPartQty.Size = new Size(125, 27);
            txtPartQty.TabIndex = 11;
            // 
            // ServiceForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(txtPartQty);
            Controls.Add(cmbPartUsed);
            Controls.Add(btnAddService);
            Controls.Add(txtCost);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(txtService);
            Controls.Add(label2);
            Controls.Add(txtBikeSerial);
            Controls.Add(label1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "ServiceForm";
            Text = "Service";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem navigateToolStripMenuItem;
        private ToolStripMenuItem inventoryToolStripMenuItem;
        private ToolStripMenuItem salesToolStripMenuItem;
        private ToolStripMenuItem serviceLogToolStripMenuItem;
        private ToolStripMenuItem reportsToolStripMenuItem;
        private Label label1;
        private TextBox txtBikeSerial;
        private Label label2;
        private TextBox txtService;
        private Label label3;
        private Label label4;
        private TextBox txtCost;
        private Button btnAddService;
        private ComboBox cmbPartUsed;
        private TextBox txtPartQty;
    }
}