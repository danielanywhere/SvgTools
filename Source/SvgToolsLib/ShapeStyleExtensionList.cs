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
		public ShapeStyleExtensionCollection Extensions
		{
			get { return mExtensions; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Remarks																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Remarks">Remarks</see>.
		/// </summary>
		private string mRemarks = "";
		/// <summary>
		/// Get/Set the remarks for this extension list.
		/// </summary>
		public string Remarks
		{
			get { return mRemarks; }
			set { mRemarks = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShapeNames																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ShapeNames">ShapeNames</see>.
		/// </summary>
		private List<string> mShapeNames = new List<string>();
		/// <summary>
		/// Get the list of shape names to which this list of extensions
		/// applies.
		/// </summary>
		public List<string> ShapeNames
		{
			get { return mShapeNames; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*


}
