using AutoMapper;
using MediatR;
using Resource.Interfaces;
using Service.Queries.Cafes;

namespace Service.Handlers.Cafes
{
    public class GetAllCafesQueryHandler : IRequestHandler<GetAllCafesQuery, IEnumerable<CafeDto>>
    {
        private readonly ICafeResource cafeResource;
        private readonly IMapper mapper;

        public GetAllCafesQueryHandler(ICafeResource cafeResource, IMapper mapper)
        {
            this.cafeResource = cafeResource;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<CafeDto>> Handle(GetAllCafesQuery request, CancellationToken cancellationToken)
        {
            var cafes = await cafeResource.GetAllAsync();
            
            return mapper.Map<IEnumerable<CafeDto>>(cafes);
        }
    }
} 