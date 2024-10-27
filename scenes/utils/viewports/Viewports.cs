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


using System.Collections.Generic;
using ClockBombGames.PixelMan.GameObjects;
using Godot;
using Godot.Collections;


namespace ClockBombGames.PixelMan.Utils
{
	public partial class Viewports : CanvasLayer
	{
		[Export] GridContainer container;
		[Export] Array<PlayerViewport> viewports;

		PlayerViewport firstViewport;
		List<Player> players = new();


		public override void _Ready()
		{
			container.Columns = 1;
		}

		public override void _Process(double delta)
		{
			Globals.GetPlayersNonAlloc(ref players);

			for (int i = 0; i < viewports.Count; i++) {
				PlayerViewport viewport = viewports[i];
				viewport.Visible = i < players.Count;

				if (!viewport.Visible) {
					continue;
				}

				Player player = players[i];

				viewport.Camera.Target = player;
			}
		}

		public Array<PlayerViewport> GetViewports()
		{
			return viewports;
		}
	}
}
