using RailwayBooking.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Application.Services.Impl
{
    public class UtilityService : IUtilityService
    {
        private static readonly Dictionary<string, decimal> UnitToSquareMeter = new()
        {
            { "m²", 1 },
            { "ha", 10000 },
        };

        public decimal ConvertToSquareMeters(decimal value, string unitName)
        {
            if (!UnitToSquareMeter.TryGetValue(unitName.ToLower(), out var factor))
                throw new ArgumentException($"Unknown area unit: {unitName}");

            return value * factor;
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public DateTime? ToVietnamTime(DateTime utcDateTime)

        {
            if (utcDateTime.Kind == DateTimeKind.Unspecified)
            {
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }
            else if (utcDateTime.Kind == DateTimeKind.Local)
            {
                utcDateTime = utcDateTime.ToUniversalTime();
            }

            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vietnamTimeZone);
        }

        public string GenerateOTP()
        {
            int otp = RandomNumberGenerator.GetInt32(0, 1000000);
            return otp.ToString("D6");
        }

        public string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return char.ToUpper(input[0]) + input.Substring(1).ToLower();
        }

        public string FormatVietnamPhoneNumber(string input)
        {
            if (input.StartsWith("0"))
                input = input.Substring(1);

            return $"+84 {input.Substring(0, 3)} {input.Substring(3, 3)} {input.Substring(6)}";
        }

        public int ExtractConversionRate(string description)
        {
            var parts = description.Split(' ');
            if (parts.Length >= 1 && int.TryParse(parts[0], out int rate))
            {
                return rate;
            }
            return 1;
        }
    }
}
