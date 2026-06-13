using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Infrastructure.Data.Models
{
    public class ReceitaModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public Guid? ArquivoId { get; set; }
        public DateTime DataRecebimento { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public Guid UsuarioId { get; set; }

        public virtual ArquivoModel? Arquivo { get; set; }
        public virtual ApplicationUser Usuario { get; set; }
    }
}
