using OfficeOpenXml;
using Roster.Sheet;

namespace Roster
{

    public class Program
    {
        public static void Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(@"c:\\code\roster\KPS Aktivitetskalender.xlsx"))
            {
                var service2 = new ExcelSheetDataSource(package);

                void Usage()
                {
                    Console.WriteLine("a: assign persons to duties");
                    Console.WriteLine("d: assign persons to duties, dry run");
                    Console.WriteLine("c: clear duties");
                    Console.WriteLine("r: create person report");
                }

                Usage();
                var cmd = Console.ReadLine();

                if (cmd == "a" || cmd == "d")
                {
                    var persons = PersonLoader.LoadPersons(service2);
                    var duties = DutyLoader.LoadDuties(service2);

                    Assign(persons, duties);

                    SpreadsheetWriter.WriteBackToSheet(duties, service2);

                    if (cmd == "a")
                        package.Save();
                }
                else if (cmd == "c")
                {
                    var duties = DutyLoader.LoadDuties(service2);
                    SpreadsheetWriter.WriteBackToSheet(duties, service2);
                    //package.Save();
                }
                else if (cmd == "r")
                {
                    var duties = DutyLoader.LoadDuties(service2);

                    var names = duties.Select(d => new[] { d.Person1Name, d.Person2Name })
                        .SelectMany(d => d)
                        .Where(s => !string.IsNullOrEmpty(s))
                        .GroupBy(k => k);

                    foreach (var name in names.OrderByDescending(k => k.Count()))
                    {
                        Console.WriteLine($"{name.Key}: {name.Count()}");
                    }
                }
                else
                {
                    Console.WriteLine($"Unknown command '{cmd}'");
                }
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