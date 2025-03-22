using MediatR;

namespace Service.Queries.Cafes
{
    public class ExistsCafeByNameQuery : IRequest<bool>
    {
        public string Name { get; set; } = string.Empty;
    }
} 