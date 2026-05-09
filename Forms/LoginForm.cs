using System;
using System.Drawing;
using System.Windows.Forms;

namespace FinancialAnalyzer.Forms
{
    public partial class LoginForm : Form
    {
        // Цветовая схема
        private readonly Color _primaryColor = Color.FromArgb(52, 73, 94);
        private readonly Color _accentColor = Color.FromArgb(41, 128, 185);
        private readonly Color _errorColor = Color.FromArgb(231, 76, 60);
        private readonly Color _textColor = Color.FromArgb(44, 62, 80);
        private readonly Color _placeholderColor = Color.FromArgb(189, 195, 199);

        // Контролы
        private Panel _headerPanel;
        private Label _lblTitle;
        private Label _lblSubtitle;
        private Panel _formPanel;
        private Label _lblLogin;
        private TextBox _txtLogin;
        private Label _lblPassword;
        private TextBox _txtPassword;
        private Button _btnTogglePassword;
        private CheckBox _chkRemember;
        private Button _btnLogin;
        private LinkLabel _linkRegister;
        private Label _lblError;
        private bool _passwordVisible = false;

        public LoginForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.SuspendLayout();

            // === Верхняя панель (тёмно-синяя) ===
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = _primaryColor
            };

            _lblTitle = new Label
            {
                Text = "FinAnalyst",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(0, 20, 0, 0)
            };

            _lblSubtitle = new Label
            {
                Text = "Анализ и прогнозирование личных финансов",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.FromArgb(189, 195, 199),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 40
            };

            _headerPanel.Controls.Add(_lblSubtitle);
            _headerPanel.Controls.Add(_lblTitle);

            // === Панель с полями ввода ===
            _formPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(40, 30, 40, 20)
            };

            // Логин
            _lblLogin = new Label
            {
                Text = "Логин",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = _textColor,
                Location = new Point(40, 15),
                Size = new Size(320, 20)
            };

            _txtLogin = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(40, 40),
                Size = new Size(320, 30),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250),
                ForeColor = _textColor
            };
            _txtLogin.Enter += (s, e) => OnTextBoxEnter(_txtLogin, "Введите логин");
            _txtLogin.Leave += (s, e) => OnTextBoxLeave(_txtLogin, "Введите логин");
            SetPlaceholder(_txtLogin, "Введите логин");

            // Пароль
            _lblPassword = new Label
            {
                Text = "Пароль",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = _textColor,
                Location = new Point(40, 85),
                Size = new Size(320, 20)
            };

            _txtPassword = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(40, 110),
                Size = new Size(280, 30),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250),
                ForeColor = _textColor,
                UseSystemPasswordChar = true
            };
            _txtPassword.Enter += (s, e) => OnTextBoxEnter(_txtPassword, "Введите пароль");
            _txtPassword.Leave += (s, e) => OnTextBoxLeave(_txtPassword, "Введите пароль");
            SetPlaceholder(_txtPassword, "Введите пароль");

            _btnTogglePassword = new Button
            {
                Text = "👁",
                Font = new Font("Segoe UI", 11),
                Location = new Point(325, 110),
                Size = new Size(35, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(248, 249, 250),
                ForeColor = _textColor,
                Cursor = Cursors.Hand
            };
            _btnTogglePassword.FlatAppearance.BorderSize = 1;
            _btnTogglePassword.FlatAppearance.BorderColor = Color.FromArgb(206, 212, 218);
            _btnTogglePassword.Click += BtnTogglePassword_Click;

            // Запомнить меня
            _chkRemember = new CheckBox
            {
                Text = "Запомнить меня",
                Font = new Font("Segoe UI", 9),
                ForeColor = _textColor,
                Location = new Point(40, 155),
                Size = new Size(320, 20)
            };

            // Ошибка
            _lblError = new Label
            {
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = _errorColor,
                Location = new Point(40, 180),
                Size = new Size(320, 20),
                Text = "",
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Кнопка Войти
            _btnLogin = new Button
            {
                Text = "Войти",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(40, 210),
                Size = new Size(320, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = _accentColor,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            _btnLogin.FlatAppearance.BorderSize = 0;
            _btnLogin.Click += BtnLogin_Click;

            // Регистрация
            _linkRegister = new LinkLabel
            {
                Text = "Нет аккаунта? Зарегистрироваться",
                Font = new Font("Segoe UI", 9),
                Location = new Point(40, 265),
                Size = new Size(320, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                LinkColor = _accentColor,
                ActiveLinkColor = _primaryColor
            };
            _linkRegister.Click += LinkRegister_Click;

            // Добавляем контролы на панель
            _formPanel.Controls.AddRange(new Control[] {
                _lblLogin, _txtLogin,
                _lblPassword, _txtPassword, _btnTogglePassword,
                _chkRemember,
                _lblError,
                _btnLogin,
                _linkRegister
            });

            // Добавляем панели на форму
            this.Controls.Add(_formPanel);
            this.Controls.Add(_headerPanel);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void SetPlaceholder(TextBox textBox, string placeholder)
        {
            textBox.Text = placeholder;
            textBox.ForeColor = _placeholderColor;
            textBox.Tag = placeholder;
        }

        private void OnTextBoxEnter(TextBox textBox, string placeholder)
        {
            if (textBox.Text == placeholder)
            {
                textBox.Text = "";
                textBox.ForeColor = _textColor;
                if (textBox == _txtPassword)
                    textBox.UseSystemPasswordChar = true;
            }
        }

        private void OnTextBoxLeave(TextBox textBox, string placeholder)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                SetPlaceholder(textBox, placeholder);
                if (textBox == _txtPassword)
                    textBox.UseSystemPasswordChar = false;
            }
        }

        private void BtnTogglePassword_Click(object sender, EventArgs e)
        {
            _passwordVisible = !_passwordVisible;
            if (_txtPassword.Text != _txtPassword.Tag?.ToString())
            {
                _txtPassword.UseSystemPasswordChar = !_passwordVisible;
                _btnTogglePassword.Text = _passwordVisible ? "🙈" : "👁";
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string login = _txtLogin.Text.Trim();
            string password = _txtPassword.Text;

            // Валидация
            if (string.IsNullOrWhiteSpace(login) || login == "Введите логин")
            {
                ShowError("Введите логин");
                _txtLogin.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password) || password == "Введите пароль")
            {
                ShowError("Введите пароль");
                _txtPassword.Focus();
                return;
            }

            // Заглушка входа (пока без БД)
            if (login == "admin" && password == "admin")
            {
                _lblError.Text = "";
                this.DialogResult = DialogResult.OK;

                // Сохраняем логин если выбрано "Запомнить"
                if (_chkRemember.Checked)
                {
                    Properties.Settings.Default.RememberedLogin = login;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Properties.Settings.Default.RememberedLogin = "";
                    Properties.Settings.Default.Save();
                }

                this.Close();
            }
            else
            {
                ShowError("Неверный логин или пароль");
            }
        }

        private void ShowError(string message)
        {
            _lblError.Text = message;
            _lblError.Visible = true;
        }

        private void LinkRegister_Click(object sender, EventArgs e)
        {
            // Пока заглушка
            MessageBox.Show("Регистрация будет доступна после подключения базы данных.",
                "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // LoginForm
            // 
            this.ClientSize = new System.Drawing.Size(396, 433);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);

        }
    }
}