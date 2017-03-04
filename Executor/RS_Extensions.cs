using RogueSharp.Random;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Executor
{
    // TODO: Wrap instead of extend! Good general policy for you deps, yea?
    public static class RS_Extensions
    {
        public static T RandomElement<T>(this IRandom _this, IEnumerable<T> items)
        {
            if (items.Count() > 0)
                return items.ElementAt(_this.Next(items.Count() - 1));
            else
                return default(T);
        }

        public static T RandomByWeight<T>(this IRandom _this, IEnumerable<T> items, Func<T, int> weight)
        {
            int totalweight = items.Sum(weight);
            int choice = _this.Next(totalweight - 1);
            int weightIndex = 0;

            foreach (var item in items)
            {
                weightIndex += weight(item);

                if (weightIndex > choice)
                    return item;
            }

            return default(T);
        }
    }
}
