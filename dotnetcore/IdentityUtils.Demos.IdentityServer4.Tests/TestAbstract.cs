using IdentityUtils.Demos.IdentityServer4.Tests.Setup;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace IdentityUtils.Demos.IdentityServer4.Tests
{
    public class TestAbstract<TStartup>
        where TStartup : class
    {
        protected static IWebHost webHost;
        protected IServiceProvider serviceProvider;
        private static readonly object lockObject = new object();
        protected string HostAddress = "https://localhost:5010";

        public TestAbstract()
        {
            serviceProvider = ServicesFactory.ServiceProvider();
            StartServer();
        }

        private void StartServer()
        {
            //Wait till server is started, otherwise multiple webhosts could be started,
            //all on same port which would cause error
            lock (lockObject)
                if (webHost == null)
                {
                    //Host and database name are stored as environment variables which IS4 app reads
                    //If needed, this can be changed
                    string host = HostAddress;
                    var databaseName = $"IntegrationTestsDatabase.db";

                    //Have clean database each run
                    File.WriteAllText(databaseName, "");

                    Environment.SetEnvironmentVariable("Is4Host", host);
                    Environment.SetEnvironmentVariable("DatabaseName", databaseName);

                    webHost = WebHost.CreateDefaultBuilder(null)
                        .UseStartup<TStartup>()
                        .UseEnvironment("Development")
                        .UseKestrel()
                        .UseUrls(host)
                        .Build();

                    webHost.Start();
                }
        }
    }
}