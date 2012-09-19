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
using Jabber.Util.UI;
using Jabber;

namespace ChicksnVixens.Screens
{
    public class GameplayScreen : Screen
    {
        void CacheSounds()
        {
            AudioManager.CacheSound("basketball");
            AudioManager.CacheSound("BellToll");
            AudioManager.CacheSound("Break_Concrete_01");
            AudioManager.CacheSound("Break_Glass_01");
            AudioManager.CacheSound("Break_Glass_02");
            AudioManager.CacheSound("Break_Glass_03");
            AudioManager.CacheSound("Break_Metal_01");
            AudioManager.CacheSound("Break_Metal_02");
            AudioManager.CacheSound("Break_Wood_01");
            AudioManager.CacheSound("Cannon_Buildup_01");
            AudioManager.CacheSound("Cannon_Buildup_02");
            AudioManager.CacheSound("Cannon_Fire_1");
            AudioManager.CacheSound("Cannon_Fire_2");
            AudioManager.CacheSound("ChangeCountry");
            AudioManager.CacheSound("Chicken_Launch_01");
            AudioManager.CacheSound("Chicken_Launch_02");
            AudioManager.CacheSound("Chicken_Launch_03");
            AudioManager.CacheSound("Chicken_Launch_04");
            AudioManager.CacheSound("Chicken_Launch_05");
            AudioManager.CacheSound("Chicken_Launch_06");

            AudioManager.CacheSound("ChickenBience");
            AudioManager.CacheSound("CountrySelect");
            AudioManager.CacheSound("EggLay");
            AudioManager.CacheSound("Explode_Chicken");
            AudioManager.CacheSound("Explosion");
            AudioManager.CacheSound("FeatherFall");

            AudioManager.CacheSound("Fox_Death_20");
            AudioManager.CacheSound("Fox_Death_21");
            AudioManager.CacheSound("Fox_Death_22");
            AudioManager.CacheSound("Hit_Chicken_1");
            AudioManager.CacheSound("Hit_Chicken_2");
            AudioManager.CacheSound("Hit_Chicken_3");
            AudioManager.CacheSound("Hit_Chicken_4");
            AudioManager.CacheSound("Hit_Egg_01");
            AudioManager.CacheSound("Hit_Egg_02");
            AudioManager.CacheSound("Hit_Glass_01");
            AudioManager.CacheSound("Hit_Glass_02");
            AudioManager.CacheSound("Hit_Glass_03");
            AudioManager.CacheSound("Hit_Glass_04");
            AudioManager.CacheSound("Hit_Glass_Crack");
            AudioManager.CacheSound("Hit_Wood_01");
            AudioManager.CacheSound("Hit_Wood_02");
            AudioManager.CacheSound("pop");

            AudioManager.CacheSound("punch_01");
            AudioManager.CacheSound("punch_02");
            AudioManager.CacheSound("punch_03");
            AudioManager.CacheSound("punch_04");
            AudioManager.CacheSound("Restart");
            AudioManager.CacheSound("Ribbit");
            AudioManager.CacheSound("slide");
            AudioManager.CacheSound("stone-rolling");
        }
        public GameplayScreen(string loc, int levelnum)
            : base()
        {
            EventManager.Get.RegisterListner(this);

            location = loc;
            levelNum = levelnum;

            AudioManager.PlayOnce("Sounds/FeatherFall");
        }

        public void Restart()
        {
            ReloadLevel();
		}
        public ThisGamesScene scene;
        public ChicksScene withChicks;
        FarWorld world;
        float timer = 0;
        public BlankNess blank = null;

        public bool IsQuitting = false;

