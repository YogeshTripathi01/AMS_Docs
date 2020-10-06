using System;
using System.Collections.Generic;
using System.Text;

namespace ScramNet.Ally.AssignVictimClient.Models
{
    public class PrimaryAgent
    {
        public Guid ApplicationUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string RoleType { get; set; }
        public string PhoneNumber { get; set; }
        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
    }
}
