namespace Roster
{
    static class PersonExt
    {
        public static Person FromName(this List<Person> list, string name) => list.Single(p => p.Name == name);

        public static IEnumerable<Person> IsAvailable(this IEnumerable<Person> list, Duty duty)
            => list.Where(p => p.IsAvailable(duty));

    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var service = SpreadsheetReader.CreateSheetsService();

            var persons = SpreadsheetReader.LoadPersons(service);
            var duties = SpreadsheetReader.LoadDuties(service);

            Assign(persons, duties);

            SpreadsheetWriter.WriteBackToSheet(duties, service);
        }

        static void Assign(List<Person> persons, List<Duty> duties)
        {
            Person GetNextPerson(Duty duty)
            {
                return persons
                    .IsAvailable(duty)
                    .OrderBy(p => p.Priority(duty))
                    .ThenBy(p => p.Duties.Count)
                    .ThenBy(p => p.AvailableDays)
                    .ThenBy(p => p.LastDuty)
                    .First();
            }

            // 1. alle nybegynnere (vakt 2 har ingen bane)

            foreach (var duty in duties)
            {
                var person = GetNextPerson(duty);

                person.Duties.Add(duty);
                duty.Person = person;

                if (duty.DutyType == DutyType.NewShooters)
                {
                    // sjekk om det finnes en vakt som er sammen med vakt1
                    var person2 = persons.FromName(person.TogetherWith);

                    person2.Duties.Add(duty);
                    duty.Person2 = person2;
                }
            }
        }


    }
}