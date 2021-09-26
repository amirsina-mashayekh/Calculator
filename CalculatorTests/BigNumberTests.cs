using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calculator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator.Tests
{
    [TestClass()]
    public class BigNumberTests
    {
        [TestMethod()]
        public void ValidationTest()
        {
            List<string> ok = new List<string>
            {
                "",
                "123654789",
                "-123654789",
                "123654789.987456321",
                "-123654789.987456321",
                "-1234567891123456789212345678931234567894.9876543211987654321298765432139876543214"
            };
            List<string> bad = new List<string>
            {
                null,
                "    ",
                "+123654789",
                "a987654321",
                "9876srwae54321",
                "huyftrdt",
                "^&RY^*576&%R&",
                "+123654789.987456321",
                "1gty23654789.987456321"
            };

            foreach (string s in ok)
            {
                try
                {
                    _ = new BigNumber(s);
                }
                catch (Exception)
                {
                    Assert.Fail();
                }
            }

            foreach (string s in bad)
            {
                try
                {
                    _ = new BigNumber(s);
                    Assert.Fail();
                }
                catch (Exception) { }
            }
        }
    }
}