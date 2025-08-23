using System;
using System.Collections.Generic;
using System.Text;

using Html;

namespace SvgToolsLibrary
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
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new instance of the SvgDocumentItem item.
		/// </summary>
		/// <param name="htmlContent">
		/// HTML content with which to initialize the document.
		/// </param>
		public SvgDocumentItem(string htmlContent)
		{
			if(htmlContent?.Length > 0)
			{
				mDocument.Html = htmlContent;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Document																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Document">Document</see>.
		/// </summary>
		private HtmlDocument mDocument = new HtmlDocument()
		{
			LineFeed = false,
			IncludeComments = true,
			PreserveSpace = true,
		};
		/// <summary>
		/// Get/Set a reference to the HTML document object of the SVG.
		/// </summary>
		public HtmlDocument Document
		{
			get { return mDocument; }
			set { mDocument = value; }
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
