/*
	PixelMan Adventures - An open-source 2D platformer game.
	Copyright (C) 2025 Edgar Lima (RobotoSkunk) <contact@robotoskunk.com>

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


using ClockBombGames.PixelMan.Events;
using Godot;
using Godot.Collections;


namespace ClockBombGames.PixelMan.GameObjects
{
	public partial class WallGun : GunBase, IGameObject
	{
		[ExportGroup("Components")]
		[Export] private AudioStreamPlayer2D audioStreamPlayer2D;

		[ExportGroup("Properties")]
		[Export] private float bulletSpeed;
		[Export] private float shootInterval = 1f;
		[Export] private float delayAtStart = 0f;

		private float timeToShoot = 0f;


		public override void _Ready()
		{
			GameEvents.OnResetGame += ResetTimeToShoot;
			ResetTimeToShoot();
		}

		public override void _PhysicsProcess(double delta)
		{
			if (timeToShoot <= 0f) {
				Shoot(GlobalPosition, bulletSpeed);

				timeToShoot = shootInterval;
				audioStreamPlayer2D.Play();

			} else {
				timeToShoot -= (float)delta;
			}
		}

		private void ResetTimeToShoot()
		{
			timeToShoot = delayAtStart;
		}

		public void Deserialize(Dictionary data)
		{
			throw new System.NotImplementedException();
		}

		public Dictionary Serialize()
		{
			throw new System.NotImplementedException();
		}
	}
}
