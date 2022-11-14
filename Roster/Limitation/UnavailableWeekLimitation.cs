namespace Roster.Limitation
{
    class UnavailableWeekLimitation : IPersonLimitation
    {
        int Week { get; }

        public UnavailableWeekLimitation(int week)
        {
            Week = week;
        }

        public bool IsAvailable(Duty duty)
        {
            return duty.WeekNumber != Week;
        }
    }
}