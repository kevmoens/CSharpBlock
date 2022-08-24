using Microsoft.Extensions.DependencyInjection;
using TestCSharpBlock.Configuration;

namespace TestCSharpBlock
{
    public class Variables
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void VariableDeclareInitializer()
        {
            var code = @"dynamic n = ""1"";";
            var expected = @"<xml>
  <variables>
    <variable>n</variable>
  </variables>
  <block type=""variables_set"">
    <field name=""VAR"">n</field>
    <value name=""VALUE"">
      <block type=""text"">
        <field name=""TEXT"">1</field>
      </block>
    </value>
  </block>
</xml>";
            var parser = Bootstrapper.ServiceProvider.GetRequiredService<SharpParse>();
            var actual = parser.Parse(code).ToString();
            Assert.AreEqual(expected, actual);
        }



        [Test]
        public void VariableDeclareInitializerSetVariableText()
        {
            var code = @"dynamic n = ""1""; n = ""2"";";
            var expected = @"<xml>
  <variables>
    <variable>n</variable>
  </variables>
  <block type=""variables_set"">
    <field name=""VAR"">n</field>
    <value name=""VALUE"">
      <block type=""text"">
        <field name=""TEXT"">1</field>
      </block>
    </value>
    <next>
      <block type=""variables_set"">
        <field name=""VAR"">n</field>
        <value name=""VALUE"">
          <block type=""text"">
            <field name=""TEXT"">2</field>
          </block>
        </value>
      </block>
    </next>
  </block>
</xml>";
            var parser = Bootstrapper.ServiceProvider.GetRequiredService<SharpParse>();
            var actual = parser.Parse(code).ToString();
            Assert.AreEqual(expected, actual);
        }



        [Test]
        public void VariableDeclareInitializerSetVariableNumeric()
        {
            var code = @"dynamic n = 1; n = 2;";
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
          <block type=""math_number"">
            <field name=""NUM"">2</field>
          </block>
        </value>
      </block>
    </next>
  </block>
</xml>";
            var parser = Bootstrapper.ServiceProvider.GetRequiredService<SharpParse>();
            var actual = parser.Parse(code).ToString();
            Assert.AreEqual(expected, actual);
        }


    }
}