namespace Bike_STore_Project
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            tableRoot = new TableLayoutPanel();
            panelLogin = new Panel();
            label1 = new Label();
            txtUser = new TextBox();
            label2 = new Label();
            txtPass = new TextBox();
            btnLogin = new Button();
            tableRoot.SuspendLayout();
            panelLogin.SuspendLayout();
            SuspendLayout();
            // 
            // tableRoot
            // 
            tableRoot.ColumnCount = 3;
            tableRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableRoot.ColumnStyles.Add(new ColumnStyle());
            tableRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableRoot.Controls.Add(panelLogin, 1, 1);
            tableRoot.Dock = DockStyle.Fill;
            tableRoot.Location = new Point(0, 0);
            tableRoot.Name = "tableRoot";
            tableRoot.RowCount = 3;
            tableRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableRoot.RowStyles.Add(new RowStyle());
            tableRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableRoot.Size = new Size(490, 425);
            tableRoot.TabIndex = 0;
            // 
            // panelLogin
            // 
            panelLogin.BorderStyle = BorderStyle.FixedSingle;
            panelLogin.Controls.Add(label1);
            panelLogin.Controls.Add(txtUser);
            panelLogin.Controls.Add(label2);
            panelLogin.Controls.Add(txtPass);
            panelLogin.Controls.Add(btnLogin);
            panelLogin.Location = new Point(30, 30);
            panelLogin.Name = "panelLogin";
            panelLogin.Padding = new Padding(30);
            panelLogin.Size = new Size(350, 220);
            panelLogin.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(75, 20);
            label1.TabIndex = 0;
            label1.Text = Strings.Get("Login_Username");
            // 
            // txtUser
            // 
            txtUser.Location = new Point(0, 25);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(260, 27);
            txtUser.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(0, 70);
            label2.Name = "label2";
            label2.Size = new Size(70, 20);
            label2.TabIndex = 2;
            label2.Text = Strings.Get("Login_Password");
            // 
            // txtPass
            // 
            txtPass.Location = new Point(0, 95);
            txtPass.Name = "txtPass";
            txtPass.PasswordChar = 'x';
            txtPass.Size = new Size(260, 27);
            txtPass.TabIndex = 3;
            txtPass.UseSystemPasswordChar = true;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(0, 150);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(260, 32);
            btnLogin.TabIndex = 4;
            btnLogin.Text = Strings.Get("Login_SignIn");
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(490, 425);
            Controls.Add(tableRoot);
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = Strings.Get("Login_SignIn");
            tableRoot.ResumeLayout(false);
            panelLogin.ResumeLayout(false);
            panelLogin.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableRoot;
        private Panel panelLogin;
        private TextBox txtUser;
        private TextBox txtPass;
        private Label label1;
        private Label label2;
        private Button btnLogin;
    }
}
