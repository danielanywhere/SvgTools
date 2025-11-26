using System;
using System.Collections.Generic;
using System.Text;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	TextFormatTypeEnum																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of known text format types.
	/// </summary>
	public enum TextFormatTypeEnum
	{
		/// <summary>
		/// No format specified or unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// Bold text.
		/// </summary>
		Bold,
		/// <summary>
		/// Text color.
		/// </summary>
		Color,
		/// <summary>
		/// Font name.
		/// </summary>
		FontName,
		/// <summary>
		/// Font size.
		/// </summary>
		FontSize,
		/// <summary>
		/// Italic text.
		/// </summary>
		Italic,
		/// <summary>
		/// Underlined text.
		/// </summary>
		Underline
	}
	//*-------------------------------------------------------------------------*

}
