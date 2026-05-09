using System;
using System.ComponentModel.DataAnnotations;

namespace FinancialAnalyzer.Models
{
    /// <summary>
    /// Модель данных для расхода
    /// </summary>
    public class ExpenseModel
    {
        
        public enum ExpenseCategoryEnum
        {
            Food = 0,
            Housing = 1,
            Transport = 2,
            Communication = 3,
            Clothing = 4,
            Health = 5,
            Entertainment = 6,
            Education = 7,
            Taxes = 8,
            Credit = 9,
            Custom = 10
        }

        public enum ExpensePeriodEnum
        {
            Once = 0,
            Monthly = 1,
            Yearly = 2
        }

        [Key]
        public int Id { get; set; }
        public ExpenseCategoryEnum Category { get; set; }
        public string CustomCategoryName { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public ExpensePeriodEnum Period { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public int? SourceReserveId { get; set; }
        public string SourceReserveName { get; set; }

        /// <summary>Месячный эквивалент расхода</summary>
        public decimal MonthlyAmount
        {
            get
            {
                switch (Period)
                {
                    case ExpensePeriodEnum.Monthly: return Amount;
                    case ExpensePeriodEnum.Yearly: return Amount / 12;
                    case ExpensePeriodEnum.Once: return 0;
                    default: return 0;
                }
            }
        }

        public string CategoryText
        {
            get
            {
                switch (Category)
                {
                    case ExpenseCategoryEnum.Food: return "Продукты";
                    case ExpenseCategoryEnum.Housing: return "ЖКХ";
                    case ExpenseCategoryEnum.Transport: return "Транспорт";
                    case ExpenseCategoryEnum.Communication: return "Связь/Интернет";
                    case ExpenseCategoryEnum.Clothing: return "Одежда";
                    case ExpenseCategoryEnum.Health: return "Здоровье";
                    case ExpenseCategoryEnum.Entertainment: return "Развлечения";
                    case ExpenseCategoryEnum.Education: return "Образование";
                    case ExpenseCategoryEnum.Taxes: return "Налоги";
                    case ExpenseCategoryEnum.Credit: return "Кредиты";
                    case ExpenseCategoryEnum.Custom: return CustomCategoryName ?? "Другое";
                    default: return "—";
                }
            }
        }

        public string PeriodText
        {
            get
            {
                switch (Period)
                {
                    case ExpensePeriodEnum.Once: return "Разовый";
                    case ExpensePeriodEnum.Monthly: return "Ежемесячный";
                    case ExpensePeriodEnum.Yearly: return "Ежегодный";
                    default: return "—";
                }
            }
        }
    }
}