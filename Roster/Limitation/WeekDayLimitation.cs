namespace Roster.Limitation
{
    class WeekDayLimitation : IPersonLimitation
    {
        private bool[] Available { get; }

        public WeekDayLimitation(bool[] available)
        {
            Available = available;
        }

        public bool IsAvailable(Duty duty)
        {
            int ix = (int)duty.DateTime.DayOfWeek;

            return Available[ix - 1];
        }
    }
}