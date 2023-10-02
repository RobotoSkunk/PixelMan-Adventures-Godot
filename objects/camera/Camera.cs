/*
	PixelMan Adventures - An open-source 2D platformer game.
	Copyright (C) 2023 Edgar Lima (RobotoSkunk) <contact@robotoskunk.com>
	Copyright (C) 2023 (Repertix)

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
using ClockBombGames.PixelMan.Utils;
using Godot;
using System;


namespace ClockBombGames.PixelMan.GameObjects 
{
	public partial class Camera : Camera2D
	{
		#region Variables

		[ExportGroup("Components")]
		[Export] public Player target;


		#region Private Variables
		/// <summary>
		///	Original offset without any shake effect or any other interaction
		/// </summary>
		private Vector2 rawOffset = Vector2.Zero;

		/// <summary>
		///	Virtualized offset that is actually applied to the camera
		/// </summary>
		private Vector2 virtualOffset = Vector2.Zero;

		/// <summary>
		///	Original target position without any alteration
		/// </summary>
		private Vector2 rawTargetPosition = Vector2.Zero;

		/// <summary>
		///	Original zoom without any alteration
		/// </summary>
		private float rawZoom = 1f;

		/// <summary>
		/// How strong is the camera actually shaking
		/// </summary>
		private float shakeStrength = 0f;

		/// <summary>
		/// Tween used to shake the camera
		/// </summary>
		private Tween shakeTween;


		#region Player Related Variables

		/// <summary>
		///	The current direction the player is facing to
		/// </summary>
		private int playerDirection = 0;

		/// <summary>
		///	The current's player velocity
		/// </summary>
		private float playerVelocity = 0f;

		#endregion
		#endregion
		#endregion



		public override void _Ready()
		{
			RestoreToTarget();
			GameEvents.OnResetGame += RestoreToTarget;
			GameEvents.OnPlayerDeath += OnPlayerDeath;
		}

		public override void _Process(double delta)
		{
			if (target != null) {
				rawZoom = 1 + 4f * playerVelocity / Constants.maxSpeed;

				rawOffset.X = playerDirection * 32f;

				rawTargetPosition = target.GlobalPosition + rawOffset;
			} else {
				playerDirection = 0;
				playerVelocity = 0f;
				rawOffset = Vector2.Zero;
				rawZoom = 1f;
			}


			// Apply zoom
			Zoom = RSMath.Lerp(Zoom, Vector2.One * (5f - rawZoom), 0.01f * RSMath.FixedDelta(delta));


			// Apply offset
			Vector2 shake = Vector2.Zero;

			if (shakeStrength > 0f) {
				shake = 2f * RSRandom.Circle2D() * shakeStrength * rawZoom;
			}

			virtualOffset += (rawOffset - Offset) / 15f * RSMath.FixedDelta(delta);

			Offset = virtualOffset + shake;


			// Apply position
			GlobalPosition += (rawTargetPosition - GlobalPosition) / 8f * RSMath.FixedDelta(delta);
		}

		public override void _PhysicsProcess(double delta)
		{
			if (target != null) {
				playerVelocity = target.Velocity.Length();

				if (Mathf.Abs(target.Velocity.X) > 16f) {
					playerDirection = Mathf.Sign(target.Velocity.X);
				}
			} else {
				playerVelocity = 0f;
				playerDirection = 0;
			}
		}


		private void RestoreToTarget()
		{
			rawOffset = Vector2.Zero;
			rawTargetPosition = target.GlobalPosition;

			playerDirection = 0;
			playerVelocity = 0f;
			rawOffset = Vector2.Zero;
			rawZoom = 1f;
		}

		private async void Shake(float strength, float duration)
		{
			shakeTween?.Kill();
			shakeTween = CreateTween();

			shakeTween.TweenProperty(this, "shakeStrength", 0f, duration);
			shakeStrength = strength;
			shakeTween.Play();

			await ToSignal(shakeTween, "finished");
			shakeTween.Kill();
			shakeStrength = 0f;
		}

		private void OnPlayerDeath()
		{
			Shake(2f, 0.3f);
		}
	}
}
