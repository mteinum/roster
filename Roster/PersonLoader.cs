using Roster.Sheet;

namespace Roster
{
    class PersonLoader
    {
        public static List<Person> LoadPersons(ISheetDataSource dataSource)
        {
            var sheet = dataSource.CreateReader("Vakter");

            var people = new List<Person>();

            for (int row = 2; ; row++)
            {
                var data = sheet.ReadLine(row, 17);

                if (string.IsNullOrEmpty(data[0]))
                    break;

                people.Add(PersonFactory.CreatePerson(data));
            }

            foreach (var person in people)
            {
                if (!string.IsNullOrEmpty(person.TogetherWith))
                {
                    var other = people.First(p => p.Name == person.TogetherWith);
                    other.Locations.Clear();
                }
            }

            return people;
        }
    }
}