namespace Roster
{
    static class PersonExt
    {
        public static Person FromName(this List<Person> list, string name) => list.Single(p => p.Name == name);

        public static IEnumerable<Person> IsAvailable(this IEnumerable<Person> list, Duty duty)
            => list.Where(p => p.IsAvailable(duty));

    }
}