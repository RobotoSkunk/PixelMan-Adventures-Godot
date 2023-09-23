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


namespace ClockBombGames.PixelMan.Events
{
	/// <summary>
	/// The GameEvents class is used to trigger events in the game.
	/// </summary>
	public static class GameEvents
	{
		public delegate void GameEvent();


		/// <summary>
		/// When the player dies, this event is triggered.
		/// </summary>
		public static event GameEvent OnPlayerDeath = delegate { };

		/// <summary>
		/// Is triggered when the director request reset all the objects in the game.
		/// </summary>
		public static event GameEvent OnResetGame = delegate { };


		/// <summary>
		/// Invoke the OnPlayerDeath event.
		/// </summary>
		public static void InvokePlayerDeath(this Director director)
		{
			OnPlayerDeath();
		}

		/// <summary>
		/// Invoke the OnResetGame event.
		/// </summary>
		public static void InvokeResetGame(this Director director)
		{
			OnResetGame();
		}
	}
}
