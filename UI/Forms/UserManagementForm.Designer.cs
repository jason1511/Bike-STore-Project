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
            tableRoot = new Panel();
            dataGridViewUsers = new DataGridView();
            panelButtons = new Panel();
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
            tableRoot.Controls.Add(dataGridViewUsers);
            tableRoot.Controls.Add(panelButtons);
            tableRoot.Dock = DockStyle.Fill;
            tableRoot.Location = new Point(0, 0);
            tableRoot.Name = "tableRoot";
            tableRoot.Size = new Size(1000, 600);
            tableRoot.TabIndex = 0;
            // 
            // dataGridViewUsers
            // 
            dataGridViewUsers.AllowUserToAddRows = false;
            dataGridViewUsers.AllowUserToDeleteRows = false;
            dataGridViewUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewUsers.ColumnHeadersHeight = 29;
            dataGridViewUsers.Dock = DockStyle.None;
            dataGridViewUsers.Location = new Point(0, 0);
            dataGridViewUsers.MultiSelect = false;
            dataGridViewUsers.Name = "dataGridViewUsers";
            dataGridViewUsers.ReadOnly = true;
            dataGridViewUsers.RowHeadersVisible = false;
            dataGridViewUsers.RowHeadersWidth = 51;
            dataGridViewUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewUsers.Size = new Size(780, 600);
            dataGridViewUsers.TabIndex = 0;
            // 
            // panelButtons
            // 
            panelButtons.AutoScroll = false;
            panelButtons.Controls.Add(btnAddUser);
            panelButtons.Controls.Add(btnResetPassword);
            panelButtons.Controls.Add(btnToggleActive);
            panelButtons.Controls.Add(btnToggleRole);
            panelButtons.Controls.Add(btnDeleteUser);
            panelButtons.Controls.Add(btnClose);
            panelButtons.Dock = DockStyle.None;
            panelButtons.Location = new Point(0, 0);
            panelButtons.Name = "panelButtons";
            panelButtons.Padding = new Padding(10);
            panelButtons.Size = new Size(1000, 62);
            panelButtons.TabIndex = 1;
            // 
            // btnAddUser
            // 
            btnAddUser.Location = new Point(13, 13);
            btnAddUser.Name = "btnAddUser";
            btnAddUser.Size = new Size(190, 36);
            btnAddUser.TabIndex = 0;
            btnAddUser.Text = Strings.Get("User_Add");
            btnAddUser.UseVisualStyleBackColor = true;
            // 
            // btnResetPassword
            // 
            btnResetPassword.Location = new Point(13, 55);
            btnResetPassword.Name = "btnResetPassword";
            btnResetPassword.Size = new Size(190, 36);
            btnResetPassword.TabIndex = 1;
            btnResetPassword.Text = Strings.Get("User_ResetPassword");
            btnResetPassword.UseVisualStyleBackColor = true;
            // 
            // btnToggleActive
            // 
            btnToggleActive.Location = new Point(13, 97);
            btnToggleActive.Name = "btnToggleActive";
            btnToggleActive.Size = new Size(190, 36);
            btnToggleActive.TabIndex = 2;
            btnToggleActive.Text = Strings.Get("User_Disable");
            btnToggleActive.UseVisualStyleBackColor = true;
            // 
            // btnToggleRole
            // 
            btnToggleRole.Location = new Point(13, 139);
            btnToggleRole.Name = "btnToggleRole";
            btnToggleRole.Size = new Size(190, 36);
            btnToggleRole.TabIndex = 3;
            btnToggleRole.Text = Strings.Get("User_MakeAdmin");
            btnToggleRole.UseVisualStyleBackColor = true;
            // 
            // btnDeleteUser
            // 
            btnDeleteUser.Location = new Point(13, 181);
            btnDeleteUser.Name = "btnDeleteUser";
            btnDeleteUser.Size = new Size(190, 36);
            btnDeleteUser.TabIndex = 4;
            btnDeleteUser.Text = Strings.Get("User_Delete");
            btnDeleteUser.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(13, 223);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(190, 36);
            btnClose.TabIndex = 5;
            btnClose.Text = Strings.Get("Common_Close");
            btnClose.UseVisualStyleBackColor = true;
            // 
            // UserManagementForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1000, 600);
            Controls.Add(tableRoot);
            Name = "UserManagementForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = Strings.Get("User_Title");
            tableRoot.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewUsers).EndInit();
            panelButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel tableRoot;
        private DataGridView dataGridViewUsers;
        private Panel panelButtons;
        private Button btnAddUser;
        private Button btnResetPassword;
        private Button btnToggleActive;
        private Button btnToggleRole;
        private Button btnDeleteUser;
        private Button btnClose;
    }
}
