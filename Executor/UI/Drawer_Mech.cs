using RLNET;

namespace Executor.UI
{
    class Drawer_Mech
    {
        private static int DrawBodyPartStatus(Entity bodyPart, int x, int y, bool mechDestroyed, RLConsole console)
        {
            var bodyPartDestroyed = bodyPart.TryGetDestroyed();
            var bodyPartStructure = bodyPart.TryGetAttribute(EntityAttributeType.STRUCTURE).Value;

            if (mechDestroyed || bodyPartDestroyed)
                console.Print(x, y, "    " + bodyPart.ToString() + ":" + bodyPartStructure + " ", RLColor.Red);
            else
                console.Print(x, y, "    " + bodyPart.ToString() + ":" + bodyPartStructure + " ", RLColor.Black);
            console.Print(x, ++y, " |----------------| ", RLColor.Black);
            y++;

            var mountedParts = bodyPart.HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.ALL)).SubEntities;
            foreach (var mountedPart in mountedParts)
            {
                var mountedPartDestroyed = mountedPart.TryGetDestroyed();
                var structureQuery = mountedPart.TryGetAttribute(EntityAttributeType.STRUCTURE);
                var outputLine = mountedPart.Label;
                if (structureQuery.IsBaseRegistered)
                    outputLine += ": " + structureQuery.Value;

                if (mechDestroyed || bodyPartDestroyed || mountedPartDestroyed)
                    console.Print(x + 3, y, outputLine, RLColor.Red);
                else
                    console.Print(x + 3, y, outputLine, RLColor.Black);

                console.Print(x + 1, y, "+", RLColor.Black);
                console.Print(x + 18, y, "+", RLColor.Black);

                if (mountedPart.HasComponentOfType<Component_AttachPoint>() &&
                    mountedPart.GetComponentOfType<Component_AttachPoint>().HasAttachedEntity)
                {
                    console.Print(x, ++y, " | ^              | ", RLColor.Black);
                }
                else
                {
                    console.Print(x, ++y, " |----------------| ", RLColor.Black);
                }
                y++;
            }

            return y - 3;
        }

        private static void DrawAIInfo(Entity mech, int x, int y, RLConsole console)
        {
            int line = y;

            console.Print(x + 1, ++line, "Enemy Threat Assessment:", RLColor.Black);
            var clauses = mech.GetComponentOfType<Component_AI>().ActionClauses;
            foreach (var clause in clauses)
            {
                console.Print(x + 1, ++line, "* " + clause.Label + " (" + clause.Description + ")", RLColor.Black);

            }
        }

        private static void DrawSkeleton(Entity mech, RLConsole console, int line, bool mechDestroyed)
        {
            var skeleton = mech.GetComponentOfType<Component_Skeleton>();
            Drawer_Mech.DrawBodyPartStatus(skeleton.InspectBodyPart(BodyPartLocation.RIGHT_ARM), 0, line + 4, mechDestroyed, console);
            Drawer_Mech.DrawBodyPartStatus(skeleton.InspectBodyPart(BodyPartLocation.RIGHT_LEG), 0, line + 14, mechDestroyed, console);

            Drawer_Mech.DrawBodyPartStatus(skeleton.InspectBodyPart(BodyPartLocation.LEFT_ARM), 40, line + 4, mechDestroyed, console);
            Drawer_Mech.DrawBodyPartStatus(skeleton.InspectBodyPart(BodyPartLocation.LEFT_LEG), 40, line + 14, mechDestroyed, console);

            Drawer_Mech.DrawBodyPartStatus(skeleton.InspectBodyPart(BodyPartLocation.HEAD), 20, line, mechDestroyed, console);
            Drawer_Mech.DrawBodyPartStatus(skeleton.InspectBodyPart(BodyPartLocation.TORSO), 20, line + 8, mechDestroyed, console);
        }

        public static void DrawMechStatus(Entity mech, RLConsole console)
        {
            int line = 1;

            int y = line;
            while (y < console.Height)
            {
                console.Print(0, y, "                                                                                                     ", RLColor.Black);
                y++;
            }
            
            var ai = mech.GetComponentOfType<Component_AI>();
            if (mech.HasComponentOfType<Component_Player>() || (ai != null && ai.Scanned))
            {
                var mechDestroyed = mech.TryGetDestroyed();
                if (mechDestroyed)
                    console.Print(1, line, mech.ToString() + "             ", RLColor.Red);
                else
                    console.Print(1, line, mech.ToString() + "             ", RLColor.Black);
                line++;
                line++;
                DrawSkeleton(mech, console, line, mechDestroyed);
                if (ai != null)
                {
                    Drawer_Mech.DrawAIInfo(mech, 0, line + 26, console);
                }
            }
            else
            {
                console.Print(console.Width / 2 - 15, console.Height / 2 - 2, "##############################", RLColor.Black);
                console.Print(console.Width / 2 - 15, console.Height / 2 - 1, "#                            #", RLColor.Black);
                console.Print(console.Width / 2 - 15, console.Height / 2,     "# NO DATA - APPROACH TO SCAN #", RLColor.Black);
                console.Print(console.Width / 2 - 15, console.Height / 2 + 1, "#                            #", RLColor.Black);
                console.Print(console.Width / 2 - 15, console.Height / 2 + 2, "##############################", RLColor.Black);
            }
        }
    }
}
