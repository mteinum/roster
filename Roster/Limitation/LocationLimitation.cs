namespace Roster.Limitation
{
    class LocationLimitation : IPersonLimitation
    {
        private IReadOnlyList<Location> Locations { get; }

        public LocationLimitation(IReadOnlyList<Location> locations)
        {
            Locations = locations;
        }

        public bool IsAvailable(Duty duty)
        {
            return Locations.Contains(duty.Location);
        }
    }
}