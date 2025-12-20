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
	//*	CssAnimationFillModeEnum																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of the known CSS animation fill modes.
	/// </summary>
	public enum CssAnimationFillModeEnum
	{
		/// <summary>
		/// No fill mode specified or unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// The object remains in the state of the last frame of the animation.
		/// </summary>
		Forwards,
		/// <summary>
		/// The object remains in the state of the first frame of the animation.
		/// </summary>
		Backwards,
		/// <summary>
		/// The object remains in the state that it was first and last affected
		/// by the animation.
		/// </summary>
		Both
	}
	//*-------------------------------------------------------------------------*

}
