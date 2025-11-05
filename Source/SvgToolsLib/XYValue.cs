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
using System.Text.RegularExpressions;

using static SvgToolsLib.SvgToolsUtil;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	XYValueCollection																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of XYValueItem Items.
	/// </summary>
	public class XYValueCollection : List<XYValueItem>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************

		//*-----------------------------------------------------------------------*
		//* Parse																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Parse the elements in the raw SVG points string into individual
		/// coordinates.
		/// </summary>
		/// <param name="points">
		/// Raw SVG point infomation, as used in points and polygons elements.
		/// </param>
		/// <returns>
		/// Collection of coordinates representing the path being drawn.
		/// </returns>
		public static XYValueCollection Parse(string points)
		{
			XYValueItem item = null;
			MatchCollection matches = null;
			int paramIndex = 0;
			XYValueCollection result = new XYValueCollection();
			string text = "";

			if(points?.Length > 0)
			{
				matches = Regex.Matches(points, ResourceMain.rxFindSvgTransformParams);
				foreach(Match matchItem in matches)
				{
					text = GetValue(matchItem, "param");
					if(IsNumeric(text))
					{
						//	This is a number.
						if(paramIndex % 2 == 0)
						{
							item = new XYValueItem()
							{
								X = ToFloat(text)
							};
							result.Add(item);
						}
						else
						{
							item.Y = ToFloat(text);
						}
						paramIndex++;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	XYValueItem																															*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual X and Y value.
	/// </summary>
	public class XYValueItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* Add																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add the values of the caller's item to the member properties of this
		/// item.
		/// </summary>
		/// <param name="item">
		/// Reference to the item containing the values to add.
		/// </param>
		public void Add(XYValueItem item)
		{
			if(item != null)
			{
				this.mX += item.mX;
				this.mY += item.mY;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Copy																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a fresh copy of this XY value.
		/// </summary>
		/// <returns>
		/// Newly created XY value object with member-level copies of the values.
		/// </returns>
		public XYValueItem Copy()
		{
			XYValueItem result = new XYValueItem()
			{
				mX = this.mX,
				mY = this.mY
			};
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Plus																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add the values of this item within another and return a new item with
		/// the resulting values.
		/// </summary>
		/// <param name="additive">
		/// The item to be added to this one.
		/// </param>
		/// <returns>
		/// Reference to a new XYValueItem containing the sum of this item and
		/// another.
		/// </returns>
		public XYValueItem Plus(XYValueItem additive)
		{
			XYValueItem result = new XYValueItem()
			{
				X = mX,
				Y = mY
			};
			if(additive != null)
			{
				result.mX += additive.mX;
				result.mY += additive.mY;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Update																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Update this item with the values of the presented item.
		/// </summary>
		/// <param name="item">
		/// Reference to the item containing the values to adopt.
		/// </param>
		public void Update(XYValueItem item)
		{
			if(item != null)
			{
				this.mX = item.mX;
				this.mY = item.mY;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	X																																			*
		//*-----------------------------------------------------------------------*
		private float mX = 0f;
		/// <summary>
		/// Get/Set the X value.
		/// </summary>
		public float X
		{
			get { return mX; }
			set { mX = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Y																																			*
		//*-----------------------------------------------------------------------*
		private float mY = 0f;
		/// <summary>
		/// Get/Set the Y value.
		/// </summary>
		public float Y
		{
			get { return mY; }
			set { mY = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Zero																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Zero the member values of this item.
		/// </summary>
		public void Zero()
		{
			this.mX = 0f;
			this.mY = 0f;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}

