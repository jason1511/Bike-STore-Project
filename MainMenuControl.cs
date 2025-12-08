using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bike_STore_Project;

namespace Bike_STore_Project
{
    public partial class MainMenuControl : UserControl
    {
        public MainMenuControl()
        {
            InitializeComponent();

            inventoryToolStripMenuItem.Click += (s, e) => OpenForm(new InventoryForm());
            salesToolStripMenuItem.Click += (s, e) => OpenForm(new SalesForm());
            serviceToolStripMenuItem.Click += (s, e) => OpenForm(new ServiceForm());
            exitToolStripMenuItem.Click += (s, e) => Application.Exit();
        }
        private void OpenForm(Form f)
        {
            // Close current top-level form
            Form parent = this.FindForm();
            parent.Hide();

            f.FormClosed += (s, e) => parent.Close();
            f.Show();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
