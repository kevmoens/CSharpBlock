using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCSharpBlock
{
    internal class Math
    {

        [Test]
        public void DeclareInitializerIncrementNumber()
        {
            var code = @"dynamic n; n = n + 1;";
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
          <shadow type=""math_number"">
            <field name=""NUM"">1</field>
          </shadow>
          <block type=""variables_get"">
            <field name=""VAR"">n</field>
          </block>
        </value>
        <value name=""B"">
          <shadow type=""math_number"">
            <field name=""NUM"">1</field>
          </shadow>
        </value>
      </block>
    </value>
  </block>
</xml>";
            var actual = SharpParse.Parse(code).ToString();
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void DeclareInitializerRemainder()
        {
            var code = @"int n = 4; n = n % 3;";
            var expected = @"<xml>
  <variables>
    <variable>n</variable>
  </variables>
  <block type=""variables_set"">
    <field name=""VAR"">n</field>
    <value name=""VALUE"">
      <block type=""math_number"">
        <field name=""NUM"">4</field>
      </block>
    </value>
    <next>
      <block type=""variables_set"">
        <field name=""VAR"">n</field>
        <value name=""VALUE"">
          <block type=""math_modulo"">
            <value name=""DIVIDEND"">
              <shadow type=""math_number"">
                <field name=""NUM"">64</field>
              </shadow>
              <block type=""variables_get"">
                <field name=""VAR"">n</field>
              </block>
            </value>
            <value name=""DIVISOR"">
              <shadow type=""math_number"">
                <field name=""NUM"">3</field>
              </shadow>
            </value>
          </block>
        </value>
      </block>
    </next>
  </block>
</xml>";
            var actual = SharpParse.Parse(code).ToString();
            Assert.AreEqual(expected, actual);
        }
    }
}
