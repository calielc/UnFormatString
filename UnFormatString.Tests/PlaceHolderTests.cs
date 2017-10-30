using System;
using NUnit.Framework;

namespace UnFormatString.Tests {
    public sealed class PlaceHolderTests {
        internal static readonly TestCaseData[] DecimalCases =
        {
            new TestCaseData("    12,788798   ", true, 12.788798m),
            new TestCaseData("12,5", true, 12.5m),
            new TestCaseData("-75,47", true, -75.47m),
            new TestCaseData("27", true, 27m),
            new TestCaseData("0", true, 0m),
            new TestCaseData("0.58", true, 58m),
            new TestCaseData("", false, default(decimal)),
        };

        [TestCaseSource(nameof(DecimalCases))]
        public void Should_try_convert_to_decimal(string text, bool expectedTry, decimal expectedValue) {
            var placeHolder = new PlaceHolder(0, text, "{0}");

            var actualTry = placeHolder.TryAsDecimal(out var actualValue);

            Assert.AreEqual(expectedTry, actualTry, "try");
            Assert.AreEqual(expectedValue, actualValue, "value");
        }

        internal static readonly TestCaseData[] DoubleCases =
        {
            new TestCaseData("    12,788798   ", true, 12.788798d),
            new TestCaseData("12,5", true, 12.5d),
            new TestCaseData("-75,47", true, -75.47d),
            new TestCaseData("27", true, 27d),
            new TestCaseData("0", true, 0d),
            new TestCaseData("0.58", true, 58d),
            new TestCaseData("", false, default(double)),
        };

        [TestCaseSource(nameof(DoubleCases))]
        public void Should_try_convert_to_double(string text, bool expectedTry, double expectedValue) {
            var placeHolder = new PlaceHolder(0, text, "{0}");

            var actualTry = placeHolder.TryAsDouble(out var actualValue);

            Assert.AreEqual(expectedTry, actualTry, "try");
            Assert.AreEqual(expectedValue, actualValue, "value");
        }

        internal static readonly TestCaseData[] LongCases =
        {
            new TestCaseData("-5789", true, -5789),
            new TestCaseData("27", true, 27),
            new TestCaseData("0", true, 0),
            new TestCaseData("    12,788798   ", false, default(long)),
            new TestCaseData("12,5", false, default(long)),
            new TestCaseData("", false, default(long)),
        };

        [TestCaseSource(nameof(LongCases))]
        public void Should_try_convert_to_long(string text, bool expectedTry, long expectedValue) {
            var placeHolder = new PlaceHolder(0, text, "{0}");

            var actualTry = placeHolder.TryAsLong(out var actualValue);

            Assert.AreEqual(expectedTry, actualTry, "try");
            Assert.AreEqual(expectedValue, actualValue, "value");
        }

        internal static readonly TestCaseData[] DateTimeCases = {
            new TestCaseData("", null, false, default(DateTime)),
            new TestCaseData("30", null, false, default(DateTime)),
            new TestCaseData("30", "dd", false, default(DateTime)),
            new TestCaseData("30", "dd", false, default(DateTime)),

            new TestCaseData("30/10/2017", null, true, DateTime.Parse("2017-10-30")),
            new TestCaseData("30/10/2017", "dd/MM/yyyy", true, DateTime.Parse("2017-10-30")),
            new TestCaseData("30-10-2017", "dd-MM-yyyy", true, DateTime.Parse("2017-10-30")),

            new TestCaseData("30-10-2017 12:58", null, true, DateTime.Parse("2017-10-30 12:58")),
            new TestCaseData("30-10-2017 12:58", "dd-MM-yyyy HH:mn", true, DateTime.Parse("2017-10-30 12:58")),

            new TestCaseData("2017-10-30T11:00:16.5609079-02:00", null, true, DateTime.Parse("2017-10-30T11:00:16.5609079-02:00")),
            new TestCaseData("2017-10-30T11:00:16.5609079-02:00", "o", true, DateTime.Parse("2017-10-30T11:00:16.5609079-02:00")),
            new TestCaseData("2017-10-30T11:00:16.5609079-02:00", "O", true, DateTime.Parse("2017-10-30T11:00:16.5609079-02:00")),
        };

        [TestCaseSource(nameof(DateTimeCases))]
        public void Should_try_convert_to_DateTime(string text, string format, bool expectedTry, DateTime expectedValue) {
            var placeHolder = format == null
                ? new PlaceHolder(0, text, "{0}")
                : new PlaceHolder(0, text, $"{{0:{format}}}");

            var actualTry = placeHolder.TryAsDateTime(out var actualValue);

            Assert.AreEqual(expectedTry, actualTry, "try");
            Assert.AreEqual(expectedValue, actualValue, "value");
        }
    }
}