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
	public partial class Trampoline : Area2D
	{
		#region Variables

		[ExportGroup("Components")]
		[Export] private AnimatedSprite2D animator;
		[Export] private AudioStreamPlayer2D audioPlayer;

		#endregion


		public override void _Ready()
		{
			BodyEntered += OnBodyEnter;
			animator.Play("idle");
		}

		private void OnBodyEnter(Node2D body)
		{
			if (body is Player player) {
				player.Impulse();

				animator.Play("bounce");
				audioPlayer.Play();
			}
		}
	}
}
