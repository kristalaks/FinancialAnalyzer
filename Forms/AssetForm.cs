using System;
using System.Drawing;
using System.Windows.Forms;
using FinancialAnalyzer.Models;

namespace FinancialAnalyzer.Forms
{
    public partial class AssetForm : Form
    {
        private readonly Color _primaryColor = Color.FromArgb(52, 73, 94);
        private readonly Color _accentColor = Color.FromArgb(41, 128, 185);
        private readonly Color _textColor = Color.FromArgb(44, 62, 80);

        private TextBox _txtTicker;
        private TextBox _txtCompany;
        private ComboBox _cmbExchange;
        private TextBox _txtQuantity;
        private TextBox _txtPrice;
        private DateTimePicker _dtpPurchase;
        private Label _lblError;
        private Label _lblCalculation;

        private AssetModel.AssetTypeEnum _assetType;
        private AssetModel _existingAsset;

        public AssetModel Result { get; private set; }

        public AssetForm(AssetModel.AssetTypeEnum assetType)
        {
            InitializeComponent();
            _assetType = assetType;
            this.Text = $"Добавить актив ({GetTypeName()})";
            SetupForm();
        }

        public AssetForm(AssetModel existingAsset)
        {
            InitializeComponent();
            _existingAsset = existingAsset;
            _assetType = existingAsset.Type;
            this.Text = $"Редактировать актив ({GetTypeName()})";
            SetupForm();
            LoadExistingData();
        }

        private string GetTypeName()
        {
            switch (_assetType)
            {
                case AssetModel.AssetTypeEnum.Stock: return "Акции";
                case AssetModel.AssetTypeEnum.Currency: return "Валюта";
                case AssetModel.AssetTypeEnum.Metal: return "Металлы";
                default: return "Актив";
            }
        }

        private void LoadExistingData()
        {
            if (_existingAsset == null) return;
            _txtTicker.Text = _existingAsset.Ticker;
            _txtCompany.Text = _existingAsset.CompanyName;
            _cmbExchange.Text = _existingAsset.Exchange;
            _txtQuantity.Text = _existingAsset.Quantity.ToString("F0");
            _txtPrice.Text = _existingAsset.PurchasePrice.ToString("F2");
            _dtpPurchase.Value = _existingAsset.PurchaseDate;
        }

        private void SetupForm()
        {
            this.SuspendLayout();

            int leftMargin = 30;
            int fieldWidth = 440;
            int currentY = 25;

            var lblTitle = new Label
            {
                Text = _existingAsset != null ? $"Редактирование: {_existingAsset.Ticker}" : $"Новый актив: {GetTypeName()}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = _primaryColor,
                Location = new Point(leftMargin, currentY),
                Size = new Size(fieldWidth, 35)
            };
            currentY += 55;

            // Тикер
            AddLabel("Тикер (краткий код):", leftMargin, currentY);
            currentY += 22;
            _txtTicker = AddTextBox(leftMargin, currentY, fieldWidth, "SBER, AAPL, USD/RUB");
            currentY += 45;

            // Компания
            AddLabel("Название компании:", leftMargin, currentY);
            currentY += 22;
            _txtCompany = AddTextBox(leftMargin, currentY, fieldWidth, "Сбербанк, Apple Inc.");
            currentY += 45;

            // Биржа
            AddLabel("Биржа:", leftMargin, currentY);
            currentY += 22;
            _cmbExchange = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(leftMargin, currentY),
                Size = new Size(fieldWidth, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbExchange.Items.AddRange(new[] { "MOEX", "NASDAQ", "NYSE", "Forex", "Другая" });
            _cmbExchange.SelectedIndex = 0;
            this.Controls.Add(_cmbExchange);
            currentY += 45;

            // Количество
            AddLabel("Количество (шт./ед.):", leftMargin, currentY);
            currentY += 22;
            _txtQuantity = AddTextBox(leftMargin, currentY, fieldWidth, "100");
            currentY += 45;

            // Цена покупки
            AddLabel("Цена покупки за единицу (₽):", leftMargin, currentY);
            currentY += 22;
            _txtPrice = AddTextBox(leftMargin, currentY, fieldWidth, "254.00");
            currentY += 45;

            // Дата покупки
            AddLabel("Дата покупки:", leftMargin, currentY);
            currentY += 22;
            _dtpPurchase = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10),
                Location = new Point(leftMargin, currentY),
                Size = new Size(fieldWidth, 25),
                Value = DateTime.Now
            };
            this.Controls.Add(_dtpPurchase);
            currentY += 45;

