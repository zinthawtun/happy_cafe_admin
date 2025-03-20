using MediatR;

namespace Service.Queries.Cafes
{
    public class GetCafeByIdQuery : IRequest<CafeDto?>
    {
        public Guid Id { get; set; }
    }
} 