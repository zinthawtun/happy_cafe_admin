using AutoMapper;
using Business.Entities;
using MediatR;
using Resource.Interfaces;
using Service.Commands.Cafes;

namespace Service.Handlers.Cafes
{
    public class UpdateCafeCommandHandler : IRequestHandler<UpdateCafeCommand, Cafe?>
    {
        private readonly ICafeResource cafeResource;
        private readonly IMapper mapper;

        public UpdateCafeCommandHandler(ICafeResource cafeResource, IMapper mapper)
        {
            this.cafeResource = cafeResource;
            this.mapper = mapper;
        }

        public async Task<Cafe?> Handle(UpdateCafeCommand request, CancellationToken cancellationToken)
        {
            return await cafeResource.UpdateAsync(
                request.Id,
                request.Name,
                request.Description,
                request.Logo,
                request.Location);
        }
    }
} 