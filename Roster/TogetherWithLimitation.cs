namespace Roster
{
    public class TogetherWithLimitation : IPersonLimitation
    {
        public string person1 { get; }

        public TogetherWithLimitation(string person1)
        {
            this.person1 = person1;
        }

        public bool IsAvailable(Duty duty)
        {
            return duty.Person?.Name == person1;
        }
    }
}