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
using System.Threading.Tasks;

namespace SvgToolsLibrary
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
		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Value																																	*
		//*-----------------------------------------------------------------------*
		private string mValue = "";
		/// <summary>
		/// Get/Set the value of the item.
		/// </summary>
		public string Value
		{
			get { return mValue; }
			set { mValue = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
