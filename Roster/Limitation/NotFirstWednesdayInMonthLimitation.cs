namespace Roster.Limitation
{
    public class NotFirstWednesdayInMonthLimitation : IPersonLimitation
    {
        public bool IsAvailable(Duty duty)
        {
            var dt = duty.DateTime;

            if (dt.DayOfWeek == DayOfWeek.Wednesday &&
                GetFirstWednesdayInMonth(dt.Year, dt.Month) == dt.Day)
            {
                return false;
            }

            return true;
        }

        public int GetFirstWednesdayInMonth(int year, int month)
        {
            for (int day = 1; day <= 31; day++)
            {
                var d = new DateTime(year, month, day);

                if (d.DayOfWeek == DayOfWeek.Wednesday)
                    return day;
            }

            throw new Exception("Problem...");
        }


    }
}