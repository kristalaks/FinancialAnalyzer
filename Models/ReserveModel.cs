using System;
using System.ComponentModel.DataAnnotations;

namespace FinancialAnalyzer.Models
{
    /// <summary>
    /// Модель для неработающих активов (кэш-резерв)
    /// </summary>
    public class ReserveModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Note { get; set; }

        /// <summary>Потери от инфляции (годовая инфляция ~7.8%)</summary>
        public decimal InflationLoss
        {
            get
            {
                decimal yearsPassed = (decimal)(DateTime.Now - CreatedAt).Days / 365m;
                decimal inflationRate = 0.078m;
                decimal realValue = Amount / (1 + inflationRate * yearsPassed);
                return Amount - realValue;
            }
        }

        public decimal RealValue
        {
            get { return Amount - InflationLoss; }
        }

        public decimal LossPercent
        {
            get
            {
                if (Amount == 0) return 0;
                return InflationLoss / Amount * 100m;
            }
        }

        public string ChangeText
        {
            get { return $"-{LossPercent:F1}%"; }
        }

        public bool IsPositive
        {
            get { return false; } // Резерв всегда теряет ценность
        }
    }
}