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


		/// <summary>
		/// Tells if the saw is moving in the positive direction.
		/// </summary>
		private bool goPositive = true;

		/// <summary>
		/// The normal vector of the floor.
		/// </summary>
		private Vector2 floorNormal;

		/// <summary>
		/// The raw angle of the object.
		/// </summary>
		private float rawAngle = 0f;


		public override void _Ready()
		{
			sprite.Play();
		}

		public override void _PhysicsProcess(double delta)
		{
			ApplyVelocity(
				ProcessVelocity(speed.X * (goPositive ? 1f : -1f) * 1.5f, true, (float)delta)
			);

			// Purrr meow meow.
			MoveAndSlide();

			sprite.Offset = new Vector2(0f, RSRandom.Circle());

			if (IsOnWall())
			{
				Velocity = new Vector2(0f, Velocity.Y);
				goPositive = !goPositive;
			}


			if (IsOnFloor()) {
				floorNormal = GetFloorNormal();

				float floorAngle = floorNormal.Angle();

				if (Mathf.RadToDeg(floorAngle) > 0f) {
					floorAngle -= Mathf.DegToRad(180f);
				}

				rawAngle = floorAngle + Mathf.DegToRad(90f);
			} else {
				floorNormal = Vector2.Zero;

				rawAngle = 0f;
			}

			Rotation = Mathf.Lerp(Rotation, rawAngle, 0.33f);
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
