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
	//*	BoundingObjectCollection																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of BoundingObjectItem Items.
	/// </summary>
	public class BoundingObjectCollection : List<BoundingObjectItem>
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
	//*	BoundingObjectItem																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual bounding object definition.
	/// </summary>
	public class BoundingObjectItem
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
		//* Copy																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a fresh copy of this bounding object.
		/// </summary>
		/// <returns>
		/// Newly created bounding object as a copy of this one.
		/// </returns>
		public BoundingObjectItem Copy()
		{
			BoundingObjectItem result = new BoundingObjectItem()
			{
				mMaxX = this.mMaxX,
				mMaxY = this.mMaxY,
				mMinX = this.mMinX,
				mMinY = this.mMinY
			};
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Fix																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the boundary values to a fixed resolution.
		/// </summary>
		/// <param name="decimalPlaces">
		/// Number of decimal places allowed on the resulting value.
		/// </param>
		public void Fix(int decimalPlaces)
		{
			float number = 0f;

			if(float.TryParse(mMaxX.ToString($"f{decimalPlaces}"), out number))
			{
				mMaxX = number;
			}
			if(float.TryParse(mMaxY.ToString($"f{decimalPlaces}"), out number))
			{
				mMaxY = number;
			}
			if(float.TryParse(mMinX.ToString($"f{decimalPlaces}"), out number))
			{
				mMinX = number;
			}
			if(float.TryParse(mMinY.ToString($"f{decimalPlaces}"), out number))
			{
				mMinY = number;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetArea																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the area of the object.
		/// </summary>
		/// <returns>
		/// The area of the bounding object.
		/// </returns>
		public float GetArea()
		{
			return GetWidth() * GetHeight();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetHeight																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the height of the object.
		/// </summary>
		/// <returns>
		/// The height of the bounding area.
		/// </returns>
		public float GetHeight()
		{
			return Math.Abs(mMaxY - mMinY);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetWidth																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the width of the object.
		/// </summary>
		/// <returns>
		/// The width of the bounding area.
		/// </returns>
		public float GetWidth()
		{
			return Math.Abs(mMaxX - mMinX);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsZero																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the caller's bounding object is zero.
		/// </summary>
		/// <param name="bounds">
		/// Reference to the bounding object to test.
		/// </param>
		/// <returns>
		/// True if the bounding object is zero. Otherwise, false.
		/// </returns>
		public static bool IsZero(BoundingObjectItem bounds)
		{
			bool result = true;

			if(bounds != null &&
				(Math.Abs(bounds.mMaxX) > float.Epsilon ||
				Math.Abs(bounds.mMaxY) > float.Epsilon ||
				Math.Abs(bounds.mMinX) > float.Epsilon ||
				Math.Abs(bounds.mMinY) > float.Epsilon))
			{
				result = false;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	MaxX																																	*
		//*-----------------------------------------------------------------------*
		private float mMaxX = float.MinValue;
		/// <summary>
		/// Get/Set the maximum X coordinate.
		/// </summary>
		public float MaxX
		{
			get { return mMaxX; }
			set { mMaxX = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	MaxY																																	*
		//*-----------------------------------------------------------------------*
		private float mMaxY = float.MinValue;
		/// <summary>
		/// Get/Set the maximum Y coordinate.
		/// </summary>
		public float MaxY
		{
			get { return mMaxY; }
			set { mMaxY = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Merge																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Merge two boundary objects together and return the result.
		/// </summary>
		/// <param name="bounds1">
		/// Reference to the first bounding object to compare.
		/// </param>
		/// <param name="bounds2">
		/// Reference to the second bounding object to compare.
		/// </param>
		/// <returns>
		/// Reference to the bounding object resulting from the two constituent
		/// items.
		/// </returns>
		public static BoundingObjectItem Merge(BoundingObjectItem bounds1,
			BoundingObjectItem bounds2)
		{
			BoundingObjectItem result = new BoundingObjectItem();

			if(bounds1 != null)
			{
				if(bounds2 != null)
				{
					//	Both bounding objects presented.
					result.mMaxX = Math.Max(bounds1.mMaxX, bounds2.mMaxX);
					result.mMaxY = Math.Max(bounds1.mMaxY, bounds2.mMaxY);
					result.mMinX = Math.Min(bounds1.mMinX, bounds2.mMinX);
					result.mMinY = Math.Min(bounds1.mMinY, bounds2.mMinY);
				}
				else
				{
					//	Only the left bounding object was present.
					result.mMaxX = bounds1.mMaxX;
					result.mMaxY = bounds1.mMaxY;
					result.mMinX = bounds1.mMinX;
					result.mMinY = bounds1.mMinY;
				}
			}
			else if(bounds2 != null)
			{
				result.mMaxX = bounds2.mMaxX;
				result.mMaxY = bounds2.mMaxY;
				result.mMinX = bounds2.mMinX;
				result.mMinY = bounds2.mMinY;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	MinX																																	*
		//*-----------------------------------------------------------------------*
		private float mMinX = float.MaxValue;
		/// <summary>
		/// Get/Set the minimum X coordinate.
		/// </summary>
		public float MinX
		{
			get { return mMinX; }
			set { mMinX = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	MinY																																	*
		//*-----------------------------------------------------------------------*
		private float mMinY = float.MaxValue;
		/// <summary>
		/// Get/Set the minimum Y coordinate.
		/// </summary>
		public float MinY
		{
			get { return mMinY; }
			set { mMinY = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* MoveRelative																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Move the extents relative to their current position.
		/// </summary>
		/// <param name="dx">
		/// Distance to move along the X axis.
		/// </param>
		/// <param name="dy">
		/// Distance to move along the Y axis.
		/// </param>
		public void MoveRelative(float dx, float dy)
		{
			this.mMinX += dx;
			this.mMinY += dy;
			this.mMaxX += dx;
			this.mMaxY += dy;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* MoveTo																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Move the object to a specified target coordinate.
		/// </summary>
		/// <param name="x">
		/// X coordinate to which the bounding box will be moved.
		/// </param>
		/// <param name="y">
		/// Y coordinate to which the bounding box will be moved.
		/// </param>
		public void MoveTo(float x, float y)
		{
			float dx = x - this.mMinX;
			float dy = y - this.mMinY;

			this.mMinX = x;
			this.mMinY = y;
			this.mMaxX += dx;
			this.mMaxY += dy;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Reset																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Reset the extents of the bounding box to uninitialized.
		/// </summary>
		/// <param name="axis">
		/// Optional axis or axes to reset. Valid options are 'X' and 'Y'.
		/// </param>
		public void Reset(string axis = "XY")
		{
			if(axis?.Length > 0)
			{
				if(axis.ToLower().Contains("x"))
				{
					mMinX = float.MaxValue;
					mMinY = float.MaxValue;
				}
				if(axis.ToLower().Contains("y"))
				{
					mMaxX = float.MinValue;
					mMaxY = float.MinValue;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetHeight																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the height of this bounding object by adjusting the MaxY value.
		/// </summary>
		/// <param name="height">
		/// New height of the area.
		/// </param>
		public void SetHeight(float height)
		{
			mMaxY = mMinY + height;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SetWidth																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the width of this bounding object by adjusting the MaxX value.
		/// </summary>
		/// <param name="width">
		/// New width of the area.
		/// </param>
		public void SetWidth(float width)
		{
			mMaxX = mMinX + width;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Zero																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Zero the extents of the bounding box.
		/// </summary>
		/// <param name="axis">
		/// Optional axis or axes to reset. Valid options are 'X' and 'Y'.
		/// </param>
		public void Zero(string axis = "XY")
		{
			if(axis?.Length > 0)
			{
				if(axis.ToLower().Contains("x"))
				{
					mMinX = 0f;
					mMinY = 0f;
				}
				if(axis.ToLower().Contains("y"))
				{
					mMaxX = 0f;
					mMaxY = 0f;
				}
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
