using System;
using System.Collections.Generic;
using System.Text;

namespace SvgToolsLibrary
{
	//*-------------------------------------------------------------------------*
	//*	SvgActionOptionCollection																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgActionOptionItem Items.
	/// </summary>
	public class SvgActionOptionCollection : List<SvgActionOptionItem>
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
		/// Parse and add an option from its text value.
		/// </summary>
		/// <param name="optionText">
		/// Text to parse.
		/// </param>
		/// <returns>
		/// Newly created and added option.
		/// </returns>
		public SvgActionOptionItem Add(string optionText)
		{
			char[] comma = new char[] { ',' };
			string[] parts = null;
			SvgActionOptionItem result = new SvgActionOptionItem();


			if(optionText?.Length > 0)
			{
				//	Text has been provided.
				parts = optionText.Split(comma,
					StringSplitOptions.RemoveEmptyEntries);
				if(parts.Length > 0)
				{
					result.Name = parts[0].Trim();
					if(parts.Length > 1)
					{
						result.Value = parts[1].Trim();
					}
				}
			}
			this.Add(result);
			return result;
		}
		//*-----------------------------------------------------------------------*



	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	SvgActionOptionItem																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual SVG action option.
	/// </summary>
	public class SvgActionOptionItem
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
		/// Get/Set the name of the option.
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
		/// <summary>
		/// Private member for <see cref="Value">Value</see>.
		/// </summary>
		private string mValue = "";
		/// <summary>
		/// Get/Set the optional value of the option.
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
