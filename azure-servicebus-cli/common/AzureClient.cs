﻿using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using System.Threading.Tasks;

namespace Common
{

    public class AzureClient
    {
        public AzureCredentials AzureCredentials { get; set; }
        public IAzure AzureInstance { get; set; }

    }
}
