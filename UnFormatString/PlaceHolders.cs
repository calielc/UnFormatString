using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnFormatString {
    public class PlaceHolders : IReadOnlyCollection<PlaceHolder[]> {
        private readonly IReadOnlyList<PlaceHolder[]> _items;

        public PlaceHolders(string format, string input, IEnumerable<PlaceHolder[]> items) {
            Format = format;
            Input = input;
            _items = items?.ToArray();
        }

        public string Format { get; }
        public string Input { get; }
        public int Count => _items.Count;

        public IReadOnlyCollection<PlaceHolder> this[int index] => index < _items.Count ? _items[index] : null;

        public IEnumerator<PlaceHolder[]> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _items).GetEnumerator();
    }
}