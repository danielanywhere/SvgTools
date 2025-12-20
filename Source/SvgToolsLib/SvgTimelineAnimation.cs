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
using System.Data;
using System.Linq;
using System.Text;

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
