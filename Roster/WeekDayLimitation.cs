namespace Roster
{
    class WeekDayLimitation : IPersonLimitation
    {
        private bool[] available { get; }

        public WeekDayLimitation(bool[] available)
        {
            this.available = available;
        }

        public bool IsAvailable(Duty duty)
        {
            int ix = (int)duty.DateTime.DayOfWeek;

            return available[ix - 1];
        }
    }
}