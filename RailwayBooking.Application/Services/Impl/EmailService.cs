using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using RailwayBooking.Application.Interfaces;
using System.Net;
using System.Threading.Tasks;

namespace RailwayBooking.Application.Services.Impl
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOtpAsync(string toEmail, string otp)
        {
            var message = new MimeMessage();
            var from = _config["Smtp:Email"] ?? "no-reply@railwaybooking.local";
            message.From.Add(MailboxAddress.Parse(from));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Mã OTP đăng ký tài khoản";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = BuildOtpHtml(otp)
            };

            message.Body = bodyBuilder.ToMessageBody();

            var host = _config["Smtp:Host"];
            var port = int.TryParse(_config["Smtp:Port"], out var p) ? p : 587;
            var user = _config["Smtp:Email"];
            var pass = _config["Smtp:Password"];

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(user, pass);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        private static string BuildOtpHtml(string otp)
        {
            var safeOtp = WebUtility.HtmlEncode(otp);

            return $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
    <title>Mã xác thực của bạn</title>
</head>
<body style='margin:0;padding:0;background-color:#f4f7fb;font-family:Arial,Helvetica,sans-serif;color:#12324f;'>
    <table role='presentation' cellpadding='0' cellspacing='0' border='0' width='100%' style='background-color:#f4f7fb;padding:18px 0;'>
        <tr>
            <td align='center'>
                <table role='presentation' cellpadding='0' cellspacing='0' border='0' width='500' style='width:500px;max-width:500px;background:#ffffff;border:1px solid #d7dde7;border-radius:4px;overflow:hidden;'>
                    <tr>
                        <td style='padding:14px 20px;border-bottom:1px solid #d7dde7;background:#ffffff;'>
                            <table role='presentation' cellpadding='0' cellspacing='0' border='0' width='100%'>
                                <tr>
                                    <td style='font-size:18px;font-weight:700;letter-spacing:.2px;color:#0f4c81;'>RailLink Express</td>
                                    <td align='right' style='font-size:11px;font-weight:700;letter-spacing:2px;color:#4f6479;'>SECURITY</td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding:28px 28px 18px 28px;'>
                            <div style='font-size:24px;line-height:1.2;font-weight:700;color:#0f4c81;margin:0 0 18px 0;'>Mã xác thực của bạn</div>
                            <div style='font-size:16px;line-height:1.7;color:#2d3b4d;margin:0 0 20px 0;'>
                                Chào bạn,<br /><br />
                                Bạn đang thực hiện thao tác đăng ký tài khoản tại <strong>RailLink Express</strong>. Vui lòng sử dụng mã xác thực (OTP) dưới đây để tiếp tục:
                            </div>

                            <table role='presentation' cellpadding='0' cellspacing='0' border='0' width='100%' style='margin:0 0 18px 0;'>
                                <tr>
                                    <td align='center' style='background:#eef1f7;border:1px solid #d5dbe8;border-radius:3px;padding:28px 20px 24px 20px;'>
                                        <div style='font-size:42px;line-height:1;font-weight:700;letter-spacing:16px;color:#0f4c81;text-indent:16px;font-family:Arial,Helvetica,sans-serif;'>{safeOtp}</div>
                                        <div style='margin-top:14px;font-size:11px;line-height:1.2;font-weight:700;letter-spacing:1px;color:#3f4d5f;text-transform:uppercase;'>Mã này có hiệu lực trong 5 phút</div>
                                    </td>
                                </tr>
                            </table>

                            <table role='presentation' cellpadding='0' cellspacing='0' border='0' width='100%' style='margin:0 0 18px 0;'>
                                <tr>
                                    <td style='border-left:4px solid #e53935;background:#fff5f5;padding:14px 16px 14px 16px;'>
                                        <div style='font-size:14px;line-height:1.55;color:#b71c1c;'>
                                            <strong>Cảnh báo bảo mật:</strong><br />
                                            Tuyệt đối <strong>KHÔNG</strong> chia sẻ mã này với bất kỳ ai, kể cả nhân viên RailLink Express. Chúng tôi sẽ không bao giờ yêu cầu bạn cung cấp mã OTP qua điện thoại hoặc mạng xã hội.
                                        </div>
                                    </td>
                                </tr>
                            </table>

                            <div style='font-size:13px;line-height:1.7;color:#6a7787;font-style:italic;margin:0 0 18px 0;'>
                                Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email hoặc liên hệ với bộ phận hỗ trợ của chúng tôi để bảo vệ tài khoản.
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding:18px 20px 14px 20px;border-top:1px solid #d7dde7;background:#f7f8fc;'>
                            <table role='presentation' cellpadding='0' cellspacing='0' border='0' width='100%'>
                                <tr>
                                    <td style='vertical-align:top;'>
                                        <div style='font-size:16px;font-weight:700;color:#0f4c81;margin:0 0 8px 0;'>RailLink Express</div>
                                        <div style='font-size:12px;line-height:1.6;color:#475869;'>123 Đường Sắt,Phường Dĩ An, TP. Hồ Chí Minh</div>
                                        <div style='font-size:12px;line-height:1.6;color:#475869;'>Hotline: 1900 3979 | support@raillink.com</div>
                                    </td>
                                    <td align='right' style='vertical-align:top;padding-top:6px;'>
                                        <table role='presentation' cellpadding='0' cellspacing='0' border='0'>
                                            <tr>
                                                <td style='padding-left:8px;'>
                                                    <div style='width:30px;height:30px;border-radius:50%;background:#0f4c81;color:#ffffff;font-size:14px;line-height:30px;text-align:center;'>🌏</div>
                                                </td>
                                                <td style='padding-left:8px;'>
                                                    <div style='width:30px;height:30px;border-radius:50%;background:#0f4c81;color:#ffffff;font-size:14px;line-height:30px;text-align:center;'>✉</div>
                                                </td>
                                                <td style='padding-left:8px;'>
                                                    <div style='width:30px;height:30px;border-radius:50%;background:#0f4c81;color:#ffffff;font-size:14px;line-height:30px;text-align:center;'>☎</div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align='center' style='padding:10px 20px 18px 20px;background:#f7f8fc;border-top:1px solid #d7dde7;'>
                            <div style='font-size:11px;line-height:1.5;color:#556273;'>© 2026 RailLink Express Operations. Precision in every journey.</div>
                            <div style='font-size:11px;line-height:1.8;color:#0f4c81;font-weight:700;'>
                                <span style='text-decoration:underline;'>Privacy Policy</span>&nbsp;&nbsp;&nbsp;&nbsp;
                                <span style='text-decoration:underline;'>Terms of Service</span>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
        }
    }
}
