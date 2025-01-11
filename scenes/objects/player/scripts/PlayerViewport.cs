/*
	PixelMan Adventures - An open-source 2D platformer game.
	Copyright (C) 2023 Edgar Lima (RobotoSkunk) <contact@robotoskunk.com>
	Copyright (C) 2023 Repertix

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


using ClockBombGames.PixelMan.GameObjects;
using Godot;


namespace ClockBombGames.PixelMan.Utils
{
	public partial class PlayerViewport : SubViewportContainer
	{
		[Export] PlayerCamera camera;
		[Export] SubViewport subViewport;

		public PlayerCamera Camera
		{
			get
			{
				return camera;
			}
		}

		public bool InUse
		{
			get
			{
				return camera.TargetPlayer != null;
			}
		}


		public override void _Process(double delta)
		{
			subViewport.World2D = Globals.World?.GetWorld2D();
			subViewport.AudioListenerEnable2D = InUse;
		}
	}
}
