using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System.Xml.Linq;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using Jabber.Util;
//using Jabber.GameStateManagement;

namespace Jabber.Media
{
    public class Sprite : BaseSprite
    {
        public override SpriteHandle Handle
        {
            set
            {
                handle = value;
                UpdateOrigin();
            }
            get
            {
                return handle;
            }
        }

        public override bool IsVisible()
        {
            return true;// base.IsVisible();
        }

        public override Vector2 CustomOrigin
        {
            get
            {
                return base.CustomOrigin;
            }
            set
            {
                base.CustomOrigin = value;
                UpdateOrigin();
            }
        
        }
        /*
        public override float UniformScale
        {
            get
            {
                return base.UniformScale;
            }
            set
            {
                base.UniformScale = value;
                UpdateOrigin();
            }
        }
        */
        public void UpdateOrigin()
        {
            Origin = Vector2.Zero;
            if (Handle != SpriteHandle.CUSTOM)
                Origin = GetOriginOffset(GetFrame().Width, GetFrame().Height, Handle);// //Vector2.Zero;
            else
                Origin += CustomOrigin;
        }

        public void ResetDimensions()
        {
            Scale = Vector2.One;
            if (currentFrame == "")
            {
                Width = Texture.Width;
                Height = Texture.Height;
            }
            else
            {
                Width = m_Frames[currentFrame].Width;
                Height = m_Frames[currentFrame].Height;
            }
        }


        public override Vector2 Scale
        {
            get
            {
                return base.Scale;
            }
            set 
            {
                base.Scale = value;
                UpdateOrigin();
            }
        }

        protected static XDocument DocumentLoad(string dir)
        {
            for(int i = 0; i < docs.Count; i++)
            {
                if (docs[i].dir == dir)
                    return docs[i].doc;
            }
            XDocs d = new XDocs();
            d.dir = dir;
            d.doc = XDocument.Load(dir);
            docs.Add(d);
            return d.doc;
        }

        class XDocs
        {
            public XDocument doc;
            public string dir;
        }
        static List<XDocs> docs = new List<XDocs>();

        class Frames
        {
            public Dictionary<string, Frame> m_Frames = new Dictionary<string, Frame>();
            public string dir = "";
        }
        static List<Frames> LoadedFrames = new List<Frames>();

        static Dictionary<string, Frame> LoadFrames(string dir)
        {
            for (int i = 0; i < LoadedFrames.Count; i++)
            {
                if (LoadedFrames[i].dir == dir)
                    return LoadedFrames[i].m_Frames;
            }

            Frames frame = new Frames();
            string name = "";
            XDocument doc = XDocument.Load(dir);
            foreach (XElement animParts in doc.Descendants())
            {
                string s = animParts.Name.ToString();
                if (animParts.Name == "Frames" || animParts.Name == "Asset")
                {
                    foreach (XElement key in animParts.Descendants())
                    {
                        if (key.Name == "Key")
                        {
                            if (name != "")
                            {
                                throw new InvalidOperationException("Error: We have a name that's not been set yet");
                            }
                            name = key.Value;
                        }
                        else if (key.Name == "Value")
                        {
                            if (name == "")
                            {
                                throw new InvalidOperationException("Error: We have no name set yet");
                            }
                            Frame f = new Frame();
                            List<string> parts = key.Value.Split(' ').ToList<string>();
                            f.Position.X = float.Parse(parts[0]);
                            f.Position.Y = float.Parse(parts[1]);

                            f.Dimensions.X = float.Parse(parts[2]);
                            f.Dimensions.Y = float.Parse(parts[3]);

                            frame.m_Frames.Add(name, f);

                            name = "";
                        }
                    }
                }
            }
            frame.dir = dir;
            LoadedFrames.Add(frame);
            return frame.m_Frames;
        }

        public virtual void CreateFramesFromXML(string dir)
        {
            dir = "Content/" + dir + ".xml";

            CurrentFrame = "";
            m_Frames.Clear();

            //XDocument doc = DocumentLoad(dir);// XDocument.Load(dir);

            m_Frames = new Dictionary<string, Frame>(LoadFrames(dir));

            /*
            string name = "";
            foreach (XElement animParts in doc.Descendants())
            {
                string s = animParts.Name.ToString();
                if (animParts.Name == "Frames" || animParts.Name == "Asset")
                {
                    foreach (XElement key in animParts.Descendants())
                    {
                        if (key.Name == "Key")
                        {
                            if (name != "")
                            {
                                throw new InvalidOperationException("Error: We have a name that's not been set yet");
                            }
                            name = key.Value;
                        }
                        else if (key.Name == "Value")
                        {
                            if (name == "")
                            {
                                throw new InvalidOperationException("Error: We have no name set yet");
                            }
                            Frame f = new Frame();
                            List<string> parts = key.Value.Split(' ').ToList<string>();
                            f.Position.X = float.Parse(parts[0]);
                            f.Position.Y = float.Parse(parts[1]);

                            f.Dimensions.X = float.Parse(parts[2]);
                            f.Dimensions.Y = float.Parse(parts[3]);

                            m_Frames.Add(name, f);

                            name = "";
                        }
                    }
                }
                else if (name == "Animations")
                {

                }
            }*/
        }
        
