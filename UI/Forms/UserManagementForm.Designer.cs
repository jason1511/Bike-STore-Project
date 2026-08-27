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
            tableRoot.Controls.Add(dataGridViewUsers, 0, 0);
            tableRoot.Controls.Add(panelButtons, 1, 0);
            tableRoot.Dock = DockStyle.Fill;
            tableRoot.Location = new Point(0, 0);
            tableRoot.Name = "tableRoot";
            tableRoot.RowCount = 1;
            tableRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableRoot.Size = new Size(1000, 600);
            tableRoot.TabIndex = 0;
            // 
            // dataGridViewUsers
            // 
            dataGridViewUsers.AllowUserToAddRows = false;
            dataGridViewUsers.AllowUserToDeleteRows = false;
            dataGridViewUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewUsers.ColumnHeadersHeight = 29;
            dataGridViewUsers.Dock = DockStyle.Fill;
            dataGridViewUsers.Location = new Point(3, 3);
            dataGridViewUsers.MultiSelect = false;
            dataGridViewUsers.Name = "dataGridViewUsers";
            dataGridViewUsers.ReadOnly = true;
            dataGridViewUsers.RowHeadersVisible = false;
            dataGridViewUsers.RowHeadersWidth = 51;
            dataGridViewUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewUsers.Size = new Size(774, 594);
            dataGridViewUsers.TabIndex = 0;
            // 
            // panelButtons
            // 
            panelButtons.AutoScroll = true;
            panelButtons.Controls.Add(btnAddUser);
            panelButtons.Controls.Add(btnResetPassword);
            panelButtons.Controls.Add(btnToggleActive);
            panelButtons.Controls.Add(btnToggleRole);
            panelButtons.Controls.Add(btnDeleteUser);
            panelButtons.Controls.Add(btnClose);
            panelButtons.Dock = DockStyle.Fill;
            panelButtons.FlowDirection = FlowDirection.TopDown;
            panelButtons.Location = new Point(783, 3);
            panelButtons.Name = "panelButtons";
            panelButtons.Padding = new Padding(10);
            panelButtons.Size = new Size(214, 594);
            panelButtons.TabIndex = 1;
            panelButtons.WrapContents = false;
            // 
            // btnAddUser
            // 
            btnAddUser.Location = new Point(13, 13);
            btnAddUser.Name = "btnAddUser";
            btnAddUser.Size = new Size(190, 36);
            btnAddUser.TabIndex = 0;
            btnAddUser.Text = "Add User";
            btnAddUser.UseVisualStyleBackColor = true;
            // 
            // btnResetPassword
            // 
            btnResetPassword.Location = new Point(13, 55);
            btnResetPassword.Name = "btnResetPassword";
            btnResetPassword.Size = new Size(190, 36);
            btnResetPassword.TabIndex = 1;
            btnResetPassword.Text = "Reset Password";
            btnResetPassword.UseVisualStyleBackColor = true;
            // 
            // btnToggleActive
            // 
            btnToggleActive.Location = new Point(13, 97);
            btnToggleActive.Name = "btnToggleActive";
            btnToggleActive.Size = new Size(190, 36);
            btnToggleActive.TabIndex = 2;
            btnToggleActive.Text = "Enable / Disable";
            btnToggleActive.UseVisualStyleBackColor = true;
            // 
            // btnToggleRole
            // 
            btnToggleRole.Location = new Point(13, 139);
            btnToggleRole.Name = "btnToggleRole";
            btnToggleRole.Size = new Size(190, 36);
            btnToggleRole.TabIndex = 3;
            btnToggleRole.Text = "Toggle Role";
            btnToggleRole.UseVisualStyleBackColor = true;
            // 
            // btnDeleteUser
            // 
            btnDeleteUser.Location = new Point(13, 181);
            btnDeleteUser.Name = "btnDeleteUser";
            btnDeleteUser.Size = new Size(190, 36);
            btnDeleteUser.TabIndex = 4;
            btnDeleteUser.Text = "Delete User";
            btnDeleteUser.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(13, 223);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(190, 36);
            btnClose.TabIndex = 5;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            // 
            // UserManagementForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 600);
            Controls.Add(tableRoot);
            Name = "UserManagementForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "User Management";
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
