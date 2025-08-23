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
using System.Text.RegularExpressions;

using static SvgToolsLibrary.SvgToolsUtil;

namespace SvgToolsLibrary
{
	//*-------------------------------------------------------------------------*
	//*	PlotPointsCollection																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of PlotPointsItem Items.
	/// </summary>
	public class PlotPointsCollection : List<PlotPointsItem>
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
		//* ParamIsDimension																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified parameter index is
		/// a dimension.
		/// </summary>
		/// <param name="action">
		/// The action to estimate.
		/// </param>
		/// <param name="paramIndex">
		/// The 0-based index of the parameter to rate.
		/// </param>
		/// <returns>
		/// True if the specified parameter of the plot item was a dimension.
		/// Otherwise, false.
		/// </returns>
		public static bool ParamIsDimension(string action, int paramIndex)
		{
			string la = "";
			bool result = false;

			if(action?.Length > 0)
			{
				la = action.ToLower();
				if(la == "a")
				{
					switch(paramIndex)
					{
						case 0:
						case 1:
							//	rx, ry
							result = true;
							break;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ParamIsHorizontal																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified parameter index is
		/// horizontal.
		/// </summary>
		/// <param name="action">
		/// The action to estimate.
		/// </param>
		/// <param name="paramIndex">
		/// The 0-based index of the parameter to rate.
		/// </param>
		/// <returns>
		/// True if the specified parameter of the plot item was horizontal.
		/// Otherwise, false.
		/// </returns>
		public static bool ParamIsHorizontal(string action, int paramIndex)
		{
			string la = "";
			bool result = false;

			if(action?.Length > 0)
			{
				la = action.ToLower();
				switch(la)
				{
					case "a":
						switch(paramIndex)
						{
							case 0:
							case 5:
								//	rx, ex
								result = true;
								break;
						}
						break;
					case "c":
						switch(paramIndex)
						{
							case 0:
							case 2:
							case 4:
								//	x1, x2, ex
								result = true;
								break;
						}
						break;
					case "h":
						//	x
						result = true;
						break;
					case "l":
						//	x
						if(paramIndex == 0)
						{
							result = true;
						}
						break;
					case "m":
						//	x
						if(paramIndex == 0)
						{
							result = true;
						}
						break;
					case "q":
						//	x1, ex
						switch(paramIndex)
						{
							case 0:
							case 2:
								result = true;
								break;
						}
						break;
					case "s":
						//	x2, ex
						switch(paramIndex)
						{
							case 0:
							case 2:
								result = true;
								break;
						}
						break;
					case "t":
						//	ex
						if(paramIndex == 0)
						{
							result = true;
						}
						break;
					case "v":
						result = false;
						break;
					case "z":
						break;
				}

			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ParamIsLocation																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified parameter index is
		/// a location.
		/// </summary>
		/// <param name="action">
		/// The action to estimate.
		/// </param>
		/// <param name="paramIndex">
		/// The 0-based index of the parameter to rate.
		/// </param>
		/// <returns>
		/// True if the specified parameter of the plot item was a location.
		/// Otherwise, false.
		/// </returns>
		public static bool ParamIsLocation(string action, int paramIndex)
		{
			string la = "";
			bool result = false;

			if(action?.Length > 0)
			{
				la = action.ToLower();
				switch(la)
				{
					case "a":
						switch(paramIndex)
						{
							case 5:
							case 6:
								//	ex, ey
								result = true;
								break;
						}
						break;
					case "c":
					case "h":
					case "l":
					case "m":
					case "q":
					case "s":
					case "t":
					case "v":
						result = true;
						break;
					case "z":
						break;
				}

			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ParamIsVertical																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified parameter index is
		/// vertical.
		/// </summary>
		/// <param name="action">
		/// The action to estimate.
		/// </param>
		/// <param name="paramIndex">
		/// The 0-based index of the parameter to rate.
		/// </param>
		/// <returns>
		/// True if the specified parameter of the plot item was vertical.
		/// Otherwise, false.
		/// </returns>
		public static bool ParamIsVertical(string action, int paramIndex)
		{
			string la = "";
			bool result = false;

			if(action?.Length > 0)
			{
				la = action.ToLower();
				switch(la)
				{
					case "a":
						switch(paramIndex)
						{
							case 1:
							case 6:
								//	ry, ey
								result = true;
								break;
						}
						break;
					case "c":
						//	y1, y2, ey
						switch(paramIndex)
						{
							case 1:
							case 3:
							case 5:
								result = true;
								break;
						}
						break;
					case "h":
						break;
					case "l":
						//	y
						if(paramIndex == 1)
						{
							result = true;
						}
						break;
					case "m":
						//	y
						if(paramIndex == 1)
						{
							result = true;
						}
						break;
					case "q":
						//	y1, ey
						switch(paramIndex)
						{
							case 1:
							case 3:
								result = true;
								break;
						}
						break;
					case "s":
						//	y2, ey
						switch(paramIndex)
						{
							case 1:
							case 3:
								result = true;
								break;
						}
						break;
					case "t":
						//	ey
						if(paramIndex == 1)
						{
							result = true;
						}
						break;
					case "v":
						result = true;
						break;
					case "z":
						break;
				}

			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Parse																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Parse the elements in the raw SVG path string into individual plot
		/// point records.
		/// </summary>
		/// <param name="path">
		/// Raw SVG line drawing path command.
		/// </param>
		/// <returns>
		/// Collection of plot points items representing the path being drawn.
		/// </returns>
		public static PlotPointsCollection Parse(string path)
		{
			PlotPointsItem item = null;
			MatchCollection matches = null;
			string number = "";
			int paramCount = 2;
			int paramIndex = 0;
			PlotPointsCollection result = new PlotPointsCollection();
			string text = "";

			if(path?.Length > 0)
			{
				matches = Regex.Matches(path, ResourceMain.rxFindSvgTransformParams);
				foreach(Match matchItem in matches)
				{
					text = GetValue(matchItem, "param");
					if(IsNumeric(text))
					{
						//	This is a number.
						number = text;
						paramIndex++;
						if(item == null)
						{
							//	A plot item has not yet been created. By default, we are
							//	using the relative move.
							item = new PlotPointsItem()
							{
								Action = "m"
							};
							result.Add(item);
						}
						else if(paramIndex > paramCount)
						{
							//	Create a new related action or ...
							//	Repeat the previous action.
							text = item.Action;
							if(text.ToLower() == "m")
							{
								//	Coordinates following the MOVETO are relative lineto
								//	commands.
								text = "l";
							}
							item = new PlotPointsItem()
							{
								Action = text
							};
							result.Add(item);
							paramIndex = 1;
						}
						item.Points.Add(number);
					}
					else
					{
						//	This item is a plot command.
						item = new PlotPointsItem()
						{
							Action = text
						};
						result.Add(item);
						switch(text)
						{
							case "A":
							case "a":
								paramCount = 7;
								break;
							case "C":
							case "c":
								paramCount = 6;
								break;
							case "H":
							case "h":
								paramCount = 1;
								break;
							case "L":
							case "l":
							case "M":
							case "m":
								paramCount = 2;
								break;
							case "Q":
							case "q":
							case "S":
							case "s":
								paramCount = 4;
								break;
							case "T":
							case "t":
								paramCount = 2;
								break;
							case "V":
							case "v":
								paramCount = 1;
								break;
							case "Z":
							case "z":
								paramCount = 0;
								break;
						}
						paramIndex = 0;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	PlotPointsItem																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual plot action with points.
	/// </summary>
	public class PlotPointsItem
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
		//*	Action																																*
		//*-----------------------------------------------------------------------*
		private string mAction = "";
		/// <summary>
		/// Get/Set the action to take.
		/// </summary>
		public string Action
		{
			get { return mAction; }
			set { mAction = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Points																																*
		//*-----------------------------------------------------------------------*
		private List<string> mPoints = new List<string>();
		/// <summary>
		/// Get a reference to the points assigned to this action.
		/// </summary>
		public List<string> Points
		{
			get { return mPoints; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
