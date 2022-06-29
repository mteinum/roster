using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Roster.Limitation;

namespace Roster
{
    class SpreadsheetReader
    {
        static DateTime Iso8601(string s)
        {
            return DateTime.ParseExact(s, "yyyyMMdd", null);
        }

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
                .Select(CreatePerson)
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

        private static Person CreatePerson(IList<object> row)
        {
            bool HasFlag(int column)
            {
                if (row.Count > column)
                    return row[column].ToString() == "1";
                return false;
            }

            bool onlyEvenWeek = HasFlag(2);
            bool newShooter = HasFlag(3);

            bool[] wd = new bool[] {
                    HasFlag(4),
                    HasFlag(5),
                    HasFlag(6),
                    HasFlag(7),
                    HasFlag(8),
                };

            var vakt1 = row.Count > 9 ? row[9].ToString() : null;
            var available = row.Count > 10 ? row[10].ToString() : null;
            var unavailable = row.Count > 11 ? row[11].ToString() : null;

            var person = new Person
            {
                Name = $"{row[1]} {row[0]}",
                Limitations = new List<IPersonLimitation>(),
                Duties = new List<Duty>(),
                TogetherWith = vakt1,
                AvailableDates = new List<DateTime>(),
                AvailableDays = wd.All(b => b == false) ? 5 : wd.Count(b => b)
            };

            person.Limitations.Add(new NoDuplicateDutyLimitation(person.Duties));

            if (available != null)
            {
                foreach (var date in available.Split(';'))
                {
                    // syntax:
                    // part;part;part
                    //
                    // part:
                    // single date: yyyyMMdd
                    // after date: yyyyMMdd+
                    // range: yyyyMMdd-yyyyMMdd (TODO)

                    if (date.EndsWith("+"))
                    {
                        person.Limitations.Add(new AvailableAfterLimitation(Iso8601(date.Substring(0, 8))));
                    }
                    else if (date.Length == 8)
                    {
                        person.AvailableDates.Add(Iso8601(date));
                    }
                }

                person.Limitations.Add(new OnlyOnAvailableDatesLimitation(person.AvailableDates));
            }

            if (unavailable != null)
            {
                foreach (var date in unavailable.Split(';'))
                {
                    if (date.Length == 8)
                    {
                        person.Limitations.Add(new UnavailableDateLimitation(Iso8601(date)));
                    }
                    else
                    {
                        var parts = date.Split('-');

                        person.Limitations.Add(new UnavailableDateLimitation(Iso8601(parts[0]), Iso8601(parts[1])));

                    }
                }
            }

            if (onlyEvenWeek)
                person.Limitations.Add(new EvenWeekLimitation());

            var dutyTypes = new List<DutyType>();

            if (newShooter)
                dutyTypes.Add(DutyType.NewShooters);
            else
                dutyTypes.Add(DutyType.Organized);

            if (wd.Last())
                dutyTypes.Add(DutyType.AirPistol);

            person.Limitations.Add(new DutyTypeLimitation(dutyTypes));
            person.DutyTypes = dutyTypes;

            person.Limitations.Add(new LocationLimitation(new List<Location> {
                Location.Farvannet,
                Location.Gimlehallen }));

            if (wd.Any(b => b))
                person.Limitations.Add(new WeekDayLimitation(wd));

            if (!string.IsNullOrEmpty(vakt1))
                person.Limitations.Add(new TogetherWithLimitation(vakt1));

            return person;
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
                    Row = rowNumber
                });
            }

            return result;
        }
    }
}