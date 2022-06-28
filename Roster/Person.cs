namespace Roster
{
    public class Person
    {
        public string Name { get; set; }

        public string TogetherWith { get; set; }

        public List<IPersonLimitation> Limitations { get; set; }

        public bool IsAvailable(Duty duty)
        {
            if (Duties.Any(d => d.DateTime == duty.DateTime))
                return false;

            return Limitations.All(l => l.IsAvailable(duty));
        }

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

        public List<DateTime> Dates { get; set; }

        public int Priority(Duty duty)
        {
            if (Dates?.Contains(duty.DateTime) == true)
                return 0;

            return 1;
        }
    }
}