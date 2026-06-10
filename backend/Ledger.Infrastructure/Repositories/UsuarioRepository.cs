using AutoMapper;
using Ledger.Domain.Entities;
using Ledger.Domain.Interfaces;
using Ledger.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Ledger.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public UsuarioRepository(UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _userManager = userManager;
        _mapper      = mapper;
    }

    public async Task<UsuarioDomain?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        return user is null ? null : _mapper.Map<UsuarioDomain>(user);
    }

    public async Task<UsuarioDomain?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is null ? null : _mapper.Map<UsuarioDomain>(user);
    }

    public async Task<bool> ExisteEmailAsync(string email, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is not null;
    }

    // AddAsync/UpdateAsync/DeleteAsync não são usados diretamente — UserManager gerencia
    public Task AddAsync(UsuarioDomain entity, CancellationToken ct = default)    => throw new NotSupportedException("Use IIdentityService para criar usuários.");
    public Task UpdateAsync(UsuarioDomain entity, CancellationToken ct = default) => throw new NotSupportedException("Use UserManager diretamente.");
    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var user = _userManager.FindByIdAsync(id.ToString()).GetAwaiter().GetResult();
        if (user is not null) _userManager.DeleteAsync(user).GetAwaiter().GetResult();
        return Task.CompletedTask;
    }
    public Task<IEnumerable<UsuarioDomain>> GetAllAsync(CancellationToken ct = default)
    {
        var users = _userManager.Users.ToList();
        return Task.FromResult(_mapper.Map<IEnumerable<UsuarioDomain>>(users));
    }
}