        public override void Update(GameTime dt)
        {
            if (!AudioManager.MusicPlaying)
            {
                Dictionary<string, string> loctosong = new Dictionary<string, string>();

                string currentSong = AudioManager.CurrentSong;
                if (currentSong == "troublemaker")
                {
                    AudioManager.PlayMusic(location);
                }
                else
                {
                    AudioManager.PlayMusic("troublemaker");
                }
            }
#if WINDOWS_PHONE 
            AdSystem.TargetTop = false;
#endif

            ExplosionPlayed = false;
            woodSFX = cementSFX = glassSFX = false;
            cementTime -= gttf(dt);
            woodTime -= gttf(dt);
            glassTime -= gttf(dt);
            if (!IsTopScreen)
            {
                for (int i = 0; i < scene.Nodes.Count; i++)
                {
                    //if (scene.Nodes[i] is Fox)
                    {
                        scene.Nodes[i].Update(dt);
                    }
                }
                return;
            }
            feathers.Update(dt);
            if (!loadedUp)
            {
                return;
            }
            if (CheckFlag(Jabber.Flags.FADE_IN))
            {
                if (loadedUp)
                {
                    OnFadeInComplete();
                }
            }
            else
            {
                world.Update(dt);
                base.Update(dt);
                timer += gttf(dt);
                gameTime = dt;

                CheckEndGame(dt);
            }

            if (blank != null)
            {
                blank.Update(dt);
                if (blank.StateFlag == Jabber.StateFlag.FADE_IN_COMPLETE)
                {
                    if (!IsQuitting)
                    {
                        LevelEndScreen n = new LevelEndScreen(this);
                        n.Initialize(Content);
                        ScreenManager.Get.AddScreen(n);

                        LowerFlag(Jabber.Flags.FADE_OUT);
                        LowerFlag(Jabber.Flags.FADE_IN);

                        if(GetRemainingFox() == 0)
                            AudioManager.PlayOnce("Sounds/Win_Stinger");
                        else
                            AudioManager.PlayOnce("Sounds/Lose_Stinger");
                    }
                    else
                    {
                        RaiseFlag(Jabber.Flags.DELETE);
                        WorldSelectScreen s = new WorldSelectScreen();
                        s.Initialize(Content);
                        s.SetCurrentCountry(location);
                        ScreenManager.Get.AddScreen(s);
                    }
                }
            }
        }
#if ANDROID || WINDOWS_PHONE
        public override void OnBackPress()
        {
            PauseScreen s = new PauseScreen();
            s.Initialize(Content);
            ScreenManager.Get.AddScreen(s);
        }
#endif
        bool AnyFox
        {
            get
            {
                bool anyFox = false;
                for (int i = 0; i < scene.Nodes.Count; i++)
                {
                    if (scene.Nodes[i] is Fox && (scene.Nodes[i] as Fox).IsAlive)
                    {
                        anyFox = true;
                        break;
                    }
                }
                return anyFox;
            }
        }

