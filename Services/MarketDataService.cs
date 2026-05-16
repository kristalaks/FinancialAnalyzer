using System;
using System.Net;
using System.Xml;
using System.Collections.Generic;

namespace FinancialAnalyzer.Services
{
    public static class MarketDataService
    {
        // Базовые URL-ы веб-сервисов ЦБ РФ [citation:1]
        private const string CbrDailyInfoServiceUrl = "https://www.cbr.ru/DailyInfoWebServ/DailyInfo.asmx";
        private const string CbrCurrencyBaseUrl = "https://www.cbr.ru/scripts/XML_daily.asp";
        private const string CbrSecuritiesServiceUrl = "https://www.cbr.ru/secinfo/secinfo.asmx"; // для акций

        // --- ВАЛЮТЫ (ЦБ РФ) ---
        public static decimal? GetCurrencyRate(string currencyCode)
        {
            try
            {
                using (var client = new WebClient())
                {
                    string xml = client.DownloadString(CbrCurrencyBaseUrl);
                    var doc = new XmlDocument();
                    doc.LoadXml(xml);

                    foreach (XmlNode node in doc.SelectNodes("//Valute"))
                    {
                        string charCode = node.SelectSingleNode("CharCode").InnerText;
                        if (charCode.ToUpper() == currencyCode.ToUpper())
                        {
                            string value = node.SelectSingleNode("Value").InnerText;
                            int nominal = int.Parse(node.SelectSingleNode("Nominal").InnerText);
                            decimal rate = decimal.Parse(value, System.Globalization.CultureInfo.GetCultureInfo("ru-RU"));
                            return rate / nominal;
                        }
                    }
                }
            }
            catch { }
            return null;
        }

        // --- АКЦИИ (ЦБ РФ) ---
        public static decimal? GetStockPrice(string ticker)
        {
            try
            {
                using (var client = new WebClient())
                {
                    string url = $"https://iss.moex.com/iss/engines/stock/markets/shares/boards/TQBR/securities/{ticker}.json?iss.meta=off&iss.only=securities&securities.columns=SECID,PREVPRICE";
                    string json = client.DownloadString(url);

                    // Ищем "PREVPRICE"
                    int prevPriceIndex = json.IndexOf("\"PREVPRICE\"");
                    if (prevPriceIndex > 0)
                    {
                        // Ищем открывающую скобку массива после "PREVPRICE"
                        int dataStart = json.IndexOf('[', prevPriceIndex);
                        if (dataStart > 0)
                        {
                            // Ищем вложенный массив с данными
                            int innerStart = json.IndexOf('[', dataStart + 1);
                            int innerEnd = json.IndexOf(']', innerStart);
                            if (innerStart > 0 && innerEnd > innerStart)
                            {
                                string innerArray = json.Substring(innerStart, innerEnd - innerStart + 1);
                                // Убираем скобки и кавычки
                                innerArray = innerArray.Trim('[', ']').Replace("\"", "");
                                var parts = innerArray.Split(',');
                                if (parts.Length >= 2)
                                {
                                    string priceStr = parts[1].Trim();
                                    if (decimal.TryParse(priceStr, 
                                        System.Globalization.NumberStyles.Any,
                                        System.Globalization.CultureInfo.InvariantCulture,
                                        out decimal price))
                                    {
                                        return price;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MOEX error: {ex.Message}");
            }
            return null;
        }

        // --- МЕТАЛЛЫ (ЦБ РФ) ---
        public static decimal? GetMetalPrice(string ticker)
        {
            try
            {
                // ЦБ РФ принимает дату в формате DD.MM.YYYY
                string date = DateTime.Now.ToString("dd.MM.yyyy");
                string url = $"https://www.cbr.ru/scripts/xml_metall.asp?date_req1={date}&date_req2={date}";

                using (var client = new WebClient())
                {
                    string xml = client.DownloadString(url);

                    // Простой парсинг строкой (без XmlDocument)
                    string tickerUpper = ticker.ToUpper();
                    string targetCode = "";

                    if (tickerUpper == "XAU") targetCode = "Code=\"1\"";
                    else if (tickerUpper == "XAG") targetCode = "Code=\"2\"";
                    else if (tickerUpper == "XPT") targetCode = "Code=\"3\"";
                    else if (tickerUpper == "XPD") targetCode = "Code=\"4\"";
                    else return null;

                    int codeIndex = xml.IndexOf(targetCode);
                    if (codeIndex > 0)
                    {
                        int buyStart = xml.IndexOf("<Buy>", codeIndex);
                        int buyEnd = xml.IndexOf("</Buy>", buyStart);
                        if (buyStart > 0 && buyEnd > buyStart)
                        {
                            string priceStr = xml.Substring(buyStart + 5, buyEnd - buyStart - 5);
                            if (decimal.TryParse(priceStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.GetCultureInfo("ru-RU"), out decimal price))
                            {
                                return price;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Metal API error: {ex.Message}");
            }
            return null;
        }

        // --- ОБНОВЛЕНИЕ ВСЕХ ЦЕН ---
        public static void UpdateAllPrices()
        {
            var assets = AssetService.GetAll();
            foreach (var asset in assets)
            {
                decimal? newPrice = null;

                if (asset.Type == Models.AssetModel.AssetTypeEnum.Currency)
                {
                    string code = asset.Ticker.Replace("/RUB", "").Trim().ToUpper();
                    newPrice = GetCurrencyRate(code);
                }
                else if (asset.Type == Models.AssetModel.AssetTypeEnum.Stock)
                {
                    newPrice = GetStockPrice(asset.Ticker);
                }
                else if (asset.Type == Models.AssetModel.AssetTypeEnum.Metal)
                {
                    newPrice = GetMetalPrice(asset.Ticker);
                }

                if (newPrice.HasValue && newPrice.Value > 0)
                {
                    asset.CurrentPrice = newPrice.Value;
                    AssetService.Update(asset);
                }
            }
        }

        
        public static decimal? GetKeyRate()
        {
            
            return 5.86m;
        }
    }
}