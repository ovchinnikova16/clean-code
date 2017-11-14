using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Markdown
{
    [TestFixture]
    class Md_ShouldRender
    {
        [TestCase("abc", ExpectedResult = "abc")]
        [TestCase("zxcv zxcv123 1", ExpectedResult = "zxcv zxcv123 1")]
        [TestCase("zxcv_b", ExpectedResult = "zxcv_b")]
        public string ParseStringWithoutTags(string markdown)
        {
            return Md.ConvertToHtml(markdown);
        }

        [TestCase("_abc_", ExpectedResult = "<em>abc</em>")]
        [TestCase("_abc_d_efg_", ExpectedResult = "<em>abc</em>d<em>efg</em>")]
        [TestCase("_abc_d_efg", ExpectedResult = "<em>abc</em>d_efg")]
        [TestCase(@"_abc\_cde_", ExpectedResult = "<em>abc_cde</em>")]
        [TestCase("abc_1_", ExpectedResult = "abc_1_")]
        [TestCase("abc_1a_", ExpectedResult = "abc_1a_")]
        public string ParseStringWithEmTags(string markdown)
        {
            return Md.ConvertToHtml(markdown);
        }

        [TestCase("__abc__", ExpectedResult = "<strong>abc</strong>")]
        [TestCase("__abc__d__", ExpectedResult = "<strong>abc</strong>d__")]
        [TestCase(@"__abc\_\_d__", ExpectedResult = "<strong>abc__d</strong>")]
        [TestCase(@"__abc____abc__", ExpectedResult = "<strong>abc</strong><strong>abc</strong>")]

        public string ParseStrongTokens(string markdown)
        {
            return Md.ConvertToHtml(markdown);
        }

        [TestCase("_a __b__ c_", ExpectedResult = "<em>a __b__ c</em>")]
        [TestCase("_a__b__c_", ExpectedResult = "<em>a__b__c</em>")]
        [TestCase("_ab __cd__ __cd__ ab_", ExpectedResult = "<em>ab __cd__ __cd__ ab</em>")]
        [TestCase("_ab __cd__ ab", ExpectedResult = "_ab <strong>cd</strong> ab")]
        public string ParseStrongInEmTags(string markdown)
        {
            return Md.ConvertToHtml(markdown);
        }

        [TestCase("__abc _def_ abc__", ExpectedResult = "<strong>abc <em>def</em> abc</strong>")]
        [TestCase("__abc_def_abc__", ExpectedResult = "<strong>abc<em>def</em>abc</strong>")]
        [TestCase("___abc_", ExpectedResult = "__<em>abc</em>")]
        public string ParseEmInStrongTags(string markdown)
        {
            return Md.ConvertToHtml(markdown);
        }
    }
}
