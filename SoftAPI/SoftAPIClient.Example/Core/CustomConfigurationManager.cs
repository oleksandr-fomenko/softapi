using System;
using Microsoft.Extensions.Configuration;
using SoftAPIClient.Core.Exceptions;

namespace SoftAPIClient.Example.Core
{
    public static class CustomConfigurationManager
    {
        private static IConfiguration _configuration;
        public static IConfiguration Configuration => GetConfiguration("appSettings.json");

        private static IConfiguration GetConfiguration(string fileName)
        {
            if (_configuration != null)
            {
                return _configuration;
            }
            try
            {
                var directory = AppDomain.CurrentDomain.BaseDirectory;
                _configuration = new ConfigurationBuilder().SetBasePath(directory)
                    .AddJsonFile(fileName)
                    .AddEnvironmentVariables()
                    .Build();

                return _configuration;
            }
            catch (Exception ex)
            {
                throw new InitializationException($"Can't load config file {fileName}:" + ex.Message);
            }
        }
    }
}
