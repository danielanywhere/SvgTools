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

using Newtonsoft.Json;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	ShapeStyleExtensionListCollection																				*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ShapeStyleExtensionListItem Items.
	/// </summary>
	public class ShapeStyleExtensionListCollection :
		List<ShapeStyleExtensionListItem>
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
	//*	ShapeStyleExtensionListItem																							*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about a set of shape style expansions.
	/// </summary>
	public class ShapeStyleExtensionListItem
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
		//*	Comment																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Comment">Comment</see>.
		/// </summary>
		private string mComment = "";
		/// <summary>
		/// Get/Set the remarks for this extension list.
		/// </summary>
		[JsonProperty(Order = 0)]
		public string Comment
		{
			get { return mComment; }
			set { mComment = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Extensions																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Extensions">Extensions</see>.
		/// </summary>
		private ShapeStyleExtensionCollection mExtensions =
			new ShapeStyleExtensionCollection();
		/// <summary>
		/// Get a reference to the collection of extensions in this list.
		/// </summary>
		[JsonProperty(Order = 3)]
		public ShapeStyleExtensionCollection Extensions
		{
			get { return mExtensions; }
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	Selector																															*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Private member for <see cref="Selector">Selector</see>.
		///// </summary>
		//private string mSelector = "";
		///// <summary>
		///// Get/Set the selector for this item.
		///// </summary>
		//[JsonProperty(Order = 2)]
		//public string Selector
		//{
		//	get { return mSelector; }
		//	set { mSelector = value; }
		//}
		////*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Selectors																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Selectors">Selectors</see>.
		/// </summary>
		private List<string> mSelectors = new List<string>();
		/// <summary>
		/// Get the list of selectors to which this list of extensions
		/// applies.
		/// </summary>
		[JsonProperty(Order = 1)]
		public List<string> Selectors
		{
			get { return mSelectors; }
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	ShapeNames																														*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Private member for <see cref="ShapeNames">ShapeNames</see>.
		///// </summary>
		//private List<string> mShapeNames = new List<string>();
		///// <summary>
		///// Get the list of shape names to which this list of extensions
		///// applies.
		///// </summary>
		//[JsonProperty(Order = 1)]
		//public List<string> ShapeNames
		//{
		//	get { return mShapeNames; }
		//}
		////*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*


}
