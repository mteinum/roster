namespace Roster.Limitation
{
    class WeaponRentLimitation : IPersonLimitation
    {
        private bool WeaponRent { get; }

        public WeaponRentLimitation(bool weaponRent)
        {
            WeaponRent = weaponRent;
        }

        public bool IsAvailable(Duty duty)
        {
            if (duty.WeaponRent)
                return WeaponRent;

            return true;
        }
    }
}
