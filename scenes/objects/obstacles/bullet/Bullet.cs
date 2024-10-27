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


namespace ClockBombGames.PixelMan.GameObjects
{
	public partial class Bullet : Area2D, IGOProjectile
	{
		[ExportGroup("Components")]
		[Export] private AnimatedSprite2D animatedSprite2D;
		[Export] private CollisionShape2D collisionShape;
		[Export] private GpuParticles2D particles2d;
		[Export] private AudioStreamPlayer2D audioStreamPlayer2D;

		private Vector2 velocity = Vector2.Zero;

		private bool isActive = false;
		private bool wasActive = false;


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

		public override void _PhysicsProcess(double delta)
		{
			if (isActive) {
				GlobalPosition += velocity * (float)delta;

				wasActive = true;

			} else if (wasActive) {
				wasActive = false;

				audioStreamPlayer2D.Play();

				particles2d.Restart();
				particles2d.Emitting = true;

				animatedSprite2D.Stop();
			}

			Monitoring = isActive;
			Monitorable = isActive;
			animatedSprite2D.Visible = isActive;
		}

		private void OnResetGame()
		{
			isActive = false;
		}

		private void OnBodyEntered(Node body)
		{
			if (!isActive) {
				return;
			}

			isActive = false;

			if (body is Player) {
				Globals.PlayerDied();
			}
		}

		private void OnAreaEntered(Node _)
		{
			if (!isActive) {
				return;
			}

			isActive = false;
		}


		public void ShootAt(Vector2 position, float direction, float speed)
		{
			GlobalPosition = position;
			GlobalRotation = direction;
			velocity = new Vector2(speed, 0).Rotated(direction);
			isActive = true;

			animatedSprite2D.Play();
		}
	}
}
