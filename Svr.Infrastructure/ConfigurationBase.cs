using System;
using Microsoft.Extensions.Configuration;

namespace Svr.Infrastructure
{
    public abstract class ConfigurationBase
    {
        protected IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
        }

        protected void RaiseValueNotFoundException(string configurationKey)
        {
            throw new Exception($"не удалось найти ключ appsettings ({configurationKey}).");
        }
    }
}
