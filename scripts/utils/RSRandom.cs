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
	/// <summary>
	///	Provides a set of methods to generate random numbers.
	/// </summary>
	public static class RSRandom
	{
		private readonly static System.Random random = new();


		/// <summary>
		///	Generates a random number between 0.0 and 1.0.
		/// </summary>
		public static float Single()
		{
			return random.NextSingle();
		}

		/// <summary>
		///	Generates a random number with a given range (inclusive).
		/// </summary>
		public static int Range(int min, int max)
		{
			return random.Next(min, max);
		}

		/// <summary>
		///	Generates a random number with a given range (inclusive).
		/// </summary>
		public static float Range(float min, float max)
		{
			return min + Single() * (max - min);
		}

		/// <summary>
		///	Generates a random number between 0 and the given value (exclusive).
		/// </summary>
		public static int RangeArray(int arrayLength)
		{
			return Range(0, arrayLength - 1);
		}

		/// <summary>
		///	Generates a random number between -1.0 and 1.0.
		/// </summary>
		public static float Circle()
		{
			return Range(-1f, 1f);
		}

		/// <summary>
		///	Generates a random vector2d with values between -1.0 and 1.0.
		/// </summary>
		public static Vector2 Circle2D()
		{
			return new Vector2(Circle(), Circle());
		}
	}
}
