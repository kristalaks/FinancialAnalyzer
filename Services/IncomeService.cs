using System;
using System.Collections.Generic;
using FinancialAnalyzer.Models;

namespace FinancialAnalyzer.Services
{
    public static class IncomeService
    {
        public static List<IncomeModel> GetDemoIncomes()
        {
            return new List<IncomeModel>
            {
                new IncomeModel
                {
                    Id = 1,
                    Source = IncomeModel.IncomeSourceEnum.Salary,
                    AmountPerPayment = 80000m,
                    PaymentsPerMonth = 2,
                    IsAfterTax = true,
                    StartDate = new DateTime(2023, 1, 1),
                    TargetDepositId = 1,
                    TargetDepositName = "Сбербанк — Накопительный"
                },
                new IncomeModel
                {
                    Id = 2,
                    Source = IncomeModel.IncomeSourceEnum.Freelance,
                    CustomName = "Веб-разработка",
                    AmountPerPayment = 35000m,
                    PaymentsPerMonth = 1,
                    IsAfterTax = false,
                    StartDate = new DateTime(2024, 3, 1)
                },
                new IncomeModel
                {
                    Id = 3,
                    Source = IncomeModel.IncomeSourceEnum.Rental,
                    AmountPerPayment = 25000m,
                    PaymentsPerMonth = 1,
                    IsAfterTax = true,
                    StartDate = new DateTime(2024, 1, 15)
                }
            };
        }
    }
}