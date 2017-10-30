namespace UnFormatString {
    public interface IUnFormatString {
        string Format { get; }

        PlaceHolders Unformat(string input);
    }
}