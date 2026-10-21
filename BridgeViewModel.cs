using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTILAB1
{
    public class BridgeViewModel
    {
        [DisplayName("Bridge Name")]
        public string BridgeName { get; set; }

        [DisplayName("Interface/Port")]
        public string PortName { get; set; }

        [DisplayName("Port Status")]
        public string PortStatus { get; set; }

        [DisplayName("Bridge Running")]
        public string BridgeRunning { get; set; }
    }
}
