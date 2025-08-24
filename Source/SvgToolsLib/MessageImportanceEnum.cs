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
	//*	MessageImportanceEnum																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of known importance levels for messages.
	/// </summary>
	public enum MessageImportanceEnum
	{
		/// <summary>
		/// The message importance was none or unspecified.
		/// </summary>
		None = 0,
		/// <summary>
		/// The message is informational.
		/// </summary>
		Info,
		/// <summary>
		/// The message is a warning.
		/// </summary>
		Warn,
		/// <summary>
		/// The message is an error.
		/// </summary>
		Err
	}
	//*-------------------------------------------------------------------------*

}
