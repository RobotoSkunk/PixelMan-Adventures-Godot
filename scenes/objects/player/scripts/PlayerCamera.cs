/*
	PixelMan Adventures - An open-source 2D platformer game.
	Copyright (C) 2023 Edgar Lima (RobotoSkunk) <contact@robotoskunk.com>
	Copyright (C) 2023 Repertix

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


namespace ClockBombGames.PixelMan.GameObjects 
{
	public partial class PlayerCamera : Camera2D
	{
		#region Variables
		/// <summary>
		///	Original offset without any shake effect or any other interaction
		/// </summary>
		private Vector2 rawOffset = Vector2.Zero;

		/// <summary>
		///	Virtualized offset that is actually applied to the camera
		/// </summary>
		private Vector2 virtualOffset = Vector2.Zero;

		/// <summary>
		///	Original TargetPlayer position without any alteration
		/// </summary>
		private Vector2 rawTargetPosition = Vector2.Zero;

		/// <summary>
		///	Original zoom without any alteration
		/// </summary>
		private float rawZoom = 1f;

		/// <summary>
		///	How much of the screen is considered safe for the player to move without the camera following it in pixels
		/// </summary>
		private readonly float verticalDeadzone = 16f;


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

		/// <summary>
		///	The current TargetPlayer of the camera
		/// </summary>
		public Player TargetPlayer { get; set; }


		private CameraAreaOptions cameraOverrideOptions = 0;
		bool enteredCameraArea = false;

		#endregion



		public override void _Ready()
		{
			ResetToTarget();
			GameEvents.OnResetGame += ResetToTarget;
			GameEvents.OnPlayerDeath += OnPlayerDeath;

			MakeCurrent();
		}

		public override void _Process(double delta)
		{
			if (TargetPlayer != null) {
				// Calculate zoom
				if ((cameraOverrideOptions & CameraAreaOptions.OVERRIDE_ZOOM) == 0) {
					rawZoom = 1 + 4f * playerVelocity / Constants.maxSpeed;

					if (TargetPlayer.PlayerIndex != 0) {
						rawZoom++;
					}
				}

				// Calculate offset
				if ((cameraOverrideOptions & CameraAreaOptions.OVERRIDE_OFFSET) == 0) {
					rawOffset.X = playerDirection * 48f;
				} else {
					rawOffset = Vector2.Zero;
				}

				// Calculate position
				if ((cameraOverrideOptions & CameraAreaOptions.CENTER_POSITION_Y) == 0) {
					if (TargetPlayer.GlobalPosition.Y < GlobalPosition.Y - verticalDeadzone) {
						rawTargetPosition.Y = TargetPlayer.GlobalPosition.Y + verticalDeadzone;

					} else if (TargetPlayer.GlobalPosition.Y > GlobalPosition.Y + verticalDeadzone) {
						rawTargetPosition.Y = TargetPlayer.GlobalPosition.Y - verticalDeadzone;
					}
				}

				if ((cameraOverrideOptions & CameraAreaOptions.CENTER_POSITION_X) == 0) {
					rawTargetPosition.X = TargetPlayer.GlobalPosition.X;
				}
			} else {
				playerDirection = 0;
				playerVelocity = 0f;
				rawOffset = Vector2.Zero;
				rawZoom = 1f;
			}


			// Apply zoom
			float zoomDelta = 0.01f;

			if ((cameraOverrideOptions & CameraAreaOptions.OVERRIDE_ZOOM) != 0) {
				zoomDelta = 0.1f;
			}

			Zoom = RSMath.Lerp(Zoom, Vector2.One * (5f - rawZoom), zoomDelta * RSMath.FixedDelta(delta));


			// Apply offset
			Vector2 shake = Vector2.Zero;

			if (Globals.ShakeStrength > 0f) {
				shake = 2f * RSRandom.Circle2D() * Globals.ShakeStrength * rawZoom;
			}

			virtualOffset += (rawOffset - Offset) / 25f * RSMath.FixedDelta(delta);

			Offset = virtualOffset + shake;


			// Apply position
			GlobalPosition += (rawTargetPosition - GlobalPosition) / 8f * RSMath.FixedDelta(delta);
		}

		public override void _PhysicsProcess(double delta)
		{
			if (TargetPlayer != null) {
				cameraOverrideOptions = 0;

				foreach (CameraAreaTrigger areaTrigger in TargetPlayer.CameraAreaTriggers) {
					cameraOverrideOptions |= areaTrigger.CameraAreaOptions;

					if ((cameraOverrideOptions & CameraAreaOptions.CENTER_POSITION_X) != 0) {
						rawTargetPosition = new Vector2(areaTrigger.GlobalPosition.X, rawTargetPosition.Y);
					}

					if ((cameraOverrideOptions & CameraAreaOptions.CENTER_POSITION_Y) != 0) {
						rawTargetPosition = new Vector2(rawTargetPosition.X, areaTrigger.GlobalPosition.Y);
					}

					if ((cameraOverrideOptions & CameraAreaOptions.OVERRIDE_ZOOM) != 0) {
						rawZoom = areaTrigger.Zoom;
					}

					if ((cameraOverrideOptions & CameraAreaOptions.OVERRIDE_OFFSET) != 0) {
						rawOffset = areaTrigger.Offset;
					}
				}



				if (TargetPlayer.CameraAreaTriggers.Count == 0) {
					enteredCameraArea = false;

				} else if (!enteredCameraArea) {
					enteredCameraArea = true;

					if ((cameraOverrideOptions & CameraAreaOptions.INSTANT_TRANSITION_ON_ENTER) != 0) {
						GlobalPosition = rawTargetPosition;
						virtualOffset = rawOffset;
						Zoom = Vector2.One * (5f - rawZoom);
					}
				}

				playerVelocity = TargetPlayer.Velocity.Length();

				if (Mathf.Abs(TargetPlayer.WantedHorizontalSpeed) != 0f) {
					playerDirection = Mathf.Sign(TargetPlayer.WantedHorizontalSpeed);
				}
			} else {
				playerVelocity = 0f;
				playerDirection = 0;
			}
		}

		public void SetTextureRect(TextureRect rect)
		{
			rect.Texture = GetViewport().GetTexture();
		}

		private void ResetToTarget()
		{
			rawOffset = Vector2.Zero;

			if (TargetPlayer != null) {
				rawTargetPosition = TargetPlayer.GlobalPosition;
				GlobalPosition = rawTargetPosition;
			}

			playerDirection = 0;
			playerVelocity = 0f;
			rawOffset = Vector2.Zero;
			rawZoom = 1f;
		}

		private void OnPlayerDeath()
		{
			Globals.Shake(2f, 0.3f);
		}
	}
}
