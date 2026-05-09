using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FinancialAnalyzer.Data
{
    public static class DatabaseHelper
    {
        private static string _connectionString;

        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FinancialDB.sqlite");
                    _connectionString = $"Data Source={dbPath};Version=3;";
                }
                return _connectionString;
            }
        }

        public static void Initialize()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FinancialDB.sqlite");

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
                CreateTables();
                SeedData();
            }
        }

        private static void CreateTables()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string sql = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        PasswordHash TEXT NOT NULL,
                        DisplayName TEXT,
                        InflationRate REAL DEFAULT 7.8,
                        FormatType TEXT DEFAULT '100 000 ₽',
                        ChangeDisplayType TEXT DEFAULT 'percent'
                    );

                    CREATE TABLE IF NOT EXISTS Deposits (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        InitialAmount REAL NOT NULL,
                        InterestRate REAL NOT NULL,
                        RateType INTEGER NOT NULL DEFAULT 0,
                        OpenDate TEXT NOT NULL,
                        CloseDate TEXT,
                        CurrentAmount REAL,
                        Profit REAL DEFAULT 0,
                        ProfitPercent REAL DEFAULT 0
                    );

                    CREATE TABLE IF NOT EXISTS Assets (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Type INTEGER NOT NULL,
                        Ticker TEXT NOT NULL,
                        CompanyName TEXT,
                        Exchange TEXT,
                        Quantity REAL NOT NULL,
                        PurchasePrice REAL NOT NULL,
                        PurchaseDate TEXT NOT NULL,
                        CurrentPrice REAL
                    );

                    CREATE TABLE IF NOT EXISTS Reserves (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Amount REAL NOT NULL,
                        CreatedAt TEXT NOT NULL,
                        Note TEXT
                    );

                    CREATE TABLE IF NOT EXISTS Incomes (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Source INTEGER NOT NULL,
                        CustomName TEXT,
                        AmountPerPayment REAL NOT NULL,
                        PaymentsPerMonth INTEGER DEFAULT 1,
                        IsAfterTax INTEGER DEFAULT 1,
                        StartDate TEXT NOT NULL,
                        TargetDepositId INTEGER,
                        TargetDepositName TEXT
                    );

                    CREATE TABLE IF NOT EXISTS Expenses (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Category INTEGER NOT NULL,
                        CustomCategoryName TEXT,
                        Name TEXT NOT NULL,
                        Amount REAL NOT NULL,
                        Period INTEGER NOT NULL,
                        Date TEXT NOT NULL,
                        Note TEXT,
                        SourceReserveId INTEGER,
                        SourceReserveName TEXT
                    );

                    CREATE TABLE IF NOT EXISTS Credits (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Type INTEGER NOT NULL,
                        TotalAmount REAL NOT NULL,
                        DownPayment REAL DEFAULT 0,
                        InterestRate REAL NOT NULL,
                        TermMonths INTEGER NOT NULL,
                        PaymentType INTEGER NOT NULL,
                        OpenDate TEXT NOT NULL,
                        MonthlyPayment REAL NOT NULL,
                        RemainingDebt REAL,
                        PaidPrincipal REAL DEFAULT 0,
                        PaidInterest REAL DEFAULT 0
                    );
                ";

                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void SeedData()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                var cmd = new SQLiteCommand(
                    "INSERT INTO Users (Username, PasswordHash, DisplayName) VALUES (@u, @p, @d)",
                    connection);
                cmd.Parameters.AddWithValue("@u", "admin");
                cmd.Parameters.AddWithValue("@p", HashPassword("admin"));
                cmd.Parameters.AddWithValue("@d", "Зыкин Егор");
                cmd.ExecuteNonQuery();
            }
        }

        private static void ExecuteInsert(SQLiteConnection connection, string sql, params (string, object)[] parameters)
        {
            using (var cmd = new SQLiteCommand(sql, connection))
            {
                foreach (var (name, value) in parameters)
                {
                    cmd.Parameters.AddWithValue(name, value ?? DBNull.Value);
                }
                cmd.ExecuteNonQuery();
            }
        }

        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        public static SQLiteConnection GetConnection()
        {
            var connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }
}