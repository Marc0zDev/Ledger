using AutoMapper;
using Ledger.Application.DTOs.Categoria;
using Ledger.Application.DTOs.Cofre;
using Ledger.Application.DTOs.Despesa;
using Ledger.Application.DTOs.DespesaPeriodo;
using Ledger.Application.DTOs.Movimentacao;
using Ledger.Application.DTOs.Participante;
using Ledger.Application.DTOs.Usuario;
using Ledger.Domain.Entities;

namespace Ledger.Application.Profiles;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<UsuarioDomain, UsuarioResponse>();

        CreateMap<ParticipanteDomain, ParticipanteResponse>()
            .ForMember(r => r.Role,  opt => opt.MapFrom(d => d.Role.ToString()))
            .ForMember(r => r.Nome,  opt => opt.MapFrom(d => d.Usuario != null ? d.Usuario.Nome  : string.Empty))
            .ForMember(r => r.Email, opt => opt.MapFrom(d => d.Usuario != null ? d.Usuario.Email : string.Empty));

        CreateMap<CategoriaDomain, CategoriaResponse>()
            .ForMember(r => r.IsSystem, opt => opt.MapFrom(d => d.IsSystem));

        CreateMap<DespesaDomain, DespesaResponse>()
            .ForMember(r => r.Tipo,           opt => opt.MapFrom(d => d.Tipo.ToString()))
            .ForMember(r => r.CategoriaNome,  opt => opt.Ignore())
            .ForMember(r => r.CategoriaIcone, opt => opt.Ignore())
            .ForMember(r => r.CategoriaCor,   opt => opt.Ignore());

        CreateMap<DespesaPeriodoDomain, DespesaPeriodoResponse>()
            .ForMember(r => r.Paga,           opt => opt.MapFrom(d => d.Paga))
            .ForMember(r => r.BoletoUrl,      opt => opt.MapFrom(d => d.BoletoPath))
            .ForMember(r => r.CategoriaNome,  opt => opt.Ignore())
            .ForMember(r => r.CategoriaIcone, opt => opt.Ignore())
            .ForMember(r => r.CategoriaCor,   opt => opt.Ignore());

        CreateMap<MovimentacaoDomain, MovimentacaoResponse>()
            .ForMember(r => r.Tipo,        opt => opt.MapFrom(d => d.Tipo.ToString()))
            .ForMember(r => r.UsuarioNome, opt => opt.MapFrom(d => d.UsuarioNome));

        CreateMap<CofreDomain, CofreResponse>()
            .ForMember(r => r.Status,       opt => opt.MapFrom(d => d.Status.ToString()))
            .ForMember(r => r.Categoria,    opt => opt.MapFrom(d => d.Categoria.ToString()))
            .ForMember(r => r.Visibilidade, opt => opt.MapFrom(d => d.Visibilidade.ToString()))
            .ForMember(r => r.Movimentacoes, opt => opt.MapFrom(d => d.Movimentacoes));
    }
}

