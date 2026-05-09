using System;
using System.Drawing;
using System.Windows.Forms;
using FinancialAnalyzer.Models;
using FinancialAnalyzer.Services;

namespace FinancialAnalyzer.Forms
{
    public partial class CreditForm : Form
    {
        private readonly Color _primaryColor = Color.FromArgb(52, 73, 94);
        private readonly Color _accentColor = Color.FromArgb(41, 128, 185);
        private readonly Color _textColor = Color.FromArgb(44, 62, 80);

        private ComboBox _cmbType;
        private TextBox _txtName;
        private TextBox _txtAmount;
        private TextBox _txtDownPayment;
        private TextBox _txtRate;
        private TextBox _txtTerm;
        private RadioButton _rbAnnuity;
        private RadioButton _rbDiff;
        private DateTimePicker _dtpOpen;
        private Label _lblError;
        private Label _lblCalc;

        public CreditModel Result { get; private set; }
        private CreditModel _existing;

        public CreditForm(CreditModel existing = null)
        {
            InitializeComponent();
            _existing = existing;
            if (existing != null) this.Text = "Редактировать кредит";
            SetupForm();
            if (existing != null) LoadData();
        }

        private void LoadData()
        {
            _txtName.Text = _existing.Name;
            _cmbType.SelectedIndex = (int)_existing.Type;
            _txtAmount.Text = _existing.TotalAmount.ToString("F0");
            _txtDownPayment.Text = _existing.DownPayment.ToString("F0");
            _txtRate.Text = _existing.InterestRate.ToString("F1");
            _txtTerm.Text = _existing.TermMonths.ToString();
            _rbAnnuity.Checked = _existing.PaymentType == CreditModel.PaymentTypeEnum.Annuity;
            _rbDiff.Checked = _existing.PaymentType == CreditModel.PaymentTypeEnum.Differentiated;
            _dtpOpen.Value = _existing.OpenDate;
        }

        private void SetupForm()
        {
            this.SuspendLayout();
            int left = 30, width = 450, y = 20;

            var lblTitle = new Label
            {
                Text = _existing != null ? "Редактирование кредита" : "Новый кредит",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = _primaryColor,
                Location = new Point(left, y),
                Size = new Size(width, 35)
            };
            y += 50;

            // Тип кредита
            AddLabel("Тип кредита:", left, y); y += 22;
            _cmbType = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(left, y),
                Size = new Size(width, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbType.Items.AddRange(new[] { "Ипотека", "Автокредит", "Потребительский", "Кредитная карта", "Другое" });
            _cmbType.SelectedIndex = 0;
            this.Controls.Add(_cmbType);
            y += 45;

            // Название
            AddLabel("Название кредита:", left, y); y += 22;
            _txtName = AddTextBox(left, y, width, "Например: Ипотека Сбер"); y += 45;

            // Сумма кредита
            AddLabel("Сумма кредита (₽):", left, y); y += 22;
            _txtAmount = AddTextBox(left, y, width, "5000000"); y += 45;

            // Первоначальный взнос
            AddLabel("Первоначальный взнос (₽, можно 0):", left, y); y += 22;
            _txtDownPayment = AddTextBox(left, y, width, "0"); y += 45;

            // Ставка
            AddLabel("Процентная ставка (% годовых):", left, y); y += 22;
            _txtRate = AddTextBox(left, y, width, "10.2"); y += 45;

            // Срок
            AddLabel("Срок (месяцев):", left, y); y += 22;
            _txtTerm = AddTextBox(left, y, width, "180"); y += 45;

            // Тип платежа
            AddLabel("Тип платежа:", left, y); y += 22;
            _rbAnnuity = new RadioButton
            {
                Text = "Аннуитетный",
                Font = new Font("Segoe UI", 10),
                ForeColor = _textColor,
                Location = new Point(left, y),
                Size = new Size(140, 25),
                Checked = true
            };
            _rbDiff = new RadioButton
            {
                Text = "Дифференцированный",
                Font = new Font("Segoe UI", 10),
                ForeColor = _textColor,
                Location = new Point(left + 170, y),
                Size = new Size(180, 25)
            };
            this.Controls.AddRange(new Control[] { _rbAnnuity, _rbDiff });
            y += 40;

            // Дата открытия
            AddLabel("Дата открытия:", left, y); y += 22;
            _dtpOpen = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10),
                Location = new Point(left, y),
                Size = new Size(width, 25),
                Value = DateTime.Now
            };
            this.Controls.Add(_dtpOpen);
            y += 45;

            // Расчёт
            var calcPanel = new Panel
            {
                Location = new Point(left, y),
                Size = new Size(width, 55),
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(10)
            };
            _lblCalc = new Label
            {
                Text = "Заполните поля для расчёта платежа",
                Font = new Font("Segoe UI", 9),
                ForeColor = _textColor,
                Dock = DockStyle.Fill
            };
            calcPanel.Controls.Add(_lblCalc);
            this.Controls.Add(calcPanel);
            y += 70;

