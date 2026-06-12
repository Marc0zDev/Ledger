using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Infrastructure.Data.Models
{
    public class ArquivoModel
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string Extensao { get; set; }
        public string ContentType { get; set; }
        public byte[] ArquivoByte { get; set; }
    }
}
