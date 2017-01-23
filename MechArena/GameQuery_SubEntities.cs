using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena
{
    public enum SubEntitiesSelector
    {
        ALL = 0,
        WEAPON,
        BODY_PART,
        ACTIVE_TRACKS_TIME,
        SWAPPABLE_MOUNTS
    }

    // This is definitely astronauting the heck out of my problem.
    public class GameQuery_SubEntities : GameQuery
    {
        public static bool MatchesSelector(Entity en, SubEntitiesSelector s)
        {
            return en.MatchesSelector(s);
        }

        private SubEntitiesSelector[] selectors;
        private List<Entity> subEntities;

        public IReadOnlyCollection<SubEntitiesSelector> Selectors { get { return Array.AsReadOnly(this.selectors); } }

        public IList<Entity> SubEntities { get { return this.subEntities.AsReadOnly(); } }

        public GameQuery_SubEntities(params SubEntitiesSelector [] selectors)
        {
            if (selectors.Count() == 0)
                throw new ArgumentException("No selectors sent to GameQuery_SubEntities! What'cha doin?");
            this.subEntities = new List<Entity>();
            this.selectors = selectors;
        }

        public bool MatchesSelectors(Entity en)
        {
            return selectors.Any(s => MatchesSelector(en, s));
        }

        public void RegisterEntity(Entity en)
        {
            if (!this.MatchesSelectors(en))
                throw new ArgumentException("Attempted to register invalid item!");
            this.subEntities.Add(en);
        }
    }
}
