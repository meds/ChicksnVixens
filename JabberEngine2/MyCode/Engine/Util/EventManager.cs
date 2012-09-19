using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jabber.Util
{
    public class Event : JabJect
    {
    }
    public class EventManager : JabJect
    {
        static EventManager manager;
        public static EventManager Get
        {
            get
            {
                if (manager == null)
                {
                    manager = new EventManager();
                }
                return manager;
            }
        }
        public EventManager()
            : base()
        {
        }

        public void RegisterListner(JabJect obj)
        {
            for (int i = 0; i < m_lsUpdateStack.Count; i++)
            {
                if (m_lsUpdateStack[i].obj == obj)
                {
                    return;
                }
            }
            m_lsUpdateStack.Add(new Listening(obj));
        }
        public void UnregisterListner(JabJect obj)
        {
            for (int i = 0; i < m_lsUpdateStack.Count; i++)
            {
                if (m_lsUpdateStack[i].obj == obj)
                {
                    m_lsUpdateStack.Remove(m_lsUpdateStack[i]);
                    --i;
                }
            }
        }

        // Forces event manager to process all messages, use with caution!
        public void Flush()
        {
            Update(null);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime dt)
        {
            List<Event> temp = new List<Event>(events); events.Clear();
            List<Listening> tempListenStack = new List<Listening>(m_lsUpdateStack);
            foreach (Event e in temp)
            {
                foreach (Listening l in tempListenStack)
                {
                    l.obj.ProcessEvent(e);
                }
            }

            base.Update(dt);
        }

        // sends an event to be processed immediately, warning: use with caution!
        public void SendImmediateEvent(Event ev)
        {
            for (int i = 0; i < m_lsUpdateStack.Count; i++)
            {
                m_lsUpdateStack[i].obj.ProcessEvent(ev);
            }
        }

        public void SendEvent(Event ev)
        {
            events.Add(ev);
        }
        List<Event> events = new List<Event>();
        class Listening
        {
            public Listening(JabJect obj)
            {
                this.obj = obj;
            }
            public JabJect obj;
        };
        List<Listening> m_lsUpdateStack = new List<Listening>();
    }
}
