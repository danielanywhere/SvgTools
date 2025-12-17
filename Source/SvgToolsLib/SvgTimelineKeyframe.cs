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
using System.Data;
using System.Text;

using Newtonsoft.Json;

using static SvgToolsLib.SvgToolsUtil;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	SvgTimelineKeyframeCollection																						*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgTimelineKeyframeItem Items.
	/// </summary>
	public class SvgTimelineKeyframeCollection : List<SvgTimelineKeyframeItem>
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
		/// Create a new instance of the SvgTimelineKeyframeCollection item.
		/// </summary>
		public SvgTimelineKeyframeCollection()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new instance of the SvgTimelineKeyframeCollection item.
		/// </summary>
		/// <param name="table">
		/// Reference to a data table containing the timeline entries for this
		/// instance.
		/// </param>
		public SvgTimelineKeyframeCollection(DataTable table)
		{
			Initialize(this, table);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Initialize																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Initialize the provided timeline from the caller's supplied data.
		/// </summary>
		/// <param name="timeline">
		/// Reference to the timeline to be initialized.
		/// </param>
		/// <param name="table">
		/// Reference to the data table containing the information to represent.
		/// </param>
		public static void Initialize(SvgTimelineKeyframeCollection timeline,
			DataTable table)
		{
			Dictionary<int, SvgTimelineKeyframeFieldEnum> colIndices =
				new Dictionary<int, SvgTimelineKeyframeFieldEnum>();
			SvgTimelineKeyframeItem entry = null;
			int index = 0;

			if(timeline != null)
			{
				timeline.Clear();
				if(table?.Columns.Count > 0 && table.Rows.Count > 0)
				{
					//	Create column map.
					foreach(DataColumn columnItem in table.Columns)
					{
						switch(columnItem.ColumnName.ToLower())
						{
							case "abs":
							case "absolute":
							case "isabsolute":
								colIndices[index] = SvgTimelineKeyframeFieldEnum.Absolute;
								break;
							case "backcolor":
								colIndices[index] = SvgTimelineKeyframeFieldEnum.BackColor;
								break;
							case "forecolor":
								colIndices[index] = SvgTimelineKeyframeFieldEnum.ForeColor;
								break;
							case "easing":
								colIndices[index] = SvgTimelineKeyframeFieldEnum.Easing;
								break;
							case "group":
							case "groupname":
								colIndices[index] = SvgTimelineKeyframeFieldEnum.GroupName;
								break;
							case "height":
								colIndices[index] = SvgTimelineKeyframeFieldEnum.Height;
								break;
							case "index":
								colIndices[index] = SvgTimelineKeyframeFieldEnum.Index;
								break;
							case "object":
							case "objectname":
								colIndices[index] = SvgTimelineKeyframeFieldEnum.ObjectName;
								break;
							case "opacity":
								colIndices[index] = SvgTimelineKeyframeFieldEnum.Opacity;
								break;
							case "remark":
								colIndices[index] = SvgTimelineKeyframeFieldEnum.Remark;
								break;
							case "repeat":
								colIndices[index] = SvgTimelineKeyframeFieldEnum.Repeat;
								break;
							case "scalexorigin":
								colIndices[index] = SvgTimelineKeyframeFieldEnum.ScaleXOrigin;
								break;
							case "scaleyorigin":
								colIndices[index] = SvgTimelineKeyframeFieldEnum.ScaleYOrigin;
								break;
							case "seconds":
							case "time":
							case "timesec":
								colIndices[index] = SvgTimelineKeyframeFieldEnum.Seconds;
								break;
							case "width":
								colIndices[index] = SvgTimelineKeyframeFieldEnum.Width;
								break;
							case "x":
								colIndices[index] = SvgTimelineKeyframeFieldEnum.X;
								break;
							case "y":
								colIndices[index] = SvgTimelineKeyframeFieldEnum.Y;
								break;
						}
						index++;
					}
					foreach(DataRow rowItem in table.Rows)
					{
						entry = new SvgTimelineKeyframeItem();
						index = 0;
						foreach(string cellItem in rowItem.ItemArray)
						{
							if(colIndices.ContainsKey(index) && cellItem?.Length > 0)
							{
								switch(colIndices[index])
								{
									case SvgTimelineKeyframeFieldEnum.Absolute:
										if(ToBool(cellItem))
										{
											entry.Absolute = true;
											entry.ActiveFields |=
												SvgTimelineKeyframeFieldEnum.Absolute;
										}
										break;
									case SvgTimelineKeyframeFieldEnum.BackColor:
										entry.BackColor = cellItem;
										entry.ActiveFields |=
											SvgTimelineKeyframeFieldEnum.BackColor;
										break;
									case SvgTimelineKeyframeFieldEnum.Easing:
										entry.Easing = cellItem;
										entry.ActiveFields |=
											SvgTimelineKeyframeFieldEnum.Easing;
										break;
									case SvgTimelineKeyframeFieldEnum.ForeColor:
										entry.ForeColor = cellItem;
										entry.ActiveFields |=
											SvgTimelineKeyframeFieldEnum.ForeColor;
										break;
									case SvgTimelineKeyframeFieldEnum.GroupName:
										entry.GroupName = cellItem;
										entry.ActiveFields |=
											SvgTimelineKeyframeFieldEnum.GroupName;
										break;
									case SvgTimelineKeyframeFieldEnum.Height:
										entry.Height = cellItem;
										entry.ActiveFields |=
											SvgTimelineKeyframeFieldEnum.Height;
										break;
									case SvgTimelineKeyframeFieldEnum.Index:
										entry.Index = ToInt(cellItem);
										entry.ActiveFields |=
											SvgTimelineKeyframeFieldEnum.Index;
										break;
									case SvgTimelineKeyframeFieldEnum.ObjectName:
										entry.ObjectName = cellItem;
										entry.ActiveFields |=
											SvgTimelineKeyframeFieldEnum.ObjectName;
										break;
									case SvgTimelineKeyframeFieldEnum.Opacity:
										entry.Opacity = ToFloat(cellItem);
										entry.ActiveFields |=
											SvgTimelineKeyframeFieldEnum.Opacity;
										break;
									case SvgTimelineKeyframeFieldEnum.Remark:
										entry.Remark = cellItem;
										entry.ActiveFields |=
											SvgTimelineKeyframeFieldEnum.Remark;
										break;
									case SvgTimelineKeyframeFieldEnum.Repeat:
										entry.Repeat = cellItem;
										entry.ActiveFields |=
											SvgTimelineKeyframeFieldEnum.Repeat;
										break;
									case SvgTimelineKeyframeFieldEnum.ScaleXOrigin:
										entry.ScaleXOrigin = cellItem;
										entry.ActiveFields |=
											SvgTimelineKeyframeFieldEnum.ScaleXOrigin;
										break;
									case SvgTimelineKeyframeFieldEnum.ScaleYOrigin:
										entry.ScaleYOrigin = cellItem;
										entry.ActiveFields |=
											SvgTimelineKeyframeFieldEnum.ScaleYOrigin;
										break;
									case SvgTimelineKeyframeFieldEnum.Seconds:
										entry.Seconds = ToFloat(cellItem);
										entry.ActiveFields |=
											SvgTimelineKeyframeFieldEnum.Seconds;
										break;
									case SvgTimelineKeyframeFieldEnum.Width:
										entry.Width = cellItem;
										entry.ActiveFields |=
											SvgTimelineKeyframeFieldEnum.Width;
										break;
									case SvgTimelineKeyframeFieldEnum.X:
										entry.X = cellItem;
										entry.ActiveFields |=
											SvgTimelineKeyframeFieldEnum.X;
										break;
									case SvgTimelineKeyframeFieldEnum.Y:
										entry.Y = cellItem;
										entry.ActiveFields |=
											SvgTimelineKeyframeFieldEnum.Y;
										break;
								}
							}
							index++;
						}
						if(entry.ActiveFields != SvgTimelineKeyframeFieldEnum.None)
						{
							timeline.Add(entry);
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	SvgTimelineKeyframeItem																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual object keyframe on the timeline.
	/// </summary>
	public class SvgTimelineKeyframeItem
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
		private bool mAbsolute = false;
		/// <summary>
		/// Get/Set a value indicating whether this frame uses absolute values.
		/// </summary>
		public bool Absolute
		{
			get { return mAbsolute; }
			set { mAbsolute = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ActiveFields																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ActiveFields">ActiveFields</see>.
		/// </summary>
		private SvgTimelineKeyframeFieldEnum mActiveFields =
			SvgTimelineKeyframeFieldEnum.None;
		/// <summary>
		/// Get/Set flags for the fields actively defined on this keyframe.
		/// </summary>
		[JsonIgnore]
		public SvgTimelineKeyframeFieldEnum ActiveFields
		{
			get { return mActiveFields; }
			set { mActiveFields = value; }
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
		private int mIndex = 0;
		/// <summary>
		/// Get/Set the sorting index of this item on the timeline.
		/// </summary>
		public int Index
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
		private float mOpacity = 0f;
		/// <summary>
		/// Get/Set the opacity setting of this entry.
		/// </summary>
		public float Opacity
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
		private float mSeconds = 0f;
		/// <summary>
		/// Get/Set the temporal position of the keyframe, in seconds.
		/// </summary>
		public float Seconds
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
