using Roster.Limitation;

namespace Roster
{
    public class Person
    {
        public string Name { get; set; }

        public string TogetherWith { get; set; }

        public List<IPersonLimitation> Limitations { get; set; }

        public bool IsAvailable(Duty duty) => Limitations.All(l => l.IsAvailable(duty));

        public List<Duty> Duties { get; set; }

        public List<DutyType> DutyTypes { get; set; }

        public DateTime LastDuty
        {
            get
            {
                if (Duties.Count == 0)
                    return DateTime.MinValue;

                return Duties.Select(d => d.DateTime).Max();
            }
        }

        public override string ToString()
        {
            return $"{Name} ({Duties.Count})";
        }

        public List<DateTime> AvailableDates { get; set; }

        public int AvailableDays { get; set; }

        public int Priority(Duty duty)
        {
            if (AvailableDates?.Contains(duty.DateTime) == true)
                return 0;

            return 1;
        }
    }
}