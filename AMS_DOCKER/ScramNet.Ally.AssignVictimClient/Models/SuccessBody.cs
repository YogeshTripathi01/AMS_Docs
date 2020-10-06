using System;
using System.Collections.Generic;
using System.Text;

namespace ScramNet.Ally.AssignVictimClient.Models
{
    public class SuccessBody
    {
        public Guid VictimId { get; set; }
        public Guid ClientId { get; set; }
    }
}
