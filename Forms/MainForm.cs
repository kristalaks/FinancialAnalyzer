using FinancialAnalyzer.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FinancialAnalyzer.Forms
{
    public partial class MainForm : Form
    {
        // Цветовая схема
        private readonly Color _sidebarColor = Color.FromArgb(44, 62, 80);
        private readonly Color _sidebarHoverColor = Color.FromArgb(52, 73, 94);
        private readonly Color _sidebarActiveColor = Color.FromArgb(41, 128, 185);
        private readonly Color _headerColor = Color.FromArgb(52, 73, 94);
        private readonly Color _contentColor = Color.FromArgb(236, 240, 241);
        private readonly Color _textLight = Color.FromArgb(236, 240, 241);
        private readonly Color _textDark = Color.FromArgb(44, 62, 80);

        // Панели
        private Panel _sidebarPanel;
        private Panel _headerPanel;
        private Panel _contentPanel;
        private Panel _reserveDetailsPanel;
        private Panel _depositDetailsPanel;
        private Panel _assetDetailsPanel;

        // Заголовок
        private Label _lblHeaderTitle;

        // Кнопки навигации
        private Button _btnDashboard;
        private Button _btnDeposits;
        private Button _btnStocks;
        private Button _btnCurrency;
        private Button _btnMetals;
        private Button _btnReserve;
        private Button _btnIncomes;
        private Button _btnExpenses;
        private Button _btnCredits;
        private Button _btnSettings;

        private Button _activeNavButton;

        // Настройки
        private decimal _inflationRate = 7.8m;  // годовая инфляция по умолчанию

        public MainForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.SuspendLayout();

            // === Боковая панель (меню) ===
            _sidebarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 200,
                BackColor = _sidebarColor
            };

            // Лого в боковой панели
            var logoPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(33, 47, 61)
            };

            var lblLogo = new Label
            {
                Text = "FinAnalyst",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = _textLight,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            logoPanel.Controls.Add(lblLogo);

            // Панель для кнопок навигации
            var navPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = _sidebarColor,
                Padding = new Padding(0, 10, 0, 10)
            };

            // Создаём кнопки навигации (в обратном порядке — последняя в коде будет сверху)
            _btnSettings = CreateNavButton("⚙  Настройки", 9);
            _btnCredits = CreateNavButton("🏠  Кредиты", 8);
            _btnExpenses = CreateNavButton("🛒  Расходы", 7);
            _btnIncomes = CreateNavButton("💰  Доходы", 6);
            _btnReserve = CreateNavButton("💵  Резерв", 5);
            _btnMetals = CreateNavButton("🪙  Металлы", 4);
            _btnCurrency = CreateNavButton("💱  Валюты", 3);
            _btnStocks = CreateNavButton("📈  Акции", 2);
            _btnDeposits = CreateNavButton("🏦  Вклады", 1);
            _btnDashboard = CreateNavButton("📊  Главная", 0);

            // Добавляем в обратном порядке (Dashboard последним — будет внизу)
            navPanel.Controls.AddRange(new Control[] {
                _btnSettings,
                _btnCredits,
                _btnExpenses,
                _btnIncomes,
                _btnReserve,
                _btnMetals,
                _btnCurrency,
                _btnStocks,
                _btnDeposits,
                _btnDashboard
            });

            _sidebarPanel.Controls.Add(navPanel);
            _sidebarPanel.Controls.Add(logoPanel);

            // === Верхняя панель (хедер) ===
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.White,
                Padding = new Padding(20, 0, 20, 0)
            };

            _lblHeaderTitle = new Label
            {
                Text = "Главная",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = _textDark,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill
            };

            var lblUser = new Label
            {
                Text = "👤 Зыкин Егор",
                Font = new Font("Segoe UI", 10),
                ForeColor = _textDark,
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Right,
                Width = 150
            };

            _headerPanel.Controls.Add(lblUser);
            _headerPanel.Controls.Add(_lblHeaderTitle);

            // === Контентная панель ===
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = _contentColor,
                Padding = new Padding(20)
            };

            // При запуске показываем Dashboard
            ShowDashboard();

            // Добавляем всё на форму
            this.Controls.Add(_contentPanel);
            this.Controls.Add(_headerPanel);
            this.Controls.Add(_sidebarPanel);

            // Активируем первую кнопку
            SetActiveButton(_btnDashboard);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Button CreateNavButton(string text, int index)
        {
            var button = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Top,
                Height = 45,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Regular),
                ForeColor = _textLight,
                BackColor = _sidebarColor,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand,
                Tag = index
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.BorderColor = _sidebarColor;
            button.FlatAppearance.MouseOverBackColor = _sidebarHoverColor;
            button.FlatAppearance.MouseDownBackColor = _sidebarActiveColor;

            button.MouseEnter += (s, e) =>
            {
                if (button != _activeNavButton)
                    button.BackColor = _sidebarHoverColor;
            };

            button.MouseLeave += (s, e) =>
            {
                if (button != _activeNavButton)
                    button.BackColor = _sidebarColor;
            };

            button.Click += NavButton_Click;

            return button;
        }

        private void NavButton_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            SetActiveButton(button);
            _lblHeaderTitle.Text = button.Text.Replace("  ", " ").Trim();

            // Пока просто меняем заголовок
            // Позже здесь будет загрузка соответствующей панели
            UpdateContentPanel((int)button.Tag);
        }

        private void SetActiveButton(Button activeButton)
        {
            // Сбрасываем предыдущую активную кнопку
            if (_activeNavButton != null)
            {
                _activeNavButton.BackColor = _sidebarColor;
                _activeNavButton.ForeColor = _textLight;
            }

            // Активируем новую
            _activeNavButton = activeButton;
            _activeNavButton.BackColor = _sidebarActiveColor;
            _activeNavButton.ForeColor = Color.White;
            _activeNavButton.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
        }

        private void UpdateContentPanel(int sectionIndex)
        {
            _contentPanel.Controls.Clear();

            switch (sectionIndex)
            {
                case 0:
                    ShowDashboard();
                    break;
                case 1:
                    ShowDepositsView();
                    break;
                case 2:
                    ShowStocksView();
                    break;
                case 3:
                    ShowCurrencyView();
                    break;
                case 4:
                    ShowMetalsView();
                    break;
                case 5:
                    ShowReserveView();
                    break;
                case 6:
                    ShowIncomesView();
                    break;
                case 7:
                    ShowExpensesView();
                    break;
                case 8:
                    ShowCreditsView();
                    break;
                case 9:
                    ShowSettingsView();
                    break;
                default:
                    ShowPlaceholder("Неизвестный раздел");
                    break;
            }
        }

        // Заглушка для остальных разделов
        private void ShowPlaceholder(string sectionName)
        {
            var placeholder = new Label
            {
                Text = $"Раздел: {sectionName}\n\nЗдесь будет содержимое раздела.",
                Font = new Font("Segoe UI", 14),
                ForeColor = _textDark,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            _contentPanel.Controls.Add(placeholder);
        }

        private void ShowDepositsView()
        {
            _contentPanel.Controls.Clear();

            // Список вкладов (пока демо-данные)
            var deposits = Services.DepositService.GetDemoDeposits();

            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White,
                Padding = new Padding(20, 15, 20, 10)
            };

            var lblTitle = new Label
            {
                Text = "🏦 Вклады",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = _textDark,
                Location = new Point(20, 12),
                Size = new Size(200, 35)
            };

            var btnAdd = new Button
            {
                Text = "+ Добавить",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(900, 15),
                Size = new Size(160, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = _sidebarActiveColor,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) =>
            {
                using (var form = new DepositForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // Пока просто показываем сообщение
                        // Позже будем добавлять в БД
                        MessageBox.Show(
                            $"Вклад \"{form.Result.Name}\" добавлен!\n" +
                            $"Сумма: {form.Result.InitialAmount:N0} ₽\n" +
                            $"Ставка: {form.Result.InterestRate}%\n" +
                            $"Тип: {form.Result.RateTypeText}",
                            "Успешно",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            };

            topPanel.Controls.AddRange(new Control[] { lblTitle, btnAdd });

            var tablePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 250,
                BackColor = _contentColor,
                Padding = new Padding(20, 0, 20, 15)
            };

            var dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10)
            };

            // Настройка стиля заголовков
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = _textDark;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView.ColumnHeadersHeight = 40;

            // Настройка стиля строк
            dataGridView.DefaultCellStyle.ForeColor = _textDark;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(234, 245, 251);
            dataGridView.DefaultCellStyle.SelectionForeColor = _textDark;
            dataGridView.RowTemplate.Height = 35;

            // Режим заполнения
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Колонки с пропорциями
            dataGridView.Columns.Add("colId", "#");
            dataGridView.Columns["colId"].FillWeight = 5;

            dataGridView.Columns.Add("colName", "Название");
            dataGridView.Columns["colName"].FillWeight = 40;

            dataGridView.Columns.Add("colAmount", "Текущая сумма");
            dataGridView.Columns["colAmount"].DefaultCellStyle.Format = "N0";
            dataGridView.Columns["colAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView.Columns["colAmount"].FillWeight = 20;

            dataGridView.Columns.Add("colRate", "Ставка");
            dataGridView.Columns["colRate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.Columns["colRate"].FillWeight = 12;

            dataGridView.Columns.Add("colType", "Тип ставки");
            dataGridView.Columns["colType"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.Columns["colType"].FillWeight = 12;

            dataGridView.Columns.Add("colChange", "Изменение");
            dataGridView.Columns["colChange"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.Columns["colChange"].FillWeight = 11;

            // Заполнение данными
            for (int i = 0; i < deposits.Count; i++)
            {
                var d = deposits[i];
                int rowIndex = dataGridView.Rows.Add(
                    d.Id,
                    d.Name,
                    d.CurrentAmount,
                    d.InterestRate.ToString("F1") + "%",
                    d.RateTypeText,
                    d.ChangeText
                );

                // Цвет строки изменения
                var changeCell = dataGridView.Rows[rowIndex].Cells["colChange"];
                changeCell.Style.ForeColor = d.IsPositive
                    ? Color.FromArgb(46, 204, 113)
                    : Color.FromArgb(231, 76, 60);
                changeCell.Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }

            // Выбор строки
            dataGridView.SelectionChanged += (s, e) =>
            {
                if (dataGridView.SelectedRows.Count > 0)
                {
                    int index = dataGridView.SelectedRows[0].Index;
                    if (index < deposits.Count)
                    {
                        ShowDepositDetails(deposits[index]);
                    }
                }
            };

            tablePanel.Controls.Add(dataGridView);


            var detailsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            _depositDetailsPanel = detailsPanel;  // сохраняем ссылку для обновления

            tablePanel.Controls.Add(dataGridView);


            _contentPanel.Controls.Add(detailsPanel);
            _contentPanel.Controls.Add(tablePanel);
            _contentPanel.Controls.Add(topPanel);

            // Показываем детали первого вклада
            if (deposits.Count > 0)
                ShowDepositDetails(deposits[0]);
        }

        private void ShowDepositDetails(Models.DepositModel deposit)
        {
            if (_depositDetailsPanel == null)
                return;

            _depositDetailsPanel.Controls.Clear();

            var lblName = new Label
            {
                Text = $"📋 {deposit.Name}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = _textDark,
                Location = new Point(20, 15),
                Size = new Size(800, 35)
            };

            var detailsText = new Label
            {
                Text = $"Начальная сумма: {deposit.InitialAmount:N0} ₽\n" +
                       $"Текущая сумма: {deposit.CurrentAmount:N0} ₽\n" +
                       $"Ставка: {deposit.InterestRate}% годовых\n" +
                       $"Тип: {deposit.RateTypeText}\n" +
                       $"Открыт: {deposit.OpenDate.ToShortDateString()}\n" +
                       (deposit.CloseDate.HasValue
                           ? $"Закрытие: {deposit.CloseDate.Value.ToShortDateString()}"
                           : "Бессрочный"),
                Font = new Font("Segoe UI", 11),
                ForeColor = _textDark,
                Location = new Point(20, 60),
                Size = new Size(400, 160)
            };

            var changeColor = deposit.IsPositive
                ? Color.FromArgb(46, 204, 113)
                : Color.FromArgb(231, 76, 60);
            var changeSign = deposit.IsPositive ? "+" : "";

            var lblChange = new Label
            {
                Text = $"Изменение:\n{changeSign}{deposit.Profit:N0} ₽ ({changeSign}{deposit.ProfitPercent:F1}%)",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = changeColor,
                Location = new Point(500, 60),
                Size = new Size(400, 80),
                TextAlign = ContentAlignment.TopRight
            };

            // Кнопки действий
            var btnEdit = new Button
            {
                Text = "✏ Изменить",
                Font = new Font("Segoe UI", 10),
                Location = new Point(500, 160),
                Size = new Size(140, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(236, 240, 241),
                ForeColor = _textDark,
                Cursor = Cursors.Hand
            };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += (s, e) =>
            {
                using (var form = new DepositForm(deposit))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show("Изменения сохранены!", "Успешно",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ShowDepositsView();  // обновляем вид
                    }
                }
            };

            var btnDelete = new Button
            {
                Text = "🗑 Удалить",
                Font = new Font("Segoe UI", 10),
                Location = new Point(660, 160),
                Size = new Size(140, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(253, 237, 236),
                ForeColor = Color.FromArgb(231, 76, 60),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += (s, e) =>
            {
                var result = MessageBox.Show(
                    $"Удалить вклад \"{deposit.Name}\"?",
                    "Подтверждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Вклад удалён (демо).", "Успешно",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ShowDepositsView();
                }
            };

            _depositDetailsPanel.Controls.AddRange(new Control[] {
            lblName, detailsText, lblChange, btnEdit, btnDelete
            });
        }

        private void ShowStocksView()
        {
            _contentPanel.Controls.Clear();

            var stocks = Services.AssetService.GetDemoStocks();

            // ==========================================
            // Верхняя панель с заголовком и поиском
            // ==========================================
            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White,
                Padding = new Padding(20, 12, 20, 10)
            };

            var lblTitle = new Label
            {
                Text = "📈 Акции",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = _textDark,
                Location = new Point(20, 12),
                Size = new Size(200, 35)
            };

            var txtSearch = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(500, 15),
                Size = new Size(250, 25),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250),
                ForeColor = _textDark,
                Text = "🔍 Поиск по тикеру или названию"
            };
            txtSearch.Enter += (s, e) =>
            {
                if (txtSearch.Text == "🔍 Поиск по тикеру или названию")
                    txtSearch.Text = "";
            };
            txtSearch.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                    txtSearch.Text = "🔍 Поиск по тикеру или названию";
            };

            var btnAdd = new Button
            {
                Text = "+ Добавить",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(770, 13),
                Size = new Size(160, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = _sidebarActiveColor,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) =>
            {
                using (var form = new AssetForm(AssetModel.AssetTypeEnum.Stock))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show(
                            $"Актив \"{form.Result.Ticker}\" добавлен!\n" +
                            $"Компания: {form.Result.CompanyName}\n" +
                            $"Количество: {form.Result.Quantity}\n" +
                            $"Цена покупки: {form.Result.PurchasePrice:F2} ₽",
                            "Успешно",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            };

            topPanel.Controls.AddRange(new Control[] { lblTitle, txtSearch, btnAdd });

            // ==========================================
            // Таблица акций
            // ==========================================
            var tablePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 230,
                BackColor = _contentColor,
                Padding = new Padding(20, 0, 20, 10)
            };

            var dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10)
            };

            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = _textDark;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView.ColumnHeadersHeight = 40;
            dataGridView.DefaultCellStyle.ForeColor = _textDark;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(234, 245, 251);
            dataGridView.DefaultCellStyle.SelectionForeColor = _textDark;
            dataGridView.RowTemplate.Height = 35;

            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView.Columns.Add("colId", "#");
            dataGridView.Columns["colId"].FillWeight = 5;

            dataGridView.Columns.Add("colTicker", "Тикер");
            dataGridView.Columns["colTicker"].FillWeight = 10;

            dataGridView.Columns.Add("colName", "Компания");
            dataGridView.Columns["colName"].FillWeight = 40;

            dataGridView.Columns.Add("colQty", "Кол-во");
            dataGridView.Columns["colQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.Columns["colQty"].FillWeight = 10;

            dataGridView.Columns.Add("colPrice", "Тек. цена");
            dataGridView.Columns["colPrice"].DefaultCellStyle.Format = "F2";
            dataGridView.Columns["colPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView.Columns["colPrice"].FillWeight = 15;

            dataGridView.Columns.Add("colChange", "Изм.");
            dataGridView.Columns["colChange"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.Columns["colChange"].FillWeight = 10;

            // Заполнение
            for (int i = 0; i < stocks.Count; i++)
            {
                var s = stocks[i];
                int rowIndex = dataGridView.Rows.Add(
                    s.Id,
                    s.Ticker,
                    s.CompanyName,
                    s.Quantity,
                    s.CurrentPrice,
                    s.ChangeText
                );

                // Цвет всей строки по прибыльности
                var changeCell = dataGridView.Rows[rowIndex].Cells["colChange"];
                changeCell.Style.ForeColor = s.IsPositive
                    ? Color.FromArgb(46, 204, 113)
                    : Color.FromArgb(231, 76, 60);
                changeCell.Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }

            dataGridView.SelectionChanged += (s, e) =>
            {
                if (dataGridView.SelectedRows.Count > 0)
                {
                    int index = dataGridView.SelectedRows[0].Index;
                    if (index < stocks.Count)
                    {
                        ShowAssetDetails(stocks[index]);
                    }
                }
            };

            tablePanel.Controls.Add(dataGridView);

            // ==========================================
            // Панель деталей
            // ==========================================
            var detailsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            _assetDetailsPanel = detailsPanel;

            // Сборка
            _contentPanel.Controls.Add(detailsPanel);
            _contentPanel.Controls.Add(tablePanel);
            _contentPanel.Controls.Add(topPanel);

            if (stocks.Count > 0)
                ShowAssetDetails(stocks[0]);
        }
        
        private void ShowAssetDetails(Models.AssetModel asset)
        {
            if (_assetDetailsPanel == null)
                return;

            _assetDetailsPanel.Controls.Clear();

            // Заголовок
            var lblHeader = new Label
            {
                Text = $"📋 {asset.CompanyName} ({asset.Ticker})",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = _textDark,
                Location = new Point(20, 15),
                Size = new Size(800, 35)
            };

            var lblExchange = new Label
            {
                Text = $"Биржа: {asset.Exchange}    •    Сектор: {asset.TypeText}",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(149, 165, 166),
                Location = new Point(20, 50),
                Size = new Size(400, 20)
            };

            // Блок текущей цены
            var priceBox = new Panel
            {
                Location = new Point(20, 85),
                Size = new Size(300, 80),
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(15)
            };

            var changeColor = asset.IsPositive
                ? Color.FromArgb(46, 204, 113)
                : Color.FromArgb(231, 76, 60);
            var changeSign = asset.IsPositive ? "+" : "";
            var changeArrow = asset.IsPositive ? "▲" : "▼";

            var lblPrice = new Label
            {
                Text = $"Текущая цена: {asset.CurrentPrice:F2} ₽",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = _textDark,
                Location = new Point(15, 10),
                Size = new Size(270, 25)
            };

            var lblDayChange = new Label
            {
                Text = $"{changeSign}{asset.ProfitPercent:F2}% {changeArrow}",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = changeColor,
                Location = new Point(15, 40),
                Size = new Size(270, 25)
            };

            priceBox.Controls.AddRange(new Control[] { lblPrice, lblDayChange });

            // Блок вашего портфеля
            var portfolioBox = new Panel
            {
                Location = new Point(340, 85),
                Size = new Size(400, 120),
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(15)
            };

            var lblPortfolioTitle = new Label
            {
                Text = "Ваш портфель",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = _textDark,
                Location = new Point(15, 8),
                Size = new Size(370, 20)
            };

            var lblQty = new Label
            {
                Text = $"Количество: {asset.Quantity} шт.",
                Font = new Font("Segoe UI", 10),
                ForeColor = _textDark,
                Location = new Point(15, 35),
                Size = new Size(370, 20)
            };

            var lblBuyPrice = new Label
            {
                Text = $"Цена покупки: {asset.PurchasePrice:F2} ₽    •    {asset.PurchaseDate.ToShortDateString()}",
                Font = new Font("Segoe UI", 10),
                ForeColor = _textDark,
                Location = new Point(15, 58),
                Size = new Size(370, 20)
            };

            var lblTotal = new Label
            {
                Text = $"Вложено: {asset.InvestedAmount:N0} ₽    →    Сейчас: {asset.CurrentTotalValue:N0} ₽",
                Font = new Font("Segoe UI", 10),
                ForeColor = _textDark,
                Location = new Point(15, 81),
                Size = new Size(370, 20)
            };

            portfolioBox.Controls.AddRange(new Control[] { lblPortfolioTitle, lblQty, lblBuyPrice, lblTotal });

            // Блок прогноза
            var forecastBox = new Panel
            {
                Location = new Point(20, 180),
                Size = new Size(720, 80),
                BackColor = Color.FromArgb(248, 249, 250),
                Padding = new Padding(15)
            };

            var monthsToForecast = 12;
            var predictedGrowth = asset.AvgMonthlyGrowthPercent * monthsToForecast;
            var predictedTotal = asset.CurrentTotalValue * (1 + predictedGrowth / 100m);
            var inflationRate = 7.8m;
            var realTotal = predictedTotal / (1 + inflationRate / 100m);

            var lblForecast = new Label
            {
                Text = $"Прогноз на {monthsToForecast} мес. (экстраполяция):\n" +
                       $"• Номинальная стоимость: {predictedTotal:N0} ₽ ({changeSign}{predictedGrowth:F1}%)\n" +
                       $"• С учётом инфляции ({inflationRate}%): {realTotal:N0} ₽",
                Font = new Font("Segoe UI", 10),
                ForeColor = _textDark,
                Location = new Point(15, 10),
                Size = new Size(690, 60)
            };
            forecastBox.Controls.Add(lblForecast);

            // Кнопки
            var btnEdit = new Button
            {
                Text = "✏ Изменить",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 280),
                Size = new Size(140, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(236, 240, 241),
                ForeColor = _textDark,
                Cursor = Cursors.Hand
            };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += (s, e) =>
            {
                using (var form = new AssetForm(asset))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show("Изменения сохранены!", "Успешно",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ShowStocksView();
                    }
                }
            };

            var btnDelete = new Button
            {
                Text = "🗑 Удалить",
                Font = new Font("Segoe UI", 10),
                Location = new Point(180, 280),
                Size = new Size(140, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(253, 237, 236),
                ForeColor = Color.FromArgb(231, 76, 60),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += (s, e) =>
            {
                var result = MessageBox.Show(
                    $"Удалить актив \"{asset.Ticker}\"?",
                    "Подтверждение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Актив удалён (демо).", "Успешно",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ShowStocksView();
                }
            };

            _assetDetailsPanel.Controls.AddRange(new Control[] {
            lblHeader, lblExchange, priceBox, portfolioBox,
            forecastBox, btnEdit, btnDelete
            });
        }

        private void ShowCurrencyView()
        {
            ShowAssetListView(
                "💱 Валюты",
                Services.AssetService.GetDemoCurrencies(),
                AssetModel.AssetTypeEnum.Currency);
        }

        private void ShowMetalsView()
        {
            ShowAssetListView(
                "🪙 Металлы",
                Services.AssetService.GetDemoMetals(),
                AssetModel.AssetTypeEnum.Metal);
        }

        private void ShowAssetListView(string title, System.Collections.Generic.List<AssetModel> assets,
            AssetModel.AssetTypeEnum assetType)
        {
            _contentPanel.Controls.Clear();

            // Верхняя панель
            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White,
                Padding = new Padding(20, 12, 20, 10)
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = _textDark,
                Location = new Point(20, 12),
                Size = new Size(300, 35)
            };

            var btnAdd = new Button
            {
                Text = "+ Добавить",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(900, 15),
                Size = new Size(160, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = _sidebarActiveColor,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) =>
            {
                using (var form = new AssetForm(assetType))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show(
                            $"Актив \"{form.Result.Ticker}\" добавлен!\n" +
                            $"Название: {form.Result.CompanyName}\n" +
                            $"Количество: {form.Result.Quantity}\n" +
                            $"Цена покупки: {form.Result.PurchasePrice:F2} ₽",
                            "Успешно",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        // Обновить вид
                        if (assetType == AssetModel.AssetTypeEnum.Currency)
                            ShowCurrencyView();
                        else
                            ShowMetalsView();
                    }
                }
            };

            topPanel.Controls.AddRange(new Control[] { lblTitle, btnAdd });

            // Таблица
            var tablePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 220,
                BackColor = _contentColor,
                Padding = new Padding(20, 0, 20, 10)
            };

            var dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10)
            };

            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = _textDark;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView.ColumnHeadersHeight = 40;
            dataGridView.DefaultCellStyle.ForeColor = _textDark;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(234, 245, 251);
            dataGridView.DefaultCellStyle.SelectionForeColor = _textDark;
            dataGridView.RowTemplate.Height = 35;

            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView.Columns.Add("colId", "#");
            dataGridView.Columns["colId"].FillWeight = 5;

            dataGridView.Columns.Add("colTicker", "Тикер");
            dataGridView.Columns["colTicker"].FillWeight = 12;

            dataGridView.Columns.Add("colName", "Название");
            dataGridView.Columns["colName"].FillWeight = 38;

            dataGridView.Columns.Add("colQty", "Кол-во");
            dataGridView.Columns["colQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.Columns["colQty"].FillWeight = 10;

            dataGridView.Columns.Add("colPrice", "Тек. цена");
            dataGridView.Columns["colPrice"].DefaultCellStyle.Format = "F2";
            dataGridView.Columns["colPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView.Columns["colPrice"].FillWeight = 15;

            dataGridView.Columns.Add("colChange", "Изм.");
            dataGridView.Columns["colChange"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.Columns["colChange"].FillWeight = 10;

            for (int i = 0; i < assets.Count; i++)
            {
                var a = assets[i];
                int rowIndex = dataGridView.Rows.Add(
                    a.Id, a.Ticker, a.CompanyName, a.Quantity, a.CurrentPrice, a.ChangeText);

                var changeCell = dataGridView.Rows[rowIndex].Cells["colChange"];
                changeCell.Style.ForeColor = a.IsPositive
                    ? Color.FromArgb(46, 204, 113)
                    : Color.FromArgb(231, 76, 60);
                changeCell.Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }

            dataGridView.SelectionChanged += (s, e) =>
            {
                if (dataGridView.SelectedRows.Count > 0)
                {
                    int index = dataGridView.SelectedRows[0].Index;
                    if (index < assets.Count)
                    {
                        ShowAssetDetails(assets[index]);
                    }
                }
            };

            tablePanel.Controls.Add(dataGridView);

            // Панель деталей
            var detailsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };
            _assetDetailsPanel = detailsPanel;

            // Сборка
            _contentPanel.Controls.Add(detailsPanel);
            _contentPanel.Controls.Add(tablePanel);
            _contentPanel.Controls.Add(topPanel);

            if (assets.Count > 0)
                ShowAssetDetails(assets[0]);
        }

        private void ShowReserveDetails(Models.ReserveModel reserve)
        {
            if (_reserveDetailsPanel == null) return;
            _reserveDetailsPanel.Controls.Clear();

            var lblName = new Label
            {
                Text = $"💵 {reserve.Name}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = _textDark,
                Location = new Point(20, 15),
                Size = new Size(600, 35)
            };

            var lblNote = new Label
            {
                Text = string.IsNullOrEmpty(reserve.Note) ? "Без описания" : reserve.Note,
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(149, 165, 166),
                Location = new Point(20, 50),
                Size = new Size(600, 20)
            };

            // Левая колонка с информацией
            var leftPanel = new Panel
            {
                Location = new Point(20, 85),
                Size = new Size(450, 130),
                BackColor = Color.White
            };

            var detailsText = new Label
            {
                Text = $"Сумма: {reserve.Amount:N0} ₽\n" +
                       $"Дата создания: {reserve.CreatedAt.ToShortDateString()}\n" +
                       $"Срок хранения: {(DateTime.Now - reserve.CreatedAt).Days} дней\n\n" +
                       $"⚠️ Внимание: резерв не приносит доход и обесценивается из-за инфляции.",
                Font = new Font("Segoe UI", 11),
                ForeColor = _textDark,
                Location = new Point(0, 0),
                Size = new Size(450, 130)
            };
            leftPanel.Controls.Add(detailsText);

            // Правая колонка — блок потерь
            var lossBox = new Panel
            {
                Location = new Point(500, 85),
                Size = new Size(350, 140),
                BackColor = Color.FromArgb(253, 237, 236),
                Padding = new Padding(15)
            };

            var lblLossTitle = new Label
            {
                Text = $"Потери от инфляции ({_inflationRate:F1}% год.)",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(15, 12),
                Size = new Size(320, 22)
            };

            var lblLossValue = new Label
            {
                Text = $"{reserve.ChangeText}",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(15, 40),
                Size = new Size(320, 35)
            };

            var lblLossAmount = new Label
            {
                Text = $"{reserve.InflationLoss:N0} ₽",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(15, 75),
                Size = new Size(320, 30)
            };

            var lblRealValue = new Label
            {
                Text = $"Реальная стоимость: {reserve.RealValue:N0} ₽",
                Font = new Font("Segoe UI", 9),
                ForeColor = _textDark,
                Location = new Point(15, 112),
                Size = new Size(320, 18)
            };

            lossBox.Controls.AddRange(new Control[] { lblLossTitle, lblLossValue, lblLossAmount, lblRealValue });

            // Кнопки
            var btnEdit = new Button
            {
                Text = "✏ Изменить",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 240),
                Size = new Size(140, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(236, 240, 241),
                ForeColor = _textDark,
                Cursor = Cursors.Hand
            };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += (s, ev) =>
            {
                using (var form = new ReserveForm(reserve))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show("Изменения сохранены!", "Успешно",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ShowReserveView();
                    }
                }
            };

            var btnDelete = new Button
            {
                Text = "🗑 Удалить",
                Font = new Font("Segoe UI", 10),
                Location = new Point(180, 240),
                Size = new Size(140, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(253, 237, 236),
                ForeColor = Color.FromArgb(231, 76, 60),
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += (s, ev) =>
            {
                var result = MessageBox.Show($"Удалить резерв \"{reserve.Name}\"?",
                    "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Резерв удалён (демо).", "Успешно",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ShowReserveView();
                }
            };

            _reserveDetailsPanel.Controls.AddRange(new Control[] {
            lblName, lblNote, leftPanel, lossBox, btnEdit, btnDelete
            });
        }

        private void ShowReserveView()
        {
            _contentPanel.Controls.Clear();

            var reserves = Services.ReserveService.GetDemoReserves();

            // Верхняя панель
            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White,
                Padding = new Padding(20, 12, 20, 10)
            };

            var lblTitle = new Label
            {
                Text = "💵 Резерв (неработающие активы)",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = _textDark,
                Location = new Point(20, 12),
                Size = new Size(500, 35)
            };

            var btnAdd = new Button
            {
                Text = "+ Добавить",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(900, 15),
                Size = new Size(160, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = _sidebarActiveColor,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) =>
            {
                using (var form = new ReserveForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show(
                            $"Резерв \"{form.Result.Name}\" добавлен!\n" +
                            $"Сумма: {form.Result.Amount:N0} ₽",
                            "Успешно",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        ShowReserveView();
                    }
                }
            };

            topPanel.Controls.AddRange(new Control[] { lblTitle, btnAdd });

            // Таблица
            var tablePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 180,
                BackColor = _contentColor,
                Padding = new Padding(20, 0, 20, 10)
            };

            var dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 10)
            };

            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = _textDark;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView.ColumnHeadersHeight = 40;
            dataGridView.DefaultCellStyle.ForeColor = _textDark;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(234, 245, 251);
            dataGridView.DefaultCellStyle.SelectionForeColor = _textDark;
            dataGridView.RowTemplate.Height = 35;

            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView.Columns.Add("colId", "#");
            dataGridView.Columns["colId"].FillWeight = 5;

            dataGridView.Columns.Add("colName", "Название");
            dataGridView.Columns["colName"].FillWeight = 30;

            dataGridView.Columns.Add("colAmount", "Сумма");
            dataGridView.Columns["colAmount"].DefaultCellStyle.Format = "N0";
            dataGridView.Columns["colAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView.Columns["colAmount"].FillWeight = 15;

            dataGridView.Columns.Add("colDate", "Дата создания");
            dataGridView.Columns["colDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.Columns["colDate"].FillWeight = 15;

            dataGridView.Columns.Add("colLoss", "Потери от инфл.");
            dataGridView.Columns["colLoss"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView.Columns["colLoss"].FillWeight = 15;

            dataGridView.Columns.Add("colReal", "Реальная стоимость");
            dataGridView.Columns["colReal"].DefaultCellStyle.Format = "N0";
            dataGridView.Columns["colReal"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView.Columns["colReal"].FillWeight = 20;

            for (int i = 0; i < reserves.Count; i++)
            {
                var r = reserves[i];
                int rowIndex = dataGridView.Rows.Add(
                    r.Id,
                    r.Name,
                    r.Amount,
                    r.CreatedAt.ToShortDateString(),
                    r.ChangeText,
                    r.RealValue
                );

                var lossCell = dataGridView.Rows[rowIndex].Cells["colLoss"];
                lossCell.Style.ForeColor = Color.FromArgb(231, 76, 60);
                lossCell.Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }

            dataGridView.SelectionChanged += (s, ev) =>
            {
                if (dataGridView.SelectedRows.Count > 0)
                {
                    int index = dataGridView.SelectedRows[0].Index;
                    if (index < reserves.Count)
                        ShowReserveDetails(reserves[index]);
                }
            };

            tablePanel.Controls.Add(dataGridView);

            // Панель деталей
            var detailsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            _reserveDetailsPanel = detailsPanel;

            _contentPanel.Controls.Add(detailsPanel);
            _contentPanel.Controls.Add(tablePanel);
            _contentPanel.Controls.Add(topPanel);

            if (reserves.Count > 0)
                ShowReserveDetails(reserves[0]);
        }

        private void ShowIncomesView()
        {
            _contentPanel.Controls.Clear();
            var incomes = Services.IncomeService.GetDemoIncomes();
            decimal totalMonthly = 0;

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White, Padding = new Padding(20, 12, 20, 10) };
            var lblTitle = new Label { Text = "💰 Доходы", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = _textDark, Location = new Point(20, 12), Size = new Size(300, 35) };
            var btnAdd = new Button { Text = "+ Добавить", Font = new Font("Segoe UI", 11, FontStyle.Bold), Location = new Point(900, 15), Size = new Size(160, 35), FlatStyle = FlatStyle.Flat, BackColor = _sidebarActiveColor, ForeColor = Color.White, Cursor = Cursors.Hand };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) =>
            {
                using (var form = new IncomeForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show(
                            $"Доход добавлен!\n" +
                            $"Источник: {form.Result.SourceText}\n" +
                            $"Сумма: {form.Result.AmountPerPayment:N0} ₽ × {form.Result.PaymentsPerMonth} раз/мес\n" +
                            $"Месячный доход: {form.Result.MonthlyAmount:N0} ₽",
                            "Успешно",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        ShowIncomesView();
                    }
                }
            };
            topPanel.Controls.AddRange(new Control[] { lblTitle, btnAdd });

            var tablePanel = new Panel { Dock = DockStyle.Top, Height = 200, BackColor = _contentColor, Padding = new Padding(20, 0, 20, 10) };
            var dataGridView = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AllowUserToResizeRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, RowHeadersVisible = false, Font = new Font("Segoe UI", 10) };
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = _textDark;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView.ColumnHeadersHeight = 40;
            dataGridView.DefaultCellStyle.ForeColor = _textDark;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(234, 245, 251);
            dataGridView.DefaultCellStyle.SelectionForeColor = _textDark;
            dataGridView.RowTemplate.Height = 35;

            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.Columns.Add("colId", "#"); dataGridView.Columns["colId"].FillWeight = 5;
            dataGridView.Columns.Add("colSource", "Источник"); dataGridView.Columns["colSource"].FillWeight = 25;
            dataGridView.Columns.Add("colAmount", "Сумма/выплата"); dataGridView.Columns["colAmount"].DefaultCellStyle.Format = "N0"; dataGridView.Columns["colAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; dataGridView.Columns["colAmount"].FillWeight = 15;
            dataGridView.Columns.Add("colCount", "Выплат/мес"); dataGridView.Columns["colCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; dataGridView.Columns["colCount"].FillWeight = 12;
            dataGridView.Columns.Add("colMonthly", "В месяц"); dataGridView.Columns["colMonthly"].DefaultCellStyle.Format = "N0"; dataGridView.Columns["colMonthly"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; dataGridView.Columns["colMonthly"].FillWeight = 15;
            dataGridView.Columns.Add("colTax", "Налог"); dataGridView.Columns["colTax"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; dataGridView.Columns["colTax"].FillWeight = 13;
            dataGridView.Columns.Add("colTarget", "Направление"); dataGridView.Columns["colTarget"].FillWeight = 15;

            foreach (var inc in incomes)
            {
                totalMonthly += inc.MonthlyAmount;
                dataGridView.Rows.Add(inc.Id, inc.SourceText, inc.AmountPerPayment, inc.PaymentsPerMonth, inc.MonthlyAmount, inc.TaxText, inc.TargetDepositName ?? "—");
            }

            // Итоговая строка
            dataGridView.Rows.Add("", "ИТОГО:", "", "", totalMonthly, "", "");
            var totalRow = dataGridView.Rows[dataGridView.Rows.Count - 1];
            totalRow.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            totalRow.DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            totalRow.Cells["colMonthly"].Style.Format = "N0";

            tablePanel.Controls.Add(dataGridView);

            var summaryPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20) };
            var lblSummary = new Label { Text = $"Общий доход: {totalMonthly:N0} ₽/мес ({totalMonthly * 12:N0} ₽/год)", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(46, 204, 113), Location = new Point(20, 20), Size = new Size(600, 35) };
            summaryPanel.Controls.Add(lblSummary);

            _contentPanel.Controls.Add(summaryPanel);
            _contentPanel.Controls.Add(tablePanel);
            _contentPanel.Controls.Add(topPanel);
        }

        private void ShowExpensesView()
        {
            _contentPanel.Controls.Clear();
            var expenses = Services.ExpenseService.GetDemoExpenses();
            decimal totalMonthly = 0;

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White, Padding = new Padding(20, 12, 20, 10) };
            var lblTitle = new Label { Text = "🛒 Расходы", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = _textDark, Location = new Point(20, 12), Size = new Size(300, 35) };
            var btnAdd = new Button { Text = "+ Добавить", Font = new Font("Segoe UI", 11, FontStyle.Bold), Location = new Point(900, 15), Size = new Size(160, 35), FlatStyle = FlatStyle.Flat, BackColor = _sidebarActiveColor, ForeColor = Color.White, Cursor = Cursors.Hand };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) =>
            {
                using (var form = new ExpenseForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show(
                            $"Расход добавлен!\n" +
                            $"Категория: {form.Result.CategoryText}\n" +
                            $"Название: {form.Result.Name}\n" +
                            $"Сумма: {form.Result.Amount:N0} ₽ ({form.Result.PeriodText})",
                            "Успешно",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        ShowExpensesView();
                    }
                }
            };
            topPanel.Controls.AddRange(new Control[] { lblTitle, btnAdd });

            var tablePanel = new Panel { Dock = DockStyle.Top, Height = 240, BackColor = _contentColor, Padding = new Padding(20, 0, 20, 10) };
            var dataGridView = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AllowUserToResizeRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, RowHeadersVisible = false, Font = new Font("Segoe UI", 10) };
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = _textDark;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView.ColumnHeadersHeight = 40;
            dataGridView.DefaultCellStyle.ForeColor = _textDark;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(234, 245, 251);
            dataGridView.DefaultCellStyle.SelectionForeColor = _textDark;
            dataGridView.RowTemplate.Height = 35;

            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.Columns.Add("colId", "#"); dataGridView.Columns["colId"].FillWeight = 5;
            dataGridView.Columns.Add("colCategory", "Категория"); dataGridView.Columns["colCategory"].FillWeight = 20;
            dataGridView.Columns.Add("colName", "Название"); dataGridView.Columns["colName"].FillWeight = 25;
            dataGridView.Columns.Add("colAmount", "Сумма"); dataGridView.Columns["colAmount"].DefaultCellStyle.Format = "N0"; dataGridView.Columns["colAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; dataGridView.Columns["colAmount"].FillWeight = 13;
            dataGridView.Columns.Add("colPeriod", "Период"); dataGridView.Columns["colPeriod"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; dataGridView.Columns["colPeriod"].FillWeight = 13;
            dataGridView.Columns.Add("colMonthly", "В месяц"); dataGridView.Columns["colMonthly"].DefaultCellStyle.Format = "N0"; dataGridView.Columns["colMonthly"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; dataGridView.Columns["colMonthly"].FillWeight = 13;
            dataGridView.Columns.Add("colNote", "Примечание"); dataGridView.Columns["colNote"].FillWeight = 11;

            foreach (var exp in expenses)
            {
                totalMonthly += exp.MonthlyAmount;
                dataGridView.Rows.Add(exp.Id, exp.CategoryText, exp.Name, exp.Amount, exp.PeriodText, exp.MonthlyAmount, exp.Note ?? "");
            }

            dataGridView.Rows.Add("", "", "ИТОГО:", "", "", totalMonthly, "");
            var totalRow = dataGridView.Rows[dataGridView.Rows.Count - 1];
            totalRow.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            totalRow.DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            tablePanel.Controls.Add(dataGridView);

            var summaryPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20) };
            var lblSummary = new Label { Text = $"Общий расход: {totalMonthly:N0} ₽/мес ({totalMonthly * 12:N0} ₽/год)", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.FromArgb(231, 76, 60), Location = new Point(20, 20), Size = new Size(600, 35) };
            summaryPanel.Controls.Add(lblSummary);

            _contentPanel.Controls.Add(summaryPanel);
            _contentPanel.Controls.Add(tablePanel);
            _contentPanel.Controls.Add(topPanel);
        }

        private void ShowCreditsView()
        {
            _contentPanel.Controls.Clear();
            var credits = Services.CreditService.GetDemoCredits();

            // Верхняя панель
            var topPanel = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White, Padding = new Padding(20, 12, 20, 10) };
            var lblTitle = new Label { Text = "🏠 Кредиты", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = _textDark, Location = new Point(20, 12), Size = new Size(300, 35) };
            var btnAdd = new Button { Text = "+ Добавить", Font = new Font("Segoe UI", 11, FontStyle.Bold), Location = new Point(900, 15), Size = new Size(160, 35), FlatStyle = FlatStyle.Flat, BackColor = _sidebarActiveColor, ForeColor = Color.White, Cursor = Cursors.Hand };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) =>
            {
                using (var form = new CreditForm())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show($"Кредит \"{form.Result.Name}\" добавлен!\nПлатёж: {form.Result.MonthlyPayment:N0} ₽/мес", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ShowCreditsView();
                    }
                }
            };
            topPanel.Controls.AddRange(new Control[] { lblTitle, btnAdd });

            // Таблица
            var tablePanel = new Panel { Dock = DockStyle.Top, Height = 200, BackColor = _contentColor, Padding = new Padding(20, 0, 20, 10) };
            var dataGridView = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.White, BorderStyle = BorderStyle.None, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AllowUserToResizeRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, RowHeadersVisible = false, Font = new Font("Segoe UI", 10) };
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = _textDark;
            dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView.ColumnHeadersHeight = 40;
            dataGridView.DefaultCellStyle.ForeColor = _textDark;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(234, 245, 251);
            dataGridView.DefaultCellStyle.SelectionForeColor = _textDark;
            dataGridView.RowTemplate.Height = 35;

            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.Columns.Add("colId", "#"); dataGridView.Columns["colId"].FillWeight = 5;
            dataGridView.Columns.Add("colName", "Название"); dataGridView.Columns["colName"].FillWeight = 25;
            dataGridView.Columns.Add("colType", "Тип"); dataGridView.Columns["colType"].FillWeight = 15;
            dataGridView.Columns.Add("colDebt", "Остаток"); dataGridView.Columns["colDebt"].DefaultCellStyle.Format = "N0"; dataGridView.Columns["colDebt"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; dataGridView.Columns["colDebt"].FillWeight = 18;
            dataGridView.Columns.Add("colPayment", "Платёж/мес"); dataGridView.Columns["colPayment"].DefaultCellStyle.Format = "N0"; dataGridView.Columns["colPayment"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; dataGridView.Columns["colPayment"].FillWeight = 15;
            dataGridView.Columns.Add("colRate", "Ставка"); dataGridView.Columns["colRate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; dataGridView.Columns["colRate"].FillWeight = 10;
            dataGridView.Columns.Add("colLeft", "Осталось"); dataGridView.Columns["colLeft"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; dataGridView.Columns["colLeft"].FillWeight = 12;

            foreach (var c in credits)
            {
                dataGridView.Rows.Add(c.Id, c.Name, c.TypeText, c.RemainingDebt, c.MonthlyPayment, c.InterestRate.ToString("F1") + "%", c.MonthsLeft + " мес.");
            }

            dataGridView.SelectionChanged += (s, ev) =>
            {
                if (dataGridView.SelectedRows.Count > 0)
                {
                    int index = dataGridView.SelectedRows[0].Index;
                    if (index < credits.Count) ShowCreditDetails(credits[index]);
                }
            };
            tablePanel.Controls.Add(dataGridView);

            // Детали
            var detailsPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(20) };
            _creditDetailsPanel = detailsPanel;

            _contentPanel.Controls.Add(detailsPanel);
            _contentPanel.Controls.Add(tablePanel);
            _contentPanel.Controls.Add(topPanel);

            if (credits.Count > 0) ShowCreditDetails(credits[0]);
        }

        private Panel _creditDetailsPanel;

        private void ShowCreditDetails(CreditModel credit)
        {
            if (_creditDetailsPanel == null) return;
            _creditDetailsPanel.Controls.Clear();

            var lblName = new Label { Text = $"🏠 {credit.Name}", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = _textDark, Location = new Point(20, 15), Size = new Size(600, 35) };

            var leftPanel = new Panel { Location = new Point(20, 60), Size = new Size(550, 150) };
            var lblInfo = new Label
            {
                Text = $"Тип: {credit.TypeText}    •    Платёж: {credit.PaymentTypeText}\n" +
                       $"Сумма кредита: {credit.TotalAmount:N0} ₽    •    Первый взнос: {credit.DownPayment:N0} ₽\n" +
                       $"Ставка: {credit.InterestRate}% годовых    •    Срок: {credit.TermMonths} мес.\n" +
                       $"Открыт: {credit.OpenDate.ToShortDateString()}    •    Закрытие: {credit.CloseDate.ToShortDateString()}\n" +
                       $"Выплачено осн. долга: {credit.PaidPrincipal:N0} ₽    •    Процентов: {credit.PaidInterest:N0} ₽\n" +
                       $"Остаток долга: {credit.RemainingDebt:N0} ₽    •    Осталось выплат: {credit.RemainingTotal:N0} ₽",
                Font = new Font("Segoe UI", 10),
                ForeColor = _textDark,
                Location = new Point(0, 0),
                Size = new Size(550, 145)
            };
            leftPanel.Controls.Add(lblInfo);

            var rightPanel = new Panel { Location = new Point(600, 60), Size = new Size(330, 150), BackColor = Color.FromArgb(253, 237, 236), Padding = new Padding(15) };
            var lblLoadTitle = new Label { Text = "Кредитная нагрузка", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(231, 76, 60), Location = new Point(15, 10), Size = new Size(300, 25) };
            var lblLoadValue = new Label { Text = $"{credit.MonthlyPayment:N0} ₽/мес", Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = Color.FromArgb(231, 76, 60), Location = new Point(15, 40), Size = new Size(300, 35) };
            var lblOverpayment = new Label { Text = $"Переплата: {credit.Overpayment:N0} ₽", Font = new Font("Segoe UI", 10), ForeColor = _textDark, Location = new Point(15, 85), Size = new Size(300, 22) };
            var lblTotal = new Label { Text = $"Всего выплат: {credit.TotalPayment:N0} ₽", Font = new Font("Segoe UI", 10), ForeColor = _textDark, Location = new Point(15, 110), Size = new Size(300, 22) };
            rightPanel.Controls.AddRange(new Control[] { lblLoadTitle, lblLoadValue, lblOverpayment, lblTotal });

            var btnEdit = new Button { Text = "✏ Изменить", Font = new Font("Segoe UI", 10), Location = new Point(20, 230), Size = new Size(140, 35), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(236, 240, 241), ForeColor = _textDark, Cursor = Cursors.Hand };
            btnEdit.FlatAppearance.BorderSize = 0;
            btnEdit.Click += (s, ev) =>
            {
                using (var form = new CreditForm(credit))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    { MessageBox.Show("Изменения сохранены!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information); ShowCreditsView(); }
                }
            };
            var btnDelete = new Button { Text = "🗑 Удалить", Font = new Font("Segoe UI", 10), Location = new Point(180, 230), Size = new Size(140, 35), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(253, 237, 236), ForeColor = Color.FromArgb(231, 76, 60), Cursor = Cursors.Hand };
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Click += (s, ev) =>
            {
                if (MessageBox.Show($"Удалить кредит \"{credit.Name}\"?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                { MessageBox.Show("Кредит удалён (демо).", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information); ShowCreditsView(); }
            };

            _creditDetailsPanel.Controls.AddRange(new Control[] { lblName, leftPanel, rightPanel, btnEdit, btnDelete });
        }

        private void ShowSettingsView()
        {
            _contentPanel.Controls.Clear();

            // Переменные для доступа из кнопки сохранения
            RadioButton rbFormat1 = null;
            RadioButton rbFormat2 = null;
            RadioButton rbChangesPercent = null;
            RadioButton rbChangesValue = null;
            RadioButton rbChangesBoth = null;
            TextBox txtName = null;

            // ==========================================
            // Заголовок
            // ==========================================
            var lblTitle = new Label
            {
                Text = "⚙ Настройки",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = _textDark,
                Location = new Point(30, 20),
                Size = new Size(300, 40)
            };
            _contentPanel.Controls.Add(lblTitle);

            int currentY = 80;
            int leftMargin = 30;
            int panelWidth = 850;

            // ==========================================
            // Блок: Экономические показатели
            // ==========================================
            var econGroup = CreateGroupBox("Экономические показатели", leftMargin, currentY, panelWidth, 110);
            currentY += 120;

            var lblInflation = new Label
            {
                Text = "Годовая инфляция (%):",
                Font = new Font("Segoe UI", 11),
                ForeColor = _textDark,
                Location = new Point(20, 25),
                Size = new Size(200, 25)
            };

            var lblInflationSaved = new Label
            {
                Text = "✅ Сохранено",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(46, 204, 113),
                Location = new Point(320, 28),
                Size = new Size(200, 20)
            };

            var txtInflation = new TextBox
            {
                Text = _inflationRate.ToString("F1"),
                Font = new Font("Segoe UI", 11),
                Location = new Point(230, 25),
                Size = new Size(80, 25),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250),
                ForeColor = _textDark,
                TextAlign = HorizontalAlignment.Center
            };
            txtInflation.TextChanged += (s, e) =>
            {
                if (decimal.TryParse(txtInflation.Text, out decimal val) && val >= 0 && val <= 100)
                {
                    _inflationRate = val;
                    lblInflationSaved.Text = "✅ Сохранено";
                    lblInflationSaved.ForeColor = Color.FromArgb(46, 204, 113);
                }
                else
                {
                    lblInflationSaved.Text = "❌ Некорректное значение (0–100)";
                    lblInflationSaved.ForeColor = Color.FromArgb(231, 76, 60);
                }
            };

            var lblInflationHint = new Label
            {
                Text = "Влияет на расчёт реальной стоимости резерва, прогнозы и доходность вкладов.",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(149, 165, 166),
                Location = new Point(20, 60),
                Size = new Size(800, 20)
            };

            econGroup.Controls.AddRange(new Control[] { lblInflation, txtInflation, lblInflationSaved, lblInflationHint });

            // ==========================================
            // Блок: Отображение
            // ==========================================
            var displayGroup = CreateGroupBox("Отображение", leftMargin, currentY, panelWidth, 120);
            currentY += 130;

            var lblFormat = new Label
            {
                Text = "Формат сумм:",
                Font = new Font("Segoe UI", 11),
                ForeColor = _textDark,
                Location = new Point(20, 25),
                Size = new Size(150, 25)
            };

            var formatPanel = new Panel
            {
                Location = new Point(180, 22),
                Size = new Size(300, 30),
                BackColor = Color.White
            };

            rbFormat1 = new RadioButton
            {
                Text = "100 000 ₽",
                Font = new Font("Segoe UI", 10),
                ForeColor = _textDark,
                Location = new Point(0, 3),
                Size = new Size(120, 25),
                Checked = true
            };

            rbFormat2 = new RadioButton
            {
                Text = "100000 руб.",
                Font = new Font("Segoe UI", 10),
                ForeColor = _textDark,
                Location = new Point(130, 3),
                Size = new Size(140, 25)
            };

            formatPanel.Controls.AddRange(new Control[] { rbFormat1, rbFormat2 });

            var lblChanges = new Label
            {
                Text = "Изменения на главной:",
                Font = new Font("Segoe UI", 11),
                ForeColor = _textDark,
                Location = new Point(20, 60),
                Size = new Size(200, 25)
            };

            var changesPanel = new Panel
            {
                Location = new Point(230, 57),
                Size = new Size(400, 30),
                BackColor = Color.White
            };

            rbChangesPercent = new RadioButton
            {
                Text = "Проценты (%)",
                Font = new Font("Segoe UI", 10),
                ForeColor = _textDark,
                Location = new Point(0, 3),
                Size = new Size(130, 25),
                Checked = true
            };

            rbChangesValue = new RadioButton
            {
                Text = "Сумма (₽)",
                Font = new Font("Segoe UI", 10),
                ForeColor = _textDark,
                Location = new Point(140, 3),
                Size = new Size(120, 25)
            };

            rbChangesBoth = new RadioButton
            {
                Text = "Оба",
                Font = new Font("Segoe UI", 10),
                ForeColor = _textDark,
                Location = new Point(270, 3),
                Size = new Size(80, 25)
            };

            changesPanel.Controls.AddRange(new Control[] { rbChangesPercent, rbChangesValue, rbChangesBoth });

            displayGroup.Controls.AddRange(new Control[] {
        lblFormat, formatPanel,
        lblChanges, changesPanel
    });

            // ==========================================
            // Блок: Профиль
            // ==========================================
            var profileGroup = CreateGroupBox("Профиль пользователя", leftMargin, currentY, panelWidth, 130);
            currentY += 140;

            var lblName = new Label
            {
                Text = "Имя:",
                Font = new Font("Segoe UI", 11),
                ForeColor = _textDark,
                Location = new Point(20, 25),
                Size = new Size(80, 25)
            };

            txtName = new TextBox
            {
                Text = "Зыкин Егор",
                Font = new Font("Segoe UI", 11),
                Location = new Point(100, 25),
                Size = new Size(250, 25),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250),
                ForeColor = _textDark
            };

            var lblLogin = new Label
            {
                Text = "Логин:",
                Font = new Font("Segoe UI", 11),
                ForeColor = _textDark,
                Location = new Point(20, 60),
                Size = new Size(80, 25)
            };

            var txtLogin = new TextBox
            {
                Text = "admin",
                Font = new Font("Segoe UI", 11),
                Location = new Point(100, 60),
                Size = new Size(250, 25),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(236, 240, 241),
                ForeColor = _textDark,
                ReadOnly = true
            };

            var btnChangePassword = new Button
            {
                Text = "🔒 Сменить пароль",
                Font = new Font("Segoe UI", 10),
                Location = new Point(400, 55),
                Size = new Size(180, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(236, 240, 241),
                ForeColor = _textDark,
                Cursor = Cursors.Hand
            };
            btnChangePassword.FlatAppearance.BorderSize = 0;
            btnChangePassword.Click += (s, e) =>
            {
                MessageBox.Show("Смена пароля будет доступна после подключения базы данных.",
                    "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            profileGroup.Controls.AddRange(new Control[] {
        lblName, txtName, lblLogin, txtLogin, btnChangePassword
    });

            // ==========================================
            // Блок: Данные
            // ==========================================
            var dataGroup = CreateGroupBox("Управление данными", leftMargin, currentY, panelWidth, 80);
            currentY += 90;

            var btnReset = new Button
            {
                Text = "🗑 Сбросить все данные",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 25),
                Size = new Size(220, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(253, 237, 236),
                ForeColor = Color.FromArgb(231, 76, 60),
                Cursor = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Click += (s, e) =>
            {
                var result = MessageBox.Show(
                    "Вы уверены? Все данные будут удалены безвозвратно.",
                    "Подтверждение сброса",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Данные сброшены.", "Готово",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            var btnExport = new Button
            {
                Text = "📥 Экспорт в Excel",
                Font = new Font("Segoe UI", 10),
                Location = new Point(260, 25),
                Size = new Size(220, 35),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Click += (s, e) =>
            {
                MessageBox.Show("Экспорт будет реализован позже (дополнительный модуль для ВКР).",
                    "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            dataGroup.Controls.AddRange(new Control[] { btnReset, btnExport });

            // ==========================================
            // Кнопка сохранения настроек
            // ==========================================
            var btnSaveSettings = new Button
            {
                Text = "💾 Сохранить все настройки",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(leftMargin, currentY),
                Size = new Size(panelWidth, 45),
                FlatStyle = FlatStyle.Flat,
                BackColor = _sidebarActiveColor,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnSaveSettings.FlatAppearance.BorderSize = 0;
            btnSaveSettings.Click += (s, e) =>
            {
                string formatText = rbFormat1.Checked ? "100 000 ₽" : "100000 руб.";
                string changesText = rbChangesPercent.Checked ? "Проценты" : rbChangesValue.Checked ? "Сумма (₽)" : "Оба";

                MessageBox.Show(
                    "Настройки сохранены!\n\n" +
                    $"Инфляция: {_inflationRate:F1}%\n" +
                    $"Формат сумм: {formatText}\n" +
                    $"Изменения на главной: {changesText}\n" +
                    $"Имя: {txtName.Text}",
                    "Настройки сохранены",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };
            _contentPanel.Controls.Add(btnSaveSettings);
            currentY += 60;

            // ==========================================
            // Блок: О программе
            // ==========================================
            var aboutGroup = CreateGroupBox("О программе", leftMargin, currentY, panelWidth, 95);

            var lblAbout = new Label
            {
                Text = "FinAnalyst v1.0\n" +
                       "ВКР: «Проектирование и разработка приложения для анализа\n" +
                       "и прогнозирования личных финансов»\n" +
                       "© 2026",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(149, 165, 166),
                Location = new Point(20, 20),
                Size = new Size(800, 65)
            };

            aboutGroup.Controls.Add(lblAbout);
        }

        private GroupBox CreateGroupBox(string title, int x, int y, int width, int height)
        {
            var group = new GroupBox
            {
                Text = title,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = _textDark,
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.White
            };
            _contentPanel.Controls.Add(group);
            return group;
        }

        private void ShowDashboard()
        {
            _contentPanel.Controls.Clear();

            var forecastPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White,
                Padding = new Padding(20, 15, 20, 15)
            };

            var lblForecastLabel = new Label
            {
                Text = "Прогноз на дату:",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = _textDark,
                Location = new Point(20, 20),
                Size = new Size(130, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var dtpForecast = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 11),
                Location = new Point(155, 20),
                Size = new Size(140, 25),
                Value = DateTime.Now.AddYears(1)
            };

            var btnCalculate = new Button
            {
                Text = "Рассчитать",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(310, 18),
                Size = new Size(130, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = _sidebarActiveColor,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnCalculate.FlatAppearance.BorderSize = 0;
            btnCalculate.Click += (s, e) =>
            {
                MessageBox.Show(
                    $"Прогноз на {dtpForecast.Value.ToShortDateString()}\n\n" +
                    "Расчёт будет доступен после добавления данных.",
                    "Прогноз",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            };

            forecastPanel.Controls.AddRange(new Control[] {
            lblForecastLabel, dtpForecast, btnCalculate
            });

            var cardsPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 180,
                BackColor = _contentColor,
                Padding = new Padding(20, 10, 20, 10)
            };

            var cardsData = new[]
            {
                new { Title = "Вклады", Value = "560 000 ₽", Change = "7.4%", IsPositive = true, Color = Color.FromArgb(46, 204, 113) },
                new { Title = "Акции", Value = "340 000 ₽", Change = "12.3%", IsPositive = true, Color = Color.FromArgb(46, 204, 113) },
                new { Title = "Валюты", Value = "210 000 ₽", Change = "3.1%", IsPositive = false, Color = Color.FromArgb(231, 76, 60) },
                new { Title = "Металлы", Value = "90 000 ₽", Change = "5.8%", IsPositive = true, Color = Color.FromArgb(46, 204, 113) },
                new { Title = "Резерв", Value = "120 000 ₽", Change = "8.0%", IsPositive = false, Color = Color.FromArgb(231, 76, 60) },
                new { Title = "Кредиты", Value = "−56 000 ₽/мес", Change = "41% нагр.", IsPositive = false, Color = Color.FromArgb(231, 76, 60) }
            };

            int cardWidth = 170;
            int cardHeight = 145;
            int spacing = 15;
            int startX = 20;

            for (int i = 0; i < cardsData.Length; i++)
            {
                var card = CreateAssetCard(
                    cardsData[i].Title,
                    cardsData[i].Value,
                    cardsData[i].Change,
                    cardsData[i].IsPositive,
                    cardsData[i].Color,
                    cardWidth,
                    cardHeight);

                card.Location = new Point(startX + (cardWidth + spacing) * i, 10);
                cardsPanel.Controls.Add(card);
            }

            var summaryPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = _contentColor,
                Padding = new Padding(20, 10, 20, 10)
            };

            // Блок "Общий баланс"
            var balanceBox = new Panel
            {
                Size = new Size(350, 95),
                Location = new Point(20, 10),
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var lblBalanceTitle = new Label
            {
                Text = "💰 Общий баланс",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = _textDark,
                Location = new Point(15, 10),
                Size = new Size(320, 25)
            };

            var lblBalanceValue = new Label
            {
                Text = "1 320 000 ₽",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Location = new Point(15, 40),
                Size = new Size(320, 40)
            };

            balanceBox.Controls.AddRange(new Control[] { lblBalanceTitle, lblBalanceValue });

            // Блок "Прогноз"
            var forecastBox = new Panel
            {
                Size = new Size(350, 95),
                Location = new Point(390, 10),
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var lblForecastTitle = new Label
            {
                Text = "📈 Прогноз (реальный)",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = _textDark,
                Location = new Point(15, 10),
                Size = new Size(320, 25)
            };

            var lblForecastValue = new Label
            {
                Text = "1 450 000 ₽",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = _sidebarActiveColor,
                Location = new Point(15, 40),
                Size = new Size(320, 40)
            };

            forecastBox.Controls.AddRange(new Control[] { lblForecastTitle, lblForecastValue });

            // Блок "Кредитная нагрузка"
            var creditBox = new Panel
            {
                Size = new Size(350, 95),
                Location = new Point(760, 10),
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            var lblCreditTitle = new Label
            {
                Text = "🏠 Кредитная нагрузка",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = _textDark,
                Location = new Point(15, 10),
                Size = new Size(320, 25)
            };

            var lblCreditValue = new Label
            {
                Text = "56 000 ₽/мес (41%)",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(243, 156, 18),  // жёлтый — повышенная нагрузка
                Location = new Point(15, 45),
                Size = new Size(320, 35)
            };

            creditBox.Controls.AddRange(new Control[] { lblCreditTitle, lblCreditValue });

            summaryPanel.Controls.AddRange(new Control[] { balanceBox, forecastBox, creditBox });

            var chartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(20)
            };

            var lblChartPlaceholder = new Label
            {
                Text = "📊 График прогноза\n\nЗдесь будет отображаться график роста капитала\nс учётом доходов, расходов и кредитов.",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(149, 165, 166),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            chartPanel.Controls.Add(lblChartPlaceholder);

            _contentPanel.Controls.Add(chartPanel);
            _contentPanel.Controls.Add(summaryPanel);
            _contentPanel.Controls.Add(cardsPanel);
            _contentPanel.Controls.Add(forecastPanel);
        }

        private Panel CreateAssetCard(string title, string value, string change,
            bool isPositive, Color accentColor, int width, int height)
        {
            var card = new Panel
            {
                Size = new Size(width, height),
                BackColor = Color.White,
                Padding = new Padding(12)
            };

            // Цветная полоса-индикатор сверху
            var indicator = new Panel
            {
                Size = new Size(width, 4),
                Location = new Point(0, 0),
                BackColor = accentColor
            };

            // Название актива
            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = _textDark,
                Location = new Point(12, 15),
                Size = new Size(width - 24, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Изменение (зелёное или красное)
            var changeColor = isPositive ? Color.FromArgb(46, 204, 113) : Color.FromArgb(231, 76, 60);
            var changeSign = isPositive ? "+" : "";

            var lblChange = new Label
            {
                Text = $"{changeSign}{change}",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = changeColor,
                Location = new Point(12, 50),
                Size = new Size(width - 24, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Значение
            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(12, 85),
                Size = new Size(width - 24, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Прогресс-бар (простая полоса)
            var progressBg = new Panel
            {
                Size = new Size(width - 24, 6),
                Location = new Point(12, 120),
                BackColor = Color.FromArgb(236, 240, 241)
            };

            // Заполнение прогресс-бара (случайное для демонстрации)
            int fillPercent = isPositive ? 65 : 30;
            var progressFill = new Panel
            {
                Size = new Size((width - 24) * fillPercent / 100, 6),
                Location = new Point(0, 0),
                BackColor = accentColor
            };
            progressBg.Controls.Add(progressFill);

            card.Controls.AddRange(new Control[] {
            indicator, lblTitle, lblChange, lblValue, progressBg
            });

            return card;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(1366, 834);
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}