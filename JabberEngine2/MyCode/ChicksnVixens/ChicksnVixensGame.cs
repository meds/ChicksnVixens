using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Jabber;
using ChicksnVixens.Screens;
using Jabber.GameScreenManager;
using Jabber.Util;
using Jabber.Media;
namespace ChicksnVixens
{
	public class ChicksnVixensGame : BaseGame
	{
        public ChicksnVixensGame():base()
		{

			IsMouseVisible = true;
			Content.RootDirectory = "Content";


            locationOrder.Add("uluru");
            locationOrder.Add("paris");
            locationOrder.Add("bavaria");
            locationOrder.Add("vesuvius");
            locationOrder.Add("polar");
        }

        static public ChicksnVixensGame Get
        {
            get
            {
                return baseGame as ChicksnVixensGame;
            }
        }

		protected override void LoadContent()
		{		
			base.LoadContent();
            SetupLevels();
            LoadFromFile();
            LoadSettings();

            //GameplayScreen s = new GameplayScreen("uluru", 11);
            MainMenuScreen s = new MainMenuScreen();
            s.Initialize(Content);
            ScreenManager.Get.AddScreen(s);
        }

        public bool LevelUnlocked(string country, int level)
        {
            if (level < 5 && IsLocationUnlocked(country))
            {
                return true;
            }
            float stars = GetTotalStars(country);
            stars /= (float)2.0f;
            
            return stars > level || GetLevelState(country, level-1).NumStars == 3;
        }

        public int GetTotalLevels(string country)
        {
            int num = 0;
            for (int j = 0; j < levels.Count; j++)
            {
                if (levels[j].CountryName == country)
                {
                    ++num;
                }
            }
            return num;
        }
        public int GetTotalStars(string country)
        {
            int i = 0;
            for (int j = 0; j < levels.Count; j++)
            {
                if (levels[j].CountryName == country)
                {
                    i += levels[j].NumStars;
                }
            }

            return i;
        }

        public void PlayCurrentLocationSotry(string curlocation)
        {
            Screen s = null;
            switch (curlocation)
            {
                case "uluru":
                    List<string> pages = new List<string>();
                    for (int i = 1; i < 9; i++)
                        pages.Add("Story/australia/p" + i);
                    s = new StoryScreen(pages);
                    s.Initialize(Content);
                    ScreenManager.Get.AddScreen(s);
                    break;
                case "paris":
                    s = new FranceStory();
                    s.Initialize(Content);
                    ScreenManager.Get.AddScreen(s);
                    break;
                case "bavaria":
                    s = new GermanyStory();
                    s.Initialize(Content);
                    ScreenManager.Get.AddScreen(s);
                    break;
                case "vesuvius":
                    s = new VesuviusStory();
                    s.Initialize(Content);
                    ScreenManager.Get.AddScreen(s);
                    break;
                case "polar":
                    s = new PolarStory();
                    s.Initialize(Content);
                    ScreenManager.Get.AddScreen(s);
                    break;
            }
        }

        public void PlayNextLocationStory(string curlocation)
        {
            Screen s = null;
            switch (curlocation)
            {
                case "uluru":
                    s = new FranceStory();
                    s.Initialize(Content);
                    ScreenManager.Get.AddScreen(s);
                    break;
                case "paris":
                    s = new GermanyStory();
                    s.Initialize(Content);
                    ScreenManager.Get.AddScreen(s);
                    break;
                case "bavaria":
                    s = new VesuviusStory();
                    s.Initialize(Content);
                    ScreenManager.Get.AddScreen(s);
                    break;
                case "vesuvius":
                    s = new PolarStory();
                    s.Initialize(Content);
                    ScreenManager.Get.AddScreen(s);
                    break;
            }
        }

        public class LevelState
        {
            public LevelState()
            {
            }
            public LevelState(string country, int stars, int levelnum, int donuts)
            {
                CountryName = country;
                NumStars = stars;
                LevelNum = levelnum;
                NumDonuts = donuts;
            }
            public int NumDonuts = 0;
            public int NumStars = 0;
            public string CountryName = "";
            public int LevelNum = 0;
            public int Score = 0;
        }
        List<LevelState> levels = new List<LevelState>();

        public void SetupLevels()
        {
            for (int i = 0; i < LevelSelectScreen.NumLevelsUluru; i++)
                levels.Add(new LevelState("uluru", 0, i + 1, 0));
            for (int i = 0; i < LevelSelectScreen.NumLevelsParis; i++)
                levels.Add(new LevelState("paris", 0, i + 1, 0));
            for (int i = 0; i < LevelSelectScreen.NumLevelsBavaria; i++)
                levels.Add(new LevelState("bavaria", 0, i + 1, 0));
            for (int i = 0; i < LevelSelectScreen.NumLevelsPolar; i++)
                levels.Add(new LevelState("polar", 0, i + 1, 0));
            for (int i = 0; i < LevelSelectScreen.NumLevelsVesuvius; i++)
                levels.Add(new LevelState("vesuvius", 0, i + 1, 0));
        }