        void CheckEndGame(GameTime dt)
        {
            if (endGameTimeOut != 0)
            {
                int k = 0;
            }
            bool anyFox = false;
            for (int i = 0; i < scene.Nodes.Count; i++)
            {
                if (scene.Nodes[i] is Fox && (scene.Nodes[i] as Fox).IsAlive)
                {
                    anyFox = true;
                    break;
                }
            }
            if (!anyFox)
            {
                forceEndGame.Colour = Color.LightGreen;
                forceEndGame.RegularScale = ScaleFactor * 0.35f;
                forceEndGame.ScaleOnHover = ScaleFactor * 1.1f * 0.35f;
            }
            else if(scene.ToFire.Count == 0)
            {
                forceEndGame.Colour = Color.Red;
                forceEndGame.RegularScale = ScaleFactor * 0.35f;
                forceEndGame.ScaleOnHover = ScaleFactor * 1.1f * 0.35f;
            }
            bool anyMoving = false;
            bool onlyFoxMoving = false;
            bool foxAllMovingVertically = true;
            bool movingFoxesAreAllDead = true;
            bool onlyChickenMoving = true;
            List<Chicken> chicks = withChicks.GetChicks();
            for (int i = 0; i < scene.Nodes.Count; i++)
            {
                if (scene.Nodes[i] is Fox)
                {
                    Fox f = scene.Nodes[i] as Fox;
                    if (f.Body.LinearVelocity.Length() > 0.0005f)
                    {
                        onlyFoxMoving = true;
                        if (f.Body.LinearVelocity.X > 0.001f)
                        {
                            foxAllMovingVertically = false;
                        }
                        if (f.IsAlive)
                        {
                            movingFoxesAreAllDead = false;
                        }
                    }
                }
            }
            for (int i = 0; i < chicks.Count; i++)
            {
                if (chicks[i].Body.LinearVelocity.Length() > 0.1f && chicks[i].PosX > cannon.Position.X && chicks[i].PosX < rightMostPos + 100)
                {
                    anyMoving = true;
                    onlyFoxMoving = false;
                }
            }
            if (!anyMoving)
            {
                onlyChickenMoving = false;
            }
            if (!anyMoving)
            {
                for (int i = 0; i < scene.Nodes.Count; i++)
                {
                    if (scene.Nodes[i] is PhysicSprite)
                    {
                        if ((scene.Nodes[i] as PhysicSprite).Body.LinearVelocity.Length() > 0.1f
                            &&
                            scene.Nodes[i].PosX < rightMostPos + 100 && scene.Nodes[i].PosX > cannon.PosX && scene.Nodes[i].PosY > -100)
                        {
                            anyMoving = true;

                            if (!(scene.Nodes[i] is Fox))
                            {
                                onlyFoxMoving = false;
                            }
                            if (!(scene.Nodes[i] is Chicken))
                            {
                                onlyChickenMoving = false;
                            }
                        }
                    }
                }
            }
            if ((!anyMoving || (onlyFoxMoving && foxAllMovingVertically)) && scene.ToFire.Count == 0 && withChicks.ActiveChicken == null)
            {
                chickenStopMovingTimer += gttf(dt);
                if (chickenStopMovingTimer > 10.0f)
                {
                    endGameTimeOut += gttf(dt);
                    if (endGameTimeOut > 10)
                        ScreenFadeOut();
                    HasWon = false;
                }
            }
            else if ((!anyMoving || onlyChickenMoving) && scene.ToFire.Count == 0 && withChicks.ActiveChicken == null)
            {
                chickenStopMovingTimer += gttf(dt);
                if (chickenStopMovingTimer > 10.0f)
                {
                    endGameTimeOut += gttf(dt);
                    if (endGameTimeOut > 10)
                        ScreenFadeOut();
                    HasWon = true;
                }
            }
            else
            {
                chickenStopMovingTimer = 0;
            }

            bool AnyCheckedDonuts = false;
            for (int i = 0; i < scene.Nodes.Count; i++)
            {
                if (scene.Nodes[i] is Donut)
                {
                    if ((scene.Nodes[i] as Donut).Chosen)
                    {
                        AnyCheckedDonuts = true;
                    }
                    AnyCheckedDonuts = true;
                }
                else if (scene.Nodes[i] is DonutCase)
                {
                    DonutCase c = scene.Nodes[i] as DonutCase;
                    if (Math.Abs(c.Rot) < 0.45f * JabMath.PI)
                    {
                        AnyCheckedDonuts = true;
                    }
                    AnyCheckedDonuts = true;
                }
            }

            if (onlyFoxMoving && foxAllMovingVertically && anyFox && scene.ToFire.Count == 0 && !movingFoxesAreAllDead)
            {
                if (AnyMovingAndNoFoxAndNoChicken == float.MinValue)
                {
                    AnyMovingAndNoFoxAndNoChicken = 1.0f;
                }
                AnyMovingAndNoFoxAndNoChicken -= gttf(dt);
                if (AnyMovingAndNoFoxAndNoChicken < 0)
                {
                    endGameTimeOut += gttf(dt);
                    if (endGameTimeOut > 1)
                        ScreenFadeOut();
                    if (!anyFox)
                        HasWon = true;
                }
            }
            else if (!AnyCheckedDonuts && movingFoxesAreAllDead)
            {
                if (((scene.ToFire.Count == 0 && !anyMoving) ||
                    (!anyMoving && !anyFox)) && withChicks.ActiveChicken == null
                    )
                {
                    endGameTimeOut += gttf(dt);
                    if (endGameTimeOut > 3)
                        ScreenFadeOut();
                    if(!anyFox)
                        HasWon = true;
                }
            }
            else if(AnyCheckedDonuts && scene.ToFire.Count == 0 && !anyMoving)
            {
                endGameTimeOut += gttf(dt);
                if (endGameTimeOut > 1)
                    ScreenFadeOut();
                HasWon = false;
            }
            else if(!onlyFoxMoving && theFinalCountDown == 10 && theFinalCountDownWin == 10)
            {
                endGameTimeOut = 0.0f;
            }

            if (!anyFox && scene.ToFire.Count == 0 && withChicks.ActiveChicken == null)
            {
                theFinalCountDownWin -= gttf(dt);
                if (theFinalCountDownWin < 0)
                {
                    endGameTimeOut += gttf(dt);
                    if (endGameTimeOut > 10)
                        ScreenFadeOut();
                    HasWon = true;
                }
            }

            if (anyMoving && !anyFox && !AnyCheckedDonuts && withChicks.ActiveChicken == null)
            {
                theFinalCountDown -= gttf(dt);
                if (theFinalCountDown < 0)
                {
                    endGameTimeOut += gttf(dt);
                    if (endGameTimeOut > 3)
                        ScreenFadeOut();
                    HasWon = false;
                }
            }
        }
        float AnyMovingAndNoFoxAndNoChicken = float.MinValue;
        float endGameTimeOut = 0;
        float chickenStopMovingTimer = 0;
        float theFinalCountDown = 10;
        float theFinalCountDownWin = 10;
        void KillLevel()
        {
            WorldSelectScreen s = new WorldSelectScreen();
            s.Initialize(Content);
            ScreenManager.Get.AddScreen(s);
            RaiseFlag(Jabber.Flags.DELETE);
        }

