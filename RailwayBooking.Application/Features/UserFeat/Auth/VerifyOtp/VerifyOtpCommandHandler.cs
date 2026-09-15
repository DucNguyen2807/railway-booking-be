using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.Interfaces;
using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.UserFeat.Auth.VerifyOtp
{
    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, BaseResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilityService _utilityService;

        public VerifyOtpCommandHandler(IUnitOfWork unitOfWork, IUtilityService utilityService)
        {
            _unitOfWork = unitOfWork;
            _utilityService = utilityService;
        }

        public async Task<BaseResponse<string>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            var user = _unitOfWork.UserRepository.Get(filter: x => x.Email == request.Mail).FirstOrDefault();

            if (user == null)
            {
                return BaseResponse<string>.FailureResponse("Tài khoản không tồn tại");
            }

            if (user.Status == (int)UserStatus.ACTIVE_STATUS)
            {
                return BaseResponse<string>.SuccessResponse("Tài khoản đã được xác minh trước đó");
            }

            if (string.IsNullOrWhiteSpace(user.OtpCodeHash) || user.OtpCodeExpiryUtc == null)
            {
                return BaseResponse<string>.FailureResponse("Mã OTP không hợp lệ, vui lòng đăng ký lại");
            }

            if (user.OtpCodeExpiryUtc < DateTime.UtcNow)
            {
                return BaseResponse<string>.FailureResponse("Mã OTP đã hết hạn");
            }

            if (!_utilityService.VerifyPassword(request.Otp, user.OtpCodeHash))
            {
                return BaseResponse<string>.FailureResponse("Mã OTP không chính xác");
            }

            user.Status = (int)UserStatus.ACTIVE_STATUS;
            user.OtpCodeHash = null;
            user.OtpCodeExpiryUtc = null;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BaseResponse<string>.SuccessResponse("Xác minh tài khoản thành công");
        }
    }
}
