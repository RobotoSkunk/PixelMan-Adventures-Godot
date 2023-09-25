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
using ClockBombGames.PixelMan.GameObjects;
using Godot;
using Godot.Collections;
using System;

namespace ClockBombGames.PixelMan.GameObjects 
{
	public partial class Camera : Camera2D, IGameObject
	{
		#region Variables

		[ExportGroup("Components")]
		[Export] public Player targetPlayer;
		[Export] public Node2D targetObject;

		#region Private Variables
		/// <summary>
		///	Original offset w/o any shake effect or any other interaction
		/// </summary>
		private Vector2 originalOffset = Vector2.Zero;

		/// <summary>
		///	Not definition yet.
		/// </summary>
		private Vector2 originalZoom = Vector2.Zero;

		/// <summary>
		/// How strong is the camera actually shaking
		/// </summary>
		private float shakeStrength = 0f;

		/// <summary>
		///	Determines if the camera follow-target is a Player or an Object
		/// </summary>
		private Mode cameraMode = Mode.PLAYER;

		/// <summary>
		///	Determines if the camera is following it's current target
		/// </summary>
		private bool followingTarget = false;

		#region Player Related Variables

		/// <summary>
		///	The current direction the player is facing to
		/// </summary>
		private int playerDirection = 0;

		#endregion

		#endregion

		#endregion

		#region Tweens

		private Tween moveTween;
		private Tween zoomTween;
		private Tween shakeTween;

		#endregion

		enum Mode 
		{
			PLAYER,
			OBJECT
		}

		public override void _Ready()
		{
			RestoreToTarget();
			GameEvents.OnResetGame += RestoreToTarget;
			GameEvents.OnPlayerDeath += OnPlayerDeath;
		}

		public override void _Process(double delta)
		{
			if (followingTarget) {
				GlobalPosition = cameraMode == Mode.PLAYER ? targetPlayer.GlobalPosition : targetObject.GlobalPosition;

				if (cameraMode == Mode.PLAYER) {
					
					if (Mathf.Abs(targetPlayer.Velocity.X) > 0.1) {
						playerDirection = Math.Sign(targetPlayer.Velocity.X);
					}

					originalOffset.X = Mathf.Lerp(originalOffset.X, playerDirection * 5f, 0.07f);
				}
			}


			//Important: When using random related stuff always use this! Otherwise the results will always be the same.
			GD.Randomize();

			Offset = originalOffset +
				new Vector2(
					(float)GD.RandRange(-shakeStrength, shakeStrength),
					(float)GD.RandRange(-shakeStrength, shakeStrength)
				);


			if ((originalZoom.X > 0f) && (originalZoom.Y > 0f)) {
				Zoom = originalZoom;
			}
		}

		private async void RestoreToTarget()
		{
			if (!followingTarget) {
				MoveTo(cameraMode == Mode.PLAYER ? targetPlayer.GlobalPosition : targetObject.GlobalPosition);
				await ToSignal(moveTween, "finished");
				followingTarget = true;
			}
		}

		private void ChangeMode(Mode newMode)
		{
			followingTarget = false;
			cameraMode = newMode;
			RestoreToTarget();
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

		private async void ZoomOn(Vector2 newZoom,
								  double zoomingTime = 0.3,
								  Tween.TransitionType transitionType = Tween.TransitionType.Cubic,
								  Tween.EaseType easingType = Tween.EaseType.Out)
		{
			zoomTween?.Kill();
			zoomTween = CreateTween();

			zoomTween.SetEase(easingType);
			zoomTween.SetTrans(transitionType);

			zoomTween.TweenProperty(this, "zoom", newZoom, zoomingTime);
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

		public Dictionary Serialize()
		{
			return new()
				{
					{ "position", Position }
				};
		}

		public void Deserialize(Dictionary data)
		{
			Position = (Vector2)data["position"];
		}
	}
}