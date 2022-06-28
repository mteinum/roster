namespace Roster
{
    class DutyTypeLimitation : IPersonLimitation
    {
        private List<DutyType> dutyTypes;

        public DutyTypeLimitation(List<DutyType> dutyTypes)
        {
            this.dutyTypes = dutyTypes;
        }

        public bool IsAvailable(Duty duty)
        {
            return dutyTypes.Contains(duty.DutyType);
        }
    }
}