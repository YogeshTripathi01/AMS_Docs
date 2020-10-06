using System;
using System.Collections.Generic;
using System.Text;

namespace ScramNet.Ally.AssignVictimClient.Models
{
    public class ClientCourtCase
    {
        /// <summary>
        /// Case Number
        /// </summary>
        public string CaseNumber { get; set; }
        /// <summary>
        /// Super Vision Type
        /// </summary>
        public string SuperVisionType { get; set; }
        /// <summary>
        /// Charge Category
        /// </summary>
        public string ChargeCategory { get; set; }
        /// <summary>
        /// Charges
        /// </summary>
        public string Charges { get; set; }
        /// <summary>
        /// Court Date
        /// </summary>
        public DateTime? CourtDate { get; set; }
        /// <summary>
        /// Judge
        /// </summary>
        public string JudgeName { get; set; }
        /// <summary>
        /// Court
        /// </summary>
        public string CourtName { get; set; }
    }
}
