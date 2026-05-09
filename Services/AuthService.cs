using System;
using System.Linq;
using FinancialAnalyzer.Data;
using FinancialAnalyzer.Models;

namespace FinancialAnalyzer.Services
{
    public static class AuthService
    {
        private static UserModel _currentUser;

        public static UserModel CurrentUser
        {
            get { return _currentUser; }
        }

        public static UserModel Login(string username, string password)
        {
            string passwordHash = DatabaseHelper.HashPassword(password);

            var rows = Repository.ExecuteQuery(
                "SELECT * FROM Users WHERE Username=@u AND PasswordHash=@p",
                ("@u", username),
                ("@p", passwordHash));

            if (rows.Count > 0)
            {
                var row = rows[0];
                _currentUser = new UserModel
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Username = row["Username"].ToString(),
                    DisplayName = row["DisplayName"]?.ToString() ?? username,
                    InflationRate = Convert.ToDecimal(row["InflationRate"]),
                    FormatType = row["FormatType"]?.ToString() ?? "100 000 ₽",
                    ChangeDisplayType = row["ChangeDisplayType"]?.ToString() ?? "percent"
                };
                return _currentUser;
            }

            _currentUser = null;
            return null;
        }

        public static void Logout()
        {
            _currentUser = null;
        }

        public static bool Register(string username, string password, string displayName)
        {
            var existing = Repository.ExecuteQuery(
                "SELECT Id FROM Users WHERE Username=@u",
                ("@u", username));

            if (existing.Count > 0)
                return false;

            string passwordHash = DatabaseHelper.HashPassword(password);

            Repository.ExecuteNonQuery(
                "INSERT INTO Users (Username, PasswordHash, DisplayName) VALUES (@u, @p, @d)",
                ("@u", username),
                ("@p", passwordHash),
                ("@d", displayName));

            return true;
        }

        public static bool ChangePassword(string oldPassword, string newPassword)
        {
            if (_currentUser == null) return false;

            string oldHash = DatabaseHelper.HashPassword(oldPassword);
            var check = Repository.ExecuteQuery(
                "SELECT Id FROM Users WHERE Id=@id AND PasswordHash=@p",
                ("@id", _currentUser.Id),
                ("@p", oldHash));

            if (check.Count == 0) return false;

            string newHash = DatabaseHelper.HashPassword(newPassword);
            Repository.ExecuteNonQuery(
                "UPDATE Users SET PasswordHash=@p WHERE Id=@id",
                ("@id", _currentUser.Id),
                ("@p", newHash));

            return true;
        }
    }
}