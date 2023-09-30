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
using ClockBombGames.PixelMan.GameObjects;
using ClockBombGames.PixelMan.Utils;
using Godot;
using Godot.Collections;
using System;
using System.Reflection.Metadata;

namespace ClockBombGames.PixelMan.GameObjects 
{
	public partial class Camera : Camera2D
	{
		#region Variables

		[ExportGroup("Components")]
		[Export] public Player target;

		#region Private Variables
		/// <summary>
		///	Original offset w/o any shake effect or any other interaction
		/// </summary>
		private Vector2 originalOffset = Vector2.Zero;

		/// <summary>
		///	Original zoom w/o any alteration
		/// </summary>
		private Vector2 originalZoom = new(3.005f, 3.005f);

		/// <summary>
		/// How strong is the camera actually shaking
		/// </summary>
		private float shakeStrength = 0f;

		/// <summary>
		///	Determines if the camera is following it's current target
		/// </summary>
		private bool followingTarget = false;

		#region Player Related Variables

		/// <summary>
		///	The current direction the player is facing to
		/// </summary>
		private int playerDirection = 0;

		/// <summary>
		///	Substracts current zoom depending on Player's velocity
		/// </summary>
		private float velocityZoom = 0f;

		/// <summary>
		/// How intense the camera is affected by Player's velocity
		/// </summary>
		private float velocityZoomIntensity = 0.003f;

		#endregion

		#endregion

		#endregion

		#region Tweens

		private Tween moveTween;
		private Tween zoomTween;
		private Tween shakeTween;

		#endregion

		public override void _Ready()
		{
			RestoreToTarget();
			GameEvents.OnResetGame += RestoreToTarget;
			GameEvents.OnPlayerDeath += OnPlayerDeath;
		}

		public override void _Process(double delta)
		{
			if (followingTarget) {

				if (target != null) {
					if (Mathf.Abs(target.Velocity.X) > 0.1f) {
						playerDirection = Math.Sign(target.Velocity.X);
						velocityZoom = Mathf.Lerp(velocityZoom, Mathf.Abs(target.Velocity.X) / Constants.maxSpeed, 0.01f) + velocityZoomIntensity;
					} else if (target.Velocity.X == 0) {
						velocityZoom = Mathf.Lerp(velocityZoom, 0f, 0.01f);
					}

					originalOffset.X = Mathf.Lerp(originalOffset.X, playerDirection * 48f, 0.01f);
				}
			}


			GlobalPosition = target.GlobalPosition;
			Offset = originalOffset + (RSRandom.Circle2D() * shakeStrength);
			Zoom = originalZoom + new Vector2(-velocityZoom, -velocityZoom);
		}

		private async void RestoreToTarget()
		{
			if (!followingTarget) {
				ZoomOn(3.005f);
				MoveTo(target.GlobalPosition);
				await ToSignal(moveTween, "finished");
				followingTarget = true;
			}
		}

		private async void MoveTo(Vector2 newPosition,
								  float movingTime = 0.3f,
								  Tween.TransitionType transitionType = Tween.TransitionType.Cubic,
								  Tween.EaseType easingType = Tween.EaseType.Out)
		{
			moveTween?.Kill();
			moveTween = CreateTween();

			moveTween.SetEase(easingType);
			moveTween.SetTrans(transitionType);

			moveTween.TweenProperty(this, "global_position", newPosition, movingTime);
			moveTween.Play();

			await ToSignal(moveTween, "finished");
			moveTween.Kill();
		}

		private async void ZoomOn(float newZoom,
								  double zoomingTime = 0.3,
								  Tween.TransitionType transitionType = Tween.TransitionType.Cubic,
								  Tween.EaseType easingType = Tween.EaseType.Out)
		{
			zoomTween?.Kill();
			zoomTween = CreateTween();

			zoomTween.SetEase(easingType);
			zoomTween.SetTrans(transitionType);

			zoomTween.TweenProperty(this, "zoom", new Vector2(newZoom, newZoom), zoomingTime);
			zoomTween.Play();

			await ToSignal(zoomTween, "finished");
			zoomTween.Kill();
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
			followingTarget = false;
		}
	}
}
