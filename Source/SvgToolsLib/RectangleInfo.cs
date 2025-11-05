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

using Newtonsoft.Json;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	RectangleInfoCollection																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of RectangleInfoItem Items.
	/// </summary>
	public class RectangleInfoCollection : List<RectangleInfoItem>
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


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	RectangleInfoItem																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about a rectangle.
	/// </summary>
	public class RectangleInfoItem
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
		//* Grow																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Grow the specified rectangle in all directions by the total number of
		/// specified pixels.
		/// </summary>
		/// <param name="rectangle">
		/// Reference to the rectangle to grow.
		/// </param>
		/// <param name="pixels">
		/// Number of total pixels to grow by.
		/// </param>
		public static void Grow(RectangleInfoItem rectangle, float pixels)
		{
			float ax = 0f;

			if(rectangle != null && Math.Abs(pixels) > float.Epsilon)
			{
				ax = pixels / 2f;
				rectangle.Left -= ax;
				rectangle.Top -= ax;
				rectangle.Width += pixels;
				rectangle.Height += pixels;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Height																																*
		//*-----------------------------------------------------------------------*
		private float mHeight = 0f;
		/// <summary>
		/// Get/Set the height of the rectangle.
		/// </summary>
		[JsonProperty(Order = 4)]
		public float Height
		{
			get { return mHeight; }
			set { mHeight = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Left																																	*
		//*-----------------------------------------------------------------------*
		private float mLeft = 0f;
		/// <summary>
		/// Get/Set the Left coordinate of the rectangle.
		/// </summary>
		[JsonProperty(Order = 1)]
		public float Left
		{
			get { return mLeft; }
			set { mLeft = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Name																																	*
		//*-----------------------------------------------------------------------*
		private string mName = "";
		/// <summary>
		/// Get/Set the name of the item.
		/// </summary>
		[JsonProperty(Order = 0)]
		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ShouldSerializeName																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the Name property should be
		/// serialized.
		/// </summary>
		/// <returns>
		/// Value indicating whether to serialize the property.
		/// </returns>
		public bool ShouldSerializeName()
		{
			return mName.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Square																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a square area from the rectangle's volume.
		/// </summary>
		/// <param name="rectangle">
		/// Reference to the rectangle info whose area will be squared.
		/// </param>
		public static void Square(RectangleInfoItem rectangle)
		{
			float maxDim = 0f;

			if(rectangle != null && rectangle.Width != rectangle.Height)
			{
				maxDim = Math.Max(rectangle.mWidth, rectangle.mHeight);

				rectangle.mLeft -= ((maxDim - rectangle.mWidth) / 2f);
				rectangle.mTop -= ((maxDim - rectangle.mHeight) / 2f);
				rectangle.mWidth = maxDim;
				rectangle.mHeight = maxDim;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Top																																		*
		//*-----------------------------------------------------------------------*
		private float mTop = 0f;
		/// <summary>
		/// Get/Set the top coordinate of the rectangle.
		/// </summary>
		[JsonProperty(Order = 2)]
		public float Top
		{
			get { return mTop; }
			set { mTop = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of this item.
		/// </summary>
		/// <returns>
		/// The string representation of this rectangle information.
		/// </returns>
		public override string ToString()
		{
			return (mName?.Length > 0 ? $"\"Name\": \"{mName}\", " : "") +
				$"\"Left\": {mLeft}, \"Top\": {mTop}, " +
				$"\"Width\": {mWidth}, \"Height\": {mHeight}";
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Width																																	*
		//*-----------------------------------------------------------------------*
		private float mWidth = 0f;
		/// <summary>
		/// Get/Set the width of the object.
		/// </summary>
		[JsonProperty(Order = 3)]
		public float Width
		{
			get { return mWidth; }
			set { mWidth = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
