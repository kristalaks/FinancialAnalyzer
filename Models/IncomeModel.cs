using System;
using System.ComponentModel.DataAnnotations;

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

        [Key]
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
        /// <summary>Месячный доход с учётом налогов</summary>
        public decimal MonthlyAmount
        {
            get
            {
                decimal total = AmountPerPayment * PaymentsPerMonth;
                if (!IsAfterTax)
                {
                    // Вычитаем НДФЛ 13%
                    total = total * 0.87m;
                }
                return total;
            }
        }

        /// <summary>Сумма до вычета налогов</summary>
        public decimal GrossMonthlyAmount
        {
            get
            {
                return AmountPerPayment * PaymentsPerMonth;
            }
        }

        /// <summary>Сумма налога (если есть)</summary>
        public decimal TaxAmount
        {
            get
            {
                if (IsAfterTax) return 0;
                return GrossMonthlyAmount * 0.13m;
            }
        }

        public string TaxText
        {
            get
            {
                if (IsAfterTax)
                    return "После налогов";
                else
                    return $"До налогов (-{TaxAmount:N0} ₽ НДФЛ)";
            }
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
    }
}