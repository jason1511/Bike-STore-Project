using System.Drawing;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public sealed class OnlineOverviewControl : UserControl
    {
        public OnlineOverviewControl()
        {
            Dock = DockStyle.Fill; BackColor = UiTheme.Canvas; Padding = new Padding(28);
            var card = new Panel { Dock = DockStyle.Top, Height = 210, BackColor = Color.White, Padding = new Padding(28) };
            var title = new Label { Text = Strings.Format("Online_Connected", AppServices.Profile.StoreName), AutoSize = true, Font = new Font("Segoe UI Semibold", 17F), ForeColor = UiTheme.Text, Dock = DockStyle.Top };
            var status = new Label { Text = Strings.Format("Online_Status", AppServices.Profile.ApiBaseUrl), AutoSize = true, Font = new Font("Segoe UI Semibold", 9F), ForeColor = UiTheme.Success, Dock = DockStyle.Top, Padding = new Padding(0, 8, 0, 0) };
            var body = new Label
            {
                Text = Strings.Get("Online_Explanation"),
                AutoSize = true, MaximumSize = new Size(760, 0), Font = new Font("Segoe UI", 10F), ForeColor = UiTheme.Muted, Dock = DockStyle.Top, Padding = new Padding(0, 18, 0, 0)
            };
            card.Controls.Add(body); card.Controls.Add(status); card.Controls.Add(title); Controls.Add(card); UiTheme.Apply(this);
        }
    }
}
