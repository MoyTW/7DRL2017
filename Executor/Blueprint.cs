using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Executor
{
    public class ComponentBlueprint
    {
        private Func<Component> construct;

        public string ComponentClass { get; set; }
        public Dictionary<string, string> Params { get; set; }

        private static Type InferType(string value)
        {
            return ConvertValue(value).GetType();
        }

        private static object ConvertValue(string value)
        {
            int parsedInt;
            bool isInt = Int32.TryParse(value, out parsedInt);
            double parsedDouble;
            bool isDouble = Double.TryParse(value, out parsedDouble);
            bool parsedBool;
            bool isBool = Boolean.TryParse(value, out parsedBool);

            if (isInt)
                return parsedInt;
            else if (isDouble)
                return parsedDouble;
            else if (isBool)
                return parsedBool;
            else if (value.Contains('.'))
            {
                var typeString = value.Substring(0, value.LastIndexOf('.'));
                var inferredType = Type.GetType(typeString, false);
                if (inferredType != null)
                {
                    return Enum.Parse(inferredType, value.Substring(value.LastIndexOf('.') + 1));
                }
            }

            return value;
        }

        private void Initialize()
        {
            Type[] blueprintTypes = this.Params.Values.Select(p => InferType(p)).ToArray();
            var constructor = Type.GetType(this.ComponentClass).GetConstructor(blueprintTypes);

            var blueprintParams = this.Params.Values.Select(p => Expression.Constant(ConvertValue(p)));

            this.construct = Expression.Lambda<Func<Component>>(Expression.New(constructor, blueprintParams))
                .Compile();
        }

        public Component BuildComponent()
        {
            if (this.construct == null)
                this.Initialize();
            return this.construct();
        }
    }

    public class Blueprint
    {
        private Entity referenceEntity;
        public Entity ReferenceEntity
        {
            get
            {
                if (this.referenceEntity == null)
                    this.referenceEntity = this.BuildEntity();
                return this.referenceEntity;
            }
        }

        public string Label { get; set; }
        public string TypeLabel { get; set; }
        public List<ComponentBlueprint> Components { get; set; }

        public Entity BuildEntity()
        {
            var e = new Entity(label: this.Label, typeLabel: this.TypeLabel);
            foreach (ComponentBlueprint c in this.Components)
            {
                e.AddComponent(c.BuildComponent());
            }
            return e;
        }

        public bool MatchesSelector(SubEntitiesSelector selector)
        {
            return this.ReferenceEntity.MatchesSelector(selector);
        }
    }
}
