using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;
using RailwayBooking.Infrastructure.Entities.Railway;

namespace RailwayBooking.Application.Features.Railway.Seats
{
    public class CreateSeatCommandHandler : IRequestHandler<CreateSeatCommand, BaseResponse<long>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateSeatCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<long>> Handle(CreateSeatCommand request, CancellationToken cancellationToken)
        {
            var seat = new Seat
            {
                CoachId = request.CoachId,
                SeatNumber = request.SeatNumber,
                SeatType = request.SeatType,
                RowNumber = request.RowNumber,
                ColumnNumber = request.ColumnNumber,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.SeatRepository.InsertAsync(seat);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BaseResponse<long>.SuccessResponse(seat.Id, "Created");
        }
    }
}
