using System;

namespace FinancialAnalyzer.Models
{
    /// <summary>
    /// Модель данных для рыночного актива (акции, валюты, металлы)
    /// </summary>
    public class AssetModel
    {
        /// <summary>Тип актива</summary>
        public enum AssetTypeEnum
        {
            Stock = 0,      // Акции
            Currency = 1,   // Валюта
            Metal = 2       // Металлы
        }

        /// <summary>Уникальный номер</summary>
        public int Id { get; set; }

        /// <summary>Тип актива</summary>
        public AssetTypeEnum Type { get; set; }

        /// <summary>Тикер (краткий код: SBER, AAPL, USD/RUB)</summary>
        public string Ticker { get; set; }

        /// <summary>Полное название компании или валютной пары</summary>
        public string CompanyName { get; set; }

        /// <summary>Биржа: MOEX, NASDAQ, NYSE, Forex</summary>
        public string Exchange { get; set; }

        /// <summary>Количество (штук или единиц валюты)</summary>
        public decimal Quantity { get; set; }

        /// <summary>Цена покупки за единицу</summary>
        public decimal PurchasePrice { get; set; }

        /// <summary>Дата покупки</summary>
        public DateTime PurchaseDate { get; set; }

        /// <summary>Текущая цена за единицу (из API или заглушка)</summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>Текущая общая стоимость</summary>
        public decimal CurrentTotalValue
        {
            get { return Quantity * CurrentPrice; }
        }

        /// <summary>Сумма вложений</summary>
        public decimal InvestedAmount
        {
            get { return Quantity * PurchasePrice; }
        }

        /// <summary>Прибыль/убыток в рублях</summary>
        public decimal Profit
        {
            get { return CurrentTotalValue - InvestedAmount; }
        }

        /// <summary>Прибыль/убыток в процентах</summary>
        public decimal ProfitPercent
        {
            get
            {
                if (InvestedAmount == 0) return 0;
                return Profit / InvestedAmount * 100m;
            }
        }

        /// <summary>Средний рост в месяц (для прогноза)</summary>
        public decimal AvgMonthlyGrowthPercent
        {
            get
            {
                int monthsPassed = Math.Max(1, (DateTime.Now - PurchaseDate).Days / 30);
                return ProfitPercent / monthsPassed;
            }
        }

        /// <summary>Прибыльно или нет</summary>
        public bool IsPositive
        {
            get { return Profit >= 0; }
        }

        /// <summary>Текст изменения с цветом</summary>
        public string ChangeText
        {
            get
            {
                string sign = Profit >= 0 ? "+" : "";
                return $"{sign}{ProfitPercent:F1}%";
            }
        }

        /// <summary>Название типа актива текстом</summary>
        public string TypeText
        {
            get
            {
                switch (Type)
                {
                    case AssetTypeEnum.Stock: return "Акции";
                    case AssetTypeEnum.Currency: return "Валюта";
                    case AssetTypeEnum.Metal: return "Металлы";
                    default: return "—";
                }
            }
        }
    }
}