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
	//*	ShapeStyleExtensionType																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of known shape style extension types.
	/// </summary>
	public enum ShapeStyleExtensionType
	{
		/// <summary>
		/// No extension type defined or unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// Property name/value pairs to be directly placed upon the target node,
		/// but only when that property has not yet been specified.
		/// </summary>
		DefaultProperties,
		/// <summary>
		/// Items panel customization.
		/// </summary>
		ItemsPanel,
		/// <summary>
		/// Child nodes will be added to the matching node.
		/// </summary>
		Nodes,
		/// <summary>
		/// Property name/value pairs to be placed directly upon the target
		/// node.
		/// </summary>
		Properties,
		/// <summary>
		/// Setter child entries will be added to the target nodes collection.
		/// </summary>
		Setters,
		/// <summary>
		/// Style extension type.
		/// </summary>
		Style,
		/// <summary>
		/// Template extension type.
		/// </summary>
		Template
	}
	//*-------------------------------------------------------------------------*

}
