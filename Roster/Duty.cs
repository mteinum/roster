namespace Roster
{
    public class Duty
    {
        public int Row { get; set; }
        public int WeekNumber { get; set; }
        public DutyType DutyType { get; set; }
        public Location Location { get; set; }
        public DateTime DateTime { get; set; }

        public Person Person { get; set; }

        public Person Person2 { get; set; }

        public override string ToString()
        {
            return $"{Row} {WeekNumber} {DutyType} {Location} {DateTime:d} {Person} {Person2}";
        }
    }
}