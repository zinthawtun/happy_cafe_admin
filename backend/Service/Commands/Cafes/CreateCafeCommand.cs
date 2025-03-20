using Business.Entities;
using MediatR;

namespace Service.Commands.Cafes
{
    public class CreateCafeCommand : IRequest<Cafe>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
} 