using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Application.DTOs.Arquivo
{
    public class ArquivoResponse
    {
        public Guid     Id          { get; set; }
        public string   Nome        { get; set; } = string.Empty;
        public DateTime DataUpload   { get; set; }
    }
}
