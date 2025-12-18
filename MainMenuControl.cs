using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Bike_STore_Project
{
    public partial class MainMenuControl : UserControl
    {
        public MainMenuControl()
        {
            InitializeComponent();

            inventoryToolStripMenuItem.Click += (s, e) => SwitchTo(() => new InventoryForm());
            salesToolStripMenuItem.Click += (s, e) => SwitchTo(() => new SalesForm());
            serviceToolStripMenuItem.Click += (s, e) => SwitchTo(() => new ServiceForm());
            transactionLogToolStripMenuItem.Click += (s, e) => SwitchTo(() => new TransactionLogForm());
            exitToolStripMenuItem.Click += (s, e) => Application.Exit();
        }

        private void SwitchTo(Func<Form> createForm)
        {
            var current = FindForm();
            if (current == null) return;

            var next = createForm();

            // Show next form, close current form after next is displayed
            next.StartPosition = FormStartPosition.CenterScreen;
            next.FormClosed += (s, e) => current.Close();

            current.Hide();
            next.Show();
        }
    }
}