            // Кнопка пересчёта
            var btnRecalc = new Button
            {
                Text = "🔄 Пересчитать платёж",
                Font = new Font("Segoe UI", 9),
                Location = new Point(left, y - 65),
                Size = new Size(180, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(236, 240, 241),
                ForeColor = _textColor,
                Cursor = Cursors.Hand
            };
            btnRecalc.FlatAppearance.BorderSize = 1;
            btnRecalc.FlatAppearance.BorderColor = Color.FromArgb(206, 212, 218);
            btnRecalc.Click += (s, e) => Recalculate();
            calcPanel.Controls.Add(btnRecalc);

            // Ошибка
            _lblError = new Label
            {
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(left, y),
                Size = new Size(width, 20)
            };
            this.Controls.Add(_lblError);
            y += 30;

            // Кнопки
            var btnSave = CreateButton("Сохранить", left, y, 215, _accentColor, Color.White);
            btnSave.Click += BtnSave_Click;
            var btnCancel = CreateButton("Отмена", left + 235, y, 215, Color.FromArgb(236, 240, 241), _textColor);
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.AddRange(new Control[] { btnSave, btnCancel });

            this.ResumeLayout(false);
        }

        private void Recalculate()
        {
            if (!decimal.TryParse(_txtAmount.Text, out decimal total) || total <= 0) return;
            if (!decimal.TryParse(_txtDownPayment.Text, out decimal down)) down = 0;
            if (down >= total) { _lblCalc.Text = "Взнос не может быть больше суммы кредита"; return; }
            if (!decimal.TryParse(_txtRate.Text, out decimal rate) || rate <= 0) return;
            if (!int.TryParse(_txtTerm.Text, out int months) || months <= 0) return;

            decimal loanAmount = total - down;

            if (_rbAnnuity.Checked)
            {
                decimal payment = CreditService.CalculateAnnuityPayment(loanAmount, rate, months);
                decimal totalPayment = payment * months;
                _lblCalc.Text = $"Ежемесячный платёж: {payment:N0} ₽\nПереплата: {totalPayment - loanAmount:N0} ₽ | Общая выплата: {totalPayment:N0} ₽";
            }
            else
            {
                decimal firstPayment = loanAmount / months + loanAmount * rate / 100m / 12m;
                decimal lastPayment = loanAmount / months + (loanAmount / months) * rate / 100m / 12m;
                _lblCalc.Text = $"Первый платёж: {firstPayment:N0} ₽\nПоследний платёж: {lastPayment:N0} ₽\nПлатежи уменьшаются ежемесячно";
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtName.Text) || _txtName.Text.Contains("Например"))
            { _lblError.Text = "Введите название кредита"; return; }
            if (!decimal.TryParse(_txtAmount.Text, out decimal total) || total <= 0)
            { _lblError.Text = "Введите сумму кредита"; return; }
            if (!decimal.TryParse(_txtDownPayment.Text, out decimal down)) down = 0;
            if (down >= total)
            { _lblError.Text = "Взнос не может быть больше суммы"; return; }
            if (!decimal.TryParse(_txtRate.Text, out decimal rate) || rate <= 0 || rate > 100)
            { _lblError.Text = "Введите ставку (0–100)"; return; }
            if (!int.TryParse(_txtTerm.Text, out int months) || months <= 0)
            { _lblError.Text = "Введите срок в месяцах"; return; }

            decimal loanAmount = total - down;
            decimal payment = _rbAnnuity.Checked
                ? CreditService.CalculateAnnuityPayment(loanAmount, rate, months)
                : loanAmount / months + loanAmount * rate / 100m / 12m;

            Result = new CreditModel
            {
                Id = _existing?.Id ?? 0,
                Name = _txtName.Text.Trim(),
                Type = (CreditModel.CreditTypeEnum)_cmbType.SelectedIndex,
                TotalAmount = total,
                DownPayment = down,
                InterestRate = rate,
                TermMonths = months,
                PaymentType = _rbAnnuity.Checked ? CreditModel.PaymentTypeEnum.Annuity : CreditModel.PaymentTypeEnum.Differentiated,
                OpenDate = _dtpOpen.Value,
                MonthlyPayment = Math.Round(payment, 0),
                RemainingDebt = loanAmount,
                PaidPrincipal = 0,
                PaidInterest = 0
            };

            _lblError.Text = "";
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void AddLabel(string text, int x, int y)
        {
            this.Controls.Add(new Label { Text = text, Font = new Font("Segoe UI", 10), ForeColor = _textColor, Location = new Point(x, y), Size = new Size(450, 20) });
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
            // CreditForm
            // 
            this.ClientSize = new System.Drawing.Size(520, 796);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CreditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);

        }
    }
}