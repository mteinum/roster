namespace Roster.Limitation
{

    class DutyTypeLimitation : IPersonLimitation
    {
        private IReadOnlyList<DutyType> DutyTypes { get; }

        public DutyTypeLimitation(IReadOnlyList<DutyType> dutyTypes)
        {
            DutyTypes = dutyTypes;
        }

        public bool IsAvailable(Duty duty)
        {
            return DutyTypes.Contains(duty.DutyType);
        }
    }
}