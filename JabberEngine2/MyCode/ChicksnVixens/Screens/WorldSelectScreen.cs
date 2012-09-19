using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jabber.GameScreenManager;
using Jabber.Media;
using Microsoft.Xna.Framework;
using Jabber.Scene;
using Jabber.Physics;
using Jabber.Util;
using Jabber;
using Jabber.Util.UI;
namespace ChicksnVixens.Screens
{
    public class ArrowScroll : Sprite
    {
        public ArrowScroll()
            : base("ui/ui")
        {
            RaiseFlag(Flags.ACCEPTINPUT);
            RaiseFlag(Flags.FADE_IN);
            EventManager.Get.RegisterListner(this);
        }

        public override void OnTap(Vector2 pos)
        {
            base.OnTap(pos);

            if (Contains(pos.X, pos.Y))
            {
                EventManager.Get.SendEvent(new MenuEvent(this, MenuEvent.EventType.TAP));
            }
        }

        public override void Initialize(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            base.Initialize(Content);
            CreateFramesFromXML("ui/ui_frames");
            CurrentFrame = "arrow";
            ResetDimensions();
        }

        public override void ProcessEvent(Event ev)
        {
            base.ProcessEvent(ev);

            if (ev is Screen.ScreenFadeInEvent)
            {
                if ((ev as Screen.ScreenFadeInEvent).Screen is WorldSelectScreen)
                {
                    LowerFlag(Flags.FADE_OUT);
                    RaiseFlag(Flags.FADE_IN);
                }
            }
            else if (ev is Screen.ScreenFadeOutEvent)
            {
                if ((ev as Screen.ScreenFadeOutEvent).Screen is WorldSelectScreen)
                {
                    LowerFlag(Flags.FADE_IN);
                    RaiseFlag(Flags.FADE_OUT);
                }
            }


        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);

            if (CheckFlag(Flags.FADE_OUT))
            {
                fadeTimer = JabMath.MoveTowards(fadeTimer, 0, gttf(dt) * 4);
            }
            else if (CheckFlag(Flags.FADE_IN))
            {
                fadeTimer = JabMath.MoveTowards(fadeTimer, 1, gttf(dt) * 6);
            }
        }

        float fadeTimer = 0;

