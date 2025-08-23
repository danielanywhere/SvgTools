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

namespace SvgToolsLibrary
{
	//*-------------------------------------------------------------------------*
	//*	SequenceCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SequenceItem Items.
	/// </summary>
	public class SequenceCollection : List<SequenceItem>
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
	//*	SequenceItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual sequence.
	/// </summary>
	public class SequenceItem
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
		//*	Actions																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Actions">Actions</see>.
		/// </summary>
		private SvgActionCollection mActions = new SvgActionCollection();
		/// <summary>
		/// Get a reference to the collection of actions on this sequence.
		/// </summary>
		public SvgActionCollection Actions
		{
			get { return mActions; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SequenceName																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="SequenceName">SequenceName</see>.
		/// </summary>
		private string mSequenceName = "";
		/// <summary>
		/// Get/Set the name of the sequence.
		/// </summary>
		public string SequenceName
		{
			get { return mSequenceName; }
			set { mSequenceName = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
