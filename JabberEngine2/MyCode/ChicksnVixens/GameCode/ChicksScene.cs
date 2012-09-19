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

using System.Xml.Linq;

using System.Globalization;

using Jabber.Media;
using Jabber.Util;
using Jabber.Physics;
#if WINDOWS || WINDOWS_PHONE
using System.Windows;
#endif
using Jabber.Scene;
using ChicksnVixens.Screens;
namespace ChicksnVixens
{
    public class ThisGamesScene : GameScene
    {
        GameplayScreen screen;
        public ThisGamesScene(GameplayScreen screen, JabWorld world,
        ChicksScene chicksScene, ContentManager content)
            : base(world, content)
        {
            this.screen = screen;
            this.chicksScene = chicksScene;


            AddSpriteLoadInterceptor("textures\\fan", LoadFan);
            AddSpriteLoadInterceptor("textures\\donut", LoadDonut);
            AddSpriteLoadInterceptor("textures\\donutcase", LoadDonutCase);

            AddSpriteLoadInterceptor("textures\\basketball", LoadBasketBall);
            AddSpriteLoadInterceptor("textures\\Physical\\basketball", LoadBasketBall);

            AddSpriteLoadInterceptor("textures\\tyre", LoadTyre);
            AddSpriteLoadInterceptor("textures\\Physical\\tyre", LoadTyre);
        }

        BaseSprite LoadTyre(BaseSprite sprite, string texturedir, XElement part)
        {
            BasketBall b = new BasketBall();
            b.IsTyre = true;
            b.Width = sprite.Width;// *sprite.ScaleX;
            b.Height = sprite.Height;// *sprite.ScaleY;
            b.Dimension = sprite.Dimension;
            b.Scale = sprite.Scale;
            b.World = world;
            b.Initialize(Content);
            b.Position = sprite.Position;
            return b;
        }

        BaseSprite LoadBasketBall(BaseSprite sprite, string texturedir, XElement part)
        {
            BasketBall b = new BasketBall();
            b.Width = sprite.Width;// *sprite.ScaleX;
            b.Height = sprite.Height;// *sprite.ScaleY;
            b.Dimension = sprite.Dimension;
            b.Scale = sprite.Scale;
            b.World = world;
            b.Initialize(Content);
            b.Position = sprite.Position;
            return b;
        }

        BaseSprite LoadDonutCase(BaseSprite sprite, string texturedir, XElement part)
        {
            DonutCase c = new DonutCase(this);
            c.Scale = sprite.Scale;
            c.Initialize(Content);
            c.Position = sprite.Position;
            if(sprite.Rot != 0)
                c.Rot = sprite.Rot;
            return c;
        }

        BaseSprite LoadDonut(BaseSprite sprite, string texturedir, XElement part)
        {
            Donut f = new Donut(this);
            f.Initialize(Content);
            f.Rot = sprite.Rot;
            f.Position = sprite.Position;
            return f;
        }

        BaseSprite LoadFan(BaseSprite sprite, string texturedir, XElement part)
        {
            Fan f = new Fan(world);
            f.Initialize(Content);
            f.Rot = sprite.Rot;
            f.Position = sprite.Position;
            return f;
        }

