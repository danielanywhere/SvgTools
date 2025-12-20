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

using Html;
using static SvgToolsLib.SvgToolsUtil;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	CssKeyframeAnimationCollection																					*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of CssKeyframeAnimationItem Items.
	/// </summary>
	public class CssKeyframeAnimationCollection : List<CssKeyframeAnimationItem>
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
		//* KeyframeCss																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the CSS text for the keyframe lists found in this collection.
		/// </summary>
		/// <returns>
		/// CSS keyframe text.
		/// </returns>
		public string KeyframeCss()
		{
			StringBuilder builder = new StringBuilder();
			string keyframeText = "";

			foreach(CssKeyframeAnimationItem animationItem in this)
			{
				keyframeText = animationItem.KeyframeCss();
				if(keyframeText.Length > 0)
				{
					builder.Append("@keyframes ");
					builder.AppendLine(animationItem.Name);
					builder.AppendLine("{");
					builder.AppendLine(keyframeText);
					builder.AppendLine("}");
				}
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of the animations in this collection.
		/// </summary>
		/// <returns>
		/// The string representation of this collection.
		/// </returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			foreach(CssKeyframeAnimationItem animationItem in this)
			{
				if(animationItem.Name?.Length > 0)
				{
					if(builder.Length > 0)
					{
						builder.Append(", ");
					}
					builder.Append(animationItem.Name);
					builder.Append(' ');
					builder.Append($"{animationItem.Duration:0.###}s");
					builder.Append(' ');
					builder.Append((animationItem.TimingFunction?.Length > 0 ?
						animationItem.TimingFunction : "linear"));
					builder.Append(' ');
					builder.Append($"{animationItem.Delay:0.###}s");
					builder.Append(' ');
					builder.Append($"{animationItem.IterationCount}");
					builder.Append(' ');
					builder.Append(SvgToolsUtil.ToCss(animationItem.Direction));
					builder.Append(' ');
					builder.Append(SvgToolsUtil.ToCss(animationItem.FillMode));
				}
			}
			if(builder.Length > 0)
			{
				builder.Insert(0, "animation: ");
				builder.Append(';');
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	CssKeyframeAnimationItem																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// A self-contained CSS-style keyframe animation definition.
	/// </summary>
	public class CssKeyframeAnimationItem
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
		/// Create a new instance of the CssKeyframeAnimationItem item.
		/// </summary>
		public CssKeyframeAnimationItem()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new instance of the CssKeyframeAnimationItem item.
		/// </summary>
		/// <param name="node">
		/// Reference to an HTML node that will be used as reference for
		/// calculating relative values.
		/// </param>
		public CssKeyframeAnimationItem(HtmlNodeItem node)
		{
			if(node != null)
			{
				mName = node.Id;
				mWidth = GetWidth(node);
				mHeight = GetHeight(node);
				mX = GetX(node);
				mY = GetY(node);
				mNode = node;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Delay																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Delay">Delay</see>.
		/// </summary>
		private float mDelay = 0f;
		/// <summary>
		/// Get/Set the delay prior to the start of the animation, in seconds.
		/// </summary>
		public float Delay
		{
			get { return mDelay; }
			set { mDelay = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Direction																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Direction">Direction</see>.
		/// </summary>
		private CssAnimationDirectionEnum mDirection =
			CssAnimationDirectionEnum.Normal;
		/// <summary>
		/// Get/Set the direction of travel on this animation.
		/// </summary>
		public CssAnimationDirectionEnum Direction
		{
			get { return mDirection; }
			set { mDirection = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Duration																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Duration">Duration</see>.
		/// </summary>
		private float mDuration = 0f;
		/// <summary>
		/// Get/Set the duration of the animation, in seconds.
		/// </summary>
		public float Duration
		{
			get { return mDuration; }
			set { mDuration = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FillMode																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="FillMode">FillMode</see>.
		/// </summary>
		private CssAnimationFillModeEnum mFillMode =
			CssAnimationFillModeEnum.Forwards;
		/// <summary>
		/// Get/Set the animation fill mode.
		/// </summary>
		public CssAnimationFillModeEnum FillMode
		{
			get { return mFillMode; }
			set { mFillMode = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* KeyframeCss																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the CSS text of the keyframe entries in this animation.
		/// </summary>
		/// <returns>
		/// CSS keyframe content for the keyframes in this animation.
		/// </returns>
		public string KeyframeCss()
		{
			StringBuilder builder = new StringBuilder();

			foreach(CssKeyframeItem keyframeItem in mKeyframes)
			{
				builder.AppendLine($"{keyframeItem.TemporalPosition:0.###}%");
				builder.AppendLine("{");
				builder.Append(keyframeItem.Styles.ToString());
				builder.AppendLine("}");
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
		//*	IterationCount																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="IterationCount">IterationCount</see>.
		/// </summary>
		private int mIterationCount = 1;
		/// <summary>
		/// Get/Set the count of iterations to be made for this animation.
		/// </summary>
		/// <remarks>
		/// <list type="bullet">
		/// <item>-1. Infinite.</item>
		/// <item>0. This animation is skipped.</item>
		/// <item>> 0. The count of items to run before stopping.</item>
		/// </list>
		/// </remarks>
		public int IterationCount
		{
			get { return mIterationCount; }
			set { mIterationCount = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Keyframes																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Keyframes">Keyframes</see>.
		/// </summary>
		private CssKeyframeCollection mKeyframes = new CssKeyframeCollection();
		/// <summary>
		/// Get a reference to the keyframe collection on this animation.
		/// </summary>
		public CssKeyframeCollection Keyframes
		{
			get { return mKeyframes; }
			set { mKeyframes = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Selector																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Name">Name</see>.
		/// </summary>
		private string mName = "";
		/// <summary>
		/// Get/Set the name of this animation sequence.
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
		//*	OriginX																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="OriginX">OriginX</see>.
		/// </summary>
		private string mOriginX = "center";
		/// <summary>
		/// Get/Set the horizontal transform origin for this animation.
		/// </summary>
		public string OriginX
		{
			get { return mOriginX; }
			set { mOriginX = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	OriginY																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="OriginY">OriginY</see>.
		/// </summary>
		private string mOriginY = "center";
		/// <summary>
		/// Get/Set the vertical transform origin for this animation.
		/// </summary>
		public string OriginY
		{
			get { return mOriginY; }
			set { mOriginY = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ProcessTimelineKeyframes																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Process timeline-style keyframes and generate CSS-style animation
		/// information and keyframes.
		/// </summary>
		/// <param name="animation">
		/// Reference to the CSS keyframe animation entry to be configured.
		/// </param>
		/// <param name="tKeyframes">
		/// Reference to the collection of source timeline keyframes.
		/// </param>
		public static void ProcessTimelineKeyframes(
			CssKeyframeAnimationItem animation,
			List<SvgTimelineKeyframeItem> tKeyframes)
		{
			CssKeyframeItem cKeyframe = null;
			float duration = 0f;
			string frameName = "";
			SvgTimelineKeyframeItem keyframe = null;
			List<SvgTimelineKeyframeItem> keyframes = null;
			float maxTime = 0f;
			string measurement = "";
			float minTime = 0f;
			float number = 0f;
			float percent = 0f;
			string repeat = "";
			List<float> times = null;
			StringBuilder transform = new StringBuilder();

			if(animation != null && tKeyframes?.Count > 0)
			{
				//	Configure the static values.
				minTime = tKeyframes.Min(n => n.Seconds);
				maxTime = tKeyframes.Max(n => n.Seconds);
				duration = maxTime - minTime;
				animation.mDelay = minTime;
				animation.mDuration = duration;
				//	TODO: Fill mode will be set in the next version of timeline.
				animation.mFillMode = CssAnimationFillModeEnum.Forwards;
				keyframe = tKeyframes.FirstOrDefault(n =>
					n.Easing.Length > 0);
				if(keyframe != null)
				{
					animation.mTimingFunction = keyframe.Easing;
				}
				keyframe = tKeyframes.FirstOrDefault(n =>
					n.Repeat.Length > 0);
				if(keyframe != null)
				{
					repeat = keyframe.Repeat;
				}
				switch(repeat.ToLower())
				{
					case "infinite":
						animation.mIterationCount = -1;
						break;
					case "none":
						animation.mIterationCount = 0;
						break;
					default:
						if(IsNumeric(repeat))
						{
							animation.mIterationCount = ToInt(repeat);
						}
						break;
				}
				//	TODO: ScaleXOrigin will be named OriginX in next version.
				keyframe = tKeyframes.FirstOrDefault(n =>
					n.ScaleXOrigin.Length > 0);
				if(keyframe != null)
				{
					animation.mOriginX = keyframe.ScaleXOrigin;
				}
				//	TODO: ScaleYOrigin will be named OriginY in next version.
				keyframe = tKeyframes.FirstOrDefault(n =>
					n.ScaleYOrigin.Length > 0);
				if(keyframe != null)
				{
					animation.mOriginY = keyframe.ScaleYOrigin;
				}
				times = tKeyframes.Select(k => k.Seconds).Distinct().ToList();
				times.Sort();
				foreach(float timeItem in times)
				{
					Clear(transform);
					percent = ((timeItem - minTime) / duration) * 100f;
					cKeyframe = animation.mKeyframes.FirstOrDefault(x =>
						x.TemporalPosition == percent);
					if(cKeyframe == null)
					{
						cKeyframe = new CssKeyframeItem()
						{
							TemporalPosition = percent
						};
						animation.mKeyframes.Add(cKeyframe);
					}
					keyframes = tKeyframes.FindAll(k => k.Seconds == timeItem);
					foreach(SvgTimelineKeyframeItem keyframeItem in keyframes)
					{
						if(keyframeItem.ActiveFields != SvgTimelineKeyframeFieldEnum.None)
						{
							//	Back color.
							if((keyframeItem.ActiveFields &
								SvgTimelineKeyframeFieldEnum.BackColor) != 0)
							{
								cKeyframe.Styles.Add("fill", keyframeItem.BackColor);
							}
							//	Fore color.
							if((keyframeItem.ActiveFields &
								SvgTimelineKeyframeFieldEnum.ForeColor) != 0)
							{
								cKeyframe.Styles.Add("stroke", keyframeItem.ForeColor);
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
									number -= animation.mX;
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
								SvgTimelineKeyframeFieldEnum.Y) != 0)
							{
								measurement = keyframeItem.Y;
								if(keyframeItem.Absolute)
								{
									measurement = MeasurementWithUnit(measurement);
									number = ToPixels(measurement);
									number -= animation.mY;
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
									if(animation.mWidth != 0f)
									{
										number /= animation.mWidth;
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
									if(animation.mHeight != 0f)
									{
										number /= animation.mHeight;
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
								cKeyframe.Styles.Add("opacity",
									$"{keyframeItem.Opacity:0.##}");
							}
						}
					}
					if(transform.Length > 0)
					{
						cKeyframe.Styles.Add("transform", transform.ToString());
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TimingFunction																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="TimingFunction">TimingFunction</see>.
		/// </summary>
		private string mTimingFunction = "linear";
		/// <summary>
		/// Get/Set the animation timing function for this animation.
		/// </summary>
		public string TimingFunction
		{
			get { return mTimingFunction; }
			set { mTimingFunction = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of this item.
		/// </summary>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			if(mName?.Length > 0)
			{
				builder.Append("animation: ");
				builder.Append(mName);
				builder.Append(' ');
				builder.Append($"{mDuration:0.###}s");
				builder.Append(' ');
				builder.Append((mTimingFunction?.Length > 0 ?
					mTimingFunction : "linear"));
				builder.Append(' ');
				builder.Append($"{mDelay:0.###}s");
				builder.Append(' ');
				builder.Append($"{mIterationCount}");
				builder.Append(' ');
				builder.Append(ToCss(mDirection));
				builder.Append(' ');
				builder.Append(ToCss(mFillMode));
				builder.Append(';');
			}

			return builder.ToString();
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
