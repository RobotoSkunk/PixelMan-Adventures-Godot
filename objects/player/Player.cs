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


using Godot;


namespace RobotoSkunk.PixelMan.GameObjects
{
	public partial class Player : CharacterBody2D, IGameObject
	{
		[ExportGroup("Components")]
		[Export] private Node2D renderer;
		[Export] private AnimatedSprite2D animator;
		[Export] private GpuParticles2D dustParticles;

		private Vector2 speed = new(144, 320);


		readonly private float maxJumpTriggerTime = 0.16f;
		// readonly private float maxHangCornerTime = 0.1f;

		private float jumpTriggerTime = 0f;
		private float hangCornerTime = 0f;
		private float horizontalInput = 0f;
		private float dustParticlesTimer = 0f;

		private bool pressedJump = false;
		private bool canReduceJump = false;
		private bool invertedGravity = false;


		/// <summary>
		/// Gravity force applied to the player.
		/// </summary>
		private float Gravity
		{
			get
			{
				if (invertedGravity) {
					return -Constants.Gravity;
				} else {
					return Constants.Gravity;
				}
			}
		}

		/// <summary>
		/// Player's jump force.
		/// </summary>
		private float JumpForce
		{
			get
			{
				if (invertedGravity) {
					return speed.Y;
				} else {
					return -speed.Y;
				}
			}
		}


		private bool IsGoingUp
		{
			get
			{
				if (invertedGravity) {
					return Velocity.Y < 0f;
				} else {
					return Velocity.Y > 0f;
				}
			}
		}



		public override void _Input(InputEvent @event)
		{
			if (@event.IsActionPressed("jump")) {
				pressedJump = true;
			}
		}

		public override void _Process(double delta)
		{
			horizontalInput = Input.GetAxis("left", "right");


			if (IsOnFloor()) {
				dustParticlesTimer = 0.08f;

			} else if (dustParticlesTimer > 0f) {
				dustParticlesTimer -= (float)delta;
			}


			if (horizontalInput != 0f) {
				int direction = horizontalInput > 0f ? 1 : -1;

				renderer.Scale = new Vector2(direction, renderer.Scale.Y);

				dustParticles.Emitting = dustParticlesTimer > 0f;
			} else {
				dustParticles.Emitting = false;
			}
		}

		public override void _PhysicsProcess(double delta)
		{
			if (pressedJump) {
				jumpTriggerTime = maxJumpTriggerTime;
				pressedJump = false;

			} else if (jumpTriggerTime > 0f) {
				jumpTriggerTime -= (float)delta;
			}


			#region Process physics

			Vector2 velocity = Velocity;

			// Vertical movement
			velocity.Y += Gravity * (float)delta;

			if (jumpTriggerTime > 0f && IsOnFloor()) {
				velocity.Y = JumpForce;
				jumpTriggerTime = 0f;
				canReduceJump = true;

			} else if (IsGoingUp && !pressedJump && canReduceJump) {
				velocity.Y *= 0.5f;
				canReduceJump = false;
			}

			// Horizontal movement
			velocity.X = speed.X * horizontalInput;

			#endregion


			// Apply changes
			Velocity = velocity;
			MoveAndSlide();
		}
	}
}
