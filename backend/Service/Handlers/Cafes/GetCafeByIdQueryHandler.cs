using AutoMapper;
using MediatR;
using Resource.Interfaces;
using Service.Queries.Cafes;

namespace Service.Handlers.Cafes
{
    public class GetCafeByIdQueryHandler : IRequestHandler<GetCafeByIdQuery, CafeDto?>
    {
        private readonly ICafeResource cafeResource;
        private readonly IMapper mapper;

        public GetCafeByIdQueryHandler(ICafeResource cafeResource, IMapper mapper)
        {
            this.cafeResource = cafeResource;
            this.mapper = mapper;
        }

        public async Task<CafeDto?> Handle(GetCafeByIdQuery request, CancellationToken cancellationToken)
        {
            var cafe = await cafeResource.GetByIdAsync(request.Id);
            
            return cafe != null ? mapper.Map<CafeDto>(cafe) : null;
        }
    }
} 