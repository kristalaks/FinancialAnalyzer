using System;
using System.Windows.Forms;
using FinancialAnalyzer.Data;
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

            // Инициализация базы данных
            DatabaseHelper.Initialize();

            // Показываем форму входа
            using (var loginForm = new LoginForm())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new MainForm());
                }
                else
                {
                    Application.Exit();
                }
            }
        }
    }
}