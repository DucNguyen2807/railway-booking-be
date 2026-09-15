using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.Interfaces;
using RailwayBooking.Domain.Entities.Users;
using RailwayBooking.Domain.Enums.Roles;
using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Domain.Interfaces;
using RailwayBooking.Infrastructure.Entities.Users;

namespace RailwayBooking.Application.Features.UserFeat.Auth.SignUp
{
    public class SignUpCommandHandler : IRequestHandler<SignUpCommand, BaseResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilityService _utilityService;
        private readonly IEmailService _emailService;

        public SignUpCommandHandler(IUnitOfWork unitOfWork, IUtilityService utilityService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _utilityService = utilityService;
            _emailService = emailService;
        }

        public async Task<BaseResponse<string>> Handle(SignUpCommand request, CancellationToken cancellationToken)
        {
            var existUser = _unitOfWork.UserRepository.Get().FirstOrDefault(x => x.Email == request.Mail || x.PhoneNumber == request.PhoneNumber);

            if (existUser != null)
            {
                return BaseResponse<string>.FailureResponse("Email hoặc số điện thoại này đã được sử dụng cho một tài khoản khác");
            }

            var otp = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var generatedOtp = _utilityService.GenerateOTP();

                var utcNow = DateTime.UtcNow;
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = request.Fullname,
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Mail,
                    PasswordHash = _utilityService.HashPassword(request.Password),
                    Role = (int)GeneralRole.USER_ROLE,
                    Status = (int)UserStatus.INACTIVE_STATUS,
                    OtpCodeHash = _utilityService.HashPassword(generatedOtp),
                    OtpCodeExpiryUtc = DateTime.SpecifyKind(utcNow.AddMinutes(5), DateTimeKind.Utc),
                    CreatedAt = utcNow,
                    UpdatedAt = utcNow
                };

                _unitOfWork.UserRepository.Insert(user);

                return generatedOtp;
            });

            try
            {
                await _emailService.SendOtpAsync(request.Mail, otp);
            }
            catch
            {
                return BaseResponse<string>.FailureResponse("Đăng ký thành công nhưng không gửi được mã OTP. Vui lòng thử lại sau.");
            }

            return BaseResponse<string>.SuccessResponse("Đăng ký thành công. Vui lòng kiểm tra email để lấy mã OTP xác minh tài khoản.");
        }
    }

}
