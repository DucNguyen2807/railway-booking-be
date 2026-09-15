using AutoMapper;
using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Auth;
using RailwayBooking.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Application.Features.UserFeat.Auth.CurrentUser
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, BaseResponse<CurrentUserResponse>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCurrentUserQueryHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<CurrentUserResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = _unitOfWork.UserRepository.GetByID(Guid.Parse(_currentUserService.GetUserId()));

            var currentUser = _mapper.Map<CurrentUserResponse>(user);

            if (currentUser == null)
            {
                return BaseResponse<CurrentUserResponse>.FailureResponse("Người dùng không hợp lệ");
            }

            return BaseResponse<CurrentUserResponse>.SuccessResponse(currentUser);
        }
    }

}
