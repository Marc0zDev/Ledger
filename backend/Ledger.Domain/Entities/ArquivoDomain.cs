using Ledger.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Domain.Entities
{
    public class ArquivoDomain : BaseDomain
    {
        public string Nome { get; private set; } = string.Empty;
        public string Extensao { get; private set; } = string.Empty;
        public string ContentType { get; private set; } = string.Empty;
        public byte[] ArquivoByte { get; private set; } = Array.Empty<byte>();

        public ArquivoDomain() { }
         public ArquivoDomain(Guid id,string nome, string extensao, string contentType, byte[] arquivoByte, DateTime createdAt, DateTime? updatedAt)
        {
            Id = id;
            Nome = nome;
            Extensao = extensao;
            ContentType = contentType;
            ArquivoByte = arquivoByte;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            Validate();
        }

        public static ArquivoDomain Criar(string nome, string extensao, string contentType, byte[] arquivoByte)
                => new(Guid.NewGuid(), nome, extensao, contentType, arquivoByte, DateTime.UtcNow, null);

        public static ArquivoDomain Reconstituir(Guid id, string nome, string extensao, string contentType, byte[] arquivoByte,
            DateTime createdAt, DateTime? updatedAt)
            => new(id, nome, extensao, contentType, arquivoByte, createdAt, updatedAt);

        public void Atualizar(string nome, string extensao, string contentType, byte[] arquivoByte)
        {
            Nome = nome;
            Extensao = extensao;
            ContentType = contentType;
            ArquivoByte = arquivoByte;
            UpdatedAt = DateTime.UtcNow;
            Validate();
        }

        protected override void Validate()
        {
            RuleFor(!string.IsNullOrWhiteSpace(Nome), nameof(Nome), "O nome do arquivo é obrigatório.");
            RuleFor(ArquivoByte != null, nameof(ArquivoByte), "Os bytes do arquivo devem ser enviados");
            RuleFor(!(Extensao.Length > 20), nameof(Extensao), "O Tamanho da extensão do arquivo deve ter menos que 20 caracteres");
        }
    }
}
