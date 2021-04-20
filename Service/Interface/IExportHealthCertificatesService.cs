﻿using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace SupplyChain.ClientApplication.Service.Interface
{
    public interface IExportHealthCertificatesService
    {
        Task<dynamic> GetEhcMetadata();

        Task<dynamic> Create(JObject requestContentParsed);

        Task<dynamic> CheckStatus(Guid application);
    }
}