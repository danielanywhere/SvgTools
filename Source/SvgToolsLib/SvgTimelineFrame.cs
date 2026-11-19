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
		//*	GetCommonFloatValue																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the common float value for the specified common property, as it
		/// can be inferred for the named object at the given frame index.
		/// </summary>
		/// <param name="frameIndex">
		/// The frame index at which the value is desired.
		/// </param>
		/// <param name="objectName">
		/// The name of the object for which the value will be retrieved.
		/// </param>
		/// <param name="propertyName">
		/// Name of the associated property to retrieve.
		/// </param>
		/// <returns>
		/// The last-known value of the common property at or prior to the
		/// given frame, if found. Otherwise, 0.
		/// </returns>
		public float GetCommonFloatValue(int frameIndex, string objectName,
			string propertyName)
		{
			int currentFrameIndex = frameIndex;
			SvgTimelineFrameItem frame = null;
			float result = 0f;

			if(objectName?.Length > 0 && propertyName?.Length > 0)
			{
				for(currentFrameIndex = frameIndex;
					currentFrameIndex >= 0;
					currentFrameIndex --)
				{
					frame = GetFrame(currentFrameIndex);
					if(frame != null)
					{
						if(frame.CommonValues.FloatValueExists(objectName, propertyName))
						{
							result =
								frame.CommonValues.GetFloatValue(objectName, propertyName);
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

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
		//*	ApplyCommonValues																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Assign the shared common properties for the referenced object on the
		/// current frame.
		/// </summary>
		/// <param name="keyframe">
		/// Reference to the keyframe whose per-object common values will be
		/// copied.
		/// </param>
		public void ApplyCommonValues(SvgTimelineKeyframeItem keyframe)
		{
			if(keyframe?.ObjectName?.Length > 0)
			{
				CommonValues.ApplyValues(keyframe);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	CommonValues																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="CommonValues">CommonValues</see>.
		/// </summary>
		private SvgTimelineFrameCommonValueCollection mCommonValues =
			new SvgTimelineFrameCommonValueCollection();
		/// <summary>
		/// Get a reference to the collection of common values shared per object
		/// on this frame.
		/// </summary>
		public SvgTimelineFrameCommonValueCollection CommonValues
		{
			get { return mCommonValues; }
		}
		//*-----------------------------------------------------------------------*

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
