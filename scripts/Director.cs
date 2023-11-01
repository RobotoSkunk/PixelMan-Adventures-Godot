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

using ClockBombGames.PixelMan.Events;
using ClockBombGames.PixelMan.GameObjects;
using System.Collections.Generic;

namespace ClockBombGames.PixelMan
{
	/// <summary>
	/// The Director is the main node of the game.
	/// </summary>
	public partial class Director : Node
	{
		[Export] private int avatarIndex = 0;
		[Export] private SpriteFrames[] avatars;
		[Export] private float shakeStrength;
		[Export] private PackedScene playerScene;

		bool playerDied = false;
		bool secondPlayer = false;
		bool gamePaused = false;
		float restartTimer = 0f;


		/// <summary>
		/// Cooldown between Input usage with UIs
		/// </summary>
		float inputTimer = 0f;

		private List<Player> players = new();
		private Tween shakeTween;


		public SpriteFrames[] Avatars
		{
			get => avatars;
		}


		public override void _Ready()
		{
			this.SetDirector();
			Globals.SceneTree = (Node2D)GetTree().Root.GetChild(
				GetTree().Root.GetChildCount() - 1
			);
			Globals.AvatarIndex = avatarIndex;
		}

		public override void _Process(double delta)
		{
			if (playerDied && !gamePaused) {
				restartTimer -= (float)delta;

				if (restartTimer <= 0f) {
					this.InvokeResetGame();
					playerDied = false;
				}
			}

			if (inputTimer > 0f) {
				inputTimer -= (float)delta;
			}
		}

		public override void _Input(InputEvent @event)
		{
			if (inputTimer > 0f) { 
				return;
			}

			if (@event.IsActionPressed("pause")) {
				TogglePause();
				inputTimer = 0.5f;
			}

			if (@event.IsActionPressed("second_player")) {
				AddSecondPlayer();
			}
		}

		public void TogglePause()
		{
			GetTree().Paused = !GetTree().Paused;
			gamePaused = GetTree().Paused;
		}

		public void TriggerPlayerDeath()
		{
			if (!playerDied) {
				playerDied = true;
				restartTimer = 1f;

				this.InvokePlayerDeath();
			}
		}

		public async void Shake(float strength, float duration)
		{
			shakeTween?.Kill();
			shakeTween = CreateTween();

			shakeTween.TweenProperty(this, "shakeStrength", 0f, duration);
			this.SetShakeStrength(strength);
			shakeTween.Play();

			await ToSignal(shakeTween, "finished");
			shakeTween.Kill();
			this.SetShakeStrength(0f);
		}

		public void AddSecondPlayer()
		{
			Globals.GetPlayersNonAlloc(ref players);

			if (players.Count >= 2) {
				return;
			}

			Player firstPlayer = players[0];
			Player secondPlayer = playerScene.Instantiate<Player>();

			secondPlayer.GlobalPosition = players[0].GlobalPosition;

			secondPlayer.PlayerIndex = 2;
			firstPlayer.PlayerIndex = 1;

			Globals.Viewports.World.AddChild(secondPlayer);
			Globals.Viewports.AddPlayer(secondPlayer);
		}
	}
}