        void NextLevel()
        {
            RaiseFlag(Jabber.Flags.DELETE);
            GameplayScreen s = new GameplayScreen(location, levelNum);
            s.Initialize(Content);
            ScreenManager.Get.AddScreen(s);
        }

        GameTime gameTime = null;

        public bool CannonBeingDragged
        {
            get
            {
                return cannon.fingerDraggingOnMe;
            }
        }

        public bool CameraFollowingChicken
        {
            get
            {
                return /*withChicks.ActiveChickens*/bFollowingChicken && !bForceInactiveChicken;
            }
        }

        public override void Draw()
        {
            if (CheckFlag(Jabber.Flags.FADE_IN))
            { 
            }
            else if(feathers.StateFlag == Jabber.StateFlag.FADE_OUT_COMPLETE || feathers.StateFlag == Jabber.StateFlag.FADE_IN_COMPLETE && loadedUp)
            {
                base.Draw();

                //scene.Draw();
                //cannon.Draw();

                world.Draw();

                //(world as FarWorld).StartPhysics();
            }


            feathers.Draw();

            if (blank != null)
                blank.Draw();
        }
        public string LevelDir = "";
        bool bForceInactiveChicken = false;
        bool bFollowingChicken = false;
        public Cannon cannon;
        public int levelNum = 0;
        public string location = "";
        public int maxNumFox = 0;
        public int maxNumDonuts = 0;

