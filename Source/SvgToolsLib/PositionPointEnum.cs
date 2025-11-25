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
	//*	PositionPointEnum																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of known alignment and position points.
	/// </summary>
	[Flags]
	public enum PositionPointEnum
	{
		/// <summary>
		/// No position point specified or unknown.
		/// </summary>
		None		= 0x00,
		/// <summary>
		/// Top edge.
		/// </summary>
		Top			= 0x01,
		/// <summary>
		/// Left edge.
		/// </summary>
		Left		= 0x02,
		/// <summary>
		/// Bottom edge.
		/// </summary>
		Bottom	= 0x04,
		/// <summary>
		/// Right edge.
		/// </summary>
		Right		= 0x08,
		/// <summary>
		/// Center horizontal point.
		/// </summary>
		Center	=	0x10,
		/// <summary>
		/// Middle vertical point.
		/// </summary>
		Middle	= 0x20
	}
	//*-------------------------------------------------------------------------*
}
