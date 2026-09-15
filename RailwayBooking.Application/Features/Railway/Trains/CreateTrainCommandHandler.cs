using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;
using RailwayBooking.Infrastructure.Entities.Railway;

namespace RailwayBooking.Application.Features.Railway.Trains
{
    public class CreateTrainCommandHandler : IRequestHandler<CreateTrainCommand, BaseResponse<long>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateTrainCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<long>> Handle(CreateTrainCommand request, CancellationToken cancellationToken)
        {
            var train = new Train
            {
                Code = request.Code,
                Name = request.Name,
                Status = 1,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.TrainRepository.InsertAsync(train);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BaseResponse<long>.SuccessResponse(train.Id, "Created");
        }
    }
}
