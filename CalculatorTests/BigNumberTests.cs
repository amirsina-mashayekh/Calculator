using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace Calculator.Tests
{
    [TestClass()]
    public class BigNumberTests
    {
        [TestMethod()]
        public void ValidationTest()
        {
            string[] ok =
            {
                "123654789",
                "-123654789",
                "123654789.987456321",
                "-123654789.987456321",
                "-1234567891123456789212345678931234567894.9876543211987654321298765432139876543214",
                "012030.030210",
                "-030210.012030",
                "-00000.000",
                "0000.000"
            };
            string[] bad =
            {
                null,
                "",
                "    ",
                "+123654789",
                "a987654321",
                "9876srwae54321",
                "huyftrdt",
                "^&RY^*576&%R&",
                "+123654789.987456321",
                "1gty23654789.987456321",
                "1545864.5132458.546897",
                "2454..143354",
                ".16587966"
            };

            foreach (string s in ok)
            {
                try
                {
                    BigNumber n = new BigNumber(s);
                    Debug.Print(n.ToString());
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

        [TestMethod()]
        public void NegationTest()
        {
            string[,] tests =
            {
                { "123456789", "-123456789" },
                { "-123456789", "123456789" },
                { "123456789.987654321", "-123456789.987654321" },
                { "-123456789.987654321", "123456789.987654321" },
                { "012340567890.098765043210", "-12340567890.09876504321" },
                { "-012340567890.098765043210", "12340567890.09876504321" }
            };

            for (int i = 0; i < tests.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(tests[i, 0]);
                Assert.AreEqual((-n).Value, tests[i, 1]);
            }
        }

        [TestMethod()]
        public void EqualNotEqualTest()
        {
            string[,] eq =
            {
                { "12340", "012340"},
                { "-012340", "-12340"},
                { "012340.056780", "00012340.0567800000"},
                { "012340.056780", "00012340.0567800000"},
                { "1", "1.000000"},
                { "0", "-0.0000000000"}
            };
            string[,] neq =
            {
                { "1234", "012340"},
                { "012340", "-12340"}
            };

            for (int i = 0; i < eq.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(eq[i, 0]);
                BigNumber n1 = new BigNumber(eq[i, 1]);
                Assert.IsTrue(n == n1);
                Assert.IsTrue(n.GetHashCode() == n1.GetHashCode());
            }

            for (int i = 0; i < neq.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(neq[i, 0]);
                BigNumber n1 = new BigNumber(neq[i, 1]);
                Assert.IsFalse(n == n1);
                Assert.IsFalse(n.GetHashCode() == n1.GetHashCode());
            }

            BigNumber num = new BigNumber("123");
            BigNumber num1 = num;
            num1.Value = "12";
            Assert.IsTrue(num == num1);
        }

        [TestMethod()]
        public void SumTest()
        {
            string[,] tests =
            {
                { "0", "0", "0" },
                { "11111111111111111111", "11111111111111111111", "22222222222222222222" },
                { "1111111111.1111111111", "1111111111.1111111111", "2222222222.2222222222" },
                { "9999999999.9999999999", "0000000000.0000000001", "10000000000.0000000000" },
                { "0606060606.0606060606", "0404040404.0404040404", "1010101010.1010101010" },
                { "-0", "0", "0" },
                { "-11111111111111111111", "-11111111111111111111", "-22222222222222222222" },
                { "-1111111111.1111111111", "-1111111111.1111111111", "-2222222222.2222222222" },
                { "-9999999999.9999999999", "-0000000000.0000000001", "-10000000000.0000000000" },
                { "-0606060606.0606060606", "-0404040404.0404040404", "-1010101010.1010101010" },
            };

            for (int i = 0; i < tests.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(tests[i, 0]);
                BigNumber n1 = new BigNumber(tests[i, 1]);
                BigNumber res = new BigNumber(tests[i, 2]);
                BigNumber sum = n + n1;
                Assert.IsTrue(sum == res);
            }
        }
    }
}