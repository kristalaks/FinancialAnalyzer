using System;
using System.ComponentModel.DataAnnotations;

namespace FinancialAnalyzer.Models
{
    /// <summary>
    /// Модель данных для кредита
    /// </summary>
    public class CreditModel
    {
        
        public enum CreditTypeEnum
        {
            Mortgage = 0,
            CarLoan = 1,
            Consumer = 2,
            CreditCard = 3,
            Other = 4
        }

        public enum PaymentTypeEnum
        {
            Annuity = 0,        // Аннуитетный (равные платежи)
            Differentiated = 1  // Дифференцированный (уменьшающиеся)
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public CreditTypeEnum Type { get; set; }
        public decimal TotalAmount { get; set; }        // Полная сумма кредита
        public decimal DownPayment { get; set; }         // Первоначальный взнос
        public decimal InterestRate { get; set; }        // Годовая ставка %
        public int TermMonths { get; set; }              // Срок в месяцах
        public PaymentTypeEnum PaymentType { get; set; }
        public DateTime OpenDate { get; set; }
        public decimal MonthlyPayment { get; set; }      // Ежемесячный платёж
        public decimal RemainingDebt { get; set; }       // Остаток основного долга
        public decimal PaidPrincipal { get; set; }       // Выплачено основного долга
        public decimal PaidInterest { get; set; }        // Выплачено процентов

        /// <summary>Сколько месяцев уже выплачено</summary>
        public int MonthsPaid
        {
            get
            {
                return Math.Max(0, (DateTime.Now.Year - OpenDate.Year) * 12 + DateTime.Now.Month - OpenDate.Month);
            }
        }

        /// <summary>Сколько месяцев осталось</summary>
        public int MonthsLeft
        {
            get { return Math.Max(0, TermMonths - MonthsPaid); }
        }

        /// <summary>Дата закрытия</summary>
        public DateTime CloseDate
        {
            get { return OpenDate.AddMonths(TermMonths); }
        }

        /// <summary>Общая сумма выплат за весь срок</summary>
        public decimal TotalPayment
        {
            get { return MonthlyPayment * TermMonths; }
        }

        /// <summary>Переплата</summary>
        public decimal Overpayment
        {
            get { return TotalPayment - (TotalAmount - DownPayment); }
        }

        /// <summary>Осталось выплатить всего</summary>
        public decimal RemainingTotal
        {
            get { return MonthlyPayment * MonthsLeft; }
        }

        public string TypeText
        {
            get
            {
                switch (Type)
                {
                    case CreditTypeEnum.Mortgage: return "Ипотека";
                    case CreditTypeEnum.CarLoan: return "Автокредит";
                    case CreditTypeEnum.Consumer: return "Потребительский";
                    case CreditTypeEnum.CreditCard: return "Кредитная карта";
                    case CreditTypeEnum.Other: return "Другое";
                    default: return "—";
                }
            }
        }

        public string PaymentTypeText
        {
            get
            {
                switch (PaymentType)
                {
                    case PaymentTypeEnum.Annuity: return "Аннуитетный";
                    case PaymentTypeEnum.Differentiated: return "Дифф.";
                    default: return "—";
                }
            }
        }
    }
}