        public int NumLevels(string loc)
        {
            int ret = 0;
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i].CountryName == loc)
                    ++ret;
            }
            return ret;
        }
        
        public LevelState LastLevel(string loc)
        {
            return GetLevelState(loc, NumLevels(loc));
        }

        public List<string> locationOrder = new List<string>();

        public bool IsLocationUnlocked(string loc)
        {
            int num = locationOrder.IndexOf(loc);
            --num;
            if (num < 0)
                return true;
            return LastLevel(locationOrder[num]).NumStars == 3;
        }

        public LevelState GetLevelState(string countryName, int levelNum)
        {
            for(int i = 0; i < levels.Count; i++)
            {
                if (levels[i].CountryName == countryName && levels[i].LevelNum == levelNum)
                {
                    return levels[i];
                }
            }

            return null;
        }

        public void SetLevel(string countryname, int score, int levelnum, int stars, int donuts)
        {
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i].CountryName == countryname && levels[i].LevelNum == levelnum)
                {
                    if(levels[i].NumStars <= stars)
                        levels[i].NumStars = stars;
                    if (levels[i].NumDonuts <= donuts)
                        levels[i].NumDonuts = donuts;
                    if(levels[i].Score < score)
                        levels[i].Score = score;
                    return;
                }
            }

            LevelState state = new LevelState(countryname, stars, levelnum, 0);
            state.Score = score;
            levels.Add(state);

            SaveToFile();
        }

        public void SaveToFile()
        {
            List<string> toSave = new List<string>();
            for (int i = 0; i < levels.Count; i++)
            {
                toSave.Add(levels[i].CountryName + "#" + levels[i].LevelNum + "#" + levels[i].NumStars + "#" + levels[i].Score + "#" + levels[i].NumDonuts);
            }

            FileSaverLoader.SaveToFile(toSave, saveFile);
        }
        string saveFile = "chix1.dat";
        string miscsave = "miscsave1.dat";
        public bool UseTrajectory = true;
        public void SaveSettings()
        {
            List<string> toSave = new List<string>();
            toSave.Add(AudioManager.MusicVolume.ToString(System.Globalization.CultureInfo.InvariantCulture));
            toSave.Add(AudioManager.SoundVolume.ToString(System.Globalization.CultureInfo.InvariantCulture));
            toSave.Add(UseTrajectory.ToString());
            FileSaverLoader.SaveToFile(toSave, miscsave);
        }
        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            SaveSettings();
            SaveToFile();
        }
        protected override void UnloadContent()
        {
            base.UnloadContent();

            SaveSettings();
            SaveToFile();
        }
        public void LoadSettings()
        {
            List<string> data = FileSaverLoader.LoadFromFile(miscsave);
            if (data == null || data.Count == 0)
            {
                //AudioManager.MusicVolume = 0.5f;
                AudioManager.SoundVolume = 1.0f;
            }
            else
            {
                AudioManager.MusicVolume = float.Parse(data[0].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                AudioManager.SoundVolume = float.Parse(data[1].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                UseTrajectory = bool.Parse(data[2].ToString());
            }
        }

        public void LoadFromFile()
        {
            List<string> data = FileSaverLoader.LoadFromFile(saveFile);
            if (data == null)
            {
                return;
            }
            for (int i = 0; i < data.Count; i++)
            {
                string thisone = data[i];
                string[] splitted = thisone.Split('#');

                LevelState s = new LevelState();
                s.CountryName = splitted[0];
                s.LevelNum = int.Parse(splitted[1], System.Globalization.CultureInfo.InvariantCulture);
                s.NumStars = int.Parse(splitted[2], System.Globalization.CultureInfo.InvariantCulture);
                s.Score = int.Parse(splitted[3], System.Globalization.CultureInfo.InvariantCulture);
                s.NumDonuts = int.Parse(splitted[4], System.Globalization.CultureInfo.InvariantCulture);
                SetLevel(s.CountryName, s.Score, s.LevelNum, s.NumStars, s.NumDonuts);
            }
        }

        #region Input Handling

        MouseState lastMouseState;

		void UpdateInput()
		{
			MouseState mouseState = Mouse.GetState();

			Point lastPoint = new Point(lastMouseState.X, lastMouseState.Y);
			Point point = new Point(mouseState.X, mouseState.Y);

			if(lastPoint != point)
			{
				OnMouseMove(point);
			}

			if(lastMouseState.LeftButton != mouseState.LeftButton)
			{
				if(mouseState.LeftButton == ButtonState.Pressed)
					OnMouseDown(point);
				else
					OnMouseUp(point);
			}

			lastMouseState = mouseState;
		}

		const int tapRadius = 3;
		Point mouseDownPoint;

		void OnMouseDown(Point point)
		{
			mouseDownPoint = point;
		}

		void OnMouseUp(Point point)
		{
			int dx = point.X - mouseDownPoint.X;
			int dy = point.Y - mouseDownPoint.Y;
			if((dx * dx) + (dy * dy) < tapRadius * tapRadius)
				OnTap(point);
		}

		void OnMouseMove(Point point)
		{
		}

		void OnTap(Point point)
		{
		}

		#endregion

		protected override void Update(GameTime gameTime)
		{
			float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

			UpdateInput();
			base.Update(gameTime);
		}


        protected override void Draw(GameTime gameTime)
        {
            try
            {
                base.Draw(gameTime);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }
        }
	}
}
