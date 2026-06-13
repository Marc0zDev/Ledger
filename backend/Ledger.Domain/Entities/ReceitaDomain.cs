using Ledger.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Domain.Entities
{
    public class ReceitaDomain : BaseDomain
    {
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public Guid? ArquivoId { get; set; }
        public DateTime DataRecebimento { get; set; }
        public Guid UsuarioId { get; set; }

        public ReceitaDomain(Guid id, string nome, decimal valor, string descricao, Guid? arquivoId, DateTime dataRecebimento, Guid usuarioId, DateTime CreatAt, DateTime UpdateAt)
        {
            Id = id;
            Nome = nome;
            Valor = valor;
            Descricao = descricao;
            ArquivoId = arquivoId;
            DataRecebimento = dataRecebimento;
            UsuarioId = usuarioId;
            CreatedAt = CreatAt;
            UpdatedAt = UpdateAt;
            Validate();
        }

        public void Update(string nome, decimal valor, string descricao, Guid? arquivoId, DateTime dataRecebimento)
        {
            Nome = nome;
            Valor = valor;
            Descricao = descricao;
            ArquivoId = arquivoId;
            DataRecebimento = dataRecebimento;
            Validate();
        }

        public void UpdateValor(decimal valor){ Valor = valor; Validate();}

        public static ReceitaDomain Criar(string nome, decimal valor, string descricao, Guid? arquivoId, DateTime dataRecebimento, Guid usuarioId)
             => new ReceitaDomain(Guid.NewGuid(),nome, valor, descricao, arquivoId, dataRecebimento, usuarioId, DateTime.UtcNow, DateTime.UtcNow);

        public static ReceitaDomain Reconstituir(Guid id, string nome, decimal valor, string descricao, Guid? arquivoId, DateTime dataRecebimento, Guid usuarioId, DateTime CreatAt, DateTime UpdateAt)
            => new ReceitaDomain(id, nome, valor, descricao, arquivoId, dataRecebimento, usuarioId, CreatAt, UpdateAt);

        public void AdiocionarArquivo(Guid arquivoId)
        {
            ArquivoId = arquivoId;
            Validate();
        }

        public void RemoverArquivo()
        {
            ArquivoId = null;
            Validate();
        }

        public void AdicionarUsuario(Guid usuarioId)
        {
            UsuarioId = usuarioId;
            Validate();
        }


        protected override void Validate()
        {
            RuleFor(!string.IsNullOrWhiteSpace(Nome), nameof(Nome), "O nome da receita é obrigatório.");
            RuleFor(Nome == null || Nome.Length <= 100, nameof(Nome), "O nome da receita deve ter no máximo 100 caracteres.");
            RuleFor(Valor > 0, nameof(Valor), "O valor da receita deve ser maior que zero.");
            RuleFor(Valor <= 999_999_999, nameof(Valor), "O valor da receita é inválido.");
            RuleFor(DataRecebimento != default, nameof(DataRecebimento), "A data de recebimento é obrigatória.");
            RuleFor(UsuarioId != Guid.Empty, nameof(UsuarioId), "O usuário é obrigatório.");
        }
    }
}
