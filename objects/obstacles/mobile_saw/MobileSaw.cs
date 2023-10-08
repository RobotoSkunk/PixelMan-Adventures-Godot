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
	public partial class MobileSaw : CharacterBody2D
	{
		[Export] float speed = 1f;


		private bool goPositive = true;


		public override void _PhysicsProcess(double delta)
		{
			Vector2 velocity = Velocity;

			velocity.X = speed * (goPositive ? 1f : -1f);
			velocity.Y += Constants.Gravity * (float)delta;


			Velocity = velocity;

			// Purrr meow meow.
			MoveAndSlide();


			if (IsOnWall())
			{
				goPositive = !goPositive;
			}
		}
	}
}
