/*
 * Copyright (c). 2026 Daniel Patterson, MCSD (danielanywhere).
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

using Geometry;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	PlotRay2																																*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// A 2D ray with plotting context.
	/// </summary>
	public class PlotRay2
	{
		//	TODO: Create an FRay2 class in the Geometry library.
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
		//*	Direction																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Direction">Direction</see>.
		/// </summary>
		private FVector2 mDirection = new FVector2();
		/// <summary>
		/// Get/Set a reference to the direction.
		/// </summary>
		public FVector2 Direction
		{
			get { return mDirection; }
			set { mDirection = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Point																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Point">Point</see>.
		/// </summary>
		private FVector2 mPoint = new FVector2();
		/// <summary>
		/// Get/Set a reference to the reference point.
		/// </summary>
		public FVector2 Point
		{
			get { return mPoint; }
			set { mPoint = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ToolDown																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ToolDown">ToolDown</see>.
		/// </summary>
		private bool mToolDown = false;
		/// <summary>
		/// Get/Set a value indicating whether the tool is down for this ray.
		/// </summary>
		public bool ToolDown
		{
			get { return mToolDown; }
			set { mToolDown = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a string representation of this item.
		/// </summary>
		/// <returns>
		/// A string representation of this item.
		/// </returns>
		public override string ToString()
		{
			string ud = (mToolDown ? "D" : "U");
			return $"Pt:{mPoint} -> Dir:{mDirection}, {ud}";
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*


}
