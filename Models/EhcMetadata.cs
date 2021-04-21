using System.Collections.Generic;

namespace SupplyChain.ClientApplication.Models
{
    public class EhcMetadata
    {
        public List<Datum> Data { get; set; }
        public int Records { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
    }

    public class Datum
    {
        public List<Link> _links { get; set; }
        public string EhcName { get; set; }
        public string Title { get; set; }
        public bool IsAvailableViaApi { get; set; }
    }

    public class Link
    {
        public string Href { get; set; }
        public string Rel { get; set; }
        public string Method { get; set; }
    }
}