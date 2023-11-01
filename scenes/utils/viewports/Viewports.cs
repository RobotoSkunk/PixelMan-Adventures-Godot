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


using System;
using ClockBombGames.PixelMan.GameObjects;
using Godot;
using Godot.Collections;


namespace ClockBombGames.PixelMan.Utils
{
	public partial class Viewports : CanvasLayer
	{
		[Export] GridContainer container;
		[Export] Node2D world;
		[Export] Array<PlayerViewport> viewports;

		PlayerViewport firstViewport;

		public Node2D World
		{
			get
			{
				return world;
			}
		}


		public override void _Ready()
		{
			container.Columns = 1;

			this.RegisterViewportsContainer();
		}

		[Obsolete("Use AddToContainer(PlayerViewport viewport) instead.")]
		public void AddToContainer(PlayerViewport viewport)
		{
			// // Move the viewport to the container
			// viewport.GetParent().CallDeferred("remove_child", viewport);
			// container.CallDeferred("add_child", viewport);

			// if (firstViewport == null) {
			// 	firstViewport = viewport;

			// 	// Move the world to the first viewport
			// 	world.GetParent().CallDeferred("remove_child", world);
			// 	viewport.SubViewport.CallDeferred("add_child", world);

			// 	// Attach the world to the viewport attribute
			// 	viewport.World = world;

			// } else {
			// 	container.Columns = 2;

			// 	viewport.SubViewport.World2D = firstViewport.SubViewport.World2D;
			// }
		}

		/// <summary>
		/// Add a player to the first available viewport
		/// </summary>
		public void AddPlayer(Player player)
		{
			foreach (PlayerViewport viewport in viewports) {
				if (viewport.InUse) {
					continue;
				}

				viewport.Camera.Target = player;
				viewport.SetWorld2D(world.GetWorld2D());
				viewport.Visible = true;
				return;
			}

			GD.PushWarning("No more viewports available.");
		}

		public Array<PlayerViewport> GetViewports()
		{
			return viewports;
		}
	}
}
