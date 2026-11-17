/*
 * Copyright (c). 2025-2026 Daniel Patterson, MCSD (danielanywhere).
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
	//*	SvgTimelineFrameCollection																							*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgTimelineFrameItem Items.
	/// </summary>
	public class SvgTimelineFrameCollection : List<SvgTimelineFrameItem>
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
		//*	GetFrame																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a reference to the frame container at the specified frame index.
		/// </summary>
		/// <param name="frameIndex">
		/// Frame index of the frame to retrieve.
		/// </param>
		/// <returns>
		/// Reference to the frame container, if found. Otherwise, null.
		/// </returns>
		public SvgTimelineFrameItem GetFrame(int frameIndex)
		{
			SvgTimelineFrameItem result = null;

			result = this.FirstOrDefault(x => x.FrameIndex == frameIndex);
			return result;
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	SvgTimelineFrameItem																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual frame index activity container on the
	/// timeline.
	/// </summary>
	public class SvgTimelineFrameItem
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
		//*	Entries																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Entries">Entries</see>.
		/// </summary>
		private SvgTimelineKeyframeCollection mEntries =
			new SvgTimelineKeyframeCollection();
		/// <summary>
		/// Get a reference to the collection of keyframes that have an activity
		/// on this frame.
		/// </summary>
		public SvgTimelineKeyframeCollection Entries
		{
			get { return mEntries; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FrameIndex																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="FrameIndex">FrameIndex</see>.
		/// </summary>
		private int mFrameIndex = 0;
		/// <summary>
		/// Get/Set the index of the frame being tracked.
		/// </summary>
		public int FrameIndex
		{
			get { return mFrameIndex; }
			set { mFrameIndex = value; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
