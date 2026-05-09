using System;
using System.Drawing;
using System.Windows.Forms;
using FinancialAnalyzer.Models;

namespace FinancialAnalyzer.Forms
{
    public partial class ReserveForm : Form
    {
        private readonly Color _accentColor = Color.FromArgb(41, 128, 185);
        private readonly Color _textColor = Color.FromArgb(44, 62, 80);

        private TextBox _txtName;
        private TextBox _txtAmount;
        private TextBox _txtNote;
        private DateTimePicker _dtpCreated;
        private Label _lblError;

        public ReserveModel Result { get; private set; }
        private ReserveModel _existingReserve;

        public ReserveForm(ReserveModel existing = null)
        {
            InitializeComponent();
            _existingReserve = existing;
            if (existing != null) this.Text = "Редактировать резерв";
            SetupForm();
            if (existing != null) LoadData();
        }

        private void LoadData()
        {
            _txtName.Text = _existingReserve.Name;
            _txtAmount.Text = _existingReserve.Amount.ToString("F0");
            _txtNote.Text = _existingReserve.Note;
            _dtpCreated.Value = _existingReserve.CreatedAt;
        }

        private void SetupForm()
        {
            this.SuspendLayout();
            int left = 30, width = 370, y = 25;

            AddLabel("Название резерва:", left, y); y += 22;
            _txtName = AddTextBox(left, y, width, "Например: Подушка безопасности"); y += 45;

            AddLabel("Сумма (₽):", left, y); y += 22;
            _txtAmount = AddTextBox(left, y, width, "100000"); y += 45;

            AddLabel("Дата создания:", left, y); y += 22;
            _dtpCreated = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10),
                Location = new Point(left, y),
                Size = new Size(width, 25),
                Value = DateTime.Now
            };
            this.Controls.Add(_dtpCreated);
            y += 45;

            AddLabel("Примечание:", left, y); y += 22;
            _txtNote = AddTextBox(left, y, width, "Необязательно"); y += 45;

            _lblError = new Label
            {
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new Point(left, y),
                Size = new Size(width, 20)
            };
            this.Controls.Add(_lblError);
            y += 30;

            var btnSave = CreateButton("Сохранить", left, y, 180, _accentColor, Color.White);
            btnSave.Click += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(_txtName.Text) || _txtName.Text.Contains("Например"))
                { _lblError.Text = "Введите название"; return; }
                if (!decimal.TryParse(_txtAmount.Text, out decimal amt) || amt <= 0)
                { _lblError.Text = "Введите корректную сумму"; return; }

                Result = new ReserveModel
                {
                    Id = _existingReserve?.Id ?? 0,
                    Name = _txtName.Text.Trim(),
                    Amount = amt,
                    CreatedAt = _dtpCreated.Value,
                    Note = _txtNote.Text.Contains("Необязательно") ? "" : _txtNote.Text.Trim()
                };
                _lblError.Text = "";
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            var btnCancel = CreateButton("Отмена", left + 195, y, 175,
                Color.FromArgb(236, 240, 241), _textColor);
            btnCancel.Click += (s, ev) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] { btnSave, btnCancel });
            this.ResumeLayout(false);
        }

        private void AddLabel(string text, int x, int y)
        {
            this.Controls.Add(new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 10),
                ForeColor = _textColor,
                Location = new Point(x, y),
                Size = new Size(370, 20)
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
            tb.Enter += (s, ev) => { if (tb.Text == placeholder) { tb.Text = ""; tb.ForeColor = _textColor; } };
            tb.Leave += (s, ev) => { if (string.IsNullOrWhiteSpace(tb.Text)) { tb.Text = placeholder; tb.ForeColor = Color.FromArgb(189, 195, 199); } };
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
            // ReserveForm
            // 
            this.ClientSize = new System.Drawing.Size(433, 421);
            this.Name = "ReserveForm";
            this.ResumeLayout(false);

        }
    }
}