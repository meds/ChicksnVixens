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
#if WINDOWS || WINDOWS_PHONE
using System.Windows;
#endif
//using Jabber.J3D;


namespace Jabber.Scene
{
    public partial class GameScene : JabJect
    {
        /// <summary>
        /// An event which takes in a BaseSprite to be added to the scene, it uses its own Content
        /// manager to initialize the BaseSprite
        /// </summary>
        public class AddNodeEvent : Event
        {
            public AddNodeEvent(BaseSprite objToAdd)
            {
                Obj = objToAdd;
            }

            public BaseSprite Obj { get; set; }
        }
        public GameScene(JabWorld world, ContentManager content)
        {
            EventManager.Get.RegisterListner(this);
            this.world = world;
            this.Content = content;
            RaiseFlag(Flags.ACCEPTINPUT);
        }

        public JabWorld World
        {
            get { return world; }
        }
        public void AddNode(BaseSprite sprite)
        {
            nodes.Add(sprite);

            nodes.Sort();
        }

        public void RemoveNode(BaseSprite sprite)
        {
            nodes.Remove(sprite);
        }

        class CustomTypeLoader
        {
            public CustomTypeLoader(LoadNode func, string name)
            {
                this.name = name;
                Loader = func;
            }
            public LoadNode Loader;
            public string name;
        };
        List<CustomTypeLoader> customTypes = new List<CustomTypeLoader>();
        public delegate void LoadNode(XElement elements, GameScene scene);
        public void AddCustomNode(string name, LoadNode node)
        {
            customTypes.Add(new CustomTypeLoader(node, name));
        }

        public void LoadRigidBodies(XElement elements)
        {
            List<Sprite> sprites = CreateSprites(elements);
            List<JabActor> bodies = CreateBodies(elements);

            for (int i = 0; i < sprites.Count; i++)
            {
                PhysicSprite s = new PhysicSprite();
                s.Body = bodies[i];
                s.Sprite = sprites[i];
                s.Body.UserData = s;
                AddNode(s);
            }
        }
                
        public void LoadSprites(XElement elements)
        {
            List<Sprite> sprites = CreateSprites(elements);
            for (int i = 0; i < sprites.Count; i++)
            {
                AddNode(sprites[i]);
            }
        }
        public void LoadTerrain(XElement elements)
        {
            Load3DShapes(elements);
        }

        public virtual bool LoadGScene(string xmldir)
        {
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
                    {
                        name = name.Substring(0, name.IndexOf("#"));
                    }
                    if (name == "Terrain")
                    {
                        LoadTerrain(layer);
                    }
                    else if (name == "RigidBodies")
                    {
                        LoadRigidBodies(layer);
                    }
                    else if (name == "Sprite")
                    {
                        LoadSprites(layer);
                    }
                    else if (name == "ShapeRigidBodies")
                    {
                  //      LoadShapeRigidBodies(layer);
                    }
                   /* else if (name == "RigidBreakableBodies")
                    {
                        LoadRigidBodies(layer);
                    }*/
                    else
                    {
                        for (int i = 0; i < customTypes.Count; i++)
                        {
                            if (customTypes[i].name == name)
                            {
                                customTypes[i].Loader(layer, this);
                            }
                        }
                    }
                  //  if (ItemCreateCallBack != null)
                    {
                  //      ItemCreateCallBack.Invoke(Nodes[nodes.Count - 1]);
                    }
                }
                catch (Exception E)
                {
#if WINDOWS || WINDOWS_PHONE
                    System.Windows.MessageBox.Show(E.ToString());
#endif
                }
            }

            return true;
        }


        public bool LoadScene(string xmldir)
        {
            XDocument doc = XDocument.Load(xmldir);
            {
                XName customDir = XName.Get("CustomType");
                var definitions = doc.Document.Descendants(customDir);
                foreach (var def in definitions)
                {
                    for (int i = 0; i < customTypes.Count; i++)
                    {
                        XName name = XName.Get(customTypes[i].name);
                      //  customTypes[i].Loader(def.Descendants(name), this);
                    }
                }
            }
            {
                var rigidbodies = doc.Document.Descendants("RigidBodies");
                foreach (var rigidset in rigidbodies)
                {
                    var definitions = rigidset.Descendants("Geometry");
                    foreach (var geomdef in definitions)
                    {
                        string texturedir = geomdef.Attribute("Texture").Value;
                        bool dynamic = geomdef.Attribute("Dynamic").Value == "true";
                        float width = float.Parse(geomdef.Attribute("Width").Value, CultureInfo.InvariantCulture);
                        float height = float.Parse(geomdef.Attribute("Height").Value, CultureInfo.InvariantCulture);

                        float posx = float.Parse(geomdef.Attribute("PosX").Value, CultureInfo.InvariantCulture);
                        float posy = float.Parse(geomdef.Attribute("PosY").Value, CultureInfo.InvariantCulture);

                        string type = geomdef.Attribute("ActorType").Value;
                        if (type == "Box")
                        {
                            PhysicSprite sprite = new PhysicSprite(new Vector2(width, height), new Vector2(posx, posy), dynamic, world, texturedir);
                            AddNode(sprite);
                        }
                    }
                }
            }

            return true;
        }
        public override void ProcessEvent(Event ev)
        {
            if (ev is AddNodeEvent)
            {
                BaseSprite sprite = (ev as AddNodeEvent).Obj;
                sprite.Initialize(Content);
                AddNode(sprite);
            }
            base.ProcessEvent(ev);
        }
        public override void UnloadContent()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].UnloadContent();
            }
            base.UnloadContent();

            EventManager.Get.UnregisterListner(this);

            world.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].CheckFlag(Flags.DELETE))
                {
                    nodes[i].UnloadContent();
                    nodes.Remove(nodes[i]);
                    --i;
                }
                else
                {
                    nodes[i].Update(gameTime);
                }
            }
            if(DoWorldUpdateDraw)
                world.Update(gameTime);
            base.Update(gameTime);
        }

        public bool DoWorldUpdateDraw = false;

        public override void Draw()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].IsVisible())//Camera.Get.IsVisible(nodes[i]))
                {
                    nodes[i].Draw();
                }
            }
   
            base.Draw();
            if(DoWorldUpdateDraw)
                world.Draw();
        }

        public List<BaseSprite> Nodes
        {
            get { return nodes; }
        }
        protected List<BaseSprite> nodes = new List<BaseSprite>();
        protected ContentManager Content;
        protected JabWorld world;




        #region inputs
        public override void OnDrag(Vector2 lastPos, Vector2 thispos)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Nodes[i].OnDrag(lastPos, thispos);
                }
            }
        }

        public override void OnTap(Vector2 pos)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Nodes[i].OnTap(pos);
                }
            }
        }
        public override void OnRelease(Vector2 pos)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Nodes[i].OnRelease(pos);
                }
            }
        }
        public override void OnMove(Vector2 pos)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Nodes[i].OnMove(pos);
                }
            }
        }
        public override void OnPress(Vector2 pos)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].CheckFlag(Flags.ACCEPTINPUT))
                {
                    Nodes[i].OnPress(pos);
                }
            }
        }
        #endregion
    }
}
