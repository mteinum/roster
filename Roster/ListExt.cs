namespace Roster
{
    public static class ListExt
    {
        public static void AddIf<T>(this List<T> list, bool condition, T value)
        {
            if (condition)
            {
                list.Add(value);
            }
        }
    }
}