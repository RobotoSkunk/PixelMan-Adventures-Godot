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
	public partial class Gun : GunBase
	{
		[Export] private float bulletSpeed;

		float timeToShoot = 0f;


		public override void _PhysicsProcess(double delta)
		{
			Player player = GetNearestPlayer();

			if (player != null) {
				LookAt(GlobalPosition.DirectionTo(player.GlobalPosition).Angle(), 10f);

				if (timeToShoot <= 0f) {
					Vector2 from = GlobalPosition + new Vector2(16f * 1.5f, 0).Rotated(GlobalRotation);

					Shoot(from, bulletSpeed);

					timeToShoot = 1f;

				} else {
					timeToShoot -= (float)delta;
				}
			} else {
				timeToShoot = 0.5f;
			}
		}
	}
}
