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

namespace SvgToolsLibrary
{
	//*-------------------------------------------------------------------------*
	//*	ShapeInfoTypeEnum																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of recognized shape information types.
	/// </summary>
	public enum ShapeInfoTypeEnum
	{
		/// <summary>
		/// No shape type defined or unknown. Used by the elements a, clippath,
		/// defs, g, and switch.
		/// </summary>
		None = 0,
		/// <summary>
		/// Circle shape type. Values used are cx, cy, and r.
		/// </summary>
		Circle,
		/// <summary>
		/// Ellipse shape type. Values used are cx, cy, rx, and ry.
		/// </summary>
		Ellipse,
		/// <summary>
		/// Line shape type. Values used are x1, y1, x2, and y2.
		/// </summary>
		Line,
		/// <summary>
		/// Location shape type. Values used are x and y.
		/// </summary>
		Location,
		/// <summary>
		/// Path shape type. The 'd' value is used.
		/// </summary>
		Path,
		/// <summary>
		/// Poly shape type. The points value is used.
		/// </summary>
		Poly,
		/// <summary>
		/// Radial shape with the values cx, cy, r, fx, and fy.
		/// </summary>
		Radial,
		/// <summary>
		/// Rectangular shape type. Values used are x, y, width, and height.
		/// Used with foreignObject, image, rect, and use.
		/// </summary>
		Rect
	}
	//*-------------------------------------------------------------------------*

}
