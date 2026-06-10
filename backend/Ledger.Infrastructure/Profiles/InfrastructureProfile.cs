using AutoMapper;
using Ledger.Domain.Entities;
using Ledger.Domain.Enums;
using Ledger.Infrastructure.Data.Models;
namespace Ledger.Infrastructure.Profiles;

public class InfrastructureProfile : Profile
{
    public InfrastructureProfile()
    {
        // ── Model → Domain (via Reconstituir) ────────────────────────────

        // ApplicationUser (Identity) → UsuarioDomain (projeção de domínio)
        CreateMap<ApplicationUser, UsuarioDomain>()
            .ConstructUsing(src => UsuarioDomain.Reconstituir(
                src.Id, src.Nome, src.Email!, src.CreatedAt, src.UpdatedAt))
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<ParticipanteModel, ParticipanteDomain>()
            .ConstructUsing((src, ctx) => ParticipanteDomain.Reconstituir(
                src.Id, src.CofreId, src.UsuarioId,
                src.Usuario != null ? ctx.Mapper.Map<UsuarioDomain>(src.Usuario) : null,
                src.CreatedAt, src.UpdatedAt))
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<DespesaModel, DespesaDomain>()
            .ConstructUsing(src => DespesaDomain.Reconstituir(
                src.Id, src.Descricao, src.Valor,
                src.DataVencimento, src.DataPagamento, src.Paga, src.BoletoPath,
                src.UsuarioId, (CategoriaDespesa)src.Categoria, src.Recorrente,
                src.CreatedAt, src.UpdatedAt))
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<MovimentacaoModel, MovimentacaoDomain>()
            .ConstructUsing(src => MovimentacaoDomain.Reconstituir(
                src.Id, src.Descricao, src.Valor, (TipoMovimentacao)src.Tipo,
                src.Data, src.CofreId, src.UsuarioId,
                src.CreatedAt, src.UpdatedAt))
            .ForAllMembers(opt => opt.Ignore());

        CreateMap<CofreModel, CofreDomain>()
            .ConstructUsing((src, ctx) => CofreDomain.Reconstituir(
                src.Id, src.Nome, src.Meta, src.Descricao,
                (CofreStatus)src.Status, (CategoriaCofre)src.Categoria,
                src.CriadoPorUsuarioId, src.CreatedAt, src.UpdatedAt,
                ctx.Mapper.Map<IEnumerable<ParticipanteDomain>>(src.Participantes),
                ctx.Mapper.Map<IEnumerable<MovimentacaoDomain>>(src.Movimentacoes)))
            .ForAllMembers(opt => opt.Ignore());

        // ── Domain → Model (apenas escalares; coleções gerenciadas pelo EF) ─

        CreateMap<ParticipanteDomain, ParticipanteModel>()
            .ForMember(m => m.Cofre,   opt => opt.Ignore())
            .ForMember(m => m.Usuario, opt => opt.Ignore());

        CreateMap<DespesaDomain, DespesaModel>()
            .ForMember(m => m.Paga,           opt => opt.MapFrom(d => d.Paga))
            .ForMember(m => m.DataVencimento,  opt => opt.MapFrom(d => d.DataVencimento))
            .ForMember(m => m.DataPagamento,   opt => opt.MapFrom(d => d.DataPagamento))
            .ForMember(m => m.BoletoPath,      opt => opt.MapFrom(d => d.BoletoPath))
            .ForMember(m => m.UsuarioId,       opt => opt.MapFrom(d => d.UsuarioId))
            .ForMember(m => m.Categoria,       opt => opt.MapFrom(d => (int)d.Categoria))
            .ForMember(m => m.Recorrente,      opt => opt.MapFrom(d => d.Recorrente));

        CreateMap<MovimentacaoDomain, MovimentacaoModel>()
            .ForMember(m => m.Tipo,  opt => opt.MapFrom(d => (int)d.Tipo))
            .ForMember(m => m.Cofre, opt => opt.Ignore());

        CreateMap<CofreDomain, CofreModel>()
            .ForMember(m => m.Status,        opt => opt.MapFrom(d => (int)d.Status))
            .ForMember(m => m.Categoria,     opt => opt.MapFrom(d => (int)d.Categoria))
            .ForMember(m => m.Participantes, opt => opt.Ignore())
            .ForMember(m => m.Movimentacoes, opt => opt.Ignore());

        // ── Convite ───────────────────────────────────────────────────────

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
