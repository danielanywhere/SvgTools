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
		Information,
		/// <summary>
		/// The message is a warning.
		/// </summary>
		Warning,
		/// <summary>
		/// The message is an error.
		/// </summary>
		Error
	}
	//*-------------------------------------------------------------------------*

}
