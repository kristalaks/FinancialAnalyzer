using System;
using System.Drawing;
using System.Windows.Forms;
using FinancialAnalyzer.Models;
using FinancialAnalyzer.Services;

namespace FinancialAnalyzer.Forms
{
    public partial class DepositForm : Form
    {
        private readonly Color _primaryColor = Color.FromArgb(52, 73, 94);
        private readonly Color _accentColor = Color.FromArgb(41, 128, 185);
        private readonly Color _textColor = Color.FromArgb(44, 62, 80);
        private readonly Color _placeholderColor = Color.FromArgb(189, 195, 199);

        private TextBox _txtName;
        private TextBox _txtAmount;
        private TextBox _txtRate;
        private RadioButton _rbSimple;
        private RadioButton _rbCompound;
        private DateTimePicker _dtpOpen;
        private DateTimePicker _dtpClose;
        private CheckBox _chkNoCloseDate;
        private Label _lblCalculation;
        private Label _lblError;

        // Результат (заполняется при успешном сохранении)
        public DepositModel Result { get; private set; }

        // Если передан существующий вклад — это редактирование
        private DepositModel _existingDeposit;

        public DepositForm(DepositModel existingDeposit = null)
        {
            InitializeComponent();
            _existingDeposit = existingDeposit;

            if (existingDeposit != null)
            {
                this.Text = "Редактировать вклад";
                LoadDepositData(existingDeposit);
            }

            SetupForm();
        }

        private void SetupForm()
        {
            this.SuspendLayout();

            int leftMargin = 30;
            int fieldWidth = 420;
            int currentY = 25;

            // === Заголовок ===
            var lblTitle = new Label
            {
                Text = _existingDeposit != null ? "Редактирование вклада" : "Новый вклад",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = _primaryColor,
                Location = new Point(leftMargin, currentY),
                Size = new Size(fieldWidth, 35)
            };
            currentY += 55;

            // === Название ===
            AddLabel("Название вклада:", leftMargin, currentY);
            currentY += 22;

            _txtName = AddTextBox(leftMargin, currentY, fieldWidth, "Например: Сбербанк — Накопительный");
            currentY += 45;

            // === Начальная сумма ===
            AddLabel("Начальная сумма (₽):", leftMargin, currentY);
            currentY += 22;

            _txtAmount = AddTextBox(leftMargin, currentY, fieldWidth, "Например: 300000");
            currentY += 45;

            // === Процентная ставка ===
            AddLabel("Процентная ставка (% годовых):", leftMargin, currentY);
            currentY += 22;

            _txtRate = AddTextBox(leftMargin, currentY, fieldWidth, "Например: 14.5");
            currentY += 45;

            // === Тип ставки ===
            AddLabel("Тип ставки:", leftMargin, currentY);
            currentY += 22;

            _rbSimple = new RadioButton
            {
                Text = "Простой процент",
                Font = new Font("Segoe UI", 10),
                ForeColor = _textColor,
                Location = new Point(leftMargin, currentY),
                Size = new Size(180, 25),
                Checked = true
            };

            _rbCompound = new RadioButton
            {
                Text = "Сложный процент (капитализация)",
                Font = new Font("Segoe UI", 10),
                ForeColor = _textColor,
                Location = new Point(leftMargin + 200, currentY),
                Size = new Size(230, 25)
            };
            this.Controls.AddRange(new Control[] { _rbSimple, _rbCompound });
            currentY += 40;

            // === Дата открытия ===
            AddLabel("Дата открытия:", leftMargin, currentY);
            currentY += 22;

            _dtpOpen = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10),
                Location = new Point(leftMargin, currentY),
                Size = new Size(fieldWidth, 25),
                Value = DateTime.Now
            };
            this.Controls.Add(_dtpOpen);
            currentY += 45;

            // === Дата закрытия ===
            AddLabel("Дата закрытия:", leftMargin, currentY);
            currentY += 22;

