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

using static SvgToolsLib.SvgToolsUtil;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	CssKeyframeCollection																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of CssKeyframeItem Items.
	/// </summary>
	public class CssKeyframeCollection : List<CssKeyframeItem>
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

			foreach(CssKeyframeItem keyframeItem in this)
			{
				text = keyframeItem.ToString();
				if(text.Length > 0)
				{
					builder.Append(text);
				}
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	CssKeyframeItem																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about the individual keyframe.
	/// </summary>
	public class CssKeyframeItem
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
		//*	Styles																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Styles">Styles</see>.
		/// </summary>
		private CssStyleCollection mStyles = new CssStyleCollection();
		/// <summary>
		/// Get a reference to the collection of styles on this keyframe.
		/// </summary>
		public CssStyleCollection Styles
		{
			get { return mStyles; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TemporalPosition																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="TemporalPosition">TemporalPosition</see>.
		/// </summary>
		private float mTemporalPosition = 0f;
		/// <summary>
		/// Get/Set the percentage of the defined total at which this keyframe
		/// activates.
		/// </summary>
		public float TemporalPosition
		{
			get { return mTemporalPosition; }
			set { mTemporalPosition = value; }
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
			StringBuilder builder = new StringBuilder();

			builder.AppendLine($"{mTemporalPosition:0.###}%");
			builder.AppendLine("{");
			builder.Append(mStyles.ToString());
			builder.AppendLine("}");
			return builder.ToString();
		}
	//*-----------------------------------------------------------------------*

}
	//*-------------------------------------------------------------------------*

}
