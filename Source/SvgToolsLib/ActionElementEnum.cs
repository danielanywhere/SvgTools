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
	//*	ActionElementEnum																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of recognized action elements.
	/// </summary>
	[Flags]
	public enum ActionElementEnum
	{
		/// <summary>
		/// No element defined or unknown.
		/// </summary>
		None = 0x00000000,
		/// <summary>
		/// Action element.
		/// </summary>
		Action = 0x00000001,
		/// <summary>
		/// Base element.
		/// </summary>
		Base = 0x00000002,
		/// <summary>
		/// Count element.
		/// </summary>
		Count = 0x00000004,
		/// <summary>
		/// Date and time value.
		/// </summary>
		DateTimeValue = 0x00000008,
		/// <summary>
		/// Count of digits.
		/// </summary>
		Digits = 0x00000010,
		/// <summary>
		/// Input filename.
		/// </summary>
		InputFilename = 0x00000020,
		/// <summary>
		/// Input folder name.
		/// </summary>
		InputFolderName = 0x00000040,
		/// <summary>
		/// List of input file or folder names, depending on context.
		/// </summary>
		Inputs = 0x00000080,
		/// <summary>
		/// Output filename.
		/// </summary>
		OutputFilename = 0x00000100,
		/// <summary>
		/// Output folder name.
		/// </summary>
		OutputFoldername = 0x00000200,
		/// <summary>
		/// Output file or folder name, depending on context.
		/// </summary>
		OutputName = 0x00000400,
		/// <summary>
		/// Regular expression pattern.
		/// </summary>
		Pattern = 0x00000800,
		/// <summary>
		/// Prefix flag.
		/// </summary>
		Prefix = 0x00001000,
		/// <summary>
		/// Range, start through end.
		/// </summary>
		Range = 0x00002000,
		/// <summary>
		/// Source data folder name.
		/// </summary>
		SourceFolderName = 0x00004000,
		/// <summary>
		/// Suffix flag.
		/// </summary>
		Suffix = 0x00008000,
		/// <summary>
		/// Text-only pattern.
		/// </summary>
		Text = 0x00010000,
		/// <summary>
		/// Working path.
		/// </summary>
		WorkingPath = 0x00020000,
		/// <summary>
		/// Prefix option.
		/// </summary>
		OptionPrefix = 0x00040000,
		/// <summary>
		/// Suffix option.
		/// </summary>
		OptionSuffix = 0x00080000
	}
	//*-------------------------------------------------------------------------*

}
