using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;
using System.Text;

using Jabber.Util;
using Jabber.Media;
using Jabber.Physics;

namespace ChicksnVixens
{
    public class ThrownHat : Sprite
    {
        public ThrownHat(string loc):base("hats/" + loc)
        {
            this.loc = loc;
        }
        string loc;
        public override void Initialize(ContentManager Content)
        {
            base.Initialize(Content);

            CreateFramesFromXML("hats/" + loc + "_frames");
            CurrentFrame = loc + "-idle-000";

            ResetDimensions();

            InitialDir = new Vector2(RandomFloatInRange(-5, 5), RandomFloatInRange(10, 20));
        }

        float RotDir = RandomFloatInRange(-1, 1);
        float fallTimer = 1.0f;
        public Vector2 InitialDir = Vector2.Zero;

        public override void Update(GameTime dt)
        {
            base.Update(dt);

            Rot += RotDir * gttf(dt);

            Position += InitialDir;
            PosY -= fallTimer * 9.8f;
            fallTimer += gttf(dt);

            if (PosY < -600)
            {
                RaiseFlag(Jabber.Flags.DELETE);
            }
        }
    }
}
