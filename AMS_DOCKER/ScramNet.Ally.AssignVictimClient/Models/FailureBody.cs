using System;
using System.Collections.Generic;
using System.Text;

namespace ScramNet.Ally.AssignVictimClient.Models
{
    public class FailureBody
    {
        public Guid VictimId { get; set; }
        public string FailureReason { get; set; }
    }
}
