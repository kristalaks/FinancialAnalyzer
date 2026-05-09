using System;
using System.Collections.Generic;
using FinancialAnalyzer.Models;

namespace FinancialAnalyzer.Services
{
    public static class CreditService
    {
        /// <summary>
        /// Расчёт аннуитетного платежа
        /// </summary>
        public static decimal CalculateAnnuityPayment(decimal loanAmount, decimal annualRate, int months)
        {
            if (months <= 0 || loanAmount <= 0) return 0;
            decimal monthlyRate = annualRate / 100m / 12m;
            if (monthlyRate == 0) return loanAmount / months;
            decimal factor = (decimal)Math.Pow((double)(1 + monthlyRate), months);
            return loanAmount * monthlyRate * factor / (factor - 1);
        }

        public static List<CreditModel> GetDemoCredits()
        {
            decimal mortgagePayment = CalculateAnnuityPayment(5000000m, 10.2m, 180);

            return new List<CreditModel>
            {
                new CreditModel
                {
                    Id = 1,
                    Name = "Ипотека Сбер",
                    Type = CreditModel.CreditTypeEnum.Mortgage,
                    TotalAmount = 5000000m,
                    DownPayment = 1000000m,
                    InterestRate = 10.2m,
                    TermMonths = 180,
                    PaymentType = CreditModel.PaymentTypeEnum.Annuity,
                    OpenDate = new DateTime(2023, 6, 10),
                    MonthlyPayment = Math.Round(mortgagePayment, 0),
                    RemainingDebt = 3200000m,
                    PaidPrincipal = 800000m,
                    PaidInterest = 580000m
                },
                new CreditModel
                {
                    Id = 2,
                    Name = "Автокредит ВТБ",
                    Type = CreditModel.CreditTypeEnum.CarLoan,
                    TotalAmount = 800000m,
                    DownPayment = 200000m,
                    InterestRate = 14.5m,
                    TermMonths = 60,
                    PaymentType = CreditModel.PaymentTypeEnum.Differentiated,
                    OpenDate = new DateTime(2024, 3, 15),
                    MonthlyPayment = 14300m,
                    RemainingDebt = 450000m,
                    PaidPrincipal = 150000m,
                    PaidInterest = 42000m
                },
                new CreditModel
                {
                    Id = 3,
                    Name = "Потребительский Альфа",
                    Type = CreditModel.CreditTypeEnum.Consumer,
                    TotalAmount = 200000m,
                    DownPayment = 0,
                    InterestRate = 18.0m,
                    TermMonths = 36,
                    PaymentType = CreditModel.PaymentTypeEnum.Annuity,
                    OpenDate = new DateTime(2024, 8, 1),
                    MonthlyPayment = 7200m,
                    RemainingDebt = 120000m,
                    PaidPrincipal = 80000m,
                    PaidInterest = 19200m
                }
            };
        }
    }
}