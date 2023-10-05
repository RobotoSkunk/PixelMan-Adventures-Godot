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
	public partial class GravitySwitch : Area2D
	{
		[ExportGroup("Components")]
		[Export] VisibleOnScreenNotifier2D screenNotifier2D;
		[Export] AudioStreamPlayer2D streamPlayer;
		[Export] TextureProgressBar progressBar;
		[Export] Sprite2D particles;

		[ExportGroup("Properties")]
		[Export] float reloadTime = 1f;


		float rotationSpeed = 10f;
		float reloadProgress = 1f;


		public override void _Ready()
		{
			rotationSpeed = RSRandom.Range(180f, 270f) * RSRandom.Sign();

			BodyEntered += OnBodyEntered;
		}


		public override void _Process(double delta)
		{
			if (screenNotifier2D.IsOnScreen() && Visible){

				// Particles
				particles.Visible = reloadProgress >= 1f;

				if (particles.Visible) {
					particles.RotationDegrees += rotationSpeed * (float)delta;
				}


				// Reload progress indicator
				progressBar.Value = reloadProgress;
			}
		}

		public override void _PhysicsProcess(double delta)
		{
			reloadProgress += (float)delta / reloadTime;
			reloadProgress = RSMath.Clamp01(reloadProgress);


			bool monitor = reloadProgress >= 1f;

			Monitoring = monitor;
			Monitorable = monitor;
		}


		private void OnBodyEntered(Node2D body)
		{
			if (reloadProgress < 1f) {
				return;
			}

			if (body is IGODynamicBody dynamicBody) {
				dynamicBody.SwitchGravity();
				reloadProgress = 0f;

				streamPlayer.Play();
			}
		}
	}
}
