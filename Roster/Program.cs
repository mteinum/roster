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

            void Usage()
            {
                Console.WriteLine("a: assign persons to duties");
                Console.WriteLine("d: assign persons to duties, dry run");
                Console.WriteLine("r: create report of persons");
            }

            Usage();
            var cmd = Console.ReadLine();

            if (cmd == "a" || cmd == "d")
            {
                var persons = SpreadsheetReader.LoadPersons(service);
                var duties = SpreadsheetReader.LoadDuties(service);

                Assign(persons, duties);

                if (cmd == "a")
                    SpreadsheetWriter.WriteBackToSheet(duties, service);
            }
            else if (cmd == "r")
            {
                SpreadsheetReader.CreatePersonReport(service);
            }
            else
            {
                Console.WriteLine($"Unknown command '{cmd}'");
            }
        }

        static void Assign(List<Person> persons, List<Duty> duties)
        {
            Person GetNextPerson(Duty duty)
            {
                var candidates = persons.IsAvailable(duty);

                var theChoosenOne = candidates
                    .OrderBy(p => p.Priority(duty))
                    .ThenBy(p => p.LastDuty)
                    .ThenBy(p => p.Duties.Count)
                    .ThenBy(p => p.AvailableDays)
                    .FirstOrDefault();

                return theChoosenOne;
            }

            // 1. alle nybegynnere (vakt 2 har ingen bane)

            foreach (var duty in duties)
            {
                var person = GetNextPerson(duty);

                if (person == null)
                    continue;

                person.Duties.Add(duty);
                duty.Person = person;

                if (duty.DutyType == DutyType.NewShooters)
                {
                    var person2 = persons.FromName(person.TogetherWith);

                    person2.Duties.Add(duty);
                    duty.Person2 = person2;
                }
            }
        }


    }
}