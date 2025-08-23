using System;
using System.Collections.Generic;
using System.Text;

namespace SvgToolsLibrary
{
	//*-------------------------------------------------------------------------*
	//*	TransformTypeEnum																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of recognized transform types.
	/// </summary>
	public enum TransformTypeEnum
	{
		/// <summary>
		/// No transform specified or unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// The first two rows of a 3x3 matrix, where the last row is implied as
		/// 0 0 1.
		/// </summary>
		Matrix,
		/// <summary>
		/// Rotation by a degrees, optionally anchored at an x, y location.
		/// </summary>
		Rotate,
		/// <summary>
		/// Scaling of the shape's size.
		/// </summary>
		Scale,
		/// <summary>
		/// Rotate the object by the specified angle, leaving the corners
		/// at their original Y coordinates.
		/// </summary>
		SkewX,
		/// <summary>
		/// Rotate the object by the specified angle, leaving the corners
		/// at their original X coordinates.
		/// </summary>
		SkewY,
		/// <summary>
		/// Translation as a relative offset of X and Y.
		/// </summary>
		Translate
	}
	//*-------------------------------------------------------------------------*

}
