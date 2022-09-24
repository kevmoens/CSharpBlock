using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCSharpBlock.Configuration;

namespace TestCSharpBlock
{
    internal class Functions
    {

        [Test]
        public void SumMetodWithParms()
        {
            var code = @"
using System;

namespace Ellabit
{

    public class Challenge
    {
        public int Sum(int a, int b)
        {

        }
    }
}
";
            var expected = @"<xml>
  <block type=""procedures_defreturn"">
    <mutation>
      <arg name=""a"" />
      <arg name=""b"" />
    </mutation>
    <field name=""NAME"">Sum</field>
    <statement name=""STACK"" />
    <next></next>
  </block>
</xml>";
            var parser = Bootstrapper.ServiceProvider.GetRequiredService<SharpParse>();
            var actual = parser.Parse(code).ToString();
            Assert.AreEqual(expected, actual);

        }



        [Test]
        public void SumMetodNoParms()
        {
            var code = @"
using System;

namespace Ellabit
{

    public class Challenge
    {
        public int Sum()
        {

        }
    }
}
";
            var expected = @"<xml>
  <block type=""procedures_defreturn"">
    <mutation></mutation>
    <field name=""NAME"">Sum</field>
    <statement name=""STACK"" />
    <next></next>
  </block>
</xml>";
            var parser = Bootstrapper.ServiceProvider.GetRequiredService<SharpParse>();
            var actual = parser.Parse(code).ToString();
            Assert.AreEqual(expected, actual);

        }
    }
}