        ChicksScene chicksScene;
        public Vector2 startPos = Vector2.Zero;
        void SetStartPos(XElement elements)
        {
            var items = elements.Descendants("Items");
            var element = elements.Descendants("CustomProperties").Descendants("Property");

            items = elements.Descendants("Items");
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

                        Camera.Get.PosX = posx;
                        Camera.Get.PosY = -posy;
                        startPos = Camera.Get.Position;
                        Camera.Get.Position = startPos;

                        return;
                    }
                }
            }
        }

        public override bool LoadGScene(string xmldir)
        {
            bool b = base.LoadGScene(xmldir);
            if (!b)
            {
                return b;
            }
            SetChickensAndCountry(xmldir);

            XDocument doc = null;

            try
            {
                doc = XDocument.Load(xmldir);
            }
            catch (Exception e)
            {
                {
                    using (IsolatedStorageFile isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (isolatedStorageFile.FileExists(xmldir))
                        {
                            using (IsolatedStorageFileStream fileStream
                                = isolatedStorageFile.OpenFile(xmldir, FileMode.Open))
                            {
                                doc = XDocument.Load(fileStream);
                                fileStream.Close();
                            }
                        }
                    }
                }
            }

            var layers = doc.Document.Descendants("Layer");
            foreach (var layer in layers)
            {
                try
                {
                    foreach (XAttribute attribute in layer.Attributes())
                    {
                        string hmm = attribute.Value;
                        XName baaa = attribute.Name;
                    }
                    string name = layer.Attribute("Name").Value;
                    if (name.IndexOf("#") > 0)
                        name = name.Substring(0, name.IndexOf("#"));

                    if (name == "RigidBreakableBodies")
                    {
                        LoadBreakableBody(layer);
                    }
                    else if (name == "StartPos")
                    {
                        SetStartPos(layer);
                    }
                    else if (name == "Fox")
                    {
                        LoadFoxie(layer);
                    }
                    else if (name == "TNT")
                    {
                        LoadTNT(layer);
                    }
                }
                catch (Exception E)
                {
#if WINDOWS || WINDOWS_PHONE
                    System.Windows.MessageBox.Show(E.ToString());
#endif
                }
            }


            //SetRightMaxPos();

            return true;
        }

        public string countryName = "australia";
        public List<int> ToFire = new List<int>();
        void SetChickensAndCountry(string dir)
        {
            XDocument doc = XDocument.Load(dir);

            var properties = doc.Document.Descendants("Level");
            var element = doc.Document.Descendants("CustomProperties").Descendants("Property");

            foreach (var prop in element)
            {
                if (prop.Parent.Parent.Name == "Level")
                {
                    if (prop.Attribute("Name") != null)
                    {
                        string propName = prop.Attribute("Name").Value;
                        if (prop.Attribute("Name").Value == "Chicks")
                        {
                            string full = prop.Descendants("string").First<XElement>().Value;
                            string[] each = full.Split('+');

                            for (int i = 0; i < each.Count<string>(); i++)
                            {
                                string thischick = each[i].ToString();
                                ToFire.Add(int.Parse(thischick));
                            }
                            ToFire.Reverse();
                        }
                        else if (prop.Attribute("Name").Value == "WorldDir")
                        {
                            countryName = prop.Descendants("string").First<XElement>().Value.ToLowerInvariant();
                            //countryName = "paris";// countryName.ToLower();
                        }
                    }
                }
            }
        }

        void LoadTNT(XElement elements)
        {
            List<BaseSprite> l = CreateBaseSprites(elements);
            List<JabActor> b = CreateBodies(elements);

            for (int i = 0; i < l.Count; i++)
            {
                Sprite s = new Sprite("misc");
                s.Initialize(Content);
                s.CreateFramesFromXML("misc_frames");
                s.CurrentFrame = "tnt";
                s.ResetDimensions();
                s.Position = l[i].Position;
                s.Rot = l[i].Rot;
                
                TNT t = new TNT(this);
                t.World = World;
                t.Body = World.CreateBox(s.Dimension * l[i].Scale, s.Position, JabActor.BodyType.DYNAMIC);
                t.Body.Rot = s.Rot;
                t.Body.Mass = 2.0f;
                t.Sprite = s;
                t.DoDimensions = false;
                t.PhysicsPosition = true;
                t.PhysicsRotate = true;
                t.Initialize(Content);

                AddNode(t);
            }
        }

        void LoadFoxie(XElement elements)
        {
            List<BaseSprite> l = CreateBaseSprites(elements);

            for (int i = 0; i < l.Count; i++)
            {
				try
				{
                    Fox f = new Fox(screen, chicksScene, l[i].Position, l[i].Rot);
                	f.Initialize(Content);
                    f.WearHat = true;
					Nodes.Add(f);
				}
				catch(Exception e)
				{
                    System.Windows.MessageBox.Show(e.ToString());
				}
            } 
        }

       
        float furthestRight = float.MinValue;
        public float GetRightMaxPos()
        {
            if (furthestRight != float.MinValue)
            {
                return furthestRight;
            }
            float carFurthestRight = float.MinValue;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is PhysicSprite)
                {
                    PhysicSprite s = nodes[i] as PhysicSprite;
                    if (s.Body.BodyState == JabActor.BodyType.DYNAMIC)
                    {
                        float rightMostRect = s.GetRectangle().Right;
                        rightMostRect = s.PosX;// Camera.Get.ScreenToWorld(new Vector2(rightMostRect, 0)).X;

                        if (rightMostRect > carFurthestRight)
                        {
                            carFurthestRight = rightMostRect;
                        }
                    }
                }
            }

            furthestRight = carFurthestRight;
            //carFurthestRight += 1000;
            return carFurthestRight;
        }

        public void LoadBreakableBody(XElement elements)
        {
            List<Sprite> sprites = CreateSprites(elements);
            List<JabActor> bodies = new List<JabActor>();
            for (int i = 0; i < sprites.Count; i++)
            {
                sprites[i].Width = 20;
                sprites[i].Height = 100;

                JabActor act = World.CreateBox(sprites[i].Dimension * sprites[i].Scale, sprites[i].Position, JabActor.BodyType.DYNAMIC);
                act.Rot = sprites[i].Rot;
                bodies.Add(act);
            }
            // LoadBreakableBodies(elements);



            for (int i = 0; i < sprites.Count; i++)
            {
                string name = (sprites[i]).CurrentFrame;
                BreakableBody s = null;

                if (name == "wood")
                    s = new BreakableBody(BreakableBody.BodyMaterial.WOOD, this);
                else if (name == "cement")
                    s = new BreakableBody(BreakableBody.BodyMaterial.CONCRETE, this);
                else if (name == "glass")
                    s = new BreakableBody(BreakableBody.BodyMaterial.GLASS, this);



                s.World = World;
                s.Body = bodies[i];
                s.Sprite = sprites[i];
                s.Initialize(Content);

                AddNode(s);
            }
        }
        public Vector2 CannonPos
        {
            get
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i] is Cannon)
                    {
                        return nodes[i].Position;
                    }
                }

                return Vector2.Zero;
            }
        }
    }



    public class ChicksScene : ThisGamesScene
    {
        public ChicksScene(GameplayScreen screen, FarWorld world, ContentManager content):base(screen, world, null, content){}

       
        public override bool LoadGScene(string xmldir)
        {
            return base.LoadGScene(xmldir);
        }

        public Chicken ActiveMotion
        {
            get
            {
                float fastest = float.MinValue;
                Chicken ret = null;
                for (int i = 0; i < GetChicks().Count; i++)
                {
                    if (GetChicks()[i].Body.LinearVelocity.Length() > fastest)
                    {
                        fastest = GetChicks()[i].Body.LinearVelocity.Length();

                        if(fastest > 0.5f)
                            ret = GetChicks()[i];
                    }
                }


                return ret;
            }
        }

        public Chicken LastChicken
        {
            get
            {
                if(GetChicks().Count > 0)
                    return GetChicks()[GetChicks().Count - 1];

                return null;
            }
        }

        public Chicken ActiveChicken
        {
            get
            {
                List<Chicken> chicks = GetChicks();
                for (int i = 0; i < chicks.Count; i++)
                {
                    if (chicks[i].IsActive)
                    {
                        return chicks[i];
                    }
                }

                return null;
            }
        }

        public List<Chicken> GetChicks()
        {
            List<Chicken> ret = new List<Chicken>();
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i] is Chicken)
                {
                    ret.Add(Nodes[i] as Chicken);
                }
            }


            return ret;
        }

        public bool ActiveChickens
        {
            get
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (Nodes[i] is Chicken)
                    {
                        bool active = (Nodes[i] as Chicken).IsActive;
                        if (active)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }
    }
}