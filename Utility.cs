namespace media
{
    public static class Utility
    {
        public static IEnumerable<string> Seperate(string str, int chunkSize)
        {
            for (int i = 0; i < str.Length; i += chunkSize)
                yield return str.Substring(i, chunkSize);
        }

        public static string SaveThumbNails(string str)
        {
            var list = Seperate(str, 3);

            return list.First();
        }
    }
}
