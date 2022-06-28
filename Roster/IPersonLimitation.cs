namespace Roster
{
    public interface IPersonLimitation
    {
        public bool IsAvailable(Duty duty);
    }
}