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
	//*	TransformTypeEnum																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of recognized transform types.
	/// </summary>
	public enum TransformTypeEnum
	{
		/// <summary>
		/// No transform specified or unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// The first two rows of a 3x3 matrix, where the last row is implied as
		/// 0 0 1.
		/// </summary>
		Matrix,
		/// <summary>
		/// Rotation by a degrees, optionally anchored at an x, y location.
		/// </summary>
		Rotate,
		/// <summary>
		/// Scaling of the shape's size.
		/// </summary>
		Scale,
		/// <summary>
		/// Rotate the object by the specified angle, leaving the corners
		/// at their original Y coordinates.
		/// </summary>
		SkewX,
		/// <summary>
		/// Rotate the object by the specified angle, leaving the corners
		/// at their original X coordinates.
		/// </summary>
		SkewY,
		/// <summary>
		/// Translation as a relative offset of X and Y.
		/// </summary>
		Translate
	}
	//*-------------------------------------------------------------------------*

}
