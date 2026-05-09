using System;
using System.Collections.Generic;
using FinancialAnalyzer.Data;
using FinancialAnalyzer.Models;

namespace FinancialAnalyzer.Services
{
    public static class IncomeService
    {
        public static List<IncomeModel> GetAll()
        {
            var incomes = new List<IncomeModel>();
            var rows = Repository.ExecuteQuery("SELECT * FROM Incomes ORDER BY Id");
            foreach (var row in rows)
            {
                incomes.Add(new IncomeModel
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Source = (IncomeModel.IncomeSourceEnum)Convert.ToInt32(row["Source"]),
                    CustomName = row["CustomName"]?.ToString(),
                    AmountPerPayment = Convert.ToDecimal(row["AmountPerPayment"]),
                    PaymentsPerMonth = Convert.ToInt32(row["PaymentsPerMonth"]),
                    IsAfterTax = Convert.ToInt32(row["IsAfterTax"]) == 1,
                    StartDate = DateTime.Parse(row["StartDate"].ToString()),
                    TargetDepositId = row["TargetDepositId"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["TargetDepositId"]),
                    TargetDepositName = row["TargetDepositName"]?.ToString()
                });
            }
            return incomes;
        }

        public static void Add(IncomeModel income)
        {
            Repository.ExecuteNonQuery(
                @"INSERT INTO Incomes (Source, CustomName, AmountPerPayment, PaymentsPerMonth, IsAfterTax, StartDate, TargetDepositId, TargetDepositName)
                  VALUES (@s, @cn, @a, @p, @t, @sd, @td, @tn)",
                ("@s", (int)income.Source),
                ("@cn", (object)income.CustomName ?? DBNull.Value),
                ("@a", income.AmountPerPayment),
                ("@p", income.PaymentsPerMonth),
                ("@t", income.IsAfterTax ? 1 : 0),
                ("@sd", income.StartDate.ToString("yyyy-MM-dd")),
                ("@td", (object)income.TargetDepositId ?? DBNull.Value),
                ("@tn", (object)income.TargetDepositName ?? DBNull.Value));
        }

        public static void Update(IncomeModel income)
        {
            Repository.ExecuteNonQuery(
                @"UPDATE Incomes SET Source=@s, CustomName=@cn, AmountPerPayment=@a, PaymentsPerMonth=@p, 
                  IsAfterTax=@t, StartDate=@sd, TargetDepositId=@td, TargetDepositName=@tn WHERE Id=@id",
                ("@id", income.Id),
                ("@s", (int)income.Source),
                ("@cn", (object)income.CustomName ?? DBNull.Value),
                ("@a", income.AmountPerPayment),
                ("@p", income.PaymentsPerMonth),
                ("@t", income.IsAfterTax ? 1 : 0),
                ("@sd", income.StartDate.ToString("yyyy-MM-dd")),
                ("@td", (object)income.TargetDepositId ?? DBNull.Value),
                ("@tn", (object)income.TargetDepositName ?? DBNull.Value));
        }

        public static void Delete(int id)
        {
            Repository.ExecuteNonQuery("DELETE FROM Incomes WHERE Id=@id", ("@id", id));
        }
    }
}