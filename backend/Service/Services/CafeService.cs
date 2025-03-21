using AutoMapper;
using Business.Entities;
using MediatR;
using Service.Commands.Cafes;
using Service.Interfaces;
using Service.Queries.Cafes;
using System.Runtime.ConstrainedExecution;

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

        public async Task<Cafe?> GetByIdAsync(Guid id)
        {
            GetCafeByIdQuery query = new GetCafeByIdQuery { Id = id };

            CafeDto? cafeDto = await mediator.Send(query);
            
            if (cafeDto == null)
                return null;
                
            return mapper.Map<Cafe>(cafeDto);
        }

        public async Task<IEnumerable<Cafe>> GetByLocationAsync(string location)
        {
            GetCafesByLocationQuery query = new GetCafesByLocationQuery { Location = location };

            IEnumerable<CafeDto> cafeDtos = await mediator.Send(query);
            
            return mapper.Map<IEnumerable<Cafe>>(cafeDtos);
        }

        public async Task<IEnumerable<Cafe>> GetAllAsync()
        {
            GetAllCafesQuery query = new GetAllCafesQuery();

            IEnumerable<CafeDto> cafeDtos = await mediator.Send(query);

            return mapper.Map<IEnumerable<Cafe>>(cafeDtos);
        }

        public async Task<Cafe> CreateAsync(Cafe cafe)
        {
            CreateCafeCommand command = new CreateCafeCommand
            {
                Name = cafe.Name,
                Description = cafe.Description,
                Logo = cafe.Logo,
                Location = cafe.Location
            };
            
            return await mediator.Send(command);
        }

        public async Task<Cafe?> UpdateAsync(Cafe cafe)
        {
            UpdateCafeCommand command = new UpdateCafeCommand
            {
                Id = cafe.Id,
                Name = cafe.Name,
                Description = cafe.Description,
                Logo = cafe.Logo,
                Location = cafe.Location
            };
            
            return await mediator.Send(command);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            DeleteCafeCommand command = new DeleteCafeCommand { Id = id };

            return await mediator.Send(command);
        }
    }
} 