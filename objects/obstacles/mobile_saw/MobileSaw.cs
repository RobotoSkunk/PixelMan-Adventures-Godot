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


using ClockBombGames.PixelMan.Utils;
using Godot;


namespace ClockBombGames.PixelMan.GameObjects
{
	public partial class MobileSaw : DynamicBody
	{
		[ExportGroup("Components")]
		[Export] AnimatedSprite2D sprite;

		private bool goPositive = true;


		public override void _Ready()
		{
			sprite.Play();
		}

		public override void _PhysicsProcess(double delta)
		{
			Vector2 velocity = ProcessVelocity(speed.X * (goPositive ? 1f : -1f) * 1.2f, true, (float)delta);

			ApplyVelocity(velocity);

			// Purrr meow meow.
			MoveAndSlide();

			sprite.Offset = new Vector2(0f, RSRandom.Circle());

			if (IsOnWall())
			{
				Velocity = new Vector2(0f, Velocity.Y);
				goPositive = !goPositive;
			}
		}

		public override void Impulse(float direction, float force)
		{
			base.Impulse(direction, force);

			Vector2 directionVector = Vector2.Right.Rotated(direction);

			if (Mathf.Abs(directionVector.X) > 0.1f) {
				goPositive = directionVector.X > 0f;
			}
		}
	}
}
