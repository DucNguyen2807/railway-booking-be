using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Railway;
using RailwayBooking.Domain.Interfaces;
using System.Collections.Generic;

namespace RailwayBooking.Application.Features.Railway.Trains
{
    public class GetTrainsQueryHandler : IRequestHandler<GetTrainsQuery, BaseResponse<PagedResult<TrainDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTrainsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<PagedResult<TrainDto>>> Handle(GetTrainsQuery request, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.TrainRepository.Get(orderBy: q => q.OrderBy(x => x.Name));

            var total = await query.CountAsync(cancellationToken);
            var items = await query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
            var dtos = _mapper.Map<List<TrainDto>>(items);

            var paged = new PagedResult<TrainDto>
            {
                Items = dtos,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = total
            };

            return BaseResponse<PagedResult<TrainDto>>.SuccessResponse(paged);
        }
    }
}