            _dtpClose = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10),
                Location = new Point(leftMargin, currentY),
                Size = new Size(fieldWidth, 25),
                Value = DateTime.Now.AddYears(1)
            };

            _chkNoCloseDate = new CheckBox
            {
                Text = "Бессрочный",
                Font = new Font("Segoe UI", 9),
                ForeColor = _textColor,
                Location = new Point(leftMargin + fieldWidth + 10, currentY),
                Size = new Size(100, 25)
            };
            _chkNoCloseDate.CheckedChanged += (s, e) =>
            {
                _dtpClose.Enabled = !_chkNoCloseDate.Checked;
            };

            this.Controls.AddRange(new Control[] { _dtpClose, _chkNoCloseDate });
            currentY += 45;

            // === Панель предварительного расчёта ===
            var calcPanel = new Panel
            {
                Location = new Point(leftMargin, currentY),
                Size = new Size(fieldWidth, 55),
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(10)
            };

            _lblCalculation = new Label
            {
                Text = "Заполните поля для расчёта",
                Font = new Font("Segoe UI", 9),
                ForeColor = _textColor,
                Dock = DockStyle.Fill
            };
            calcPanel.Controls.Add(_lblCalculation);
            this.Controls.Add(calcPanel);
            currentY += 70;

            // === Ошибка ===
            _lblError = new Label
            {
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(leftMargin, currentY),
                Size = new Size(fieldWidth, 20),
                Text = ""
            };
            this.Controls.Add(_lblError);
            currentY += 30;

            // === Кнопки ===
            var btnSave = new Button
            {
                Text = "Сохранить",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(leftMargin, currentY),
                Size = new Size(200, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = _accentColor,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            var btnCancel = new Button
            {
                Text = "Отмена",
                Font = new Font("Segoe UI", 12),
                Location = new Point(leftMargin + 220, currentY),
                Size = new Size(200, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(236, 240, 241),
                ForeColor = _textColor,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] { btnSave, btnCancel });

            this.ResumeLayout(false);
        }

        private void LoadDepositData(DepositModel deposit)
        {
            _txtName = new TextBox { Text = deposit.Name };
            _txtAmount = new TextBox { Text = deposit.InitialAmount.ToString("F0") };
            _txtRate = new TextBox { Text = deposit.InterestRate.ToString("F1") };

            if (deposit.RateType == 0)
                _rbSimple.Checked = true;
            else
                _rbCompound.Checked = true;

            _dtpOpen.Value = deposit.OpenDate;
            if (deposit.CloseDate.HasValue)
            {
                _dtpClose.Value = deposit.CloseDate.Value;
                _chkNoCloseDate.Checked = false;
            }
            else
            {
                _chkNoCloseDate.Checked = true;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(_txtName.Text) || _txtName.Text.Contains("Например"))
            {
                ShowError("Введите название вклада");
                return;
            }

            if (!decimal.TryParse(_txtAmount.Text, out decimal amount) || amount <= 0)
            {
                ShowError("Введите корректную сумму (положительное число)");
                return;
            }

            if (!decimal.TryParse(_txtRate.Text, out decimal rate) || rate <= 0 || rate > 100)
            {
                ShowError("Введите корректную ставку (от 0 до 100)");
                return;
            }

            if (_dtpOpen.Value > DateTime.Now)
            {
                ShowError("Дата открытия не может быть в будущем");
                return;
            }

            if (!_chkNoCloseDate.Checked && _dtpClose.Value <= _dtpOpen.Value)
            {
                ShowError("Дата закрытия должна быть позже даты открытия");
                return;
            }

            // Создаём результат
            Result = new DepositModel
            {
                Id = _existingDeposit?.Id ?? 0,
                Name = _txtName.Text.Trim(),
                InitialAmount = amount,
                InterestRate = rate,
                RateType = _rbCompound.Checked ? 1 : 0,
                OpenDate = _dtpOpen.Value,
                CloseDate = _chkNoCloseDate.Checked ? (DateTime?)null : _dtpClose.Value,
                CurrentAmount = amount,  // для нового вклада = начальной сумме
                Profit = 0,
                ProfitPercent = 0
            };

            // Рассчитываем текущую сумму
            Result.CurrentAmount = DepositService.CalculateCurrentAmount(Result);
            Result.Profit = Result.CurrentAmount - Result.InitialAmount;
            if (Result.InitialAmount > 0)
                Result.ProfitPercent = Result.Profit / Result.InitialAmount * 100m;

            _lblError.Text = "";
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ShowError(string message)
        {
            _lblError.Text = message;
        }

        // Вспомогательные методы для создания контролов
        private void AddLabel(string text, int x, int y)
        {
            var label = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = _textColor,
                Location = new Point(x, y),
                Size = new Size(420, 20)
            };
            this.Controls.Add(label);
        }

        private TextBox AddTextBox(int x, int y, int width, string placeholder)
        {
            var textBox = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(x, y),
                Size = new Size(width, 25),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250),
                ForeColor = _textColor,
                Text = placeholder
            };

            textBox.Enter += (s, e) =>
            {
                if (textBox.Text == placeholder)
                {
                    textBox.Text = "";
                    textBox.ForeColor = _textColor;
                }
            };

            textBox.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholder;
                    textBox.ForeColor = _placeholderColor;
                }
            };

            this.Controls.Add(textBox);
            return textBox;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DepositForm
            // 
            this.ClientSize = new System.Drawing.Size(560, 633);
            this.Name = "DepositForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);

        }
    }
}