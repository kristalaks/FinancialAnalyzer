using System;
using System.Collections.Generic;
using FinancialAnalyzer.Models;

namespace FinancialAnalyzer.Services
{
    public static class ExpenseService
    {
        public static List<ExpenseModel> GetDemoExpenses()
        {
            return new List<ExpenseModel>
            {
                new ExpenseModel
                {
                    Id = 1,
                    Category = ExpenseModel.ExpenseCategoryEnum.Food,
                    Name = "Продукты на неделю",
                    Amount = 15000m,
                    Period = ExpenseModel.ExpensePeriodEnum.Monthly,
                    Date = new DateTime(2024, 1, 1)
                },
                new ExpenseModel
                {
                    Id = 2,
                    Category = ExpenseModel.ExpenseCategoryEnum.Housing,
                    Name = "Квартплата и ЖКХ",
                    Amount = 8000m,
                    Period = ExpenseModel.ExpensePeriodEnum.Monthly,
                    Date = new DateTime(2024, 1, 1)
                },
                new ExpenseModel
                {
                    Id = 3,
                    Category = ExpenseModel.ExpenseCategoryEnum.Transport,
                    Name = "Проездной",
                    Amount = 3000m,
                    Period = ExpenseModel.ExpensePeriodEnum.Monthly,
                    Date = new DateTime(2024, 1, 1)
                },
                new ExpenseModel
                {
                    Id = 4,
                    Category = ExpenseModel.ExpenseCategoryEnum.Entertainment,
                    Name = "Кино и кафе",
                    Amount = 5000m,
                    Period = ExpenseModel.ExpensePeriodEnum.Monthly,
                    Date = new DateTime(2024, 1, 1)
                },
                new ExpenseModel
                {
                    Id = 5,
                    Category = ExpenseModel.ExpenseCategoryEnum.Credit,
                    Name = "Платёж по ипотеке",
                    Amount = 38500m,
                    Period = ExpenseModel.ExpensePeriodEnum.Monthly,
                    Date = new DateTime(2024, 1, 1),
                    Note = "Ипотека Сбер"
                },
                new ExpenseModel
                {
                    Id = 6,
                    Category = ExpenseModel.ExpenseCategoryEnum.Clothing,
                    Name = "Зимняя куртка",
                    Amount = 15000m,
                    Period = ExpenseModel.ExpensePeriodEnum.Once,
                    Date = new DateTime(2024, 11, 20)
                }
            };
        }
    }
}