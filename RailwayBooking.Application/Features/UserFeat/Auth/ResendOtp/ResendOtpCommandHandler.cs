using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.Interfaces;
using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.UserFeat.Auth.ResendOtp
{
    public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, BaseResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilityService _utilityService;
        private readonly IEmailService _emailService;

        public ResendOtpCommandHandler(IUnitOfWork unitOfWork, IUtilityService utilityService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _utilityService = utilityService;
            _emailService = emailService;
        }

        public async Task<BaseResponse<string>> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        {
            var user = _unitOfWork.UserRepository.Get(filter: x => x.Email == request.Mail).FirstOrDefault();

            if (user == null)
            {
                return BaseResponse<string>.FailureResponse("Tài khoản không tồn tại");
            }

            if (user.Status == (int)UserStatus.ACTIVE_STATUS)
            {
                return BaseResponse<string>.SuccessResponse("Tài khoản đã được xác minh");
            }

            var otp = _utilityService.GenerateOTP();
            user.OtpCodeHash = _utilityService.HashPassword(otp);
            user.OtpCodeExpiryUtc = DateTime.SpecifyKind(DateTime.UtcNow.AddMinutes(5), DateTimeKind.Utc);
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            try
            {
                await _emailService.SendOtpAsync(request.Mail, otp);
            }
            catch
            {
                return BaseResponse<string>.FailureResponse("Không gửi được mã OTP, vui lòng thử lại sau.");
            }

            return BaseResponse<string>.SuccessResponse("Mã OTP đã được gửi lại");
        }
    }
}
