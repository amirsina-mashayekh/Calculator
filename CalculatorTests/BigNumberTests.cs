using BigNumbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace BigNumbers.Tests
{
    [TestClass()]
    public class BigNumberTests
    {
        [TestMethod()]
        public void ValidationTest()
        {
            string[,] ok =
            {
                { "123654789", "123456789" },
                { "-123654789", "-123456789" },
                { "123654789.987456321", "123654789.987456321" },
                { "-123654789.987456321", "-123654789.987456321" },
                { "-1234567891123456789212345678931234567894.9876543211987654321298765432139876543214",
                    "-1234567891123456789212345678931234567894.9876543211987654321298765432139876543214" },
                { "012030.030210", "12030.03021" },
                { "+123654789", "123654789" },
                { "+123654789.987456321", "123654789.987456321" },
                { "-030210.012030", "-30210.01203" },
                { "-00000.000", "0" },
                { "0000.000", "0" },
                { ".16587966", "0.16587966" }
            };
            string[] bad =
            {
                "",
                "    ",
                "a987654321",
                "9876srwae54321",
                "huyftrdt",
                "^&RY^*576&%R&",
                "1gty23654789.987456321",
                "1545864.5132458.546897",
                "2454..143354",
                "9.5."
            };

            for (int i = 0; i < ok.GetLength(0); i++)
            {
                Assert.AreEqual(ok[i, 1], new BigNumber(ok[i, 0]).Value);
            }

            foreach (string s in bad)
            {
                _ = Assert.ThrowsException<FormatException>(() => new BigNumber(s));
            }

            _ = Assert.ThrowsException<ArgumentNullException>(() => new BigNumber(null));
            Assert.AreEqual(new BigNumber(0), new BigNumber("0"));
            Assert.AreEqual(new BigNumber(1), new BigNumber("1"));
            Assert.AreEqual(new BigNumber(1.1M), new BigNumber("1.1"));
            Assert.AreEqual(new BigNumber(-1.1M), new BigNumber("-1.1"));
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
                Debug.Print("n: " + n + "  ==  -:" + tests[i, 1]);
                Assert.AreEqual(tests[i, 1], (-n).Value);
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
                Debug.Print("n: " + n + "  ==  n1:" + n1);
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
                Debug.Print("n: " + n + "  !=  n1:" + n1);
                Assert.IsTrue(n != n1);
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
                { "-79228162514264337593543950335", "-1" },
                { "-1", "0" },
                { "12340", "012341" },
                { "-012341", "-12340" },
                { "-012341", "12340" },
                { "1.0000001", "1.0021" },
                { "-1.0021", "-1.0000001" },
                { "-1.0021", "1.0000001" }
            };
            for (int i = 0; i < t1.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(t1[i, 0]);
                BigNumber n1 = new BigNumber(t1[i, 1]);
                Debug.Print("n: " + n + "  <  n1:" + n1);
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
        public void AdditionTest()
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
                { "999.001", "000.999", "1000" },
                { "-606.0606", "-404.0404", "-1010.101" },
                { "999.999", "-111.111", "888.888" },
                { "-999.9990", "0111.111", "-888.888" },
                { "10101.01010", "-0909.0909", "9191.9192" },
                { "-10101.0101", "909.0909", "-9191.9192" }
            };

            for (int i = 0; i < tests.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(tests[i, 0]);
                BigNumber n1 = new BigNumber(tests[i, 1]);
                BigNumber sum = n + n1;
                Assert.AreEqual(tests[i, 2], sum.Value);
            }

            BigNumber num0 = new BigNumber(-2);
            num0++;
            Assert.AreEqual(new BigNumber(-1), num0++);
            Assert.AreEqual(new BigNumber(1), ++num0);
        }

        [TestMethod()]
        public void SubtractionTest()
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
                { "-1", "1", "-2" },
                { "999.999", "111.111", "888.888" },
                { "-999.9990", "-0111.111", "-888.888" },
                { "10101.01010", "0909.0909", "9191.9192" },
                { "-10101.0101", "-909.0909", "-9191.9192" },
                { "999.001", "-000.999", "1000" },
                { "-606.0606", "404.0404", "-1010.101" }
            };

            for (int i = 0; i < tests.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(tests[i, 0]);
                BigNumber n1 = new BigNumber(tests[i, 1]);
                BigNumber dif = n - n1;
                Assert.AreEqual(tests[i, 2], dif.Value);
            }

            BigNumber num0 = new BigNumber(2);
            num0--;
            Assert.AreEqual(new BigNumber(1), num0--);
            Assert.AreEqual(new BigNumber(-1), --num0);
        }

        [TestMethod()]
        public void MultiplicationTest()
        {
            string[,] tests =
            {
                { "0", "0", "0" },
                { "-0", "0", "0" },
                { "0", "1", "0" },
                { "0", "-1", "0" },
                { "1", "2", "2" },
                { "-1", "-2", "2" },
                { "1", "-2", "-2" },
                { "0.0001", "10", "0.001" },
                { "1234567890", "000090087650", "111219319975558500" },
                { "9870654.30210", "-012034.560789", "-118788989225.8268203569" }
            };

            for (int i = 0; i < tests.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(tests[i, 0]);
                BigNumber n1 = new BigNumber(tests[i, 1]);
                BigNumber mul = n * n1;
                Assert.AreEqual(tests[i, 2], mul.Value);
            }
        }

        [TestMethod()]
        public void DivisionTest()
        {
            string[,] tests =
            {
                { "4", "2", "2" },
                { "1000", "500", "2" },
                { "1024", "16", "64" },
                { "1000000000000000000000000000000", "10", "100000000000000000000000000000" },
                { "10.24", "0.000000000064", "160000000000" }
            };

            for (int i = 0; i < tests.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(tests[i, 0]);
                BigNumber n1 = new BigNumber(tests[i, 1]);
                BigNumber div = n / n1;
                Assert.AreEqual(tests[i, 2], div.Value);
            }

            _ = Assert.ThrowsException<DivideByZeroException>(() => new BigNumber(1) / new BigNumber(0));
        }

        [TestMethod()]
        public void ModulusTest()
        {
            string[,] tests =
            {
                { "25", "6", "1" },
                { "25", "-6", "-5" },
                { "-25", "6", "5" },
                { "-25", "-6", "-1" },
                { "25.7", "5.1", "0.2" },
                { "25.7", "-5.1", "-4.9" },
                { "-25.7", "5.1", "4.9" },
                { "-25.7", "-5.1", "-0.2" }
            };

            for (int i = 0; i < tests.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(tests[i, 0]);
                BigNumber n1 = new BigNumber(tests[i, 1]);
                BigNumber mod = n % n1;
                Assert.AreEqual(tests[i, 2], mod.Value);
            }

            _ = Assert.ThrowsException<DivideByZeroException>(() => new BigNumber(1) % new BigNumber(0));
        }

        [TestMethod()]
        public void ToDecimalTest()
        {
            Assert.AreEqual(0M, new BigNumber("0").ToDecimal());
            Assert.AreEqual(1M, new BigNumber("1").ToDecimal());
            Assert.AreEqual(-1M, new BigNumber("-1").ToDecimal());
            Assert.AreEqual(1.1M, new BigNumber("1.1").ToDecimal());
            Assert.AreEqual(-1.1M, new BigNumber("-1.1").ToDecimal());
            Assert.AreEqual(decimal.MaxValue, new BigNumber(decimal.MaxValue).ToDecimal());
            Assert.AreEqual(decimal.MinValue, new BigNumber(decimal.MinValue).ToDecimal());
            Assert.AreEqual((decimal)Math.PI, new BigNumber((decimal)Math.PI).ToDecimal());
            _ = Assert.ThrowsException<OverflowException>(() => new BigNumber("79228162514264337593543950336").ToDecimal());
            _ = Assert.ThrowsException<OverflowException>(() => new BigNumber("-79228162514264337593543950336").ToDecimal());
            _ = Assert.ThrowsException<OverflowException>(() => new BigNumber("792281625142643375935439503350").ToDecimal());
            _ = Assert.ThrowsException<OverflowException>(() => new BigNumber("-792281625142643375935439503350").ToDecimal());
        }

        [TestMethod()]
        public void RoundTest()
        {
            string[,] tests =
            {
                { "0", "0", "0" },
                { "0", "10", "0" },
                { "1.2", "0", "1" },
                { "1.1111", "3", "1.111" },
                { "1.1111", "4", "1.1111" },
                { "1.1111", "5", "1.1111" },
                { "12.3456", "3", "12.346" },
                { "12.3456", "4", "12.3456" },
                { "12.3456", "5", "12.3456" },
                { "-1.1111", "3", "-1.111" },
                { "-1.1111", "4", "-1.1111" },
                { "-1.1111", "5", "-1.1111" },
                { "-12.3456", "3", "-12.346" },
                { "-12.3456", "4", "-12.3456" },
                { "-12.3456", "5", "-12.3456" }
            };

            for (int i = 0; i < tests.GetLength(0); i++)
            {
                BigNumber n = new BigNumber(tests[i, 0]);
                Assert.AreEqual(tests[i, 2], n.Round(int.Parse(tests[i, 1])).Value);
            }

            BigNumber num = new BigNumber(12.3456M);
            Assert.AreEqual("12", num.Round().Value);
            Assert.AreEqual(num.Round().Value, num.Round(0).Value);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => num.Round(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => num.Round(-10));
        }
    }
}