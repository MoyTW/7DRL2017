using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    public static class EntityBuilder
    {
        // TODO: Hardcoded values all over!
        public const string PartTypeLabel = "Part";

        public static Entity BuildBodyPart(BodyPartLocation location, int slotSpace, int internalStructure)
        {
            return new Entity(label: location.ToString(), typeLabel: "BodyPart")
                .AddComponent(new Component_BodyPartLocation(location))
                .AddComponent(new Component_SlottedContainer(slotSpace))
                .AddComponent(new Component_InternalStructure(internalStructure));
        }

        public static Entity BuildMech(string label)
        {
            return new Entity(label: label, typeLabel: "Mech")
                .AddComponent(new Component_MechSkeleton());
        }
        
        public static Entity BuildArmorPart()
        {
            return new Entity(label: "Armor", typeLabel: PartTypeLabel)
                .AddComponent(new Component_Slottable(1))
                .AddComponent(new Component_InternalStructure(4));
        }
    }
}
