using System;
using System.Collections.Generic;
using FinancialAnalyzer.Data;
using FinancialAnalyzer.Models;

namespace FinancialAnalyzer.Services
{
    public static class DepositService
    {
        public static List<DepositModel> GetAll()
        {
            var deposits = new List<DepositModel>();
            var rows = Repository.ExecuteQuery("SELECT * FROM Deposits ORDER BY Id");

            foreach (var row in rows)
            {
                deposits.Add(new DepositModel
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    InitialAmount = Convert.ToDecimal(row["InitialAmount"]),
                    InterestRate = Convert.ToDecimal(row["InterestRate"]),
                    RateType = Convert.ToInt32(row["RateType"]),
                    OpenDate = DateTime.Parse(row["OpenDate"].ToString()),
                    CloseDate = row["CloseDate"] == DBNull.Value ? (DateTime?)null : DateTime.Parse(row["CloseDate"].ToString()),
                    CurrentAmount = Convert.ToDecimal(row["CurrentAmount"]),
                    Profit = Convert.ToDecimal(row["Profit"]),
                    ProfitPercent = Convert.ToDecimal(row["ProfitPercent"])
                });
            }

            return deposits;
        }

        public static void Add(DepositModel deposit)
        {
            string closeDate = deposit.CloseDate.HasValue ? $"'{deposit.CloseDate.Value:yyyy-MM-dd}'" : "NULL";
            string sql = $@"INSERT INTO Deposits (Name, InitialAmount, InterestRate, RateType, OpenDate, CloseDate, CurrentAmount, Profit, ProfitPercent)
                           VALUES (@n, @a, @r, @t, @o, {closeDate}, @ca, @p, @pp)";

            Repository.ExecuteNonQuery(sql,
                ("@n", deposit.Name),
                ("@a", deposit.InitialAmount),
                ("@r", deposit.InterestRate),
                ("@t", deposit.RateType),
                ("@o", deposit.OpenDate.ToString("yyyy-MM-dd")),
                ("@ca", deposit.CurrentAmount),
                ("@p", deposit.Profit),
                ("@pp", deposit.ProfitPercent));
        }

        public static void Update(DepositModel deposit)
        {
            string closeDate = deposit.CloseDate.HasValue ? $"'{deposit.CloseDate.Value:yyyy-MM-dd}'" : "NULL";
            string sql = $@"UPDATE Deposits SET Name=@n, InitialAmount=@a, InterestRate=@r, RateType=@t, 
                           OpenDate=@o, CloseDate={closeDate}, CurrentAmount=@ca, Profit=@p, ProfitPercent=@pp
                           WHERE Id=@id";

            Repository.ExecuteNonQuery(sql,
                ("@id", deposit.Id),
                ("@n", deposit.Name),
                ("@a", deposit.InitialAmount),
                ("@r", deposit.InterestRate),
                ("@t", deposit.RateType),
                ("@o", deposit.OpenDate.ToString("yyyy-MM-dd")),
                ("@ca", deposit.CurrentAmount),
                ("@p", deposit.Profit),
                ("@pp", deposit.ProfitPercent));
        }

        public static void Delete(int id)
        {
            Repository.ExecuteNonQuery("DELETE FROM Deposits WHERE Id=@id", ("@id", id));
        }

        public static decimal CalculateCurrentAmount(DepositModel deposit)
        {
            int daysPassed = (DateTime.Now - deposit.OpenDate).Days;
            if (daysPassed <= 0) return deposit.InitialAmount;

            decimal dailyRate = deposit.InterestRate / 100m / 365m;

            if (deposit.RateType == 0)
            {
                return deposit.InitialAmount + deposit.InitialAmount * dailyRate * daysPassed;
            }
            else
            {
                int monthsPassed = daysPassed / 30;
                decimal monthlyRate = deposit.InterestRate / 100m / 12m;
                decimal result = deposit.InitialAmount;
                for (int i = 0; i < monthsPassed; i++)
                    result += result * monthlyRate;
                int remainingDays = daysPassed % 30;
                if (remainingDays > 0)
                    result += result * dailyRate * remainingDays;
                return result;
            }
        }
    }
}