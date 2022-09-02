using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OminiHost_Server.Data
{
    public class Config
    {
        public string vcenter_ip { get; set; }
        public string vcenter_user { get; set; }
        public string vcenter_pass { get; set; }
        public string httpLink { get; set; }   
        public string password { get; set; }
        public string name { get; set; }
    }
}
