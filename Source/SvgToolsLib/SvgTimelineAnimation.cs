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

using Geometry;
using Html;
using SkiaSharp;
using Svg.Skia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using static SvgToolsLib.SvgToolsUtil;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	SvgTimelineAnimation																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Controller for freeform timeline-based animations on SVG.
	/// </summary>
	public static class SvgTimelineAnimation
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	ProcessDiscreteString																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Process the master frames for a linear floating point value
		/// interpolation property.
		/// </summary>
		/// <param name="node">
		/// Reference to the source HTML node item to process.
		/// </param>
		/// <param name="keyframes">
		/// Reference to the collection of keyframes to process.
		/// </param>
		/// <param name="masterFrames">
		/// Reference to the collection of master frames to be appended to.
		/// </param>
		/// <param name="objectName">
		/// Name of the object to resolve.
		/// </param>
		/// <param name="attributeName">
		/// Name of the object attribute to inspect.
		/// </param>
		/// <param name="frameField">
		/// Keyframe field type to resolve.
		/// </param>
		private static void ProcessDiscreteString(
			HtmlNodeItem node,
			List<SvgTimelineKeyframeItem> keyframes,
			SvgTimelineFrameCollection masterFrames,
			string objectName, string attributeName,
			SvgTimelineKeyframeFieldEnum frameField)
		{
			int count = 0;
			SvgTimelineKeyframeItem frame = null;
			int index = 0;
			SvgTimelineFrameItem masterFrame = null;
			SvgTimelineKeyframeItem matchingFrame = null;
			List<SvgTimelineKeyframeItem> matchingFrames = null;
			PropertyInfo property = null;
			string propertyName = frameField.ToString();
			string value = "";

			if(keyframes?.Count > 0 && masterFrames != null &&
				objectName?.Length > 0)
			{
				property = typeof(SvgTimelineKeyframeItem).GetProperty(propertyName);
				if(property != null)
				{
					matchingFrames = keyframes.FindAll(x =>
						x.ObjectName == objectName &&
						(x.ActiveFields & frameField) !=
						SvgTimelineKeyframeFieldEnum.None);
						//.OrderBy(y => property.GetValue(y, null)).ToList();
					count = matchingFrames.Count;
					if(count > 0)
					{
						for(index = 0; index < count; index++)
						{
							matchingFrame = matchingFrames[index];
							if(matchingFrame != null)
							{
								masterFrame = masterFrames.GetFrame(matchingFrame.FrameIndex);
								if(masterFrame != null)
								{
									frame = new SvgTimelineKeyframeItem()
									{
										ObjectName = objectName,
										FrameIndex = matchingFrame.FrameIndex,
										ActiveFields = frameField,
										Node = node
									};
									value = property.GetValue(matchingFrame, null).ToString();
									property.SetValue(frame, value);
									masterFrame.Entries.Add(frame);
								}
							}
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ProcessLinearFloat																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Process the master frames for a linear floating point value
		/// interpolation property.
		/// </summary>
		/// <param name="node">
		/// Reference to the source HTML node item to process.
		/// </param>
		/// <param name="keyframes">
		/// Reference to the collection of keyframes to process.
		/// </param>
		/// <param name="masterFrames">
		/// Reference to the collection of master frames to be appended to.
		/// </param>
		/// <param name="objectName">
		/// Name of the object to resolve.
		/// </param>
		/// <param name="attributeName">
		/// Name of the object attribute to inspect.
		/// </param>
		/// <param name="frameField">
		/// Keyframe field type to resolve.
		/// </param>
		private static void ProcessLinearFloat(
			HtmlNodeItem node,
			List<SvgTimelineKeyframeItem> keyframes,
			SvgTimelineFrameCollection masterFrames,
			string objectName, string attributeName,
			SvgTimelineKeyframeFieldEnum frameField)
		{
			string attributeValue = "";
			int count = 0;
			CssEasingTypeEnum easing = CssEasingTypeEnum.Linear;
			CssEasingTypeEnum easingTest = CssEasingTypeEnum.None;
			SvgTimelineKeyframeItem frame = null;
			int frameDifference = 0;
			int frameIndexCurrent = 0;
			int frameIndexNext = 0;
			int index = 0;
			SvgTimelineFrameItem masterFrame = null;
			SvgTimelineKeyframeItem matchingFrame = null;
			List<SvgTimelineKeyframeItem> matchingFrames = null;
			float progressPerFrame = 0f;
			float progressValue = 0f;
			PropertyInfo property = null;
			string propertyName = frameField.ToString();
			float valueEnd = 0f;
			float valueStart = 0f;

			if(keyframes?.Count > 0 && masterFrames != null &&
				objectName?.Length > 0)
			{
				property = typeof(SvgTimelineKeyframeItem).GetProperty(propertyName);
				if(property != null)
				{
					matchingFrames = keyframes.FindAll(x =>
						x.ObjectName == objectName &&
						(x.ActiveFields & frameField) !=
						SvgTimelineKeyframeFieldEnum.None);
						//.OrderBy(y => property.GetValue(y, null)).ToList();
					count = matchingFrames.Count;
					if(count > 0)
					{
						for(index = 0; index < count; index++)
						{
							//	Matching frame represents the first frame of the next set.
							matchingFrame = matchingFrames[index];
							if(((matchingFrame.ActiveFields &
								SvgTimelineKeyframeFieldEnum.Easing) !=
									SvgTimelineKeyframeFieldEnum.None) &&
								Enum.TryParse<CssEasingTypeEnum>(matchingFrame.Easing,
									true, out easingTest))
							{
								easing = easingTest;
							}
							if(index == 0)
							{
								//	The first value is always absolute.
								frameIndexCurrent = 1;
								attributeValue = GetAttributeValue(node, attributeName);
								if(attributeValue.Length > 0)
								{
									valueStart = ToFloat(attributeValue);
								}
								else
								{
									valueStart = ToFloat(
										property.GetValue(matchingFrame, null).ToString());
								}
								valueEnd = ToFloat(
									property.GetValue(matchingFrame, null).ToString());
							}
							else
							{
								frameIndexCurrent = frameIndexNext;
								valueStart = valueEnd;
								if(matchingFrame.Absolute)
								{
									valueEnd = ToFloat(
										property.GetValue(matchingFrame, null).ToString());
								}
								else
								{
									valueEnd += ToFloat(
										property.GetValue(matchingFrame, null).ToString());
								}
							}
							frameIndexNext = matchingFrame.FrameIndex;
							frameDifference = frameIndexNext - frameIndexCurrent - 1;
							if(frameDifference > 0f)
							{
								progressPerFrame = 1f / (float)frameDifference;
							}
							else
							{
								progressPerFrame = 0f;
							}
							for(progressValue = 0f;
								(index + 1 < count ?
									frameIndexCurrent < frameIndexNext :
									frameIndexCurrent <= frameIndexNext);
								frameIndexCurrent++, progressValue += progressPerFrame)
							{
								masterFrame = masterFrames.GetFrame(frameIndexCurrent);
								if(masterFrame != null)
								{
									frame = new SvgTimelineKeyframeItem()
									{
										ObjectName = objectName,
										FrameIndex = frameIndexCurrent,
										ActiveFields = frameField,
										Node = node
									};
									property.SetValue(frame, ToFloat(
										EaseInterpolate(valueStart, valueEnd, progressValue,
											easing)));
									masterFrame.Entries.Add(frame);
								}
							}
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ProcessLinearHexColor																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Process the master frames for a linear hex color interpolation
		/// property.
		/// </summary>
		/// <param name="node">
		/// Reference to the source HTML node item to process.
		/// </param>
		/// <param name="keyframes">
		/// Reference to the collection of keyframes to process.
		/// </param>
		/// <param name="masterFrames">
		/// Reference to the collection of master frames to be appended to.
		/// </param>
		/// <param name="objectName">
		/// Name of the object to resolve.
		/// </param>
		/// <param name="attributeName">
		/// Name of the object attribute to inspect.
		/// </param>
		/// <param name="frameField">
		/// Keyframe field type to resolve.
		/// </param>
		private static void ProcessLinearHexColor(
			HtmlNodeItem node,
			List<SvgTimelineKeyframeItem> keyframes,
			SvgTimelineFrameCollection masterFrames,
			string objectName, string attributeName,
			SvgTimelineKeyframeFieldEnum frameField)
		{
			string attributeValue = "";
			SKColor colorEnd = SKColor.Empty;
			SKColor colorStart = SKColor.Empty;
			int count = 0;
			CssEasingTypeEnum easing = CssEasingTypeEnum.Linear;
			CssEasingTypeEnum easingTest = CssEasingTypeEnum.None;
			SvgTimelineKeyframeItem frame = null;
			int frameDifference = 0;
			int frameIndexCurrent = 0;
			int frameIndexNext = 0;
			int index = 0;
			SvgTimelineFrameItem masterFrame = null;
			SvgTimelineKeyframeItem matchingFrame = null;
			List<SvgTimelineKeyframeItem> matchingFrames = null;
			float progressPerFrame = 0f;
			float progressValue = 0f;
			PropertyInfo property = null;
			string propertyName = frameField.ToString();

			if(keyframes?.Count > 0 && masterFrames != null &&
				objectName?.Length > 0)
			{
				property = typeof(SvgTimelineKeyframeItem).GetProperty(propertyName);
				if(property != null)
				{
					matchingFrames = keyframes.FindAll(x =>
						x.ObjectName == objectName &&
						(x.ActiveFields & frameField) !=
						SvgTimelineKeyframeFieldEnum.None);
						//.OrderBy(y => property.GetValue(y, null)).ToList();
					count = matchingFrames.Count;
					if(count > 0)
					{
						for(index = 0; index < count; index ++)
						{
							//	Matching frame represents the first frame of the next set.
							matchingFrame = matchingFrames[index];
							if(((matchingFrame.ActiveFields &
								SvgTimelineKeyframeFieldEnum.Easing) !=
									SvgTimelineKeyframeFieldEnum.None) &&
								Enum.TryParse<CssEasingTypeEnum>(matchingFrame.Easing,
									true, out easingTest))
							{
								easing = easingTest;
							}
							if(index == 0)
							{
								frameIndexCurrent = 1;
								attributeValue = GetAttributeValue(node, attributeName);
								if(attributeValue.Length > 0)
								{
									colorStart = ToSKColor(attributeValue);
								}
								else
								{
									colorStart = ToSKColor(
										property.GetValue(matchingFrame, null).ToString());
								}
							}
							else
							{
								frameIndexCurrent = frameIndexNext;
								colorStart = colorEnd;
							}
							frameIndexNext = matchingFrame.FrameIndex;
							frameDifference = frameIndexNext - frameIndexCurrent - 1;
							if(frameDifference > 0f)
							{
								progressPerFrame = 1f / (float)frameDifference;
							}
							else
							{
								progressPerFrame = 0f;
							}
							colorEnd = ToSKColor(
								property.GetValue(matchingFrame, null).ToString());
							for(progressValue = 0f;
								(index + 1 < count ?
									frameIndexCurrent < frameIndexNext :
									frameIndexCurrent <= frameIndexNext);
								frameIndexCurrent++, progressValue += progressPerFrame)
							{
								masterFrame = masterFrames.GetFrame(frameIndexCurrent);
								if(masterFrame != null)
								{
									frame = new SvgTimelineKeyframeItem()
									{
										ObjectName = objectName,
										FrameIndex = frameIndexCurrent,
										ActiveFields = frameField,
										Node = node
									};
									property.SetValue(frame, ToHexColor(
										EaseInterpolate(colorStart, colorEnd, progressValue,
											easing)));
									masterFrame.Entries.Add(frame);
								}
							}
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ProcessRotation																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Process rotation keyframes for the provided HTML node.
		/// </summary>
		/// <param name="node">
		/// Reference to the source HTML node item to process.
		/// </param>
		/// <param name="keyframes">
		/// Reference to the collection of keyframes to process.
		/// </param>
		/// <param name="masterFrames">
		/// Reference to the collection of master frames to be appended to.
		/// </param>
		/// <param name="objectName">
		/// Name of the object to resolve.
		/// </param>
		private static void ProcessRotation(HtmlNodeItem node,
			List<SvgTimelineKeyframeItem> keyframes,
			SvgTimelineFrameCollection masterFrames,
			string objectName)
		{
			string attributeValue = "";
			int count = 0;
			CssEasingTypeEnum easing = CssEasingTypeEnum.Linear;
			CssEasingTypeEnum easingTest = CssEasingTypeEnum.None;
			SvgTimelineKeyframeItem frame = null;
			int frameDifference = 0;
			int frameIndexCurrent = 0;
			int frameIndexNext = 0;
			int index = 0;
			SvgTimelineFrameItem masterFrame = null;
			SvgTimelineKeyframeItem matchingFrame = null;
			List<SvgTimelineKeyframeItem> matchingFrames = null;
			float progressPerFrame = 0f;
			float progressValue = 0f;
			TransformItem transform = null;
			TransformCollection transforms = null;
			float valueEnd = 0f;
			float valueStart = 0f;

			if(keyframes?.Count > 0 && masterFrames != null &&
				objectName?.Length > 0)
			{
				matchingFrames = keyframes.FindAll(x =>
					x.ObjectName == objectName &&
					(x.ActiveFields & SvgTimelineKeyframeFieldEnum.Rotation) !=
					SvgTimelineKeyframeFieldEnum.None);
				count = matchingFrames.Count;
				if(count > 0)
				{
					for(index = 0; index < count; index++)
					{
						//	Matching frame represents the first frame of the next set.
						matchingFrame = matchingFrames[index];
						if(((matchingFrame.ActiveFields &
							SvgTimelineKeyframeFieldEnum.Easing) !=
								SvgTimelineKeyframeFieldEnum.None) &&
							Enum.TryParse<CssEasingTypeEnum>(matchingFrame.Easing,
								true, out easingTest))
						{
							easing = easingTest;
						}
						if(index == 0)
						{
							frameIndexCurrent = 1;
							attributeValue = GetAttributeValue(node, "transform");
							transforms = TransformCollection.Parse(attributeValue);
							transform = transforms.FirstOrDefault(x =>
								x.TransformType == TransformTypeEnum.Rotate);
							if(transform?.Parameters.Count > 0)
							{
								valueStart = transform.Parameters[0];
							}
							else
							{
								valueStart = matchingFrame.Rotation;
							}
							valueEnd = matchingFrame.Rotation;
						}
						else
						{
							frameIndexCurrent = frameIndexNext;
							valueStart = valueEnd;
							if(matchingFrame.Absolute)
							{
								valueEnd = matchingFrame.Rotation;
							}
							else
							{
								valueEnd += matchingFrame.Rotation;
							}
						}
						frameIndexNext = matchingFrame.FrameIndex;
						frameDifference = frameIndexNext - frameIndexCurrent - 1;
						if(frameDifference > 0f)
						{
							progressPerFrame = 1f / (float)frameDifference;
						}
						else
						{
							progressPerFrame = 0f;
						}
						for(progressValue = 0f;
							(index + 1 < count ?
								frameIndexCurrent < frameIndexNext :
								frameIndexCurrent <= frameIndexNext);
							frameIndexCurrent++, progressValue += progressPerFrame)
						{
							masterFrame = masterFrames.GetFrame(frameIndexCurrent);
							if(masterFrame != null)
							{
								frame = new SvgTimelineKeyframeItem()
								{
									ObjectName = objectName,
									FrameIndex = frameIndexCurrent,
									ActiveFields = SvgTimelineKeyframeFieldEnum.Rotation,
									Rotation = ToFloat(
									EaseInterpolate(valueStart, valueEnd, progressValue,
										easing)),
									Node = node
								};
								masterFrame.Entries.Add(frame);
							}
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ProcessScaleX																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Process scale-x keyframes for the provided HTML node.
		/// </summary>
		/// <param name="node">
		/// Reference to the source HTML node item to process.
		/// </param>
		/// <param name="keyframes">
		/// Reference to the collection of keyframes to process.
		/// </param>
		/// <param name="masterFrames">
		/// Reference to the collection of master frames to be appended to.
		/// </param>
		/// <param name="objectName">
		/// Name of the object to resolve.
		/// </param>
		private static void ProcessScaleX(HtmlNodeItem node,
			List<SvgTimelineKeyframeItem> keyframes,
			SvgTimelineFrameCollection masterFrames,
			string objectName)
		{
			string attributeValue = "";
			int count = 0;
			CssEasingTypeEnum easing = CssEasingTypeEnum.Linear;
			CssEasingTypeEnum easingTest = CssEasingTypeEnum.None;
			SvgTimelineKeyframeItem frame = null;
			int frameDifference = 0;
			int frameIndexCurrent = 0;
			int frameIndexNext = 0;
			int index = 0;
			SvgTimelineFrameItem masterFrame = null;
			SvgTimelineKeyframeItem matchingFrame = null;
			List<SvgTimelineKeyframeItem> matchingFrames = null;
			float progressPerFrame = 0f;
			float progressValue = 0f;
			TransformItem transform = null;
			TransformCollection transforms = null;
			float valueEnd = 0f;
			float valueStart = 0f;

			if(keyframes?.Count > 0 && masterFrames != null &&
				objectName?.Length > 0)
			{
				matchingFrames = keyframes.FindAll(x =>
					x.ObjectName == objectName &&
					(x.ActiveFields & SvgTimelineKeyframeFieldEnum.ScaleX) !=
					SvgTimelineKeyframeFieldEnum.None);
				count = matchingFrames.Count;
				if(count > 0)
				{
					for(index = 0; index < count; index++)
					{
						//	Matching frame represents the first frame of the next set.
						matchingFrame = matchingFrames[index];
						if(((matchingFrame.ActiveFields &
							SvgTimelineKeyframeFieldEnum.Easing) !=
								SvgTimelineKeyframeFieldEnum.None) &&
							Enum.TryParse<CssEasingTypeEnum>(matchingFrame.Easing,
								true, out easingTest))
						{
							easing = easingTest;
						}
						if(index == 0)
						{
							frameIndexCurrent = 1;
							attributeValue = GetAttributeValue(node, "transform");
							transforms = TransformCollection.Parse(attributeValue);
							transform = transforms.FirstOrDefault(x =>
								x.TransformType == TransformTypeEnum.Scale);
							if(transform?.Parameters.Count > 0)
							{
								valueStart = transform.Parameters[0];
							}
							else
							{
								valueStart = matchingFrame.ScaleX;
							}
							valueEnd = matchingFrame.ScaleX;
						}
						else
						{
							frameIndexCurrent = frameIndexNext;
							valueStart = valueEnd;
							if(matchingFrame.Absolute)
							{
								valueEnd = matchingFrame.ScaleX;
							}
							else
							{
								valueEnd += matchingFrame.ScaleX;
							}
						}
						frameIndexNext = matchingFrame.FrameIndex;
						frameDifference = frameIndexNext - frameIndexCurrent - 1;
						if(frameDifference > 0f)
						{
							progressPerFrame = 1f / (float)frameDifference;
						}
						else
						{
							progressPerFrame = 0f;
						}
						for(progressValue = 0f;
							(index + 1 < count ?
								frameIndexCurrent < frameIndexNext :
								frameIndexCurrent <= frameIndexNext);
							frameIndexCurrent++, progressValue += progressPerFrame)
						{
							masterFrame = masterFrames.GetFrame(frameIndexCurrent);
							if(masterFrame != null)
							{
								frame = new SvgTimelineKeyframeItem()
								{
									ObjectName = objectName,
									FrameIndex = frameIndexCurrent,
									ActiveFields = SvgTimelineKeyframeFieldEnum.ScaleX,
									ScaleX = ToFloat(
									EaseInterpolate(valueStart, valueEnd, progressValue,
										easing)),
									Node = node
								};
								masterFrame.Entries.Add(frame);
							}
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ProcessScaleY																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Process scale-y keyframes for the provided HTML node.
		/// </summary>
		/// <param name="node">
		/// Reference to the source HTML node item to process.
		/// </param>
		/// <param name="keyframes">
		/// Reference to the collection of keyframes to process.
		/// </param>
		/// <param name="masterFrames">
		/// Reference to the collection of master frames to be appended to.
		/// </param>
		/// <param name="objectName">
		/// Name of the object to resolve.
		/// </param>
		private static void ProcessScaleY(HtmlNodeItem node,
			List<SvgTimelineKeyframeItem> keyframes,
			SvgTimelineFrameCollection masterFrames,
			string objectName)
		{
			string attributeValue = "";
			int count = 0;
			CssEasingTypeEnum easing = CssEasingTypeEnum.Linear;
			CssEasingTypeEnum easingTest = CssEasingTypeEnum.None;
			SvgTimelineKeyframeItem frame = null;
			int frameDifference = 0;
			int frameIndexCurrent = 0;
			int frameIndexNext = 0;
			int index = 0;
			SvgTimelineFrameItem masterFrame = null;
			SvgTimelineKeyframeItem matchingFrame = null;
			List<SvgTimelineKeyframeItem> matchingFrames = null;
			float progressPerFrame = 0f;
			float progressValue = 0f;
			TransformItem transform = null;
			TransformCollection transforms = null;
			float valueEnd = 0f;
			float valueStart = 0f;

			if(keyframes?.Count > 0 && masterFrames != null &&
				objectName?.Length > 0)
			{
				matchingFrames = keyframes.FindAll(x =>
					x.ObjectName == objectName &&
					(x.ActiveFields & SvgTimelineKeyframeFieldEnum.ScaleY) !=
					SvgTimelineKeyframeFieldEnum.None);
				count = matchingFrames.Count;
				if(count > 0)
				{
					for(index = 0; index < count; index++)
					{
						//	Matching frame represents the first frame of the next set.
						matchingFrame = matchingFrames[index];
						if(((matchingFrame.ActiveFields &
							SvgTimelineKeyframeFieldEnum.Easing) !=
								SvgTimelineKeyframeFieldEnum.None) &&
							Enum.TryParse<CssEasingTypeEnum>(matchingFrame.Easing,
								true, out easingTest))
						{
							easing = easingTest;
						}
						if(index == 0)
						{
							frameIndexCurrent = 1;
							attributeValue = GetAttributeValue(node, "transform");
							transforms = TransformCollection.Parse(attributeValue);
							transform = transforms.FirstOrDefault(x =>
								x.TransformType == TransformTypeEnum.Scale);
							if(transform?.Parameters.Count > 1)
							{
								valueStart = transform.Parameters[1];
							}
							else
							{
								valueStart = matchingFrame.ScaleY;
							}
							valueEnd = matchingFrame.ScaleY;
						}
						else
						{
							frameIndexCurrent = frameIndexNext;
							valueStart = valueEnd;
							if(matchingFrame.Absolute)
							{
								valueEnd = matchingFrame.ScaleY;
							}
							else
							{
								valueEnd += matchingFrame.ScaleY;
							}
						}
						frameIndexNext = matchingFrame.FrameIndex;
						frameDifference = frameIndexNext - frameIndexCurrent - 1;
						if(frameDifference > 0f)
						{
							progressPerFrame = 1f / (float)frameDifference;
						}
						else
						{
							progressPerFrame = 0f;
						}
						for(progressValue = 0f;
							(index + 1 < count ?
								frameIndexCurrent < frameIndexNext :
								frameIndexCurrent <= frameIndexNext);
							frameIndexCurrent++, progressValue += progressPerFrame)
						{
							masterFrame = masterFrames.GetFrame(frameIndexCurrent);
							if(masterFrame != null)
							{
								frame = new SvgTimelineKeyframeItem()
								{
									ObjectName = objectName,
									FrameIndex = frameIndexCurrent,
									ActiveFields = SvgTimelineKeyframeFieldEnum.ScaleY,
									ScaleY = ToFloat(
									EaseInterpolate(valueStart, valueEnd, progressValue,
										easing)),
									Node = node
								};
								masterFrame.Entries.Add(frame);
							}
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetCommonProperties																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set common properties for entries on each master frame.
		/// </summary>
		/// <param name="keyframes">
		/// Reference to the collection of timeline keyframes.
		/// </param>
		/// <param name="masterFrames">
		/// Reference to the collection of master keyframes from which each output
		/// frame will be rendered.
		/// </param>
		private static void SetCommonProperties(
			List<SvgTimelineKeyframeItem> keyframes,
			SvgTimelineFrameCollection masterFrames)
		{
			List<SvgTimelineKeyframeItem> matchingKeyframes = null;

			if(keyframes?.Count > 0 && masterFrames?.Count > 0)
			{
				foreach(SvgTimelineFrameItem frameItem in masterFrames)
				{
					matchingKeyframes = keyframes.FindAll(x =>
						x.FrameIndex == frameItem.FrameIndex);
					foreach(SvgTimelineKeyframeItem keyframeItem in matchingKeyframes)
					{
						frameItem.ApplyCommonValues(keyframeItem);
					}
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
		//* GenerateAnimationCss																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Generate CSS-based animation for the objects in the provided SVG
		/// document and the freeform timeline in the supplied data set.
		/// </summary>
		/// <param name="doc">
		/// Reference to the SVG document being inspected.
		/// </param>
		/// <param name="data">
		/// Reference to the dataset containing freeform timeline keyframes.
		/// </param>
		/// <param name="tableName">
		/// Optional. Name of the table to load.
		/// </param>
		/// <returns>
		/// A string containing raw CSS generated from the supplied keyframes, if
		/// found. Otherwise, an empty string.
		/// </returns>
		public static string GenerateAnimationCss(HtmlDocument doc, DataSet data,
			string tableName = "")
		{
			CssKeyframeAnimationItem animation = null;
			CssKeyframeAnimationSetItem animationSet = null;
			CssKeyframeAnimationSetCollection animationSets = null;
			StringBuilder builder = new StringBuilder();
			string css = "";
			//SvgTimelineKeyframeGroupItem group = null;
			List<string> groupNames = null;
			//SvgTimelineKeyframeGroupCollection groups = null;
			SvgTimelineKeyframeCollection keyframes = null;
			SvgTimelineKeyframeCollection keyframeSet = null;
			HtmlNodeItem node = null;
			List<string> objectNames = null;
			SvgTimelineNodeItem timelineNode = null;

			if(doc != null && data?.Tables.Count > 0)
			{
				animationSets = new CssKeyframeAnimationSetCollection();
				//groups = new SvgTimelineKeyframeGroupCollection();
				keyframeSet = new SvgTimelineKeyframeCollection();
				if(string.IsNullOrEmpty(tableName) ||
					!data.Tables.Contains(tableName))
				{
					keyframes =
						new SvgTimelineKeyframeCollection(data.Tables[0]);
				}
				else
				{
					keyframes =
						new SvgTimelineKeyframeCollection(data.Tables[tableName]);
				}
				groupNames = keyframes
					.Select(k => k.GroupName)
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.ToList();
				foreach(string groupNameItem in groupNames)
				{
					//group = new SvgTimelineKeyframeGroupItem();
					//group.Name = groupNameItem;

					keyframeSet.Clear();
					keyframeSet.AddRange(
						keyframes.FindAll(k =>
							StringComparer.OrdinalIgnoreCase
								.Equals(k.GroupName, groupNameItem)));
					//	Resolve all of the entries in this timeline.
					objectNames = keyframeSet
						.Select(k => k.ObjectName)
						.Distinct(StringComparer.OrdinalIgnoreCase)
						.ToList();
					foreach(string objectNameItem in objectNames)
					{
						node = doc.Nodes.FindMatch(node =>
							StringComparer.OrdinalIgnoreCase
								.Equals(node.Id, objectNameItem));
						if(node != null)
						{
							animationSet = animationSets.FirstOrDefault(x =>
								StringComparer.OrdinalIgnoreCase
									.Equals(x.Selector, $"#{objectNameItem}"));
							if(animationSet == null)
							{
								animationSet = new CssKeyframeAnimationSetItem()
								{
									Selector = $"#{objectNameItem}"
								};
								animationSets.Add(animationSet);
							}
							animation = new CssKeyframeAnimationItem(node);
							animation.Name = $"{objectNameItem}-" +
									Right(Guid.NewGuid().ToString("N"), 8);
							CssKeyframeAnimationItem.ProcessTimelineKeyframes(
								animation,
								keyframeSet.FindAll(k =>
									StringComparer.OrdinalIgnoreCase
										.Equals(k.ObjectName, objectNameItem)));
							animationSet.Animations.Add(animation);
						}
						else
						{
							Trace.WriteLine(
								$"SVG object id not found for: {objectNameItem}",
								$"{MessageImportanceEnum.Warn}");
						}
					}
				}
				if(animationSets.Count > 0)
				{
					builder.Append(animationSets.ToString());
				}
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GenerateAnimationFrames																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Generate frame-based animation for the objects in the provided SVG
		/// document and the freeform timeline in the supplied data set.
		/// </summary>
		/// <param name="doc">
		/// Reference to the SVG document being inspected.
		/// </param>
		/// <param name="sourceDirectory">
		/// Reference to the base source directory from which linked images and
		/// resources can be found.
		/// </param>
		/// <param name="targetDirectory">
		/// Reference to the target directory where the images will be stored.
		/// </param>
		/// <param name="filenameTemplate">
		/// The template filename for output frames.
		/// </param>
		/// <param name="data">
		/// Reference to the dataset containing freeform timeline keyframes.
		/// </param>
		/// <param name="tableName">
		/// Optional. Name of the table to load.
		/// </param>
		/// <param name="frameRate">
		/// The number of frames to generate per second.
		/// </param>
		/// <remarks>
		/// <para>
		/// If a static filename with no patterns is given, the output name will
		/// include that name with a dash and numeric suffix, as in
		/// 'OutputFilename-0001.png'.
		/// </para>
		/// <para>
		/// Because curly braces are reserved for real time variables, keyword
		/// patterns are specified in square braces, as in
		/// 'BaseName-[Patterns].png', where multiple pattern sections can be
		/// specified in a single filename template, as in
		/// 'BaseName-[yyyyMMdd]-[Index:4].png'. The following patterns are
		/// recognized.
		/// </para>
		/// <list type="bullet">
		/// <item>'-' - Literal dash.</item>
		/// <item>'_' - Literal underline.</item>
		/// <item>'.' - Literal period.</item>
		/// <item>'~' - Literal tilde.</item>
		/// <item>Index - Index of the output file with no padding.</item>
		/// <item>Index:n - Index of the output file within the collection,
		/// left-padded with 'n' zeros.</item>
		/// <item>AP - Single upper case 'A' or 'P' meridiem.</item>
		/// <item>AMPM - Double upper case 'AM' or 'PM' meridiem.</item>
		/// <item>ap - Single lower case 'a' or 'p' meridiem.</item>
		/// <item>ampm - Double lower case 'am' or 'pm' meridiem.</item>
		/// <item>d - Day of month with no padding.</item>
		/// <item>dd - Two digit day of month.</item>
		/// <item>f:n - Fraction of a second padded to 'n' places.</item>
		/// <item>H - 24-hour base hour number with no padding.</item>
		/// <item>HH - Two digit base hour number.</item>
		/// <item>h - 12-hour base hour number with no padding.</item>
		/// <item>hh - Two digit 12-hour base hour number.</item>
		/// <item>M - Month number with no padding.</item>
		/// <item>MM - Two digit month.</item>
		/// <item>m - Minute with no padding.</item>
		/// <item>mm - Two digit minute.</item>
		/// <item>s - Seconds with no padding.</item>
		/// <item>ss - Two digit seconds.</item>
		/// <item>yy - Two digit year.</item>
		/// <item>yyyy - Four digit year.</item>
		/// </list>
		/// </remarks>
		public static void GenerateAnimationFrames(HtmlDocument doc,
			DirectoryInfo sourceDirectory,
			DirectoryInfo targetDirectory,
			string filenameTemplate, DataSet data, string tableName = "",
			float frameRate = 24f)
		{
			string attributeValue = "";
			StringBuilder builder = new StringBuilder();
			SKColor colorCurrent = SKColor.Empty;
			SKColor colorNext = SKColor.Empty;
			SKColor colorParsed = SKColor.Empty;
			int count = 0;
			string css = "";
			NameValueCollection filenameVariables = new NameValueCollection();
			int frameDifference = 0;
			int frameIndexCurrent = 0;
			int frameIndexNext = 0;
			List<string> groupNames = null;
			int index = 0;
			int indexCurrent = 0;
			int indexNext = 0;
			List<SvgTimelineKeyframeItem> keyframes = null;
			SvgTimelineFrameItem masterFrame = null;
			SvgTimelineFrameCollection masterFrames = null;
			int maxIndex = 0;
			HtmlNodeItem node = null;
			List<string> objectNames = null;
			float originX = 0f;
			float originY = 0f;
			string outputPath = "";
			float paramValue = 0f;
			int prevIndex = 0;
			float progressPerFrame = 0f;
			float progressValue = 0f;
			//FArea rectangle = null;
			SKSvg svg = null;
			SvgTimelineNodeItem timelineNode = null;
			TransformItem transform = null;
			TransformCollection transforms = null;

			if(doc != null && data?.Tables.Count > 0 && frameRate != 0f &&
				targetDirectory != null && filenameTemplate?.Length > 0)
			{
				AssureFolder(targetDirectory.FullName, true);
				if(string.IsNullOrEmpty(tableName) ||
					!data.Tables.Contains(tableName))
				{
					keyframes =
						new SvgTimelineKeyframeCollection(data.Tables[0]);
				}
				else
				{
					keyframes =
						new SvgTimelineKeyframeCollection(data.Tables[tableName]);
				}


				// While GenerateAnimationCss is object-centric,
				// GenerateAnimationFrames is frame-centeric. In this method,
				// the times at which actions occur occupy the main processing
				// list. At each timing tick, all affected objects are adjusted
				// and a frame is created.

				//	Configure the timing.
				//	Every entry receives a frame index.
				prevIndex = 1;
				foreach(SvgTimelineKeyframeItem keyframeItem in keyframes)
				{
					if((keyframeItem.ActiveFields &
						SvgTimelineKeyframeFieldEnum.Seconds) ==
						SvgTimelineKeyframeFieldEnum.None &&
						keyframeItem.FrameIndex > 0)
					{
						//	The frame index overrides an empty linear time.
						keyframeItem.Seconds = keyframeItem.FrameIndex / frameRate;
						prevIndex = keyframeItem.FrameIndex;
					}
					else if(keyframeItem.FrameIndex == -1 &&
						(keyframeItem.ActiveFields &
						SvgTimelineKeyframeFieldEnum.Seconds) !=
						SvgTimelineKeyframeFieldEnum.None)
					{
						//	Keyframe was not specified, but time was specified.
						keyframeItem.FrameIndex =
							(int)(keyframeItem.Seconds * frameRate);
						prevIndex = keyframeItem.FrameIndex;
					}
					else if((keyframeItem.ActiveFields &
						SvgTimelineKeyframeFieldEnum.FrameIndex) ==
						SvgTimelineKeyframeFieldEnum.None)
					{
						//	Neither frame index nor time were specified.
						keyframeItem.FrameIndex = prevIndex;
					}
				}
				keyframes = keyframes
					.OrderBy(x => x.FrameIndex)
					.ThenBy(y => y.ObjectName)
					.ToList();
				maxIndex = keyframes.Max(x => x.FrameIndex);

				masterFrames = new SvgTimelineFrameCollection();
				for(index = 1; index <= maxIndex; index++)
				{
					masterFrames.Add(new SvgTimelineFrameItem()
					{
						FrameIndex = index
					});
				}

				//	* Interpolate Every Linear Object *
				objectNames = keyframes.Select(k => k.ObjectName)
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.ToList();
				foreach(string objectNameItem in objectNames)
				{
					node = doc.Nodes.FindMatch(x =>
						string.Equals(x.Id, objectNameItem,
							StringComparison.OrdinalIgnoreCase));
					if(node != null)
					{
						SetCommonProperties(keyframes, masterFrames);
						ProcessLinearHexColor(node, keyframes, masterFrames,
							objectNameItem, "fill",
							SvgTimelineKeyframeFieldEnum.BackColor);
						ProcessLinearHexColor(node, keyframes, masterFrames,
							objectNameItem, "stroke",
							SvgTimelineKeyframeFieldEnum.ForeColor);
						ProcessLinearFloat(node, keyframes, masterFrames,
							objectNameItem, "height",
							SvgTimelineKeyframeFieldEnum.Height);
						ProcessLinearFloat(node, keyframes, masterFrames,
							objectNameItem, "width",
							SvgTimelineKeyframeFieldEnum.Width);
						ProcessLinearFloat(node, keyframes, masterFrames,
							objectNameItem, "x",
							SvgTimelineKeyframeFieldEnum.X);
						ProcessLinearFloat(node, keyframes, masterFrames,
							objectNameItem, "y",
							SvgTimelineKeyframeFieldEnum.Y);
						ProcessLinearFloat(node, keyframes, masterFrames,
							objectNameItem, "opacity",
							SvgTimelineKeyframeFieldEnum.Opacity);
						ProcessRotation(node, keyframes, masterFrames, objectNameItem);
						ProcessScaleX(node, keyframes, masterFrames, objectNameItem);
						ProcessScaleY(node, keyframes, masterFrames, objectNameItem);
						ProcessDiscreteString(node, keyframes, masterFrames,
							objectNameItem, "display",
							SvgTimelineKeyframeFieldEnum.Display);
					}
				}

				//	Apply changes and render output for every master frame.
				if(targetDirectory != null)
				{
					targetDirectory = AssureFolder(targetDirectory.FullName, true);
				}
				if(targetDirectory != null)
				{
					if(masterFrames.Count > 0)
					{
						foreach(SvgTimelineFrameItem masterFrameItem in masterFrames)
						{
							//	Each frame.
							foreach(SvgTimelineKeyframeItem keyframeItem in
								masterFrameItem.Entries)
							{
								node = keyframeItem.Node;
								switch(keyframeItem.ActiveFields)
								{
									case SvgTimelineKeyframeFieldEnum.Absolute:
									case SvgTimelineKeyframeFieldEnum.Easing:
									case SvgTimelineKeyframeFieldEnum.FrameIndex:
									case SvgTimelineKeyframeFieldEnum.GroupName:
									case SvgTimelineKeyframeFieldEnum.Index:
									case SvgTimelineKeyframeFieldEnum.None:
									case SvgTimelineKeyframeFieldEnum.ObjectName:
									case SvgTimelineKeyframeFieldEnum.OriginX:
									case SvgTimelineKeyframeFieldEnum.OriginY:
									case SvgTimelineKeyframeFieldEnum.Remark:
									case SvgTimelineKeyframeFieldEnum.Repeat:
									case SvgTimelineKeyframeFieldEnum.Seconds:
										//	Control and reference fields.
										break;
									case SvgTimelineKeyframeFieldEnum.BackColor:
										SetAttributeValue(node, "fill", keyframeItem.BackColor);
										break;
									case SvgTimelineKeyframeFieldEnum.Display:
										SetAttributeValue(node, "display",
											(ToBool(keyframeItem.Display) ? "inline" : "none"));
										break;
									case SvgTimelineKeyframeFieldEnum.ForeColor:
										SetAttributeValue(node, "stroke", keyframeItem.ForeColor);
										break;
									case SvgTimelineKeyframeFieldEnum.Height:
										SetAttributeValue(node, "height", keyframeItem.Height);
										break;
									case SvgTimelineKeyframeFieldEnum.Opacity:
										SetAttributeValue(node, "opacity",
											$"{(keyframeItem.Opacity * 100f):0}%");
										break;
									case SvgTimelineKeyframeFieldEnum.Width:
										SetAttributeValue(node, "width", keyframeItem.Width);
										break;
									case SvgTimelineKeyframeFieldEnum.X:
										SetAttributeValue(node, "x", keyframeItem.X);
										break;
									case SvgTimelineKeyframeFieldEnum.Y:
										SetAttributeValue(node, "y", keyframeItem.Y);
										break;
									case SvgTimelineKeyframeFieldEnum.Rotation:
										transform = null;
										originX = masterFrames.GetCommonFloatValue(
											masterFrameItem.FrameIndex,
											keyframeItem.ObjectName,
											"OriginX");
										originY = masterFrames.GetCommonFloatValue(
											masterFrameItem.FrameIndex,
											keyframeItem.ObjectName,
											"OriginY");
										attributeValue = GetAttributeValue(node, "transform");
										if(attributeValue.Length > 0)
										{
											transforms = TransformCollection.Parse(attributeValue);
											transform = transforms.FirstOrDefault(x =>
												x.TransformType == TransformTypeEnum.Rotate);
										}
										else
										{
											transforms = new TransformCollection();
										}
										if(transform == null)
										{
											transform = new TransformItem()
											{
												TransformType = TransformTypeEnum.Rotate
											};
											transform.Parameters.Add(keyframeItem.Rotation);
											transforms.Add(transform);
										}
										else
										{
											transform.Parameters[0] = keyframeItem.Rotation;
										}
										//	Set the pivot point for the shape.
										//	Currently, this is being done in SetAttributeValue
										//	through the following attributes.
										//	transform-box: fill-box;
										//	transform-origin: center;
										if(transform.Parameters.Count > 1)
										{
											transform.Parameters[1] = originX;
										}
										else
										{
											transform.Parameters.Add(originX);
										}
										if(transform.Parameters.Count > 2)
										{
											transform.Parameters[2] = originY;
										}
										else
										{
											transform.Parameters.Add(originY);
										}
										SetAttributeValue(node, "transform", transforms.ToString());
										//Trace.WriteLine(
										//	$"{keyframeItem.FrameIndex}. " +
										//	$"Rotation: {keyframeItem.Rotation}, " +
										//	$"Style: {HtmlAttributeCollection.GetAttributeValue(node, "style")}",
										//	$" {MessageImportanceEnum.Info}");
										break;
									case SvgTimelineKeyframeFieldEnum.ScaleX:
										//if(keyframeItem.ScaleX > 1f)
										//{
										//	Trace.WriteLine("SvgTimelineAnimation.GenerateAnimationFrames.ScaleX: Break here...");
										//}
										transform = null;
										attributeValue = GetAttributeValue(node, "transform");
										if(attributeValue.Length > 0)
										{
											transforms = TransformCollection.Parse(attributeValue);
											transform = transforms.FirstOrDefault(x =>
												x.TransformType == TransformTypeEnum.Scale);
										}
										else
										{
											transforms = new TransformCollection();
										}
										if(transform == null)
										{
											transform = new TransformItem()
											{
												TransformType = TransformTypeEnum.Scale
											};
											transform.Parameters.Add(keyframeItem.ScaleX);
											transform.Parameters.Add(1f);
											transforms.Add(transform);
										}
										else
										{
											transform.Parameters[0] = keyframeItem.ScaleX;
										}
										SetAttributeValue(node, "transform", transforms.ToString());
										break;
									case SvgTimelineKeyframeFieldEnum.ScaleY:
										//if(keyframeItem.ScaleX > 1f)
										//{
										//	Trace.WriteLine("SvgTimelineAnimation.GenerateAnimationFrames.ScaleY: Break here...");
										//}
										transform = null;
										attributeValue = GetAttributeValue(node, "transform");
										if(attributeValue.Length > 0)
										{
											transforms = TransformCollection.Parse(attributeValue);
											transform = transforms.FirstOrDefault(x =>
												x.TransformType == TransformTypeEnum.Scale);
										}
										else
										{
											transforms = new TransformCollection();
										}
										if(transform == null)
										{
											transform = new TransformItem()
											{
												TransformType = TransformTypeEnum.Scale
											};
											transform.Parameters.Add(1f);
											transform.Parameters.Add(keyframeItem.ScaleY);
											transforms.Add(transform);
										}
										else
										{
											transform.Parameters[1] = keyframeItem.ScaleY;
										}
										SetAttributeValue(node, "transform", transforms.ToString());
										break;
									default:
										//	Unrecognized field.
										Trace.WriteLine(
											$" Unrecognized field: {keyframeItem.ActiveFields}",
											$" {MessageImportanceEnum.Warn}");
										break;
								}
							}
							filenameVariables.SetValue("Index",
								masterFrameItem.FrameIndex.ToString());
							outputPath = Path.Combine(targetDirectory.FullName,
								GetExtrapolatedString(filenameTemplate, filenameVariables));
							try
							{
								using(FileStream stream = File.OpenWrite(outputPath))
								{
									svg = SKSvg.CreateFromSvg(doc.Html);
									svg.Picture.ToImage(stream, SKColors.Empty,
										SKEncodedImageFormat.Png, 100, 1f, 1f,
										SKColorType.Bgra8888, SKAlphaType.Unpremul,
										SKColorSpace.CreateSrgb());
								}
							}
							catch(Exception ex)
							{
								Trace.WriteLine(ex.Message,
									$" {MessageImportanceEnum.Err}");
							}
						}
					}
					else
					{
						Trace.WriteLine($"No frames were generated.",
							$" {MessageImportanceEnum.Warn}");
					}
				}
				else
				{
					Trace.WriteLine($"Target directory not found: {targetDirectory}",
						$" {MessageImportanceEnum.Err}");
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetObjectNames																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a list of object names found in the provided timeline animation
		/// data.
		/// </summary>
		/// <param name="data">
		/// Reference to the dataset containing freeform timeline keyframes.
		/// </param>
		/// <param name="tableName">
		/// Optional. Name of the table to load.
		/// </param>
		/// <returns>
		/// Distinct list of object names listed in the supplied animation data.
		/// </returns>
		public static List<string> GetObjectNames(DataSet data,
			string tableName = "")
		{
			SvgTimelineKeyframeCollection keyframes = null;
			List<string> names = null;

			if(data?.Tables.Count > 0)
			{
				if(string.IsNullOrEmpty(tableName) ||
					!data.Tables.Contains(tableName))
				{
					keyframes =
						new SvgTimelineKeyframeCollection(data.Tables[0]);
				}
				else
				{
					keyframes =
						new SvgTimelineKeyframeCollection(data.Tables[tableName]);
				}
				names = keyframes
					.Select(k => k.ObjectName)
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.ToList();
			}
			if(names == null)
			{
				names = new List<string>();
			}
			return names;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
