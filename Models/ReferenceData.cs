using System.Collections.Generic;

namespace SupplyChain.ClientApplication.Models
{
    public class ReferenceDataMeta
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ReferenceDataMetaLinks> _links { get; set; }
    }

    public class ReferenceDataMetaLinks
    {
        public string Href { get; set; }
        public string Rel { get; set; }
        public string Method { get; set; }
    }
}