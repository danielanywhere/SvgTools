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
	//*	CssStyleCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of CssStyleItem Items.
	/// </summary>
	public class CssStyleCollection : List<CssStyleItem>
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
		//*	Add																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add a new style to the collection by member values.
		/// </summary>
		/// <param name="name">
		/// Name of the style to add.
		/// </param>
		/// <param name="value">
		/// The value of the new style.
		/// </param>
		/// <returns>
		/// Reference to the newly created and added style.
		/// </returns>
		public CssStyleItem Add(string name, string value)
		{
			CssStyleItem result = new CssStyleItem();

			if(name?.Length > 0)
			{
				result.Name = name;
			}
			if(value?.Length > 0)
			{
				result.Value = value;
			}
			this.Add(result);
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of the contents of this collection.
		/// </summary>
		/// <returns>
		/// String representation of this collection.
		/// </returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			string text = "";

			foreach(CssStyleItem styleItem in this)
			{
				text = styleItem.ToString();
				if(text.Length > 0)
				{
					builder.AppendLine(text);
				}
			}
			return builder.ToString();
		}
	//*-----------------------------------------------------------------------*


}
//*-------------------------------------------------------------------------*

//*-------------------------------------------------------------------------*
//*	CssStyleItem																														*
//*-------------------------------------------------------------------------*
/// <summary>
/// Name and value settings for a CSS style.
/// </summary>
public class CssStyleItem
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
		/// <summary>
		/// Private member for <see cref="Name">Name</see>.
		/// </summary>
		private string mName = "";
		/// <summary>
		/// Get/Set the name of the style.
		/// </summary>
		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of this item.
		/// </summary>
		/// <returns>
		/// String representation of this item.
		/// </returns>
		public override string ToString()
		{
			return (mName?.Length > 0 && mValue?.Length > 0 ?
				$"{mName}: {mValue};" : "");
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Value																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Value">Value</see>.
		/// </summary>
		private string mValue = "";
		/// <summary>
		/// Get/Set the value of the style.
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
