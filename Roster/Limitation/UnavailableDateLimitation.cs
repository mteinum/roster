namespace Roster.Limitation
{
    class UnavailableDateLimitation : IPersonLimitation
    {
        DateTime From { get; }
        DateTime To { get; }

        public UnavailableDateLimitation(DateTime from) : this(from, from)
        {
        }

        public UnavailableDateLimitation(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }

        public bool IsAvailable(Duty duty)
        {
            if (duty.DateTime > To)
                return true;

            if (duty.DateTime < From)
                return true;

            return false;
        }
    }
}