using System.Drawing;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    internal static class DialogPrompt
    {
        public static string Show(IWin32Window owner, string title, string label, string initial = "")
        {
            using var form = new Form
            {
                Text = title, StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog, MinimizeBox = false, MaximizeBox = false,
                ClientSize = new Size(430, 145)
            };
            var prompt = new Label { Text = label, Left = 12, Top = 14, Width = 400, AutoSize = false };
            var input = new TextBox { Text = initial, Left = 12, Top = 42, Width = 400 };
            var ok = new Button { Text = "OK", Left = 245, Top = 92, Width = 80, DialogResult = DialogResult.OK };
            var cancel = new Button { Text = "Cancel", Left = 332, Top = 92, Width = 80, DialogResult = DialogResult.Cancel };
            form.Controls.AddRange(new Control[] { prompt, input, ok, cancel });
            form.AcceptButton = ok;
            form.CancelButton = cancel;
            return form.ShowDialog(owner) == DialogResult.OK ? input.Text.Trim() : "";
        }
    }
}
