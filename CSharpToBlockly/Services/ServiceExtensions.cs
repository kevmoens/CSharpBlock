using CSharpToBlockly.Functions;
using CSharpToBlockly.Variables;
using Microsoft.Extensions.DependencyInjection;
using Neleus.DependencyInjection.Extensions;
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

            services.AddSingleton<SharpExpressionStatement>();
            services.AddSingleton<SharpExpressionSyntax>();
            services.AddSingleton<SharpIdentifierNameSyntax>();
            services.AddSingleton<SharpClassDeclaration>();
            services.AddSingleton<SharpEqualValueClauseSyntax>();
            services.AddSingleton<SharpFieldDeclarationSyntax>();
            services.AddSingleton<SharpLocalDeclarationStatement>();
            services.AddSingleton<SharpVariableDeclaratorSyntax>();
            services.AddSingleton<SharpMethodDeclaration>();
            services.AddSingleton<SharpVariableInitializer>();

            services.AddByName<ISharpSyntax>()
                .Add<SharpExpressionStatement>("SharpExpressionStatement")
                .Add<SharpExpressionSyntax>("SharpExpressionSyntax")
                .Add<SharpIdentifierNameSyntax>("SharpIdentifierNameSyntax")
                .Add<SharpClassDeclaration>("SharpClassDeclaration")
                .Add<SharpEqualValueClauseSyntax>("SharpEqualValueClauseSyntax")
                .Add<SharpFieldDeclarationSyntax>("SharpFieldDeclarationSyntax")
                .Add<SharpLocalDeclarationStatement>("SharpLocalDeclarationStatement")
                .Add<SharpVariableDeclaratorSyntax>("SharpVariableDeclaratorSyntax")
                .Add<SharpMethodDeclaration>("SharpMethodDeclaration")
                .Add<SharpVariableInitializer>("SharpVariableInitializer")
                .Build();

            return services;
        }
    }
}
