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


namespace ClockBombGames.PixelMan.GameObjects
{
	public partial class Block : StaticBody2D
	{
		[Export] private float _friction;
		[Export] private float _acceleration;
		[Export] private bool _overrideFrictionOnAir;

		public float Friction
		{
			get => _friction;
		}

		public float Acceleration
		{
			get => _acceleration;
		}

		public bool OverrideFrictionOnAir
		{
			get => _overrideFrictionOnAir;
		}
	}
}
