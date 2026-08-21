using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public sealed class InvoiceManagementForm : Form
    {
        private readonly LocalAdminRepository _repo = new();
        private readonly BindingList<InvoiceDraftItem> _cart = new();
        private readonly DataGridView _products = Grid();
        private readonly DataGridView _items = Grid();
        private readonly DataGridView _history = Grid();
        private readonly TextBox _customer = new() { Width = 190 };
        private readonly TextBox _phone = new() { Width = 150 };
        private readonly TextBox _address = new() { Width = 250 };
        private readonly TextBox _notes = new() { Width = 230 };
        private readonly TextBox _frames = new() { Width = 220, Multiline = true, Height = 50 };
        private readonly TextBox _search = new() { Width = 220 };
        private readonly NumericUpDown _qty = new() { Minimum = 1, Maximum = 999, Width = 75 };
        private readonly NumericUpDown _price = new() { Minimum = 1, Maximum = 1_000_000_000, ThousandsSeparator = true, Width = 135 };
        private readonly ComboBox _payment = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 135 };
        private readonly ComboBox _bank = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 110 };
        private readonly Label _total = new() { AutoSize = true, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
        private InvoiceHeader? _printInvoice;

        public InvoiceManagementForm()
        {
            Text = $"Bike Store - Invoices - {AppSession.Username} ({AppSession.Role})";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1100, 700);

            var menu = new MainMenuControl { Dock = DockStyle.Top };
            var tabs = new TabControl { Dock = DockStyle.Fill };
            tabs.TabPages.Add(BuildCreateTab());
            tabs.TabPages.Add(BuildHistoryTab());
            Controls.Add(tabs);
            Controls.Add(menu);

            _payment.Items.AddRange(new object[] { "CASH", "BANK TRANSFER" });
            _payment.SelectedIndex = 0;
            _bank.Items.AddRange(new object[] { "", "BRI", "BNI", "BCA", "OTHER" });
            _bank.SelectedIndex = 0;
            _payment.SelectedIndexChanged += (_, __) => _bank.Enabled = _payment.Text == "BANK TRANSFER";
            _bank.Enabled = false;

            _items.AutoGenerateColumns = true;
            _items.DataSource = _cart;
            _products.SelectionChanged += (_, __) => ApplySelectedPrice();
            _cart.ListChanged += (_, __) => UpdateTotal();
            _search.TextChanged += (_, __) => LoadHistory();
            Load += (_, __) => { LoadProducts(); LoadHistory(); };
        }

        private TabPage BuildCreateTab()
        {
            var tab = new TabPage("New Invoice");
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4, ColumnCount = 1, Padding = new Padding(12) };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 48));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var customerPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, WrapContents = true };
            AddField(customerPanel, "Customer *", _customer);
            AddField(customerPanel, "Phone", _phone);
            AddField(customerPanel, "Address", _address);
            AddField(customerPanel, "Payment", _payment);
            AddField(customerPanel, "Bank", _bank);
            AddField(customerPanel, "Notes", _notes);

            var productPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            productPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68));
            productPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
            _products.AutoGenerateColumns = true;
            productPanel.Controls.Add(_products, 0, 0);
            var addPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(12), AutoScroll = true };
            addPanel.Controls.Add(new Label { Text = "Selected bicycle", Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true });
            addPanel.Controls.Add(new Label { Text = "Quantity", AutoSize = true });
            addPanel.Controls.Add(_qty);
            addPanel.Controls.Add(new Label { Text = "Unit selling price", AutoSize = true });
            addPanel.Controls.Add(_price);
            addPanel.Controls.Add(new Label { Text = "Frame numbers (one per line, optional)", AutoSize = true });
            addPanel.Controls.Add(_frames);
            var add = new Button { Text = "Add item", Width = 130, Height = 34 };
            add.Click += (_, __) => AddItem();
            addPanel.Controls.Add(add);
            productPanel.Controls.Add(addPanel, 1, 0);

            var cartPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            cartPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            cartPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            cartPanel.Controls.Add(_items, 0, 0);
            var cartActions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            var remove = new Button { Text = "Remove selected" };
            remove.Click += (_, __) => RemoveItem();
            cartActions.Controls.Add(remove);
            cartActions.Controls.Add(_total);
            cartPanel.Controls.Add(cartActions, 0, 1);

            var savePanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.RightToLeft };
            var save = new Button { Text = "Save & print invoice", Width = 180, Height = 40 };
            save.Click += (_, __) => SaveInvoice(print: true);
            var saveOnly = new Button { Text = "Save invoice", Width = 130, Height = 40 };
            saveOnly.Click += (_, __) => SaveInvoice(print: false);
            savePanel.Controls.Add(save);
            savePanel.Controls.Add(saveOnly);

            root.Controls.Add(customerPanel, 0, 0);
            root.Controls.Add(productPanel, 0, 1);
            root.Controls.Add(cartPanel, 0, 2);
            root.Controls.Add(savePanel, 0, 3);
            tab.Controls.Add(root);
            return tab;
        }

        private TabPage BuildHistoryTab()
        {
            var tab = new TabPage("Invoice History");
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, Padding = new Padding(12) };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            actions.Controls.Add(new Label { Text = "Search", AutoSize = true, Padding = new Padding(0, 7, 0, 0) });
            actions.Controls.Add(_search);
            var refresh = new Button { Text = "Refresh" };
            refresh.Click += (_, __) => LoadHistory();
            var print = new Button { Text = "Print selected" };
            print.Click += (_, __) => PrintSelected();
            var voidButton = new Button { Text = "Void & restore stock", Enabled = AppSession.IsAdmin, AutoSize = true };
            voidButton.Click += (_, __) => VoidSelected();
            actions.Controls.Add(refresh);
            actions.Controls.Add(print);
            actions.Controls.Add(voidButton);
            root.Controls.Add(actions, 0, 0);
            root.Controls.Add(_history, 0, 1);
            tab.Controls.Add(root);
            return tab;
        }

        private void LoadProducts()
        {
            _products.DataSource = _repo.GetAvailableProducts();
            if (_products.Columns.Contains("id")) _products.Columns["id"].Visible = false;
            if (_products.Columns.Contains("sell_price")) _products.Columns["sell_price"].DefaultCellStyle.Format = "N0";
            ApplySelectedPrice();
        }

        private void ApplySelectedPrice()
        {
            if (_products.CurrentRow?.DataBoundItem is not DataRowView row) return;
            var value = Convert.ToDecimal(row["sell_price"]);
            if (value >= _price.Minimum && value <= _price.Maximum) _price.Value = value;
        }

        private void LoadHistory()
        {
            _history.DataSource = _repo.GetInvoices(_search.Text);
            if (_history.Columns.Contains("id")) _history.Columns["id"].Visible = false;
            if (_history.Columns.Contains("total"))
            {
                _history.Columns["total"].DefaultCellStyle.Format = "C0";
                _history.Columns["total"].DefaultCellStyle.FormatProvider = CultureInfo.GetCultureInfo("id-ID");
            }
        }

        private void AddItem()
        {
            if (_products.CurrentRow?.DataBoundItem is not DataRowView row)
            {
                MessageBox.Show("Select an available bicycle first.");
                return;
            }
            var available = Convert.ToInt32(row["available"]);
            var productId = Convert.ToInt32(row["id"]);
            var already = _cart.Where(x => x.ProductId == productId).Sum(x => x.Quantity);
            var quantity = (int)_qty.Value;
            if (quantity + already > available)
            {
                MessageBox.Show($"Only {available - already} more unit(s) are available.");
                return;
            }
            var frames = _frames.Lines.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
            if (frames.Length > quantity)
            {
                MessageBox.Show("Frame-number count cannot exceed item quantity.");
                return;
            }
            _cart.Add(new InvoiceDraftItem
            {
                ProductId = productId,
                Brand = Convert.ToString(row["brand"]) ?? "",
                Type = Convert.ToString(row["type"]) ?? "",
                Color = Convert.ToString(row["color"]),
                Quantity = quantity,
                UnitPrice = _price.Value,
                FrameNumbers = string.Join(Environment.NewLine, frames)
            });
            _frames.Clear();
        }

        private void RemoveItem()
        {
            if (_items.CurrentRow?.DataBoundItem is InvoiceDraftItem item) _cart.Remove(item);
        }

        private void SaveInvoice(bool print)
        {
            try
            {
                var number = _repo.CreateInvoice(_customer.Text, _phone.Text, _address.Text,
                    _payment.Text, _bank.Text, _notes.Text, _cart.ToList());
                MessageBox.Show($"Invoice {number} saved.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _printInvoice = _repo.GetInvoice(FindInvoiceId(number));
                if (print) ShowPrintPreview();
                ClearDraft();
                LoadProducts();
                LoadHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Invoice not saved", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private int FindInvoiceId(string number)
        {
            using var conn = Database.OpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT id FROM invoices WHERE invoice_number=$number;";
            cmd.Parameters.AddWithValue("$number", number);
            return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
        }

        private void VoidSelected()
        {
            var id = SelectedInvoiceId();
            if (id == 0) return;
            var reason = DialogPrompt.Show(this, "Void invoice", "Reason for voiding this invoice:", "Customer cancellation");
            if (string.IsNullOrWhiteSpace(reason)) return;
            if (MessageBox.Show("Void this invoice and restore all stock?", "Confirm void",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try
            {
                _repo.VoidInvoice(id, reason);
                LoadHistory();
                LoadProducts();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Void failed"); }
        }

        private void PrintSelected()
        {
            var id = SelectedInvoiceId();
            if (id == 0) return;
            _printInvoice = _repo.GetInvoice(id);
            ShowPrintPreview();
        }

        private int SelectedInvoiceId()
        {
            if (_history.CurrentRow?.DataBoundItem is not DataRowView row)
            {
                MessageBox.Show("Select an invoice first.");
                return 0;
            }
            return Convert.ToInt32(row["id"]);
        }

        private void ShowPrintPreview()
        {
            if (_printInvoice == null) return;
            var document = new PrintDocument { DocumentName = _printInvoice.InvoiceNumber };
            document.DefaultPageSettings.PaperSize = new PaperSize("A5", 583, 827);
            document.PrintPage += PrintInvoice;
            using var preview = new PrintPreviewDialog { Document = document, Width = 1000, Height = 750 };
            preview.ShowDialog(this);
        }

        private void PrintInvoice(object? sender, PrintPageEventArgs e)
        {
            if (_printInvoice == null || e.Graphics == null) return;
            var g = e.Graphics;
            var left = e.MarginBounds.Left;
            var right = e.MarginBounds.Right;
            var y = e.MarginBounds.Top;
            using var title = new Font("Segoe UI", 15, FontStyle.Bold);
            using var heading = new Font("Segoe UI", 9, FontStyle.Bold);
            using var body = new Font("Segoe UI", 8.5f);
            g.DrawString("CV NIAGA BERSAMA ABADI", title, Brushes.Black, left, y);
            y += 28;
            g.DrawString("FAKTUR PENJUALAN", heading, Brushes.Black, left, y);
            g.DrawString(_printInvoice.InvoiceNumber, heading, Brushes.Black, right - 140, y);
            y += 24;
            g.DrawString($"Tanggal: {_printInvoice.CreatedAt:dd MMM yyyy HH:mm}", body, Brushes.Black, left, y);
            y += 18;
            g.DrawString($"Pelanggan: {_printInvoice.CustomerName}   Tel: {_printInvoice.CustomerPhone}", body, Brushes.Black, left, y);
            y += 18;
            if (!string.IsNullOrWhiteSpace(_printInvoice.CustomerAddress))
            {
                g.DrawString($"Alamat: {_printInvoice.CustomerAddress}", body, Brushes.Black, left, y);
                y += 18;
            }
            g.DrawLine(Pens.Black, left, y, right, y);
            y += 8;
            foreach (var item in _printInvoice.Items)
            {
                g.DrawString($"{item.Brand} {item.Type} {item.Color}", heading, Brushes.Black, left, y);
                y += 17;
                g.DrawString($"{item.Quantity} x {item.UnitPrice:N0}", body, Brushes.Black, left + 12, y);
                g.DrawString($"Rp {item.LineTotal:N0}", body, Brushes.Black, right - 110, y);
                y += 17;
                if (!string.IsNullOrWhiteSpace(item.FrameNumbers))
                {
                    g.DrawString($"No. rangka: {item.FrameNumbers.Replace(Environment.NewLine, ", ")}", body, Brushes.Black, left + 12, y);
                    y += 17;
                }
            }
            g.DrawLine(Pens.Black, left, y, right, y);
            y += 8;
            g.DrawString("TOTAL", heading, Brushes.Black, left, y);
            g.DrawString($"Rp {_printInvoice.Total:N0}", title, Brushes.Black, right - 150, y);
            y += 28;
            g.DrawString($"Pembayaran: {_printInvoice.PaymentMethod} {_printInvoice.PaymentBank}".Trim(), body, Brushes.Black, left, y);
            y += 18;
            g.DrawString($"Dibuat oleh: {_printInvoice.CreatedBy}    Status: {_printInvoice.Status}", body, Brushes.Black, left, y);
        }

        private void ClearDraft()
        {
            _cart.Clear();
            _customer.Clear(); _phone.Clear(); _address.Clear(); _notes.Clear(); _frames.Clear();
            _payment.SelectedIndex = 0; _bank.SelectedIndex = 0; _qty.Value = 1; _price.Value = 1;
        }

        private void UpdateTotal() => _total.Text = $"Total: Rp {_cart.Sum(x => x.LineTotal):N0}";

        private static void AddField(FlowLayoutPanel panel, string label, Control control)
        {
            var box = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true, Margin = new Padding(5) };
            box.Controls.Add(new Label { Text = label, AutoSize = true });
            box.Controls.Add(control);
            panel.Controls.Add(box);
        }

        private static DataGridView Grid() => new()
        {
            Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
            AllowUserToDeleteRows = false, MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false
        };
    }
}
