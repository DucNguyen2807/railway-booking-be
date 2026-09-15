using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Railway;
using RailwayBooking.Domain.Interfaces;
using System.Collections.Generic;

namespace RailwayBooking.Application.Features.Railway.Coaches
{
    public class GetCoachesQueryHandler : IRequestHandler<GetCoachesQuery, BaseResponse<PagedResult<CoachDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCoachesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<PagedResult<CoachDto>>> Handle(GetCoachesQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.CoachRepository.Get(filter: request.TrainId.HasValue ? (System.Linq.Expressions.Expression<Func<RailwayBooking.Infrastructure.Entities.Railway.Coach, bool>>)(c => c.TrainId == request.TrainId.Value) : null,
                orderBy: q => q.OrderBy(x => x.CoachNumber));

            var total = await query.CountAsync(cancellationToken);
            var items = await query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
            var dtos = _mapper.Map<List<CoachDto>>(items);

            var paged = new PagedResult<CoachDto>
            {
                Items = dtos,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = total
            };

            return BaseResponse<PagedResult<CoachDto>>.SuccessResponse(paged);
        }
    }
}
