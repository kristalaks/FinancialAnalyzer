using System;
using System.Collections.Generic;
using FinancialAnalyzer.Models;

namespace FinancialAnalyzer.Services
{
    /// <summary>
    /// Сервис для работы с активами (акции, валюты, металлы)
    /// </summary>
    public static class AssetService
    {
        /// <summary>
        /// Возвращает демо-данные по акциям
        /// </summary>
        public static List<AssetModel> GetDemoStocks()
        {
            return new List<AssetModel>
            {
                new AssetModel
                {
                    Id = 1,
                    Type = AssetModel.AssetTypeEnum.Stock,
                    Ticker = "SBER",
                    CompanyName = "Сбербанк",
                    Exchange = "MOEX",
                    Quantity = 100,
                    PurchasePrice = 254.00m,
                    PurchaseDate = new DateTime(2024, 1, 15),
                    CurrentPrice = 285.50m
                },
                new AssetModel
                {
                    Id = 2,
                    Type = AssetModel.AssetTypeEnum.Stock,
                    Ticker = "GAZP",
                    CompanyName = "Газпром",
                    Exchange = "MOEX",
                    Quantity = 500,
                    PurchasePrice = 149.50m,
                    PurchaseDate = new DateTime(2024, 3, 20),
                    CurrentPrice = 142.00m
                },
                new AssetModel
                {
                    Id = 3,
                    Type = AssetModel.AssetTypeEnum.Stock,
                    Ticker = "AAPL",
                    CompanyName = "Apple Inc.",
                    Exchange = "NASDAQ",
                    Quantity = 10,
                    PurchasePrice = 150.00m,
                    PurchaseDate = new DateTime(2024, 6, 10),
                    CurrentPrice = 175.00m
                },
                new AssetModel
                {
                    Id = 4,
                    Type = AssetModel.AssetTypeEnum.Stock,
                    Ticker = "YNDX",
                    CompanyName = "Яндекс",
                    Exchange = "MOEX",
                    Quantity = 30,
                    PurchasePrice = 2390.00m,
                    PurchaseDate = new DateTime(2024, 9, 1),
                    CurrentPrice = 2450.00m
                }
            };
        }
        
        public static List<AssetModel> GetDemoCurrencies()
        {
            return new List<AssetModel>
            {
                new AssetModel
                {
                    Id = 1,
                    Type = AssetModel.AssetTypeEnum.Currency,
                    Ticker = "USD/RUB",
                    CompanyName = "Доллар США",
                    Exchange = "Forex",
                    Quantity = 5000,
                    PurchasePrice = 88.50m,
                    PurchaseDate = new DateTime(2024, 2, 10),
                    CurrentPrice = 95.20m
                },
                new AssetModel
                {
                    Id = 2,
                    Type = AssetModel.AssetTypeEnum.Currency,
                    Ticker = "EUR/RUB",
                    CompanyName = "Евро",
                    Exchange = "Forex",
                    Quantity = 3000,
                    PurchasePrice = 96.00m,
                    PurchaseDate = new DateTime(2024, 4, 5),
                    CurrentPrice = 102.80m
                },
                new AssetModel
                {
                    Id = 3,
                    Type = AssetModel.AssetTypeEnum.Currency,
                    Ticker = "CNY/RUB",
                    CompanyName = "Китайский юань",
                    Exchange = "Forex",
                    Quantity = 50000,
                    PurchasePrice = 12.20m,
                    PurchaseDate = new DateTime(2024, 7, 15),
                    CurrentPrice = 11.90m
                }
            };
         }

                /// <summary>
                /// Возвращает демо-данные по металлам
                /// </summary>
        public static List<AssetModel> GetDemoMetals()
        {
            return new List<AssetModel>
            {
                new AssetModel
                {
                    Id = 1,
                    Type = AssetModel.AssetTypeEnum.Metal,
                    Ticker = "XAU",
                    CompanyName = "Золото (за грамм)",
                    Exchange = "Metals",
                    Quantity = 50,
                    PurchasePrice = 5800.00m,
                    PurchaseDate = new DateTime(2024, 1, 20),
                    CurrentPrice = 6250.00m
                },
                new AssetModel
                {
                    Id = 2,
                    Type = AssetModel.AssetTypeEnum.Metal,
                    Ticker = "XAG",
                    CompanyName = "Серебро (за грамм)",
                    Exchange = "Metals",
                    Quantity = 1000,
                    PurchasePrice = 72.00m,
                    PurchaseDate = new DateTime(2024, 5, 10),
                    CurrentPrice = 68.50m
                }
            };
        }
    }
}