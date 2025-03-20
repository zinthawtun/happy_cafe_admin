using MediatR;
using Resource.Interfaces;
using Service.Commands.Cafes;

namespace Service.Handlers.Cafes
{
    public class DeleteCafeCommandHandler : IRequestHandler<DeleteCafeCommand, bool>
    {
        private readonly ICafeResource cafeResource;

        public DeleteCafeCommandHandler(ICafeResource cafeResource)
        {
            this.cafeResource = cafeResource;
        }

        public async Task<bool> Handle(DeleteCafeCommand request, CancellationToken cancellationToken)
        {
            return await cafeResource.DeleteAsync(request.Id);
        }
    }
} 