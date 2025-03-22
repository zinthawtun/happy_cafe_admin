using MediatR;
using Resource.Interfaces;
using Service.Queries.Cafes;

namespace Service.Handlers.Cafes
{
    public class ExistsCafeByNameQueryHandler : IRequestHandler<ExistsCafeByNameQuery, bool>
    {
        private readonly ICafeResource cafeResource;

        public ExistsCafeByNameQueryHandler(ICafeResource cafeResource)
        {
            this.cafeResource = cafeResource;
        }

        public async Task<bool> Handle(ExistsCafeByNameQuery request, CancellationToken cancellationToken)
        {
            return await cafeResource.ExistsByNameAsync(request.Name);
        }
    }
} 