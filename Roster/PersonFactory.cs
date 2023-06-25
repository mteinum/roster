using Roster.Limitation;

namespace Roster
{
    public static class ListExt
    {
        public static void AddIf<T>(this List<T> list, bool condition, T value)
        {
            if (condition)
            {
                list.Add(value);
            }
        }
    }

    class PersonFactory
    {
        static DateTime Iso8601(string s)
        {
            return DateTime.ParseExact(s, "dd.MM.yyyy", null);
        }

        public class PersonDto
        {
            public string Name { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
            public bool OddWeek { get; set; }
            public bool Youth { get; set; }
            public bool NewShooter { get; set; }
            public bool Air { get; set; }
            public string Locations { get; set; }
            public bool Monday { get; set; }
            public bool Tuesday { get; set; }
            public bool Wednesday { get; set; }
            public bool Thursday { get; set; }
            public string Person2 { get; set; }
            public string Available { get; set; }
            public string Unavailable { get; set; }
            public bool NotFirstWednesdayInMonth { get; set; }
            public bool WeaponRent { get; set; }

            public static PersonDto Create(IList<string> row)
            {
                bool True(int n) => string.Equals(row[n], "true", StringComparison.InvariantCultureIgnoreCase);

                return new PersonDto
                {
                    Name = row[0],
                    Mobile = row[1],
                    Email = row[2],
                    Person2 = row[3],
                    OddWeek = True(4),
                    Youth = True(5),
                    NewShooter = True(6),
                    Air = True(7),
                    Locations = row[8],
                    Monday = True(9),
                    Tuesday = True(10),
                    Wednesday = True(11),
                    Thursday = True(12),
                    Available = row[13],
                    Unavailable = row[14],
                    NotFirstWednesdayInMonth = True(15),
                    WeaponRent = True(16)
                };
            }
        }

        public static Person CreatePerson(IList<string> row) =>
            CreatePerson(PersonDto.Create(row.Select(s => s ?? string.Empty).ToList()));

        public static Person CreatePerson(PersonDto row)
        {
            bool[] weekdays = new bool[] {
                    row.Monday,
                    row.Tuesday,
                    row.Wednesday,
                    row.Thursday
                };

            var person = new Person
            {
                Name = row.Name,
                Mobile = row.Mobile,
                Limitations = new List<IPersonLimitation>(),
                Duties = new List<Duty>(),
                DutyTypes = new List<DutyType>(),
                TogetherWith = row.Person2,
                AvailableDates = new List<DateTime>(),
                AvailableDays = weekdays.All(b => b == false) ? weekdays.Length : weekdays.Count(b => b),
                WeaponRent = row.WeaponRent,
                Locations = new List<Location>(),
                Youth = row.Youth,
                Air = row.Air,
                NewShooter = row.NewShooter
            };

            person.Limitations.Add(new WeaponRentLimitation(row.WeaponRent));
            person.Limitations.Add(new NoDuplicateDutyLimitation(person.Duties));
            person.Limitations.Add(new OnlyOnAvailableDatesLimitation(person.AvailableDates));
            person.Limitations.Add(new DutyTypeLimitation(person.DutyTypes));
            person.Limitations.Add(new LocationLimitation(person.Locations));

            person.Limitations.AddIf(row.NotFirstWednesdayInMonth, new NotFirstWednesdayInMonthLimitation());
            person.Limitations.AddIf(row.OddWeek, new OddWeekLimitation());
            person.Limitations.AddIf(weekdays.Any(b => b), new WeekDayLimitation(weekdays));
            //person.Limitations.AddIf(!string.IsNullOrEmpty(row.Person2), new TogetherWithLimitation(row.Person2));

            person.Locations.AddIf(row.Locations.Contains('G'), Location.Gimlehallen);
            person.Locations.AddIf(row.Locations.Contains('F'), Location.Farvannet);

            AddAvailable(person, row);
            AddUnavailable(person, row);
            AddDutyTypes(person, row);

            return person;
        }

        static void AddDutyTypes(Person person, PersonDto row)
        {
            if (row.NewShooter)
            {
                person.DutyTypes.Add(DutyType.NewShooters);
            }
            else if (row.Air)
            {
                person.DutyTypes.Add(DutyType.AirPistol);
            }
            else if (row.Youth)
            {
                // Manuell handtering
            }
            else
            {
                person.DutyTypes.Add(DutyType.Organized);
            }
        }

        static void AddUnavailable(Person person, PersonDto row)
        {
            foreach (var date in row.Unavailable.Split(',').Select(d => d.Trim()))
            {
                if (date.Length == 10)
                {
                    person.Limitations.Add(new UnavailableDateLimitation(Iso8601(date)));
                }
                else if (date.Length == 3 && date[0] == 'U')
                {
                    person.Limitations.Add(new UnavailableWeekLimitation(date.SkipOneAndParse()));
                }
            }
        }

        static void AddAvailable(Person person, PersonDto row)
        {
            foreach (var date in row.Available.Split(',').Select(d => d.Trim()))
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
                    person.Limitations.Add(new AvailableAfterLimitation(Iso8601(date[..10])));
                }
                else if (date.Length == 10)
                {
                    person.AvailableDates.Add(Iso8601(date));
                }
            }
        }

    }
}