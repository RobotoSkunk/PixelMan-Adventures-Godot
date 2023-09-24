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


namespace ClockBombGames.PixelMan.GameObjects
{
	public partial class Platform : CharacterBody2D
	{
		[Export] private float speed = 1f;
		[Export] private PlatformType type = PlatformType.Normal;
		[Export] private bool startPositive = true;


		private bool goPositive = true;

		private float delayToMove = 0f;

		private Vector2 startPosition = Vector2.Zero;
		private Vector2 previousPosition = Vector2.Zero;

		public enum PlatformType {
			Normal,
			Vertical,
		}


		public override void _Ready()
		{
			startPosition = Position;
			goPositive = startPositive;
		}

		public override void _PhysicsProcess(double delta)
		{
			if (delayToMove > 0f) {
				delayToMove -= (float)delta;
				Velocity = Vector2.Zero;

				previousPosition = Vector2.One * Mathf.Inf;
			} else {
				if (type == PlatformType.Vertical) {
					Velocity = speed * (goPositive ? Vector2.Down : Vector2.Up);
				} else {
					Velocity = speed * (goPositive ? Vector2.Right : Vector2.Left);
				}

				if (previousPosition.DistanceSquaredTo(Position) < 0.1f) {
					goPositive = !goPositive;
					delayToMove = 0.25f;
				}

				previousPosition = Position;
			}


			// Purrr meow meow.
			MoveAndSlide();

			// Freeze the platform in the correct position.
			if (type == PlatformType.Vertical) {
				Position = new Vector2(startPosition.X, Position.Y);
			} else {
				Position = new Vector2(Position.X, startPosition.Y);
			}
		}
	}
}