        public Sprite(string texturedir)
        {
            textureDir = texturedir;
        }

        public override void Initialize(ContentManager Content)
        {
            Texture = Content.Load<Texture2D>(textureDir);
            Width = Texture.Width;
            Height = Texture.Height;

            Handle = SpriteHandle.CENTER;
        }

        public Vector2 Origin = Vector2.Zero;

        public override float Width
        {
            set
            {
                base.Width = value;
                UpdateOrigin();
            }
            get
            {
                return base.Width;
            }
        }
        public override float Height
        {
            set
            {
                base.Height = value;
                UpdateOrigin();
            }
            get
            {
                return base.Height;
            }
        }

        void DrawRectangle()
        {
#if WINDOWS || WINDOWS_PHONE
            SpriteBatch.End();
            SpriteBatch.Begin();
            if (dummyTexture == null)
            {
                dummyTexture = new Texture2D(BaseGame.Get.GraphicsDevice, 1, 1);
                dummyTexture.SetData(new Color[] { Color.White });
            }

            if (DrawIn == DrawSpace.WORLDSPACE)
            {
                Rectangle rect = new Rectangle((int)(GetRectangle().X * BaseGame.Get.BackBufferWidth), (int)(GetRectangle().Y * BaseGame.Get.BackBufferHeight),
                                                   (int)(GetRectangle().Width * BaseGame.Get.BackBufferWidth), (int)(GetRectangle().Height * BaseGame.Get.BackBufferHeight));

                BaseGame.Get.SpriteBatch.Draw(dummyTexture, rect, Color.Red);
            }
            else
            {
                Rectangle rect = new Rectangle((int)(GetRectangle().X * BaseGame.Get.BackBufferWidth), (int)(GetRectangle().Y * BaseGame.Get.BackBufferHeight),
                                                (int)(GetRectangle().Width * BaseGame.Get.BackBufferWidth), (int)(GetRectangle().Height * BaseGame.Get.BackBufferHeight));

                BaseGame.Get.SpriteBatch.Draw(dummyTexture, rect, Color.Black);
            }
            SpriteBatch.End();
            BaseGame.Get.Begin();
#endif
        }

        public override void Draw()
        {
            Rectangle rect = GetFrame();

            float scaleX = Width * Scale.X / rect.Width;
            float scaleY = Height * Scale.Y / rect.Height;

            Vector2 camPos = Camera.Get.Position;
            camPos.Y = -1;
            camPos *= Vector2.One - CamPosScale;

            SpriteEffects effect = SpriteEffects.None;
            if (Effect == SpriteEffect.FLIPHORIZONTAL)
                effect = SpriteEffects.FlipHorizontally;

            Vector2 origin = Origin;
            SpriteBatch.Draw(Texture, new Vector2(PosX, -PosY) + camPos, GetFrame(), Colour, Rot, origin, new Vector2(scaleX, scaleY), effect, 0);
        }
		
#if WINDOWS || WINDOWS_PHONE
        Texture2D dummyTexture;
#endif
        public void CreateFramesFromFile(string meh)
        {
        }

        public void AddFrame(string name, Vector2 pos, Vector2 dim, Vector2 offset, bool proportional)
        {
            Frame f = new Frame();
            f.Position = pos;
            f.Dimensions = dim;
            f.Offset = offset;
            f.IsProportional = proportional;

            m_Frames.Add(name, f);
        }

        public string CurrentFrame
        {
            get
            {
                return currentFrame;
            }
            set
            {
                currentFrame = value;
            }
        }
        string currentFrame = "";
        public Rectangle GetFrame()
        {
            Rectangle r = new Rectangle();
            if (CurrentFrame == "")
            {
                r.Width = Texture.Width;
                r.Height = Texture.Height;
                r.X = 0;
                r.Y = 0;
            }
            else
            {
                Frame f = m_Frames[CurrentFrame];
                r.Width = (int)f.Width;
                r.Height = (int)f.Height;
                r.X = (int)f.Position.X;
                r.Y = (int)f.Position.Y;
            }
            return r;
        }

        protected class Frame
        {
            public float Width
            {
                get
                {
                    return Dimensions.X;
                }
            }
            public float Height
            {
                get
                {
                    return Dimensions.Y;
                }
            }
            public Vector2 Position;
            public Vector2 Dimensions;
            public Vector2 Offset = Vector2.Zero;
            public bool IsProportional = false;
        }
        protected Dictionary<string, Frame> m_Frames = new Dictionary<string, Frame>();

        public string TextureDir
        {
            get
            {
                return textureDir;
            }
        }

        string textureDir = "";
        public Texture2D Texture;

        Vector2 topLeftPosition = Vector2.Zero;
    }
}