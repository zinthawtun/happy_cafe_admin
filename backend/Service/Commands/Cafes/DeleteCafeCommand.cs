using MediatR;

namespace Service.Commands.Cafes
{
    public class DeleteCafeCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
} 