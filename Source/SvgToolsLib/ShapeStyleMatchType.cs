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
	//*	ShapeStyleMatchType																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of known types of matches found in the MatchPatterns array of
	/// a shape style extension list.
	/// </summary>
	public enum ShapeStyleMatchType
	{
		/// <summary>
		/// No match type specified or unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// All of the values in the property must match.
		/// </summary>
		And,
		/// <summary>
		/// Any of the value in the property can match.
		/// </summary>
		Or
	}
	//*-------------------------------------------------------------------------*

}
