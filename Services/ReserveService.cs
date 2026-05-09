using System;
using System.Collections.Generic;
using FinancialAnalyzer.Data;
using FinancialAnalyzer.Models;

namespace FinancialAnalyzer.Services
{
    public static class ReserveService
    {
        public static List<ReserveModel> GetAll()
        {
            var reserves = new List<ReserveModel>();
            var rows = Repository.ExecuteQuery("SELECT * FROM Reserves ORDER BY Id");
            foreach (var row in rows)
            {
                reserves.Add(new ReserveModel
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Amount = Convert.ToDecimal(row["Amount"]),
                    CreatedAt = DateTime.Parse(row["CreatedAt"].ToString()),
                    Note = row["Note"]?.ToString()
                });
            }
            return reserves;
        }

        public static void Add(ReserveModel reserve)
        {
            Repository.ExecuteNonQuery(
                "INSERT INTO Reserves (Name, Amount, CreatedAt, Note) VALUES (@n, @a, @c, @no)",
                ("@n", reserve.Name),
                ("@a", reserve.Amount),
                ("@c", reserve.CreatedAt.ToString("yyyy-MM-dd")),
                ("@no", (object)reserve.Note ?? DBNull.Value));
        }

        public static void Update(ReserveModel reserve)
        {
            Repository.ExecuteNonQuery(
                "UPDATE Reserves SET Name=@n, Amount=@a, CreatedAt=@c, Note=@no WHERE Id=@id",
                ("@id", reserve.Id),
                ("@n", reserve.Name),
                ("@a", reserve.Amount),
                ("@c", reserve.CreatedAt.ToString("yyyy-MM-dd")),
                ("@no", (object)reserve.Note ?? DBNull.Value));
        }

        public static void Delete(int id)
        {
            Repository.ExecuteNonQuery("DELETE FROM Reserves WHERE Id=@id", ("@id", id));
        }

        public static void UpdateAmount(int id, decimal newAmount)
        {
            Repository.ExecuteNonQuery(
                "UPDATE Reserves SET Amount=@a WHERE Id=@id",
                ("@id", id),
                ("@a", newAmount));
        }

        public static ReserveModel GetById(int id)
        {
            var rows = Repository.ExecuteQuery(
                "SELECT * FROM Reserves WHERE Id=@id",
                ("@id", id));

            if (rows.Count > 0)
            {
                var row = rows[0];
                return new ReserveModel
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Amount = Convert.ToDecimal(row["Amount"]),
                    CreatedAt = DateTime.Parse(row["CreatedAt"].ToString()),
                    Note = row["Note"]?.ToString()
                };
            }
            return null;
        }
    }
}