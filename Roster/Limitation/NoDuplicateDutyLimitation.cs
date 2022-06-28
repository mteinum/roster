namespace Roster.Limitation
{
    public class NoDuplicateDutyLimitation : IPersonLimitation
    {
        private IReadOnlyList<Duty> Assigned { get; }

        public NoDuplicateDutyLimitation(IReadOnlyList<Duty> assigned)
        {
            Assigned = assigned;
        }

        public bool IsAvailable(Duty duty)
        {
            if (Assigned.Any(d => d.DateTime == duty.DateTime))
                return false;

            return true;

        }
    }
}