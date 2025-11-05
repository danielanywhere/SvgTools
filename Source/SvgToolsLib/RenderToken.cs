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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	RenderTokenCollection																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of RenderTokenItem Items.
	/// </summary>
	public class RenderTokenCollection : List<RenderTokenItem>
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
	//*	RenderTokenItem																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual render token.
	/// </summary>
	public class RenderTokenItem
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
		//*	DeepCopy																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a deep member-values copy of the supplied token.
		/// </summary>
		/// <param name="token">
		/// Reference to the token to be copied.
		/// </param>
		/// <returns>
		/// Reference to a new token object with matching memberwise values to
		/// the provided source, if legitimate. Otherwise, null.
		/// </returns>
		public static RenderTokenItem DeepCopy(RenderTokenItem token)
		{
			string content = "";
			RenderTokenItem result = null;

			if(token != null)
			{
				content = JsonConvert.SerializeObject(token);
				try
				{
					result = JsonConvert.DeserializeObject<RenderTokenItem>(content);
				}
				catch { }
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	DeepCopyWithRemove																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a deep member-values copy of the supplied token, removing any
		/// of the specified properties in the process.
		/// </summary>
		/// <param name="token">
		/// Reference to the token to be copied.
		/// </param>
		/// <param name="removeProperties">
		/// Optional array of property names to remove during the cloning process.
		/// </param>
		/// <returns>
		/// Reference to a new token object with matching memberwise values to
		/// the provided source, if legitimate. Otherwise, null.
		/// </returns>
		public static RenderTokenItem DeepCopyWithRemove(RenderTokenItem token,
			params string[] removeProperties)
		{
			string content = "";
			string lowerName = "";
			RenderTokenItem result = null;

			if(token != null)
			{
				content = JsonConvert.SerializeObject(token);
				try
				{
					result = JsonConvert.DeserializeObject<RenderTokenItem>(content);
					if(removeProperties?.Length > 0)
					{
						foreach(string propertyNameItem in removeProperties)
						{
							lowerName = propertyNameItem.ToLower();
							result.mProperties.RemoveAll(x => x.Name.ToLower() == lowerName);
						}
					}
				}
				catch { }
			}
			return result;
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
		/// Get a reference to the collection of name/value pairs to use in this
		/// instance.
		/// </summary>
		public NameValueCollection Properties
		{
			get { return mProperties; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
