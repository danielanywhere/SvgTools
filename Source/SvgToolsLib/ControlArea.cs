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
using System.Diagnostics;
using System.Linq;
using System.Text;

using Geometry;
using Html;
using static SvgToolsLib.SvgToolsUtil;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	ControlAreaCollection																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ControlAreaItem Items.
	/// </summary>
	public class ControlAreaCollection : List<ControlAreaItem>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* AppendMatches																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Append items matching the specified pattern to the list of items.
		/// </summary>
		/// <param name="areas">
		/// Reference to the collection of source areas to search.
		/// </param>
		/// <param name="targetItems">
		/// Reference to the collection of target items.
		/// </param>
		/// <param name="match">
		/// Reference to the function pattern to match.
		/// </param>
		private static void AppendMatches(
			List<ControlAreaItem> areas,
			List<ControlAreaItem> targetItems,
			Func<ControlAreaItem, bool> match)
		{
			if(areas?.Count > 0 && targetItems != null && match != null)
			{
				foreach(ControlAreaItem areaItem in areas)
				{
					if(match.Invoke(areaItem) &&
						!targetItems.Contains(areaItem))
					{
						targetItems.Add(areaItem);
					}
					AppendMatches(areaItem.FrontAreas, targetItems, match);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FillFlatAreaLevel																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Fill the caller's flat area level with the source items and their
		/// children.
		/// </summary>
		/// <param name="source">
		/// Reference to the source area structure to be copied.
		/// </param>
		/// <param name="target">
		/// Reference to the target area structure to build.
		/// </param>
		private static void FillFlatAreaLevel(List<ControlAreaItem> source,
			List<ControlAreaItem> target)
		{
			if(source?.Count > 0 && target != null)
			{
				//	Copy every item at the current level before proceding to the
				//	next z-order index.
				foreach(ControlAreaItem sourceAreaItem in source)
				{
					target.Add(sourceAreaItem);
				}
				foreach(ControlAreaItem sourceAreaItem in source)
				{
					FillFlatAreaLevel(sourceAreaItem.FrontAreas, target);
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
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new instance of the ControlAreaCollection item.
		/// </summary>
		public ControlAreaCollection()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new instance of the ControlAreaCollection item.
		/// </summary>
		public ControlAreaCollection(List<ControlAreaItem> areas)
		{
			if(areas?.Count > 0)
			{
				this.AddRange(areas);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Dump																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Dump the current area collection and its members to the active log
		/// writer.
		/// </summary>
		/// <param name="areas">
		/// Reference to the collection of controls being enumerated.
		/// </param>
		/// <param name="indent">
		/// Current indent level.
		/// </param>
		public static void Dump(ControlAreaCollection areas, int indent)
		{
			if(areas?.Count > 0)
			{
				foreach(ControlAreaItem areaItem in areas)
				{
					if(indent > 1)
					{
						Trace.Write(new string('-', indent - 1));
					}
					Trace.Write(' ');
					Trace.Write(areaItem.Intent.ToString());
					Trace.Write(' ');
					if(areaItem.Node != null)
					{
						Trace.Write(areaItem.Node.Id);
					}
					Trace.Write("\t{");
					Trace.Write(areaItem.X);
					Trace.Write(',');
					Trace.Write(areaItem.Y);
					Trace.Write(',');
					Trace.Write(areaItem.Width);
					Trace.Write(',');
					Trace.Write(areaItem.Height);
					Trace.WriteLine("}");
					Dump(areaItem.FrontAreas, indent + 1);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FindMatch																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Retrieve the first match found for the specified predicate.
		/// </summary>
		/// <param name="match">
		/// Reference to the predicate pattern to match.
		/// </param>
		/// <param name="recurse">
		/// Value indicating whether to recurse the descendant items of the
		/// tree while searching for matches.
		/// </param>
		/// <returns>
		/// Reference to the first match found for the specified pattern, if found.
		/// Otherwise, null.
		/// </returns>
		public ControlAreaItem FindMatch(Func<ControlAreaItem, bool> match,
			bool recurse = true)
		{
			ControlAreaItem result = null;

			if(match != null)
			{
				foreach(ControlAreaItem areaItem in this)
				{
					if(match.Invoke(areaItem))
					{
						result = areaItem;
						break;
					}
					if(result == null && recurse && areaItem.FrontAreas.Count > 0)
					{
						result = areaItem.FrontAreas.FindMatch(match);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FindMatches																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Retrieve all of the matches found for the specified predicate.
		/// </summary>
		/// <param name="match">
		/// Reference to the predicate pattern to match.
		/// </param>
		/// <param name="recurse">
		/// Value indicating whether to recurse the descendant items of the
		/// tree while searching for matches.
		/// </param>
		/// <returns>
		/// Reference to a list of control areas, if found. Otherwise, an empty
		/// list.
		/// </returns>
		public List<ControlAreaItem> FindMatches(Func<ControlAreaItem, bool> match,
			bool recurse = true)
		{
			List<ControlAreaItem> result = new List<ControlAreaItem>();

			if(match != null)
			{
				foreach(ControlAreaItem areaItem in this)
				{
					if(match.Invoke(areaItem))
					{
						result.Add(areaItem);
					}
					AppendMatches(areaItem.FrontAreas, result, match);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetFlatList																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a flat list of items in the control area collection, ordered by
		/// lowest z-order to highest.
		/// </summary>
		/// <param name="areas">
		/// Reference to the collection from which to create the flat list.
		/// </param>
		/// <returns>
		/// Reference to the list of controls in the tree, ordered from lowest
		/// z-order to highest.
		/// </returns>
		public static List<ControlAreaItem> GetFlatList(
			ControlAreaCollection areas)
		{
			List<ControlAreaItem> result = new List<ControlAreaItem>();

			if(areas?.Count > 0)
			{
				FillFlatAreaLevel(areas, result);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* PlaceInFront																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Place the provided area in front of the frontmost area over which
		/// that area overlaps.
		/// </summary>
		/// <param name="area">
		/// The area to find a place for.
		/// </param>
		/// <param name="areas">
		/// Reference to the collection of target areas to find a match.
		/// </param>
		public static void PlaceInFront(ControlAreaItem area,
			ControlAreaCollection areas)
		{
			bool bFound = false;
			int count = 0;
			List<ControlAreaItem> flat = null;
			ControlAreaItem item = null;
			int index = 0;
			FArea overlap = null;
			List<FArea> overlapItems = new List<FArea>();

			if(areas != null && area != null)
			{
				flat = ControlAreaCollection.GetFlatList(areas);
				count = flat.Count;
				for(index = count - 1; index > -1; index--)
				{
					item = flat[index];
					if(FArea.Contains(item, area))
					{
						//	The current area is squarely in the specified parent area.
						item.FrontAreas.Add(area);
						bFound = true;
						break;
					}
					else if(FArea.HasIntersection(item, area))
					{
						//	The area wasn't fully within the target, but there is an
						//	overlap.
						overlapItems.Clear();
						overlapItems.Add(item);
						overlapItems.Add(area);
						overlap = FArea.BoundingBox(overlapItems);

						if(Math.Abs(GetArea(overlap) - GetArea(item)) <
							(0.2f * Math.Min(GetArea(overlap), GetArea(item))))
						{
							//	The current item is 'mostly' overlapping the parent area.
							item.FrontAreas.Add(area);
							bFound = true;
							break;
						}
					}
				}
				if(!bFound)
				{
					areas.Add(area);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SortPosition																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Sort all of the areas of the collection by Horizontal then Vertical
		/// directions.
		/// </summary>
		/// <param name="areas">
		/// Reference to the collection of areas at which to begin sorting.
		/// </param>
		public static void SortPosition(ControlAreaCollection areas)
		{
			List<ControlAreaItem> sortedAreas = null;

			if(areas?.Count > 0)
			{
				sortedAreas = areas.OrderBy(x => x.X).ThenBy(y => y.Y).ToList();
				areas.Clear();
				areas.AddRange(sortedAreas);
				foreach(ControlAreaItem areaItem in areas)
				{
					SortPosition(areaItem.FrontAreas);
				}
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ControlAreaItem																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual control area.
	/// </summary>
	public class ControlAreaItem : FArea
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
		/// Create a new instance of the ControlAreaItem item.
		/// </summary>
		public ControlAreaItem()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*/
		/// <summary>
		/// Create a new instance of the ControlAreaItem item.
		/// </summary>
		public ControlAreaItem(ControlAreaItem area)
		{
			if(area != null)
			{
				TransferValues(area, this);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FrontAreas																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="FrontAreas">Areas</see>.
		/// </summary>
		private ControlAreaCollection mFrontAreas = new ControlAreaCollection();
		/// <summary>
		/// Get a reference to the collection of areas in front of this one.
		/// </summary>
		public ControlAreaCollection FrontAreas
		{
			get { return mFrontAreas; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Intent																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Intent">Intent</see>.
		/// </summary>
		private ImpliedDesignIntentEnum mIntent = ImpliedDesignIntentEnum.None;
		/// <summary>
		/// Get/Set the intent of this control.
		/// </summary>
		public ImpliedDesignIntentEnum Intent
		{
			get { return mIntent; }
			set { mIntent = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Node																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Node">Node</see>.
		/// </summary>
		private HtmlNodeItem mNode = null;
		/// <summary>
		/// Get/Set a reference to the HTML node to associate with this area.
		/// </summary>
		public HtmlNodeItem Node
		{
			get { return mNode; }
			set { mNode = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TransferValues																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Transfer the base member values from the source area to the target.
		/// </summary>
		/// <param name="source">
		/// Reference to the source area to be copied.
		/// </param>
		/// <param name="target">
		/// Reference to the target area to receive copies of the values.
		/// </param>
		/// <remarks>
		/// The FrontAreas collection is copied at the list level.
		/// </remarks>
		public static void TransferValues(ControlAreaItem source,
			ControlAreaItem target)
		{
			if(source != null && target != null)
			{
				FArea.TransferValues(source, target);
				target.mIntent = source.mIntent;
				target.mNode = source.mNode;
				target.mFrontAreas.AddRange(source.mFrontAreas);
				NameValueCollection.TransferValues(source.mProperties,
					target.mProperties);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Properties																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Properties">Properties</see>.
		/// </summary>
		private NameValueCollection mProperties = new NameValueCollection();
		/// <summary>
		/// Get a reference to the collection of custom properties for this area.
		/// </summary>
		public NameValueCollection Properties
		{
			get { return mProperties; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
