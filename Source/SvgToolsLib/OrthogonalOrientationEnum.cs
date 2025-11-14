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
	//*	OrthogonalOrientationEnum																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of available orthogonal orientations.
	/// </summary>
	[Flags]
	public enum OrthogonalOrientationEnum
	{
		/// <summary>
		/// No orientation defined or unknown.
		/// </summary>
		None =				0x00,
		/// <summary>
		/// Horizontal orientation.
		/// </summary>
		Horizontal =	0x01,
		/// <summary>
		/// Vertical orientation.
		/// </summary>
		Vertical =		0x02,
		/// <summary>
		/// Orientation consists of horizontal and vertical elements.
		/// </summary>
		Grid =				0x03
	}
	//*-------------------------------------------------------------------------*

}
