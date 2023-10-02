/*
	PixelMan Adventures - An open-source 2D platformer game.
	Copyright (C) 2023 Edgar Lima (RobotoSkunk) <contact@robotoskunk.com>

	This program is free software: you can redistribute it and/or modify
	it under the terms of the GNU Affero General Public License as published
	by the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU Affero General Public License for more details.

	You should have received a copy of the GNU Affero General Public License
	along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using Godot;


namespace ClockBombGames.PixelMan.Utils
{
	public static class RSMath
	{
		public static readonly int targetFPS = 60;


		public static Vector2 GetDirection(Vector2 from, Vector2 to)
		{
			return (to - from).Normalized();
		}

		public static Vector2 AngleToVector(float angle)
		{
			return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		}

		public static Vector2 Abs(Vector2 vector)
		{
			return new Vector2(Mathf.Abs(vector.X), Mathf.Abs(vector.Y));
		}

		public static Vector2 Lerp(Vector2 from, Vector2 to, float amount)
		{
			return from + (to - from) * amount;
		}


		public static float Clamp01(float value)
		{
			if (value < 0f) {
				return 0f;
			}

			if (value > 1f) {
				return 1f;
			}

			return value;
		}


		public static float FixedDelta(double delta)
		{
			return (float)delta / (1f / targetFPS);
		}
	}
}
