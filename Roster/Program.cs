namespace Roster
{

    public class Program
    {
        private const string spreadsheetId = "17od-9xfJT-gJ0NPSx9H4SmfZ-cGY0-MQg0Z2qqYFYiM";

        public static void Main(string[] args)
        {
            var service = SpreadsheetReader.CreateSheetsService();

            var persons = SpreadsheetReader.LoadPersons(service, spreadsheetId);
            var duties = SpreadsheetReader.LoadDuties(service, spreadsheetId);

            Assign(persons, duties);

            SpreadsheetWriter.WriteBackToSheet(duties, service, spreadsheetId);
        }

        static void Assign(List<Person> persons, List<Duty> duties)
        {
            Person GetNextPerson(Duty duty)
            {
                return persons
                    .Where(p => p.IsAvailable(duty))
                    .Where(p => string.IsNullOrEmpty(p.TogetherWith))
                    .OrderBy(p => p.Priority(duty))
                    .ThenBy(p => p.Duties.Count)
                    .ThenBy(p => p.LastDuty)
                    .First();
            }

            foreach (var duty in duties)
            {
                var person = GetNextPerson(duty);

                person.Duties.Add(duty);
                duty.Person = person;

                if (duty.DutyType == DutyType.NewShooters)
                {
                    // sjekk om det finnes en vakt som er sammen med vakt1
                    var person2 = persons.FirstOrDefault(p => p.TogetherWith == person.Name);

                    if (person2 == null)
                    {
                        var onlyAsPerson1 = persons
                            .Where(p => string.IsNullOrEmpty(p.TogetherWith) == false)
                            .Select(p => p.TogetherWith)
                            .ToList();

                        person2 = persons
                            .Where(p => p != duty.Person)
                            .Where(p => p.IsAvailable(duty))
                            .Where(p => onlyAsPerson1.Contains(p.Name) == false)
                            .OrderBy(p => p.Priority(duty))
                            .ThenBy(p => p.Duties.Count)
                            .ThenBy(p => p.LastDuty)
                            .First();
                    }

                    person2.Duties.Add(duty);
                    duty.Person2 = person2;
                }
            }
        }


    }
}