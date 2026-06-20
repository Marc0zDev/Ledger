using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Application.DTOs.Arquivo
{
    public class ArquivoRequest
    {
        public string Nome { get; set; }
        public string Extensao { get; set; }
        public Guid DespesaID { get; set; }
        public byte[] ArquivoByte { get; set; }
        public string Content { get; set; }
    }
}
