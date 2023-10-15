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


using ClockBombGames.PixelMan.Utils;
using Godot.Collections;
using Godot;
using System.Linq;
using ClockBombGames.PixelMan.Events;


namespace ClockBombGames.PixelMan.GameObjects
{
	public partial class Saw : Area2D
	{
		[ExportCategory("Components")]
		[Export] VisibleOnScreenNotifier2D notifier;
		[Export] Sprite2D sprite;
		[Export] Line2D linePath;

		[ExportGroup("Settings")]
		[Export] Array<Vector2> path;
		[Export] bool returnToStart; // Instead of just reversing the path, it will return directly to the start
		[Export] float speed;


		/// <summary>
		/// If true, the path will be looped.
		/// </summary>
		float rotationSpeed = 0f;

		/// <summary>
		/// The current index of the path.
		/// </summary>
		int pathIndex = 0;

		/// <summary>
		/// If true, the saw is moving forward in the path.
		/// </summary>
		bool goingForward = true;

		/// <summary>
		/// The initial position of the saw.
		/// </summary>
		Vector2 initialPosition = Vector2.Zero;


		/// <summary>
		/// If true, the saw will move.
		/// </summary>
		public bool Enabled { get; set; } = true;

		/// <summary>
		/// If true, it means that the saw has a path to follow.
		/// </summary>
		public bool HasAPath
		{
			get
			{
				return path != null && path.Count > 1;
			}
		}


		public override void _Ready()
		{
			rotationSpeed = RSRandom.Range(600f, 700f) * RSRandom.Sign();
			initialPosition = Position;

			GameEvents.OnResetGame += OnGameReset;

			if (HasAPath) {
				linePath.Points = path.ToArray();

				if (returnToStart) {
					linePath.AddPoint(path[0]);
				}
			}

			linePath.Visible = HasAPath;
		}

		public override void _Process(double delta)
		{
			if (notifier.IsOnScreen() && Enabled) {
				sprite.RotationDegrees += rotationSpeed * (float)delta;
				sprite.Position = RSRandom.Circle2D();
			}

			if (HasAPath) {
				linePath.GlobalPosition = initialPosition;
			}
		}

		public override void _PhysicsProcess(double delta)
		{
			if (HasAPath && Enabled) {
				Vector2 target = initialPosition + path[pathIndex];

				Position = Position.MoveToward(target, speed * (float)delta);


				if (Position.DistanceTo(target) < 1f) {

					// Point to the next target
					if (goingForward) {
						pathIndex++;
					} else {
						pathIndex--;
					}

					// Check if the path is finished
					if (pathIndex >= path.Count) {

						// Check if the path should be looped or reversed
						if (returnToStart) {
							pathIndex = 0;

						} else {
							goingForward = false;

							pathIndex -= 2; // -2 because the index was already incremented
											// and we want to go back to the previous point
						}
					} else if (pathIndex < 0) { // This should only happen if the path is reversed
						goingForward = true;
						pathIndex += 2;
					}
				}
			}
		}


		private void OnGameReset()
		{
			Position = initialPosition;
			goingForward = true;
			pathIndex = 0;
		}
	}
}
