using System;
using System.Collections.Generic;
using FinancialAnalyzer.Models;

namespace FinancialAnalyzer.Services
{

    public static class DepositService
    {

        public static decimal CalculateCurrentAmount(DepositModel deposit)
        {
            // Сколько дней прошло с открытия
            int daysPassed = (DateTime.Now - deposit.OpenDate).Days;
            if (daysPassed <= 0)
                return deposit.InitialAmount;

            // Переводим годовую ставку в дневную
            decimal dailyRate = deposit.InterestRate / 100m / 365m;

            if (deposit.RateType == 0)
            {
                // Простой процент
                decimal interest = deposit.InitialAmount * dailyRate * daysPassed;
                return deposit.InitialAmount + interest;
            }
            else
            {
                // Сложный процент (капитализация раз в месяц)
                int monthsPassed = daysPassed / 30;
                if (monthsPassed <= 0)
                    return deposit.InitialAmount;

                decimal monthlyRate = deposit.InterestRate / 100m / 12m;
                decimal result = deposit.InitialAmount;

                for (int i = 0; i < monthsPassed; i++)
                {
                    result = result + result * monthlyRate;
                }

                // Добавляем проценты за оставшиеся дни неполного месяца
                int remainingDays = daysPassed % 30;
                if (remainingDays > 0)
                {
                    result = result + result * dailyRate * remainingDays;
                }

                return result;
            }
        }

        public static List<DepositModel> GetDemoDeposits()
        {
            return new List<DepositModel>
            {
                new DepositModel
                {
                    Id = 1,
                    Name = "Сбербанк — Накопительный",
                    InitialAmount = 300000m,
                    InterestRate = 14.5m,
                    RateType = 1,
                    OpenDate = new DateTime(2024, 3, 15),
                    CloseDate = null,
                    CurrentAmount = 352000m,
                    Profit = 52000m,
                    ProfitPercent = 17.3m
                },
                new DepositModel
                {
                    Id = 2,
                    Name = "ВТБ — Срочный",
                    InitialAmount = 150000m,
                    InterestRate = 12.0m,
                    RateType = 0,
                    OpenDate = new DateTime(2024, 6, 1),
                    CloseDate = new DateTime(2025, 6, 1),
                    CurrentAmount = 163500m,
                    Profit = 13500m,
                    ProfitPercent = 9.0m
                },
                new DepositModel
                {
                    Id = 3,
                    Name = "Альфа-Банк — Премиум",
                    InitialAmount = 110000m,
                    InterestRate = 16.0m,
                    RateType = 1,
                    OpenDate = new DateTime(2024, 11, 20),
                    CloseDate = null,
                    CurrentAmount = 118200m,
                    Profit = 8200m,
                    ProfitPercent = 7.5m
                }
            };
        }
    }
}