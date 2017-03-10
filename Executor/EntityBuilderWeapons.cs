using System;

namespace Executor
{
    public static class EntityBuilderWeapons
    {
        private static Entity BuildWeapon(string label, AttachmentSize size, int maxRange, int damage, 
            int refireTicks)
        {
            return new Entity(label: label, typeLabel: "Weapon")
                .AddComponent(new Component_Attachable(size))
                .AddComponent(new Component_Weapon(size, Config.ZERO, maxRange, damage, refireTicks))
                .AddComponent(new Component_Attacker());
        }

        public static Entity BuildHFBlade()
        {
            return EntityBuilderWeapons.BuildWeapon("H.F. Blade", AttachmentSize.MEDIUM, 1, 10, 0);
        }

        public static Entity BuildPistol()
        {
            return EntityBuilderWeapons.BuildWeapon("Pistol", AttachmentSize.SMALL, 20, 3, 0);
        }

        public static Entity BuildCombatRifle()
        {
            return EntityBuilderWeapons.BuildWeapon("Combat Rifle", AttachmentSize.LARGE, 50, 8, 0);
        }
    }
}

