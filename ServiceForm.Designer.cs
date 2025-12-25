namespace Bike_STore_Project
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
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            numServiceCost = new NumericUpDown();
            label5 = new Label();
            label6 = new Label();
            txtNotes = new TextBox();
            btnAddService = new Button();
            btnClear = new Button();
            mainMenuControl1 = new MainMenuControl();
            txtBrand = new TextBox();
            txtType = new TextBox();
            txtColor = new TextBox();
            ((System.ComponentModel.ISupportInitialize)numServiceCost).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(33, 41);
            label1.Name = "label1";
            label1.Size = new Size(48, 20);
            label1.TabIndex = 0;
            label1.Text = "Brand";
            label1.TextAlign = ContentAlignment.TopCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(33, 90);
            label2.Name = "label2";
            label2.Size = new Size(40, 20);
            label2.TabIndex = 3;
            label2.Text = "Type";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(31, 141);
            label3.Name = "label3";
            label3.Size = new Size(45, 20);
            label3.TabIndex = 5;
            label3.Text = "Color";
            // 
            // numServiceCost
            // 
            numServiceCost.Location = new Point(102, 190);
            numServiceCost.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            numServiceCost.Name = "numServiceCost";
            numServiceCost.Size = new Size(150, 27);
            numServiceCost.TabIndex = 8;
            numServiceCost.ThousandsSeparator = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 194);
            label5.Name = "label5";
            label5.Size = new Size(89, 20);
            label5.TabIndex = 9;
            label5.Text = "Service Cost";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(33, 279);
            label6.Name = "label6";
            label6.Size = new Size(48, 20);
            label6.TabIndex = 10;
            label6.Text = "Notes";
            // 
            // txtNotes
            // 
            txtNotes.CharacterCasing = CharacterCasing.Upper;
            txtNotes.Location = new Point(102, 272);
            txtNotes.Multiline = true;
            txtNotes.Name = "txtNotes";
            txtNotes.Size = new Size(193, 97);
            txtNotes.TabIndex = 11;
            // 
            // btnAddService
            // 
            btnAddService.Location = new Point(356, 41);
            btnAddService.Name = "btnAddService";
            btnAddService.Size = new Size(122, 29);
            btnAddService.TabIndex = 12;
            btnAddService.Text = "ADD SERVICE";
            btnAddService.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            btnClear.Location = new Point(531, 41);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(94, 29);
            btnClear.TabIndex = 13;
            btnClear.Text = "CLEAR";
            btnClear.UseVisualStyleBackColor = true;
            // 
            // mainMenuControl1
            // 
            mainMenuControl1.Dock = DockStyle.Top;
            mainMenuControl1.Location = new Point(0, 0);
            mainMenuControl1.Name = "mainMenuControl1";
            mainMenuControl1.Size = new Size(800, 31);
            mainMenuControl1.TabIndex = 14;
            // 
            // txtBrand
            // 
            txtBrand.Location = new Point(102, 41);
            txtBrand.Name = "txtBrand";
            txtBrand.Size = new Size(125, 27);
            txtBrand.TabIndex = 15;
            // 
            // txtType
            // 
            txtType.Location = new Point(102, 90);
            txtType.Name = "txtType";
            txtType.Size = new Size(125, 27);
            txtType.TabIndex = 16;
            // 
            // txtColor
            // 
            txtColor.Location = new Point(102, 138);
            txtColor.Name = "txtColor";
            txtColor.Size = new Size(125, 27);
            txtColor.TabIndex = 17;
            // 
            // ServiceForm
            // 
            AcceptButton = btnAddService;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(txtColor);
            Controls.Add(txtType);
            Controls.Add(txtBrand);
            Controls.Add(mainMenuControl1);
            Controls.Add(btnClear);
            Controls.Add(btnAddService);
            Controls.Add(txtNotes);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(numServiceCost);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "ServiceForm";
            Text = "ServiceForm";
            ((System.ComponentModel.ISupportInitialize)numServiceCost).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Label label1;
        private Label label2;
        private Label label3;
        private NumericUpDown numServiceCost;
        private Label label5;
        private Label label6;
        private TextBox txtNotes;
        private Button btnAddService;
        private Button btnClear;
        private MainMenuControl mainMenuControl1;
        private TextBox txtBrand;
        private TextBox txtType;
        private TextBox txtColor;
    }
}