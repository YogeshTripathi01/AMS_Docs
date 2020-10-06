using System;
using System.Collections.Generic;
using System.Text;

namespace ScramNet.Ally.AssignVictimClient.Models
{
    public class ReceivedClientData
    {
        public Guid VictimId { get; set; }
        public Guid ClientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public PrimaryAgent PrimaryAgent { get; set; }
        public DateTime? StartOfService { get; set; }
        public DateTime? EndOfService { get; set; }      
        public List<ClientCourtCase> CourtCases { get; set; }
    }
}
