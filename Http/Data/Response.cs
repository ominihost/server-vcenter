using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OminiHost_Server.Http.Data
{
    class Response
    {
        public bool hasError = false;
        public string Error = "";
        public dynamic Result;        
    }
}
