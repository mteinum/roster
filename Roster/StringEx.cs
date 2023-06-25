namespace Roster
{
    static class StringEx
    {
        public static int SkipOneAndParse(this string s)
        {
            return int.Parse(string.Join("", s.Skip(1)));
        }
    }
}