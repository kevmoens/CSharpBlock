using CSharpToBlockly.Services;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCSharpBlock.Configuration
{

    class Bootstrapper
    {

        public static IServiceCollection services;

        public static IServiceProvider ServiceProvider { get; set; }

        static Bootstrapper()
        {
            var serviceProviderFactory = new DefaultServiceProviderFactory();
            services = new ServiceCollection();

            services.AddSharpToBlockly();

            //Logging
            services.AddLogging(builder =>
            {
                builder.AddNLog();
            });

            ServiceProvider = serviceProviderFactory.CreateServiceProvider(services);
        }

    }
}
