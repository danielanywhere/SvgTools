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

using Html;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	SvgDocumentCollection																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgDocumentItem Items.
	/// </summary>
	public class SvgDocumentCollection : List<SvgDocumentItem>
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
	//*	SvgDocumentItem																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about the loaded SVG document.
	/// </summary>
	public class SvgDocumentItem
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
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new instance of the SvgDocumentItem item.
		/// </summary>
		public SvgDocumentItem()
		{
			Document = new HtmlDocument()
			{
				LineFeed = false,
				IncludeComments = true,
				PreserveSpace = true,
			};
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new instance of the SvgDocumentItem item.
		/// </summary>
		/// <param name="htmlDocument">
		/// HTML document with which to initialize the SVG document.
		/// </param>
		public SvgDocumentItem(HtmlDocument htmlDocument)
		{
			if(htmlDocument != null)
			{
				Document = htmlDocument;
			}
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new instance of the SvgDocumentItem item.
		/// </summary>
		/// <param name="htmlContent">
		/// HTML content with which to initialize the document.
		/// </param>
		public SvgDocumentItem(string htmlContent) : this()
		{
			if(htmlContent?.Length > 0)
			{
				mDocument.Html = htmlContent;
				mChanged = false;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Changed																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Changed">Changed</see>.
		/// </summary>
		private bool mChanged = false;
		/// <summary>
		/// Get/Set a value indicating whether data has changed on the current
		/// document.
		/// </summary>
		public bool Changed
		{
			get { return mChanged; }
			set { mChanged = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Document																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Document">Document</see>.
		/// </summary>
		private HtmlDocument mDocument = null;
		/// <summary>
		/// Get/Set a reference to the HTML document object of the SVG.
		/// </summary>
		public HtmlDocument Document
		{
			get { return mDocument; }
			set
			{
				mDocument = value;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	IsLocal																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="IsLocal">IsLocal</see>.
		/// </summary>
		private bool mIsLocal = true;
		/// <summary>
		/// Get/Set a value indicating whether this document was loaded at the
		/// local level (true) or a base level (false).
		/// </summary>
		public bool IsLocal
		{
			get { return mIsLocal; }
			set { mIsLocal = value; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
