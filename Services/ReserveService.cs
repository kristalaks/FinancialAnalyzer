using System;
using System.Collections.Generic;
using FinancialAnalyzer.Models;

namespace FinancialAnalyzer.Services
{
    public static class ReserveService
    {
        public static List<ReserveModel> GetDemoReserves()
        {
            return new List<ReserveModel>
            {
                new ReserveModel
                {
                    Id = 1,
                    Name = "Подушка безопасности",
                    Amount = 100000m,
                    CreatedAt = new DateTime(2023, 6, 1),
                    Note = "На чёрный день"
                },
                new ReserveModel
                {
                    Id = 2,
                    Name = "Наличные",
                    Amount = 20000m,
                    CreatedAt = new DateTime(2024, 1, 15),
                    Note = ""
                }
            };
        }
    }
}