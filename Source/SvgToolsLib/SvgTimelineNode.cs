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
using System.Security.Cryptography;
using System.Text;

using Html;

using static SvgToolsLib.SvgToolsUtil;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	SvgTimelineNodeCollection																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgTimelineNodeItem Items.
	/// </summary>
	public class SvgTimelineNodeCollection : List<SvgTimelineNodeItem>
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
	//*	SvgTimelineNodeItem																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Html node association to a timeline for quick association.
	/// </summary>
	public class SvgTimelineNodeItem
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
		/// Create a new instance of the SvgTimelineNodeItem item.
		/// </summary>
		public SvgTimelineNodeItem()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new instance of the SvgTimelineNodeItem item.
		/// </summary>
		/// <param name="node">
		/// Reference to the HTML node to associate.
		/// </param>
		public SvgTimelineNodeItem(HtmlNodeItem node)
		{
			if(node != null)
			{
				mName = node.Id;
				mWidth = GetWidth(node);
				mHeight = GetHeight(node);
				mX = GetX(node);
				mY = GetY(node);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GenerateAnimationCss																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the CSS animation information for the provided timeline and
		/// node association.
		/// </summary>
		/// <param name="node">
		/// Reference to the timeline node association for which the animation
		/// CSS will be generated.
		/// </param>
		/// <returns>
		/// A string containing the animation CSS styles for the specified node,
		/// if found. Otherwise, an empty string.
		/// </returns>
		public static string GenerateAnimationCss(SvgTimelineNodeItem node)
		{
			StringBuilder builder = new StringBuilder();
			string easing = "linear";
			string frameName = "";
			SvgTimelineKeyframeItem keyframe = null;
			List<SvgTimelineKeyframeItem> keyframes = null;
			float maxTime = 0f;
			string measurement = "";
			float minTime = 0f;
			float number = 0f;
			float percent = 0f;
			string repeat = "";
			string scaleXOrigin = "center";
			string scaleYOrigin = "center";
			List<float> times = null;
			StringBuilder transform = new StringBuilder();


			if(node?.mKeyframes.Count > 0)
			{
				//	Configure the static values.
				minTime = node.mKeyframes.Min(n => n.Seconds);
				maxTime = node.mKeyframes.Max(n => n.Seconds);
				keyframe = node.mKeyframes.FirstOrDefault(n =>
					n.Easing.Length > 0);
				if(keyframe != null)
				{
					easing = keyframe.Easing;
				}
				keyframe = node.mKeyframes.FirstOrDefault(n =>
					n.Repeat.Length > 0);
				if(keyframe != null)
				{
					repeat = keyframe.Repeat;
				}
				keyframe = node.mKeyframes.FirstOrDefault(n =>
					n.ScaleXOrigin.Length > 0);
				if(keyframe != null)
				{
					scaleXOrigin = keyframe.ScaleXOrigin;
				}
				keyframe = node.mKeyframes.FirstOrDefault(n =>
					n.ScaleYOrigin.Length > 0);
				if(keyframe != null)
				{
					scaleYOrigin = keyframe.ScaleYOrigin;
				}
				times = node.mKeyframes.Select(k => k.Seconds).Distinct().ToList();
				times.Sort();
				foreach(float timeItem in times)
				{
					Clear(transform);
					percent = (timeItem / maxTime) * 100f;
					AppendLineIndented(builder, 1, $"{percent:0.###}%");
					AppendLineIndented(builder, 1, "{");
					keyframes = node.mKeyframes.FindAll(k => k.Seconds == timeItem);
					foreach(SvgTimelineKeyframeItem keyframeItem in keyframes)
					{
						if(keyframeItem.ActiveFields != SvgTimelineKeyframeFieldEnum.None)
						{
							//	Back color.
							if((keyframeItem.ActiveFields &
								SvgTimelineKeyframeFieldEnum.BackColor) != 0)
							{
								AppendLineIndented(builder, 2,
									$"fill: {keyframeItem.BackColor}");
							}
							//	Fore color.
							if((keyframeItem.ActiveFields &
								SvgTimelineKeyframeFieldEnum.ForeColor) != 0)
							{
								AppendLineIndented(builder, 2,
									$"stroke: {keyframeItem.ForeColor}");
							}
							//	X.
							if((keyframeItem.ActiveFields &
								SvgTimelineKeyframeFieldEnum.X) != 0)
							{
								measurement = keyframeItem.X;
								if(keyframeItem.Absolute)
								{
									measurement = MeasurementWithUnit(measurement);
									number = ToPixels(measurement);
									number -= node.X;
									if(transform.Length > 0)
									{
										transform.Append(' ');
									}
									transform.Append($"translateX({number:0.##}px)");
								}
								else
								{
									number = ToFloat(GetNumeric(measurement));
									if(transform.Length > 0)
									{
										transform.Append(' ');
									}
									transform.Append($"translateX({number:0.##}px)");
								}
							}
							//	Y.
							if((keyframeItem.ActiveFields &
								SvgTimelineKeyframeFieldEnum.Height) != 0)
							{
								measurement = keyframeItem.Y;
								if(keyframeItem.Absolute)
								{
									measurement = MeasurementWithUnit(measurement);
									number = ToPixels(measurement);
									number -= node.Y;
									if(transform.Length > 0)
									{
										transform.Append(' ');
									}
									transform.Append($"translateY({number:0.##}px)");
								}
								else
								{
									number = ToFloat(GetNumeric(measurement));
									if(transform.Length > 0)
									{
										transform.Append(' ');
									}
									transform.Append($"translateY({number:0.##}px)");
								}
							}
							//	Width.
							if((keyframeItem.ActiveFields &
								SvgTimelineKeyframeFieldEnum.Width) != 0)
							{
								measurement = keyframeItem.Width;
								if(keyframeItem.Absolute)
								{
									measurement = MeasurementWithUnit(measurement);
									number = ToPixels(measurement);
									if(node.Width != 0f)
									{
										number /= node.Width;
									}
									else
									{
										number = 1f;
									}
									if(transform.Length > 0)
									{
										transform.Append(' ');
									}
									transform.Append($"scaleX({number})");
								}
								else
								{
									number = ToFloat(GetNumeric(measurement));
									if(transform.Length > 0)
									{
										transform.Append(' ');
									}
									transform.Append($"scaleX({number})");
								}
							}
							//	Height.
							if((keyframeItem.ActiveFields &
								SvgTimelineKeyframeFieldEnum.Height) != 0)
							{
								measurement = keyframeItem.Height;
								if(keyframeItem.Absolute)
								{
									measurement = MeasurementWithUnit(measurement);
									number = ToPixels(measurement);
									if(node.Height != 0f)
									{
										number /= node.Height;
									}
									else
									{
										number = 1f;
									}
									if(transform.Length > 0)
									{
										transform.Append(' ');
									}
									transform.Append($"scaleY({number})");
								}
								else
								{
									number = ToFloat(GetNumeric(measurement));
									if(transform.Length > 0)
									{
										transform.Append(' ');
									}
									transform.Append($"scaleY({number})");
								}
							}
							//	Opacity.
							if((keyframeItem.ActiveFields &
								SvgTimelineKeyframeFieldEnum.Opacity) != 0)
							{
								AppendLineIndented(builder, 2,
									$"opacity: {keyframeItem.Opacity:0.##};");
							}
						}
					}
					if(transform.Length > 0)
					{
						AppendLineIndented(builder, 2, $"transform: {transform};");
					}
					AppendLineIndented(builder, 1, "}");
				}
				if(builder.Length > 0)
				{
					//	If a keyframes list was built, then seal it and create an
					//	associated object style.
					frameName = $"{node.mName}{Right(Guid.NewGuid().ToString("N"), 8)}";
					builder.Insert(0, $"@keyframes {frameName}\r\n{{\r\n");
					builder.AppendLine("}");
					builder.AppendLine($"#{node.mName}");
					builder.AppendLine("{");
					builder.Append('\t');
					builder.Append("animation: ");
					builder.Append($"{frameName} ");
					builder.Append($"{maxTime}s ");
					builder.Append($"{easing} ");
					builder.Append("forwards");
					builder.AppendLine(";");
					if(repeat.Length > 0)
					{
						AppendLineIndented(builder, 1,
							$"animation-iteration-count: {repeat};");
					}
					AppendLineIndented(builder, 1, "transform-box: fill-box;");
					builder.Append("\ttransform-origin: ");
					builder.AppendLine($"{scaleYOrigin} {scaleXOrigin};");
					builder.AppendLine("}");
				}
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Height																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Height">Height</see>.
		/// </summary>
		private float mHeight = 0f;
		/// <summary>
		/// Get/Set the reference height of the object.
		/// </summary>
		public float Height
		{
			get { return mHeight; }
			set { mHeight = value; }
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
		/// Get a reference to the collection of keyframes on this node.
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
		/// Get/Set the object name of the node.
		/// </summary>
		public string Name
		{
			get { return mName; }
			set { mName = value; }
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
		/// Get/Set a reference to the associated HTML node.
		/// </summary>
		public HtmlNodeItem Node
		{
			get { return mNode; }
			set { mNode = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Width																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Width">Width</see>.
		/// </summary>
		private float mWidth = 0f;
		/// <summary>
		/// Get/Set the reference width of the object.
		/// </summary>
		public float Width
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
		private float mX = 0f;
		/// <summary>
		/// Get/Set the reference X position of the object.
		/// </summary>
		public float X
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
		private float mY = 0f;
		/// <summary>
		/// Get/Set the reference Y position of the object.
		/// </summary>
		public float Y
		{
			get { return mY; }
			set { mY = value; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
