using System;
using System.Drawing;
using System.Windows.Forms;

namespace FinancialAnalyzer.Forms
{
    public partial class RegisterForm : Form
    {
        private readonly Color _primaryColor = Color.FromArgb(52, 73, 94);
        private readonly Color _accentColor = Color.FromArgb(41, 128, 185);
        private readonly Color _errorColor = Color.FromArgb(231, 76, 60);
        private readonly Color _successColor = Color.FromArgb(46, 204, 113);
        private readonly Color _textColor = Color.FromArgb(44, 62, 80);
        private readonly Color _placeholderColor = Color.FromArgb(189, 195, 199);

        private Panel _headerPanel;
        private Label _lblTitle;
        private TextBox _txtLogin;
        private TextBox _txtDisplayName;
        private TextBox _txtPassword;
        private TextBox _txtPasswordConfirm;
        private Button _btnRegister;
        private Button _btnCancel;
        private Label _lblMessage;

        public RegisterForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.SuspendLayout();

            _headerPanel = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = _primaryColor };
            _lblTitle = new Label
            {
                Text = "Регистрация",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            _headerPanel.Controls.Add(_lblTitle);

            int left = 40, width = 320, y = 100;

            // Логин
            AddLabel("Логин:", left, y); y += 22;
            _txtLogin = AddTextBox(left, y, width, "Придумайте логин"); y += 45;

            // Имя
            AddLabel("Отображаемое имя:", left, y); y += 22;
            _txtDisplayName = AddTextBox(left, y, width, "Как вас называть?"); y += 45;

            // Пароль
            AddLabel("Пароль:", left, y); y += 22;
            _txtPassword = AddTextBox(left, y, width, "Минимум 4 символа");
            _txtPassword.UseSystemPasswordChar = true;
            y += 45;

            // Подтверждение
            AddLabel("Подтвердите пароль:", left, y); y += 22;
            _txtPasswordConfirm = AddTextBox(left, y, width, "Повторите пароль");
            _txtPasswordConfirm.UseSystemPasswordChar = true;
            y += 45;

            // Сообщение
            _lblMessage = new Label
            {
                Font = new Font("Segoe UI", 9),
                Location = new Point(left, y),
                Size = new Size(width, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(_lblMessage);
            y += 35;

            // Кнопки
            _btnRegister = new Button
            {
                Text = "Зарегистрироваться",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(left, y),
                Size = new Size(width, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = _accentColor,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            _btnRegister.FlatAppearance.BorderSize = 0;
            _btnRegister.Click += BtnRegister_Click;

            _btnCancel = new Button
            {
                Text = "Отмена",
                Font = new Font("Segoe UI", 11),
                Location = new Point(left, y + 50),
                Size = new Size(width, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(236, 240, 241),
                ForeColor = _textColor,
                Cursor = Cursors.Hand
            };
            _btnCancel.FlatAppearance.BorderSize = 0;
            _btnCancel.Click += (s, e) => { this.Close(); };

            this.Controls.AddRange(new Control[] { _btnRegister, _btnCancel });
            this.Controls.Add(_headerPanel);

            this.ResumeLayout(false);
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string login = _txtLogin.Text.Trim();
            string displayName = _txtDisplayName.Text.Trim();
            string password = _txtPassword.Text;
            string confirm = _txtPasswordConfirm.Text;

            // Валидация
            if (string.IsNullOrWhiteSpace(login) || login == "Придумайте логин")
            {
                ShowMessage("Введите логин", _errorColor);
                return;
            }
            if (login.Length < 3)
            {
                ShowMessage("Логин должен быть не менее 3 символов", _errorColor);
                return;
            }
            if (string.IsNullOrWhiteSpace(password) || password == "Минимум 4 символа")
            {
                ShowMessage("Введите пароль", _errorColor);
                return;
            }
            if (password.Length < 4)
            {
                ShowMessage("Пароль должен быть не менее 4 символов", _errorColor);
                return;
            }
            if (password != confirm)
            {
                ShowMessage("Пароли не совпадают", _errorColor);
                return;
            }

            // Регистрация
            bool success = Services.AuthService.Register(login, password,
                string.IsNullOrWhiteSpace(displayName) || displayName == "Как вас называть?" ? login : displayName);

            if (success)
            {
                ShowMessage("✅ Регистрация успешна! Теперь войдите.", _successColor);
                _btnRegister.Enabled = false;

                // Закрываем через 1.5 секунды
                var timer = new Timer { Interval = 1500 };
                timer.Tick += (s, ev) => { timer.Stop(); this.DialogResult = DialogResult.OK; this.Close(); };
                timer.Start();
            }
            else
            {
                ShowMessage("❌ Пользователь с таким логином уже существует", _errorColor);
            }
        }

        private void ShowMessage(string text, Color color)
        {
            _lblMessage.Text = text;
            _lblMessage.ForeColor = color;
        }

        private void AddLabel(string text, int x, int y)
        {
            this.Controls.Add(new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10),
                ForeColor = _textColor,
                Location = new Point(x, y),
                Size = new Size(320, 20)
            });
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
                ForeColor = _textColor,
                Text = placeholder
            };
            tb.Enter += (s, ev) =>
            {
                if (tb.Text == placeholder)
                {
                    tb.Text = "";
                    tb.ForeColor = _textColor;
                    if (tb == _txtPassword || tb == _txtPasswordConfirm)
                        tb.UseSystemPasswordChar = true;
                }
            };
            tb.Leave += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    tb.Text = placeholder;
                    tb.ForeColor = _placeholderColor;
                    if (tb == _txtPassword || tb == _txtPasswordConfirm)
                        tb.UseSystemPasswordChar = false;
                }
            };
            this.Controls.Add(tb);
            return tb;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // RegisterForm
            // 
            this.ClientSize = new System.Drawing.Size(405, 567);
            this.Name = "RegisterForm";
            this.Text = "Регистрация";
            this.ResumeLayout(false);

        }
    }
}