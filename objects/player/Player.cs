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


using System;
using Godot;


namespace RobotoSkunk.PixelMan.GameObjects
{
	public partial class Player : CharacterBody2D, IGameObject
	{
		[ExportGroup("Components")]
		[Export] private Node2D renderer;
		[Export] private AnimatedSprite2D animator;

		[ExportGroup("Properties")]
		[Export] private Vector2 speed = new(0, 0);
		[Export] private float gravityForce = 140f;


		readonly private float maxJumpTriggerTime = 0.1f;
		readonly private float maxHangCornerTime = 0.1f;

		private float jumpTriggerTime = 0f;
		private float hangCornerTime = 0f;

		private float horizontalInput = 0f;
		private bool pressedJump = false;
		private bool invertedGravity = false;

		private Vector2 velocity = new(0, 0);



		/// <summary>
		/// Gravity force applied to the player.
		/// </summary>
		private float Gravity
		{
			get
			{
				if (invertedGravity) {
					return -gravityForce;
				} else {
					return gravityForce;
				}
			}
		}



		public override void _Process(double delta)
		{
			horizontalInput = Input.GetAxis("left", "right");
			pressedJump = Input.IsActionPressed("jump");
		}

		public override void _PhysicsProcess(double delta)
		{
			if (pressedJump) {
				jumpTriggerTime = maxJumpTriggerTime;
			} else {
				jumpTriggerTime -= (float)delta;
			}

			// Process physics
			velocity.Y += Gravity * (float)delta;

			if (jumpTriggerTime > 0f) {
				velocity.Y = speed.Y;
			}

			velocity.X = speed.X * horizontalInput;


			// Apply changes
			Velocity = velocity;
			MoveAndSlide();
		}
	}
}
