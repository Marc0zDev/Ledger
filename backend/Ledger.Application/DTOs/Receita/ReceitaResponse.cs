using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Application.DTOs.Receita
{
    public class ReceitaResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public Guid? ArquivoId { get; set; }
        public DateTime DataRecebimento { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
