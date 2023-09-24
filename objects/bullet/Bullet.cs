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
	public partial class Bullet : Area2D, IGOProjectile
	{
		[Export] private Sprite2D sprite2d;
		[Export] private CollisionShape2D collisionShape;

		private Vector2 velocity = Vector2.Zero;
		private bool isActive = false;


		public bool IsActive
		{
			get => isActive;
		}


		public override void _Ready()
		{
			BodyEntered += (other) =>
			{
				isActive = false;

				if (other is Player) {
					Globals.PlayerDied();
				}
			};

			AreaEntered += (other) =>
			{
				isActive = false;
			};
		}

		public override void _PhysicsProcess(double delta)
		{
			if (isActive) {
				GlobalPosition += velocity * (float)delta;
			}

			Monitoring = isActive;
			sprite2d.Visible = isActive;			
		}

		public void ShootAt(Vector2 position, float direction, float speed)
		{
			GlobalPosition = position;
			GlobalRotation = direction;
			velocity = new Vector2(speed, 0).Rotated(direction);
			isActive = true;

			Monitoring = false;
			sprite2d.Visible = true;
		}
	}
}
