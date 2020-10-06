using System;
using System.Collections.Generic;
using System.Text;

namespace ScramNet.Ally.AssignVictimClient.Models
{
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
    }
}
