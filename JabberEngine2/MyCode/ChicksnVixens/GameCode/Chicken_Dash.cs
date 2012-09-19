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

using Jabber.Util;
using Jabber.Media;
using Jabber.Physics;
using Jabber.Scene;

namespace ChicksnVixens
{
    public class Chicken_Dash : Chicken
    {
        public Chicken_Dash(ThisGamesScene world)
            : base(world)
        {
            EventManager.Get.RegisterListner(this);
        }
     
        public override void ProcessEvent(Event ev)
        {
            base.ProcessEvent(ev);

            if (ev is CollisionEvent)
            {
                if ((ev as CollisionEvent).ActorPresent(Body) != null && !World.IsEntityGroup((ev as CollisionEvent).ActorPresent(Body).CollisionGroup))
                {
                    accelerate = false;
                    somethinghit = true;
                    active = false;
                    AccelerateAnimation = "Accelerate_Dash";
                   // Body.LinearDamping = 0.25f;
                }
            }
        }

        public override void Initialize(ContentManager Content)
        {
            base.Initialize(Content);

            AccelerateAnimation = "Accelerate_Dash";
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void OnPress(Vector2 pos)
        {
            base.OnPress(pos);

            if (active)
            {
                active = false;

                Vector2 tarpos = Camera.Get.ScreenToWorld(pos);


                Vector2 dir = tarpos - Body.Position;
                dir.Normalize();

                float velocity = Body.LinearVelocity.Length();
                dir *= velocity * 1.5f;
                if (dir.Length() < 20)
                {
                    dir.Normalize();
                    dir *= 20.0f;
                }
                Body.LinearVelocity = dir;


                /*
                if (dir.Length() < 25)
                {
                    dir.Normalize();
                    Body.LinearVelocity = dir * 25;
                }*/

                AccelerateAnimation = "Accelerate_Flap";
                AudioQueue.PlayOnce("Sounds/FeatherFall");
                accelerate = true;
            }
        }
        bool accelerate = false;
        bool somethinghit = false;
  
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (accelerate && !somethinghit)
            {
                Vector2 dir = Body.LinearVelocity;

                if (dir.Length() < 25)
                {
                 //   dir.Normalize();
                 //   Body.LinearVelocity = dir * 25;
                }
            }
        }
    }
}
