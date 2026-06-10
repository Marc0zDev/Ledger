namespace Ledger.Domain.Exceptions;

/// <summary>
/// Lançada quando uma entidade de domínio viola regras de negócio.
/// Carrega as mensagens de validação do Flunt para a camada de apresentação.
/// </summary>
public class DomainValidationException : Exception
{
    public IReadOnlyCollection<string> Errors { get; }

    public DomainValidationException(IEnumerable<string> errors)
        : base("Validação de domínio falhou.")
    {
        Errors = errors.ToList().AsReadOnly();
    }
}
