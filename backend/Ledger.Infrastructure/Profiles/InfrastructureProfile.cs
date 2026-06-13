using AutoMapper;
using Ledger.Domain.Entities;
using Ledger.Domain.Enums;
using Ledger.Infrastructure.Data.Models;namespace Ledger.Infrastructure.Profiles;

public class InfrastructureProfile : Profile
{
    public InfrastructureProfile()
    {
        // -- Model ? Domain (via Reconstituir) ----------------------------

        CreateMap<ApplicationUser, UsuarioDomain>()
            .ConstructUsing(src => UsuarioDomain.Reconstituir(
                src.Id, src.Nome, src.Email!, src.CreatedAt, src.UpdatedAt))
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<ParticipanteModel, ParticipanteDomain>()
            .ConstructUsing((src, ctx) => ParticipanteDomain.Reconstituir(
                src.Id, src.CofreId, src.UsuarioId,
                (RoleParticipante)src.Role,
                src.Usuario != null ? ctx.Mapper.Map<UsuarioDomain>(src.Usuario) : null,
                src.CreatedAt, src.UpdatedAt))
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<CategoriaModel, CategoriaDomain>()
            .ConstructUsing(src => CategoriaDomain.Reconstituir(
                src.Id, src.UsuarioId, src.Nome, src.Icone, src.Cor,
                src.CreatedAt, src.UpdatedAt))
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<DespesaModel, DespesaDomain>()
            .ConstructUsing(src => DespesaDomain.Reconstituir(
                src.Id, src.Nome, (TipoDespesa)src.Tipo, src.ValorPlanejado,
                src.DiaVencimento, src.Ativa, src.ArquivoId, src.CategoriaId, src.UsuarioId,
                src.CreatedAt, src.UpdatedAt))
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<DespesaPeriodoModel, DespesaPeriodoDomain>()
            .ConstructUsing(src => DespesaPeriodoDomain.Reconstituir(
                src.Id, src.DespesaId, src.CategoriaId, src.UsuarioId, src.Descricao,
                src.ValorPlanejado, src.ValorRealizado, src.PagaEm, src.BoletoPath,
                src.ComprovanteId, src.Competencia, src.CreatedAt, src.UpdatedAt))
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<MovimentacaoModel, MovimentacaoDomain>()
            .ConstructUsing(src => MovimentacaoDomain.Reconstituir(
                src.Id, src.Descricao, src.Valor, (TipoMovimentacao)src.Tipo,
                src.Data, src.CofreId, src.UsuarioId,
                src.CreatedAt, src.UpdatedAt,
                src.Usuario != null ? src.Usuario.Nome : null))
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<CofreModel, CofreDomain>()
            .ConstructUsing((src, ctx) => CofreDomain.Reconstituir(
                src.Id, src.Nome, src.Meta, src.Descricao,
                (CofreStatus)src.Status, (CategoriaCofre)src.Categoria,
                (VisibilidadeCofre)src.Visibilidade,
                src.CriadoPorUsuarioId, src.CreatedAt, src.UpdatedAt,
                ctx.Mapper.Map<IEnumerable<ParticipanteDomain>>(src.Participantes),
                ctx.Mapper.Map<IEnumerable<MovimentacaoDomain>>(src.Movimentacoes)))
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<ArquivoModel, ArquivoDomain>()
            .ConstructUsing(src => ArquivoDomain.Reconstituir(
                src.Id, src.Nome, src.Extensao, src.ContentType, src.ArquivoByte,
                src.DataCriacao, src.DataAlteracao))
            .ForAllMembers(opt => opt.Ignore());

        // -- Domain ? Model ------------------------------------------------

        CreateMap<ParticipanteDomain, ParticipanteModel>()
            .ForMember(m => m.Role,    opt => opt.MapFrom(d => (int)d.Role))
            .ForMember(m => m.Cofre,   opt => opt.Ignore())
            .ForMember(m => m.Usuario, opt => opt.Ignore());

        CreateMap<CategoriaDomain, CategoriaModel>();

        CreateMap<DespesaDomain, DespesaModel>()
            .ForMember(m => m.Tipo,     opt => opt.MapFrom(d => (int)d.Tipo))
            .ForMember(m => m.Categoria, opt => opt.Ignore());

        CreateMap<DespesaPeriodoDomain, DespesaPeriodoModel>()
            .ForMember(m => m.Despesa,   opt => opt.Ignore())
            .ForMember(m => m.Categoria, opt => opt.Ignore());

        CreateMap<MovimentacaoDomain, MovimentacaoModel>()
            .ForMember(m => m.Tipo,  opt => opt.MapFrom(d => (int)d.Tipo))
            .ForMember(m => m.Cofre, opt => opt.Ignore());

        CreateMap<CofreDomain, CofreModel>()
            .ForMember(m => m.Status,        opt => opt.MapFrom(d => (int)d.Status))
            .ForMember(m => m.Categoria,     opt => opt.MapFrom(d => (int)d.Categoria))
            .ForMember(m => m.Visibilidade,  opt => opt.MapFrom(d => (int)d.Visibilidade))
            .ForMember(m => m.Participantes, opt => opt.Ignore())
            .ForMember(m => m.Movimentacoes, opt => opt.Ignore());

        CreateMap<ArquivoDomain, ArquivoModel>()
            .ForMember(m => m.DataCriacao,  opt => opt.MapFrom(d => d.CreatedAt))
            .ForMember(m => m.DataAlteracao, opt => opt.MapFrom(d => d.UpdatedAt ?? d.CreatedAt));

        // -- Convite -------------------------------------------------------

        CreateMap<ReceitaModel, ReceitaDomain>()
            .ConstructUsing(src => ReceitaDomain.Reconstituir(
                src.Id, src.Nome, src.Valor, src.Descricao, src.ArquivoId,
                src.DataRecebimento, src.UsuarioId, src.DataCriacao, src.DataAtualizacao ?? src.DataCriacao))
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<ReceitaDomain, ReceitaModel>()
            .ForMember(m => m.DataCriacao,    opt => opt.MapFrom(d => d.CreatedAt))
            .ForMember(m => m.DataAtualizacao, opt => opt.MapFrom(d => d.UpdatedAt))
            .ForMember(m => m.Arquivo,        opt => opt.Ignore())
            .ForMember(m => m.Usuario,        opt => opt.Ignore());

        CreateMap<ConviteModel, ConviteDomain>()
            .ConstructUsing(src => ConviteDomain.Reconstituir(
                src.Id, src.CofreId, src.ConvidadoPorUsuarioId, src.UsuarioId,
                src.Token, (ConviteStatus)src.Status, src.ExpiresAt,
                src.CreatedAt, src.UpdatedAt))
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<ConviteDomain, ConviteModel>()
            .ForMember(m => m.Status,   opt => opt.MapFrom(d => (int)d.Status))
            .ForMember(m => m.Cofre,    opt => opt.Ignore());
    }
}