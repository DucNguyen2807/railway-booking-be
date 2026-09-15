using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;
using RailwayBooking.Infrastructure.Entities.Railway;

namespace RailwayBooking.Application.Features.Railway.Coaches
{
    public class CreateCoachCommandHandler : IRequestHandler<CreateCoachCommand, BaseResponse<long>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCoachCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<long>> Handle(CreateCoachCommand request, CancellationToken cancellationToken)
        {
            var coach = new Coach
            {
                TrainId = request.TrainId,
                CoachNumber = request.CoachNumber,
                ClassType = request.ClassType,
                SeatCapacity = request.SeatCapacity,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.CoachRepository.InsertAsync(coach);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BaseResponse<long>.SuccessResponse(coach.Id, "Created");
        }
    }
}
