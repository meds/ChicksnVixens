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
        public List<BaseSprite> CreateBaseSprites(XElement elements)
        {
            BaseSprite sprite = null;
            var items = elements.Descendants("Items");
            var element = elements.Descendants("CustomProperties").Descendants("Property");

            float layerDepth = 0;
            foreach (var property in element)
            {
                if (property.Attribute("Name") != null)
                {
                    string name = property.Attribute("Name").Value;
                    if (name == "LayerDepth")
                    {
                        layerDepth = float.Parse(property.Descendants("string").First<XElement>().Value, CultureInfo.InvariantCulture);
                    }
                }
            }

            List<BaseSprite> lsSprites = new List<BaseSprite>();

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

                        float rot = float.Parse(part.Elements("Rotation").ElementAt(0).Value, CultureInfo.InvariantCulture);

                        element = part.Descendants("Scale");
                        float width = float.Parse(element.Elements("X").ElementAt(0).Value, CultureInfo.InvariantCulture);
                        float height = float.Parse(element.Elements("Y").ElementAt(0).Value, CultureInfo.InvariantCulture);

                        sprite = new BaseSprite();
                        sprite.Rot = rot;
                        sprite.PosZ = layerDepth;
                        sprite.Position = new Vector2(posx, -posy);
                        sprite.Scale = new Vector2(width, height);
                        lsSprites.Add(sprite);
                    }
                }
            }

            return lsSprites;
        }

        public List<Sprite> CreateSprites(XElement elements)
        {
            BaseSprite sprite = null;
            var items = elements.Descendants("Items");
            var element = elements.Descendants("CustomProperties").Descendants("Property");

            float layerDepth = 0;
            foreach (var property in element)
            {
                if (property.Attribute("Name") != null)
                {
                    string name = property.Attribute("Name").Value;
                    if (name == "LayerDepth")
                    {
                        layerDepth = float.Parse(property.Descendants("string").First<XElement>().Value, CultureInfo.InvariantCulture);
                    }
                }
            }

            List<Sprite> lsSprites = new List<Sprite>();

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

                        float rot = float.Parse(part.Elements("Rotation").ElementAt(0).Value, CultureInfo.InvariantCulture);

                        element = part.Descendants("Scale");
                        float width = float.Parse(element.Elements("X").ElementAt(0).Value, CultureInfo.InvariantCulture);
                        float height = float.Parse(element.Elements("Y").ElementAt(0).Value, CultureInfo.InvariantCulture);

                        element = part.Descendants("asset_name");
                        string textureDir = element.ElementAt(0).Value;

                        textureDir = "textures\\" + textureDir;

                        bool continueLoad = true;
                        if (GetInterception(textureDir) != null)
                        {
                            SingleTextureToMassTexture s = GetInterception(textureDir);
                            string newTextureDir = s.newTextureDir;
                            string newTextureDirXML = s.newTextureDirXML;

                            sprite = new Sprite(newTextureDir);
                            sprite.Initialize(Content);
                            (sprite as Sprite).CreateFramesFromXML(newTextureDirXML);
                            (sprite as Sprite).CurrentFrame = s.FrameName;
                            (sprite as Sprite).ResetDimensions();
                        }
                        else if (HasObjInterceptor(textureDir))
                        {
                            sprite = new BaseSprite();
                        }
                        else
                        {
                            continueLoad = false;
                            sprite = null;
                            /*
                            try
                            {
                                Texture2D texture = Content.Load<Texture2D>(textureDir);
                                sprite = new Sprite(textureDir);
                                sprite.Initialize(Content);

                            }
                            catch (Exception e)
                            {
                                sprite = null;
                                int k = 0;
                            }*/
                        }

                        if (sprite != null)// && !HasObjInterceptor(sprite.TextureDir))
                        {
                            sprite.Rot = rot;
                            sprite.PosZ = layerDepth;
                            sprite.Position = new Vector2(posx, -posy);
                            sprite.Scale = new Vector2(width, height);

                            if (HasObjInterceptor(textureDir))
                            {
                                AddNode(ObjInterceptor(textureDir)(sprite, textureDir, part));
                            }
                            else
                            {
                                lsSprites.Add(sprite as Sprite);
                            }
                        }
                        
                        
                    }
                }
            }

            return lsSprites;
        }

        LoadSpriteIntercept ObjInterceptor(string dir)
        {
            for (int i = 0; i < objectLoadInterceptor.Count; i++)
                if (objectLoadInterceptor[i].Dir == dir)
                    return objectLoadInterceptor[i].interceptor;
            return null;
        }

        bool HasObjInterceptor(string dir)
        {
            for (int i = 0; i < objectLoadInterceptor.Count; i++)
                if (objectLoadInterceptor[i].Dir == dir)
                    return true;
            return false;
        }
    }
}
