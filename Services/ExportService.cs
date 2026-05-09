using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using FinancialAnalyzer.Data;
using FinancialAnalyzer.Models;

namespace FinancialAnalyzer.Services
{
    public static class ExportService
    {
        public static void ExportAllData()
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*";
                saveDialog.FileName = $"FinAnalyst_export_{DateTime.Now:yyyy-MM-dd}.csv";
                saveDialog.Title = "Экспорт данных";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var sb = new StringBuilder();

                    // Вклады
                    sb.AppendLine("=== ВКЛАДЫ ===");
                    sb.AppendLine("Название;Сумма;Ставка;Тип;Дата открытия;Текущая сумма;Прибыль");
                    foreach (var d in DepositService.GetAll())
                    {
                        sb.AppendLine($"{d.Name};{d.InitialAmount};{d.InterestRate}%;{d.RateTypeText};{d.OpenDate:d};{d.CurrentAmount};{d.Profit}");
                    }
                    sb.AppendLine();

                    // Акции
                    sb.AppendLine("=== АКЦИИ ===");
                    sb.AppendLine("Тикер;Компания;Кол-во;Цена покупки;Текущая цена;Стоимость;Изменение %");
                    foreach (var a in AssetService.GetByType(AssetModel.AssetTypeEnum.Stock))
                    {
                        sb.AppendLine($"{a.Ticker};{a.CompanyName};{a.Quantity};{a.PurchasePrice};{a.CurrentPrice};{a.CurrentTotalValue};{a.ProfitPercent:F1}%");
                    }
                    sb.AppendLine();

                    // Валюты
                    sb.AppendLine("=== ВАЛЮТЫ ===");
                    sb.AppendLine("Тикер;Название;Кол-во;Цена покупки;Текущий курс;Стоимость;Изменение %");
                    foreach (var a in AssetService.GetByType(AssetModel.AssetTypeEnum.Currency))
                    {
                        sb.AppendLine($"{a.Ticker};{a.CompanyName};{a.Quantity};{a.PurchasePrice};{a.CurrentPrice};{a.CurrentTotalValue};{a.ProfitPercent:F1}%");
                    }
                    sb.AppendLine();

                    // Металлы
                    sb.AppendLine("=== МЕТАЛЛЫ ===");
                    sb.AppendLine("Тикер;Название;Кол-во;Цена покупки;Текущая цена;Стоимость;Изменение %");
                    foreach (var a in AssetService.GetByType(AssetModel.AssetTypeEnum.Metal))
                    {
                        sb.AppendLine($"{a.Ticker};{a.CompanyName};{a.Quantity};{a.PurchasePrice};{a.CurrentPrice};{a.CurrentTotalValue};{a.ProfitPercent:F1}%");
                    }
                    sb.AppendLine();

                    // Резерв
                    sb.AppendLine("=== РЕЗЕРВ ===");
                    sb.AppendLine("Название;Сумма;Дата создания;Потери от инфляции;Реальная стоимость");
                    foreach (var r in ReserveService.GetAll())
                    {
                        sb.AppendLine($"{r.Name};{r.Amount};{r.CreatedAt:d};{r.InflationLoss};{r.RealValue}");
                    }
                    sb.AppendLine();

                    // Доходы
                    sb.AppendLine("=== ДОХОДЫ ===");
                    sb.AppendLine("Источник;Сумма/выплата;Выплат/мес;В месяц;Налоги;Направление");
                    foreach (var i in IncomeService.GetAll())
                    {
                        sb.AppendLine($"{i.SourceText};{i.AmountPerPayment};{i.PaymentsPerMonth};{i.MonthlyAmount};{i.TaxText};{i.TargetDepositName ?? "—"}");
                    }
                    sb.AppendLine();

                    // Расходы
                    sb.AppendLine("=== РАСХОДЫ ===");
                    sb.AppendLine("Категория;Название;Сумма;Период;В месяц;Примечание");
                    foreach (var e in ExpenseService.GetAll())
                    {
                        sb.AppendLine($"{e.CategoryText};{e.Name};{e.Amount};{e.PeriodText};{e.MonthlyAmount};{e.Note ?? ""}");
                    }
                    sb.AppendLine();

                    // Кредиты
                    sb.AppendLine("=== КРЕДИТЫ ===");
                    sb.AppendLine("Название;Тип;Сумма;Остаток;Платёж/мес;Ставка;Осталось мес.");
                    foreach (var c in CreditService.GetAll())
                    {
                        sb.AppendLine($"{c.Name};{c.TypeText};{c.TotalAmount};{c.RemainingDebt};{c.MonthlyPayment};{c.InterestRate}%;{c.MonthsLeft}");
                    }
                    sb.AppendLine();

                    // Сводка
                    var incomes = IncomeService.GetAll();
                    var expenses = ExpenseService.GetAll();
                    var credits = CreditService.GetAll();
                    decimal totalIncome = 0, totalExpense = 0, totalCredit = 0;
                    foreach (var i in incomes) totalIncome += i.MonthlyAmount;
                    foreach (var e in expenses) totalExpense += e.MonthlyAmount;
                    foreach (var c in credits) totalCredit += c.MonthlyPayment;

                    sb.AppendLine("=== СВОДКА ===");
                    sb.AppendLine($"Доходы/мес;{totalIncome}");
                    sb.AppendLine($"Расходы/мес;{totalExpense}");
                    sb.AppendLine($"Кредиты/мес;{totalCredit}");
                    sb.AppendLine($"Свободный поток;{totalIncome - totalExpense - totalCredit}");

                    File.WriteAllText(saveDialog.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show($"Данные экспортированы в:\n{saveDialog.FileName}", "Экспорт завершён",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}