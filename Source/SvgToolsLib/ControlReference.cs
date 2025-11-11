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
using System.Linq;
using System.Text;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	ControlReferenceCollection																							*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ControlReferenceItem Items.
	/// </summary>
	public class ControlReferenceCollection : List<ControlReferenceItem>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		/// <summary>
		/// The horizontal tolerance per band, when sorting spatially.
		/// </summary>
		private const float mHorizontalTolerance = 5f;

		//*-----------------------------------------------------------------------*
		//* FillTree																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Fill the provided tree with references to the provided areas.
		/// </summary>
		/// <param name="areas">
		/// Reference to the collection of control areas to be represented.
		/// </param>
		/// <param name="parents">
		/// Reference to the collection of candidate parent reference nodes for
		/// the presented areas.
		/// </param>
		private static void FillTree(List<ControlAreaItem> areas,
			List<ControlReferenceItem> parents)
		{
			float minX = float.MaxValue;
			ControlReferenceItem parent = null;
			ControlReferenceItem reference = null;
			List<ControlReferenceItem> references = null;
			List<ControlAreaItem> remaining = null;
			List<ControlReferenceItem> ySort = null;

			if(areas?.Count > 0 && parents?.Count > 0)
			{
				ySort = parents.OrderByDescending(y => y.Area.Y).ToList();
				references = new List<ControlReferenceItem>();
				foreach(ControlAreaItem areaItem in areas)
				{
					minX = Math.Min(areaItem.X, minX);
				}
				foreach(ControlAreaItem areaItem in areas)
				{
					if(areaItem.X - minX < mHorizontalTolerance)
					{
						parent = ySort.FirstOrDefault(y => areaItem.Y > y.Area.Y);
						if(parent == null)
						{
							parent = parents[0];
						}
						reference = new ControlReferenceItem()
						{
							Area = areaItem
						};
						parent.References.Add(reference);
						references.Add(reference);
					}
					else
					{
						remaining.Add(areaItem);
					}
				}
				if(remaining.Count > 0)
				{
					FillTree(remaining, references);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************

		//*-----------------------------------------------------------------------*
		//*	CreateTree																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the tree structure represented by the caller's staggered areas.
		/// </summary>
		/// <param name="areas">
		/// Reference to the list of control areas being enumerated.
		/// </param>
		/// <returns>
		/// Reference to a tree of control area references, as found in a
		/// left-to-right, top-to-bottom order.
		/// </returns>
		public static ControlReferenceCollection CreateTree(
			List<ControlAreaItem> areas)
		{
			float minX = float.MaxValue;
			List<ControlAreaItem> remaining = null;
			ControlReferenceCollection result = new ControlReferenceCollection();

			if(areas?.Count > 0)
			{
				foreach(ControlAreaItem areaItem in areas)
				{
					minX = Math.Min(areaItem.X, minX);
				}
				foreach(ControlAreaItem areaItem in areas)
				{
					if(areaItem.X - minX < mHorizontalTolerance)
					{
						result.Add(new ControlReferenceItem()
						{
							Area = areaItem
						});
					}
					else
					{
						remaining.Add(areaItem);
					}
				}
				if(remaining.Count > 0)
				{
					FillTree(remaining, result);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ControlReferenceItem																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about a reference to a single control area.
	/// </summary>
	public class ControlReferenceItem
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
		//*	Area																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Area">Area</see>.
		/// </summary>
		private ControlAreaItem mArea = null;
		/// <summary>
		/// Get/Set a reference to the control area being referenced.
		/// </summary>
		public ControlAreaItem Area
		{
			get { return mArea; }
			set { mArea = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	References																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="References">References</see>.
		/// </summary>
		private ControlReferenceCollection mReferences =
			new ControlReferenceCollection();
		/// <summary>
		/// Get a reference to the collection of control references grouped by this
		/// one.
		/// </summary>
		public ControlReferenceCollection References
		{
			get { return mReferences; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
