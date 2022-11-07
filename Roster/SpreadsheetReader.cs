using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Roster
{

    class SpreadsheetReader
    {
        public static string ApplicationName = "Roster";

        public static SheetsService CreateSheetsService()
        {
            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = UserCredentialBuilder.LoadUserCredential(),
                ApplicationName = ApplicationName
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        public static List<Person> LoadPersons(SheetsService service)
        {
            SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(Config.SpreadsheetId, Config.PersonRange);

            var persons = request
                .Execute()
                .Values
                .Select(PersonFactory.CreatePerson)
                .ToList();

            foreach (var person in persons)
            {
                if (!string.IsNullOrEmpty(person.TogetherWith))
                {
                    var other = persons.First(p => p.Name == person.TogetherWith);
                    other.Locations.Clear();
                    //other.DutyTypes.Remove(DutyType.Organized);
                }
            }

            return persons;
        }



        public static List<Duty> LoadDuties(SheetsService service)
        {
            var result = new List<Duty>();

            SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(Config.SpreadsheetId, Config.DutyRange);

            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;

            if (values == null || values.Count == 0)
            {
                Console.WriteLine("No data found.");
            }

            int rowNumber = 229; // see Config.DutyRange

            foreach (var row in values)
            {
                rowNumber++;

                if (row.Count != 12)
                    continue;

                DutyType dutyType;
                Location location;

                if (row[4].ToString() == "Nybegynner")
                {
                    dutyType = DutyType.NewShooters;
                }
                else if (row[4].ToString() == "Organisert")
                {
                    dutyType = DutyType.Organized;
                }
                else if (row[4].ToString() == "Luft" && row[2].ToString() == "Fredag")
                {
                    dutyType = DutyType.AirPistol;
                }
                else
                {
                    // ungdom, stevne, annet som ikke automatisk skal tildeles
                    continue;
                }

                if (row[3].ToString() == "Gimlehallen")
                {
                    location = Location.Gimlehallen;
                }
                else
                {
                    location = Location.Farvannet;
                }

                result.Add(new Duty
                {
                    DateTime = DateTime.ParseExact(row[1].ToString(), "yyyy-MM-dd", null),
                    DutyType = dutyType,
                    Location = location,
                    WeekNumber = int.Parse(row[0].ToString()),
                    Row = rowNumber,
                    WeaponRent = Equals("TRUE", row[11])
                });
            }

            return result;
        }

        public static void CreatePersonReport(SheetsService service)
        {
            SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(Config.SpreadsheetId, Config.PersonReportRange);

            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;

            if (values == null || values.Count == 0)
            {
                Console.WriteLine("No data found.");
            }

            var persons = new List<string>();

            foreach (var row in values)
            {
                if (row.Count > 0)
                    persons.Add(row[0].ToString());
                if (row.Count > 2)
                    persons.Add(row[2].ToString());
            }

            var report = persons
                .Where(s => string.IsNullOrEmpty(s) == false)
                .GroupBy(s => s);

            foreach (var r in report.OrderByDescending(x => x.Count()))
            {
                Console.WriteLine($"{r.Key}: {r.Count()}");
            }
        }
    }
}