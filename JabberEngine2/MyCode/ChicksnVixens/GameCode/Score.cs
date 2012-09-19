using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jabber.Physics;
using Jabber.Media;
using Jabber.Scene;
using Microsoft.Xna.Framework;
using Jabber.Util;
using ChicksnVixens;
using Jabber;
using Jabber.Util.UI;
using Jabber.Media;

namespace ChicksnVixens
{
    public class SingleScore : MediaTextDrawer
    {
        public SingleScore(Score board)
            : base("ui/ui", "ui/ui_frames")
        {
            Text = "12345";
            Layer = SpriteLayer.LAYER0;

            this.board = board;

            UniformScale = 0.1f;
        }


        public override void Draw()
        {
            base.Draw();
        }


        float scaleTimer = 0.0f;
        public override void Update(GameTime dt)
        {
            base.Update(dt);
            
            /*
            if (scaleUp)
            {
                scaleTimer += gttf(dt) * 6.0f;
                if (scaleTimer >= 1.5f)
                {
                    scaleUp = false;
                }
            }
            else
            {
                scaleTimer -= gttf(dt);
            }
            Colour = new Color(scaleTimer, scaleTimer, scaleTimer, scaleTimer);

            if (scaleTimer <= 0 && !scaleUp)
            {
                scaleTimer = 0;
                RaiseFlag(Flags.DELETE);
            }
            Colour = Color.White * scaleTimer;
            */

            scaleTimer += gttf(dt) * 5.0f;
            if (scaleTimer < 1.0f)
            {
                UniformScale = scaleTimer * 2.0f;
            }
            else if (scaleTimer < 4.0f)
            {
                UniformScale = 2.0f;
            }
            else
            {
              /*  Vector2 targetPos = board.ScaledPos;// +new Vector2(board.text.Width * board.text.ScaleX / 2.0f, 0);
                Position = JabMath.MoveTowards(Position, targetPos, gttf(dt) * 10);
                UniformScale = board.ScaledScale;// JabMath.MoveTowards(UniformScale, board.text.UniformScale, gttf(dt));

                float a = Colour.A / 255.0f;
                a -= gttf(dt);
                Colour = new Color(a, a, a, a);

                if (a == 0)
                {
                    RaiseFlag(Flags.DELETE);
                }
                if ((Position - targetPos).Length() < 5)
                {
                    RaiseFlag(Flags.DELETE);
                }
               * */

                UniformScale = JabMath.MoveTowards(UniformScale, 0.0f, gttf(dt) * 4.0f, 0.001f);
                if (UniformScale == 0.0f)
                {
                    RaiseFlag(Flags.DELETE);
                }
            }
        }
        Score board;
    }

    public class Score : MenuObj
    {
        public Score(GameScene scene, string country, int level)
            : base("ui/ui")
        {
            Layer = SpriteLayer.UILAYER0;
            EventManager.Get.RegisterListner(this);

            this.scene = scene;

            Country = country;
            this.level = level;
        }
        string Country = "";
        int level = 0;
        MenuObj highscore;
        MediaTextDrawer curscore;
        MenuObj scoretext;

        MediaTextDrawer textScore = new Jabber.Media.MediaTextDrawer("ui/ui", "ui/ui_frames");

        GameScene scene;
        public override void ProcessEvent(Event ev)
        {
            base.ProcessEvent(ev);

            if (ev is BreakableDestroyed && !(ev is BreakableHit))
            {
                SingleScore s = new SingleScore(this);
                int scoreToAdd = 0;
                switch ((ev as BreakableDestroyed).Broken)
                {
                    case BreakableBody.BodyMaterial.WOOD:
                        scoreToAdd += 100;
                        break;
                    case BreakableBody.BodyMaterial.GLASS:
                        scoreToAdd += 50;
                        break;
                    case BreakableBody.BodyMaterial.CONCRETE:
                        scoreToAdd += 200;
                        break;
                }
                score += scoreToAdd;
                s.Text = scoreToAdd.ToString();
                s.Initialize(BaseGame.Get.Content);
                s.Width = s.Height = 10;
                s.Position = ev.Position;
                scene.AddNode(s);
            }
        }

