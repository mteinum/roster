using OfficeOpenXml;

namespace Roster
{
    static class StringEx
    {
        public static int SkipOneAndParse(this string s)
        {
            return int.Parse(string.Join("", s.Skip(1)));
        }
    }

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

    class SpreadsheetReader
    {
        public static string ApplicationName = "Roster";


        public static List<Duty> LoadDuties(ISheetDataSource dataSource)
        {
            var result = new List<Duty>();

            var sheet = dataSource.CreateReader("Aktiviteter");

            var previousBlank = false;

            for (int rowNumber=5; ; rowNumber++)
            {
                var row = sheet.ReadLine(rowNumber, 12);

                if (string.IsNullOrEmpty(row[0]) && previousBlank)
                    break;

                previousBlank = string.IsNullOrEmpty(row[0]);

                if (row.Count != 12)
                    continue;

                DutyType dutyType;
                Location location;

                if (row[4] == "Nybegynner")
                {
                    dutyType = DutyType.NewShooters;
                }
                else if (row[4] == "Organisert")
                {
                    dutyType = DutyType.Organized;
                }
                else if (row[4] == "Luft" && row[2] == "Fredag")
                {
                    dutyType = DutyType.AirPistol;
                }
                else
                {
                    // ungdom, stevne, annet som ikke automatisk skal tildeles
                    continue;
                }

                if (row[3] == "Gimlehallen")
                {
                    location = Location.Gimlehallen;
                }
                else
                {
                    location = Location.Farvannet;
                }

                result.Add(new Duty
                {
                    Row = rowNumber,
                    WeekNumber = int.Parse(row[0]),
                    DateTime = DateTime.FromOADate(Convert.ToDouble(row[1])),
                    DutyType = dutyType,
                    Location = location,
                    Person1Name = row[7],
                    Person1Phone = row[8],
                    Person2Name = row[9],
                    Person2Phone = row[10],
                    WeaponRent = Equals("True", row[11]),
                });
            }

            return result;
        }
    }
}