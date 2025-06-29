using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pinger
{
    internal class TargetHost
    {
        public string IP { get; set; } = string.Empty;
        public string? Hostname { get; set; } = null;
        public bool ISMP_OK { get; set; } = false;
    }
}
