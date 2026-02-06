using System;
using System.Linq;
using System.Windows.Forms;

namespace Bike_STore_Project
{
    public partial class ServiceForm : Form
    {
        private readonly ProductRepository _repo = new();

        public ServiceForm()
        {
            InitializeComponent();

            // uppercase everywhere
            txtBrand.CharacterCasing = CharacterCasing.Upper;
            txtType.CharacterCasing = CharacterCasing.Upper;
            txtColor.CharacterCasing = CharacterCasing.Upper;

            // money-like input
            numServiceCost.DecimalPlaces = 2;
            numServiceCost.Minimum = 0;

            SetupAutocomplete();

            txtBrand.TextChanged += (s, e) =>
            {
                SetupTypeAutocomplete();
                SetupColorAutocomplete();
            };

            txtType.TextChanged += (s, e) =>
            {
                SetupColorAutocomplete();
            };

            btnAddService.Click += BtnAddService_Click;
            btnClear.Click += (s, e) => ClearInputs();
        }

        private void SetupAutocomplete()
        {
            txtBrand.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtBrand.AutoCompleteSource = AutoCompleteSource.CustomSource;

            txtType.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtType.AutoCompleteSource = AutoCompleteSource.CustomSource;

            txtColor.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            txtColor.AutoCompleteSource = AutoCompleteSource.CustomSource;

            SetupBrandAutocomplete();
        }

        private void SetupBrandAutocomplete()
        {
            var src = new AutoCompleteStringCollection();
            src.AddRange(_repo.GetDistinctBrands().ToArray());
            txtBrand.AutoCompleteCustomSource = src;
        }

        private void SetupTypeAutocomplete()
        {
            var src = new AutoCompleteStringCollection();

            if (!string.IsNullOrWhiteSpace(txtBrand.Text))
                src.AddRange(_repo.GetDistinctTypes(txtBrand.Text).ToArray());

            txtType.AutoCompleteCustomSource = src;
        }

        private void SetupColorAutocomplete()
        {
            var src = new AutoCompleteStringCollection();

            if (!string.IsNullOrWhiteSpace(txtBrand.Text) &&
                !string.IsNullOrWhiteSpace(txtType.Text))
            {
                src.AddRange(_repo.GetDistinctColors(txtBrand.Text, txtType.Text).ToArray());
            }

            txtColor.AutoCompleteCustomSource = src;
        }

        private void BtnAddService_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBrand.Text))
            {
                MessageBox.Show("Brand is required.");
                txtBrand.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtType.Text))
            {
                MessageBox.Show("Type is required.");
                txtType.Focus();
                return;
            }

            var brand = txtBrand.Text.Trim().ToUpperInvariant();
            var type = txtType.Text.Trim().ToUpperInvariant();
            var color = string.IsNullOrWhiteSpace(txtColor.Text) ? null : txtColor.Text.Trim().ToUpperInvariant();
            var notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim();
            var cost = (double)numServiceCost.Value;

            try
            {
                using var conn = Database.OpenConnection();
                using var tx = conn.BeginTransaction();

                long serviceId;

                // ✅ Insert service WITH actor fields
                using (var cmd = conn.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = @"
INSERT INTO services
(brand, type, color, quantity, service_cost, notes, created_by_user_id, created_by_username, created_at)
VALUES
($brand, $type, $color, 1, $cost, $notes, $uid, $uname, $createdAt);
SELECT last_insert_rowid();";

                    cmd.Parameters.AddWithValue("$brand", brand);
                    cmd.Parameters.AddWithValue("$type", type);
                    cmd.Parameters.AddWithValue("$color", (object?)color ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$cost", cost);
                    cmd.Parameters.AddWithValue("$notes", (object?)notes ?? DBNull.Value);

                    cmd.Parameters.AddWithValue("$uid", AppSession.UserId > 0 ? AppSession.UserId : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("$uname", string.IsNullOrWhiteSpace(AppSession.Username) ? (object)DBNull.Value : AppSession.Username);
                    cmd.Parameters.AddWithValue("$createdAt", DateTime.UtcNow.ToString("o"));

                    serviceId = (long)(cmd.ExecuteScalar() ?? 0L);
                }

                // ✅ Audit log
                using (var audit = conn.CreateCommand())
                {
                    audit.Transaction = tx;
                    audit.CommandText = @"
INSERT INTO audit_log (action, entity, entity_id, actor_user_id, actor_username, detail, created_at)
VALUES ($action, $entity, $entityId, $actorId, $actorUser, $detail, $at);";

                    audit.Parameters.AddWithValue("$action", "CREATE_SERVICE");
                    audit.Parameters.AddWithValue("$entity", "services");
                    audit.Parameters.AddWithValue("$entityId", serviceId);

                    audit.Parameters.AddWithValue("$actorId", AppSession.UserId > 0 ? AppSession.UserId : (object)DBNull.Value);
                    audit.Parameters.AddWithValue("$actorUser", string.IsNullOrWhiteSpace(AppSession.Username) ? (object)DBNull.Value : AppSession.Username);

                    audit.Parameters.AddWithValue("$detail",
                        $"brand={brand}, type={type}, color={(color ?? "")}, cost={cost}, notes={(notes ?? "")}");

                    audit.Parameters.AddWithValue("$at", DateTime.UtcNow.ToString("o"));

                    audit.ExecuteNonQuery();
                }

                tx.Commit();

                MessageBox.Show("Service saved!");
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save service: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txtBrand.Clear();
            txtType.Clear();
            txtColor.Clear();
            txtNotes.Clear();

            numServiceCost.Value = 0;

            SetupBrandAutocomplete();
            txtBrand.Focus();
        }
    }
}
