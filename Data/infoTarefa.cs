using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OminiHost_Server.Data
{
    public class infoTarefa
    {
        public string statusAtual { get; set; }
        public string tipoTarefa { get; set; }
        public int? progresso { get; set; }
        public string msg { get; set; }
    }
}
