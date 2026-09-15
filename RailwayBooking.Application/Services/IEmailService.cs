using System.Threading.Tasks;

namespace RailwayBooking.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpAsync(string toEmail, string otp);
    }
}
