namespace WinFormsApp1
{
    partial class SalesForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtBikeModel = new TextBox();
            txtBikeSerial = new TextBox();
            txtPrice = new TextBox();
            label1 = new Label();
            label2 = new Label();
            txtCustomerName = new TextBox();
            label3 = new Label();
            label4 = new Label();
            btnAddSale = new Button();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            navigateToolStripMenuItem = new ToolStripMenuItem();
            salesFormToolStripMenuItem = new ToolStripMenuItem();
            salesToolStripMenuItem = new ToolStripMenuItem();
            salesFormToolStripMenuItem1 = new ToolStripMenuItem();
            reportToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // txtBikeModel
            // 
            txtBikeModel.Location = new Point(57, 91);
            txtBikeModel.Name = "txtBikeModel";
            txtBikeModel.Size = new Size(125, 27);
            txtBikeModel.TabIndex = 0;
            // 
            // txtBikeSerial
            // 
            txtBikeSerial.Location = new Point(254, 91);
            txtBikeSerial.Name = "txtBikeSerial";
            txtBikeSerial.Size = new Size(125, 27);
            txtBikeSerial.TabIndex = 1;
            // 
            // txtPrice
            // 
            txtPrice.Location = new Point(429, 91);
            txtPrice.Name = "txtPrice";
            txtPrice.Size = new Size(125, 27);
            txtPrice.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(57, 50);
            label1.Name = "label1";
            label1.Size = new Size(84, 20);
            label1.TabIndex = 3;
            label1.Text = "Bike Model";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(254, 50);
            label2.Name = "label2";
            label2.Size = new Size(78, 20);
            label2.TabIndex = 4;
            label2.Text = "Bike Serial";
            // 
            // txtCustomerName
            // 
            txtCustomerName.Location = new Point(612, 91);
            txtCustomerName.Name = "txtCustomerName";
            txtCustomerName.Size = new Size(125, 27);
            txtCustomerName.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(429, 50);
            label3.Name = "label3";
            label3.Size = new Size(41, 20);
            label3.TabIndex = 6;
            label3.Text = "Price";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(612, 50);
            label4.Name = "label4";
            label4.Size = new Size(116, 20);
            label4.TabIndex = 7;
            label4.Text = "Customer Name";
            // 
            // btnAddSale
            // 
            btnAddSale.Location = new Point(57, 201);
            btnAddSale.Name = "btnAddSale";
            btnAddSale.Size = new Size(94, 29);
            btnAddSale.TabIndex = 8;
            btnAddSale.Text = "ADD SALES";
            btnAddSale.UseVisualStyleBackColor = true;
            btnAddSale.Click += btnAddSale_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, navigateToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 28);
            menuStrip1.TabIndex = 9;
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
            navigateToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { salesFormToolStripMenuItem, salesToolStripMenuItem, salesFormToolStripMenuItem1, reportToolStripMenuItem });
            navigateToolStripMenuItem.Name = "navigateToolStripMenuItem";
            navigateToolStripMenuItem.Size = new Size(83, 24);
            navigateToolStripMenuItem.Text = "Navigate";
            // 
            // salesFormToolStripMenuItem
            // 
            salesFormToolStripMenuItem.Name = "salesFormToolStripMenuItem";
            salesFormToolStripMenuItem.Size = new Size(224, 26);
            salesFormToolStripMenuItem.Text = "Inventory";
            // 
            // salesToolStripMenuItem
            // 
            salesToolStripMenuItem.Name = "salesToolStripMenuItem";
            salesToolStripMenuItem.Size = new Size(224, 26);
            salesToolStripMenuItem.Text = "Sales";
            // 
            // salesFormToolStripMenuItem1
            // 
            salesFormToolStripMenuItem1.Name = "salesFormToolStripMenuItem1";
            salesFormToolStripMenuItem1.Size = new Size(224, 26);
            salesFormToolStripMenuItem1.Text = "Service Log";
            salesFormToolStripMenuItem1.Click += salesFormToolStripMenuItem1_Click;
            // 
            // reportToolStripMenuItem
            // 
            reportToolStripMenuItem.Name = "reportToolStripMenuItem";
            reportToolStripMenuItem.Size = new Size(224, 26);
            reportToolStripMenuItem.Text = "Report";
            // 
            // SalesForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnAddSale);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(txtCustomerName);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtPrice);
            Controls.Add(txtBikeSerial);
            Controls.Add(txtBikeModel);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "SalesForm";
            Text = "Form1";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtBikeModel;
        private TextBox txtBikeSerial;
        private TextBox txtPrice;
        private Label label1;
        private Label label2;
        private TextBox txtCustomerName;
        private Label label3;
        private Label label4;
        private Button btnAddSale;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem navigateToolStripMenuItem;
        private ToolStripMenuItem salesFormToolStripMenuItem;
        private ToolStripMenuItem salesToolStripMenuItem;
        private ToolStripMenuItem salesFormToolStripMenuItem1;
        private ToolStripMenuItem reportToolStripMenuItem;
    }
}
