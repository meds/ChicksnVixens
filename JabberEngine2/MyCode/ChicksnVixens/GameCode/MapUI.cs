using Jabber.Media;
using Jabber.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;

using Jabber.Util;
using Jabber;

namespace ChicksnVixens
{
    public class NewLocationSelected : Event
    {
    }

    public class LocationTapped : Event
    {
        public LocationTapped(string loc)
        {
            location = loc;
        }
        public string location;
    }

    public class MapUI : Sprite
    {
        public MapUI()
            : base("WorldMap/map")
        {
            Layer = SpriteLayer.LAYER6;
        }

        public override void Initialize(ContentManager Content)
        {
            base.Initialize(Content);
            /*CreateFramesFromXML("WorldMap/map_ui_frames");

            CurrentFrame = "map";
            ResetDimensions();*/

            if (worldLocations.Count == 0)
            {
                worldLocations.Add("paris", new Vector2(-747, 268));
                worldLocations.Add("bavaria", new Vector2(-608, 287));
                worldLocations.Add("uluru", new Vector2(524, -590));
                worldLocations.Add("polar", new Vector2(-195, 959));
                worldLocations.Add("vesuvius", new Vector2(-671, 205));
            }
        }
        
        public Vector2 GetLocation(string location)
        {
            return worldLocations[location];
        }

        public static Dictionary<string, Vector2> worldLocations = new Dictionary<string, Vector2>();
    }

    public class LocationIcon : Sprite
    {
        public LocationIcon(string location, MapUI map)
            : base("WorldMap/map_ui")
        {
            this.location = location;
            this.map = map;

            Layer = SpriteLayer.LAYER5;
            RaiseFlag(Flags.ACCEPTINPUT);
        }

        public string location = "";
        MapUI map;
        public override void Initialize(ContentManager Content)
        {
            base.Initialize(Content);
            CreateFramesFromXML("WorldMap/map_ui_frames");

            CurrentFrame = location;
            ResetDimensions();

            UniformScale = ScaleFactor * 0.25f;

            Position = map.GetLocation(location);

            EventManager.Get.RegisterListner(this);
        }

        public override void OnTap(Vector2 pos)
        {
            Vector2 worldPos = Camera.Get.ScreenToWorld(pos);
        }


        float timer = 0;
        bool updatecalled = false;
        public override void Update(GameTime dt)
        {
            base.Update(dt);

            updatecalled = true;

            if (CheckFlag(Flags.FADE_OUT))
            {
                timer = JabMath.MoveTowards(timer, 0, gttf(dt) * 4.5f, 0.001f);

                if (timer == 0)
                {
                    RaiseFlag(Flags.DELETE);
                }
            }
            else
                timer = JabMath.MoveTowards(timer, 1.0f, gttf(dt) * 1.5f);
            Colour = new Color(timer, timer, timer, timer);
        }

        public override void Draw()
        {
            if (!updatecalled)
            {
                int k = 0;
            }
            base.Draw();
        }


        public override void ProcessEvent(Event ev)
        {
            base.ProcessEvent(ev);

            if (ev is NewLocationSelected)
            {
                RaiseFlag(Flags.FADE_OUT);// RaiseFlag(Flags.DELETE);
            }
        }
    }
}
