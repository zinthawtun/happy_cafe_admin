using MediatR;

namespace Service.Queries.Cafes
{
    public class GetAllCafesQuery : IRequest<IEnumerable<CafeDto>>
    {
    }
} 