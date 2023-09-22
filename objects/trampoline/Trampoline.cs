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
using Godot.Collections;


namespace RobotoSkunk.PixelMan.GameObjects
{
	public partial class Trampoline : Area2D, IGameObject
	{
		#region Variables

		[ExportGroup("Components")]
		[Export] private AnimatedSprite2D animator;
		[Export] private AudioStreamPlayer2D audioPlayer;

		[ExportGroup("Object Properties")]
		[Export] private float pushForce = 500f;

		#endregion


		public override void _Ready()
		{
			BodyEntered += OnBodyEnter;
			BodyExited += OnBodyExit;

			animator.Play("idle");
		}

		private void OnBodyEnter(Node2D body)
		{
			if (body is Player player) {
				player.Impulse(Mathf.DegToRad(RotationDegrees - 90f));
				player.isInTrampoline = false;

				animator.Play("bounce");
				audioPlayer.Play();
			}
		}

		private void OnBodyExit(Node2D body)
		{
			if (body is Player player) {
				player.isInTrampoline = false;
			}
		}



		public Dictionary Serialize()
		{
			return new()
			{
				{ "position", Position },
				{ "rotation", Rotation },
				{ "scale", Scale.X }
			};
		}

		public void Deserialize(Dictionary data)
		{
			Position = (Vector2)data["position"];
			Rotation = (float)data["rotation"];
			Scale = new Vector2((float)data["scale"], (float)data["scale"]);
		}
	}
}
