using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Application.DTOs.Receita
{
    public class ReceitaRequest
    {
        public Guid UsuarioId { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public Guid? ArquivoId { get; set; }
        public DateTime DataRecebimento { get; set; }
    }
}
