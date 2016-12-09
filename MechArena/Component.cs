using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    public class Component
    {
        public GameEvent HandleEvent(GameEvent e)
        {
            return this._HandleEvent(e);
        }

        protected virtual GameEvent _HandleEvent(GameEvent e)
        {
            throw new NotImplementedException();
        }
    }
}
