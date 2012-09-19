using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;
using System.Text;

using System.Xml.Linq;

using System.Globalization;
using Microsoft.Phone.Tasks;

using Jabber.Media;
using Jabber.Util;
using Jabber.Physics;
//using Jabber.J3D;

namespace Jabber.Scene
{
    public partial class GameScene : JabJect
    {
        public List<JabActor> CreateBodies(XElement elements)
        {
            List<JabActor> bodies = new List<JabActor>();
            var items = elements.Descendants("Items");
            var element = elements.Descendants("CustomProperties").Descendants("Property");

            float layerDepth = 0;
            float mass = 0.25f;
            string type = "";
            float friction = 0.5f;
            float density = 15.0f;
            float restitution = 0;
            bool dynamic = true;
            float angulardamping = 0;
            foreach (var property in element)
            {
                if (property.Attribute("Name") != null)
                {
                    string name = property.Attribute("Name").Value;
                    if (name == "LayerDepth")
                    {
                        layerDepth = float.Parse(property.Descendants("string").First<XElement>().Value, CultureInfo.InvariantCulture);
                    }
                    else if (name == "Mass")
                    {
                        mass = float.Parse(property.Descendants("string").First<XElement>().Value, CultureInfo.InvariantCulture);
                    }
                    else if (name == "Friction")
                    {
                        friction = float.Parse(property.Descendants("string").First<XElement>().Value, CultureInfo.InvariantCulture);
                    }
                    else if (name == "Type")
                    {
                        type = property.Descendants("string").First<XElement>().Value;
                    }
                    else if (name == "Density")
                    {
                        density = float.Parse(property.Descendants("string").First<XElement>().Value, CultureInfo.InvariantCulture);
                    }
                    else if (name == "Restitution")
                    {
                        restitution = float.Parse(property.Descendants("string").First<XElement>().Value, CultureInfo.InvariantCulture);
                    }
                    else if (name == "Dynamic")
                    {
                        dynamic = property.Descendants("string").First<XElement>().Value.ToLowerInvariant() == "true";
                    }
                    else if (name == "AngularDamping")
                    {
                        angulardamping = float.Parse(property.Descendants("string").First<XElement>().Value, CultureInfo.InvariantCulture);
                    }
                }
            }

            List<Sprite> baseSprites = CreateSprites(elements);

            for (int i = 0; i < baseSprites.Count; i++)
            {
                Sprite s = baseSprites[i];
                float posx = s.PosX;
                float posy = s.PosY;

                float rot = s.Rot;

                float width = s.ScaleX * s.Width;
                float height = s.ScaleY * s.Height;

                JabActor.BodyType bodyState = JabActor.BodyType.DYNAMIC;
                JabActor actor = null;
                if (!dynamic)
                {
                    bodyState = JabActor.BodyType.STATIC;
                }

                if (type == "Circle")
                {
                    actor = world.CreateSphere(width, new Vector2(posx, posy), bodyState);
                }
                else
                {
                    actor = world.CreateBox(new Vector2(width, height), new Vector2(posx, posy), bodyState);
                }

                actor.Friction = friction;
                actor.Mass = mass;
                actor.Restitution = restitution;
                actor.Mass = mass;
                actor.Rot = rot;
                actor.Friction = friction;
                actor.PosZ = layerDepth;
                actor.Restitution = restitution;
                actor.Density = density;
                actor.AngularDamping = angulardamping;

                bodies.Add(actor);
            }

            return bodies;
        }
    }
}
