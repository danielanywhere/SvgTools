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

using static SvgToolsLib.SvgToolsUtil;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	SvgPathMoveActionCollection																							*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgPathMoveActionItem Items.
	/// </summary>
	public class SvgPathMoveActionCollection : List<SvgPathMoveActionItem>
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
	//*	SvgPathMoveActionItem																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual cursor move action.
	/// </summary>
	public class SvgPathMoveActionItem : SvgPathActionItem
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
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new instance of the SvgPathMoveActionItem item.
		/// </summary>
		public SvgPathMoveActionItem()
		{
			mActionType = SvgPathActionType.Move;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FromPlotPoint																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Inspect the caller's plot point and return a strongly-typed
		/// representation.
		/// </summary>
		/// <param name="plotPoint">
		/// The parameterized plot point of a cursor move action.
		/// </param>
		/// <param name="currentPoint">
		/// The last-known current coordinate.
		/// </param>
		/// <param name="lastMovePoint">
		/// The last-known move-to coordinate.
		/// </param>
		/// <returns>
		/// Reference to a new cursor move action item representing the
		/// caller's parameters, if found. Otherwise, null.
		/// </returns>
		public static SvgPathMoveActionItem FromPlotPoint(
			PlotPointsFItem plotPoint, FVector2 currentPoint, FVector2 lastMovePoint)
		{
			SvgPathMoveActionItem result = null;

			if(plotPoint?.Action?.Length > 0 && plotPoint.Points.Count > 1 &&
				currentPoint != null && lastMovePoint != null)
			{
				result = new SvgPathMoveActionItem();
				FVector2.TransferValues(currentPoint, result.Start);
				if(IsUpperCase(plotPoint.Action[0]))
				{
					//	Absolute.
					result.End.X = plotPoint.Points[0];
					result.End.Y = plotPoint.Points[1];
				}
				else
				{
					//	Relative.
					result.End.X = currentPoint.X + plotPoint.Points[0];
					result.End.Y = currentPoint.Y + plotPoint.Points[1];
				}
				FVector2.TransferValues(result.End, currentPoint);
				FVector2.TransferValues(result.End, lastMovePoint);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