        public override void UnloadContent()
        {
            base.UnloadContent();
            SetLevelState();
        }
        public int NumStars
        {
            get
            {
                return (int)(3.0f - 3.0f * (float)GetRemainingFox() / maxNumFox);
            }
        }
        void SetLevelState()
        {
            int numFox = (int)(3.0f * (float)GetRemainingFox() / maxNumFox);
            numFox = 3 - numFox;
            (Jabber.BaseGame.Get as ChicksnVixensGame).SetLevel(location, score.score, levelNum, numFox, donutScore.TargetScore);

            ChicksnVixensGame.Get.SaveToFile();
        }
        void ReloadLevel()
        {
            if (scene != null)
            {
                SetLevelState();

                scene.UnloadContent();
                withChicks.UnloadContent();
            }
            //if (blank != null)
            {
                blank = null;
                LowerFlag(Jabber.Flags.FADE_OUT);
            }
            IsQuitting = false;
            Components.Clear();

            world = new FarWorld();
            world.Initialize(new Vector2(0, -15.0f));
            world.SimulationSpeedFactor = 1.0f;

            withChicks = new ChicksScene(this, world, Content);
            withChicks.DoWorldUpdateDraw = false;
           // withChicks.AddNode(cannon);

            scene = new ThisGamesScene(this, world, withChicks, Content);
            scene.DoWorldUpdateDraw = false;

            scene.AddTextureLoadInterceptor("textures\\Physical\\wood", "break", "break_frames", "wood");
            scene.AddTextureLoadInterceptor("textures\\Physical\\cement", "break", "break_frames", "cement");
            scene.AddTextureLoadInterceptor("textures\\Physical\\glass", "break", "break_frames", "glass");


            scene.AddTextureLoadInterceptor("textures\\Backgrounds\\Bavaria\\cowright", "textures/backgrounds/bavaria/bavaria", "textures/backgrounds/bavaria/bavaria_frames", "cowleft");
            scene.AddTextureLoadInterceptor("textures\\Backgrounds\\Bavaria\\cowleft", "textures/backgrounds/bavaria/bavaria", "textures/backgrounds/bavaria/bavaria_frames", "cowright");

            LevelDir = "Content/Levels/" + location + "/Level" + levelNum + ".xml";
            scene.LoadGScene(LevelDir);

            withChicks.startPos = scene.startPos;


            ChickenDrawer chickdrawer = new ChickenDrawer(withChicks, scene);
            chickdrawer.Initialize(Content);
            scene.AddNode(chickdrawer);

            ChickenBience bience = new ChickenBience(this);
            bience.Initialize(Content);
            bience.Position = scene.startPos;
            scene.AddNode(bience);

            cannon = new Cannon(this);
            cannon.Position = scene.startPos;
            cannon.Initialize(Content);
            scene.AddNode(cannon);
            //scene.AddNode(cannon);


            if (withChicks.GetRightMaxPos() > scene.GetRightMaxPos())
            {
                rightMostPos = withChicks.GetRightMaxPos() + 200.0f;
            }
            else
            {
                rightMostPos = scene.GetRightMaxPos() + 200.0f;
            }


            leftMostPos = scene.startPos.X - 500;
            /// rightMostPos = 10000000;
            world.SetCollisionForAll(Fox.FOX_NONE_COLLISION_GROUP, false);
            world.SetCollisionForAll(BreakableBody.BodyNoneCollisionGroup, false);

            string country = scene.countryName;
            //scene.ToFire.Clear();
            if (scene.ToFire.Count == 0)
                for (int i = 0; i < 4; i++)
                {
                    scene.ToFire.Add(0);
                }
            //GameScene worldLoc = new GameScene(world, Content);
            //WorldLocation.CreateVesuvius(scene, (int)leftMostPos, (int)rightMostPos);

           // WorldLocation.CreatePolar(scene, (int)leftMostPos, (int)rightMostPos);
            
            switch (scene.countryName)
            {
                case "bavaria":
                    WorldLocation.CreateBavaria(scene, (int)leftMostPos, (int)rightMostPos);
                    break;
                case "paris":
                    WorldLocation.CreateParis(scene, (int)leftMostPos, (int)rightMostPos);
                    break;
                case "australia":
                    WorldLocation.CreateAustralia(scene, (int)leftMostPos, (int)rightMostPos);
                    break;
                case "polar":
                    WorldLocation.CreatePolar(scene, (int)leftMostPos, (int)rightMostPos);
                    break;
                case "vesuvius":
                    WorldLocation.CreateVesuvius(scene, (int)leftMostPos, (int)rightMostPos);
                    break;
            }
             
            feathers.RaiseFlag(Jabber.Flags.FADE_OUT);
            Components.Add(scene);
            Components.Add(withChicks);


            MenuObj pause = new Button("ui/ui");
            pause.Initialize(Content);
            pause.Name = "pause";
            pause.CreateFramesFromXML("ui/ui_frames");
            pause.CurrentFrame = "pause";
            pause.ResetDimensions();
            pause.UniformScale = ScaleFactor;
            (pause as Button).RegularScale = ScaleFactor;
            (pause as Button).ScaleOnHover = (pause as Button).RegularScale * 1.4f;
            pause.PosX = -Jabber.BaseGame.Get.BackBufferWidth / 2.0f + pause.Width * pause.ScaleX;
            pause.PosY = Jabber.BaseGame.Get.BackBufferHeight / 2.0f - pause.Height * pause.ScaleY;
            Components.Add(pause);


            forceEndGame = new Button("ui/ui");
            forceEndGame.Initialize(Content);
            forceEndGame.UniformScale = 0.001f;
            forceEndGame.Name = "forceendgame";
            forceEndGame.CreateFramesFromXML("ui/ui_frames");
            forceEndGame.CurrentFrame = "doublearrow";
            forceEndGame.ResetDimensions();
            forceEndGame.RegularScale = 0.0f;// ScaleFactor * 0.35f;
            forceEndGame.ScaleOnHover = 0.0f;// ScaleFactor * 1.1f * 0.35f;
            forceEndGame.PosX = -Jabber.BaseGame.Get.BackBufferWidth / 2.0f + pause.Width * pause.ScaleX;
            forceEndGame.PosY = Jabber.BaseGame.Get.BackBufferHeight / 2.0f - pause.Height * pause.ScaleY * 2.3f;
            Components.Add(forceEndGame);
            forceEndGame.RaiseFlag(Flags.PASSRENDER);
            

            float widthHeightToUse = pause.Width;

            pause = new Button("ui/ui");
            pause.Initialize(Content);
            pause.Name = "restart";
            pause.Colour = Color.Green;
            pause.CreateFramesFromXML("ui/ui_frames");
            pause.CurrentFrame = "restart";
            (pause as Button).RegularScale = ScaleFactor;
            (pause as Button).ScaleOnHover = (pause as Button).RegularScale * 1.4f;
            pause.ResetDimensions();
            pause.UniformScale = ScaleFactor;
            pause.Width = pause.Height = widthHeightToUse;
            pause.PosX = -Jabber.BaseGame.Get.BackBufferWidth / 2.0f + pause.Width * pause.ScaleX * 2.5f;
            pause.PosY = Jabber.BaseGame.Get.BackBufferHeight / 2.0f - pause.Height * pause.ScaleY;
            Components.Add(pause);


            maxNumFox = 0;
            maxNumDonuts = 0;
            for (int i = 0; i < scene.Nodes.Count; i++)
            {
                if (scene.Nodes[i] is Fox)
                {
                    ++maxNumFox;
                }
                else if (scene.Nodes[i] is Donut)
                {
                    ++maxNumDonuts;
                }
                else if (scene.Nodes[i] is DonutCase)
                {
                    maxNumDonuts += 5;
                }
            }

            donutScore = new DonutScore();
            donutScore.Initialize(Content);
            scene.AddNode(donutScore);

            

            score = new Score(scene, this.location, levelNum);
            score.Initialize(Content);
            scene.AddNode(score);
            
            AudioQueue q = new AudioQueue(this);
            q.Initialize(Content);

            scene.AddNode(q);
        }

