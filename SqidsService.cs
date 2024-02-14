using Sqids;

namespace UrlSh
{
    public class SqidsService
    {
        private const string _alphabet = "VjXZ0vRgQAYtqcenW3hN41apEfKDwb5zJs9FS8MoCH2xLdIkTrUi6uBP7ymOlG";
        private const int _minLength = 6;
        private readonly Lazy<SqidsEncoder> _lazySqidsEncoder;

        public SqidsService()
        {
            _lazySqidsEncoder = new Lazy<SqidsEncoder>(() => new SqidsEncoder(new()
            {
                Alphabet = _alphabet,
                MinLength = _minLength
            }));
        }

        public int? Decode(string sqid)
        {
            var ids = _lazySqidsEncoder.Value.Decode(sqid);
            return ids.Count == 1 ? ids[0] : null;
        }

        public string Encode(int id)
        {
            return _lazySqidsEncoder.Value.Encode(id);
        }
    }
}
