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

using Html;
using System;
using System.Collections.Generic;
using System.Text;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	SvgTimelineCollection																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgTimelineItem Items.
	/// </summary>
	public class SvgTimelineCollection : List<SvgTimelineItem>
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
	//*	SvgTimelineItem																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// An individual timeline of events for SVG animations.
	/// </summary>
	public class SvgTimelineItem
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
		//*	GroupName																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="GroupName">GroupName</see>.
		/// </summary>
		private string mGroupName = "";
		/// <summary>
		/// Get/Set the name of the group associated with this timeline.
		/// </summary>
		public string GroupName
		{
			get { return mGroupName; }
			set { mGroupName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Keyframes																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Keyframes">Keyframes</see>.
		/// </summary>
		private SvgTimelineKeyframeCollection mKeyframes =
			new SvgTimelineKeyframeCollection();
		/// <summary>
		/// Get a reference to the collection of keyframes for this timeline.
		/// </summary>
		public SvgTimelineKeyframeCollection Keyframes
		{
			get { return mKeyframes; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Nodes																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Nodes">Nodes</see>.
		/// </summary>
		private SvgTimelineNodeCollection mNodes = new SvgTimelineNodeCollection();
		/// <summary>
		/// Get a reference to the list of object nodes in this timeline.
		/// </summary>
		public SvgTimelineNodeCollection Nodes
		{
			get { return mNodes; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
