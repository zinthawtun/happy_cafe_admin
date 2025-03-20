using AutoMapper;
using Business.Entities;
using MediatR;
using Resource.Interfaces;
using Service.Commands.Cafes;

namespace Service.Handlers.Cafes
{
    public class CreateCafeCommandHandler : IRequestHandler<CreateCafeCommand, Cafe>
    {
        private readonly ICafeResource cafeResource;
        private readonly IMapper mapper;

        public CreateCafeCommandHandler(ICafeResource cafeResource, IMapper mapper)
        {
            this.cafeResource = cafeResource;
            this.mapper = mapper;
        }

        public async Task<Cafe> Handle(CreateCafeCommand request, CancellationToken cancellationToken)
        {
            return await cafeResource.CreateAsync(
                request.Name,
                request.Description,
                request.Logo,
                request.Location);
        }
    }
} 