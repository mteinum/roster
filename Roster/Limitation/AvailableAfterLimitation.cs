namespace Roster.Limitation
{
    class AvailableAfterLimitation : IPersonLimitation
    {
        private DateTime AvailableAfter { get; }

        public AvailableAfterLimitation(DateTime availableAfter)
        {
            AvailableAfter = availableAfter;
        }

        public bool IsAvailable(Duty duty)
        {
            return duty.DateTime >= AvailableAfter;
        }
    }
}