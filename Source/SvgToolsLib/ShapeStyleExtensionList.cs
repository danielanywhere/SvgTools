using System;
using System.Collections.Generic;
using System.Text;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	ShapeStyleExtensionListCollection																				*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ShapeStyleExtensionListItem Items.
	/// </summary>
	public class ShapeStyleExtensionListCollection :
		List<ShapeStyleExtensionListItem>
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
	//*	ShapeStyleExtensionListItem																							*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about a set of shape style expansions.
	/// </summary>
	public class ShapeStyleExtensionListItem
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
		//*	Extensions																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Extensions">Extensions</see>.
		/// </summary>
		private ShapeStyleExtensionCollection mExtensions =
			new ShapeStyleExtensionCollection();
		/// <summary>
		/// Get a reference to the collection of extensions in this list.
		/// </summary>
		public ShapeStyleExtensionCollection Extensions
		{
			get { return mExtensions; }
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
		/// Get/Set the remarks for this extension list.
		/// </summary>
		public string Remarks
		{
			get { return mRemarks; }
			set { mRemarks = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShapeNames																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ShapeNames">ShapeNames</see>.
		/// </summary>
		private List<string> mShapeNames = new List<string>();
		/// <summary>
		/// Get the list of shape names to which this list of extensions
		/// applies.
		/// </summary>
		public List<string> ShapeNames
		{
			get { return mShapeNames; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*


}
