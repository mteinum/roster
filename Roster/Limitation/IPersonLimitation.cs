namespace Roster.Limitation
{
    public interface IPersonLimitation
    {
        public bool IsAvailable(Duty duty);
    }
}