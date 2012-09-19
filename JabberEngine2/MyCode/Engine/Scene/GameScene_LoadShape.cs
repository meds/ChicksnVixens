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
using Jabber.J3D;

namespace Jabber.Scene
{
    public partial class GameScene : JabJect
    {
        public void Load3DShapes(XElement elements)
        {
            var items = elements.Descendants("Items");
            var element = elements.Descendants("CustomProperties").Descendants("Property");

            float tileSize = 0;
            float layerDepth = 0;
            float friction = 10.2f;
            string tileTexture = "";
            foreach (var property in element)
            {
                if (property.Attribute("Name") != null)
                {
                    string name = property.Attribute("Name").Value;
                    if (name == "TileSize")
                    {
                        tileSize = float.Parse(property.Attribute("Description").Value, CultureInfo.InvariantCulture);
                    }
                    else if (name == "Texture")
                    {
                        tileTexture = property.Attribute("Description").Value;
                    }
                    else if (name == "LayerDepth")
                    {
                        if (property.Descendants("string").First<XElement>().Value == "")
                        {
                            layerDepth = 0.0000002f;
                        }
                        else
                        {
                            layerDepth = float.Parse(property.Descendants("string").First<XElement>().Value, CultureInfo.InvariantCulture);

                            if (layerDepth == 2)
                            {
                                layerDepth = 0.0000002f;
                            }
                        }
                    }
                    else if (name == "Friction")
                    {
                        friction = float.Parse(property.Descendants("string").First<XElement>().Value, CultureInfo.InvariantCulture);
                    }
                }
            }
            layerDepth = 0.0000002f;

            PhysicShape shape = new PhysicShape(world, JabActor.BodyType.STATIC);
            shape.TextureDir = tileTexture;
            shape.TileSize = 256;// tileSize * 50.0f;
            shape.Initialize(Content);
            foreach (var item in items)
            {
                var parts = item.Descendants("Item");

                foreach (var part in parts)
                {
                    if (part.Name == "Item")
                    {
                        element = part.Descendants("Position");
                        float posx = float.Parse(element.Elements("X").ElementAt(0).Value, CultureInfo.InvariantCulture);
                        float posy = float.Parse(element.Elements("Y").ElementAt(0).Value, CultureInfo.InvariantCulture);

                        //element = part.Descendants("Rotation");
                        float rot = -float.Parse(part.Elements("Rotation").ElementAt(0).Value, CultureInfo.InvariantCulture);

                        element = part.Descendants("Scale");
                        float width = (float)double.Parse(element.Elements("X").ElementAt(0).Value, CultureInfo.InvariantCulture);
                        float height = float.Parse(element.Elements("Y").ElementAt(0).Value, CultureInfo.InvariantCulture);

                        JabActor box = world.CreateBox(new Vector2(width, height), new Vector2(posx, -posy), JabActor.BodyType.STATIC);
                        box.Friction = friction;
                        box.Rot = -rot;
                        shape.AddRectangle(new Vector3(posx, -posy, 0), new Vector2(width, height), rot);
                    }
                }
            }
            shape.Layer = BaseSprite.SpriteLayer.LAYER9;
            shape.FinalizeVertices();
            shape.PosZ = layerDepth;
            AddNode(shape);

            //shape.Friction = friction;
        }
    }
}