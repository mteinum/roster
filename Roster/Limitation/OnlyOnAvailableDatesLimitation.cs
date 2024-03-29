﻿namespace Roster.Limitation
{
    public class OnlyOnAvailableDatesLimitation : IPersonLimitation
    {
        private IReadOnlyList<DateTime> Dates { get; }

        public OnlyOnAvailableDatesLimitation(IReadOnlyList<DateTime> dates)
        {
            Dates = dates;
        }

        public bool IsAvailable(Duty duty)
        {
            if (Dates.Count == 0)
                return true;

            if (Dates.Contains(duty.DateTime))
                return true;

            return false;
        }
    }
}