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
    internal class Text
    {



        [Test]
        public void AppendText()
        {
            var code = @"var item = """"; item = item + ""less"";";
            var expected = @"<xml>
  <block type=""variables_set"">
    <field name=""VAR"">item</field>
    <value name=""VALUE"">
      <block type=""text"">
        <field name=""TEXT""></field>
      </block>
    </value>
    <next>
      <block type=""text_append"">
        <field name=""VAR"">item</field>
        <value name=""TEXT"">
          <block type=""text"">
            <field name=""TEXT"">less</field>
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
        public void LengthOfText()
        {
            var code = @"item = ""Kevin"".Length;";
            var expected = @"<xml>
  <variables>
    <variable>item</variable>
  </variables>
  <block type=""variables_set"">
    <field name=""VAR"">item</field>
    <value name=""VALUE"">
      <block type=""text_length"">
        <value name=""VALUE"">
          <block type=""text"">
            <field name=""TEXT"">Kevin</field>
          </block>
          <field name=""VAR"">Length</field>
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
        public void TextToUpper()
        {
            var code = @"""Kevin"".ToUpper();";
            var expected = @"<xml>
  <block type=""text_changeCase"">
    <field name=""CASE"">UPPERCASE</field>
    <value name=""TEXT"">
      <shadow type=""text"">
        <field name=""TEXT"">abc</field>
      </shadow>
      <block type=""text"">
        <field name=""TEXT"">Kevin</field>
      </block>
    </value>
  </block>
</xml>";
            var parser = Bootstrapper.ServiceProvider.GetRequiredService<SharpParse>();
            var actual = parser.Parse(code).ToString();
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void Substring1()
        {
            var code = @"text = text.Substring(1,1);";
            var expected = @"<xml>
  <variables>
    <variable>text</variable>
    <variable>i</variable>
  </variables>
  <block type=""variables_set"">
    <field name=""VAR"">text</field>
    <value name=""VALUE"">
      <block type=""text_charAt"">
        <mutation at=""true""></mutation>
        <field name=""WHERE"">FROM_START</field>
        <value name=""VALUE"">
          <block type=""variables_get"">
            <field name=""VAR"">text</field>
          </block>
        </value>
        <value name=""AT"">
          <block type=""variables_get"">
            <field name=""VAR"">i</field>
          </block>
        </value>
      </block>
    </value>
  </block>
</xml>";
            var parser = Bootstrapper.ServiceProvider.GetRequiredService<SharpParse>();
            var actual = parser.Parse(code).ToString();
            Assert.AreEqual(expected, actual);
        }

    }
}
