using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyContracts
{
    /// <summary>
    /// status message payload from door to cloud
    /// </summary>
    public  class DoorStatus
    {
        public string Operation { get; set; }

        public string Status { get; set; }

        public string UserName { get; set; }

        public string UserId { get; set; }

    }
}
