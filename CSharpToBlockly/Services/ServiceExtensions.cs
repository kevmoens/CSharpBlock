using CSharpToBlockly.Functions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpToBlockly.Services
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Use for quick setup.  
        /// implented along with this method to complete the dependency injection
        ///  - ISCApplication
        ///  - ISmartCardDispatcher
        ///  - IMessageBox 
        ///  - AddLogging for NLog 
        /// </summary>
        public static IServiceCollection AddSharpToBlockly(this IServiceCollection services)
        {
            services.AddSingleton<SharpParse>();
            services.AddSingleton<ISharpMethodDeclaration, SharpMethodDeclaration>();
            return services;
        }
    }
}
