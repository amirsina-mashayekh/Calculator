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
        public void Equal_NotEqualTest()
        {
            string[,] eq =
            {
                { "12340", "012340" },
                { "-012340", "-12340" },
                { "012340.056780", "00012340.0567800000" },
                { "012340.056780", "00012340.0567800000" },
                { "1", "1.000000" },
                { "0", "-0.0000000000" }
            };
            for (int i = 0; i < eq.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(eq[i, 0]);
                BigNumber n1 = new BigNumber(eq[i, 1]);
                Assert.IsTrue(n == n1);
                Assert.IsTrue(n.GetHashCode() == n1.GetHashCode());
                Assert.IsTrue(n >= n1);
                Assert.IsTrue(n <= n1);
            }

            BigNumber num = new BigNumber("123");
            BigNumber num1 = num;
            num1.Value = "12";
            Assert.IsTrue(num == num1);

            string[,] neq =
            {
                { "1234", "012340" },
                { "012340", "-12340" }
            };
            for (int i = 0; i < neq.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(neq[i, 0]);
                BigNumber n1 = new BigNumber(neq[i, 1]);
                Assert.IsFalse(n == n1);
                Assert.IsFalse(n.GetHashCode() == n1.GetHashCode());
            }
        }

        [TestMethod()]
        public void CompareTest()
        {
            string[,] t1 =
            {
                { "0", "1" },
                { "-1", "0" },
                { "12340", "012341" },
                { "-012340", "-12341" },
                { "-012341", "12340" },
                { "1.0000001", "1.0021" },
                { "-1.0021", "-1.0000001" },
                { "-1.0021", "1.0000001" }
            };
            for (int i = 0; i < t1.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(t1[i, 0]);
                BigNumber n1 = new BigNumber(t1[i, 1]);
                Assert.IsTrue(n < n1);
                Assert.IsFalse(n > n1);
                Assert.IsTrue(n <= n1);
                Assert.IsFalse(n >= n1);
                Assert.IsTrue(n1 > n);
                Assert.IsFalse(n1 < n);
                Assert.IsTrue(n1 >= n);
                Assert.IsFalse(n1 <= n);
            }
        }

        [TestMethod()]
        public void SumTest()
        {
            string[,] tests =
            {
                { "0", "0", "0" },
                { "-0", "0", "0" },
                { "0", "1", "1" },
                { "0", "-1", "-1" },
                { "1", "0", "1" },
                { "-1", "0", "-1" },
                { "2", "-1", "1" },
                { "-2", "1", "-1" },
                { "999.001", "000.999", "1000.000" },
                { "-606.0606", "-404.0404", "-1010.1010" },
                { "999.999", "-111.111", "888.888" },
                { "-999.9990", "0111.111", "-888.888" },
                { "10101.01010", "-0909.0909", "0009191.9192" },
                { "-10101.0101", "909.0909", "-9191.919200" }
            };

            for (int i = 0; i < tests.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(tests[i, 0]);
                BigNumber n1 = new BigNumber(tests[i, 1]);
                BigNumber res = new BigNumber(tests[i, 2]);
                BigNumber sum = n + n1;
                Assert.IsTrue(sum == res);
            }

            BigNumber num0 = new BigNumber("-2");
            num0++;
            Assert.IsTrue(num0++ == new BigNumber("-1"));
            Assert.IsTrue(++num0 == new BigNumber("1"));
        }

        [TestMethod()]
        public void MinusTest()
        {
            string[,] tests =
            {
                { "0", "0", "0" },
                { "-0", "0", "0" },
                { "0", "1", "-1" },
                { "0", "-1", "1" },
                { "1", "0", "1" },
                { "-1", "0", "-1" },
                { "1", "2", "-1" },
                { "-1", "-2", "1" },
                { "999.999", "111.111", "888.888" },
                { "-999.9990", "-0111.111", "-888.888" },
                { "10101.01010", "0909.0909", "0009191.9192" },
                { "-10101.0101", "-909.0909", "-9191.919200" },
                { "999.001", "-000.999", "1000.000" },
                { "-606.0606", "404.0404", "-1010.1010" }
            };

            for (int i = 0; i < tests.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(tests[i, 0]);
                BigNumber n1 = new BigNumber(tests[i, 1]);
                BigNumber res = new BigNumber(tests[i, 2]);
                BigNumber sum = n - n1;
                Assert.IsTrue(sum == res);
            }

            BigNumber num0 = new BigNumber("2");
            num0--;
            Assert.IsTrue(num0-- == new BigNumber("1"));
            Assert.IsTrue(--num0 == new BigNumber("-1"));
        }
    }
}