using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnFormatString {
    public class UnFormatString : IUnFormatString {
        private readonly Regex _regularExpression;
        private readonly List<Term> _terms = new List<Term>();

        public UnFormatString(string format) {
            Format = format;

            var expression = Regex.Escape(format);

            var discoveryRegEx = new Regex(@"(\\\{(?<index>[\d]*)(,(?<span>[\-\d]*))?(:(?<format>[\d\w\/ \.\,\\\:\#]*))?\})");
            foreach (Match match in discoveryRegEx.Matches(expression)) {
                var anchor = Regex.Unescape(match.Value);

                Term term;
                var terms = _terms.Where(item => item.Anchor == anchor).Take(1).ToArray();
                if (terms.Length == 0) {
                    term = new Term(
                        id: $"id{_terms.Count}",
                        anchor: anchor,
                        index: Convert.ToInt32(match.Groups["index"].Value),
                        span: string.IsNullOrEmpty(match.Groups["span"].Value)
                            ? 0
                            : Convert.ToInt32(match.Groups["span"].Value),
                        format: string.IsNullOrEmpty(match.Groups["format"].Value)
                            ? null
                            : Regex.Unescape(match.Groups["format"].Value));

                    _terms.Add(term);
                }
                else {
                    term = terms.Single();
                }

                expression = expression.Replace(match.Value, $@"(?<{term.Id}>[\s\S]*)");
            }
            expression = "^" + expression + "$";

            _regularExpression = new Regex(expression);
        }

        public string Format { get; }

        public IReadOnlyCollection<Term> Terms => _terms;

        public PlaceHolders Unformat(string input) {
            if (input == null) {
                return null;
            }

            if (_regularExpression.IsMatch(input) == false) {
                return null;
            }

            var match = _regularExpression.Match(input);

            var maxIndex = _terms.Max(term => term.Index);

            var result = new List<PlaceHolder>[maxIndex + 1];
            for (var i = 0; i < result.Length; i++) {
                result[i] = new List<PlaceHolder>();
            }

            foreach (var term in _terms) {
                var value = match.Groups[term.Id].Value;

                var placeHolder = new PlaceHolder(term.Index, value, term.Anchor, term.Span, term.Format);

                result[term.Index].Add(placeHolder);
            }

            return new PlaceHolders(Format, input, result.Select(line => line.ToArray()));
        }

        [DebuggerDisplay("(Id: {Id}, Anchor: {Anchor}, Index: {Index}, Span: {Span}, Format: {Format})")]
        public struct Term {
            internal Term(string id, string anchor, int index, int span, string format) {
                Id = id;
                Anchor = anchor;
                Index = index;
                Span = span;
                Format = format;
            }

            public string Id { get; }
            public string Anchor { get; }
            public int Index { get; }
            public int Span { get; }
            public string Format { get; }

#if DEBUG
            public override string ToString() => $"(Id: {Id}, Anchor: {Anchor}, Index: {Index}, Span: {Span}, Format: {Format})";
#endif
        }
    }
}