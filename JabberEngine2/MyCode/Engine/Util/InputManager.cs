using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using System.IO.IsolatedStorage;

using Microsoft.Xna.Framework.Input;
using Jabber.GameScreenManager;


namespace Jabber.Util
{
    /// <summary>
    /// Helper for reading input from keyboard, gamepad, and touch input. This class 
    /// tracks both the current and previous state of the input devices, and implements 
    /// query methods for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputManager : JabJect
    {
        #region singleton
        static InputManager manager = null;
        static public InputManager Get
        {
            get
            {
                if (manager == null)
                {
                    manager = new InputManager();
                }
                return manager;
            }
        }



        #endregion
        #region Fields

        public const int MaxInputs = 4;

        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly GamePadState[] CurrentGamePadStates;

        public readonly KeyboardState[] LastKeyboardStates;
        public readonly GamePadState[] LastGamePadStates;

        public readonly bool[] GamePadWasConnected;

        //public TouchCollection TouchState;

        //public readonly List<GestureSample> Gestures = new List<GestureSample>();

        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputManager()
        {
            //TouchPanel.EnabledGestures = GestureType.Tap;
            //bool b = TouchPanel.GetCapabilities().IsConnected;
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            CurrentGamePadStates = new GamePadState[MaxInputs];

            LastKeyboardStates = new KeyboardState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];

            GamePadWasConnected = new bool[MaxInputs];

            
#if WINDOWS
#endif
        }


        #endregion

        #region Public Methods

        public static bool FingerPressed
        {
            get { return (Mouse.GetState().LeftButton == ButtonState.Pressed); }
        }
        public static int FingerXPos
        {
            get { return Mouse.GetState().X; }
        }
        public static int FingerYPos
        {
            get {
                return Mouse.GetState().Y; 
            }
        }

        class FingerPosID
        {
            public int id = int.MinValue;
            public Vector2 FingerPos = Vector2.Zero;
          //  public bool valid = false;
        }
        FingerPosID Finger1 = new FingerPosID();
        FingerPosID Finger2 = new FingerPosID();

        public int Finger1ID
        {
            get
            {
                return Finger1.id;
            }
        }
        public int Finger2ID
        {
            get
            {
                return Finger2.id;
            }
        }

        public bool Finger1Valid
        {
            get { return Finger1.id >= 0; }
        }
        public bool Finger2Valid
        {
            get { return Finger2.id >= 0; }
        }
        public Vector2 Finger1Pos
        {
            get { return Finger1.FingerPos; }
        }
        public Vector2 Finger2Pos
        {
            get { return Finger2.FingerPos; }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime dt)
        {
            UpdateInput();
            base.Update(dt);
        }

#if WINDOWS
        public override void OnMouseScroll(int delta)
        {
            ScreenManager.Get.OnMouseScroll(delta);
        }

        int mouseScrollLast = 0;
#endif

        MouseState lastMouseState;

        void UpdateInput()
        {
#if WINDOWS_PHONE
            TouchCollection touchCollection = TouchPanel.GetState();

            bool idStillValid = false;
            if (Finger1Valid)
            {
                foreach (TouchLocation touch in touchCollection)
                {
                    if (touch.Id == Finger1.id)
                    {
                        idStillValid = true;
                        break;
                    }
                }
                if (!idStillValid)
                {
                    Finger1.id = -1;
                }
            }
            idStillValid = false;
            if (Finger2Valid)
            {
                foreach (TouchLocation touch in touchCollection)
                {
                    if (touch.Id == Finger2.id)
                    {
                        idStillValid = true;
                        break;
                    }
                }
                if (!idStillValid)
                {
                    Finger2.id = -1;
                }
            }

            foreach (TouchLocation touch in touchCollection)
            {
                if ((!Finger1Valid || Finger1.id == touch.Id) && Finger2.id != touch.Id)
                {
                    Finger1.id = touch.Id;
                    Finger1.FingerPos = touch.Position;
                }
                if ((!Finger2Valid || Finger2.id == touch.Id) && Finger1.id != touch.Id)
                {
                    Finger2.id = touch.Id;
                    Finger2.FingerPos = touch.Position;
                }
            }

            Finger1.FingerPos /= BaseGame.Get.BackBufferDimensions;
            Finger2.FingerPos /= BaseGame.Get.BackBufferDimensions;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                OnBackPressed();
            } 
#endif

