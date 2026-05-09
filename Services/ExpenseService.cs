using System;
using System.Collections.Generic;
using FinancialAnalyzer.Data;
using FinancialAnalyzer.Models;

namespace FinancialAnalyzer.Services
{
    public static class ExpenseService
    {
        public static List<ExpenseModel> GetAll()
        {
            var expenses = new List<ExpenseModel>();
            var rows = Repository.ExecuteQuery("SELECT * FROM Expenses ORDER BY Id");
            foreach (var row in rows)
            {
                expenses.Add(new ExpenseModel
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Category = (ExpenseModel.ExpenseCategoryEnum)Convert.ToInt32(row["Category"]),
                    CustomCategoryName = row["CustomCategoryName"]?.ToString(),
                    Name = row["Name"].ToString(),
                    Amount = Convert.ToDecimal(row["Amount"]),
                    Period = (ExpenseModel.ExpensePeriodEnum)Convert.ToInt32(row["Period"]),
                    Date = DateTime.Parse(row["Date"].ToString()),
                    Note = row["Note"]?.ToString(),
                    SourceReserveId = row["SourceReserveId"] == DBNull.Value ? (int?)null : Convert.ToInt32(row["SourceReserveId"]),
                    SourceReserveName = row["SourceReserveName"]?.ToString()
                });
            }
            return expenses;
        }

        public static void Add(ExpenseModel expense)
        {
            Repository.ExecuteNonQuery(
                @"INSERT INTO Expenses (Category, CustomCategoryName, Name, Amount, Period, Date, Note, SourceReserveId, SourceReserveName)
        VALUES (@c, @cc, @n, @a, @p, @d, @no, @sr, @sn)",
                ("@c", (int)expense.Category),
                ("@cc", (object)expense.CustomCategoryName ?? DBNull.Value),
                ("@n", expense.Name),
                ("@a", expense.Amount),
                ("@p", (int)expense.Period),
                ("@d", expense.Date.ToString("yyyy-MM-dd")),
                ("@no", (object)expense.Note ?? DBNull.Value),
                ("@sr", (object)expense.SourceReserveId ?? DBNull.Value),
                ("@sn", (object)expense.SourceReserveName ?? DBNull.Value));
        }

        public static void Update(ExpenseModel expense)
        {
            Repository.ExecuteNonQuery(
                @"UPDATE Expenses SET Category=@c, CustomCategoryName=@cc, Name=@n, Amount=@a, 
          Period=@p, Date=@d, Note=@no, SourceReserveId=@sr, SourceReserveName=@sn WHERE Id=@id",
                ("@id", expense.Id),
                ("@c", (int)expense.Category),
                ("@cc", (object)expense.CustomCategoryName ?? DBNull.Value),
                ("@n", expense.Name),
                ("@a", expense.Amount),
                ("@p", (int)expense.Period),
                ("@d", expense.Date.ToString("yyyy-MM-dd")),
                ("@no", (object)expense.Note ?? DBNull.Value),
                ("@sr", (object)expense.SourceReserveId ?? DBNull.Value),
                ("@sn", (object)expense.SourceReserveName ?? DBNull.Value));
        }

        public static void Delete(int id)
        {
            Repository.ExecuteNonQuery("DELETE FROM Expenses WHERE Id=@id", ("@id", id));
        }
    }
}