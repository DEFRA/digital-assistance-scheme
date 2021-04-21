using System.Collections.Generic;
using System.Threading.Tasks;
using SupplyChain.ClientApplication.Models;

namespace SupplyChain.ClientApplication.Service.Interface
{
    public interface IReferenceDataService
    {
        Task<List<ReferenceDataMeta>> GetEhcMetadata();
        Task<dynamic> GetRefDataWithDynamicEndpoint(string endpoint);
    }
}
