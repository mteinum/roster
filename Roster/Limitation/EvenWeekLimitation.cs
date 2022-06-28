namespace Roster.Limitation
{
    public class EvenWeekLimitation : IPersonLimitation
    {
        public bool IsAvailable(Duty duty)
        {
            return duty.WeekNumber % 2 == 0;
        }
    }
}