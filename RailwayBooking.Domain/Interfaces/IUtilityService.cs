using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Domain.Interfaces
{
    public interface IUtilityService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        //string GenerateCode(EntityType type);
        DateTime? ToVietnamTime(DateTime utcDateTime);
        string CapitalizeFirstLetter(string input);
        string FormatVietnamPhoneNumber(string input);
        int ExtractConversionRate(string description);
        decimal ConvertToSquareMeters(decimal value, string unitName);
        string GenerateOTP();
    }
}
