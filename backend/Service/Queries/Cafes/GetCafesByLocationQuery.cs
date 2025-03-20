using MediatR;

namespace Service.Queries.Cafes
{
    public class GetCafesByLocationQuery : IRequest<IEnumerable<CafeDto>>
    {
        public string Location { get; set; } = string.Empty;
    }
} 