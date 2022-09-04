using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TestCSharpBlock.Configuration;

namespace TestCSharpBlock
{
    internal class Math
    {


        [Test]
        public void SetNumber()
        {
            var code = @"n = 1;";
            var expected = @"<xml>
  <block type=""variables_set"">
    <field name=""VAR"">n</field>
    <value name=""VALUE"">
      <block type=""math_number"">
        <field name=""NUM"">1</field>
      </block>
    </value>
  </block>
</xml>";
            var parser = Bootstrapper.ServiceProvider.GetRequiredService<SharpParse>();
            var actual = parser.Parse(code).ToString();
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void DeclareIncrementNumber()
        {
            var code = @"n = n + 1;";
            var expected = @"<xml>
  <variables>
    <variable>n</variable>
  </variables>
  <block type=""variables_set"">
    <field name=""VAR"">n</field>
    <value name=""VALUE"">
      <block type=""math_arithmetic"">
        <field name=""OP"">ADD</field>
        <value name=""A"">
          <block type=""variables_get"">
            <field name=""VAR"">n</field>
          </block>
        </value>
        <value name=""B"">
          <block type=""math_number"">
            <field name=""NUM"">1</field>
          </block>
        </value>
      </block>
    </value>
  </block>
</xml>";
            var parser = Bootstrapper.ServiceProvider.GetRequiredService<SharpParse>();
            var actual = parser.Parse(code).ToString();

            var doc = XDocument.Parse(expected);
            doc.Descendants().Where(x => x.Name == "variables").Remove();
            Assert.IsTrue(expected == actual || doc.ToString() == actual);
        }


        [Test]
        public void DeclareInitializerIncrementNumber()
        {
            var code = @"dynamic n = 1; n = n + 1;";
            var expected = @"<xml>
  <variables>
    <variable>n</variable>
  </variables>
  <block type=""variables_set"">
    <field name=""VAR"">n</field>
    <value name=""VALUE"">
      <block type=""math_number"">
        <field name=""NUM"">1</field>
      </block>
    </value>
    <next>
      <block type=""variables_set"">
        <field name=""VAR"">n</field>
        <value name=""VALUE"">
          <block type=""math_arithmetic"">
            <field name=""OP"">ADD</field>
            <value name=""A"">
              <block type=""variables_get"">
                <field name=""VAR"">n</field>
              </block>
            </value>
            <value name=""B"">
              <block type=""math_number"">
                <field name=""NUM"">1</field>
              </block>
            </value>
          </block>
        </value>
      </block>
    </next>
  </block>
</xml>";
            var parser = Bootstrapper.ServiceProvider.GetRequiredService<SharpParse>();
            var actual = parser.Parse(code).ToString();

            var doc = XDocument.Parse(expected);
            doc.Descendants().Where(x => x.Name == "variables").Remove();
            Assert.IsTrue(expected == actual || doc.ToString() == actual);
        }


        [Test]
        public void DeclareInitializerRemainder()
        {
            var code = @"int n = 3; n = 4 % n;";
            var expected = @"<xml>
  <variables>
    <variable>n</variable>
  </variables>
  <block type=""variables_set"">
    <field name=""VAR"">n</field>
    <value name=""VALUE"">
      <block type=""math_number"">
        <field name=""NUM"">3</field>
      </block>
    </value>
    <next>
      <block type=""variables_set"">
        <field name=""VAR"">n</field>
        <value name=""VALUE"">
          <block type=""math_modulo"">
            <value name=""DIVIDEND"">
              <shadow type=""math_number"">
                <field name=""NUM"">4</field>
              </shadow>
            </value>
            <value name=""DIVISOR"">
              <shadow type=""math_number"">
                <field name=""NUM"">64</field>
              </shadow>
              <block type=""variables_get"">
                <field name=""VAR"">n</field>
              </block>
            </value>
          </block>
        </value>
      </block>
    </next>
  </block>
</xml>";
            var parser = Bootstrapper.ServiceProvider.GetRequiredService<SharpParse>();
            var actual = parser.Parse(code).ToString();

            var doc = XDocument.Parse(expected);
            doc.Descendants().Where(x => x.Name == "variables").Remove();
            Assert.IsTrue(expected == actual || doc.ToString() == actual);
        }
    }
}
