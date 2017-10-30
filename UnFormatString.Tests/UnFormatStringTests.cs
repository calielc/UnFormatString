using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace UnFormatString.Tests {
    public sealed class UnFormatStringTests {
        [TestCase("Texto: {0}", "ABC")]
        [TestCase("Texto: {0}", 123)]
        [TestCase("Texto: {0}", false)]
        [TestCase("{0} foi o texto", "DEF")]
        [TestCase("{0} foi o texto", 489)]
        [TestCase("{0} foi o texto", true)]
        [TestCase("texto: {0} era esse", "ER")]
        [TestCase("texto: {0} era esse", 789)]
        [TestCase("texto: {0} era esse", 58.87d)]
        public void Should_identify_with_1_placeholder(string format, object value) {
            var expected = new[] { new PlaceHolder(0, value.ToString(), "{0}") };

            var input = string.Format(format, value);

            var unformat = new UnFormatString(format);
            var actual = unformat.Unformat(input);

            AssertPlaceHolders(format, input, actual, expected);
        }

        [TestCase("Data: {0:dd/MM/yyy}", "dd/MM/yyy")]
        [TestCase("Data: {0:dd}", "dd")]
        [TestCase("Data: {0:HH:mm:ss}", "HH:mm:ss")]
        public void Should_identify_with_mask(string format, string dateFormat) {
            var data = DateTime.Parse("2016-12-04");

            var expected = new[] { new PlaceHolder(0, data.ToString(dateFormat), $"{{0:{dateFormat}}}", dateFormat) };

            var input = string.Format(format, data);

            var unformat = new UnFormatString(format);
            var actual = unformat.Unformat(input);

            AssertPlaceHolders(format, input, actual, expected);
        }

        [TestCase("Texto: {0}, Idade: {1}", "ABC", 12)]
        [TestCase("Idade: {1}, Texto: {0}", "FEG", 25)]
        public void Should_identify_with_2_placeholder(string format, object value1, object value2) {
            var expected0 = new[] { new PlaceHolder(0, value1.ToString(), "{0}") };
            var expected1 = new[] { new PlaceHolder(1, value2.ToString(), "{1}") };

            var input = string.Format(format, value1, value2);

            var unformat = new UnFormatString(format);
            var actual = unformat.Unformat(input);


            AssertPlaceHolders(format, input, actual, expected0, expected1);
        }

        [TestCase("Puro: {0}, Mascara1: {0:000.00}, mascara2: {0:p}, mascara3: {0:#,##0.00}")]
        public void Should_identify_same_placeholder_multiply_times(string format) {
            var expected = new[]
            {
                new PlaceHolder(0, "15689,4", "{0}"),
                new PlaceHolder(0, "15689,40", "{0:000.00}", "000.00"),
                new PlaceHolder(0, "1.568.940,00%", "{0:p}", "p"),
                new PlaceHolder(0, "15.689,40", "{0:#,##0.00}", "#,##0.00"),
            };

            var input = string.Format(format, 15689.4);

            var unformat = new UnFormatString(format);
            var actual = unformat.Unformat(input);

            AssertPlaceHolders(format, input, actual, expected);
        }

        [TestCase("Texto: ({0})", "")]
        [TestCase("Texto: ({0})", null)]
        public void Should_identify_empty_value(string format, object value) {
            var expected = new[] { new PlaceHolder(0, string.Empty, "{0}") };

            var input = string.Format(format, value);

            var unformat = new UnFormatString(format);
            var actual = unformat.Unformat(input);

            AssertPlaceHolders(format, input, actual, expected);
        }

        [TestCase("abc: {0,20}, def: {0:0.00}, fgh: {0,20:#00.00}, ih: {0,-20}")]
        public void Should_identify_span_and_mask(string format) {
            var expected = new[]
            {
                new PlaceHolder(0,"                12,5", "{0,20}", 20),
                new PlaceHolder(0,"12,50", "{0:0.00}", "0.00"),
                new PlaceHolder(0,"               12,50", "{0,20:#00.00}", 20, "#00.00"),
                new PlaceHolder(0,"12,5                ", "{0,-20}", -20),
            };

            var input = string.Format(format, 12.5);

            var unformat = new UnFormatString(format);
            var actual = unformat.Unformat(input);

            AssertPlaceHolders(format, input, actual, expected);
        }

        [Test]
        public void Should_identify_multiple_uses_like_1() {
            var expected0 = new[]
            {
                new PlaceHolder(0, "2", "{0}"),
                new PlaceHolder(0, "2,00", "{0:0.00}", "0.00"),
            };
            var expected1 = new[]
            {
                new PlaceHolder(1, "3", "{1}"),
            };
            var expected2 = new[]
            {
                new PlaceHolder(2, "   5,00000", "{2,10:0.00000}", 10, "0.00000"),
            };

            const string format = "mascara: {0}, repetido: {0}, outro: {1}, mesmo mas diferente: {0:0.00}, outro: {2,10:0.00000}, repetido: {0}";
            var input = string.Format(format, 2, 3, 5);

            var unformat = new UnFormatString(format);
            var actual = unformat.Unformat(input);

            AssertPlaceHolders(format, input, actual, expected0, expected1, expected2);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("Text: 12")]
        public void Should_return_null_if_doesnt_match(string value) {
            var unformat = new UnFormatString("Texto: {0}");
            var actual = unformat.Unformat(value);

            Assert.IsNull(actual);
        }

        [Test]
        public void Should_contain_original_data() {
            const string format = "Puro: {0}, Mascara1: {0:000.00}, mascara2: {0:p}, mascara3: {0:#,##0.00}, outro: {1:o}, outro²: {2,20:00.00}, repetido: {0:000.00}, outro³: {3}";

            var expected = new[] {
                new UnFormatString.Term("id0", "{0}", 0, 0, null),
                new UnFormatString.Term("id1", "{0:000.00}", 0, 0, "000.00"),
                new UnFormatString.Term("id2", "{0:p}", 0, 0, "p"),
                new UnFormatString.Term("id3", "{0:#,##0.00}", 0, 0, "#,##0.00"),
                new UnFormatString.Term("id4", "{1:o}", 1, 0, "o"),
                new UnFormatString.Term("id5", "{2,20:00.00}", 2, 20, "00.00"),
                new UnFormatString.Term("id6", "{3}", 3, 0, null),
            };

            var unformat = new UnFormatString(format);

            Assert.AreEqual(format, unformat.Format);
            CollectionAssert.AreEqual(expected, unformat.Terms);
        }

        [Test]
        public void Shoud_escape_caracters() {
            const string format = "Issue não é uma ancora {{0}}, mas isso sim {0}";
            var terms = new[] {
                new UnFormatString.Term("id0", "{0}", 0, 0, null),
            };

            var unformat = new UnFormatString(format);

            Assert.AreEqual(format, unformat.Format, nameof(unformat.Format));
            CollectionAssert.AreEqual(terms, unformat.Terms, nameof(unformat.Terms));

            var input = string.Format(format, " AbC ");
            var expected = new[] { new PlaceHolder(0, " AbC ", "{0}") };

            var placeHolders = unformat.Unformat(input);

            AssertPlaceHolders(format, input, placeHolders, expected);
        }

        private static void AssertPlaceHolders(string format, string input, PlaceHolders actual, params IReadOnlyCollection<PlaceHolder>[] expecteds) {
            Assert.IsNotNull(actual, "actual != null");

            Assert.AreEqual(format, actual.Format, nameof(actual.Format));
            Assert.AreEqual(input, actual.Input, nameof(actual.Input));

            Assert.AreEqual(expecteds.Length, actual.Count, nameof(actual.Count));
            for (var i = 0; i < expecteds.Length; i++) {
                CollectionAssert.AreEqual(expecteds[i], actual[i], $"[{i}]");
            }

        }
    }
}