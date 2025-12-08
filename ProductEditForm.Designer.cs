
namespace Bike_STore_Project
{
    partial class ProductEditForm
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
            txtSerial = new TextBox();
            txtModel = new TextBox();
            label1 = new Label();
            label2 = new Label();
            numPrice = new NumericUpDown();
            btnOk = new Button();
            btnCancel = new Button();
            mainMenuControl1 = new MainMenuControl();
            ((System.ComponentModel.ISupportInitialize)numPrice).BeginInit();
            SuspendLayout();
            // 
            // txtSerial
            // 
            txtSerial.Location = new Point(52, 70);
            txtSerial.Name = "txtSerial";
            txtSerial.Size = new Size(125, 27);
            txtSerial.TabIndex = 0;
            // 
            // txtModel
            // 
            txtModel.Location = new Point(301, 69);
            txtModel.Name = "txtModel";
            txtModel.Size = new Size(125, 27);
            txtModel.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(56, 44);
            label1.Name = "label1";
            label1.Size = new Size(46, 20);
            label1.TabIndex = 2;
            label1.Text = "Serial";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(301, 44);
            label2.Name = "label2";
            label2.Size = new Size(52, 20);
            label2.TabIndex = 3;
            label2.Text = "Model";
            // 
            // numPrice
            // 
            numPrice.Location = new Point(492, 69);
            numPrice.Name = "numPrice";
            numPrice.Size = new Size(150, 27);
            numPrice.TabIndex = 4;
            // 
            // btnOk
            // 
            btnOk.Location = new Point(205, 153);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(94, 29);
            btnOk.TabIndex = 5;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(423, 155);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(94, 29);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "CANCEL";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // mainMenuControl1
            // 
            mainMenuControl1.Dock = DockStyle.Top;
            mainMenuControl1.Location = new Point(0, 0);
            mainMenuControl1.Name = "mainMenuControl1";
            mainMenuControl1.Size = new Size(800, 41);
            mainMenuControl1.TabIndex = 7;
            // 
            // ProductEditForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(mainMenuControl1);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(numPrice);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtModel);
            Controls.Add(txtSerial);
            Name = "ProductEditForm";
            Text = "ProductEditForm";
            ((System.ComponentModel.ISupportInitialize)numPrice).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private TextBox txtSerial;
        private TextBox txtModel;
        private Label label1;
        private Label label2;
        private NumericUpDown numPrice;
        private Button btnOk;
        private Button btnCancel;
        private MainMenuControl mainMenuControl1;
    }
}