            MouseState mouseState = Mouse.GetState();

            Point lastPoint = new Point(lastMouseState.X, lastMouseState.Y);
            Point point = new Point(mouseState.X, mouseState.Y);

            if (lastPoint != point)
            {
                OnMouseMove(point);
            }

            if (lastMouseState.LeftButton != mouseState.LeftButton)
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                    OnMouseDown(point);
                else
                    OnMouseUp(point);
            }

            MouseDrag(point);



            
#if WINDOWS
            int dif = mouseState.ScrollWheelValue - lastMouseState.ScrollWheelValue;
            if(dif != 0)
            {
                OnMouseScroll(dif);
            }
#endif

            lastMouseState = mouseState;
        }

        const int tapRadius = 3;
        Point mouseDownPoint;
        bool mouseIsDown = false;

        void OnMouseDown(Point point)
        {
            Vector2 vector = new Vector2(point.X, point.Y) / BaseGame.Get.BackBufferDimensions;
            //if (OnPressed != null)
            {
                ScreenManager.Get.OnPress(vector);
              //  OnPressed.Invoke(vector);
            }
            mouseDownPoint = point;

            mouseIsDown = true;
            lastMousePosition = vector;
        }

        void OnMouseUp(Point point)
        {
            Vector2 vector = new Vector2(point.X, point.Y) / BaseGame.Get.BackBufferDimensions;
           // if (OnRelease != null)
            {
                ScreenManager.Get.OnRelease(vector);
                //OnRelease.Invoke(vector);
            }

            int dx = point.X - mouseDownPoint.X;
            int dy = point.Y - mouseDownPoint.Y;
            if ((dx * dx) + (dy * dy) < tapRadius * tapRadius)
                OnMouseTap(point);

            mouseIsDown = false;
        }

#if WINDOWS_PHONE || ANDROID
        void OnBackPressed()
        {
            ScreenManager.Get.OnBackPress();
        }
#endif

        void OnMouseMove(Point point)
        {
            Vector2 vector = new Vector2(point.X, point.Y) / BaseGame.Get.BackBufferDimensions;
            //if (OnMove != null)
            {
                ScreenManager.Get.OnMove(vector);
               // OnMove.Invoke(vector);
            }
        }
        Vector2 lastMousePosition = Vector2.Zero;
        void MouseDrag(Point point)
        {
            Vector2 vector = new Vector2(point.X, point.Y) / BaseGame.Get.BackBufferDimensions;
            if (mouseIsDown)
            {
                ScreenManager.Get.OnDrag(lastMousePosition, vector);
                lastMousePosition = vector;
            }
        }
        void OnMouseTap(Point point)
        {
            Vector2 vector = new Vector2(point.X, point.Y) / BaseGame.Get.BackBufferDimensions;
            //if (OnTap != null)
            {
                ScreenManager.Get.OnTap(vector);
                //OnTap.Invoke(vector);
            }

            AdSystem.OnTap(vector);
        }

        public delegate void OnTouchInputEvent(Vector2 point);

        /// <summary>
        /// Helper for checking if a key was newly pressed during this update. The
        /// controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a keypress
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer,
                                            out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentKeyboardStates[i].IsKeyDown(key) &&
                        LastKeyboardStates[i].IsKeyUp(key));
            }
            else
            {
                // Accept input from any player.
                return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
            }
        }


        /// <summary>
        /// Helper for checking if a button was newly pressed during this update.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a button press
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer,
                                                     out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentGamePadStates[i].IsButtonDown(button) &&
                        LastGamePadStates[i].IsButtonUp(button));
            }
            else
            {
                // Accept input from any player.
                return (IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                        IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
            }
        }


        /// <summary>
        /// Checks for a "menu select" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuSelect(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
                   IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu cancel" input action.
        /// The controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When the action
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsMenuCancel(PlayerIndex? controllingPlayer,
                                 out PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu up" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuUp(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "menu down" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsMenuDown(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex);
        }


        /// <summary>
        /// Checks for a "pause the game" input action.
        /// The controllingPlayer parameter specifies which player to read
        /// input for. If this is null, it will accept input from any player.
        /// </summary>
        public bool IsPauseGame(PlayerIndex? controllingPlayer)
        {
            PlayerIndex playerIndex;

            return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex) ||
                   IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
        }


        #endregion
    }
}