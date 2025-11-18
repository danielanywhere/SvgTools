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
using Newtonsoft.Json.Converters;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	ShapeStyleExtensionCollection																						*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ShapeStyleExtensionItem Items.
	/// </summary>
	public class ShapeStyleExtensionCollection : List<ShapeStyleExtensionItem>
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
	//*	ShapeStyleExtensionItem																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about a shape style extension.
	/// </summary>
	public class ShapeStyleExtensionItem
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
		//*	ExtensionType																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ExtensionType">ExtensionType</see>.
		/// </summary>
		private ShapeStyleExtensionType mExtensionType =
			ShapeStyleExtensionType.None;
		/// <summary>
		/// Get/Set the extension type defined by this entry.
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		[JsonProperty(Order = 1)]
		public ShapeStyleExtensionType ExtensionType
		{
			get { return mExtensionType; }
			set { mExtensionType = value; }
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
		/// Get/Set the remarks associated with this item.
		/// </summary>
		[JsonProperty(Order = 0)]
		public string Remarks
		{
			get { return mRemarks; }
			set { mRemarks = value; }
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
		/// Get/Set the selector for this item.
		/// </summary>
		[JsonProperty(Order = 2)]
		public string Selector
		{
			get { return mSelector; }
			set { mSelector = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Settings																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Settings">Settings</see>.
		/// </summary>
		private NameValueNodesCollection mSettings =
			new NameValueNodesCollection();
		/// <summary>
		/// Get a reference to the collection of settings for this extension.
		/// </summary>
		[JsonProperty(Order = 3)]
		public NameValueNodesCollection Settings
		{
			get { return mSettings; }
			set { mSettings = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*


}
