namespace Roster.Limitation
{
    public class TogetherWithLimitation : IPersonLimitation
    {
        public string Person1 { get; }

        public TogetherWithLimitation(string person1)
        {
            Person1 = person1;
        }

        public bool IsAvailable(Duty duty)
        {
            return duty.Person?.Name == Person1;
        }
    }
}