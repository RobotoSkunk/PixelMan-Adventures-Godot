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
	public partial class PlayerDeathParticle : RigidBody2D
	{
		Vector2 resetPosition;

		bool isResetting = false;
		bool isVisible = false;

		float lateVisibleTimer = 0f;


		public bool LateVisible
		{
			get => Visible;
			set {
				isVisible = value;
				lateVisibleTimer = 0.05f;
			}
		}


		public void Reset(Vector2 position)
		{
			resetPosition = position;
			isResetting = true;
		}

		public override void _PhysicsProcess(double delta)
		{
			if (lateVisibleTimer > 0f) {
				lateVisibleTimer -= (float)delta;

				if (lateVisibleTimer <= 0f) {
					Visible = isVisible;
				}
			}
		}


		// Courtesy of KidsCanCode (https://kidscancode.org/godot_recipes/4.x/physics/asteroids_physics/), thank you!
		public override void _IntegrateForces(PhysicsDirectBodyState2D state)
		{
			if (isResetting) {
				Transform2D transform = state.Transform;

				transform.Origin = resetPosition;

				state.Transform = transform;

				isResetting = false;
			}
		}
	}
}
