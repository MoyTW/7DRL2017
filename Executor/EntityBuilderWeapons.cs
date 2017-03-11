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

        public static Entity BuildBaton()
        {
            return EntityBuilderWeapons.BuildWeapon("Baton", AttachmentSize.SMALL, 1, 3, 0);
        }

        public static Entity BuildPistol()
        {
            return EntityBuilderWeapons.BuildWeapon("Pistol", AttachmentSize.SMALL, 20, 3, 0);
        }

        public static Entity BuildShotgun()
        {
            return EntityBuilderWeapons.BuildWeapon("Shotgun", AttachmentSize.MEDIUM, 12, 5, 0);
        }

        public static Entity BuildRifle()
        {
            return EntityBuilderWeapons.BuildWeapon("Rifle", AttachmentSize.MEDIUM, 25, 4, 0);
        }

        public static Entity HFBaton()
        {
            return EntityBuilderWeapons.BuildWeapon("H.F. Baton", AttachmentSize.MEDIUM, 1, 9, 0);
        }

        public static Entity BuildCarbine()
        {
            return EntityBuilderWeapons.BuildWeapon("Rifle", AttachmentSize.LARGE, 25, 9, 0);
        }

        public static Entity BuildAssaultRifle()
        {
            return EntityBuilderWeapons.BuildWeapon("Rifle", AttachmentSize.LARGE, 30, 6, 0);
        }

        public static Entity BuildCombatRifle()
        {
            return EntityBuilderWeapons.BuildWeapon("Combat Rifle", AttachmentSize.LARGE, 50, 8, 0);
        }
    }
}

