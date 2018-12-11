using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Svr.Infrastructure
{
    public class DatabaseConfiguration : ConfigurationBase
    {
        private readonly string DataConnectionKey = "svrDataConnection";
        private readonly string AuthConnectionKey = "svrIdentityConnection";

        public string GetDataConnectionString() => GetConfiguration().GetConnectionString(DataConnectionKey);

        public string GetAuthConnectionString()=> GetConfiguration().GetConnectionString(AuthConnectionKey);
    }
}
