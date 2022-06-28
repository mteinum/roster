using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace Roster
{

    public class Program
    {
        private const string spreadsheetId = "17od-9xfJT-gJ0NPSx9H4SmfZ-cGY0-MQg0Z2qqYFYiM";
        private static string ApplicationName = "Roster";
        private static string[] Scopes = { SheetsService.Scope.Spreadsheets };

        private static UserCredential LoadUserCredential()
        {
            using (var stream =
                       new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                /* The file token.json stores the user's access and refresh tokens, and is created
                 automatically when the authorization flow completes for the first time. */
                string credPath = "token.json";
                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
        }

        public static void Main(string[] args)
        {
            UserCredential credential = LoadUserCredential();

            var service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });


            var persons = LoadPersons(service);
            var duties = LoadDuties(service);

            Assign(persons, duties);

            WriteBackToSheet(duties, service);

            Console.WriteLine("Hello, World!");
        }

        static void WriteBackToSheet(List<Duty> duties, SheetsService service)
        {
            var values = new List<ValueRange>();

            static ValueRange CreateValueRange(int row, string column, string content)
            {
                var oblist = new List<object>() { content };

                var valueRange = new ValueRange
                {
                    MajorDimension = "COLUMNS",
                    Range = $"Høst 2022!{column}{row}",
                    Values = new List<IList<object>> { oblist }
                };

                return valueRange;
            }

            foreach (var duty in duties)
            {
                values.Add(CreateValueRange(duty.Row, "H", duty.Person.Name));

                if (duty.Person2 != null)
                {
                    values.Add(CreateValueRange(duty.Row, "J", duty.Person2.Name));
                }

            }

            BatchUpdateValuesRequest body = new()
            {
                Data = values,
                ValueInputOption = "RAW"
            };


            SpreadsheetsResource.ValuesResource.BatchUpdateRequest update2 = service.Spreadsheets.Values.BatchUpdate(
                body, spreadsheetId);

            var response = update2.Execute();

        }

        static void Assign(List<Person> persons, List<Duty> duties)
        {
            Person GetNextPerson(Duty duty)
            {
                return persons
                    .Where(p => p.IsAvailable(duty))
                    .Where(p => string.IsNullOrEmpty(p.TogetherWith))
                    .OrderBy(p => p.Priority(duty))
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
                            .ThenBy(p => p.LastDuty)
                            .First();
                    }

                    person2.Duties.Add(duty);
                    duty.Person2 = person2;
                }
            }
        }

        static List<Duty> LoadDuties(SheetsService service)
        {
            var result = new List<Duty>();

            string range = "Høst 2022!A6:L215";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(spreadsheetId, range);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        static List<Person> LoadPersons(SheetsService service)
        {
            var result = new List<Person>();

            string range = "Vakter!A2:K33";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(spreadsheetId, range);

            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;

            if (values == null || values.Count == 0)
            {
                Console.WriteLine("No data found.");
            }

            foreach (var row in values)
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
                var dates = row.Count > 10 ? row[10].ToString() : null;

                var person = new Person
                {
                    Name = $"{row[1]} {row[0]}",
                    Limitations = new List<IPersonLimitation>(),
                    Duties = new List<Duty>(),
                    TogetherWith = vakt1
                };

                if (dates != null)
                {
                    person.Dates = dates.Split(';').Select(s => DateTime.ParseExact(s, "yyyyMMdd", null)).ToList();
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

                if (wd.Any(b => b))
                    person.Limitations.Add(new WeekDayLimitation(wd));

                if (!string.IsNullOrEmpty(vakt1))
                    person.Limitations.Add(new TogetherWithLimitation(vakt1));

                result.Add(person);
            }

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
    }
}