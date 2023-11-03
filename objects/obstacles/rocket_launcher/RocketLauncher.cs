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


using ClockBombGames.PixelMan.Events;
using Godot;
using Godot.Collections;


namespace ClockBombGames.PixelMan.GameObjects
{
	public partial class RocketLauncher : GunBase, IGameObject
	{
		[ExportGroup("Components")]
		[Export] private AudioStreamPlayer2D audioStreamPlayer2D;

		[ExportGroup("Properties")]
		[Export] private float bulletSpeed;
		[Export] private float shootInterval = 1f;

		private bool playerIsDead = false;

		private float initialRotation = 0f;
		private float timeToShoot = 0f;
		private float temporalAngle = 0f;


		public override void _Ready()
		{
			initialRotation = GlobalRotationDegrees;
			angle = initialRotation;

			GameEvents.OnPlayerDeath += OnPlayerDeath;
			GameEvents.OnResetGame += OnResetGame;
		}

		public override void _PhysicsProcess(double delta)
		{
			if (playerIsDead) {
				return;
			}

			Player player = GetNearestPlayer();

			if (player != null) {
				LookAt(GlobalPosition.DirectionTo(player.GlobalPosition).Angle(), 10f);
				temporalAngle = GlobalRotationDegrees;

				if (timeToShoot <= 0f) {
					Vector2 from = GlobalPosition + new Vector2(16f * 1.5f, 0).Rotated(GlobalRotation);

					IGOProjectile rocket = Shoot(from, bulletSpeed);

					if (rocket is Rocket _rocket) {
						_rocket.SetTarget(player);
					}

					timeToShoot = shootInterval;
					audioStreamPlayer2D.Play();

				} else {
					timeToShoot -= (float)delta;
				}
			} else {
				temporalAngle++;
				LookAt(Mathf.DegToRad(temporalAngle), 10f);


				timeToShoot = 0.5f;
			}
		}

		private void OnPlayerDeath()
		{
			playerIsDead = true;
		}

		private void OnResetGame()
		{
			playerIsDead = false;

			GlobalRotationDegrees = initialRotation;
			angle = initialRotation;

			timeToShoot = 0.5f;
		}


		public Dictionary Serialize()
		{
			return new() {
				{ "position", GlobalPosition },
				{ "rotation", GlobalRotationDegrees },
				{ "bulletSpeed", bulletSpeed },
				{ "shootInterval", shootInterval }
			};
		}

		public void Deserialize(Dictionary data)
		{
			GlobalPosition = data["position"].AsVector2();
			GlobalRotationDegrees = data["rotation"].AsSingle();

			bulletSpeed = data["bulletSpeed"].AsSingle();
			shootInterval = data["shootInterval"].AsSingle();
		}
	}
}
