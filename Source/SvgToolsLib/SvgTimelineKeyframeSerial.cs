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
	//*	SvgTimelineKeyframeSerialCollection																			*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgTimelineKeyframeSerialItem Items.
	/// </summary>
	public class SvgTimelineKeyframeSerialCollection :
		List<SvgTimelineKeyframeSerialItem>
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
	//*	SvgTimelineKeyframeSerialItem																						*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// SVG timeline-style keyframe data serializer item.
	/// </summary>
	public class SvgTimelineKeyframeSerialItem
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
		//*	Absolute																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Absolute">Absolute</see>.
		/// </summary>
		private string mAbsolute = "";
		/// <summary>
		/// Get/Set a value indicating whether this frame uses absolute values.
		/// </summary>
		public string Absolute
		{
			get { return mAbsolute; }
			set { mAbsolute = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	BackColor																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="BackColor">BackColor</see>.
		/// </summary>
		private string mBackColor = "";
		/// <summary>
		/// Get/Set the background color for this setting.
		/// </summary>
		public string BackColor
		{
			get { return mBackColor; }
			set { mBackColor = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Easing																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Easing">Easing</see>.
		/// </summary>
		private string mEasing = "";
		/// <summary>
		/// Get/Set the easing function setting for this entry.
		/// </summary>
		/// <remarks>
		/// This value of this property corresponds to the easing-function CSS
		/// style.
		/// </remarks>
		public string Easing
		{
			get { return mEasing; }
			set { mEasing = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ForeColor																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ForeColor">ForeColor</see>.
		/// </summary>
		private string mForeColor = "";
		/// <summary>
		/// Get/Set the foreground color of this entry.
		/// </summary>
		public string ForeColor
		{
			get { return mForeColor; }
			set { mForeColor = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GroupName																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="GroupName">GroupName</see>.
		/// </summary>
		private string mGroupName = "";
		/// <summary>
		/// Get/Set the name of the group to which this keyframe is assigned.
		/// </summary>
		public string GroupName
		{
			get { return mGroupName; }
			set { mGroupName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Height																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Height">Height</see>.
		/// </summary>
		private string mHeight = "";
		/// <summary>
		/// Get/Set the value of the height dimension.
		/// </summary>
		public string Height
		{
			get { return mHeight; }
			set { mHeight = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Index																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Index">Index</see>.
		/// </summary>
		private string mIndex = "";
		/// <summary>
		/// Get/Set the sorting index of this item on the timeline.
		/// </summary>
		public string Index
		{
			get { return mIndex; }
			set { mIndex = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ObjectName																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ObjectName">ObjectName</see>.
		/// </summary>
		private string mObjectName = "";
		/// <summary>
		/// Get/Set the name of the object being updated.
		/// </summary>
		public string ObjectName
		{
			get { return mObjectName; }
			set { mObjectName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Opacity																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Opacity">Opacity</see>.
		/// </summary>
		private string mOpacity = "";
		/// <summary>
		/// Get/Set the opacity setting of this entry.
		/// </summary>
		public string Opacity
		{
			get { return mOpacity; }
			set { mOpacity = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Remark																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Remark">Remark</see>.
		/// </summary>
		private string mRemark = "";
		/// <summary>
		/// Get/Set a brief remark about the action.
		/// </summary>
		public string Remark
		{
			get { return mRemark; }
			set { mRemark = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Repeat																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Repeat">Repeat</see>.
		/// </summary>
		private string mRepeat = "";
		/// <summary>
		/// Get/Set the repeat pattern of this and releated entries.
		/// </summary>
		public string Repeat
		{
			get { return mRepeat; }
			set { mRepeat = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ScaleXOrigin																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ScaleXOrigin">ScaleXOrigin</see>.
		/// </summary>
		private string mScaleXOrigin = "";
		/// <summary>
		/// Get/Set the value of the X-scaling origin.
		/// </summary>
		/// <remarks>
		/// The value in this property corresponds to the horizontal or second
		/// parameter of the transform-origin CSS style.
		/// </remarks>
		public string ScaleXOrigin
		{
			get { return mScaleXOrigin; }
			set { mScaleXOrigin = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ScaleYOrigin																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ScaleYOrigin">ScaleYOrigin</see>.
		/// </summary>
		private string mScaleYOrigin = "";
		/// <summary>
		/// Get/Set the value of the Y-scaling origin.
		/// </summary>
		/// <remarks>
		/// The value in this property corresponds to the vertical or first
		/// parameter of the transform-origin CSS style.
		/// </remarks>
		public string ScaleYOrigin
		{
			get { return mScaleYOrigin; }
			set { mScaleYOrigin = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Seconds																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Seconds">Seconds</see>.
		/// </summary>
		private string mSeconds = "";
		/// <summary>
		/// Get/Set the temporal position of the keyframe, in seconds.
		/// </summary>
		public string Seconds
		{
			get { return mSeconds; }
			set { mSeconds = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Width																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Width">Width</see>.
		/// </summary>
		private string mWidth = "";
		/// <summary>
		/// Get/Set the value of the width dimension.
		/// </summary>
		public string Width
		{
			get { return mWidth; }
			set { mWidth = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	X																																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="X">X</see>.
		/// </summary>
		private string mX = "";
		/// <summary>
		/// Get/Set the value of the X position.
		/// </summary>
		public string X
		{
			get { return mX; }
			set { mX = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Y																																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Y">Y</see>.
		/// </summary>
		private string mY = "";
		/// <summary>
		/// Get/Set the value of the Y position.
		/// </summary>
		public string Y
		{
			get { return mY; }
			set { mY = value; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