        public override void Draw()
        {
            
            if (Right)
            {
                Handle = SpriteHandle.CENTERRIGHT;
                Position = Camera.Get.Position + new Vector2(Camera.Get.GetViewPort().X + (1.0f - fadeTimer) * Width*ScaleX * 1.5f, 0);
            }
            else
            {
                Effect = SpriteEffect.FLIPHORIZONTAL;
                Handle = SpriteHandle.CENTERLEFT;
                Position = Camera.Get.Position + new Vector2(-Camera.Get.GetViewPort().X - (1.0f - fadeTimer) * Width * ScaleX * 1.5f, 0);
            }
            UniformScale = ScaleFactor / Camera.Get.UniformWorldScale;


            base.Draw();
        }
        public bool Right = true;
    }
    public class MenuCamera : Camera
    {
        public Vector2 TargetPos
        {
            get
            {
                return targetPos;
            }
            set
            {
                if (targetPos != value)
                {
                    lastPos = Position;
                }
                targetPos = value;
            }
        }
        Vector2 lastPos = Vector2.Zero;
        Vector2 targetPos = Vector2.Zero;
        public float targetScale = 1;
        public bool LevelSelect = false;
        public float minScale = 1.8f;
        float lastcurLength = 0;
        public override void Update(GameTime dt)
        {
            base.Update(dt);

            if (lastPos != targetPos && !LevelSelect)
            {
                float totalLength = (lastPos - targetPos).Length();
                float curLength = (Position - targetPos).Length();
                float prop = 1.0f - curLength / totalLength;


                float interp = JabMath.Sin(curLength * JabMath.PI / totalLength);

                if (curLength == 0 && UniformWorldScale != minScale)
                {
                    if (lastcurLength != 0)
                    {
                        lastcurLength = 0;
                        UniformWorldScale = minScale;
                    }
                    else
                    {
                        UniformWorldScale = JabMath.MoveTowards(UniformWorldScale, minScale, gttf(dt) * 5.0f, 0.001f);
                    }
                }
                else if (curLength != 0)
                {
                    lastcurLength = curLength;
                    if (totalLength < 300)
                    {
                        UniformWorldScale = JabMath.LinearInterpolate(minScale, 1.5f, interp);
                    }
                    else
                    {
                        UniformWorldScale = JabMath.LinearInterpolate(minScale, 0.8f, interp);
                    }
                }
                
            }
            else if (LevelSelect)
            {
                UniformWorldScale = JabMath.MoveTowards(UniformWorldScale, targetScale, gttf(dt) * 5.0f, 0.001f);
            }
            else
            {
                UniformWorldScale = JabMath.MoveTowards(UniformWorldScale, minScale, gttf(dt) * 5.0f, 0.001f);
            }
            Position = JabMath.MoveTowards(Position, targetPos, gttf(dt)*5.0f, 5.0f);
        }
    }
    public class WorldSelectScreen : Screen
    {
        public WorldSelectScreen()
            : base()
        {
            screenCamera = new MenuCamera();
            EventManager.Get.RegisterListner(this);

            RaiseFlag(Flags.ACCEPTINPUT);
        }

#if WINDOWS_PHONE || ANDROID
        public override void OnBackPress()
        {
            base.OnBackPress();

            EventManager.Get.SendEvent(new ScreenCancelEvent(this));
            backToMainMenu = true;
            blank = new BlankNess();
            blank.Initialize(Content);
            blank.fullBlankity = 1.0f;
            Components.Add(blank);
            blank.RaiseFlag(Flags.FADE_IN);
        }      
#endif
        public void SetCurrentCountry(string loc)
        {
            curLocation = worldLocations.IndexOf(loc);
            Cam.TargetPos = map.GetLocation(loc);
            Cam.targetScale = 2;
            Cam.UniformWorldScale = Cam.minScale;
            Cam.LevelSelect = false;
            Cam.Position = Cam.TargetPos;

            playLocationSetSFX = false;
            loadLocation = true;
        }
        bool playLocationSetSFX = true;
        BlankNess blank;
        public override void OnFadeOutComplete()
        {
            base.OnFadeOutComplete();
            if (backToMainMenu)
            {
                RaiseFlag(Flags.DELETE);
                MainMenuScreen s = new MainMenuScreen();
                s.Initialize(Content);
                ScreenManager.Get.AddScreen(s);
            }
        }

        Dictionary<string, Vector2> worldPositions = new Dictionary<string, Vector2>();
        MapUI map;
        GameScene scene;
        MenuCamera Cam
        {
            get { return screenCamera as MenuCamera; }
        }


        public override void Draw()
        {
            base.Draw();

            locationText.Position = Camera.Get.Position;
            locationText.Text = worldLocations[curLocation];
            locationText.Draw();
        }

        public override void ProcessEvent(Event ev)
        {
            base.ProcessEvent(ev);

            if (ev is ScreenCancelEvent)
            {
                if ((ev as ScreenCancelEvent).Screen is LevelSelectScreen)
                {
                    Cam.targetScale = 2.0f;
                    Cam.LevelSelect = false;
                    EventManager.Get.SendEvent(new ScreenFadeInEvent(this));
                }
                else if ((ev as ScreenCancelEvent).Screen is WorldSelectScreen)
                {
                    EventManager.Get.SendEvent(new ScreenFadeOutEvent(this));
                }
            }
            else if (ev is MenuEvent)
            {
                MenuEvent e = ev as MenuEvent;
                if (e.sender is ArrowScroll)
                {
                    ArrowScroll s = e.sender as ArrowScroll;

                    if (s.Right)
                        NextLocation();
                    else
                        PrevLocation();

                    EventManager.Get.SendImmediateEvent(new NewLocationSelected());

                    string location = worldLocations[curLocation];

                    Cam.TargetPos = map.GetLocation(location);
                    Cam.targetScale = 2;
                    Cam.LevelSelect = false;
                    /*
                    currentLocation = new LocationIcon(location, map);
                    currentLocation.Initialize(Content);
                    scene.AddNode(currentLocation);*/

                    AudioManager.PlayOnce("Sounds/ChangeCountry");
                }
            }
            /*
            if (ev is ScreenAddedEvent)
            {
                if ((ev as ScreenAddedEvent).Screen is GameplayScreen)
                {
                    RaiseFlag(Flags.DELETE);
                }
            }*/

            /*if (ev is ScreenFadeInCompleteEvent)
            {
                if ((ev as ScreenFadeInCompleteEvent).Screen is GameplayScreen)
                {
                    RaiseFlag(Flags.DELETE);
                }
            }*/

            if (ev is FadeOutEvent)
            {
                if ((ev as FadeOutEvent).Sender is FallingFeathers)
                {
                    RaiseFlag(Flags.DELETE);
                }
            }
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }
        bool loadLocation = false;
        bool backToMainMenu = false;
        

        public override void OnTap(Vector2 pos)
        {
            if (!IsTopScreen)
            {
                return;
            }
            base.OnTap(pos);

            if (ChicksnVixensGame.Get.IsLocationUnlocked(worldLocations[curLocation]))
            {
                if (pos.X > 0.2f && pos.X < 0.8f && pos.Y > 0.1f && pos.Y < 0.9f)
                {
                    loadLocation = true;
                    return;
                }

                if (currentLocation != null)
                {
                    if (currentLocation.Contains(pos.X, pos.Y))
                    {
                        loadLocation = true;
                        return;
                    }
                }
            }
            /*
            ++curLocation;
            if (curLocation > worldLocations.Count - 1)
            {
                curLocation = 0;
            }*/
        }

        void NextLocation()
        {
            ++curLocation;
            if (curLocation > worldLocations.Count - 1)
            {
                curLocation = 0;
            }
        }
        void PrevLocation()
        {
            --curLocation;
            if (curLocation < 0)
            {
                curLocation = worldLocations.Count - 1;
            }
        }

        LocationIcon currentLocation = null;
        List<string> worldLocations = new List<string>();
        int curLocation = 0;

        public override void Update(GameTime dt)
        {
            if(!ChicksnVixensGame.Get.IsLocationUnlocked(worldLocations[curLocation]))
            {
                Vector4 col = locked.Colour.ToVector4();
                col.W = JabMath.LinearMoveTowards(col.W, 1.0f, gttf(dt));
                locked.Colour = new Color(col.W, col.W, col.W, col.W);
            }
            else
            {
                Vector4 col = locked.Colour.ToVector4();
                col.W = JabMath.LinearMoveTowards(col.W, 0.0f, gttf(dt));
                locked.Colour = new Color(col.W, col.W, col.W, col.W);
            }
            if (blank != null)
            {
                if (blank.StateFlag == Jabber.StateFlag.FADE_IN_COMPLETE)
                {
                    OnFadeOutComplete();
                }
            }
            if (IsTopScreen || ScreenManager.Get.TopScreen.CheckFlag(Flags.FADE_OUT))
            {
                /*
                if (Cam.TargetPos == Cam.Position)
                {
                    Cam.targetScale = 1.8f;
                }*/
                Cam.LevelSelect = false;
            }
            else if (!IsTopScreen)
            {
                Cam.LevelSelect = true;
                Cam.targetScale = 1.2f;
            }
            if (loadLocation)
            {
                LevelSelectScreen screen = new LevelSelectScreen(worldLocations[curLocation]);//  currentLocation.location);
                screen.Initialize(Content);
                ScreenManager.Get.AddScreen(screen);
                Cam.targetScale = 1.2f;
                Cam.LevelSelect = true;
                EventManager.Get.SendEvent(new ScreenFadeOutEvent(this));
                loadLocation = false;
                if(playLocationSetSFX)
                    AudioManager.PlayOnce("Sounds/CountrySelect");

                playLocationSetSFX = true;
            }
#if WINDOWS_PHONE
            AdSystem.TargetTop = false;
#endif
            base.Update(dt);
        }

        TextDrawer locationText = new TextDrawer("ui/LevelFont");
        public override void Initialize(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            base.Initialize(content);
            FarWorld world = new FarWorld();
            scene = new GameScene(world, Content);

            locationText.Initialize(Content);
            locationText.Text = "NONE";

            ArrowScroll a = new ArrowScroll();
            a.Initialize(Content);
            a.Right = false;
            scene.AddNode(a);


            a = new ArrowScroll();
            a.Initialize(Content);
            scene.AddNode(a);

            
            map = new MapUI();
            map.Initialize(Content);

            scene.AddNode(map);


            Components.Add(scene);


            for (int i = 0; i < ChicksnVixensGame.Get.locationOrder.Count; i++)
            {
                worldLocations.Add(ChicksnVixensGame.Get.locationOrder[i]);
            }

            BlankNess faderInner = new BlankNess();
            faderInner.Initialize(Content);

            faderInner.fullBlankity = 1.0f;
            faderInner.fadeInTimer = 1.0f;
            faderInner.fadeSpeed = 2.0f;
            faderInner.RaiseFlag(Flags.FADE_OUT);
            Components.Add(faderInner);


            locked = new MenuObj("ui/ui");
            locked.Initialize(content);
            locked.CreateFramesFromXML("ui/ui_frames");
            locked.CurrentFrame = "lock";
            locked.ResetDimensions();
            locked.UniformScale = ScaleFactor * 2.0f;
            locked.Position = Vector2.Zero;
            locked.Colour = new Color(0, 0, 0, 0);
            Components.Add(locked);
            
            EventManager.Get.SendImmediateEvent(new NewLocationSelected());

            string location = worldLocations[curLocation];

            Cam.TargetPos = map.GetLocation(location);
            Cam.targetScale = 2;
            Cam.LevelSelect = false;
        }

        MenuObj locked = null;
    }
}