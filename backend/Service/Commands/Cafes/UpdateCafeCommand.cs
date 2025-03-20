using Business.Entities;
using MediatR;

namespace Service.Commands.Cafes
{
    public class UpdateCafeCommand : IRequest<Cafe?>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
} 