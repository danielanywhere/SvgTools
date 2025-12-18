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
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LibreOfficeODS
{
	//*-------------------------------------------------------------------------*
	//*	OdsReader																																*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// OpenDocument ODS spreadsheet file reader for applications like
	/// LibreOffice Calc.
	/// </summary>
	public static class OdsReader
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* CreateTableFromSheet																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a data table from the provided ODS sheet.
		/// </summary>
		/// <param name="tableElement">
		/// Reference to the XML element containing the information about the
		/// XML table representing the sheet.
		/// </param>
		/// <param name="tableNs">
		/// The namespace for the XML table.
		/// </param>
		/// <param name="sheetIndex">
		/// The index of the sheet in the workbook.
		/// </param>
		/// <returns>
		/// Reference to a newly created data table, if found. Otherwise, null.
		/// </returns>
		private static DataTable CreateTableFromSheet(XElement tableElement,
			XNamespace tableNs, int sheetIndex)
		{
			bool bHeaderProcessed = false;
			List<String> cellValues = null;
			string columnName = "";
			int count = 0;
			int index = 0;
			int limit = 0;
			DataRow row = null;
			IEnumerable<XElement> rowElements = null;
			DataTable table = null;
			string tableName = "";

			if(tableElement != null && tableNs != null)
			{
				table = new DataTable();
				if(tableElement.Attribute(tableNs + "name") != null)
				{
					tableName = tableElement.Attribute(tableNs + "name").Value;
				}
				else
				{
					tableName = $"Sheet{sheetIndex}";
				}
				table.TableName = tableName;
				rowElements = tableElement.Elements(tableNs + "table-row");
				foreach(XElement rowElement in rowElements)
				{
					cellValues = ReadCells(rowElement, tableNs);
					if(!bHeaderProcessed)
					{
						index = 0;
						count = cellValues.Count;
						for(index = 0; index < count; index++)
						{
							columnName = cellValues[index];
							if(string.IsNullOrEmpty(columnName))
							{
								columnName = "Column" + (index + 1).ToString();
							}
							table.Columns.Add(FixColumnName(columnName), typeof(String));
						}
						bHeaderProcessed = true;
					}
					else
					{
						row = table.NewRow();
						index = 0;
						limit = Math.Min(cellValues.Count, table.Columns.Count);
						for(index = 0; index < limit; index++)
						{
							row[index] = cellValues[index];
						}
						table.Rows.Add(row);
					}
				}
			}
			return table;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FixColumnName																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a legal column name from the caller-supplied value.
		/// </summary>
		/// <param name="name">
		/// The name to convert.
		/// </param>
		/// <returns>
		/// The legal column name for data columns.
		/// </returns>
		private static string FixColumnName(string name)
		{
			string result = "";

			if(name?.Length > 0)
			{
				result = Regex.Replace(name, @"(?<nonan>\W+)", "");
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetCellValue																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the text value of the provided cell.
		/// </summary>
		/// <param name="cellElement">
		/// Reference to the XML cell element to inspect.
		/// </param>
		/// <param name="tableNs">
		/// Namespace for the XML table.
		/// </param>
		/// <returns>
		/// The value of the cell, if found. Otherwise, an empty string.
		/// </returns>
		private static string GetCellValue(XElement cellElement,
			XNamespace tableNs)
		{
			XAttribute formulaAttr = null;
			string result = "";
			XElement textElement = null;

			if(cellElement != null)
			{
				textElement = cellElement
					.Descendants()
					.FirstOrDefault(element => element.Name.LocalName == "p");
				if(textElement?.Value?.Length > 0)
				{
					result = textElement.Value;
				}
				else
				{
					formulaAttr = cellElement.Attribute(tableNs + "formula");
					if(formulaAttr != null)
					{
						result = formulaAttr.Value;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetContent																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the raw XML content of the specified file.
		/// </summary>
		/// <param name="filePath">
		/// Fully qualified path and filename of the file to open.
		/// </param>
		/// <returns>
		/// The raw XML content found within the 
		/// </returns>
		private static string GetContent(String filePath)
		{
			ZipArchiveEntry entry = null;
			string result = "";

			try
			{
				using(ZipArchive archive = ZipFile.OpenRead(filePath))
				{
					entry = archive.GetEntry("content.xml");
					if(entry != null)
					{
						using(Stream stream = entry.Open())
						{
							using(StreamReader reader = new StreamReader(stream))
							{
								result = reader.ReadToEnd();
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				Trace.WriteLine($"Err: {ex.Message}");
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ReadCells																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Read the cell values from the current row.
		/// </summary>
		/// <param name="rowElement">
		/// Reference to the row for which the cells will be retrieved.
		/// </param>
		/// <param name="tableNs">
		/// The active table namespace.
		/// </param>
		/// <returns>
		/// Reference to a list of cell values for the current row.
		/// </returns>
		private static List<string> ReadCells(XElement rowElement,
			XNamespace tableNs)
		{
			IEnumerable<XElement> cellElements = null;
			List<string> cells = new List<string>();
			int index = 0;
			int parsed = 0;
			XAttribute repeatAttr = null;
			int repeatCount = 1;
			string value = "";

			if(rowElement != null && tableNs != null)
			{
				cellElements = rowElement.Elements(tableNs + "table-cell");
				foreach(XElement cellElement in cellElements)
				{
					value = GetCellValue(cellElement, tableNs);
					repeatCount = 1;
					repeatAttr =
						cellElement.Attribute(tableNs + "number-columns-repeated");
					if(repeatAttr != null)
					{
						parsed = 0;
						if(int.TryParse(repeatAttr.Value, out parsed))
						{
							repeatCount = parsed;
						}
					}
					for(index = 0; index < repeatCount; index++)
					{
						cells.Add(value);
					}
				}
			}
			return cells;
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************

		//*-----------------------------------------------------------------------*
		//* ReadOds																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Read an ODS file and return a data set where each table corresponds to
		/// the contents of a sheet in the workbook.
		/// </summary>
		/// <param name="filename">
		/// Fully qualified path and filename of the file to read.
		/// </param>
		/// <returns>
		/// Reference to a data set containing the contents of the workbook
		/// sheets.
		/// </returns>
		public static DataSet ReadOds(string filename)
		{
			string content = "";
			DataSet data = new DataSet();
			XDocument document = null;
			int index = 1;
			DataTable table = null;
			IEnumerable<XElement> tableElements = null;
			XNamespace tableNs = "urn:oasis:names:tc:opendocument:xmlns:table:1.0";

			content = GetContent(filename);
			if(content?.Length > 0)
			{
				document = XDocument.Parse(content);
				tableElements = document.Descendants(tableNs + "table");
				index = 1;
				foreach(XElement tableElement in tableElements)
				{
					table = CreateTableFromSheet(tableElement, tableNs, index);
					if(table != null)
					{
						data.Tables.Add(table);
					}
					index++;
				}
			}
			return data;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*
}
