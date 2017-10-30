using System;
using System.Diagnostics;
using System.Globalization;

namespace UnFormatString {
    [DebuggerDisplay("({Index} => Value: {Value}, Anchor: {Anchor}, Span: {Span}, Format: {Format})")]
    public struct PlaceHolder {
        public PlaceHolder(int index, string value, string anchor) : this(index, value, anchor, 0, null) { }

        public PlaceHolder(int index, string value, string anchor, string format) : this(index, value, anchor, 0, format) { }

        public PlaceHolder(int index, string value, string anchor, int span) : this(index, value, anchor, span, null) { }

        public PlaceHolder(int index, string value, string anchor, int span, string format) {
            Index = index;
            Value = value;
            Anchor = anchor;
            Span = span;
            Format = format;
        }

        public int Index { get; }
        public string Value { get; }
        public string Anchor { get; }
        public int Span { get; }
        public string Format { get; }

#if DEBUG
        public override string ToString() => $"({Index} => Value: {Value ?? "null"}, Anchor: {Anchor ?? "null"}, Span: {Span}, Format: {Format ?? "null"})";
#endif

        public bool TryAsDecimal(out decimal value) => decimal.TryParse(Value.Trim(), out value);

        public bool TryAsDouble(out double value) => double.TryParse(Value.Trim(), out value);

        public bool TryAsLong(out long value) => long.TryParse(Value.Trim(), out value);

        public bool TryAsDateTime(out DateTime value) => Format == null
            ? DateTime.TryParse(Value.Trim(), out value)
            : DateTime.TryParseExact(Value.Trim(), Format, CultureInfo.CurrentCulture, DateTimeStyles.None, out value);
    }
}