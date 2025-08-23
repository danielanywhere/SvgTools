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
