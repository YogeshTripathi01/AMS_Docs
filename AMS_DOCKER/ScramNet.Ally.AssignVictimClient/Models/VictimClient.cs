using System;
using System.Collections.Generic;
using System.Text;

namespace ScramNet.Ally.AssignVictimClient.Models
{
    public class VictimClient
    {
        public Guid VictimId { get; set; }
        public Client Client { get; set; }

        public VictimClient(ReceivedClientData clientData)
        {
            VictimId = clientData.VictimId;
            Client = new Client(clientData);
        }
    }
}
