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

using Newtonsoft.Json;

namespace SvgToolsLibrary
{
	//*-------------------------------------------------------------------------*
	//*	ConditionCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ConditionItem Items.
	/// </summary>
	public class ConditionCollection : List<ConditionItem>
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
	//*	ConditionItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual condition and assignment.
	/// </summary>
	public class ConditionItem
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
		//*	Assignment																														*
		//*-----------------------------------------------------------------------*
		private string mAssignment = "";
		/// <summary>
		/// Get/Set the assignment.
		/// </summary>
		[JsonProperty(Order = 1)]
		public string Assignment
		{
			get { return mAssignment; }
			set { mAssignment = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Condition																															*
		//*-----------------------------------------------------------------------*
		private string mCondition = "";
		/// <summary>
		/// Get/Set the condition.
		/// </summary>
		[JsonProperty(Order = 0)]
		public string Condition
		{
			get { return mCondition; }
			set { mCondition = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
