using System;
using System.Drawing;
using System.Windows.Forms;
using FinancialAnalyzer.Models;
using FinancialAnalyzer.Services;

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
        private Label _lblCalc;
        private Button _btnGetPrice;
        private Label _lblCurrentPrice;

        private AssetModel.AssetTypeEnum _assetType;
        private AssetModel _existingAsset;
        private decimal? _apiPrice = null;

        public AssetModel Result { get; private set; }

        public AssetForm(AssetModel.AssetTypeEnum assetType)
        {
            InitializeComponent();
            _assetType = assetType;
            this.Text = $"Добавить ({GetTypeName()})";
            SetupForm();
        }

        public AssetForm(AssetModel existingAsset)
        {
            InitializeComponent();
            _existingAsset = existingAsset;
            _assetType = existingAsset.Type;
            this.Text = $"Редактировать ({GetTypeName()})";
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

        private string GetDefaultTicker()
        {
            switch (_assetType)
            {
                case AssetModel.AssetTypeEnum.Stock: return "SBER";
                case AssetModel.AssetTypeEnum.Currency: return "USD";
                case AssetModel.AssetTypeEnum.Metal: return "XAU";
                default: return "";
            }
        }

        private void LoadExistingData()
        {
            if (_existingAsset == null) return;
            _txtTicker.Text = _existingAsset.Ticker;
            _txtTicker.ForeColor = _textColor;
            _txtCompany.Text = _existingAsset.CompanyName;
            _txtCompany.ForeColor = _textColor;
            _cmbExchange.Text = _existingAsset.Exchange;
            _txtQuantity.Text = _existingAsset.Quantity.ToString("F0");
            _txtQuantity.ForeColor = _textColor;
            _txtPrice.Text = _existingAsset.PurchasePrice.ToString("F2");
            _txtPrice.ForeColor = _textColor;
            _dtpPurchase.Value = _existingAsset.PurchaseDate;
            UpdateCalculation();
        }

        private void SetupForm()
        {
            this.SuspendLayout();
            int left = 30, width = 420, y = 25;

            // Заголовок
            var lblTitle = new Label
            {
                Text = _existingAsset != null ? $"Редактирование: {_existingAsset.Ticker}" : $"Новый актив: {GetTypeName()}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = _primaryColor,
                Location = new Point(left, y),
                Size = new Size(width, 35)
            };
            y += 55;

            // Тикер
            AddLabel("Тикер:", left, y); y += 22;
            _txtTicker = AddTextBox(left, y, width - 110, GetDefaultTicker());
            _txtTicker.ForeColor = _textColor;
            _txtTicker.TextChanged += (s, e) => { _apiPrice = null; _lblCurrentPrice.Text = "Текущая цена: нажмите «Узнать цену»"; _lblCurrentPrice.ForeColor = Color.FromArgb(149, 165, 166); };

            _btnGetPrice = new Button
            {
                Text = "💰 Узнать цену",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(left + width - 100, y),
                Size = new Size(100, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            _btnGetPrice.FlatAppearance.BorderSize = 0;
            _btnGetPrice.Click += BtnGetPrice_Click;
            this.Controls.Add(_btnGetPrice);
            y += 45;

            // Название компании
            AddLabel("Название:", left, y); y += 22;
            _txtCompany = AddTextBox(left, y, width, "");
            y += 45;

            // Биржа
            AddLabel("Биржа:", left, y); y += 22;
            _cmbExchange = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(left, y),
                Size = new Size(width, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cmbExchange.Items.AddRange(new[] { "MOEX", "NASDAQ", "NYSE", "Forex", "Metals", "Другое" });
            this.Controls.Add(_cmbExchange);
            y += 45;

            // Количество
            AddLabel("Количество (шт./ед.):", left, y); y += 22;
            _txtQuantity = AddTextBox(left, y, width, "1");
            _txtQuantity.TextChanged += (s, e) => UpdateCalculation();
            y += 45;

            // Цена покупки
            AddLabel("Цена покупки за единицу (₽):", left, y); y += 22;
            _txtPrice = AddTextBox(left, y, width, "0");
            _txtPrice.TextChanged += (s, e) => UpdateCalculation();
            y += 45;

            // Текущая цена
            _lblCurrentPrice = new Label
            {
                Text = "Текущая цена: нажмите «Узнать цену» для получения из API ЦБ",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(149, 165, 166),
                Location = new Point(left, y),
                Size = new Size(width, 30)
            };
            this.Controls.Add(_lblCurrentPrice);
            y += 40;

            // Дата покупки
            AddLabel("Дата покупки:", left, y); y += 22;
            _dtpPurchase = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10),
                Location = new Point(left, y),
                Size = new Size(width, 25),
                Value = DateTime.Now
            };
            this.Controls.Add(_dtpPurchase);
            y += 45;

            // Предварительный расчёт
            var calcPanel = new Panel
            {
                Location = new Point(left, y),
                Size = new Size(width, 35),
                BackColor = Color.FromArgb(248, 249, 250)
            };
            _lblCalc = new Label
            {
                Text = "Введите количество и цену покупки",
                Font = new Font("Segoe UI", 9),
                ForeColor = _textColor,
                Location = new Point(10, 8),
                Size = new Size(width - 20, 20)
            };
            calcPanel.Controls.Add(_lblCalc);
            this.Controls.Add(calcPanel);
            y += 50;

            // Ошибка
            _lblError = new Label
            {
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(left, y),
                Size = new Size(width, 20),
                Text = ""
            };
            this.Controls.Add(_lblError);
            y += 30;

            // Кнопки
            var btnSave = CreateButton("Сохранить", left, y, 200, _accentColor, Color.White);
            btnSave.Click += BtnSave_Click;
            var btnCancel = CreateButton("Отмена", left + 220, y, 200, Color.FromArgb(236, 240, 241), _textColor);
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] { btnSave, btnCancel });

            // Предустановка биржи
            switch (_assetType)
            {
                case AssetModel.AssetTypeEnum.Stock: _cmbExchange.SelectedIndex = 0; break;
                case AssetModel.AssetTypeEnum.Currency: _cmbExchange.SelectedIndex = 3; break;
                case AssetModel.AssetTypeEnum.Metal: _cmbExchange.SelectedIndex = 4; break;
            }

            this.ResumeLayout(false);
        }

        private void BtnGetPrice_Click(object sender, EventArgs e)
        {
            string ticker = _txtTicker.Text.Trim().ToUpper();
            if (string.IsNullOrEmpty(ticker))
            {
                _lblError.Text = "Введите тикер";
                return;
            }
            _lblError.Text = "";

            Cursor = Cursors.WaitCursor;
            _btnGetPrice.Enabled = false;

            try
            {
                _apiPrice = null;

                if (_assetType == AssetModel.AssetTypeEnum.Currency)
                {
                    _apiPrice = MarketDataService.GetCurrencyRate(ticker);
                    if (_apiPrice.HasValue)
                    {
                        _txtCompany.Text = GetCurrencyName(ticker);
                        _txtCompany.ForeColor = _textColor;
                        _cmbExchange.SelectedIndex = 3;
                    }
                }
                else if (_assetType == AssetModel.AssetTypeEnum.Stock)
                {
                    _apiPrice = MarketDataService.GetStockPrice(ticker);
                    if (_apiPrice.HasValue)
                    {
                        if (string.IsNullOrWhiteSpace(_txtCompany.Text) || _txtCompany.ForeColor == Color.FromArgb(189, 195, 199))
                        {
                            _txtCompany.Text = ticker;
                            _txtCompany.ForeColor = _textColor;
                        }
                        _cmbExchange.SelectedIndex = 0;
                    }
                }
                else if (_assetType == AssetModel.AssetTypeEnum.Metal)
                {
                    _apiPrice = MarketDataService.GetMetalPrice(ticker);
                    if (_apiPrice.HasValue)
                    {
                        _txtCompany.Text = GetMetalName(ticker);
                        _txtCompany.ForeColor = _textColor;
                        _cmbExchange.SelectedIndex = 4;
                    }
                }

                if (_apiPrice.HasValue && _apiPrice.Value > 0)
                {
                    _lblCurrentPrice.Text = $"Текущая цена (ЦБ РФ): {_apiPrice.Value:F2} ₽";
                    _lblCurrentPrice.ForeColor = Color.FromArgb(46, 204, 113);

                    if (_txtPrice.Text == "0" || string.IsNullOrWhiteSpace(_txtPrice.Text))
                    {
                        _txtPrice.Text = _apiPrice.Value.ToString("F2");
                        _txtPrice.ForeColor = _textColor;
                    }
                }
                else
                {
                    _lblCurrentPrice.Text = $"Текущая цена: не найдена для '{ticker}'";
                    _lblCurrentPrice.ForeColor = Color.FromArgb(231, 76, 60);
                }
                UpdateCalculation();
            }
            catch
            {
                _lblCurrentPrice.Text = "Ошибка при запросе к API ЦБ";
                _lblCurrentPrice.ForeColor = Color.FromArgb(231, 76, 60);
            }

            _btnGetPrice.Enabled = true;
            Cursor = Cursors.Default;
        }

        private string GetCurrencyName(string code)
        {
            switch (code.ToUpper())
            {
                case "USD": return "Доллар США";
                case "EUR": return "Евро";
                case "CNY": return "Китайский юань";
                case "GBP": return "Британский фунт";
                case "JPY": return "Японская йена";
                case "CHF": return "Швейцарский франк";
                default: return code;
            }
        }

        private string GetMetalName(string ticker)
        {
            switch (ticker.ToUpper())
            {
                case "XAU": return "Золото (грамм)";
                case "XAG": return "Серебро (грамм)";
                case "XPT": return "Платина (грамм)";
                case "XPD": return "Палладий (грамм)";
                default: return ticker;
            }
        }

        private void UpdateCalculation()
        {
            if (decimal.TryParse(_txtPrice.Text, out decimal price) &&
                decimal.TryParse(_txtQuantity.Text, out decimal qty))
            {
                decimal invested = price * qty;
                string text = $"Вложено: {invested:N2} ₽";

                if (_apiPrice.HasValue && _apiPrice.Value > 0)
                {
                    decimal current = _apiPrice.Value * qty;
                    decimal profit = current - invested;
                    decimal percent = invested > 0 ? profit / invested * 100 : 0;
                    string sign = profit >= 0 ? "+" : "";
                    text += $" | Сейчас: {current:N2} ₽ | {sign}{percent:F1}%";
                }

                _lblCalc.Text = text;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string ticker = _txtTicker.Text.Trim().ToUpper();
            string company = _txtCompany.Text.Trim();

            if (string.IsNullOrWhiteSpace(ticker))
            {
                ShowError("Введите тикер");
                return;
            }
            if (string.IsNullOrWhiteSpace(company))
                company = ticker;
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
                Ticker = ticker,
                CompanyName = company,
                Exchange = _cmbExchange.SelectedItem.ToString(),
                Quantity = qty,
                PurchasePrice = price,
                PurchaseDate = _dtpPurchase.Value,
                CurrentPrice = _apiPrice ?? price
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
                Size = new Size(420, 20)
            };
            this.Controls.Add(lbl);
        }

        private TextBox AddTextBox(int x, int y, int w, string placeholder)
        {
            var tb = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(x, y),
                Size = new Size(w, 25),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250),
                ForeColor = Color.FromArgb(189, 195, 199),
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

        private Button CreateButton(string text, int x, int y, int w, Color back, Color fore)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(x, y),
                Size = new Size(w, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = back,
                ForeColor = fore,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // AssetForm
            // 
            this.ClientSize = new System.Drawing.Size(485, 680);
            this.Name = "AssetForm";
            this.ResumeLayout(false);

        }
    }
}