using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Geometry;
using Html;
using static SvgToolsLibrary.SvgToolsUtil;

namespace SvgToolsLibrary
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
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
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
					Trace.Write(new string(' ', indent));
					Trace.Write(areaItem.Intent.ToString());
					Trace.Write(' ');
					Trace.WriteLine(areaItem.Node.Id);
					Dump(areaItem.FrontAreas, indent + 1);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* PlaceInFront																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Place the provided area in front of the frontmost area over which
		/// that area overlaps.
		/// </summary>
		/// <param name="areas">
		/// Reference to the collection of areas to search.
		/// </param>
		/// <param name="area">
		/// The area to find a place for.
		/// </param>
		/// <returns>
		/// True if the area was placed. Otherwise, false.
		/// </returns>
		public static bool PlaceInFront(ControlAreaCollection areas,
			ControlAreaItem area)
		{
			bool bHasInterection = false;
			FArea overlap = null;
			List<FArea> overlapItems = new List<FArea>();
			bool result = false;

			if(areas != null && area != null)
			{
				foreach(ControlAreaItem areaItem in areas)
				{
					if(FArea.Contains(areaItem, area))
					{
						//	The current area is squarely in the specified parent area.
						if(!PlaceInFront(areaItem.FrontAreas, area))
						{
							areaItem.FrontAreas.Add(area);
							result = true;
							break;
						}
					}
					else
					{
						try
						{
							//	TODO: !1 - Stopped here...
							//	TODO: Geometry.FLine.cs Line 401. When 'point' is null. An exception is thrown.
							bHasInterection = FArea.HasIntersection(areaItem, area);
						}
						catch { }
						if(bHasInterection)
						{
							//	The area wasn't fully within the target, but there is an
							//	overlap.
							overlapItems.Clear();
							overlapItems.Add(areaItem);
							overlapItems.Add(area);
							overlap = FArea.BoundingBox(overlapItems);

							if(Math.Abs(GetArea(overlap) - GetArea(areaItem)) <
								(0.2f * Math.Min(GetArea(overlap), GetArea(areaItem))))
							{
								//	The current item is 'mostly' overlapping the parent area.
								if(!PlaceInFront(areaItem.FrontAreas, area))
								{
									areaItem.FrontAreas.Add(area);
									result = true;
									break;
								}
							}
						}
						else
						{
							//	This item might be located over one of the other objects.
							result = PlaceInFront(areaItem.FrontAreas, area);
							if(result)
							{
								break;
							}
						}
					}
				}
			}
			return result;
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

	}
	//*-------------------------------------------------------------------------*

}
