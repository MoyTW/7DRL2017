using System;

namespace Executor
{
    public static class EntityBuilderWeapons
    {
        private static Entity BuildWeapon(string label, AttachmentSize size, int maxRange, int damage, 
            int refireTicks)
        {
            return new Entity(label, "Weapon")
                .AddComponent(new Component_Attachable(size))
                .AddComponent(new Component_Weapon(size, Config.ZERO, maxRange, damage, refireTicks))
                .AddComponent(new Component_Attacker());
        }

        public static Entity BuildRifle()
        {
            return EntityBuilderWeapons.BuildWeapon("Rifle", AttachmentSize.LARGE, 20, 8, 0);
        }
    }
}

