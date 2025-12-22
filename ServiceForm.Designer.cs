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
            txtBrand = new TextBox();
            txtType = new TextBox();
            label2 = new Label();
            txtColor = new TextBox();
            label3 = new Label();
            label4 = new Label();
            numQuantity = new NumericUpDown();
            numServiceCost = new NumericUpDown();
            label5 = new Label();
            label6 = new Label();
            txtNotes = new TextBox();
            btnAddService = new Button();
            btnClear = new Button();
            mainMenuControl1 = new MainMenuControl();
            ((System.ComponentModel.ISupportInitialize)numQuantity).BeginInit();
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
            // txtBrand
            // 
            txtBrand.CharacterCasing = CharacterCasing.Upper;
            txtBrand.Location = new Point(87, 41);
            txtBrand.Name = "txtBrand";
            txtBrand.Size = new Size(125, 27);
            txtBrand.TabIndex = 1;
            // 
            // txtType
            // 
            txtType.CharacterCasing = CharacterCasing.Upper;
            txtType.Location = new Point(87, 87);
            txtType.Name = "txtType";
            txtType.Size = new Size(125, 27);
            txtType.TabIndex = 2;
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
            // txtColor
            // 
            txtColor.CharacterCasing = CharacterCasing.Upper;
            txtColor.Location = new Point(87, 138);
            txtColor.Name = "txtColor";
            txtColor.Size = new Size(125, 27);
            txtColor.TabIndex = 4;
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
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(31, 190);
            label4.Name = "label4";
            label4.Size = new Size(65, 20);
            label4.TabIndex = 6;
            label4.Text = "Quantity";
            // 
            // numQuantity
            // 
            numQuantity.Location = new Point(102, 188);
            numQuantity.Name = "numQuantity";
            numQuantity.Size = new Size(150, 27);
            numQuantity.TabIndex = 7;
            // 
            // numServiceCost
            // 
            numServiceCost.Location = new Point(102, 234);
            numServiceCost.Name = "numServiceCost";
            numServiceCost.Size = new Size(150, 27);
            numServiceCost.TabIndex = 8;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(7, 236);
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
            // ServiceForm
            // 
            AcceptButton = btnAddService;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(txtBrand);
            Controls.Add(mainMenuControl1);
            Controls.Add(btnClear);
            Controls.Add(btnAddService);
            Controls.Add(txtNotes);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(numServiceCost);
            Controls.Add(numQuantity);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(txtColor);
            Controls.Add(label2);
            Controls.Add(txtType);
            Controls.Add(label1);
            Name = "ServiceForm";
            Text = "ServiceForm";
            ((System.ComponentModel.ISupportInitialize)numQuantity).EndInit();
            ((System.ComponentModel.ISupportInitialize)numServiceCost).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Label label1;
        private TextBox txtBrand;
        private TextBox txtType;
        private Label label2;
        private TextBox txtColor;
        private Label label3;
        private Label label4;
        private NumericUpDown numQuantity;
        private NumericUpDown numServiceCost;
        private Label label5;
        private Label label6;
        private TextBox txtNotes;
        private Button btnAddService;
        private Button btnClear;
        private MainMenuControl mainMenuControl1;
    }
}