// <auto-generated />
namespace IronJS.Tests.UnitTests.Sputnik.Regression
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class RegressionTests : SputnikTestFixture
    {
        public RegressionTests()
            : base(@"Regression")
        {
        }

        [Test]
        [Category("Sputnik Regression")]
        [Category("ECMA 11.13.2")]
        [TestCase("S11.13.2_R2.3_T1.js", Description = "")]
        [TestCase("S11.13.2_R2.3_T2.js", Description = "")]
        [TestCase("S11.13.2_R2.3_T3.js", Description = "")]
        [TestCase("S11.13.2_R2.3_T4.js", Description = "")]
        [TestCase("S11.13.2_R2.3_T5.js", Description = "")]
        public void ResultIsNotToNumberUndefinedToNumberAssignmentExpression(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Regression")]
        [Category("ECMA 11.13.2")]
        [TestCase("S11.13.2_R2.3_T10.js", Description = "")]
        [TestCase("S11.13.2_R2.3_T11.js", Description = "")]
        [TestCase("S11.13.2_R2.3_T6.js", Description = "")]
        [TestCase("S11.13.2_R2.3_T7.js", Description = "")]
        [TestCase("S11.13.2_R2.3_T8.js", Description = "")]
        [TestCase("S11.13.2_R2.3_T9.js", Description = "")]
        public void ResultIsNotToInt32UndefinedToInt32AssignmentExpression(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Regression")]
        [Category("ECMA 12.6.4")]
        [Category("ECMA 9.9")]
        [TestCase("S12.6.4_R1.js", Description = "")]
        public void TheForInStatementMustNotThrowTypeErrorForUndefinedValuesCauseItSABugInTheStandard(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Regression")]
        [Category("ECMA 12.6.4")]
        [Category("ECMA 9.9")]
        [TestCase("S12.6.4_R2.js", Description = "")]
        public void TheForInStatementMustNotThrowTypeErrorForNullValuesCauseItSABugInTheStandard(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Regression")]
        [Category("ECMA 15.1.1.1")]
        [Category("ECMA 9.3")]
        [TestCase("S15.1.1.1_R1.js", Description = "")]
        public void TheNaNIsNotReadOnlyCheckNumberForResettabilityOfResult(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Regression")]
        [Category("ECMA 15.1.1.1")]
        [Category("ECMA 15.1.2")]
        [Category("ECMA 15.1.3")]
        [TestCase("S15.1.1.1_R2.1.js", Description = "")]
        public void TheNaNIsNotReadOnlyCheckIsNaNFunctionsThatUseToNumber(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Regression")]
        [Category("ECMA 15.1.1.1")]
        [Category("ECMA 15.1.2")]
        [Category("ECMA 15.1.3")]
        [TestCase("S15.1.1.1_R2.2.js", Description = "")]
        public void TheNaNIsNotReadOnlyCheckIsFiniteFunctionsThatUseToNumber(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Regression")]
        [Category("ECMA 15.1.1.1")]
        [Category("ECMA 15.1.2.4")]
        [TestCase("S15.1.1.1_R3.1.js", Description = "")]
        public void TheNaNIsNotReadOnlyCheckIsNaNForResettabilityOfResult(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Regression")]
        [Category("ECMA 15.1.1.1")]
        [Category("ECMA 15.1.2.4")]
        [TestCase("S15.1.1.1_R3.2.js", Description = "")]
        public void TheNaNIsNotReadOnlyCheckIsFiniteForResettabilityOfResultCHECK1NaN1(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Regression")]
        [Category("ECMA 11")]
        [Category("ECMA 15.1.1.1")]
        [TestCase("S15.1.1.1_R4.js", Description = "")]
        public void TheNaNIsNotReadOnlyCheckOperatorsThatUseToNumber(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Regression")]
        [Category("ECMA 15.1.1.2")]
        [Category("ECMA 9.3")]
        [TestCase("S15.1.1.2_R1.js", Description = "")]
        public void TheInfinityIsNotReadOnlyCheckNumberForResettabilityOfResult(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Regression")]
        [Category("ECMA 15.1.1.2")]
        [Category("ECMA 15.1.2")]
        [Category("ECMA 15.1.3")]
        [TestCase("S15.1.1.2_R2.1.js", Description = "")]
        public void TheInfinityIsNotReadOnlyCheckIsNaNFunctionsThatUseToNumber(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Regression")]
        [Category("ECMA 15.1.1.2")]
        [Category("ECMA 15.1.2")]
        [Category("ECMA 15.1.3")]
        [TestCase("S15.1.1.2_R2.2.js", Description = "")]
        public void TheInfinityIsNotReadOnlyCheckIsFiniteFunctionsThatUseToNumber(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Regression")]
        [Category("ECMA 15.1.1.2")]
        [Category("ECMA 15.1.2.4")]
        [TestCase("S15.1.1.2_R3.1.js", Description = "")]
        public void TheInfinityIsNotReadOnlyCheckIsNaNForResettabilityOfResult(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Regression")]
        [Category("ECMA 15.1.1.2")]
        [Category("ECMA 15.1.2.4")]
        [TestCase("S15.1.1.2_R3.2.js", Description = "")]
        public void TheInfinityIsNotReadOnlyCheckIsFiniteForResettabilityOfResultCHECK1Infinity0(string file)
        {
            RunFile(file);
        }

        [Test]
        [Category("Sputnik Regression")]
        [Category("ECMA 11")]
        [Category("ECMA 15.1.1.2")]
        [TestCase("S15.1.1.2_R4.js", Description = "")]
        public void TheInfinityIsNotReadOnlyCheckOperatorsThatUseToNumber(string file)
        {
            RunFile(file);
        }
    }
}