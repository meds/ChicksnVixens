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

namespace ChicksnVixens
{
    public class DonutScore : MenuObj
    {
        public DonutScore()
            : base("misc")
        {
            Layer = SpriteLayer.UILAYER1;
        }

        public override void Initialize(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            base.Initialize(Content);

            CreateFramesFromXML("misc_frames");
            CurrentFrame = "donut";
            ResetDimensions();

            PosX = 0.368f * BaseGame.Get.BackBufferWidth;
            PosY = 0.34f * BaseGame.Get.BackBufferHeight;

            score = new MediaTextDrawer("ui/ui", "ui/ui_frames");
            score.Initialize(Content);
            score.Handle = SpriteHandle.CENTERLEFT;
            score.Text = TargetScore.ToString();

            UniformScale = ScaleFactor * 0.5f;
        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);

            score.Position = ScaledPos + new Vector2(Width / 1.5f * ScaledScale, 0);
            score.UniformScale = ScaledScale * 1.25f + scoreWooTimer / Camera.Get.UniformWorldScale;
            score.UniformScale *= 0.5f;
            score.Colour = Color.Tomato;

            UniformScale = (scoreWooTimer / Camera.Get.UniformWorldScale)*0.2f + 1;
            UniformScale *= ScaleFactor;
            UniformScale *= 0.5f;


            if (scoreWooTimer > 0)
            {
                scoreWooTimer = JabMath.LinearMoveTowards(scoreWooTimer, 0.0f, gttf(dt) );
            }
        }

        public override void Draw()
        {
            base.Draw();
            score.Draw();
        }

        MediaTextDrawer score;

        public void IncrementScore()
        {
            ++TargetScore;
            scoreWooTimer = 0.2f;

            score.Text = TargetScore.ToString();
        }

        float scoreWooTimer = 0.0f;
        public int TargetScore = 0;
    }
}
