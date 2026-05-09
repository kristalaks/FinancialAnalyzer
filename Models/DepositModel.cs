using System;

namespace FinancialAnalyzer.Models
{
    public class DepositModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal InitialAmount { get; set; }

        public decimal InterestRate { get; set; }

        public int RateType { get; set; }

        public DateTime OpenDate { get; set; }

        public DateTime? CloseDate { get; set; }

        public decimal CurrentAmount { get; set; }

        public decimal Profit { get; set; }

        public decimal ProfitPercent { get; set; }

        public string RateTypeText
        {
            get
            {
                if (RateType == 0)
                    return "Простой";
                else
                    return "Сложный";
            }
        }

        public string ChangeText
        {
            get
            {
                string sign = Profit >= 0 ? "+" : "";
                return $"{sign}{ProfitPercent:F1}%";
            }
        }

        public bool IsPositive
        {
            get { return Profit >= 0; }
        }
    }
}