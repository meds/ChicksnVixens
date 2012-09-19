using System;

namespace Microsoft.Xna.Framework.Input
{
	public static class Mouse
	{
		#region Silverlight Mouse State Tracking

		static internal MouseState currentState;

		#endregion


		#region XNA API

		public static MouseState GetState()
		{
			return currentState;
		}

		#endregion
	}
}
