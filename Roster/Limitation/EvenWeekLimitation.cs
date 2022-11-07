namespace Roster.Limitation
{
    public class OddWeekLimitation : IPersonLimitation
    {
        public bool IsAvailable(Duty duty)
        {
            return duty.WeekNumber % 2 == 1;
        }
    }
}