        Button forceEndGame = null;
        public DonutScore donutScore = null;
        public Score score = null;

        public int GetRemainingFox()
        {
            int num = 0;
            for (int i = 0; i < scene.Nodes.Count; i++)
            {
                if (scene.Nodes[i] is Fox && (scene.Nodes[i] as Fox).IsAlive)
                {
                    ++num;
                }
            }

            return num;
        }
        public bool HasWon = false;
        public override void ScreenFadeOut()
        {
            if (blank == null)
            {
                blank = new BlankNess();
                blank.Initialize(Content);
                blank.fullBlankity = 0.5f;
                if (IsQuitting)
                    blank.fullBlankity = 1.0f;
                blank.fadeSpeed = 2.0f;
                blank.RaiseFlag(Jabber.Flags.FADE_IN);

                base.ScreenFadeOut();
            }
        }

        public void ThreadedInitialize()
        {
            loadedUp = false;
            RaiseFlag(Jabber.Flags.FADE_IN);
            //RaiseFlag(Jabber.Flags.PASSRENDER);
            //RaiseFlag(Jabber.Flags.PASSUPDATE);
            
            GameCamera.CurrentCamera = screenCamera;

            while (feathers == null)
            {
                System.Threading.Thread.Sleep(10);
            }

         //   while (!feathers.CheckStateFlag(Jabber.StateFlag.FADE_IN_COMPLETE))
            {
           //     System.Threading.Thread.Sleep(100);
            }

            ReloadLevel();


            screenCamera = new GameCamera(this);
            screenCamera.Position = scene.startPos;
            (screenCamera as GameCamera).lastPosBeforeFire = scene.startPos;
            (screenCamera as GameCamera).TargetPos = scene.startPos;

            (screenCamera as GameCamera).screen = this;
            (screenCamera as GameCamera).Initialize(Content);

            (screenCamera as GameCamera).TargetPos = (screenCamera as GameCamera).targetAreaPos;
            (screenCamera as GameCamera).Position = (screenCamera as GameCamera).targetAreaPos;

            loadedUp = true;

            for (int i = 0; i < ScreenManager.Get.Screens.Count; i++)
            {
                if (ScreenManager.Get.Screens[i] is LevelSelectScreen || ScreenManager.Get.Screens[i] is WorldSelectScreen)
                {
                    ScreenManager.Get.Screens[i].RaiseFlag(Jabber.Flags.DELETE);
                }
            }
        }
        public bool loadedUp = false;
        public override void Initialize(Microsoft.Xna.Framework.Content.ContentManager d)
        {
            RaiseFlag(Jabber.Flags.FADE_IN);
            base.Initialize(Content);
            System.Threading.Thread thread = new System.Threading.Thread( new System.Threading.ThreadStart(ThreadedInitialize) );
            thread.Start();
            
            feathers = new FallingFeathers();
            feathers.Initialize(Content);

            CacheSounds();
        }

