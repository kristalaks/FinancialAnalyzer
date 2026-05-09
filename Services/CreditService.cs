using System;
using System.Collections.Generic;
using FinancialAnalyzer.Data;
using FinancialAnalyzer.Models;

namespace FinancialAnalyzer.Services
{
    public static class CreditService
    {
        public static List<CreditModel> GetAll()
        {
            var credits = new List<CreditModel>();
            var rows = Repository.ExecuteQuery("SELECT * FROM Credits ORDER BY Id");
            foreach (var row in rows)
            {
                credits.Add(new CreditModel
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Type = (CreditModel.CreditTypeEnum)Convert.ToInt32(row["Type"]),
                    TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                    DownPayment = Convert.ToDecimal(row["DownPayment"]),
                    InterestRate = Convert.ToDecimal(row["InterestRate"]),
                    TermMonths = Convert.ToInt32(row["TermMonths"]),
                    PaymentType = (CreditModel.PaymentTypeEnum)Convert.ToInt32(row["PaymentType"]),
                    OpenDate = DateTime.Parse(row["OpenDate"].ToString()),
                    MonthlyPayment = Convert.ToDecimal(row["MonthlyPayment"]),
                    RemainingDebt = Convert.ToDecimal(row["RemainingDebt"]),
                    PaidPrincipal = Convert.ToDecimal(row["PaidPrincipal"]),
                    PaidInterest = Convert.ToDecimal(row["PaidInterest"])
                });
            }
            return credits;
        }

        public static decimal CalculateAnnuityPayment(decimal loanAmount, decimal annualRate, int months)
        {
            if (months <= 0 || loanAmount <= 0) return 0;
            decimal monthlyRate = annualRate / 100m / 12m;
            if (monthlyRate == 0) return loanAmount / months;
            decimal factor = (decimal)Math.Pow((double)(1 + monthlyRate), months);
            return loanAmount * monthlyRate * factor / (factor - 1);
        }

        public static void Add(CreditModel credit)
        {
            Repository.ExecuteNonQuery(
                @"INSERT INTO Credits (Name, Type, TotalAmount, DownPayment, InterestRate, TermMonths, PaymentType, OpenDate, MonthlyPayment, RemainingDebt, PaidPrincipal, PaidInterest)
                  VALUES (@n, @t, @ta, @dp, @ir, @tm, @pt, @od, @mp, @rd, @pp, @pi)",
                ("@n", credit.Name),
                ("@t", (int)credit.Type),
                ("@ta", credit.TotalAmount),
                ("@dp", credit.DownPayment),
                ("@ir", credit.InterestRate),
                ("@tm", credit.TermMonths),
                ("@pt", (int)credit.PaymentType),
                ("@od", credit.OpenDate.ToString("yyyy-MM-dd")),
                ("@mp", credit.MonthlyPayment),
                ("@rd", credit.RemainingDebt),
                ("@pp", credit.PaidPrincipal),
                ("@pi", credit.PaidInterest));
        }

        public static void Update(CreditModel credit)
        {
            Repository.ExecuteNonQuery(
                @"UPDATE Credits SET Name=@n, Type=@t, TotalAmount=@ta, DownPayment=@dp, InterestRate=@ir, 
                  TermMonths=@tm, PaymentType=@pt, OpenDate=@od, MonthlyPayment=@mp, RemainingDebt=@rd, 
                  PaidPrincipal=@pp, PaidInterest=@pi WHERE Id=@id",
                ("@id", credit.Id),
                ("@n", credit.Name),
                ("@t", (int)credit.Type),
                ("@ta", credit.TotalAmount),
                ("@dp", credit.DownPayment),
                ("@ir", credit.InterestRate),
                ("@tm", credit.TermMonths),
                ("@pt", (int)credit.PaymentType),
                ("@od", credit.OpenDate.ToString("yyyy-MM-dd")),
                ("@mp", credit.MonthlyPayment),
                ("@rd", credit.RemainingDebt),
                ("@pp", credit.PaidPrincipal),
                ("@pi", credit.PaidInterest));
        }

        public static void Delete(int id)
        {
            Repository.ExecuteNonQuery("DELETE FROM Credits WHERE Id=@id", ("@id", id));
        }
    }
}