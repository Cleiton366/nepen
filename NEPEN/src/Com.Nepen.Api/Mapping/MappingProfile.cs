using AutoMapper;
using Desafio_NEPEN.Com.Nepen.Core.Entities;
using Desafio_NEPEN.Com.Nepen.Api.Dtos.Leitura;
using Desafio_NEPEN.Com.Nepen.Api.Dtos.Medidor;

namespace Desafio_NEPEN.Com.Nepen.Api.Mapping;


public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Leitura, LeituraDto>();
        CreateMap<LeituraCreateDto, Leitura>()
            .ForMember(dest => dest.DataCriacao, opt => opt.MapFrom(_ => DateTime.UtcNow));
        
        CreateMap<Medidor, MedidorDto>()
            .ForMember(dest => dest.TotalRegistros, 
                opt => opt.MapFrom(src => src.Leituras.Count))
            .ForMember(dest => dest.Periodo, 
                opt => opt.MapFrom(src => new PeriodoDto
            {
                Inicio = src.Leituras.Min(l => l.Timestamp),
                Fim = src.Leituras.Max(l => l.Timestamp)
            }));
    }
}