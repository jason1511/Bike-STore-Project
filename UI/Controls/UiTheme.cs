using System.Drawing;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    internal static class UiTheme
    {
        public static readonly Color Sidebar = Color.FromArgb(31, 51, 51);
        public static readonly Color SidebarHover = Color.FromArgb(47, 79, 79);
        public static readonly Color Canvas = Color.FromArgb(245, 247, 246);
        public static readonly Color Card = Color.White;
        public static readonly Color Border = Color.FromArgb(218, 225, 222);
        public static readonly Color Text = Color.FromArgb(31, 42, 41);
        public static readonly Color Muted = Color.FromArgb(101, 116, 112);
        public static readonly Color Accent = Color.FromArgb(34, 112, 147);
        public static readonly Color Success = Color.FromArgb(38, 132, 86);
        public static readonly Color Warning = Color.FromArgb(190, 124, 25);
        public static readonly Color Danger = Color.FromArgb(185, 55, 55);

        public static void Apply(Control root)
        {
            root.Font = new Font("Segoe UI", 9F);
            if (root.Tag?.ToString() == "card") root.BackColor = Card;
            else if (root is Form or UserControl or TabPage or TableLayoutPanel or FlowLayoutPanel) root.BackColor = Canvas;

            foreach (Control control in root.Controls)
            {
                switch (control)
                {
                    case DataGridView grid:
                        StyleGrid(grid);
                        break;
                    case Button button when button.Tag?.ToString() is not ("nav" or "primary"):
                        var destructive = button.Text.Contains("delete", System.StringComparison.OrdinalIgnoreCase) ||
                                          button.Text.Contains("void", System.StringComparison.OrdinalIgnoreCase) ||
                                          button.Text.Contains("deactivate", System.StringComparison.OrdinalIgnoreCase);
                        StyleButton(button, destructive);
                        break;
                    case TextBox textBox:
                        textBox.BackColor = Color.White;
                        textBox.ForeColor = Text;
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        break;
                    case ComboBox combo:
                        combo.BackColor = Color.White;
                        combo.ForeColor = Text;
                        combo.FlatStyle = FlatStyle.Flat;
                        break;
                    case NumericUpDown number:
                        number.BackColor = Color.White;
                        number.ForeColor = Text;
                        number.BorderStyle = BorderStyle.FixedSingle;
                        break;
                    case Label label:
                        label.ForeColor = label.Tag?.ToString() switch
                        {
                            "muted" => Muted,
                            "accent" => Accent,
                            "light" => Color.White,
                            _ => Text
                        };
                        break;
                    case TabControl tabs:
                        tabs.Padding = new Point(18, 7);
                        break;
                }
                Apply(control);
            }
        }

        public static void StyleButton(Button button, bool destructive = false)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = destructive ? Danger : Border;
            button.BackColor = destructive ? Color.FromArgb(255, 242, 242) : Color.White;
            button.ForeColor = destructive ? Danger : Text;
            button.Cursor = Cursors.Hand;
            button.Padding = new Padding(8, 0, 8, 0);
        }

        public static void StyleGrid(DataGridView grid)
        {
            grid.BackgroundColor = Card;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Border;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(235, 240, 238);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Text;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9F);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 240, 238);
            grid.ColumnHeadersHeight = 42;
            grid.RowTemplate.Height = 40;
            grid.DefaultCellStyle.BackColor = Card;
            grid.DefaultCellStyle.ForeColor = Text;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(218, 235, 242);
            grid.DefaultCellStyle.SelectionForeColor = Text;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 251, 250);
            grid.CellFormatting -= GridCellFormatting;
            grid.CellFormatting += GridCellFormatting;
            foreach (DataGridViewColumn column in grid.Columns)
            {
                var name = column.Name.ToLowerInvariant();
                if (name.Contains("price") || name.Contains("cost") || name.Contains("revenue") ||
                    name.Contains("profit") || name == "total" || name.Contains("sell_price"))
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                if (name.Contains("quantity") || name is "stock" or "available")
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private static void GridCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (sender is not DataGridView grid || e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var name = grid.Columns[e.ColumnIndex].Name.ToLowerInvariant();
            var value = e.Value?.ToString()?.ToUpperInvariant() ?? "";
            if (name.Contains("status"))
            {
                if (value is "ACTIVE" or "COMPLETED") SetBadge(e, Color.FromArgb(226, 246, 235), Success);
                else if (value is "VOID" or "CANCELLED") SetBadge(e, Color.FromArgb(255, 232, 232), Danger);
                else SetBadge(e, Color.FromArgb(255, 246, 224), Warning);
            }
            else if (name == "role")
                SetBadge(e, value == "ADMIN" ? Color.FromArgb(229, 237, 251) : Color.FromArgb(237, 241, 240), value == "ADMIN" ? Accent : Muted);
            else if (name == "is_active")
                SetBadge(e, value is "1" or "TRUE" ? Color.FromArgb(226, 246, 235) : Color.FromArgb(255, 232, 232), value is "1" or "TRUE" ? Success : Danger);
        }

        private static void SetBadge(DataGridViewCellFormattingEventArgs e, Color background, Color foreground)
        {
            e.CellStyle.BackColor = background;
            e.CellStyle.ForeColor = foreground;
            e.CellStyle.SelectionBackColor = background;
            e.CellStyle.SelectionForeColor = foreground;
            e.CellStyle.Font = new Font("Segoe UI Semibold", 8.5F);
        }
    }
}
