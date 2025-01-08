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


using Godot;
using Godot.Collections;


namespace ClockBombGames.PixelMan.GameObjects
{
	public partial class LaserGun : Node2D, IGameObject
	{
		[ExportGroup("Components")]
		[Export] private Sprite2D renderer;
		[Export] private Sprite2D rendererOutline;
		[Export] private RayCast2D rayCast2D;
		[Export] private RayCast2D playerRayCast2D;
		[Export] private AudioStreamPlayer2D audioStreamPlayer;

		[ExportGroup("Laser Components")]
		[Export] private Sprite2D laserBody;
		[Export] private Sprite2D laserDots;
		[Export] private Sprite2D laserLine;
		[Export] private Area2D laserHitbox;

		[ExportGroup("Sprites")]
		[Export] private Array<Texture2D> rendererSprites;
		[Export] private Array<Texture2D> laserSprites;
		[Export] private Array<Texture2D> dotsSprites;
		[Export] private AudioStream audioDetected;
		[Export] private AudioStream audioShoot;

		[ExportGroup("Properties")]
		[Export] private float reloadTime = 1f;


		float reloadProgress = 1f;
		float playerFoundTicks = 0f;
		bool shootLaser = false;
		bool displayOutline = false;

		int dotsSpritesIndex = 0;
		int laserSpritesIndex = 0;
		float animationsDeltaTime = 0f;

		float laserDistance = 1600f;

		public override void _Ready()
		{
			laserHitbox.BodyEntered += (body) =>
			{
				if (body is Player player) {
					Globals.PlayerDied();
				}
			};
		}

		public override void _Process(double delta)
		{
			animationsDeltaTime += (float)delta;
			bool executeFrame = animationsDeltaTime > 0.032f;

			// Laser dots animation
			if (reloadProgress < 1f) {
				laserLine.Visible = true;
				rendererOutline.Visible = false;
				laserLine.Modulate = new Color(1, 1, 1, 0.5f);

				if (executeFrame) {
					laserSpritesIndex++;

					if (laserSpritesIndex == 1) {
						laserSpritesIndex = 0;
					}

					laserBody.Texture = laserSprites[laserSpritesIndex];
				}

			} else if (!shootLaser) {
				if (executeFrame) {
					dotsSpritesIndex++;

					if (dotsSpritesIndex >= dotsSprites.Count) {
						dotsSpritesIndex = 0;
					}

					laserDots.Texture = dotsSprites[dotsSpritesIndex];
				}

				laserDots.Visible = true;

				rendererOutline.Visible = false;
				laserLine.Visible = false;
			} else {
				if (laserDots.Visible) {
					laserDots.Visible = false;

					animationsDeltaTime = 0f;
					displayOutline = false;

					laserLine.Modulate = new Color(1, 1, 1, 1f);
				}

				if (executeFrame) {
					displayOutline = !displayOutline;

					laserLine.Visible = displayOutline;
					rendererOutline.Visible = displayOutline;
				}
			}

			if (executeFrame) {
				animationsDeltaTime = 0f;
			}

			// Laser dots
			Rect2 laserDotsRect = laserDots.RegionRect;
			laserDotsRect.Size = new Vector2(laserDistance, 1f);
			laserDots.RegionRect = laserDotsRect;

			laserLine.Scale = new Vector2(laserDistance, 1f);

			// Laser Gun reload sprites
			float fixedProgress = Mathf.Clamp(reloadProgress, 0, 0.99f);

			renderer.Texture = rendererSprites[(int) (fixedProgress * rendererSprites.Count) % rendererSprites.Count];
		}

		public override void _PhysicsProcess(double delta)
		{
			if (rayCast2D.IsColliding()) {
				laserDistance = rayCast2D.GetCollisionPoint().DistanceTo(rayCast2D.GlobalPosition);
			}

			if (reloadProgress < 1f) {
				reloadProgress += (float)delta / reloadTime;

			} else if (!shootLaser) {
				playerRayCast2D.TargetPosition = new Vector2(laserDistance, 0f);

				if (playerRayCast2D.IsColliding()) {
					playerFoundTicks = 0.4f;
					shootLaser = true;

					audioStreamPlayer.Stream = audioDetected;
					audioStreamPlayer.Play();
				}
			} else {
				playerFoundTicks -= (float)delta;

				if (playerFoundTicks <= 0f) {
					shootLaser = false;
					reloadProgress = 0f;

					Globals.Shake(1f, 0.2f);

					laserBody.Scale = new Vector2(laserDistance, 1.5f);

					audioStreamPlayer.Stream = audioShoot;
					audioStreamPlayer.Play();
				}
			}


			if (laserBody.Scale.Y > 0.05f) {
				float newY = Mathf.Lerp(laserBody.Scale.Y, 0f, 0.15f);

				laserBody.Visible = true;
				laserHitbox.Monitorable = true;

				laserBody.Scale = new Vector2(laserDistance, newY);
			} else {
				if (laserHitbox.Monitorable) {
					laserHitbox.Monitorable = false;
				}

				laserBody.Visible = false;
			}
		}


		public Dictionary Serialize()
		{
			throw new System.NotImplementedException();
		}

		public void Deserialize(Dictionary data)
		{
			throw new System.NotImplementedException();
		}
	}
}
