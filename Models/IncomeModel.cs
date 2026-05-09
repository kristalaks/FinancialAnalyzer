using System;

namespace FinancialAnalyzer.Models
{
    /// <summary>
    /// Модель данных для источника дохода
    /// </summary>
    public class IncomeModel
    {
        public enum IncomeSourceEnum
        {
            Salary = 0,
            Freelance = 1,
            Rental = 2,
            Investment = 3,
            Other = 4
        }

        public int Id { get; set; }
        public IncomeSourceEnum Source { get; set; }
        public string CustomName { get; set; }
        public decimal AmountPerPayment { get; set; }
        public int PaymentsPerMonth { get; set; }
        public bool IsAfterTax { get; set; }
        public DateTime StartDate { get; set; }
        public int? TargetDepositId { get; set; }
        public string TargetDepositName { get; set; }

        /// <summary>Месячный доход</summary>
        public decimal MonthlyAmount
        {
            get { return AmountPerPayment * PaymentsPerMonth; }
        }

        /// <summary>Годовой доход</summary>
        public decimal YearlyAmount
        {
            get { return MonthlyAmount * 12; }
        }

        public string SourceText
        {
            get
            {
                switch (Source)
                {
                    case IncomeSourceEnum.Salary: return "Зарплата";
                    case IncomeSourceEnum.Freelance: return "Фриланс";
                    case IncomeSourceEnum.Rental: return "Аренда";
                    case IncomeSourceEnum.Investment: return "Инвестиции";
                    case IncomeSourceEnum.Other: return CustomName ?? "Другое";
                    default: return "—";
                }
            }
        }

        public string TaxText
        {
            get { return IsAfterTax ? "После налогов" : "До налогов"; }
        }
    }
}