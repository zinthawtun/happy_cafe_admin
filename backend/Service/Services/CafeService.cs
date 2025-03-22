using AutoMapper;
using Business.Entities;
using MediatR;
using Service.Commands.Cafes;
using Service.Interfaces;
using Service.Queries.Cafes;

namespace Service.Services
{
    public class CafeService : ICafeService
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public CafeService(IMediator mediator, IMapper mapper)
        {
            this.mediator = mediator;
            this.mapper = mapper;
        }

        public async Task<CafeDto?> GetByIdAsync(Guid id)
        {
            GetCafeByIdQuery query = new GetCafeByIdQuery { Id = id };

            return await mediator.Send(query);
        }

        public async Task<IEnumerable<CafeDto>> GetByLocationAsync(string location)
        {
            GetCafesByLocationQuery query = new GetCafesByLocationQuery { Location = location };

            return await mediator.Send(query);
        }

        public async Task<IEnumerable<CafeDto>> GetAllAsync()
        {
            GetAllCafesQuery query = new GetAllCafesQuery();

            return await mediator.Send(query);
        }

        public async Task<CafeDto?> CreateAsync(CreateCafeCommand command)
        {
            Cafe cafe = await mediator.Send(command);

            return mapper.Map<CafeDto>(cafe);
        }

        public async Task<CafeDto?> UpdateAsync(UpdateCafeCommand command)
        {
            Cafe? cafe = await mediator.Send(command);

            if (cafe == null)
                return null;

            return mapper.Map<CafeDto>(cafe);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            DeleteCafeCommand command = new DeleteCafeCommand { Id = id };

            return await mediator.Send(command);
        }
    }
} 