using System;
using System.Collections.Generic;
using System.Text;

namespace ScramNet.Ally.AssignVictimClient.Models
{
    public class Client
    {  
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
        public Client(ReceivedClientData clientData)
        {
            ClientId = clientData.ClientId;
            FirstName = clientData.FirstName;
            LastName = clientData.LastName;
            MiddleName = clientData.MiddleName;
            AccountId = clientData.AccountId;
            AccountName = clientData.AccountName;
            PrimaryAgent = clientData.PrimaryAgent;
            StartOfService = clientData.StartOfService;
            EndOfService = clientData.EndOfService;
            CourtCases = clientData.CourtCases;
        }
    }
}
