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

using Html;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	SvgTools																																*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// SVG Tools main controller.
	/// </summary>
	public class SvgTools
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
		//*	Document																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Document">Document</see>.
		/// </summary>
		private HtmlDocument mDocument = new HtmlDocument();
		/// <summary>
		/// Get/Set a reference to the active SVG document.
		/// </summary>
		public HtmlDocument Document
		{
			get { return mDocument; }
			set { mDocument = value; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*


}
