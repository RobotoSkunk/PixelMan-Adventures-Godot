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
	public partial class CameraAreaTrigger : Area2D
	{
		[Export(
			PropertyHint.Flags,
			"CENTER_POSITION_X," +
			"CENTER_POSITION_Y," +
			"OVERRIDE_ZOOM," +
			"SET_HORIZONTAL_LIMITS," +
			"SET_VERTICAL_LIMITS," +
			"OVERRIDE_OFFSET," +
			"INSTANT_TRANSITION_ON_ENTER," +
			"OVERRIDE_OVERLAPING_AREAS_OPTIONS"
		)]
		private int overrideOptions;
		
		[ExportGroup("Area Options")]
		[Export] private float zoom = 2f;
		[Export] private Vector2 offset;


		public CameraAreaOptions CameraAreaOptions
		{
			get => (CameraAreaOptions)overrideOptions;
		}

		public Vector2 Offset
		{
			get => offset;
		}

		public float Zoom
		{
			get => zoom;
		}
	}

	public enum CameraAreaOptions
	{
		CENTER_POSITION_X                 = 1 << 0,
		CENTER_POSITION_Y                 = 1 << 1,
		OVERRIDE_ZOOM                     = 1 << 2,
		SET_HORIZONTAL_LIMITS             = 1 << 3,
		SET_VERTICAL_LIMITS               = 1 << 4,
		OVERRIDE_OFFSET                   = 1 << 5,
		INSTANT_TRANSITION_ON_ENTER       = 1 << 6,
		OVERRIDE_OVERLAPING_AREAS_OPTIONS = 1 << 7,
	}
}
