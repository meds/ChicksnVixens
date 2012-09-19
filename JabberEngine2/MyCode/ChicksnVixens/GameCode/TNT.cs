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
    public class TNT : PhysicSprite
    {
        public TNT(GameScene scene)
            : base()
        {
            this.scene = scene;
            EventManager.Get.RegisterListner(this);
        }


        void OnContact(Collision col)
        {
            JabActor other = col.HasActor(Body);
            if (other != null && !World.IsEntityGroup(other.CollisionGroup))
            {
                if ((other.LinearVelocity - Body.LinearVelocity).Length() + Math.Abs(Body.AngularVelocity - other.AngularVelocity) > 2.5f)
                {
                    Detonate();
                }
            }
        }
        public void Detonate()
        {
            if (CheckFlag(Flags.DELETE))
            {
                return;
            }
            RaiseFlag(Flags.DELETE);
            Body.CollisionGroup = Fox.FOX_NONE_COLLISION_GROUP;
            Body.IgnoreRayCast = true;
            Explosion exp = new Explosion(100, 500, 1.0f, 1.0f, scene.World, Position);
            exp.Initialize(BaseGame.Get.Content);
            scene.AddNode(exp);

            AudioQueue.PlayOnce("Sounds/Explosion");
        }
        public override void Initialize(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            base.Initialize(Content);
            World.BeginContact += OnContact;
            Body.Friction = 1.0f;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            World.BeginContact -= OnContact;
        }

        GameScene scene = null;
        public override void ProcessEvent(Event ev)
        {
            base.ProcessEvent(ev);
            /*
            if (ev is CollisionEvent)
            {
                CollisionEvent e = ev as CollisionEvent;
                JabActor other = e.ActorPresent(Body);
                if (other != null)
                {
                    if ((other.LinearVelocity - Body.LinearVelocity).Length() > 0.05f)
                    {
                        RaiseFlag(Flags.DELETE);
                        Explosion exp = new Explosion(40, 300, 1.0f, 1.5f, scene.World, Position);
                        exp.Initialize(BaseGame.Get.Content);
                        scene.AddNode(exp);

                        AudioQueue.PlayOnce("Sounds/Explosion");
                    }
                }
            }*/
        }

        public override void Draw()
        {
            AsType.Dimension = Body.Dimension * Scale;
            base.Draw();
        }
    }
}
