using System.ComponentModel.DataAnnotations;

namespace FinancialAnalyzer.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string DisplayName { get; set; }
        public decimal InflationRate { get; set; }
        public string FormatType { get; set; }
        public string ChangeDisplayType { get; set; }
    }
}