            // Предварительный расчёт
            var calcPanel = new Panel
            {
                Location = new Point(leftMargin, currentY),
                Size = new Size(fieldWidth, 40),
                BackColor = Color.FromArgb(248, 249, 250)
            };
            _lblCalculation = new Label
            {
                Text = "Заполните поля для расчёта",
                Font = new Font("Segoe UI", 9),
                ForeColor = _textColor,
                Location = new Point(10, 10),
                Size = new Size(fieldWidth - 20, 20)
            };
            calcPanel.Controls.Add(_lblCalculation);
            this.Controls.Add(calcPanel);
            currentY += 55;

            // Ошибка
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

            // Кнопки
            var btnSave = new Button
            {
                Text = "Сохранить",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(leftMargin, currentY),
                Size = new Size(210, 40),
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
                Location = new Point(leftMargin + 230, currentY),
                Size = new Size(210, 40),
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(_txtTicker.Text) || _txtTicker.Text.Contains("SBER"))
            {
                ShowError("Введите тикер актива");
                return;
            }
            if (string.IsNullOrWhiteSpace(_txtCompany.Text) || _txtCompany.Text.Contains("Сбербанк"))
            {
                ShowError("Введите название компании");
                return;
            }
            if (!decimal.TryParse(_txtQuantity.Text, out decimal qty) || qty <= 0)
            {
                ShowError("Введите корректное количество");
                return;
            }
            if (!decimal.TryParse(_txtPrice.Text, out decimal price) || price <= 0)
            {
                ShowError("Введите корректную цену покупки");
                return;
            }
            if (_dtpPurchase.Value > DateTime.Now)
            {
                ShowError("Дата покупки не может быть в будущем");
                return;
            }

            Result = new AssetModel
            {
                Id = _existingAsset?.Id ?? 0,
                Type = _assetType,
                Ticker = _txtTicker.Text.Trim().ToUpper(),
                CompanyName = _txtCompany.Text.Trim(),
                Exchange = _cmbExchange.SelectedItem.ToString(),
                Quantity = qty,
                PurchasePrice = price,
                PurchaseDate = _dtpPurchase.Value,
                CurrentPrice = price  // для нового актива = цене покупки
            };

            _lblError.Text = "";
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ShowError(string message)
        {
            _lblError.Text = message;
        }

        private void AddLabel(string text, int x, int y)
        {
            var lbl = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10),
                ForeColor = _textColor,
                Location = new Point(x, y),
                Size = new Size(440, 20)
            };
            this.Controls.Add(lbl);
        }

        private TextBox AddTextBox(int x, int y, int width, string placeholder)
        {
            var tb = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(x, y),
                Size = new Size(width, 25),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250),
                ForeColor = _textColor,
                Text = placeholder
            };
            tb.Enter += (s, ev) =>
            {
                if (tb.Text == placeholder)
                {
                    tb.Text = "";
                    tb.ForeColor = _textColor;
                }
            };
            tb.Leave += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    tb.Text = placeholder;
                    tb.ForeColor = Color.FromArgb(189, 195, 199);
                }
            };
            this.Controls.Add(tb);
            return tb;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AssetForm
            // 
            this.ClientSize = new System.Drawing.Size(501, 631);
            this.Name = "AssetForm";
            this.ResumeLayout(false);

        }
    }
}
