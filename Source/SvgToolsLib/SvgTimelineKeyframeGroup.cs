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

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	SvgTimelineKeyframeGroupCollection																			*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgTimelineKeyframeGroupItem Items.
	/// </summary>
	public class SvgTimelineKeyframeGroupCollection : List<SvgTimelineKeyframeGroupItem>
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
	//*	SvgTimelineKeyframeGroupItem																						*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual group of timeline keyframe entries.
	/// </summary>
	public class SvgTimelineKeyframeGroupItem
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
		//*	Keyframes																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Keyframes">Keyframes</see>.
		/// </summary>
		private SvgTimelineKeyframeCollection mKeyframes =
			new SvgTimelineKeyframeCollection();
		/// <summary>
		/// Get a reference to the collection of timeline keyframes in this group.
		/// </summary>
		public SvgTimelineKeyframeCollection Keyframes
		{
			get { return mKeyframes; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Name																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Name">Name</see>.
		/// </summary>
		private string mName = "";
		/// <summary>
		/// Get/Set the name of this group.
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
