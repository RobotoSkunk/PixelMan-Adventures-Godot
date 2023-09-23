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


namespace ClockBombGames.PixelMan
{
	/// <summary>
	/// The Director is the main node of the game.
	/// </summary>
	public partial class Director : Node2D
	{
		[Export] private int avatarIndex = 0;
		[Export] private SpriteFrames[] avatars;

		public SpriteFrames[] Avatars
		{
			get => avatars;
		}


		public override void _Ready()
		{
			this.SetDirector();
			Globals.AvatarIndex = avatarIndex;
		}
	}
}
