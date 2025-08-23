using System;
using System.Collections.Generic;
using System.Text;

namespace SvgToolsLibrary
{
	//*-------------------------------------------------------------------------*
	//*	ShapeInfoTypeEnum																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of recognized shape information types.
	/// </summary>
	public enum ShapeInfoTypeEnum
	{
		/// <summary>
		/// No shape type defined or unknown. Used by the elements a, clippath,
		/// defs, g, and switch.
		/// </summary>
		None = 0,
		/// <summary>
		/// Circle shape type. Values used are cx, cy, and r.
		/// </summary>
		Circle,
		/// <summary>
		/// Ellipse shape type. Values used are cx, cy, rx, and ry.
		/// </summary>
		Ellipse,
		/// <summary>
		/// Line shape type. Values used are x1, y1, x2, and y2.
		/// </summary>
		Line,
		/// <summary>
		/// Location shape type. Values used are x and y.
		/// </summary>
		Location,
		/// <summary>
		/// Path shape type. The 'd' value is used.
		/// </summary>
		Path,
		/// <summary>
		/// Poly shape type. The points value is used.
		/// </summary>
		Poly,
		/// <summary>
		/// Radial shape with the values cx, cy, r, fx, and fy.
		/// </summary>
		Radial,
		/// <summary>
		/// Rectangular shape type. Values used are x, y, width, and height.
		/// Used with foreignObject, image, rect, and use.
		/// </summary>
		Rect
	}
	//*-------------------------------------------------------------------------*

}
