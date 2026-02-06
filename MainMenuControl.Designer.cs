namespace Bike_STore_Project
{
    partial class MainMenuControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            menuLogout = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            menuExit = new ToolStripMenuItem();
            operationsToolStripMenuItem = new ToolStripMenuItem();
            menuInventory = new ToolStripMenuItem();
            menuSales = new ToolStripMenuItem();
            menuService = new ToolStripMenuItem();
            logsToolStripMenuItem = new ToolStripMenuItem();
            menuTransactionLog = new ToolStripMenuItem();
            menuServiceLog = new ToolStripMenuItem();
            adminToolStripMenuItem = new ToolStripMenuItem();
            menuUserManagement = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, operationsToolStripMenuItem, logsToolStripMenuItem, adminToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(8, 2, 0, 2);
            menuStrip1.Size = new Size(700, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { menuLogout, toolStripSeparator1, menuExit });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 24);
            fileToolStripMenuItem.Text = "&File";
            // 
            // menuLogout
            // 
            menuLogout.Name = "menuLogout";
            menuLogout.ShortcutKeys = Keys.Control | Keys.Shift | Keys.L;
            menuLogout.Size = new Size(224, 26);
            menuLogout.Text = "&Logout";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(221, 6);
            // 
            // menuExit
            // 
            menuExit.Name = "menuExit";
            menuExit.ShortcutKeys = Keys.Alt | Keys.F4;
            menuExit.Size = new Size(224, 26);
            menuExit.Text = "E&xit";
            // 
            // operationsToolStripMenuItem
            // 
            operationsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { menuInventory, menuSales, menuService });
            operationsToolStripMenuItem.Name = "operationsToolStripMenuItem";
            operationsToolStripMenuItem.Size = new Size(96, 24);
            operationsToolStripMenuItem.Text = "&Operations";
            // 
            // menuInventory
            // 
            menuInventory.Name = "menuInventory";
            menuInventory.ShortcutKeys = Keys.Control | Keys.I;
            menuInventory.Size = new Size(224, 26);
            menuInventory.Text = "&Inventory";
            // 
            // menuSales
            // 
            menuSales.Name = "menuSales";
            menuSales.ShortcutKeys = Keys.Control | Keys.S;
            menuSales.Size = new Size(224, 26);
            menuSales.Text = "&Sales";
            // 
            // menuService
            // 
            menuService.Name = "menuService";
            menuService.ShortcutKeys = Keys.Control | Keys.E;
            menuService.Size = new Size(224, 26);
            menuService.Text = "S&ervice";
            // 
            // logsToolStripMenuItem
            // 
            logsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { menuTransactionLog, menuServiceLog });
            logsToolStripMenuItem.Name = "logsToolStripMenuItem";
            logsToolStripMenuItem.Size = new Size(54, 24);
            logsToolStripMenuItem.Text = "&Logs";
            // 
            // menuTransactionLog
            // 
            menuTransactionLog.Name = "menuTransactionLog";
            menuTransactionLog.ShortcutKeys = Keys.Control | Keys.T;
            menuTransactionLog.Size = new Size(246, 26);
            menuTransactionLog.Text = "&Transaction Log";
            // 
            // menuServiceLog
            // 
            menuServiceLog.Name = "menuServiceLog";
            menuServiceLog.ShortcutKeys = Keys.Control | Keys.L;
            menuServiceLog.Size = new Size(246, 26);
            menuServiceLog.Text = "Ser&vice Log";
            // 
            // adminToolStripMenuItem
            // 
            adminToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { menuUserManagement });
            adminToolStripMenuItem.Name = "adminToolStripMenuItem";
            adminToolStripMenuItem.Size = new Size(67, 24);
            adminToolStripMenuItem.Text = "&Admin";
            // 
            // menuUserManagement
            // 
            menuUserManagement.Name = "menuUserManagement";
            menuUserManagement.Size = new Size(224, 26);
            menuUserManagement.Text = "&User Management";
            // 
            // MainMenuControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(menuStrip1);
            Name = "MainMenuControl";
            Size = new Size(700, 30);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;

        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem menuLogout;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem menuExit;

        private ToolStripMenuItem operationsToolStripMenuItem;
        private ToolStripMenuItem menuInventory;
        private ToolStripMenuItem menuSales;
        private ToolStripMenuItem menuService;

        private ToolStripMenuItem logsToolStripMenuItem;
        private ToolStripMenuItem menuTransactionLog;
        private ToolStripMenuItem menuServiceLog;

        private ToolStripMenuItem adminToolStripMenuItem;
        private ToolStripMenuItem menuUserManagement;
    }
}