        public override void OnFadeInComplete()
        {
            base.OnFadeInComplete();
        }

        public override void OnDrag(Vector2 lastPos, Vector2 thispos)
        {
            base.OnDrag(lastPos, thispos);

            if ((lastPos - thispos).Length() > 0.01f)
            {
                ForceChickenInActive();
            }
        }

        public void ForceChickenInActive()
        {
            bForceInactiveChicken = true;
            bFollowingChicken = false;
            if (withChicks == null)
            {
                return;
            }
            for (int i = 0; i < withChicks.GetChicks().Count; i++)
            {
                withChicks.GetChicks()[i].Deactivate();
            }
        }


        bool woodSFX = false;
        bool cementSFX = false;
        bool glassSFX = false;

        float woodTime = 0.0f;
        float cementTime = 0.0f;
        float glassTime = 0.0f;
        
        bool SFXPlayed(BreakableBody.BodyMaterial material)
        {
            switch (material)
            {
                case BreakableBody.BodyMaterial.CONCRETE:
                    return cementSFX;
                case BreakableBody.BodyMaterial.WOOD:
                    return woodSFX;
                case BreakableBody.BodyMaterial.GLASS:
                    return glassSFX;
            }

            return true;
        }

        float SFXTime(BreakableBody.BodyMaterial material)
        {
            switch (material)
            {
                case BreakableBody.BodyMaterial.CONCRETE:
                    return cementTime;
                case BreakableBody.BodyMaterial.WOOD:
                    return woodTime;
                case BreakableBody.BodyMaterial.GLASS:
                    return glassTime;
            }

            return 1;
        }

#if WINDOWS_PHONE
        float timeToWaitForSFX = 1 / 30.0f * 3.0f;
#else
        float timeToWaitForSFX = 1 / 60.0f;
#endif
        void SFXPlayed(BreakableBody.BodyMaterial material, bool hit)
        {
            switch (material)
            {
                case BreakableBody.BodyMaterial.CONCRETE:
                    cementSFX = hit;
                    cementTime = timeToWaitForSFX;
                    break;
                case BreakableBody.BodyMaterial.WOOD:
                    woodSFX = hit;
                    woodTime = timeToWaitForSFX;
                    break;
                case BreakableBody.BodyMaterial.GLASS:
                    glassSFX = hit;
                    glassTime = timeToWaitForSFX;
                    break;
            }
        }
        bool ExplosionPlayed = false;
        public override void ProcessEvent(Jabber.Util.Event ev)
        {
            if (!IsTopScreen)
            {
                return;
            }
            if (ev is ExplosionEvent)
            {
                ExplosionPlayed = true;
            }
            if (!ExplosionPlayed)
            {
                /*
                if (ev is BreakableDestroyed && !(ev is BreakableHit))
                {
                    if (!SFXPlayed((ev as BreakableDestroyed).Broken))
                    {
                       // if (SFXTime((ev as BreakableDestroyed).Broken) <= 0)
                        {
                            BreakableBody.PlaySFXBreakForMaterial((ev as BreakableDestroyed).Broken);
                            SFXPlayed((ev as BreakableDestroyed).Broken, true);
                        }
                    }
                    else
                    {
                        int k = 0;
                    }
                }
                else if (ev is BreakableHit)
                {
                    if (!SFXPlayed((ev as BreakableHit).Broken))
                    {
                        if (SFXTime((ev as BreakableHit).Broken) <= 0.0f)
                        {
                            BreakableBody.PlayHitSFXForMaterial((ev as BreakableHit).Broken);
                            SFXPlayed((ev as BreakableHit).Broken, true);
                        }
                    }
                }*/
            }
            if (ev is CannonFireEvent && scene.ToFire.Count > 0)
            {
                CannonFireEvent e = ev as CannonFireEvent;

                Vector2 dir = e.Dir;
                Chicken chick = null;// new Chicken_Dash(world);

                switch (scene.ToFire[scene.ToFire.Count - 1])
                {
                    case 0:
                        chick = new Chicken(scene);
                        break;
                    case 1:
                        chick = new Chicken_Dash(scene);
                        break;
                    case 2:
                        chick = new Chicken_Egg(scene);
                        break;
                    case 3:
                        chick = new Chicken_Explode(scene);
                        break;
                    case 4:
                        chick = new Chicken_Mini_Launcher(scene);
                        break;
                }


                chick.Initialize(Content);
                chick.Position = e.Position;
                chick.Body.LinearVelocity = dir;
                withChicks.AddNode(chick);
                
                bForceInactiveChicken = false;
                bFollowingChicken = true;

                scene.ToFire.RemoveAt(scene.ToFire.Count - 1);
            }
            else if (ev is MenuEvent && IsTopScreen)
            {
                if ((ev as MenuEvent).sender is MenuObj)
                {
                    if (((ev as MenuEvent).sender as MenuObj).Name == "pause")
                    {
                        PauseScreen s = new PauseScreen();
                        s.Initialize(Content);
                        ScreenManager.Get.AddScreen(s);
                    }
                    else if (((ev as MenuEvent).sender as MenuObj).Name == "restart")
                    {
                        Restart();
                        AudioManager.PlayOnce("Sounds/Restart");
                    }
                    else if (((ev as MenuEvent).sender as MenuObj).Name == "forceendgame")
                    {
                        //if (!AnyFox)
                        {
                            for (int i = 0; i < scene.Nodes.Count; i++)
                            {
                                if (scene.Nodes[i] is DonutCase)
                                {
                                    scene.Nodes[i].RaiseFlag(Flags.DELETE);
                                    scene.Nodes[i].UnloadContent();
                                    scene.Nodes.RemoveAt(i); --i;
                                }
                                else if(scene.Nodes[i] is Donut)
                                {
                                    scene.Nodes[i].RaiseFlag(Flags.DELETE);
                                    scene.Nodes[i].UnloadContent();
                                    scene.Nodes.RemoveAt(i); --i;
                                }
                            }
                            //HasWon = true;
                            LevelEndScreen n = new LevelEndScreen(this);
                            n.Initialize(Content);
                            ScreenManager.Get.AddScreen(n);

                            LowerFlag(Jabber.Flags.FADE_OUT);
                            LowerFlag(Jabber.Flags.FADE_IN);

                            if (GetRemainingFox() == 0)
                                AudioManager.PlayOnce("Sounds/Win_Stinger");
                            else
                                AudioManager.PlayOnce("Sounds/Lose_Stinger");
                        }
                    }
                }
            }
            base.ProcessEvent(ev);
        }


        public float rightMostPos = float.MinValue;
        public float leftMostPos = float.MaxValue;
        public FallingFeathers feathers;
    }
}
