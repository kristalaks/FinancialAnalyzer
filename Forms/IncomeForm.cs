using System;
using System.Drawing;
using System.Windows.Forms;
using FinancialAnalyzer.Models;
using FinancialAnalyzer.Services;

namespace FinancialAnalyzer.Forms
{
    public partial class IncomeForm : Form
    {
        private readonly Color _primaryColor = Color.FromArgb(52, 73, 94);
        private readonly Color _accentColor = Color.FromArgb(41, 128, 185);
        private readonly Color _textColor = Color.FromArgb(44, 62, 80);

        private ComboBox _cmbSource;
        private TextBox _txtCustomName;
        private TextBox _txtAmount;
        private NumericUpDown _nudPaymentsPerMonth;
        private CheckBox _chkAfterTax;
        private DateTimePicker _dtpStartDate;
        private ComboBox _cmbTargetDeposit;
        private Label _lblError;
        private Label _lblCalc;

        public IncomeModel Result { get; private set; }
        private IncomeModel _existing;

        public IncomeForm(IncomeModel existing = null)
        {
            InitializeComponent();
            _existing = existing;
            if (existing != null) this.Text = "Редактировать доход";
            SetupForm();
            if (existing != null) LoadData();
        }

        private void LoadData()
        {
            _cmbSource.SelectedIndex = (int)_existing.Source;
            _txtCustomName.Text = _existing.CustomName ?? "";
            _txtAmount.Text = _existing.AmountPerPayment.ToString("F0");
            _nudPaymentsPerMonth.Value = _existing.PaymentsPerMonth;
            _chkAfterTax.Checked = _existing.IsAfterTax;
            _dtpStartDate.Value = _existing.StartDate;
            if (!string.IsNullOrEmpty(_existing.TargetDepositName))
            {
                for (int i = 0; i < _cmbTargetDeposit.Items.Count; i++)
                {
                    if (_cmbTargetDeposit.Items[i].ToString() == _existing.TargetDepositName)
                    {
                        _cmbTargetDeposit.SelectedIndex = i;
                        break;
                    }
                }
            }
            UpdateCalc();
        }

