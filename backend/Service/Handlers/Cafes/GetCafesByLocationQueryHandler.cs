using AutoMapper;
using MediatR;
using Resource.Interfaces;
using Service.Queries.Cafes;

namespace Service.Handlers.Cafes
{
    public class GetCafesByLocationQueryHandler : IRequestHandler<GetCafesByLocationQuery, IEnumerable<CafeDto>>
    {
        private readonly ICafeResource cafeResource;
        private readonly IMapper mapper;

        public GetCafesByLocationQueryHandler(ICafeResource cafeResource, IMapper mapper)
        {
            this.cafeResource = cafeResource;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<CafeDto>> Handle(GetCafesByLocationQuery request, CancellationToken cancellationToken)
        {
            var cafes = await cafeResource.GetByLocationLikeAsync(request.Location);
            
            return mapper.Map<IEnumerable<CafeDto>>(cafes);
        }
    }
} 