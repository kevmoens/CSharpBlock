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
            services.AddSingleton<ParsePersistence>();
            services.AddSingleton<ISharpExpressionStatement, SharpExpressionStatement>();
            services.AddSingleton<ISharpExpressionSyntax, SharpExpressionSyntax>();
            services.AddSingleton<ISharpEqualValueClauseSyntax, SharpEqualValueClauseSyntax>();
            services.AddSingleton<ISharpIdentifierNameSyntax, SharpIdentifierNameSyntax>();
            services.AddSingleton<ISharpFieldDeclarationSyntax, SharpFieldDeclarationSyntax>();
            services.AddSingleton<ISharpLocalDeclarationStatement, SharpLocalDeclarationStatement>();
            services.AddSingleton<ISharpMethodDeclaration, SharpMethodDeclaration>();
            services.AddSingleton<ISharpVariableInitializer, SharpVariableInitializer>();
            return services;
        }
    }
}
