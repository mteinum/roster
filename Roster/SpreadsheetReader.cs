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

            var result = request
                .Execute()
                .Values
                .Select(PersonFactory.CreatePerson)
                .ToList();

            foreach (var row in result)
            {
                if (!string.IsNullOrEmpty(row.TogetherWith))
                {
                    var other = result.First(p => p.Name == row.TogetherWith);

                    other.DutyTypes.Remove(DutyType.Organized);
                }
            }

            return result;
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

            int rowNumber = 5;

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
                else if (row[4].ToString() == "Luft")
                {
                    dutyType = DutyType.AirPistol;
                }
                else
                {
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
    }
}