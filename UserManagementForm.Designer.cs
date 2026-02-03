namespace Bike_STore_Project
{
    partial class UserManagementForm
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
            dataGridViewUsers = new DataGridView();
            panelButtons = new FlowLayoutPanel();
            btnAddUser = new Button();
            btnResetPassword = new Button();
            btnToggleActive = new Button();
            btnToggleRole = new Button();
            btnDeleteUser = new Button();
            btnClose = new Button();

            tableRoot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewUsers).BeginInit();
            panelButtons.SuspendLayout();
            SuspendLayout();

            // 
            // tableRoot
            // 
            tableRoot.ColumnCount = 2;
            tableRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
            tableRoot.Dock = DockStyle.Fill;
            tableRoot.RowCount = 1;
            tableRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableRoot.Controls.Add(dataGridViewUsers, 0, 0);
            tableRoot.Controls.Add(panelButtons, 1, 0);

            // 
            // dataGridViewUsers
            // 
            dataGridViewUsers.Dock = DockStyle.Fill;
            dataGridViewUsers.AllowUserToAddRows = false;
            dataGridViewUsers.AllowUserToDeleteRows = false;
            dataGridViewUsers.ReadOnly = true;
            dataGridViewUsers.MultiSelect = false;
            dataGridViewUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewUsers.RowHeadersVisible = false;
            dataGridViewUsers.Name = "dataGridViewUsers";

            // 
            // panelButtons
            // 
            panelButtons.Dock = DockStyle.Fill;
            panelButtons.FlowDirection = FlowDirection.TopDown;
            panelButtons.WrapContents = false;
            panelButtons.Padding = new Padding(10);
            panelButtons.AutoScroll = true;

            // 
            // btnAddUser
            // 
            btnAddUser.Name = "btnAddUser";
            btnAddUser.Text = "Add User";
            btnAddUser.Width = 190;
            btnAddUser.Height = 36;
            btnAddUser.UseVisualStyleBackColor = true;

            // 
            // btnResetPassword
            // 
            btnResetPassword.Name = "btnResetPassword";
            btnResetPassword.Text = "Reset Password";
            btnResetPassword.Width = 190;
            btnResetPassword.Height = 36;
            btnResetPassword.UseVisualStyleBackColor = true;

            // 
            // btnToggleActive
            // 
            btnToggleActive.Name = "btnToggleActive";
            btnToggleActive.Text = "Enable / Disable";
            btnToggleActive.Width = 190;
            btnToggleActive.Height = 36;
            btnToggleActive.UseVisualStyleBackColor = true;

            // 
            // btnToggleRole
            // 
            btnToggleRole.Name = "btnToggleRole";
            btnToggleRole.Text = "Toggle Role";
            btnToggleRole.Width = 190;
            btnToggleRole.Height = 36;
            btnToggleRole.UseVisualStyleBackColor = true;

            // 
            // btnDeleteUser
            // 
            btnDeleteUser.Name = "btnDeleteUser";
            btnDeleteUser.Text = "Delete User";
            btnDeleteUser.Width = 190;
            btnDeleteUser.Height = 36;
            btnDeleteUser.UseVisualStyleBackColor = true;

            // 
            // btnClose
            // 
            btnClose.Name = "btnClose";
            btnClose.Text = "Close";
            btnClose.Width = 190;
            btnClose.Height = 36;
            btnClose.UseVisualStyleBackColor = true;

            // 
            // panelButtons Controls
            // 
            panelButtons.Controls.Add(btnAddUser);
            panelButtons.Controls.Add(btnResetPassword);
            panelButtons.Controls.Add(btnToggleActive);
            panelButtons.Controls.Add(btnToggleRole);
            panelButtons.Controls.Add(btnDeleteUser);
            panelButtons.Controls.Add(btnClose);

            // 
            // UserManagementForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 600);
            Controls.Add(tableRoot);
            Name = "UserManagementForm";
            Text = "User Management";
            StartPosition = FormStartPosition.CenterParent;
            WindowState = FormWindowState.Maximized;

            tableRoot.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewUsers).EndInit();
            panelButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableRoot;
        private DataGridView dataGridViewUsers;
        private FlowLayoutPanel panelButtons;
        private Button btnAddUser;
        private Button btnResetPassword;
        private Button btnToggleActive;
        private Button btnToggleRole;
        private Button btnDeleteUser;
        private Button btnClose;
    }
}
