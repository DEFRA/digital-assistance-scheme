using System.Collections.Generic;

namespace SupplyChain.ClientApplication.Models
{
    public class EhcMetadata
    {
        public List<Datum> data { get; set; }
        public int records { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int totalRecords { get; set; }
        public int totalPages { get; set; }
    }

    public class Datum
    {
        public List<Link> _links { get; set; }
        public string ehcName { get; set; }
        public string title { get; set; }
        public bool isAvailableViaApi { get; set; }
    }

    public class Link
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; }
    }
}