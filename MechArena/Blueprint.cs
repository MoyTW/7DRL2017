using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena
{
    public class ComponentBlueprint
    {
        public string Class { get; set; }
        public Dictionary<string, string> Params { get; set; }

        public Component BuildComponent()
        {
            throw new NotImplementedException();
        }
    }

    public class Blueprint
    {
        public string Label { get; set; }
        public string TypeLabel { get; set; }
        public List<ComponentBlueprint> Components { get; set; }

        public Entity BuildEntity()
        {
            var e = new Entity(label: this.Label, typeLabel: this.TypeLabel);
            foreach(ComponentBlueprint c in this.Components)
            {
                e.AddComponent(c.BuildComponent());
            }
            return e;
        }
    }
}
