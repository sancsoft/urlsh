using Sqids;

namespace UrlSh
{
    public class SqidsService
    {
        private const string _alphabet = "VjXZ0vRgQAYtqcenW3hN41apEfKDwb5zJs9FS8MoCH2xLdIkTrUi6uBP7ymOlG";
        private const int _minLength = 6;

        public int? Decode(string sqid)
        {
            var sqids = new SqidsEncoder(new()
            {
                Alphabet = _alphabet,
                MinLength = _minLength
            });

            var ids = sqids.Decode(sqid);
            if (ids.Count != 1)
            {
                return null;
            }

            return ids[0];
        }

        public string Encode(int id)
        {
            var sqids = new SqidsEncoder(new()
            {
                Alphabet = _alphabet,
                MinLength = _minLength
            });

            return sqids.Encode(id);
        }
    }
}
