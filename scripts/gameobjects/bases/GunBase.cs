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


using System.Collections.Generic;
using System;

using Godot;


namespace ClockBombGames.PixelMan.GameObjects
{
	public partial class GunBase : Node2D
	{
		[Export] protected PackedScene bulletScene;
		[Export] protected RayCast2D rayCast;

		protected readonly List<IGOProjectile> bullets = new();
		protected float angle = 0f;


		/// <summary>
		/// Shoots a projectile with a given speed to the direction where the object is facing.
		/// </summary>
		protected virtual IGOProjectile Shoot(Vector2 from, float speed)
		{
			if (bulletScene == null) {
				throw new NullReferenceException("Gun: bulletScene is null.");
			}


			foreach (IGOProjectile _projectile in bullets) {
				if (!_projectile.IsActive) {
					_projectile.ShootAt(from, GlobalRotation, speed);
					return _projectile;
				}
			}


			var instance = (Node2D)bulletScene.Instantiate();

			if (instance is not IGOProjectile) {
				throw new InvalidCastException("Gun: bulletScene is not an IGOProjectile.");
			}


			var projectile = (IGOProjectile)instance;

			bullets.Add(projectile);
			projectile.ShootAt(from, GlobalRotation, speed);
			instance.ZIndex = ZIndex - 1;

			Globals.World.AddChild(instance);

			return projectile;
		}


		/// <summary>
		/// Gets the nearest reachable player.
		/// </summary>
		protected virtual Player GetNearestPlayer()
		{
			if (rayCast == null) {
				throw new NullReferenceException(Name + ": a raycast is required to get the nearest player.");
			}


			Player nearestPlayer = null;
			float nearestDistance = float.MaxValue;

			foreach (Player _player in Globals.GetPlayers()) {

				// Ignore players that are behind walls.
				rayCast.TargetPosition = ToLocal(_player.GlobalPosition);

				if (rayCast.IsColliding()) {
					continue;
				}

				// Get the nearest player.
				float distance = GlobalPosition.DistanceTo(_player.GlobalPosition);

				if (distance < nearestDistance) {
					nearestPlayer = _player;
					nearestDistance = distance;
				}
			}

			return nearestPlayer;
		}

		/// <summary>
		/// Makes the object look at a given position.
		/// </summary>
		/// <param name="direction">The direction to look at in radians.</param>
		/// <param name="delta">The delta value to use in the interpolation.</param>
		protected virtual void LookAt(float direction, float delta)
		{
			angle += Mathf.Sin(direction - Mathf.DegToRad(angle)) * delta;

			GlobalRotationDegrees = angle;	
		}
	}
}
