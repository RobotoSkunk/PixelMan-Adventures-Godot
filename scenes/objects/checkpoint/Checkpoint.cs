/*
	PixelMan Adventures - An open-source 2D platformer game.
	Copyright (C) 2025 Edgar Lima (RobotoSkunk) <contact@robotoskunk.com>

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
	public partial class Checkpoint : Node2D
	{
		[ExportGroup("Components")]
		[Export] private Sprite2D signSprite;
		[Export] private Sprite2D numberSprite;
		[Export] private Area2D hitbox;

		[ExportGroup("Properties")]
		[Export] private int availableAttempts;
		[Export(PropertyHint.ArrayType)] private Texture2D[] numbersTextures;


		Player currentPlayer;
		float resurrectTime;
		bool resurrectPlayer;


		public override void _Ready()
		{
			hitbox.BodyEntered += (body) =>
			{
				if (body is Player player && currentPlayer == null) {
					player.CheckpointAttempts = availableAttempts;
					currentPlayer = player;

					SetNumber();
				}
			};

			GameEvents.OnResetGame += OnResetGame;
			GameEvents.OnPlayerDeath += OnPlayerDeath;

			numberSprite.Visible = false;

			signSprite.RotationDegrees = 360f;
		}

		public override void _PhysicsProcess(double delta)
		{
			if (currentPlayer != null) {
				if (resurrectTime > 0f) {
					resurrectTime -= (float)delta;
					resurrectPlayer = true;

				} else if (resurrectPlayer) {
					currentPlayer.CheckpointAttempts--;
					currentPlayer.Resurrect(GlobalPosition + Vector2.Up * 32f);

					SetNumber();

					resurrectPlayer = false;
				}
			}


			if (signSprite.RotationDegrees < 359f) {
				signSprite.RotationDegrees = Mathf.Lerp(signSprite.RotationDegrees, 360f, 0.15f);

			} else {
				signSprite.RotationDegrees = 360f;
			}
		}

		private void SetNumber()
		{
			if (currentPlayer.CheckpointAttempts == 0) {
				signSprite.Visible = false;
				return;
			}

			if (currentPlayer.CheckpointAttempts < numbersTextures.Length) {
				numberSprite.Texture = numbersTextures[currentPlayer.CheckpointAttempts];
			}

			numberSprite.Visible = true;

			signSprite.RotationDegrees = 0f;
		}

		private void OnPlayerDeath(Player player)
		{
			if (player != currentPlayer) {
				return;
			}

			if (currentPlayer.CheckpointAttempts > 0) {
				resurrectTime = 1f;
			}
		}

		private void OnResetGame()
		{
			currentPlayer = null;

			resurrectTime = 0f;

			numberSprite.Visible = false;
			signSprite.Visible = true;
		}
	}
}
