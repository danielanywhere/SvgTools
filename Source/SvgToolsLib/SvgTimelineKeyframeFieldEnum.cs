/*
 * Copyright (c). 2025 Daniel Patterson, MCSD (danielanywhere).
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	SvgTimelineKeyframeFieldEnum																						*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of known fields in an SVG timeline keyframe.
	/// </summary>
	[Flags]
	public enum SvgTimelineKeyframeFieldEnum
	{
		/// <summary>
		/// No field type specified or unknown.
		/// </summary>
		None =					0x00000000,
		/// <summary>
		/// The absolute positioning field.
		/// </summary>
		Absolute =			0x00000001,
		/// <summary>
		/// The ground color field.
		/// </summary>
		BackColor =			0x00000002,
		/// <summary>
		/// The easing pattern field.
		/// </summary>
		Easing =				0x00000004,
		/// <summary>
		/// The foreground color field.
		/// </summary>
		ForeColor =			0x00000008,
		/// <summary>
		/// The group name field.
		/// </summary>
		GroupName =			0x00000010,
		/// <summary>
		/// The height field.
		/// </summary>
		Height =				0x00000020,
		/// <summary>
		/// The index field.
		/// </summary>
		Index =					0x00000040,
		/// <summary>
		/// The object name field.
		/// </summary>
		ObjectName =		0x00000080,
		/// <summary>
		/// The opacity field.
		/// </summary>
		Opacity =				0x00000100,
		/// <summary>
		/// The brief remark field.
		/// </summary>
		Remark =				0x00000200,
		/// <summary>
		/// The repeat strategy field.
		/// </summary>
		Repeat =				0x00000400,
		/// <summary>
		/// The x-scale origin type field.
		/// </summary>
		ScaleXOrigin =	0x00000800,
		/// <summary>
		/// The y-scale origin type field.
		/// </summary>
		ScaleYOrigin =	0x00001000,
		/// <summary>
		/// The current time field.
		/// </summary>
		Seconds =				0x00002000,
		/// <summary>
		/// The width field.
		/// </summary>
		Width =					0x00004000,
		/// <summary>
		/// The X position field.
		/// </summary>
		X =							0x00008000,
		/// <summary>
		/// The Y position field.
		/// </summary>
		Y =							0x00010000
	}
	//*-------------------------------------------------------------------------*

}
