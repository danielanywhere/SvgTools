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
	//*	SvgPathActionType																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// SVG path action type.
	/// </summary>
	public enum SvgPathActionType
	{
		/// <summary>
		/// No action specified or unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// Close the path.
		/// </summary>
		ClosePath,
		/// <summary>
		/// Cubic Bezier curve.
		/// </summary>
		CubicBezier,
		/// <summary>
		/// Elliptical arc.
		/// </summary>
		EllipticalArc,
		/// <summary>
		/// Horizontal line.
		/// </summary>
		HorizontalLine,
		/// <summary>
		/// Line.
		/// </summary>
		Line,
		/// <summary>
		/// Move cursor.
		/// </summary>
		Move,
		/// <summary>
		/// Quadratic Bezier curve.
		/// </summary>
		QuadraticBezier,
		/// <summary>
		/// Shorthand Cubic Bezier curve.
		/// </summary>
		ShorthandCubicCurve,
		/// <summary>
		/// Shorthand Quadratic Bezier curve.
		/// </summary>
		ShorthandQuadraticCurve,
		/// <summary>
		/// Vertical line.
		/// </summary>
		VerticalLine
	}
	//*-------------------------------------------------------------------------*

}
