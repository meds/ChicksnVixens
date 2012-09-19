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
    public class Cow : PhysicSprite
    {
        public Cow()
            : base()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Body.UserData = this;
            Body.Restitution = 2.0f;
        }
    }
}
