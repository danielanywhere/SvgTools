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

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	CssKeyframeAnimationSetCollection																				*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of CssKeyframeAnimationSetItem Items.
	/// </summary>
	public class CssKeyframeAnimationSetCollection :
		List<CssKeyframeAnimationSetItem>
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
		//*	ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of this collection.
		/// </summary>
		/// <returns>
		/// The string representation of this collection.
		/// </returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			string text = "";

			foreach(CssKeyframeAnimationSetItem setItem in this)
			{
				text = setItem.ToString();
				if(text.Length > 0)
				{
					if(builder.Length > 0)
					{
						builder.AppendLine();
					}
					builder.Append(text);
				}
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*



	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	CssKeyframeAnimationSetItem																							*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual CSS-style keyframe animation set.
	/// </summary>
	public class CssKeyframeAnimationSetItem
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
		//*	Animations																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Animations">Animations</see>.
		/// </summary>
		private CssKeyframeAnimationCollection mAnimations =
			new CssKeyframeAnimationCollection();
		/// <summary>
		/// Get a reference to the collection of animations on this set.
		/// </summary>
		public CssKeyframeAnimationCollection Animations
		{
			get { return mAnimations; }
			set { mAnimations = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Selector																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Selector">Selector</see>.
		/// </summary>
		private string mSelector = "";
		/// <summary>
		/// Get/Set the selector for this set.
		/// </summary>
		public string Selector
		{
			get { return mSelector; }
			set { mSelector = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of this item.
		/// </summary>
		/// <returns>
		/// The string representation of this item.
		/// </returns>
		public override string ToString()
		{
			CssKeyframeAnimationItem animation = null;
			string animationCss = "";
			StringBuilder builder = new StringBuilder();
			string originX = "center";
			string originY = "center";

			if(mSelector?.Length > 0 && mAnimations.Count > 0)
			{
				animationCss = mAnimations.ToString();
				if(animationCss.Length > 0)
				{
					animation = mAnimations.FirstOrDefault(x => x.OriginX.Length > 0);
					if(animation != null)
					{
						originX = animation.OriginX;
					}
					animation = mAnimations.FirstOrDefault(y => y.OriginY.Length > 0);
					if(animation != null)
					{
						originY = animation.OriginY;
					}
					builder.AppendLine(mSelector);
					builder.AppendLine("{");
					builder.AppendLine(animationCss);
					builder.AppendLine("transform-box: fill-box;");
					builder.Append("transform-origin: ");
					builder.AppendLine($"{originX} {originY}");
					builder.AppendLine("}");
					builder.AppendLine();
					builder.Append(mAnimations.KeyframeCss());
				}
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
