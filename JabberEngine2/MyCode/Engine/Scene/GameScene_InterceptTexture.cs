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
        public class SingleTextureToMassTexture
        {
            public string textureDir;
            public string newTextureDir;
            public string newTextureDirXML;
            public string FrameName;
        }
        List<SingleTextureToMassTexture> SingleTextureToMass = new List<SingleTextureToMassTexture>();


        public delegate BaseSprite LoadSpriteIntercept(BaseSprite dir, string texturedir, XElement details);
        class SpriteIntercept
        {
            public SpriteIntercept(string dir, LoadSpriteIntercept intercept)
            {
                this.Dir = dir;
                this.interceptor = intercept;
            }
            public string Dir;
            public LoadSpriteIntercept interceptor;
        }
        List<SpriteIntercept> objectLoadInterceptor = new List<SpriteIntercept>();
        public void AddSpriteLoadInterceptor(string texturedir, LoadSpriteIntercept intercept)
        {
            objectLoadInterceptor.Add(new SpriteIntercept(texturedir, intercept));
        }

        public void AddTextureLoadInterceptor(string textureDir, string newTextureDir, string newTextureDirXML, string FrameName)
        {
            SingleTextureToMassTexture s = new SingleTextureToMassTexture();
            s.textureDir = textureDir;
            s.newTextureDir = newTextureDir;
            s.newTextureDirXML = newTextureDirXML;
            s.FrameName = FrameName;

            SingleTextureToMass.Add(s);
        }

        public SingleTextureToMassTexture GetInterception(string texturedir)
        {
            if (texturedir.Contains("cow"))
            {
                int k = 0;
            }
            for (int i = 0; i < SingleTextureToMass.Count; i++)
            {
                if (SingleTextureToMass[i].textureDir == texturedir)
                {
                    return SingleTextureToMass[i];
                }
            }
            return null;
        }
    }
}