        private void SetupForm()
        {
            this.SuspendLayout();
            int left = 30, width = 420, y = 25;

            var lblTitle = new Label { Text = _existing != null ? "Редактирование дохода" : "Новый доход", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = _primaryColor, Location = new Point(left, y), Size = new Size(width, 35) };
            y += 50;

            // Источник
            AddLabel("Источник дохода:", left, y); y += 22;
            _cmbSource = new ComboBox { Font = new Font("Segoe UI", 10), Location = new Point(left, y), Size = new Size(width, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbSource.Items.AddRange(new[] { "Заработная плата", "Фриланс", "Аренда", "Инвестиции", "Другое" });
            _cmbSource.SelectedIndex = 0;
            _cmbSource.SelectedIndexChanged += (s, e) => { _txtCustomName.Visible = _cmbSource.SelectedIndex == 4; };
            this.Controls.Add(_cmbSource);
            y += 40;

            // Своё название (для "Другое")
            _txtCustomName = AddTextBox(left, y, width, "Название источника"); _txtCustomName.Visible = false; y += 30;

            // Сумма
            AddLabel("Сумма за одну выплату (₽, до налогов):", left, y); y += 22;
            _txtAmount = AddTextBox(left, y, width, "80000"); y += 45;

            // Выплат в месяц
            AddLabel("Количество выплат в месяц:", left, y); y += 22;
            _nudPaymentsPerMonth = new NumericUpDown { Font = new Font("Segoe UI", 10), Location = new Point(left, y), Size = new Size(width, 25), Minimum = 1, Maximum = 31, Value = 2 };
            this.Controls.Add(_nudPaymentsPerMonth);
            y += 45;

            // Налог
            _chkAfterTax = new CheckBox { Text = "Сумма указана после вычета налогов", Font = new Font("Segoe UI", 10), ForeColor = _textColor, Location = new Point(left, y), Size = new Size(width, 25), Checked = true };
            this.Controls.Add(_chkAfterTax);
            y += 35;
            _chkAfterTax.CheckedChanged += (s, e) => UpdateCalc();

            // Дата начала
            AddLabel("Дата начала поступлений:", left, y); y += 22;
            _dtpStartDate = new DateTimePicker { Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 10), Location = new Point(left, y), Size = new Size(width, 25), Value = DateTime.Now };
            this.Controls.Add(_dtpStartDate);
            y += 45;

            // Направление на вклад
            AddLabel("Направлять на накопительный счёт (опционально):", left, y); y += 22;
            _cmbTargetDeposit = new ComboBox { Font = new Font("Segoe UI", 10), Location = new Point(left, y), Size = new Size(270, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbTargetDeposit.Items.Add("Не направлять");
            var deposits = Services.DepositService.GetAll();
            foreach (var dep in deposits)
            {
                _cmbTargetDeposit.Items.Add(dep.Name);
            }
            _cmbTargetDeposit.SelectedIndex = 0;
            this.Controls.Add(_cmbTargetDeposit);
            y += 45;

            // Предрасчёт
            var calcPanel = new Panel { Location = new Point(left, y), Size = new Size(width, 35), BackColor = Color.FromArgb(248, 249, 250) };
            _lblCalc = new Label { Text = "Месячный доход: 0 ₽", Font = new Font("Segoe UI", 9), ForeColor = _textColor, Location = new Point(10, 8), Size = new Size(width - 20, 20) };
            calcPanel.Controls.Add(_lblCalc);
            this.Controls.Add(calcPanel);
            _txtAmount.TextChanged += (s, e) => UpdateCalc();
            _nudPaymentsPerMonth.ValueChanged += (s, e) => UpdateCalc();
            y += 50;

            // Ошибка
            _lblError = new Label { Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(231, 76, 60), Location = new Point(left, y), Size = new Size(width, 20) };
            this.Controls.Add(_lblError);
            y += 30;

            // Кнопки
            var btnSave = CreateButton("Сохранить", left, y, 200, _accentColor, Color.White);
            btnSave.Click += BtnSave_Click;
            var btnCancel = CreateButton("Отмена", left + 220, y, 200, Color.FromArgb(236, 240, 241), _textColor);
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] { btnSave, btnCancel });
            this.ResumeLayout(false);
        }

        private void UpdateCalc()
        {
            if (decimal.TryParse(_txtAmount.Text, out decimal amt))
            {
                int payments = (int)_nudPaymentsPerMonth.Value;
                decimal gross = amt * payments;
                decimal net = _chkAfterTax.Checked ? gross : gross * 0.87m;
                decimal tax = _chkAfterTax.Checked ? 0 : gross * 0.13m;

                _lblCalc.Text = _chkAfterTax.Checked
                    ? $"Месячный доход: {net:N0} ₽ (чистыми)   •   Годовой: {net * 12:N0} ₽"
                    : $"До налогов: {gross:N0} ₽   •   НДФЛ: {tax:N0} ₽   •   На руки: {net:N0} ₽/мес";
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(_txtAmount.Text, out decimal amt) || amt <= 0)
            { _lblError.Text = "Введите корректную сумму"; return; }

            Result = new IncomeModel
            {
                Id = _existing?.Id ?? 0,
                Source = (IncomeModel.IncomeSourceEnum)_cmbSource.SelectedIndex,
                CustomName = _cmbSource.SelectedIndex == 4 ? _txtCustomName.Text.Trim() : null,
                AmountPerPayment = amt,
                PaymentsPerMonth = (int)_nudPaymentsPerMonth.Value,
                IsAfterTax = _chkAfterTax.Checked,
                StartDate = _dtpStartDate.Value,
                TargetDepositId = _cmbTargetDeposit.SelectedIndex > 0 ? _cmbTargetDeposit.SelectedIndex : (int?)null,
                TargetDepositName = _cmbTargetDeposit.SelectedIndex > 0 ? _cmbTargetDeposit.SelectedItem.ToString() : null
            };

            _lblError.Text = "";
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void AddLabel(string text, int x, int y)
        {
            this.Controls.Add(new Label { Text = text, Font = new Font("Segoe UI", 10), ForeColor = _textColor, Location = new Point(x, y), Size = new Size(420, 20) });
        }

        private TextBox AddTextBox(int x, int y, int w, string placeholder)
        {
            var tb = new TextBox { Font = new Font("Segoe UI", 10), Location = new Point(x, y), Size = new Size(w, 25), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(248, 249, 250), ForeColor = _textColor, Text = placeholder };
            tb.Enter += (s, ev) => { if (tb.Text == placeholder) { tb.Text = ""; tb.ForeColor = _textColor; } };
            tb.Leave += (s, ev) => { if (string.IsNullOrWhiteSpace(tb.Text)) { tb.Text = placeholder; tb.ForeColor = Color.FromArgb(189, 195, 199); } };
            this.Controls.Add(tb);
            return tb;
        }

        private Button CreateButton(string text, int x, int y, int w, Color back, Color fore)
        {
            var btn = new Button { Text = text, Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(x, y), Size = new Size(w, 40), FlatStyle = FlatStyle.Flat, BackColor = back, ForeColor = fore, Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // IncomeForm
            // 
            this.ClientSize = new System.Drawing.Size(482, 655);
            this.Name = "IncomeForm";
            this.ResumeLayout(false);

        }
    }
}