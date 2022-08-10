using System;
using IronBlock;
using IronBlock.Blocks;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using Microsoft.JSInterop;

namespace CSharpBlock.Pages
{
	public partial class Index : ComponentBase
	{

        private IJSObjectReference? module;
        internal class CustomPrintBlock : IBlock
        {
            public List<string> Text { get; set; } = new List<string>();

            public override object Evaluate(Context context)
            {
                Text.Add((this.Values.FirstOrDefault(x => x.Name == "TEXT")?.Evaluate(context) ?? "").ToString());
                return base.Evaluate(context);
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await JS.InvokeAsync<IJSObjectReference>("import",
                    "./javascript/blockly_ui_interop.js");
                await module.InvokeVoidAsync("initialize", new object?[] { });
            }
            await base.OnAfterRenderAsync(firstRender);
        }
        [JSInvokable]
        public static string Evaluate(string xml)
        {
            var printBlock = new CustomPrintBlock();

            var parser =
            new IronBlock.Parser()
              .AddStandardBlocks()
              .AddBlock("text_print", printBlock)
              .Parse(xml);
            var syntax = parser.Generate();
            

            parser.Evaluate();

            return syntax.ToFullString() + '\n' + string.Join('\n', printBlock.Text);

        }
        public async Task OnRun()
        {
            if (module == null)
            {
                return;
            }
            await module.InvokeVoidAsync("evalProgram");
        }
    }
}

