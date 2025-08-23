using System;
using System.Collections.Generic;
using System.Text;

namespace SvgToolsLibrary
{
	//*-------------------------------------------------------------------------*
	//*	RenderFileTypeEnum																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of available output file types.
	/// </summary>
	public enum RenderFileTypeEnum
	{
		/// <summary>
		/// No render file output type specified or unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// Automatically decided at runtime. This is the default value.
		/// </summary>
		Auto,
		/// <summary>
		/// The contents of the rectangle info collection will be output.
		/// </summary>
		RectangleInfoList
	}
	//*-------------------------------------------------------------------------*
}
