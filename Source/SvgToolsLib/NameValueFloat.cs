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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using static SvgToolsLib.SvgToolsUtil;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	NameValueFloatCollection																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of NameValueFloatItem Items.
	/// </summary>
	public class NameValueFloatCollection : List<NameValueFloatItem>
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
		/// name, if found. Otherwise, null.
		/// </summary>
		/// <param name="name">
		/// Name of the property to find.
		/// </param>
		public NameValueFloatItem this[string name]
		{
			get
			{
				NameValueFloatItem result = null;

				if(name?.Length > 0)
				{
					result = this.FirstOrDefault(x =>
						string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
				}
				return result;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetValue																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the named property.
		/// </summary>
		/// <param name="name">
		/// Name of the property to retrieve.
		/// </param>
		/// <returns>
		/// Value of the specified property, if found. Otherwise, 0.
		/// </returns>
		public float GetValue(string name)
		{
			NameValueFloatItem item = null;
			float result = 0f;

			if(name?.Length > 0)
			{
				item = this.FirstOrDefault(x => string.Equals(x.Name, name,
					StringComparison.OrdinalIgnoreCase));
				if(item != null)
				{
					result = item.Value;
				}
			}
			return result;
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
		public static NameValueFloatCollection ParseSemi(
			string semiColonDelimitedValue)
		{
			char[] colon = new char[] { ':' };
			string[] entries = null;
			NameValueFloatItem item = null;
			NameValueFloatCollection result = new NameValueFloatCollection();
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
						item = new NameValueFloatItem()
						{
							Name = sides[0].Trim()
						};
						if(sides.Length > 1)
						{
							item.Value = ToFloat(sides[1].Trim());
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
		public void SetValue(string name, float value)
		{
			NameValueFloatItem item = null;

			if(name?.Length > 0)
			{
				item = this.FirstOrDefault(x => string.Equals(x.Name, name,
					StringComparison.OrdinalIgnoreCase));
				if(item == null)
				{
					item = new NameValueFloatItem()
					{
						Name = name
					};
					this.Add(item);
				}
				item.Value = value;
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
		public static void TransferValues(List<NameValueFloatItem> source,
			List<NameValueFloatItem> target)
		{
			if(source?.Count > 0 && target != null)
			{
				target.Clear();
				foreach(NameValueFloatItem nameValueItem in source)
				{
					target.Add(new NameValueFloatItem()
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
	//*	NameValueFloatItem																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// An individual name and float value entry.
	/// </summary>
	public class NameValueFloatItem
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
			string value = $"{mValue:0.###}";

			return $"{name}:{value}";
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Value																																	*
		//*-----------------------------------------------------------------------*
		private float mValue = 0f;
		/// <summary>
		/// Get/Set the value of the item.
		/// </summary>
		[JsonProperty(Order = 1)]
		public float Value
		{
			get { return mValue; }
			set { mValue = value; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
