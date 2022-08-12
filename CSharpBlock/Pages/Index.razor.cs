using System;
using System.Diagnostics.Metrics;
using IronBlock;
using IronBlock.Blocks;
using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;
using Microsoft.JSInterop;
using CSharpToBlockly;

namespace CSharpBlock.Pages
{
	public partial class Index : ComponentBase
	{
        string code = "{dynamic n;n=1;for(int intcount=0;count<4;count++){n=(n*2);}}";
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
        public async void OnCodeToBlocks()
        {
            var result = @"<xml xmlns=""https://developers.google.com/blockly/xml"">
  <variables>
    <variable id=""y_#IRpM1S9FHYY3S0k?U"">n</variable>
  </variables>
  <block type=""variables_set"" id=""^iTy[t5D]UJ0$VFR?T}u"" x=""10"" y=""10"">
    <field name=""VAR"" id=""y_#IRpM1S9FHYY3S0k?U"">n</field>
    <value name=""VALUE"">
      <block type=""math_arithmetic"" id=""Un+toI+8b0jPKV9pe=X!"">
        <field name=""OP"">ADD</field>
        <value name=""A"">
          <block type=""variables_get"" id=""PHI${/;/:#TPt,gbT}z#"">
            <field name=""VAR"" id=""y_#IRpM1S9FHYY3S0k?U"">n</field>
          </block>
        </value>
        <value name=""B"">
          <shadow type=""math_number"" id=""8`.nB;%qLfF,j~1~.yM_"">
            <field name=""NUM"">1</field>
          </shadow>
        </value>
      </block>
    </value>
  </block>
</xml>";

            //var result = SharpParse.Parse(code ?? "");
            await module.InvokeVoidAsync("setBlocks", new object?[] { result });
            StateHasChanged();
            //SyntaxTree codeTree = CSharpSyntaxTree.ParseText(code ?? "");
            //var node = codeTree.GetRoot();
            //foreach (var child in node.ChildNodes())
            //{
            //    System.Console.WriteLine($"child type name {child.GetType().Name}");
            //    System.Console.WriteLine($"child kind {child.Kind().ToString()}");

            //    foreach (var grandchild in child.ChildNodes())
            //    {
            //        System.Console.WriteLine($"grandchild kind {grandchild.GetType().Name}");
            //        foreach (var greatgrandchild in grandchild.ChildNodes())
            //        {
            //            System.Console.WriteLine($"greatgrandchild kind {greatgrandchild.GetType().Name}");
            //            System.Console.WriteLine($"greatgrandchild kind {greatgrandchild.Kind().ToString()}");
            //            if (greatgrandchild is LocalDeclarationStatementSyntax )
            //            {
            //                var greatgrandchildLDSS = greatgrandchild as LocalDeclarationStatementSyntax;
            //                System.Console.WriteLine($"greatgrandchild VariableDeclartor Type {greatgrandchildLDSS.Declaration.Type.ToFullString()}");

            //                foreach (var variable in greatgrandchildLDSS.Declaration.Variables)
            //                {
            //                    System.Console.WriteLine($"greatgrandchildLDSS variable Name {variable.ToFullString()}");
            //                }

            //            }

            //            foreach (var greatgranchildToken in greatgrandchild.ChildTokens())
            //            {
            //                System.Console.WriteLine($"greatgranchild token text {greatgranchildToken.Text}");
            //               System.Console.WriteLine($"greatgranchild token value text {greatgranchildToken.ValueText}");
            //               System.Console.WriteLine($"greatgranchild token Kind {greatgranchildToken.Kind().ToString()}");
            //            }
            //            foreach (var g2grandchild in greatgrandchild.ChildNodes())
            //            {
            //                System.Console.WriteLine($"g2grandchild kind {g2grandchild.GetType().Name}");
            //                System.Console.WriteLine($"g2grandchild kind {g2grandchild.Kind().ToString()}");
            //                System.Console.WriteLine(g2grandchild.GetType().Name);
            //                if (g2grandchild is VariableDeclaratorSyntax)
            //                {
            //                    var g2grandchildVDS = g2grandchild as VariableDeclaratorSyntax;
            //                    System.Console.WriteLine($"g2grandchild VariableDeclartor Value {g2grandchildVDS.Identifier.Value}");
            //                    System.Console.WriteLine($"g2grandchild VariableDeclartor Value {g2grandchildVDS.Identifier.ValueText}");

            //                }
            //                foreach (var g2granchildToken in g2grandchild.ChildTokens())
            //                {
            //                    System.Console.WriteLine($"g2granchildToken text {g2granchildToken.Text}");
            //                    System.Console.WriteLine($"g2granchildToken value text {g2granchildToken.ValueText}");
            //                    System.Console.WriteLine($"g2granchildToken Kind {g2granchildToken.Kind().ToString()}");
            //                }
            //            }
            //        }
            //    }
            //}
            //System.Diagnostics.Debug.Print("HERE 2");
        }
        public void OnTestMethod()
        {
            dynamic n; 
            n = 1; 
            for (int count = 0; count < 4; count++) 
            { 
                n = (n * 2); 
            }
        }
    }
}

