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
	//*	NameValueCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of NameValueItem Items.
	/// </summary>
	public class NameValueCollection : List<NameValueItem>
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
		public NameValueItem this[string name]
		{
			get
			{
				string lowerName = "";
				NameValueItem result = null;

				if(name?.Length > 0)
				{
					lowerName = name.ToLower();
					result = this.FirstOrDefault(x => x.Name.ToLower() == lowerName);
				}
				return result;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ParseSemi																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Parse a semi-colon delimited string of colon delimited names and values
		/// and return the result to the caller.
		/// </summary>
		/// <param name="semiColonDelimitedValue">
		/// Semi-colon delimited series of colon delimited names and values.
		/// </param>
		/// <returns>
		/// Reference to a newly created and populated NameValueCollection.
		/// </returns>
		public static NameValueCollection ParseSemi(string semiColonDelimitedValue)
		{
			char[] colon = new char[] { ':' };
			string[] entries = null;
			NameValueItem item = null;
			NameValueCollection result = new NameValueCollection();
			char[] semi = new char[] { ';' };
			string[] sides = null;

			if(semiColonDelimitedValue?.Length > 0)
			{
				entries = semiColonDelimitedValue.Split(semi,
					StringSplitOptions.RemoveEmptyEntries);
				foreach(string entryItem in entries)
				{
					sides = entryItem.Split(colon,
						StringSplitOptions.RemoveEmptyEntries);
					if(sides.Length > 0)
					{
						item = new NameValueItem()
						{
							Name = sides[0].Trim()
						};
						if(sides.Length > 1)
						{
							item.Value = sides[1].Trim();
						}
						result.Add(item);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SetValue																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the value of the property of the specified name, creating a new
		/// item in the collection, if necessary.
		/// </summary>
		/// <param name="name">
		/// Name of the property to set.
		/// </param>
		/// <param name="value">
		/// Value to place on the property.
		/// </param>
		public void SetValue(string name, string value)
		{
			string lowerName = "";
			NameValueItem item = null;

			if(name?.Length > 0)
			{
				lowerName = name.ToLower();
				item = this.FirstOrDefault(x => x.Name.ToLower() == lowerName);
				if(item == null)
				{
					item = new NameValueItem()
					{
						Name = name
					};
					this.Add(item);
				}
				if(value?.Length > 0)
				{
					item.Value = value;
				}
				else
				{
					item.Value = "";
				}	
			}

		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TransferValues																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Copy the member values of each item in the source collection to new
		/// items on the target collection.
		/// </summary>
		/// <param name="source">
		/// Reference to the source collection to copy.
		/// </param>
		/// <param name="target">
		/// Reference to the target collection to receive the new values.
		/// </param>
		public static void TransferValues(List<NameValueItem> source,
			List<NameValueItem> target)
		{
			if(source?.Count > 0 && target != null)
			{
				target.Clear();
				foreach(NameValueItem nameValueItem in source)
				{
					target.Add(new NameValueItem()
					{
						Name = nameValueItem.Name,
						Value = nameValueItem.Value
					});
				}
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	NameValueItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual name and value.
	/// </summary>
	public class NameValueItem
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
