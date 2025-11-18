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

using Newtonsoft.Json;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	NameValueNodesCollection																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of NameValueNodesItem Items.
	/// </summary>
	public class NameValueNodesCollection : List<NameValueNodesItem>
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
		//*	_Indexer																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the first matching property with the specified case-insensitive
		/// name.
		/// </summary>
		/// <param name="name">
		/// Name of the property to find.
		/// </param>
		public NameValueNodesItem this[string name]
		{
			get
			{
				string lowerName = "";
				NameValueNodesItem result = null;

				if(name?.Length > 0)
				{
					lowerName = name.ToLower();
					result = this.FirstOrDefault(x => x.Name.ToLower() == lowerName);
				}
				return result;
			}
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////* ParseSemi																															*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Parse a semi-colon delimited string of colon delimited names and values
		///// and return the result to the caller.
		///// </summary>
		///// <param name="semiColonDelimitedValue">
		///// Semi-colon delimited series of colon delimited names and values.
		///// </param>
		///// <returns>
		///// Reference to a newly created and populated NameValueCollection.
		///// </returns>
		//public static NameValueNodesCollection ParseSemi(
		//	string semiColonDelimitedValue)
		//{
		//	char[] colon = new char[] { ':' };
		//	string[] entries = null;
		//	NameValueNodesItem item = null;
		//	NameValueNodesCollection result = new NameValueNodesCollection();
		//	char[] semi = new char[] { ';' };
		//	string[] sides = null;

		//	if(semiColonDelimitedValue?.Length > 0)
		//	{
		//		entries = semiColonDelimitedValue.Split(semi,
		//			StringSplitOptions.RemoveEmptyEntries);
		//		foreach(string entryItem in entries)
		//		{
		//			sides = entryItem.Split(colon,
		//				StringSplitOptions.RemoveEmptyEntries);
		//			if(sides.Length > 0)
		//			{
		//				item = new NameValueNodesItem()
		//				{
		//					Name = sides[0].Trim()
		//				};
		//				if(sides.Length > 1)
		//				{
		//					item.Value = sides[1].Trim();
		//				}
		//				result.Add(item);
		//			}
		//		}
		//	}
		//	return result;
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	SetValue																															*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Set the value of the property of the specified name, creating a new
		///// item in the collection, if necessary.
		///// </summary>
		///// <param name="name">
		///// Name of the property to set.
		///// </param>
		///// <param name="value">
		///// Value to place on the property.
		///// </param>
		//public void SetValue(string name, string value)
		//{
		//	string lowerName = "";
		//	NameValueNodesItem item = null;

		//	if(name?.Length > 0)
		//	{
		//		lowerName = name.ToLower();
		//		item = this.FirstOrDefault(x => x.Name.ToLower() == lowerName);
		//		if(item == null)
		//		{
		//			item = new NameValueNodesItem()
		//			{
		//				Name = name
		//			};
		//			this.Add(item);
		//		}
		//		if(value?.Length > 0)
		//		{
		//			item.Value = value;
		//		}
		//		else
		//		{
		//			item.Value = "";
		//		}
		//	}

		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////* TransferValues																												*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Copy the member values of each item in the source collection to new
		///// items on the target collection.
		///// </summary>
		///// <param name="source">
		///// Reference to the source collection to copy.
		///// </param>
		///// <param name="target">
		///// Reference to the target collection to receive the new values.
		///// </param>
		//public static void TransferValues(List<NameValueNodesItem> source,
		//	List<NameValueNodesItem> target)
		//{
		//	if(source?.Count > 0 && target != null)
		//	{
		//		target.Clear();
		//		foreach(NameValueNodesItem nameValueItem in source)
		//		{
		//			target.Add(new NameValueNodesItem()
		//			{
		//				Name = nameValueItem.Name,
		//				Value = nameValueItem.Value
		//			});
		//		}
		//	}
		//}
		////*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	NameValueNodesItem																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual name and value with nodes.
	/// </summary>
	public class NameValueNodesItem
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
		//*	Name																																	*
		//*-----------------------------------------------------------------------*
		private string mName = "";
		/// <summary>
		/// Get/Set the name of the item.
		/// </summary>
		[JsonProperty(Order = 0)]
		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Nodes																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Nodes">Nodes</see>.
		/// </summary>
		private NameValueNodesCollection mNodes = new NameValueNodesCollection();
		/// <summary>
		/// Get a reference to the collection of nodes on this item.
		/// </summary>
		[JsonProperty(Order = 3)]
		public NameValueNodesCollection Nodes
		{
			get { return mNodes; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Properties																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Properties">Properties</see>.
		/// </summary>
		private NameValueCollection mProperties = new NameValueCollection();
		/// <summary>
		/// Get a reference to the collection of immediate properties for this
		/// item.
		/// </summary>
		[JsonProperty(Order = 2)]
		public NameValueCollection Properties
		{
			get { return mProperties; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of this item.
		/// </summary>
		/// <returns>
		/// The string representation of this item.
		/// </returns>
		public override string ToString()
		{
			string name = (mName?.Length > 0 ? mName : "(empty)");
			string value = (mValue?.Length > 0 ? mValue : "(empty)");

			return $"{name}:{value}";
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Value																																	*
		//*-----------------------------------------------------------------------*
		private string mValue = "";
		/// <summary>
		/// Get/Set the value of the item.
		/// </summary>
		[JsonProperty(Order = 1)]
		public string Value
		{
			get { return mValue; }
			set { mValue = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
