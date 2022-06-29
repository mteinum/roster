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
            return DateTime.ParseExact(s, "yyyyMMdd", null);
        }

        public class PersonDto
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Mobile { get; set; }
            public bool EvenWeek { get; set; }
            public bool NewShooter { get; set; }
            public bool Monday { get; set; }
            public bool Tuesday { get; set; }
            public bool Wednesday { get; set; }
            public bool Thursday { get; set; }
            public bool Friday { get; set; }
            public string Person2 { get; set; }
            public string Available { get; set; }
            public string Unavailable { get; set; }
            public bool WeaponRent { get; set; }

            public static PersonDto Create(IList<object> row)
            {
                return new PersonDto
                {
                    LastName = row[0].ToString(),
                    FirstName = row[1].ToString(),
                    Mobile = row[2].ToString(),
                    EvenWeek = Equals("1", row[3]),
                    NewShooter = Equals("1", row[4]),
                    Monday = Equals("1", row[5]),
                    Tuesday = Equals("1", row[6]),
                    Wednesday = Equals("1", row[7]),
                    Thursday = Equals("1", row[8]),
                    Friday = Equals("1", row[9]),
                    Person2 = row[10].ToString(),
                    Available = row[11].ToString(),
                    Unavailable = row[12].ToString(),
                    WeaponRent = Equals(row[13], "TRUE")
                };
            }
        }

        public static Person CreatePerson(IList<object> row) => CreatePerson(PersonDto.Create(row));

        public static Person CreatePerson(PersonDto row)
        {
            bool[] weekdays = new bool[] {
                    row.Monday,
                    row.Tuesday,
                    row.Wednesday,
                    row.Thursday,
                    row.Friday
                };

            var person = new Person
            {
                Name = $"{row.FirstName} {row.LastName}",
                Mobile = row.Mobile,
                Limitations = new List<IPersonLimitation>(),
                Duties = new List<Duty>(),
                DutyTypes = new List<DutyType>(),
                TogetherWith = row.Person2,
                AvailableDates = new List<DateTime>(),
                AvailableDays = weekdays.All(b => b == false) ? 5 : weekdays.Count(b => b),
                WeaponRent = row.WeaponRent
            };

            person.Limitations.Add(new WeaponRentLimitation(row.WeaponRent));
            person.Limitations.Add(new NoDuplicateDutyLimitation(person.Duties));
            person.Limitations.Add(new OnlyOnAvailableDatesLimitation(person.AvailableDates));
            person.Limitations.Add(new DutyTypeLimitation(person.DutyTypes));
            person.Limitations.Add(new LocationLimitation(new List<Location> {
                Location.Farvannet,
                Location.Gimlehallen }));

            person.Limitations.AddIf(row.EvenWeek, new EvenWeekLimitation());
            person.Limitations.AddIf(weekdays.Any(b => b), new WeekDayLimitation(weekdays));
            person.Limitations.AddIf(!string.IsNullOrEmpty(row.Person2), new TogetherWithLimitation(row.Person2));

            AddAvailable(person, row);
            AddUnavailable(person, row);
            AddDutyTypes(person, row, weekdays);

            return person;
        }

        static void AddDutyTypes(Person person, PersonDto row, bool[] weekdays)
        {
            if (row.NewShooter)
                person.DutyTypes.Add(DutyType.NewShooters);
            else
                person.DutyTypes.Add(DutyType.Organized);

            if (weekdays.Last())
                person.DutyTypes.Add(DutyType.AirPistol);
        }

        static void AddUnavailable(Person person, PersonDto row)
        {
            foreach (var date in row.Unavailable.Split(';'))
            {
                if (date.Length == 8)
                {
                    person.Limitations.Add(new UnavailableDateLimitation(Iso8601(date)));
                }
                else if (date.Length == (8 + 1 + 8))
                {
                    var parts = date.Split('-');

                    person.Limitations.Add(new UnavailableDateLimitation(Iso8601(parts[0]), Iso8601(parts[1])));
                }
            }
        }

        static void AddAvailable(Person person, PersonDto row)
        {
            foreach (var date in row.Available.Split(';'))
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
        }

    }
}