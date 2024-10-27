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
		TextureRect textureRect;

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
		///	Original Target position without any alteration
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
		///	The current Target of the camera
		/// </summary>
		public Player Target { get; set; }

		#endregion



		public override void _Ready()
		{
			RestoreToTarget();
			GameEvents.OnResetGame += RestoreToTarget;
			GameEvents.OnPlayerDeath += OnPlayerDeath;

			MakeCurrent();
		}

		public override void _Process(double delta)
		{
			if (Target != null) {
				// Calculate zoom
				rawZoom = 1 + 4f * playerVelocity / Constants.maxSpeed;

				// Calculate offset
				rawOffset.X = playerDirection * 48f;

				// Calculate position
				if (Target.GlobalPosition.Y < GlobalPosition.Y - verticalDeadzone) {
					rawTargetPosition.Y = Target.GlobalPosition.Y + verticalDeadzone;

				} else if (Target.GlobalPosition.Y > GlobalPosition.Y + verticalDeadzone) {
					rawTargetPosition.Y = Target.GlobalPosition.Y - verticalDeadzone;
				}

				rawTargetPosition.X = Target.GlobalPosition.X;

				// float distanceToPlayerY = Mathf.Abs(Target.GlobalPosition.Y - GlobalPosition.Y);

				// if (distanceToPlayerY > viewportSize.Y * verticalSafeZone) {
				// 	rawTargetPosition.Y = Target.GlobalPosition.Y - viewportSize.Y * verticalSafeZone;
				// }

				// rawTargetPosition.X = Target.GlobalPosition.X;


				if (Target.PlayerIndex != 0) {
					rawZoom++;
				}
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
			if (Target != null) {
				playerVelocity = Target.Velocity.Length();

				if (Mathf.Abs(Target.WantedHorizontalSpeed) != 0f) {
					playerDirection = Mathf.Sign(Target.WantedHorizontalSpeed);
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

		private void RestoreToTarget()
		{
			rawOffset = Vector2.Zero;

			if (Target != null) {
				rawTargetPosition = Target.GlobalPosition;
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
