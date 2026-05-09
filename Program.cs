using System;
using System.Windows.Forms;
using FinancialAnalyzer.Forms;

namespace FinancialAnalyzer
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Показываем форму входа
            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Успешный вход — запускаем главную форму
                    Application.Run(new MainForm());
                }
                else
                {
                    // Пользователь закрыл форму входа — выходим
                    Application.Exit();
                }
            }
        }
    }
}