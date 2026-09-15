using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Station;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.Stations
{
    public class GetStationsQueryHandler : IRequestHandler<GetStationsQuery, BaseResponse<PagedResult<StationDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetStationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<PagedResult<StationDto>>> Handle(GetStationsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.StationRepository.Get(orderBy: q => q.OrderBy(x => x.Name));

            var total = await query.CountAsync(cancellationToken);
            var items = await query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
            var dtos = _mapper.Map<List<StationDto>>(items);

            var paged = new PagedResult<StationDto>
            {
                Items = dtos,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = total
            };

            return BaseResponse<PagedResult<StationDto>>.SuccessResponse(paged);
        }
    }
}
