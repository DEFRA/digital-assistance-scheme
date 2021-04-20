using System;
using System.Collections.Generic;

namespace SupplyChain.ClientApplication.Models
{
    public class EhcApplicationResponse
    {
        public Guid RequestId { get; set; }
        public string CustomerReference { get; set; }
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<EhcApplicationResponseLinks> _links { get; set; }
    }

    public class EhcApplicationResponseLinks
    {
        public string Href { get; set; }
        public string Rel { get; set; }
        public string Method { get; set; }
    }
}