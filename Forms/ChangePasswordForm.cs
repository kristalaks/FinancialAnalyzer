using System;
using System.Drawing;
using System.Windows.Forms;

namespace FinancialAnalyzer.Forms
{
    public partial class ChangePasswordForm : Form
    {
        private readonly Color _primaryColor = Color.FromArgb(52, 73, 94);
        private readonly Color _accentColor = Color.FromArgb(41, 128, 185);
        private readonly Color _errorColor = Color.FromArgb(231, 76, 60);
        private readonly Color _successColor = Color.FromArgb(46, 204, 113);
        private readonly Color _textColor = Color.FromArgb(44, 62, 80);

        private TextBox _txtOldPassword;
        private TextBox _txtNewPassword;
        private TextBox _txtConfirmPassword;
        private Label _lblMessage;

        public ChangePasswordForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.SuspendLayout();
            int left = 40, width = 300, y = 30;

            var lblTitle = new Label
            {
                Text = "🔒 Смена пароля",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = _primaryColor,
                Location = new Point(left, y),
                Size = new Size(width, 35)
            };
            y += 50;

            // Старый пароль
            AddLabel("Текущий пароль:", left, y); y += 22;
            _txtOldPassword = AddPasswordBox(left, y, width, "Введите текущий пароль"); y += 45;

            // Новый пароль
            AddLabel("Новый пароль:", left, y); y += 22;
            _txtNewPassword = AddPasswordBox(left, y, width, "Минимум 4 символа"); y += 45;

            // Подтверждение
            AddLabel("Подтвердите пароль:", left, y); y += 22;
            _txtConfirmPassword = AddPasswordBox(left, y, width, "Повторите новый пароль"); y += 45;

            // Сообщение
            _lblMessage = new Label
            {
                Font = new Font("Segoe UI", 9),
                Location = new Point(left, y),
                Size = new Size(width, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(_lblMessage);
            y += 30;

            // Кнопки
            var btnSave = new Button
            {
                Text = "Сменить пароль",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(left, y),
                Size = new Size(width, 40),
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
                Font = new Font("Segoe UI", 11),
                Location = new Point(left, y + 50),
                Size = new Size(width, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(236, 240, 241),
                ForeColor = _textColor,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.Close(); };

            this.Controls.AddRange(new Control[] { lblTitle, btnSave, btnCancel });
            this.ResumeLayout(false);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string oldPassword = _txtOldPassword.Text;
            string newPassword = _txtNewPassword.Text;
            string confirm = _txtConfirmPassword.Text;

            // Валидация
            if (string.IsNullOrWhiteSpace(oldPassword) || oldPassword == "Введите текущий пароль")
            {
                ShowMessage("Введите текущий пароль", _errorColor);
                return;
            }
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword == "Минимум 4 символа")
            {
                ShowMessage("Введите новый пароль", _errorColor);
                return;
            }
            if (newPassword.Length < 4)
            {
                ShowMessage("Пароль должен быть не менее 4 символов", _errorColor);
                return;
            }
            if (newPassword != confirm)
            {
                ShowMessage("Новые пароли не совпадают", _errorColor);
                return;
            }

            // Проверка старого пароля и смена
            bool success = Services.AuthService.ChangePassword(oldPassword, newPassword);

            if (success)
            {
                ShowMessage("✅ Пароль успешно изменён!", _successColor);
                var timer = new Timer { Interval = 1200 };
                timer.Tick += (s, ev) => { timer.Stop(); this.Close(); };
                timer.Start();
            }
            else
            {
                ShowMessage("❌ Неверный текущий пароль", _errorColor);
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
                Size = new Size(300, 20)
            });
        }

        private TextBox AddPasswordBox(int x, int y, int w, string placeholder)
        {
            var tb = new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Location = new Point(x, y),
                Size = new Size(w, 25),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 249, 250),
                ForeColor = Color.FromArgb(189, 195, 199),
                Text = placeholder,
                UseSystemPasswordChar = false
            };
            tb.Enter += (s, ev) =>
            {
                if (tb.Text == placeholder)
                {
                    tb.Text = "";
                    tb.ForeColor = _textColor;
                    tb.UseSystemPasswordChar = true;
                }
            };
            tb.Leave += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    tb.Text = placeholder;
                    tb.ForeColor = Color.FromArgb(189, 195, 199);
                    tb.UseSystemPasswordChar = false;
                }
            };
            this.Controls.Add(tb);
            return tb;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new Size(380, 310);
            this.Name = "ChangePasswordForm";
            this.ResumeLayout(false);
        }
    }
}