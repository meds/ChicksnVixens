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
using Jabber.Util;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Jabber.Media
{
    public class AnimSprite : Sprite
    {
        public AnimSprite(string texturedir)
            : base(texturedir)
        {
        }

        public delegate void FrameChange(int prevFrame, int curFrame);
        public FrameChange FrameChanges = null;

        public override void Update(GameTime gameTime)
        {
            int oldFrame = m_Animations[Animation].curFrame;

            base.Update(gameTime);

            m_Animations[Animation].Update(gameTime);
            CurrentFrame = m_Animations[Animation].FrameName;

            int newFrame = m_Animations[Animation].curFrame;

            if (oldFrame != newFrame)
            {
                if (FrameChanges != null)
                    FrameChanges.Invoke(oldFrame, newFrame);
            }
        }
        public override void CreateFramesFromXML(string dir)
        {
            base.CreateFramesFromXML(dir);
            dir = "Content/" + dir + ".xml";
            XDocument doc = XDocument.Load(dir);
            foreach (XElement animPart in doc.Descendants())
            {
                if (animPart.Name == "Animations")
                {
                    foreach (XElement animSet in animPart.Descendants())
                    {
                        if (animSet.Name == "AnimSet")
                        {
                            AnimaionSet set = new AnimaionSet();
                            string name = "";

                            foreach (XElement animBit in animSet.Descendants())
                            {
                                if (animBit.Name == "Name")
                                {
                                    name = animBit.Value;
                                }
                                if (animBit.Name == "Speed")
                                {
                                    set.Speed = float.Parse(animBit.Value);
                                }

                                if (animBit.Name == "AnimFrames")
                                {
                                    string first = "";
                                    string last = "";
                                    bool regularFrame = false;
                                    bool frameLoop = false;
                                    foreach (XElement frame in animBit.Descendants())
                                    {
                                        if (frame.Name == "Frame")
                                        {
                                            foreach (XElement frameElem in frame.Descendants())
                                            {
                                                if (frameElem.Name == "Key")
                                                {
                                                    regularFrame = true;
                                                    set.animFrames.Add(frameElem.Value);
                                                }
                                            }
                                        }
                                        else if (frame.Name == "FrameStart")
                                        {
                                            first = frame.Value;
                                        }
                                        else if (frame.Name == "FrameEnd")
                                        {
                                            last = frame.Value;
                                        }
                                        else if (frame.Name == "Loop")
                                        {
                                            frameLoop = true;
                                        }
                                    }
                                    if (first != "" && last == "" || first == "" && last != "" || ((first != "" || last != "") && regularFrame))
                                    {
                                        throw new InvalidOperationException("error: start frame defined and last frame not, or a key frame was deifned along side a start and end frame, for shame!");
                                    }

                                    if (first != "" && last != "")
                                    {
                                        string start = first;
                                        string end = last;

                                        // match constant part and ending digits
                                        var matchstart = Regex.Match(start, @"^(.*?)(\d+)$");
                                        int numberstart = int.Parse(matchstart.Groups[2].Value);

                                        var matchend = Regex.Match(end, @"^(.*?)(\d+)$");
                                        int numberend = int.Parse(matchend.Groups[2].Value);

                                        // constant parts must be the same
                                        if (matchstart.Groups[1].Value != matchend.Groups[1].Value)
                                            throw new ArgumentException("");

                                        // create a format string with same number of digits as original
                                        string format = new string('0', matchstart.Groups[2].Length);

                                        for (int ii = numberstart; ii <= numberend; ++ii)
                                            set.animFrames.Add(matchstart.Groups[1].Value + ii.ToString(format));// Console.WriteLine(matchstart.Groups[1].Value + ii.ToString(format));
                                    }
                                    if (frameLoop)
                                    {
                                        for (int i = set.animFrames.Count - 2; i > 0; i--)
                                        {
                                            set.animFrames.Add(set.animFrames[i]);
                                        }
                                    }
                                }
                            }



                            m_Animations.Add(name, set);
                        }
                    }
                }

            }
        }

        private int FindTrailingNumber(string str)
        {
            string numString = "";
            int numTest;
            for (int i = str.Length - 1; i > 0; i--)
            {
                char c = str[i];
                if (int.TryParse(c.ToString(), out numTest))
                {
                    numString = c + numString;
                }
            }
            return int.Parse(numString);
        }


        public string Animation
        {
            get;
            set;
        }

        class AnimaionSet
        {
            public float Speed = 1.0f;
            public List<string> animFrames = new List<string>();
            public int curFrame = 0;

            public string FrameName
            {
                get
                {
                    return animFrames[curFrame];
                }
            }

            float timer = 0;
            public void Update(GameTime gameTime)
            {
                timer += gameTime.ElapsedGameTime.Milliseconds / 1000.0f * Speed;
                if (timer >= 1.0f)
                {
                    timer = 0.0f;
                    ++curFrame;

                    if (curFrame >= animFrames.Count)
                    {
                        curFrame = 0;
                    }

                }
            }
        }
        Dictionary<string, AnimaionSet> m_Animations = new Dictionary<string, AnimaionSet>();

    }
}
