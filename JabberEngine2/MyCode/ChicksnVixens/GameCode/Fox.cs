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
using ChicksnVixens.Screens;
using ChicksnVixens;

using Jabber.GameScreenManager;

namespace ChicksnVixens
{
    public class Fox : PhysicAnimSprite
    {
        public static int FOX_NONE_COLLISION_GROUP = 13;
        static float maxTime = 0.33f;
		public override void Draw()
		{
            if (invincibilityTime < maxTime)
            {
                return;
            }
            AsType.Handle = SpriteHandle.CENTER;
            if (AsType.Animation == "Corpse")
            {
                int k = 0;
            }
			base.Draw();

            if (WearHat)
            {
                if (AnimationForCurrentAnimExists)
                {
                    hat.Position = AsType.Position;
                    hat.Rot = AsType.Rot;
                    hat.Scale = Scale;

                    try
                    {
                        hat.CurrentFrame = FrameForAnim;
                        hat.Draw();
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
		}

        string FrameForAnim
        {
            get
            {
                if (!AnimationForCurrentAnimExists)
                    return "";
                else
                {
                    switch (AsType.CurrentFrame)
                    {
                        case "fox-watch-00000":
                            return locationHat + "-watch-000";
                        case "fox-watch-00001":
                            return locationHat + "-watch-001";
                        case "fox-watch-00002":
                            return locationHat + "-watch-002";
                        case "fox-watch-00003":
                            return locationHat + "-watch-003";
                        case "fox-watch-00004":
                            return locationHat + "-watch-004";
                        case "fox-watch-00005":
                            return locationHat + "-watch-005";
                        case "fox-watch-00006":
                            return locationHat + "-watch-006";
                        case "fox-watch-00007":
                            return locationHat + "-watch-007";
                        case "fox-watch-00008":
                            return locationHat + "-watch-008";
                        case "fox-watch-00009":
                            return locationHat + "-watch-009";
                        case "fox-watch-00010":
                            return locationHat + "-watch-010";
                        case "fox-watch-00011":
                            return locationHat + "-watch-011";
                        case "fox-idle-00000":
                            return locationHat + "-idle-000";
                        case "fox-idle-00001":
                            return locationHat + "-idle-001";
                        case "fox-idle-00002":
                            return locationHat + "-idle-002";
                        case "fox-idle-00003":
                            return locationHat + "-idle-003";
                        case "fox-idle-00004":
                            return locationHat + "-idle-004";
                        case "fox-idle-00005":
                            return locationHat + "-idle-005";
                        case "fox-idle-00006":
                            return locationHat + "-idle-006";
                        case "fox-idle-00007":
                            return locationHat + "-idle-007";
                        case "fox-idle-00008":
                            return locationHat + "-idle-008";
                        case "fox-idle-00009":
                            return locationHat + "-idle-009";
                        case "fox-idle-00010":
                            return locationHat + "-idle-010";
                        case "fox-idle-00011":
                            return locationHat + "-idle-011";
                    }


                    return "";
                }
            }
        }

        bool AnimationForCurrentAnimExists
        {
            get
            {
                if (AsType.Animation == "Roll" || AsType.Animation == "Celebrate")
                {
                    return false;
                }
                return (AsType.Animation == "Idle" || AsType.Animation == "Watch" || AsType.CurrentFrame.Substring(0, 9) == "fox-watch");
            }
        }
        public bool WearHat = false;
        ChicksScene chicksScene;

        public Fox(GameplayScreen screen, ChicksScene world, /*Screens.GameplayScreen screen,*/ Vector2 position, float rot)
            : base(87, position, true, world.World, "fox")
        {
            this.screen = screen;
            chicksScene = world;
            DoDimensions = false;
            PhysicsPosition = false;

            Body.Mass = 1.55f;

            RaiseFlag(Jabber.Flags.ACCEPTINPUT);

            EventManager.Get.RegisterListner(this);

            Layer = SpriteLayer.LAYER10;

            Body.UserData = this;

            Body.Friction = 10.0f;
            Body.AngularDamping = 10.0f;



            Body.LinearDamping = maxDamp;
            Body.Friction = maxDamp;
            Body.AngularDamping = maxDamp;

          //  this.screen = screen;
            for (int i = 0; i < ScreenManager.Get.Screens.Count; i++)
            {
                if (ScreenManager.Get.Screens[i] is GameplayScreen)
                {
                    screen = ScreenManager.Get.Screens[i] as GameplayScreen;
                }
            }
            //screen = Jabber.GameScreenManager.ScreenManager.Get.TopScreen as GameplayScreen;
        }
        GameplayScreen screen;
        float health = 5;
        public bool IsAlive
        {
            get
            {
                return health > 0;
            }
        }
        float invincibilityTime = 0.0f;
        public override void ProcessEvent(Event ev)
        {
            if (invincibilityTime < maxTime)
            {
                if (ev is CollisionEvent)
                {
                    CollisionEvent e = ev as CollisionEvent;

                    {
                        if (e.ActorPresent(Body) != null)
                        {
                            if (e.ActorPresent(Body).UserData is Fan)
                            {
                                InFan = true;
                            }
                        }
                    }
                }
                return;
            }
            base.ProcessEvent(ev);

            if (ev is CollisionEvent)
            {
                CollisionEvent e = ev as CollisionEvent;

                if (e.ActorPresent(Body) != null && !World.IsEntityGroup(e.ActorPresent(Body).CollisionGroup))
                {
                    JabActor other = e.ActorPresent(Body);

                    Vector2 relHit = other.LinearVelocity - Body.LinearVelocity;
                    float speedHit = (relHit).Length();

                    if (other.UserData is PhysicSprite)
                    {
                        if (((other.UserData as PhysicSprite).Sprite as Sprite).CurrentFrame.Contains("cow"))
                        {
                            if(Math.Abs(Body.AngularVelocity) < 0.5f)
                                AudioQueue.PlayOnce("Sounds/Hit_Chicken_1");

                            if (health <= 0)
                            {
                                if (++NumTimesOnCowJumpAfterHealthHitZero > 5)
                                {
                                    if (AsType.Animation != "Die")
                                        AudioQueue.PlayOnce("Sounds/Fox_Death_2" + Jabber.BaseGame.Random.Next(0, 3));
                                    AsType.Animation = "Die";
                                }
                            }
                            return;
                        }
                    }
                    if (speedHit > 1.0f && other.Name != "cow")
                    {
                        health = -1;
                    }
                    else if (speedHit > 0.5f && other.UserData is Fox)
                    {
                        health = -1;
                    }

                    if (other.UserData is Chicken)
                    {
                        AudioQueue.PlayOnce("Sounds/punch_0" + Jabber.BaseGame.Random.Next(1, 5));
                    }
                    else if (speedHit > 1.0f && screen.screenCamera.IsVisible(this))
                    {
                        if (Math.Abs(Body.AngularVelocity) < 0.5f)
                            AudioQueue.PlayOnce("Sounds/Hit_Chicken_1");// + Jabber.BaseGame.Random.Next(1, 5));
                    }

                    if (speedHit > 0.25f)
                    {
                      /*  if (hitSound == null)
                        {
                            //hitSound = AudioManager.CreateSound("Sounds/Hit_Chicken_1");
                        }
                        if (!hitSound.IsPlaying)
                        {
                            if(other.UserData is Chicken || other.BodyState == JabActor.BodyType.STATIC)
                                hitSound.Play(0.2f);
                        }*/
                        //AudioManager.PlayOnce("Sounds/Hit_Chicken_1");
                    }
                }
                else if (e.ActorPresent(Body) != null)
                {
                    if (e.ActorPresent(Body).UserData is Fan)
                    {
                        InFan = true;
                    }
                }
            }
        }
        int NumTimesOnCowJumpAfterHealthHitZero = 0;
        bool InFan = false;
        public override void OnMove(Vector2 pos)
        {
            base.OnMove(pos);
        }
        Sprite hat;
        public override void Initialize(ContentManager content)
        {
            base.Initialize(content);
            AsType.CreateFramesFromXML("fox_frames");

            AsType.ResetDimensions();
            AsType.UniformScale = 1.0f;

            AsType.Animation = "Idle";

            AsType.FrameChanges += OnFrameChange;

            AsType.Width = 128;
            AsType.Height = 128;

          //  if (screen.location == "bavaria")
            {
                locationHat = screen.location;
                locationHat = LocationToHat;
            }

            hat = new Sprite("hats/" + locationHat);
            hat.Initialize(content);
            hat.CreateFramesFromXML("hats/" + locationHat + "_frames");
            hat.CurrentFrame = locationHat + "-idle-000";
            hat.ResetDimensions();
        }
        string locationHat = "australia";


        string LocationToHat
        {
            get
            {
                switch (locationHat)
                {
                    case "uluru":
                        return "australia";
                    case "bavaria":
                        return "bavaria";
                    case "vesuvius":
                        return "visuvi";
                    case "polar":
                        return "polar";
                    case "paris":
                        return "france";
                }

                return locationHat;
            }
        }
        public void SwitchActiveBody()
        {
            //circle.CollisionGroup = FOX_NONE_COLLISION_GROUP;
            // return;
            /* if (Body == box)
             {
                 box.CollisionGroup = FOX_NONE_COLLISION_GROUP;
                 circle.CollisionGroup = 0;

                 Body = circle;
                 Body.LinearVelocity = box.LinearVelocity;
                 Body.AngularVelocity = box.AngularVelocity;
                 Body.Position = box.Position;
             }
             else
             {
                 circle.CollisionGroup = FOX_NONE_COLLISION_GROUP;
                 box.CollisionGroup = 0;

                 Body = box;
                 Body.LinearVelocity = circle.LinearVelocity;
                 Body.AngularVelocity = circle.AngularVelocity;
                 Body.Position = circle.Position;
             }*/

            if (Body.Friction != maxDamp)
            {
                Body.LinearDamping = maxDamp;
                Body.Friction = maxDamp;
                Body.AngularDamping = maxDamp;
            }
            else
            {
                Body.Friction = minDamp;
                Body.AngularDamping = minDamp;
                Body.LinearDamping = minDamp;
            }
        }

        static float maxDamp = 60.0f;
        static float minDamp = 0.5f;
        public bool IsBox
        {
            get { return Body.Friction == maxDamp; }
        }
        bool IsCircle
        {
            get { return Body.Friction == minDamp; }
        }
        /*
        JabActor Other
        {
            get
            {
                if (Body == box)
                {
                    return circle;
                }
                else
                {
                    return box;
                }
            }
        }
        */
        void SetPos()
        {
            Vector2 offset = Vector2.Zero;
            offset.X = (float)Math.Sin((float)Body.Rot);
            offset.Y = (float)Math.Cos((float)Body.Rot);

            Vector2 posToUse = offset * 18.0f + Body.Position;
            float rotToUse = Body.Rot;
            if (IsBox)
            {
                Body.IgnoreRayCast = true;
                RayCastHit hit = ActorStandingOn; //World.RayCast(Body.Position, new Vector2(0, -Height / 1.8f) + Body.Position);
                if (hit.actor != null)
                {
                    rotToUse = (float)Math.Atan2((float)hit.worldNormal.X, (float)hit.worldNormal.Y);

                    offset = hit.worldNormal * 18.0f;
                    posToUse = Body.Position + offset;
                }
                Body.IgnoreRayCast = false;

                PhysicsRotate = false;
            }
            else
            {
                PhysicsRotate = true;
            }


          //  Other.Position = posToUse;
          //  Other.Rot = rotToUse;


            AsType.Position = posToUse;
            AsType.Rot = rotToUse;
        }

        public bool CarryingSomething
        {
            get
            {
                Body.IgnoreRayCast = true;
                RayCastHit hit = World.RayCast(Body.Position, Body.Position + new Vector2(0, Height * 0.8f));

                if (hit.actor == null)
                {
                    hit = World.RayCast(Body.Position + new Vector2(20, 0), Body.Position + new Vector2(20, Height * 0.95f));
                }
                if (hit.actor == null)
                {
                    hit = World.RayCast(Body.Position + new Vector2(-20, 0), Body.Position + new Vector2(-20, Height * 0.95f));
                }
                Body.IgnoreRayCast = false;
                if (health <= 0)
                {
                    int k = 0;
                }
                return hit.actor != null;
            }
        }
        public bool ThingStandingOnIsMoving
        {
            get
            {
                if (!StandingOnSomething)
                    return false;
                else
                {
                    Body.IgnoreRayCast = true;
                    RayCastHit hit = World.RayCast(Body.Position, Body.Position + new Vector2(0, -Height * 0.55f));

                    if (hit.actor == null)
                    {
                        hit = World.RayCast(Body.Position + new Vector2(-20, 0), Body.Position + new Vector2(-20, -Height * 0.55f));
                    }
                    if (hit.actor == null)
                    {
                        hit = World.RayCast(Body.Position + new Vector2(20, 0), Body.Position + new Vector2(20, -Height * 0.55f));
                    }


                    Body.IgnoreRayCast = false;
                    return hit.actor.LinearVelocity.Length() > 0.2f || Math.Abs(hit.actor.AngularVelocity) > 0.2f;
                }
            }
        }

        public RayCastHit ActorStandingOn
        {
            get
            {
                Body.IgnoreRayCast = true;
                RayCastHit hit = World.RayCast(Body.Position, Body.Position + new Vector2(0, -Height * 0.55f));

                if (hit.actor == null)
                {
                    hit = World.RayCast(Body.Position + new Vector2(-20, 0), Body.Position + new Vector2(-20, -Height * 0.55f));
                }
                if (hit.actor == null)
                {
                    hit = World.RayCast(Body.Position + new Vector2(20, 0), Body.Position + new Vector2(20, -Height * 0.55f));
                }


                Body.IgnoreRayCast = false;
                return hit;
            }
        }

        public bool StandingOnSomething
        {
            get
            {
                return ActorStandingOn.actor != null;
                Body.IgnoreRayCast = true;
                RayCastHit hit = World.RayCast(Body.Position, Body.Position + new Vector2(0, -Height * 0.55f));

                if (hit.actor == null)
                {
                    hit = World.RayCast(Body.Position + new Vector2(-20, 0), Body.Position + new Vector2(-20, -Height * 0.55f));
                }
                if (hit.actor == null)
                {
                    hit = World.RayCast(Body.Position + new Vector2(20, 0), Body.Position + new Vector2(20, -Height * 0.55f));
                }


                Body.IgnoreRayCast = false;
                return hit.actor != null;
              //  return false;
            }
        }


        public override void Update(GameTime gameTime)
        {
            if (invincibilityTime < maxTime)
            {
                invincibilityTime += gttf(gameTime);
                Body.Awake = true;
                return;
            }
            else
            {
                invincibilityTime += gttf(gameTime);
            }
            if (screen == null)
            {
                for (int i = 0; i < ScreenManager.Get.Screens.Count; i++)
                {
                    if (ScreenManager.Get.Screens[i] is GameplayScreen)
                    {
                        screen = ScreenManager.Get.Screens[i] as GameplayScreen;
                    }
                }
            }
            if (health > 0)
            {
                if (ThingStandingOnIsMoving && IsBox)
                {
                   // SwitchActiveBody();
                }
                if ((Body.LinearVelocity.Length() + Math.Abs(Body.AngularVelocity) >= 0.5f && IsBox) || (!StandingOnSomething && IsBox))
                {
                    SwitchActiveBody();
                    if (AsType.Animation != "Roll")
                    {
                        AsType.Animation = "Roll";
                    }
                }
                else if ((!IsBox && Body.LinearVelocity.Length() + Math.Abs(Body.AngularVelocity) < 0.5f) && StandingOnSomething && !ThingStandingOnIsMoving)
                {
                    SwitchActiveBody();
                    if (AsType.Animation != "Idle" && chicksScene.ActiveChicken == null)
                    {
                        AsType.Animation = "Idle";
                    }
                    else
                    {
                        Chicken active = chicksScene.ActiveChicken;
                        AsType.Animation = "";
                        AsType.CurrentFrame = "fox-watch-00010";
                    }
                }

                if (AsType.Animation == "Idle" && CarryingSomething)
                {
                    AsType.Animation = "Carrying";
                }
                else if (AsType.Animation == "Carrying" && !CarryingSomething)
                {
                    AsType.Animation = "Idle";
                }

                if ((AsType.Animation == "Idle" || AsType.Animation == "") && chicksScene.ActiveMotion != null)
                {
                    Chicken active = chicksScene.ActiveMotion;
                    AsType.Animation = "";

                    Vector2 dirToChicken = active.Position - AsType.Position;
                    dirToChicken.Normalize();

                    float rotToChicken = (float)Math.Atan2((float)-dirToChicken.X, (float)-dirToChicken.Y);
                    rotToChicken -= 0.5f * (float)Math.PI + AsType.Rot;
                    while (rotToChicken < 0)
                    {
                        rotToChicken += 2 * (float)Math.PI;
                    }
                    while (rotToChicken > 2 * (float)Math.PI)
                    {
                        rotToChicken -= 2 * (float)Math.PI;
                    }
                    rotToChicken /= 2 * (float)Math.PI;
                    int num = (int)(rotToChicken * 11);

                    if (num.ToString().Length == 2)
                    {
                        AsType.CurrentFrame = "fox-watch-000" + num.ToString();
                    }
                    else
                    {
                        AsType.CurrentFrame = "fox-watch-0000" + num.ToString();
                    }
                }
                else if (AsType.Animation == "")
                {
                    AsType.Animation = "Idle";
                }
                if (health > 0)
                {
                    if (PosX > screen.scene.GetRightMaxPos() + 1000 || PosX < screen.scene.startPos.X - 1000 || PosY < -10 || PosY > 10000)
                    {
                        health = -1;
                    }
                    if (!(ScreenManager.Get.TopScreen is PauseScreen) && ScreenManager.Get.TopScreen != screen)
                    {
                        if (AsType.Animation != "Celebrate" && AsType.Animation != "Roll" && AsType.Animation != "Carrying")
                            AsType.Animation = "Celebrate";
                    }
                    else if (AsType.Animation == "Celebrate")
                    {
                        AsType.Animation = "Idle";
                    }
                }

                SetPos();
            }
            else
            {
                if (AsType.Animation == "Roll")
                {
                    SetPos();
                    if (Body.LinearVelocity.Length() < 0.3f && StandingOnSomething)
                    {
                        if(AsType.Animation != "Die")
                            AudioQueue.PlayOnce("Sounds/Fox_Death_2" + Jabber.BaseGame.Random.Next(0, 3));
                        AsType.Animation = "Die";
                    }
                    else if (Body.LinearVelocity.Length() < 0.3f || InFan)
                    {
                        deadWhileInRollButStillTimer += gttf(gameTime);
                        if (deadWhileInRollButStillTimer > 1.0f)
                        {
                            if (AsType.Animation != "Die")
                                AudioQueue.PlayOnce("Sounds/Fox_Death_2" + Jabber.BaseGame.Random.Next(0, 3));
                            AsType.Animation = "Die";
                        }
                    }
                    else
                    {
                        deadWhileInRollButStillTimer = 0.0f;
                    }
                }
                else if (AsType.Animation != "Die" && AsType.Animation != "Corpse")
                {
                    if (AsType.Animation != "Die")
                        AudioQueue.PlayOnce("Sounds/Fox_Death_2" + Jabber.BaseGame.Random.Next(0, 3));
                    AsType.Animation = "Die";

                    Body.CollisionGroup = FOX_NONE_COLLISION_GROUP;
                    Body.IgnoreRayCast = true;
                    PhysicsPosition = false;
                    PhysicsRotate = false;
                }
                else if (AsType.Animation == "Die" || AsType.Animation == "Corpse")
                {
                    Body.CollisionGroup = FOX_NONE_COLLISION_GROUP;
                    Body.IgnoreRayCast = true;
                    PhysicsPosition = false;
                    PhysicsRotate = false;

                    RayCastHit hit = World.RayCast(AsType.Position, AsType.Position + new Vector2(0, -(AsType.Height / 1.4f)));
                    if ( hit.actor != null)
                    {
                        if (true)//AsType.PosY - (hit.worldImpact.Y + AsType.Height / 2.0f) > 0) todo: fix
                        {
                            AsType.PosY = hit.worldImpact.Y + AsType.Height / 2.0f;
                            AsType.Rot = (float)Math.Atan2((float)hit.worldNormal.X, (float)hit.worldNormal.Y);
                            deathFallTimer = 0;
                        }
                        
                        if(AsType.Animation  == "Corpse")
                            alphaOutTimer += gttf(gameTime);

                        if (alphaOutTimer > 1.0f)
                        {
                            alphaingOut += gttf(gameTime);
                            float val = 1.0f - alphaingOut;
                            if (alphaingOut > 1)
                            {
                                RaiseFlag(Jabber.Flags.DELETE);
                            }
                            else
                            {
                                Colour = new Color(val, val, val,val);
                            }
                        }
                    }
                    else
                    {
                        alphaOutTimer = 0.0f;
                        deathFallTimer += gttf(gameTime);
                        AsType.PosY -= deathFallTimer * 9.8f;
                    }

                    Body.IgnoreRayCast = true;
                }
            }
            base.Update(gameTime);

            if (InFan)
            {
                Body.LinearDamping = 0.0f;
            }
            else if (InFanLastFrame)
            {
                Body.Friction = 0.5f;
                Body.AngularDamping = 0.5f;
                Body.LinearDamping = 0.5f;

                AsType.Animation = "Roll";
            }
            InFanLastFrame = InFan;
            InFan = false;

            if (AsType.Animation == "Roll" || AsType.Animation == "Die" || AsType.Animation == "Corpse")
            {
                if (WearHat && invincibilityTime > maxTime * 2.0f)
                {
                    ThrownHat hat = new ThrownHat(LocationToHat);
                    hat.Initialize(Jabber.BaseGame.Get.Content);
                    hat.Position = Body.Position + ActorStandingOn.worldNormal*64;
                    hat.Rot = Rot;
                    chicksScene.AddNode(hat);

                    WearHat = false;
                }
            }
        }
        bool InFanLastFrame = false;
        float deathFallTimer = 0;
        float alphaOutTimer = 0;
        float alphaingOut = 0;
        float deadWhileInRollButStillTimer = 0;
        void OnFrameChange(int before, int after)
        {
            if (AsType.Animation == "Die")
            {
                if (after == 0)
                {
                    AsType.Animation = "Corpse";
                }
            }
        }
       // SoundInst hitSound = null;
        public override void UnloadContent()
        {
         //   if (hitSound != null)
            {
               // hitSound.RaiseFlag(Jabber.Flags.DELETE);
            }
          //  circle.RaiseFlag(Jabber.Flags.DELETE);
            //box.RaiseFlag(Jabber.Flags.DELETE);
            base.UnloadContent();
        }
    }
}