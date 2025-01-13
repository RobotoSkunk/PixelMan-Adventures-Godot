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
using Godot.Collections;
using Godot;


namespace ClockBombGames.PixelMan.GameObjects
{
	public partial class Rocket : Area2D, IGOProjectile
	{
		[ExportGroup("Components")]
		[Export] private AnimatedSprite2D animatedSprite2D;
		[Export] private CollisionShape2D collisionShape;
		[Export] private GpuParticles2D trailParticles;
		[Export] private GpuParticles2D explosionParticles;
		[Export] private AudioStreamPlayer2D rocketAudio;
		[Export] private AudioStreamPlayer2D explosionAudio;

		[ExportGroup("Properties")]
		[Export] private float factor;


		private Vector2 velocity = Vector2.Zero;

		private bool isActive = false;
		private bool wasActive = false;

		Player target = null;
		float direction;
		float speed;
		float angle;


		public bool IsActive
		{
			get => isActive;
		}


		public override void _Ready()
		{
			BodyEntered += OnBodyEntered;
			AreaEntered += OnAreaEntered;

			GameEvents.OnResetGame += OnResetGame;
		}

		public override void _Process(double delta)
		{
			if (isActive) {
				if (!rocketAudio.Playing) {
					rocketAudio.Play();
				}

				if (!trailParticles.Emitting) {
					trailParticles.Emitting = true;
				}

			} else {
				if (rocketAudio.Playing) {
					rocketAudio.Stop();
				}

				if (trailParticles.Emitting) {
					trailParticles.Emitting = false;
				}
			}
		}

		public override void _PhysicsProcess(double delta)
		{
			if (isActive) {
				UpdateDirection();

				GlobalPosition += velocity * (float)delta;

				wasActive = true;

			} else if (wasActive) {
				wasActive = false;

				Globals.Shake(1f, 0.3f);

				explosionAudio.Play();

				explosionParticles.Restart();
				explosionParticles.Emitting = true;

				animatedSprite2D.Stop();
			}

			Monitoring = isActive;
			Monitorable = isActive;
			animatedSprite2D.Visible = isActive;
		}

		private void OnResetGame()
		{
			isActive = false;
			wasActive = false;

			explosionAudio.Stop();

			explosionParticles.Restart();
			explosionParticles.Emitting = false;

			trailParticles.Restart();
			trailParticles.Emitting = false;

			animatedSprite2D.Stop();
		}

		private void OnBodyEntered(Node body)
		{
			Destroy();

			if (body is Player player) {
				player.KillPlayer();
			}
		}

		private void OnAreaEntered(Node _)
		{
			Destroy();
		}


		public void ShootAt(Vector2 position, float direction, float speed)
		{
			GlobalPosition = position;

			this.direction = direction;
			this.speed = speed;

			isActive = true;
			UpdateDirection(false);

			animatedSprite2D.Play();
		}

		public void SetTarget(Player target)
		{
			this.target = target;
		}

		void UpdateDirection(bool smooth = true)
		{
			if (target != null) {
				direction = GlobalPosition.DirectionTo(target.GlobalPosition).Angle();
			}

			if (smooth) {
				angle += Mathf.Sin(direction - Mathf.DegToRad(angle)) * factor;
			} else {
				angle = Mathf.RadToDeg(direction);
			}

			GlobalRotationDegrees = angle;

			velocity = new Vector2(speed, 0).Rotated(Mathf.DegToRad(angle));
		}

		public void Destroy()
		{
			if (!isActive) {
				return;
			}

			isActive = false;
		}
	}
}
