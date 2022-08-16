using CSharpToBlockly.Functions;
using CSharpToBlockly.Variables;
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
        ///  - AddLogging for NLog 
        /// </summary>
        public static IServiceCollection AddSharpToBlockly(this IServiceCollection services)
        {
            services.AddSingleton<SharpParse>();
            services.AddSingleton<ISharpMethodDeclaration, SharpMethodDeclaration>();
            services.AddSingleton<ISharpExpressionStatement, SharpExpressionStatement>();
            services.AddSingleton<ISharpExpressionSyntax, SharpExpressionSyntax>();
            services.AddSingleton<ISharpLocalDeclarationStatement, SharpLocalDeclarationStatement>();
            services.AddSingleton<ISharpVariableInitializer, SharpVariableInitializer>();
            return services;
        }
    }
}
