/*
 * Copyright (c). 2026 Daniel Patterson, MCSD (danielanywhere).
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
	//*	LineOffsetType																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of the types of offset that can be applied to a line.
	/// </summary>
	public enum LineOffsetType
	{
		/// <summary>
		/// No offset specified or unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// Offset to the left of the line's direction of travel.
		/// </summary>
		Left,
		/// <summary>
		/// Offset to the right of the line's direction of travel.
		/// </summary>
		Right
	}
	//*-------------------------------------------------------------------------*

}