        public int score = 0;
        public override void Initialize(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            base.Initialize(Content);
            /*
            text.Initialize(Content);
            text.Text = "Highscore: ";
            text.Handle = SpriteHandle.CENTERLEFT;
            */


            textScore = new Jabber.Media.MediaTextDrawer("ui/ui", "ui/ui_frames");
            textScore.Initialize(Content);
            textScore.Text = "10000";
            textScore.Handle = SpriteHandle.CENTERLEFT;


            PosX = 0.15f * BaseGame.Get.BackBufferWidth - 15 * ScaleFactor;
            PosY = 0.355f * BaseGame.Get.BackBufferHeight;

            UniformScale = 1.0f * ScaleFactor;


            curscore = new MediaTextDrawer("ui/ui", "ui/ui_frames");
            curscore.Initialize(Content);
            curscore.CreateFramesFromXML("ui/ui_frames");
           // curscore.PosX = 0.15f * BaseGame.Get.BackBufferWidth - 15 * ScaleFactor;
         //   curscore.PosY = 0.4f * BaseGame.Get.BackBufferHeight;
            curscore.UniformScale = ScaleFactor;
            curscore.Text = "0123456789";
            curscore.Handle = SpriteHandle.CENTERLEFT;

            highscore = new MenuObj("ui/ui");
            highscore.Initialize(Content);
            highscore.CreateFramesFromXML("ui/ui_frames");
            highscore.CurrentFrame = "highscore";
            highscore.ResetDimensions();

            highscore.PosX = 0.35f * BaseGame.Get.BackBufferWidth - 15 * ScaleFactor;
            highscore.PosY = 0.455f * BaseGame.Get.BackBufferHeight;
            highscore.UniformScale = ScaleFactor * 0.75f;



            scoretext = new MenuObj("ui/ui");
            scoretext.Initialize(Content);
            scoretext.CreateFramesFromXML("ui/ui_frames");
            scoretext.CurrentFrame = "score";
            scoretext.ResetDimensions();
            scoretext.PosX = 0.35f * BaseGame.Get.BackBufferWidth - 15 * ScaleFactor;
            scoretext.PosY = 0.45f * BaseGame.Get.BackBufferHeight;
            scoretext.UniformScale = ScaleFactor * 0.75f;
            scoretext.Handle = SpriteHandle.CENTERRIGHT;
        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);

            highscore.Update(dt);

            scoretext.PosY = highscore.PosY - highscore.Height * highscore.UniformScale;
            scoretext.PosX = highscore.Position.X + (highscore.Width*highscore.ScaleX/2.0f);


            curscore.Position = scoretext.ScaledPos;
            curscore.PosX += (5 *ScaleFactor) / Camera.Get.UniformWorldScale;
            curscore.PosY -= (1 * ScaleFactor) / Camera.Get.UniformWorldScale;
            curscore.UniformScale = scoretext.ScaledScale * 0.45f;
            curscore.Text = score.ToString();


            textScore.Position = scoretext.ScaledPos;
            textScore.PosX += (5 * ScaleFactor) / Camera.Get.UniformWorldScale;
            textScore.PosY += (scoretext.Height * ScaleFactor) / Camera.Get.UniformWorldScale;
            textScore.UniformScale = highscore.ScaledScale * 0.45f;
            textScore.Text = ChicksnVixensGame.Get.GetLevelState(Country, level).Score.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }


        public override void Draw()
        {
            //text.Position = ScaledPos;
            //text.UniformScale = ScaledScale;
          //  text.Draw();

            highscore.Draw();
            scoretext.Draw();
            curscore.Colour = Color.Yellow;
            curscore.Draw();
            textScore.Draw();
            //base.Draw();
        }
        public TextDrawer text;
    }
}
