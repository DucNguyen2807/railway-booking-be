using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Railway;
using RailwayBooking.Domain.Interfaces;
using System.Collections.Generic;

namespace RailwayBooking.Application.Features.Railway.Seats
{
    public class GetSeatsQueryHandler : IRequestHandler<GetSeatsQuery, BaseResponse<PagedResult<SeatDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSeatsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<PagedResult<SeatDto>>> Handle(GetSeatsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.SeatRepository.Get(filter: request.CoachId.HasValue ? (System.Linq.Expressions.Expression<Func<RailwayBooking.Infrastructure.Entities.Railway.Seat, bool>>)(s => s.CoachId == request.CoachId.Value) : null,
                orderBy: q => q.OrderBy(x => x.SeatNumber));

            var total = await query.CountAsync(cancellationToken);
            var items = await query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
            var dtos = _mapper.Map<List<SeatDto>>(items);

            var paged = new PagedResult<SeatDto>
            {
                Items = dtos,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = total
            };

            return BaseResponse<PagedResult<SeatDto>>.SuccessResponse(paged);
        }
    }
}
