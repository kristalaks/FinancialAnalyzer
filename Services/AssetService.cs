using System;
using System.Collections.Generic;
using FinancialAnalyzer.Data;
using FinancialAnalyzer.Models;

namespace FinancialAnalyzer.Services
{
    public static class AssetService
    {
        public static List<AssetModel> GetByType(AssetModel.AssetTypeEnum type)
        {
            var assets = new List<AssetModel>();
            var rows = Repository.ExecuteQuery(
                "SELECT * FROM Assets WHERE Type=@type ORDER BY Id",
                ("@type", (int)type));

            foreach (var row in rows)
            {
                assets.Add(MapAsset(row));
            }
            return assets;
        }

        public static List<AssetModel> GetAll()
        {
            var assets = new List<AssetModel>();
            var rows = Repository.ExecuteQuery("SELECT * FROM Assets ORDER BY Type, Id");
            foreach (var row in rows)
                assets.Add(MapAsset(row));
            return assets;
        }

        public static void Add(AssetModel asset)
        {
            Repository.ExecuteNonQuery(
                @"INSERT INTO Assets (Type, Ticker, CompanyName, Exchange, Quantity, PurchasePrice, PurchaseDate, CurrentPrice)
                  VALUES (@t, @tk, @cn, @ex, @q, @pp, @pd, @cp)",
                ("@t", (int)asset.Type),
                ("@tk", asset.Ticker),
                ("@cn", asset.CompanyName),
                ("@ex", asset.Exchange),
                ("@q", asset.Quantity),
                ("@pp", asset.PurchasePrice),
                ("@pd", asset.PurchaseDate.ToString("yyyy-MM-dd")),
                ("@cp", asset.CurrentPrice));
        }

        public static void Update(AssetModel asset)
        {
            Repository.ExecuteNonQuery(
                @"UPDATE Assets SET Type=@t, Ticker=@tk, CompanyName=@cn, Exchange=@ex, 
                  Quantity=@q, PurchasePrice=@pp, PurchaseDate=@pd, CurrentPrice=@cp
                  WHERE Id=@id",
                ("@id", asset.Id),
                ("@t", (int)asset.Type),
                ("@tk", asset.Ticker),
                ("@cn", asset.CompanyName),
                ("@ex", asset.Exchange),
                ("@q", asset.Quantity),
                ("@pp", asset.PurchasePrice),
                ("@pd", asset.PurchaseDate.ToString("yyyy-MM-dd")),
                ("@cp", asset.CurrentPrice));
        }

        public static void Delete(int id)
        {
            Repository.ExecuteNonQuery("DELETE FROM Assets WHERE Id=@id", ("@id", id));
        }

        private static AssetModel MapAsset(Dictionary<string, object> row)
        {
            return new AssetModel
            {
                Id = Convert.ToInt32(row["Id"]),
                Type = (AssetModel.AssetTypeEnum)Convert.ToInt32(row["Type"]),
                Ticker = row["Ticker"].ToString(),
                CompanyName = row["CompanyName"]?.ToString(),
                Exchange = row["Exchange"]?.ToString(),
                Quantity = Convert.ToDecimal(row["Quantity"]),
                PurchasePrice = Convert.ToDecimal(row["PurchasePrice"]),
                PurchaseDate = DateTime.Parse(row["PurchaseDate"].ToString()),
                CurrentPrice = Convert.ToDecimal(row["CurrentPrice"])
            };
        }
    }
}