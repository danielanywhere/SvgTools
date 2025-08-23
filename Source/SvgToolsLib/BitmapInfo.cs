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

using SkiaSharp;

namespace SvgToolsLibrary
{
	//*-------------------------------------------------------------------------*
	//*	BitmapInfoCollection																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of BitmapInfoItem Items.
	/// </summary>
	public class BitmapInfoCollection : List<BitmapInfoItem>
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
	//*	BitmapInfoItem																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual loaded bitmap.
	/// </summary>
	public class BitmapInfoItem
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
		//*	Bitmap																																*
		//*-----------------------------------------------------------------------*
		private SKBitmap mBitmap = null;
		/// <summary>
		/// Get/Set a reference to the bitmap object.
		/// </summary>
		public SKBitmap Bitmap
		{
			get { return mBitmap; }
			set { mBitmap = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Name																																	*
		//*-----------------------------------------------------------------------*
		private string mName = "";
		/// <summary>
		/// Get/Set the local name of the image.
		/// </summary>
		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
