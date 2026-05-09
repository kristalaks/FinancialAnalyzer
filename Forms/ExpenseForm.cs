using System;
using System.Drawing;
using System.Windows.Forms;
using FinancialAnalyzer.Models;

namespace FinancialAnalyzer.Forms
{
    public partial class ExpenseForm : Form
    {
        private readonly Color _primaryColor = Color.FromArgb(52, 73, 94);
        private readonly Color _accentColor = Color.FromArgb(41, 128, 185);
        private readonly Color _textColor = Color.FromArgb(44, 62, 80);

        private ComboBox _cmbCategory;
        private TextBox _txtCustomCategory;
        private TextBox _txtName;
        private TextBox _txtAmount;
        private ComboBox _cmbPeriod;
        private DateTimePicker _dtpDate;
        private TextBox _txtNote;
        private Label _lblError;

        public ExpenseModel Result { get; private set; }
        private ExpenseModel _existing;

        // Стандартные категории
        private readonly string[] _standardCategories = {
            "Продукты", "ЖКХ", "Транспорт", "Связь/Интернет",
            "Одежда", "Здоровье", "Развлечения", "Образование",
            "Налоги", "Кредиты", "Создать свою категорию..."
        };

        public ExpenseForm(ExpenseModel existing = null)
        {
            InitializeComponent();
            _existing = existing;
            if (existing != null) this.Text = "Редактировать расход";
            SetupForm();
            if (existing != null) LoadData();
        }

        private void LoadData()
        {
            if (_existing.Category == ExpenseModel.ExpenseCategoryEnum.Custom)
            {
                _cmbCategory.SelectedIndex = 10; // "Создать свою..."
                _txtCustomCategory.Visible = true;
                _txtCustomCategory.Text = _existing.CustomCategoryName;
            }
            else
            {
                _cmbCategory.SelectedIndex = (int)_existing.Category;
            }
            _txtName.Text = _existing.Name;
            _txtAmount.Text = _existing.Amount.ToString("F0");
            _cmbPeriod.SelectedIndex = (int)_existing.Period;
            _dtpDate.Value = _existing.Date;
            _txtNote.Text = _existing.Note ?? "";
        }

        private void SetupForm()
        {
            this.SuspendLayout();
            int left = 30, width = 420, y = 25;

            var lblTitle = new Label { Text = _existing != null ? "Редактирование расхода" : "Новый расход", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = _primaryColor, Location = new Point(left, y), Size = new Size(width, 35) };
            y += 50;

            // Категория
            AddLabel("Категория:", left, y); y += 22;
            _cmbCategory = new ComboBox { Font = new Font("Segoe UI", 10), Location = new Point(left, y), Size = new Size(width, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbCategory.Items.AddRange(_standardCategories);
            _cmbCategory.SelectedIndex = 0;
            _cmbCategory.SelectedIndexChanged += (s, e) =>
            {
                bool isCustom = _cmbCategory.SelectedIndex == 10;
                _txtCustomCategory.Visible = isCustom;
                if (!isCustom && string.IsNullOrEmpty(_txtName.Text.Contains("Например") ? "" : _txtName.Text))
                    _txtName.Text = _cmbCategory.SelectedItem.ToString();
            };
            this.Controls.Add(_cmbCategory);
            y += 40;

            // Своя категория
            _txtCustomCategory = AddTextBox(left, y, width, "Название категории");
            _txtCustomCategory.Visible = false;
            y += 30;

            // Название расхода
            AddLabel("Название расхода:", left, y); y += 22;
            _txtName = AddTextBox(left, y, width, "Например: Продукты на неделю");
            y += 45;

            // Сумма
            AddLabel("Сумма (₽):", left, y); y += 22;
            _txtAmount = AddTextBox(left, y, width, "15000");
            y += 45;

            // Период
            AddLabel("Периодичность:", left, y); y += 22;
            _cmbPeriod = new ComboBox { Font = new Font("Segoe UI", 10), Location = new Point(left, y), Size = new Size(width, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbPeriod.Items.AddRange(new[] { "Разовый", "Ежемесячный", "Ежегодный" });
            _cmbPeriod.SelectedIndex = 1;
            this.Controls.Add(_cmbPeriod);
            y += 45;

            // Дата
            AddLabel("Дата:", left, y); y += 22;
            _dtpDate = new DateTimePicker { Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 10), Location = new Point(left, y), Size = new Size(width, 25), Value = DateTime.Now };
            this.Controls.Add(_dtpDate);
            y += 45;

            // Примечание
            AddLabel("Примечание (необязательно):", left, y); y += 22;
            _txtNote = AddTextBox(left, y, width, "");
            y += 45;

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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (_cmbCategory.SelectedIndex == 10 && string.IsNullOrWhiteSpace(_txtCustomCategory.Text))
            { _lblError.Text = "Введите название своей категории"; return; }
            if (string.IsNullOrWhiteSpace(_txtName.Text) || _txtName.Text.Contains("Например"))
            { _lblError.Text = "Введите название расхода"; return; }
            if (!decimal.TryParse(_txtAmount.Text, out decimal amt) || amt <= 0)
            { _lblError.Text = "Введите корректную сумму"; return; }

            bool isCustom = _cmbCategory.SelectedIndex == 10;

            Result = new ExpenseModel
            {
                Id = _existing?.Id ?? 0,
                Category = isCustom ? ExpenseModel.ExpenseCategoryEnum.Custom : (ExpenseModel.ExpenseCategoryEnum)_cmbCategory.SelectedIndex,
                CustomCategoryName = isCustom ? _txtCustomCategory.Text.Trim() : null,
                Name = _txtName.Text.Trim(),
                Amount = amt,
                Period = (ExpenseModel.ExpensePeriodEnum)_cmbPeriod.SelectedIndex,
                Date = _dtpDate.Value,
                Note = string.IsNullOrWhiteSpace(_txtNote.Text) ? null : _txtNote.Text.Trim()
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
            // ExpenseForm
            // 
            this.ClientSize = new System.Drawing.Size(478, 599);
            this.Name = "ExpenseForm";
            this.ResumeLayout(false);

        }
    }
}