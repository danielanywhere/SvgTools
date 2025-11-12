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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;

using Flee;
using Flee.PublicTypes;
using Geometry;
using Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SkiaSharp;

using static SvgToolsLib.SvgToolsUtil;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	SvgActionCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgActionItem Items.
	/// </summary>
	public class SvgActionCollection : List<SvgActionItem>
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
		//*	GetBase																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the base number or filename pattern of the source or target
		/// files, depending upon the action.
		/// </summary>
		public string GetBase()
		{
			string result = "";

			if(mParent != null)
			{
				result = mParent.Base;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetCount																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the count property from the parent entity.
		/// </summary>
		/// <returns>
		/// The value of the count property in the parent entity, if found.
		/// Otherwise, 0.
		/// </returns>
		public float GetCount()
		{
			float result = 0f;

			if(mParent != null)
			{
				result = mParent.Count;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetCurrentFile																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the CurrentFile property from the parent entity.
		/// </summary>
		/// <returns>
		/// The value of the CurrentFile property in the parent entity, if found.
		/// Otherwise, null.
		/// </returns>
		public FileInfo GetCurrentFile()
		{
			FileInfo result = null;

			if(mParent != null)
			{
				result = mParent.CurrentFile;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetDateTimeValue																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the DateTimeValue property of the parent entity.
		/// </summary>
		/// <returns>
		/// The DateTimeValue property value of the parent entity, if found.
		/// Otherwise, DateTime.MinValue.
		/// </returns>
		public DateTime GetDateTimeValue()
		{
			DateTime result = DateTime.MinValue;

			if(mParent != null)
			{
				result = mParent.DateTimeValue;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetDigits																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the Digits property from the parent entity.
		/// </summary>
		/// <returns>
		/// The value of the Digits property on the parent entity, if found.
		/// Otherwise, 0.
		/// </returns>
		public int GetDigits()
		{
			int result = 0;

			if(mParent != null)
			{
				result = mParent.Digits;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetInputFilename																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the InputFilename property from the parent entity.
		/// </summary>
		/// <returns>
		/// Value of the InputFilename property on the parent entity, if found.
		/// Otherwise, an empty string.
		/// </returns>
		public string GetInputFilename()
		{
			string result = "";

			if(mParent != null)
			{
				result = mParent.InputFilename;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetInputFiles																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a reference to the collection of file information used as input
		/// in this session.
		/// </summary>
		/// <returns>
		/// A reference to the parent's InputFiles collection, if found. Otherwise,
		/// an empty collection.
		/// </returns>
		public List<FileInfo> GetInputFiles()
		{
			List<FileInfo> result = null;

			if(mParent != null)
			{
				result = mParent.InputFiles;
			}
			else
			{
				result = new List<FileInfo>();
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetInputFolderName																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the InputFolderName property value from the parent entity.
		/// </summary>
		/// <returns>
		/// Value of the InputFolderName property on the parent entity, if found.
		/// Otherwise, an empty string.
		/// </returns>
		public string GetInputFolderName()
		{
			string result = "";

			if(mParent != null)
			{
				result = mParent.InputFolderName;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetInputNames																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get a reference to the list of filenames or foldernames with
		/// or without wildcards. This parameter can be specified multiple times
		/// on the command line with different values to load multiple input files.
		/// </summary>
		public List<string> GetInputNames()
		{
			List<string> result = null;

			if(mParent != null)
			{
				//	If this item is not overridden, then default to the parent.
				result = mParent.InputNames;
			}
			else
			{
				result = new List<string>();
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetOptionByName																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the option specified by name from a parent entity.
		/// </summary>
		/// <param name="collection">
		/// Reference to the collection for which the option is being found.
		/// </param>
		/// <param name="optionName">
		/// Name of the option to retrieve.
		/// </param>
		/// <returns>
		/// Reference to the specified option, if found. Otherwise, null.
		/// </returns>
		public static SvgActionOptionItem GetOptionByName(
			SvgActionCollection collection, string optionName)
		{
			SvgActionOptionItem result = null;

			if(collection != null && collection.mParent != null)
			{
				result =
					SvgActionItem.GetOptionByName(collection.mParent, optionName);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetOutputFolderName																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the OutputFolderName property on the parent entity.
		/// </summary>
		/// <returns>
		/// Value of the OutputFolderName property on the parent entity, if found.
		/// Otherwise, an empty string.
		/// </returns>
		public string GetOutputFolderName()
		{
			string result = "";

			if(mParent != null)
			{
				result = mParent.OutputFolderName;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetOutputName																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the OutputName property on the parent entity.
		/// </summary>
		/// <returns>
		/// Value of the OutputName property on the parent entity, if found.
		/// Otherwise, and empty string.
		/// </returns>
		public string GetOutputName()
		{
			string result = "";

			if(mParent != null)
			{
				result = mParent.OutputName;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetOutputType																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the OutputType property on the parent entity.
		/// </summary>
		/// <returns>
		/// The value of the OutputType property on the parent entity, if found.
		/// Otherwise, RenderFileTypeEnum.Auto.
		/// </returns>
		public RenderFileTypeEnum GetOutputType()
		{
			RenderFileTypeEnum result = RenderFileTypeEnum.Auto;

			if(mParent != null)
			{
				result = mParent.OutputType;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetPattern																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the Pattern property on the parent entity.
		/// </summary>
		/// <returns>
		/// The value of the Pattern property on the parent entity, if found.
		/// Otherwise, an empty string.
		/// </returns>
		public string GetPattern()
		{
			string result = "";

			if(mParent != null)
			{
				result = mParent.Pattern;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetRange																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a reference to the Range property on the parent entity.
		/// </summary>
		/// <returns>
		/// Reference to the Range property on the parent entity, if found.
		/// Otherwise, null.
		/// </returns>
		public StartEndItem GetRange()
		{
			StartEndItem result = null;

			if(mParent != null)
			{
				result = mParent.Range;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetSourceFolderName																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the SourceFolderName property value from the parent entity.
		/// </summary>
		/// <returns>
		/// Value of the SourceFolderName property on the parent entity, if found.
		/// Otherwise, an empty string.
		/// </returns>
		public string GetSourceFolderName()
		{
			string result = "";

			if(mParent != null)
			{
				result = mParent.SourceFolderName;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetText																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the Text property from the parent entity.
		/// </summary>
		/// <returns>
		/// Value of the Text property on the parent entity, if found. Otherwise,
		/// an empty string.
		/// </returns>
		public string GetText()
		{
			string result = "";

			if(mParent != null)
			{
				result = mParent.Text;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetWorkingPath																												*
		//*-----------------------------------------------------------------------*
		//private string mWorkingPath = "";
		/// <summary>
		/// Return the working path for operations in this instance.
		/// </summary>
		public string GetWorkingPath()
		{
			string result = "";

			if(mParent != null)
			{
				result = mParent.WorkingPath;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetWorkingSvg																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the working SVG file for operations in this instance.
		/// </summary>
		public SvgDocumentItem GetWorkingSvg()
		{
			SvgDocumentItem result = null;

			if(mParent != null)
			{
				result = mParent.WorkingSvg;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* InitializeParent																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Initialize the Parent property in all of the decendants of the
		/// specified collection.
		/// </summary>
		/// <param name="actions">
		/// Reference to a collection of actions.
		/// </param>
		public static void InitializeParent(SvgActionCollection actions)
		{
			if(actions?.Count > 0)
			{
				foreach(SvgActionItem actionItem in actions)
				{
					actionItem.Parent = actions;
					actionItem.Actions.Parent = actionItem;
					InitializeParent(actionItem.Actions);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Parent																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Parent">Parent</see>.
		/// </summary>
		private SvgActionItem mParent = null;
		/// <summary>
		/// Get/Set a reference to the batch file to which this sequence belongs.
		/// </summary>
		[JsonIgnore]
		public SvgActionItem Parent
		{
			get { return mParent; }
			set
			{
				//	NOTE: This is stupid because Newtonsoft JSON ...
				//	bypasses an overridden Add(Item) method.
				mParent = value;
				foreach(SvgActionItem actionItem in this)
				{
					actionItem.Parent = this;
				}
			}
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	SvgActionItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual action that will be taken on the SVG.
	/// </summary>
	public class SvgActionItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		/// <summary>
		/// Public properties of this class.
		/// </summary>
		private static List<PropertyInfo> mPublicProperties =
			new List<PropertyInfo>();
		/// <summary>
		/// Working path monitor.
		/// </summary>
		private static string mWorkingPathLast = "";

		//*-----------------------------------------------------------------------*
		//* ApplyTransforms																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Reduce the number of transformations in the document.
		/// </summary>
		/// <param name="item">
		/// Reference to the action that specifies the document to update.
		/// </param>
		public static void ApplyTransforms(SvgActionItem item)
		{
			string content = "";
			SvgDocumentItem doc = null;
			HtmlNodeItem svg = null;

			if(item != null)
			{
				doc = GetSpecifiedOrWorking(item);
				if(doc != null)
				{
					HtmlDocument.FillUniqueIds(doc.Document);
					svg = doc.Document.Nodes.FindMatch(x => x.NodeType == "svg");
					if(svg != null)
					{
						SvgToolsUtil.ApplyTransforms(svg);
						if(doc.IsLocal)
						{
							content = doc.Document.Html;
							File.WriteAllText(item.OutputFile.FullName, content);
							Trace.WriteLine($" File written: {item.OutputFile.Name}",
								$"{MessageImportanceEnum.Info}");
						}
					}
				}
				else
				{
					Trace.WriteLine($" Error: Input files were not specified.",
						$"{MessageImportanceEnum.Err}");
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* CalculateTransform																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Solve a single transform whose values are stored in the properties
		/// collection.
		/// </summary>
		/// <param name="item">
		/// Reference to the action item to process.
		/// </param>
		private static void CalculateTransform(SvgActionItem item)
		{
			float angle = 0f;
			bool bContinue = true;
			bool bHeight = false;
			bool bWidth = false;
			float cx = 0f;
			float cy = 0f;
			float dx = 0f;
			float dy = 0f;
			float height = 0f;
			float ix1 = 0f;
			float ix2 = 0f;
			float iy1 = 0f;
			float iy2 = 0f;
			float length = 0f;
			float ox1 = 0f;
			float ox2 = 0f;
			float oy1 = 0f;
			float oy2 = 0f;
			Matrix32 m32 = null;
			FVector2 point = null;
			NameValueItem property = null;
			TransformItem transform = null;
			TransformCollection transforms = null;
			FVector2 vector = null;
			float width = 0f;

			if(item != null)
			{
				property = item.Properties.FirstOrDefault(x =>
					x.Name.ToLower() == "transform");
				if(property != null)
				{
					//	Transform property found.
					transforms = TransformCollection.Parse(property.Value);
					if(transforms.Count > 0)
					{
						transform = transforms[0];
						if(transform.Parameters.Count == 0)
						{
							Trace.WriteLine(" Error: transform has no argument values.",
								$"{MessageImportanceEnum.Err}");
							bContinue = false;
						}
					}
					if(bContinue)
					{
						if(transform.TransformType == TransformTypeEnum.Matrix &&
							transform.Parameters.Count < 6)
						{
							Trace.WriteLine(
								" Error: matrix transform requires 6 arguments.",
								$"{MessageImportanceEnum.Err}");
							bContinue = false;
						}
					}
					if(bContinue)
					{
						property = item.Properties.FirstOrDefault(x =>
							x.Name.ToLower() == "x");
						if(property != null)
						{
							ix1 = ToFloat(property.Value);
						}
						else
						{
							Trace.WriteLine(" Error: 'x' parameter was not specified.",
								$"{MessageImportanceEnum.Err}");
							bContinue = false;
						}
						if(bContinue)
						{
							property = item.Properties.FirstOrDefault(x =>
								x.Name.ToLower() == "y");
							if(property != null)
							{
								iy1 = ToFloat(property.Value);
							}
							else
							{
								Trace.WriteLine(" Error: 'y' parameter was not specified.",
									$"{MessageImportanceEnum.Err}");
								bContinue = false;
							}
						}
						if(bContinue)
						{
							property = item.Properties.FirstOrDefault(x =>
								x.Name.ToLower() == "width");
							if(property != null)
							{
								width = ToFloat(property.Value);
								bWidth = true;
							}
							property = item.Properties.FirstOrDefault(x =>
								x.Name.ToLower() == "height");
							if(property != null)
							{
								height = ToFloat(property.Value);
								bHeight = true;
							}
							if((bWidth || bHeight) && (!bWidth || !bHeight))
							{
								if(bWidth)
								{
									Trace.WriteLine("'width' was omitted, and will be 0.",
										$"{MessageImportanceEnum.Info}");
								}
								else
								{
									Trace.WriteLine("'height' was omitted, and will be 0.",
										$"{MessageImportanceEnum.Info}");
								}
							}
						}
						if(bContinue)
						{
						}
						if(bContinue)
						{
							//	At least x and y were supplied.
							if(bWidth || bHeight)
							{
								ix2 = ix1 + width;
								iy2 = iy1 + height;
							}
							switch(transform.TransformType)
							{
								case TransformTypeEnum.Matrix:
									m32 = new Matrix32();
									m32.AssignSequential(transform.Parameters);
									point = m32.Transform(ix1, iy1);
									ox1 = point.X;
									oy1 = point.Y;
									if(bWidth || bHeight)
									{
										point = m32.Transform(ix2, iy2);
										ox2 = point.X;
										oy2 = point.Y;
									}
									break;
								case TransformTypeEnum.Rotate:
									bWidth = false;
									bHeight = false;
									if(transform.Parameters.Count > 0)
									{
										angle = Trig.DegToRad(transform.Parameters[0]);
										if(transform.Parameters.Count > 2)
										{
											cx = transform.Parameters[1];
											cy = transform.Parameters[2];
										}
										dx = ix1 - cx;
										dy = iy1 - cy;
										length = Trig.GetLineHypFromAdjOpp(dx, dy);
										vector = Trig.GetDestPoint(cx, cy, angle, length);
										ox1 = vector.X;
										oy1 = vector.Y;
									}
									break;
								case TransformTypeEnum.Scale:
									if(transform.Parameters.Count == 1)
									{
										ox1 = ix1 * transform.Parameters[0];
										oy1 = iy1 * transform.Parameters[0];
										if(bWidth || bHeight)
										{
											ox2 = ix2 * transform.Parameters[0];
											oy2 = iy2 * transform.Parameters[0];
										}
									}
									else
									{
										ox1 = ix1 * transform.Parameters[0];
										oy1 = iy1 * transform.Parameters[1];
										if(bWidth || bHeight)
										{
											ox2 = ix2 * transform.Parameters[0];
											oy2 = iy2 * transform.Parameters[1];
										}
									}
									break;
								case TransformTypeEnum.Translate:
									if(transform.Parameters.Count == 1)
									{
										ox1 = ix1 + transform.Parameters[0];
										oy1 = iy1 + transform.Parameters[0];
										if(bWidth || bHeight)
										{
											ox2 = ix2 + transform.Parameters[0];
											oy2 = iy2 + transform.Parameters[0];
										}
									}
									else
									{
										ox1 = ix1 + transform.Parameters[0];
										oy1 = iy1 + transform.Parameters[1];
										if(bWidth || bHeight)
										{
											ox2 = ix2 + transform.Parameters[0];
											oy2 = iy2 + transform.Parameters[1];
										}
									}
									break;
								default:
									Trace.WriteLine(
										" Error: Transform type not implemented: " +
										$"{transform.TransformType}",
										$"{MessageImportanceEnum.Err}");
									bContinue = false;
									break;
							}
						}
						if(bContinue)
						{
							Trace.WriteLine(
								$"Transform {transform.TransformType} starting...",
								$"{MessageImportanceEnum.Info}");
							Trace.WriteLine($" Left:   {ix1}",
								$"{MessageImportanceEnum.Info}");
							Trace.WriteLine($" Top:    {iy1}",
								$"{MessageImportanceEnum.Info}");
							if(bWidth || bHeight)
							{
								Trace.WriteLine($" Right:  {ix2}",
									$"{MessageImportanceEnum.Info}");
								Trace.WriteLine($" Bottom: {iy2}",
									$"{MessageImportanceEnum.Info}");
								Trace.WriteLine($" Width:  {width}",
									$"{MessageImportanceEnum.Info}");
								Trace.WriteLine($" Height: {height}",
									$"{MessageImportanceEnum.Info}");
							}
						}
						if(bContinue)
						{
							if(bWidth || bHeight)
							{
								width = ox2 - ox1;
								height = oy2 - oy1;
							}
							Trace.WriteLine("Transform completed...",
								$"{MessageImportanceEnum.Info}");
							Trace.WriteLine($" Left:   {ox1}",
								$"{MessageImportanceEnum.Info}");
							Trace.WriteLine($" Top:    {oy1}",
								$"{MessageImportanceEnum.Info}");
							if(bWidth || bHeight)
							{
								Trace.WriteLine($" Right:  {ox2}",
									$"{MessageImportanceEnum.Info}");
								Trace.WriteLine($" Bottom: {oy2}",
									$"{MessageImportanceEnum.Info}");
								Trace.WriteLine($" Width:  {width}",
									$"{MessageImportanceEnum.Info}");
								Trace.WriteLine($" Height: {height}",
									$"{MessageImportanceEnum.Info}");
							}
						}
					}
					else
					{
						Trace.WriteLine(
							$" Error: Unknown transform: '{property.Value}'",
							$"{MessageImportanceEnum.Err}");
					}
				}
				else
				{
					Trace.WriteLine(" Error: 'transform' property not specified.",
						$"{MessageImportanceEnum.Err}");
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* CheckElements																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Check all of the specified elements and return a value indicating
		/// whether the masked items were all valid.
		/// </summary>
		/// <param name="item">
		/// Reference to the file action item for which the elements are being
		/// tested.
		/// </param>
		/// <param name="element">
		/// Bitmasked action element flags to require on this action.
		/// </param>
		/// <param name="includeInherited">
		/// Optional value indicating whether to include inherited values,
		/// if true, or to include only local values, if false. Default = true.
		/// </param>
		/// <param name="quiet">
		/// Optional value indicating whether to run the operation in quiet mode.
		/// If true, no warnings or errors will be sent. Default = false.
		/// </param>
		/// <returns>
		/// Value indicating whether the check was successful.
		/// </returns>
		/// <remarks>
		/// Error messages are printed to the console when one or more of the
		/// specified elements are not found.
		/// </remarks>
		private static bool CheckElements(SvgActionItem item,
			ActionElementEnum element, bool includeInherited = true,
			bool quiet = false)
		{
			int count = 0;
			DirectoryInfo dir = null;
			FileInfo file = null;
			int index = 0;
			bool result = true;
			bool sendMessages = !quiet;
			string vBase = "";
			float vCount = 0f;
			DateTime vDateTime = DateTime.MinValue;
			string vInputFilename = "";
			List<FileInfo> vInputFiles = null;
			int vInt = 0;
			string vPattern = "";
			StartEndItem vRange = null;
			string vText = "";
			string workingFolder = "";

			if(item != null && element != ActionElementEnum.None)
			{
				if((element & ActionElementEnum.Action) != ActionElementEnum.None)
				{
					if(item.Action == SvgActionTypeEnum.None)
					{
						if(sendMessages)
						{
							Trace.WriteLine(" Error: No action specified.",
								$"{MessageImportanceEnum.Err}");
						}
						result = false;
					}
				}
				if((element & ActionElementEnum.Base) != ActionElementEnum.None)
				{
					vBase = (includeInherited ? item.Base : item.mBase);
					if(vBase.Length == 0)
					{
						if(sendMessages)
						{
							Trace.WriteLine(" Error: Base is required in this action.",
								$"{MessageImportanceEnum.Err}");
						}
						result = false;
					}
				}
				if((element & ActionElementEnum.Count) != ActionElementEnum.None)
				{
					vCount = (includeInherited ? item.Count : item.mCount);
					if(vCount == 0f)
					{
						if(sendMessages)
						{
							Trace.WriteLine(" Error: Count is required for this action.",
								$"{MessageImportanceEnum.Err}");
						}
						result = false;
					}
				}
				if((element & ActionElementEnum.DateTimeValue) !=
					ActionElementEnum.None)
				{
					vDateTime = (includeInherited ?
						item.DateTimeValue : item.mDateTimeValue);
					if(vDateTime == DateTime.MinValue)
					{
						if(sendMessages)
						{
							Trace.WriteLine(" Error: DateTime was not specified.",
								$"{MessageImportanceEnum.Err}");
						}
						result = false;
					}
				}
				if((element & ActionElementEnum.Digits) != ActionElementEnum.None)
				{
					vInt = (includeInherited ? item.Digits : item.mDigits);
					if(vInt == 0)
					{
						if(sendMessages)
						{
							Trace.WriteLine(" Error: A value is required for Digits.",
								$"{MessageImportanceEnum.Err}");
						}
						result = false;
					}
				}
				if((element & ActionElementEnum.InputFilename) !=
					ActionElementEnum.None)
				{
					//	In this version, when InputFilename is expressed, only files
					//	are specified in the InputFiles collection.
					vInputFiles = (includeInherited ?
						item.InputFiles : item.mInputFiles);
					count = vInputFiles.Count;
					for(index = 0; index < count; index++)
					{
						file = vInputFiles[index];
						if((file.Attributes & FileAttributes.Directory) !=
							(FileAttributes)0)
						{
							//	This item is a directory. Remove it.
							vInputFiles.RemoveAt(index);
							index--;  //	Deindex.
							count--;  //	Decount.
						}
					}
					if(vInputFiles.Count == 0)
					{
						if(sendMessages)
						{
							Trace.WriteLine(" Error: Input files were not specified.",
								$"{MessageImportanceEnum.Err}");
						}
						result = false;
					}
				}
				if((element & ActionElementEnum.InputFolderName) !=
					ActionElementEnum.None)
				{
					//	In this version, when InputFoldername is expressed, only
					//	folders are specified in the InputFiles collection.
					vInputFiles = (includeInherited ?
						item.InputFiles : item.mInputFiles);
					count = vInputFiles.Count;
					for(index = 0; index < count; index++)
					{
						file = vInputFiles[index];
						if((file.Attributes & FileAttributes.Directory) ==
							(FileAttributes)0)
						{
							//	This item is a file. Remove it.
							vInputFiles.RemoveAt(index);
							index--;  //	Deindex.
							count--;  //	Decount.
						}
					}
					if(vInputFiles.Count > 0)
					{
						//	Input folders are present.
						item.InputDir = new DirectoryInfo(vInputFiles[0].FullName);
					}
					if(vInputFiles.Count == 0)
					{
						//	If no files are specified, use the working folder.
						workingFolder = GetPropertyByName(item, nameof(WorkingPath));
						if(workingFolder.Length > 0 && Directory.Exists(workingFolder))
						{
							file = new FileInfo(workingFolder);
							vInputFiles.Add(file);
							item.InputDir = new DirectoryInfo(workingFolder);
						}
					}
					if(vInputFiles.Count == 0)
					{
						if(sendMessages)
						{
							Trace.WriteLine(" Error: Input folders were not specified.",
								$"{MessageImportanceEnum.Err}");
						}
						result = false;
					}
				}
				if((element & ActionElementEnum.Inputs) != ActionElementEnum.None)
				{
					//	Multiple input files.
					vInputFiles = (includeInherited ?
						item.InputFiles : item.mInputFiles);
					if(vInputFiles.Count == 0)
					{
						if(sendMessages)
						{
							Trace.WriteLine(" Error: No input files specified.",
								$"{MessageImportanceEnum.Err}");
						}
						result = false;
					}
				}
				if((element & ActionElementEnum.OutputFilename) !=
					ActionElementEnum.None)
				{
					vInputFiles = (includeInherited ?
						item.InputFiles : item.mInputFiles);
					vInputFilename = (includeInherited ?
						item.InputFilename : item.mInputFilename);
					if(item.OutputFile == null &&
						vInputFiles?.Count == 1 &&
						((element & ActionElementEnum.InputFilename) !=
						ActionElementEnum.None))
					{
						//	If the input and output are both expected, and
						//	only the input was supplied, then use the
						//	input file as the output.
						item.OutputFilename = vInputFilename;
						item.OutputFile = vInputFiles[0];
					}
					if(item.OutputFile == null)
					{
						if(sendMessages)
						{
							Trace.WriteLine(" Error: Output filename was not specified.",
								$"{MessageImportanceEnum.Err}");
						}
						result = false;
					}
					else
					{
						dir = new DirectoryInfo(
							Path.GetDirectoryName(item.OutputFile.FullName));
						if(!dir.Exists)
						{
							try
							{
								dir.Create();
							}
							catch
							{
								if(sendMessages)
								{
									Trace.WriteLine(
										" Error: Could not create output directory.",
										$"{MessageImportanceEnum.Err}");
								}
							}
						}
					}
				}
				if((element & ActionElementEnum.OutputFoldername) !=
					ActionElementEnum.None)
				{
					if(item.OutputDir == null)
					{
						//	If no output folder was specified, use the working folder.
						workingFolder = GetPropertyByName(item, nameof(WorkingPath));
						if(workingFolder.Length > 0 && Directory.Exists(workingFolder))
						{
							dir = new DirectoryInfo(workingFolder);
							item.OutputDir = dir;
						}
					}
					if(item.OutputDir == null)
					{
						if(sendMessages)
						{
							Trace.WriteLine(" Error: Output folder name was not specified.",
								$"{MessageImportanceEnum.Err}");
						}
						result = false;
					}
				}
				if((element & ActionElementEnum.OutputName) != ActionElementEnum.None)
				{
					//	In this version, output can be either a file or a folder.
					if(item.OutputDir == null && item.OutputFile == null)
					{
						if(sendMessages)
						{
							Trace.WriteLine(
								" Error: Output name was not specified.",
								$"{MessageImportanceEnum.Err}");
						}
						result = false;
					}
				}
				if((element & ActionElementEnum.Pattern) != ActionElementEnum.None)
				{
					vPattern = (includeInherited ? item.Pattern : item.mPattern);
					if(vPattern.Length == 0)
					{
						if(sendMessages)
						{
							Trace.WriteLine(" Error: Pattern was not specified.",
								$"{MessageImportanceEnum.Err}");
						}
						result = false;
					}
				}
				if((element & ActionElementEnum.Range) != ActionElementEnum.None)
				{
					//	In this version, the range can be a single ended specification.
					vRange = (includeInherited ? item.Range : item.mRange);
					if(vRange.StartValue.Length == 0 &&
						vRange.EndValue.Length == 0)
					{
						if(sendMessages)
						{
							Trace.WriteLine(" Error: Range was not specified.",
								$"{MessageImportanceEnum.Err}");
						}
						result = false;
					}
				}
				if((element & ActionElementEnum.SourceFolderName) !=
					ActionElementEnum.None)
				{
					//	This version only has one source folder.
					item.SourceDir = null;
					if(item.SourceFolderName.Length > 0)
					{
						//	Source folder name has been specified.
						item.SourceDir = new DirectoryInfo(
							AbsolutePath(item.WorkingPath, item.SourceFolderName));
						if(!item.SourceDir.Exists)
						{
							//	If the folder doesn't exist, release it.
							item.SourceDir = null;
						}
					}
					if(item.SourceDir == null)
					{
						if(sendMessages)
						{
							Trace.WriteLine(" Error: Source folder was not specified.",
								$"{MessageImportanceEnum.Err}");
						}
						result = false;
					}
				}
				if((element & ActionElementEnum.Text) != ActionElementEnum.None)
				{
					vText = (includeInherited ? item.Text : item.mText);
					if(vText.Length == 0)
					{
						if(sendMessages)
						{
							Trace.WriteLine(" Error: Text parameter was not specified.",
								$"{MessageImportanceEnum.Err}");
						}
						result = false;
					}
				}
				if((element & ActionElementEnum.WorkingPath) !=
					ActionElementEnum.None)
				{
					//	Working path can always be inherited.
					if(item.WorkingPath.Length == 0)
					{
						if(sendMessages)
						{
							Trace.WriteLine(" Error: Working path was not specified.",
								$"{MessageImportanceEnum.Err}");
						}
						result = false;
					}
					else
					{
						dir = new DirectoryInfo(
							GetPropertyByName(item, nameof(WorkingPath)));
						if(!dir.Exists)
						{
							if(sendMessages)
							{
								Trace.WriteLine(" Error: Working path does not exist.",
									$"{MessageImportanceEnum.Err}");
							}
							result = false;
						}
						else if((dir.Attributes & FileAttributes.Directory) !=
							FileAttributes.Directory)
						{
							if(sendMessages)
							{
								Trace.WriteLine(
									" Error: A file was specified as the working directory.",
									$"{MessageImportanceEnum.Err}");
							}
							result = false;
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* CleanupSvg																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Clean up the specified SVG file by dereferencing links,
		/// applying transforms, purging unused defs, and rounding all
		/// values.
		/// </summary>
		/// <param name="item">
		/// Reference to the item containing information about the file and
		/// properties to use.
		/// </param>
		private static void CleanupSvg(SvgActionItem item)
		{
			string content = "";
			SvgDocumentItem doc = null;
			int precision = 0;

			if(item != null)
			{
				doc = GetSpecifiedOrWorking(item);
				if(doc != null)
				{
					Trace.WriteLine(" Dereference Links...",
						$"{MessageImportanceEnum.Info}");
					Trace.WriteLine(" ApplyTransforms...",
						$"{MessageImportanceEnum.Info}");
					SvgToolsUtil.ApplyTransforms(doc.Document);
					Trace.WriteLine(" PurgeDefs...",
						$"{MessageImportanceEnum.Info}");
					SvgToolsUtil.PurgeDefs(doc.Document);
					Trace.WriteLine(" RoundAllValues...",
						$"{MessageImportanceEnum.Info}");
					precision = GetPrecision(item);
					SvgToolsUtil.RoundAllValues(doc.Document, precision);

					if(doc.IsLocal)
					{
						//	Per-file mode is active.
						content = doc.Document.Html;
						File.WriteAllText(item.OutputFile.FullName, content);
						Trace.WriteLine($" File written: {item.OutputFile.Name}",
							$"{MessageImportanceEnum.Info}");
					}
				}
				else
				{
					Trace.WriteLine($" Error: Input files were not specified.",
						$"{MessageImportanceEnum.Err}");
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ClearInputFiles																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Clear the local InputFiles collection for the immediate parent item.
		/// </summary>
		/// <param name="item">
		/// Reference to the action item calling for the InputFiles collection to
		/// be cleared.
		/// </param>
		private static void ClearInputFiles(SvgActionItem item)
		{
			if(item != null && item.mParent != null && item.mParent.Parent != null)
			{
				item.mParent.Parent.mInputFiles.Clear();
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* DereferenceLinks																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Find objects used by reference through href, such as through the use
		/// element, create local instances of those objects contained by the user,
		/// and remove the href attributes.
		/// </summary>
		/// <param name="item">
		/// Reference to the action item describing the files to activate.
		/// </param>
		private static void DereferenceLinks(SvgActionItem item)
		{
			string content = "";
			HtmlNodeItem svg = null;
			SvgDocumentItem doc = null;

			if(item != null)
			{
				doc = GetSpecifiedOrWorking(item);
				if(doc != null)
				{
					HtmlDocument.FillUniqueIds(doc.Document);
					svg = doc.Document.Nodes.FindMatch(x => x.NodeType == "svg");
					if(svg != null)
					{
						SvgToolsUtil.DereferenceLinks(svg);
						if(doc.IsLocal)
						{
							//	Per-file mode is active.
							content = doc.Document.Html;
							File.WriteAllText(item.OutputFile.FullName, content);
							Trace.WriteLine($" File written: {item.OutputFile.Name}",
								$"{MessageImportanceEnum.Info}");
						}
					}
				}
				else
				{
					Trace.WriteLine($" Error: Input files were not specified.",
						$"{MessageImportanceEnum.Err}");
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* DrawImage																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Draw the image specified by ImageName onto the working image at the
		/// location specified by user properties Left and Top.
		/// </summary>
		/// <param name="item">
		/// Reference to the action item describing the image to draw and the
		/// location at which to draw it.
		/// </param>
		private static void DrawImage(SvgActionItem item)
		{
			int height = 0;
			SKBitmap bitmap = null;
			BitmapInfoItem sourceImage = null;
			SKRect sourceRect = SKRect.Empty;
			SKRect targetRect = SKRect.Empty;
			int width = 0;
			int x = 0;
			int y = 0;

			if(item != null && WorkingImage?.Bitmap != null)
			{
				sourceImage = Images.FirstOrDefault(x =>
					x.Name == GetPropertyByName(item, "ImageName"));
				if(sourceImage != null)
				{
					Trace.WriteLine($" {sourceImage.Name}",
						$"{MessageImportanceEnum.Info}");
					bitmap = sourceImage.Bitmap;
					sourceRect =
						new SKRect(0, 0, bitmap.Width, bitmap.Height);
					x = ToInt(GetPropertyByName(item, "Left"));
					y = ToInt(GetPropertyByName(item, "Top"));
					width = ToInt(GetPropertyByName(item, "Width"));
					height = ToInt(GetPropertyByName(item, "Height"));
					if(width == 0)
					{
						width = bitmap.Width;
					}
					if(height == 0)
					{
						height = bitmap.Height;
					}
					targetRect = new SKRect(x, y, width, height);
					DrawBitmap(bitmap, WorkingImage.Bitmap, sourceRect, targetRect);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FileOpenImage																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Open the image specified in InputFilename.
		/// </summary>
		/// <param name="item">
		/// Reference to the item from which the item will be opened.
		/// </param>
		/// <remarks>
		/// This method works upon the currently open file.
		/// </remarks>
		private static void FileOpenImage(SvgActionItem item)
		{
			SKBitmap bitmap = null;
			SKBitmap bitmapA = null;
			FileInfo file = null;
			string imageFilename = "";
			BitmapInfoItem imageInfo = null;
			string imageName = "";
			SKSamplingOptions samplingOptions =
				new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);

			if(item != null)
			{
				imageFilename = GetPropertyByName(item, "ImageFilename");
				if(imageFilename.Length > 0)
				{
					file = new FileInfo(AbsolutePath(item.WorkingPath, imageFilename));
				}
				else
				{
					file = GetCurrentFile(item);
				}
				if(file != null && file.Exists)
				{
					imageName = GetPropertyByName(item, "ImageName");
					if(imageName.Length == 0)
					{
						//imageName = $"Image{PadLeft("0", Images.Count, 5)}";
						imageName = file.Name;
					}
					Trace.WriteLine($" {imageName}",
						$"{MessageImportanceEnum.Info}");
					bitmap = SKBitmap.Decode(file.FullName);
					bitmapA = new SKBitmap(bitmap.Width, bitmap.Height,
						SKColorType.Rgba8888, SKAlphaType.Premul);
					DrawBitmap(bitmap, bitmapA, SKPoint.Empty);
					bitmap.Dispose();
					bitmap = bitmapA;
					imageInfo = Images.FirstOrDefault(x => x.Name == imageName);
					if(imageInfo == null)
					{
						imageInfo = new BitmapInfoItem()
						{
							Name = imageName
						};
						Images.Add(imageInfo);
					}
					imageInfo.Bitmap = bitmap;
					if(ToBool(GetPropertyByName(item, "IsWorkingImage"), true))
					{
						WorkingImage = imageInfo;
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FileOverlayImage																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Open each image from the range and place the image specified in
		/// InputFilename at the options specified by Left, Top, Width, and Height.
		/// </summary>
		/// <param name="item">
		/// Reference to the item from which the item will be opened.
		/// </param>
		private static void FileOverlayImage(SvgActionItem item)
		{
			SKRect area = SKRect.Empty;
			bool bContinue = true;
			bool bRange = false;
			byte[] bytes = null;
			int height = 0;
			int left = 0;
			SKBitmap maskBitmap = null;
			FileInfo maskFile = null;
			List<string> names = null;
			SvgActionOptionItem optionHeight = null;
			SvgActionOptionItem optionLeft = null;
			SvgActionOptionItem optionMask = null;
			SvgActionOptionItem optionTop = null;
			SvgActionOptionItem optionWidth = null;
			SKSamplingOptions samplingOptions =
				new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);
			SKBitmap sourceBitmap = null;
			FileInfo sourceFile = null;
			List<string> sourceFilenames = new List<string>();
			int top = 0;
			int width = 0;

			if(item != null)
			{
				optionMask = GetOptionByName(item, "MaskFilename");
				if(optionMask != null)
				{
					maskFile = new FileInfo(optionMask.Value);
					if(!maskFile.Exists)
					{
						maskFile = null;
					}
				}
				optionLeft = GetOptionByName(item, "Left");
				optionTop = GetOptionByName(item, "Top");
				optionWidth = GetOptionByName(item, "Width");
				optionHeight = GetOptionByName(item, "Height");
				if(item.InputFiles.Count > 0)
				{
					bContinue = true;
				}
				else
				{
					bRange = bContinue = CheckElements(item,
						ActionElementEnum.InputFolderName |
						ActionElementEnum.Range);
				}
				if(bContinue &&
					maskFile != null &&
					optionLeft != null && optionTop != null &&
					optionWidth != null && optionHeight != null)
				{
					maskBitmap = SKBitmap.Decode(maskFile.FullName);
					left = ToInt(optionLeft.Value);
					top = ToInt(optionTop.Value);
					width = ToInt(optionWidth.Value);
					height = ToInt(optionHeight.Value);
					if(bRange)
					{
						//	Range-based.
						names = EnumerateRange(item.Range, item.Digits);
						foreach(string nameItem in names)
						{
							sourceFilenames.Add(
								Path.Combine(item.InputDir.FullName, nameItem));
						}
					}
					else
					{
						sourceFilenames = new List<string>();
						foreach(FileInfo fileInfoItem in item.InputFiles)
						{
							sourceFilenames.Add(fileInfoItem.FullName);
						}
					}
					if(sourceFilenames.Count > 0)
					{
						//	Source filenames were generated.
						foreach(string sourceFilenameItem in sourceFilenames)
						{
							sourceFile = new FileInfo(sourceFilenameItem);
							if(sourceFile.Exists)
							{
								bytes = File.ReadAllBytes(sourceFile.FullName);
								using(var ms = new MemoryStream(bytes))
								{
									sourceBitmap = SKBitmap.Decode(ms);
								}
								area =
									new SKRect(0, 0, sourceBitmap.Width, sourceBitmap.Height);
								DrawBitmap(maskBitmap, sourceBitmap, area);
								try
								{
									SaveBitmap(sourceBitmap, sourceFile.FullName);
									Trace.WriteLine(
										$" File saved: {Path.GetFileName(sourceFile.FullName)}",
										$"{MessageImportanceEnum.Info}");
								}
								catch(Exception ex)
								{
									Trace.WriteLine($"Error: {ex.Message}",
										$"{MessageImportanceEnum.Err}");
								}
								Trace.WriteLine($" {Path.GetFileName(sourceFilenameItem)}",
									$"{MessageImportanceEnum.Info}");
							}
						}
					}
					else
					{
						Trace.WriteLine(
							" Error: Source filenames could not be enumerated from the " +
							"given range.",
							$"{MessageImportanceEnum.Err}");
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FileSaveImage																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Save the working image to the specified OutputFilename.
		/// </summary>
		/// <param name="item">
		/// Reference to the item from which the item will be opened.
		/// </param>
		private static void FileSaveImage(SvgActionItem item)
		{
			FileInfo file = null;

			if(item != null && WorkingImage != null)
			{
				if(item.OutputFile != null)
				{
					file = item.OutputFile;
				}
				else if(item.CurrentFile != null && item.OutputDir != null)
				{
					file = new FileInfo(
						Path.Combine(item.OutputDir.FullName, item.CurrentFile.Name));
				}
				if(file != null)
				{
					AssureFolder(file.Directory.FullName, true, quiet: true);
					try
					{
						SaveBitmap(WorkingImage.Bitmap, file.FullName);
						Trace.WriteLine($" File saved: {file.Name}",
							$"{MessageImportanceEnum.Info}");
					}
					catch(Exception ex)
					{
						Trace.WriteLine($"Error: {ex.Message}",
							$"{MessageImportanceEnum.Err}");
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ForEachFile																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Run the Actions collection of the presented object through all of the
		/// files in this item's InputFiles collection using the CurrentFile
		/// property for each one.
		/// </summary>
		/// <param name="item">
		/// Reference to the file action item representing the loop base.
		/// </param>
		private static async void ForEachFile(SvgActionItem item)
		{
			if(item != null)
			{
				foreach(FileInfo fileItem in item.InputFiles)
				{
					item.CurrentFile = fileItem;
					await RunActions(item.Actions);
					//foreach(FileActionItem actionItem in item.Actions)
					//{
					//	await actionItem.Run();
					//}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetCurrentFile																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the current file.
		/// </summary>
		/// <param name="item">
		/// Reference to the file action for which the current file will be
		/// retrieved.
		/// </param>
		/// <returns>
		/// Reference to the current file in focus, if found. Otherwise, null.
		/// </returns>
		/// <remarks>
		/// If a file has been placed in focus by a loop or other method, that
		/// object will be returned from the CurrentFile property. Otherwise, the
		/// first item in InputFiles collection is returned.
		/// </remarks>
		private static FileInfo GetCurrentFile(SvgActionItem item)
		{
			FileInfo result = null;

			if(item != null)
			{
				//	An item has been provided.
				//	Test order.
				//	-	Local current file.
				//	- Local first item.
				//	- Parent current file.
				//	- Parent first item.
				if(item.mCurrentFile != null)
				{
					result = item.mCurrentFile;
				}
				else if(item.mInputFiles.Count > 0)
				{
					result = item.mInputFiles[0];
				}
				else
				{
					result = item.CurrentFile;
					if(result == null && item.InputFiles.Count > 0)
					{
						result = item.InputFiles[0];
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetPrecision																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the precision specified in the supplied item's Precision user
		/// property.
		/// </summary>
		/// <param name="item">
		/// Reference to the item containing the properties to check.
		/// </param>
		/// <returns>
		/// Decimal precision to use on the current item.
		/// </returns>
		private static int GetPrecision(SvgActionItem item)
		{
			int precision = 3;
			NameValueItem property = null;

			if(item != null)
			{
				property = item.Properties.FirstOrDefault(x =>
					x.Name.ToLower() == "precision");
				if(property != null)
				{
					precision = ToInt(property.Value);
					Trace.WriteLine($" Decimal Precision: {precision}",
						$"{MessageImportanceEnum.Info}");
				}
				else
				{
					Trace.WriteLine(" Precision property not specified. " +
						$"Defaulting to {precision}.",
						$"{MessageImportanceEnum.Info}");
				}
			}
			return precision;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IdentifyInputFiles																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Identify the input and output files and directories at the current
		/// level for the specified item.
		/// </summary>
		/// <param name="item">
		/// Reference to the item being fulfilled.
		/// </param>
		/// <remarks>
		/// <para>
		/// When this method is called, make sure that the InitializeLevels
		/// method has already been called.
		/// </para>
		/// <para>
		/// Only call CheckElements after first calling IdentifyFiles. That
		/// method relies on the file objects in this version.
		/// </para>
		/// <para>
		/// In this version, only the local InputFiles group is resolved. If you
		/// want to use a globally resolvable template, implement a user property
		/// containing that template name and set the InputFilename, etc., property
		/// at the site with a reference to that custom property.
		/// </para>
		/// </remarks>
		private static void IdentifyInputFiles(SvgActionItem item)
		{
			DirectoryInfo dir = null;
			FileInfo file = null;
			string filename = "";
			bool result = true;

			if(item != null && item.mInputNames.Count > 0)
			{
				//	Working path.
				if(item.WorkingPath.Length > 0)
				{
					dir = new DirectoryInfo(
						GetPropertyByName(item, nameof(WorkingPath)));
					if(!dir.Exists)
					{
						Trace.WriteLine(" Error: Working path does not exist.",
							$"{MessageImportanceEnum.Err}");
						result = false;
					}
					else if((dir.Attributes & FileAttributes.Directory) !=
						FileAttributes.Directory)
					{
						Trace.WriteLine(
							" Error: A file was specified as the working directory.",
							$"{MessageImportanceEnum.Err}");
						result = false;
					}
				}

				//	Input.
				item.mInputDir = null;
				item.mInputFiles.Clear();

				if(result)
				{
					if(item.mInputNames.Count > 0)
					{
						//	Input files are present.
						foreach(string filenameItem in item.mInputNames)
						{
							filename = AbsolutePath(
								GetPropertyByName(item, nameof(WorkingPath)),
								NormalizeValue(item, filenameItem));
							if(filename.Length > 0)
							{
								//	A filename has been retrieved.
								//	Check for wildcards and resolve variables.
								item.mInputFiles.AddRange(
									ResolveFilename(filename, false));
							}
						}
						if(item.mInputFiles.Count > 0)
						{
							file = item.mInputFiles[0];
							if((file.Attributes & FileAttributes.Directory) ==
								(FileAttributes)0)
							{
								//	This item is a file.
								item.mInputDir = new DirectoryInfo(file.Directory.FullName);
							}
							else
							{
								//	This item is a directory.
								item.mInputDir = new DirectoryInfo(file.FullName);
							}
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IdentifyOutputFiles																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Identify the output files and directories at the current
		/// level for the specified item.
		/// </summary>
		/// <param name="item">
		/// Reference to the item being fulfilled.
		/// </param>
		/// <remarks>
		/// <para>
		/// When this method is called, make sure that the InitializeLevels
		/// method has already been called.
		/// </para>
		/// <para>
		/// Only call CheckElements after first calling IdentifyFiles. That
		/// method relies on the file objects in this version.
		/// </para>
		/// </remarks>
		private static void IdentifyOutputFiles(SvgActionItem item)
		{
			DirectoryInfo dir = null;
			FileInfo file = null;
			string filename = "";
			List<FileInfo> files = null;
			bool result = true;

			if(item != null)
			{
				//	Working path.
				if(item.WorkingPath.Length > 0)
				{
					dir = new DirectoryInfo(
						GetPropertyByName(item, nameof(WorkingPath)));
					if(!dir.Exists)
					{
						Trace.WriteLine(" Error: Working path does not exist.",
							$"{MessageImportanceEnum.Err}");
						result = false;
					}
					else if((dir.Attributes & FileAttributes.Directory) !=
						FileAttributes.Directory)
					{
						Trace.WriteLine(
							" Error: A file was specified as the working directory.",
							$"{MessageImportanceEnum.Err}");
						result = false;
					}
				}

				//	Output.
				item.OutputDir = null;
				item.OutputFile = null;

				if(result)
				{
					if(item.OutputName?.Length > 0 && item.IsOutputLocal())
					{
						//	Output folder or file is present.
						files = new List<FileInfo>();
						filename = AbsolutePath(
							GetPropertyByName(item, nameof(WorkingPath)),
							GetPropertyByName(item, nameof(OutputName)));
						files.AddRange(ResolveFilename(filename, true));
						if(files.Count > 0)
						{
							file = files[0];
							if((!file.Exists && file.Extension.Length > 0) ||
								(file.Exists &&
								((file.Attributes & FileAttributes.Directory) ==
								(FileAttributes)0)))
							{
								//	This item is a file.
								item.OutputFile = file;
								item.OutputDir = new DirectoryInfo(file.Directory.FullName);
							}
							else
							{
								//	This item is a directory.
								item.OutputDir = new DirectoryInfo(file.FullName);
							}
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* If																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Run one or more sets of actions if their conditions are true.
		/// </summary>
		/// <param name="item">
		/// Reference to the action item for which this action is being
		/// called.
		/// </param>
		private static async void If(SvgActionItem item)
		{
			bool bMatch = false;
			ConditionCollection conditions = null;
			bool conditionResult = false;
			ExpressionContext context;
			IDynamicExpression dynCondition = null;

			if(item != null)
			{
				context = new ExpressionContext();
				//// Allow the expression to use all static public methods of
				//// System.Math.
				//context.Imports.AddType(typeof(Math));
				context.Variables["CurrentFilename"] = item.CurrentFile.Name;
				context.Variables["CurrentFileNumber"] =
					GetIndexValue(item.CurrentFile.Name);

				foreach(SvgActionItem actionItem in item.Actions)
				{
					if(!actionItem.Options.Exists(x => x.Name.ToLower() == "mute"))
					{
						conditions = GetConditions(actionItem);
						bMatch = true;
						foreach(ConditionItem conditionItem in conditions)
						{
							dynCondition = context.CompileDynamic(conditionItem.Condition);
							conditionResult = (bool)dynCondition.Evaluate();
							if(!conditionResult)
							{
								bMatch = false;
								break;
							}
						}
						if(bMatch)
						{
							//	This item evaluates to true. Run its actions.
							await RunActions(actionItem.Actions);
							//foreach(FileActionItem trueActionItem in actionItem.Actions)
							//{
							//	await trueActionItem.Run();
							//}
						}
					}
					else
					{
						Trace.WriteLine($"Action {actionItem.Action} is muted...",
							$"{MessageImportanceEnum.Info}");
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ImageBackground																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the background color or image on the working image.
		/// </summary>
		/// <param name="item">
		/// Reference to the file action item for which this action is being
		/// called.
		/// </param>
		private static void ImageBackground(SvgActionItem item)
		{
			string backgroundColor = "";
			SKBitmap backgroundBitmap = null;
			string backgroundFilename = "";
			SKBitmap bitmap = null;
			SKRect rectSource = SKRect.Empty;
			SKRect rectTarget = SKRect.Empty;
			SKSamplingOptions samplingOptions =
				new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear);
			int height = 0;
			int width = 0;

			if(item != null && WorkingImage?.Bitmap != null)
			{
				//	An item was presented and a working image is present.
				width = WorkingImage.Bitmap.Width;
				height = WorkingImage.Bitmap.Height;
				rectTarget = new SKRect(0, 0, width, height);
				backgroundBitmap = new SKBitmap(width, height);

				//	Set the color first.
				backgroundColor = GetPropertyByName(item, "BackgroundColor");
				if(backgroundColor.Length > 0)
				{
					using(SKCanvas canvas = new SKCanvas(WorkingImage.Bitmap))
					{
						using(SKPaint paint = new SKPaint())
						{
							paint.Color = SKColor.Parse(backgroundColor);
							paint.Style = SKPaintStyle.Fill;
							canvas.DrawRect(rectTarget, paint);
						}
					}
				}
				//	Check for image.
				backgroundFilename = GetPropertyByName(item, "BackgroundImage");
				if(backgroundFilename.Length > 0)
				{
					bitmap = SKBitmap.Decode(
						AbsolutePath(item.WorkingPath, backgroundFilename));
					DrawBitmap(bitmap, WorkingImage.Bitmap, rectSource, rectTarget);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ImagesClear																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Clear the contents of the Images collection.
		/// </summary>
		/// <param name="item">
		/// Reference to the file action item for which this action is being
		/// called.
		/// </param>
		/// <remarks>
		/// This method also clears the WorkingImage property.
		/// </remarks>
		private static void ImagesClear(SvgActionItem item)
		{
			if(item != null)
			{
				Images.Clear();
				WorkingImage = null;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ImpliedDesignEnumerateControls																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Enumerate the controls in the caller's implied form design file.
		/// </summary>
		/// <param name="item">
		/// Reference to the item to be intialized.
		/// </param>
		private static void ImpliedDesignEnumerateControls(SvgActionItem item)
		{
			ControlAreaCollection areas = null;
			string content = "";
			SvgDocumentItem doc = null;

			if(item != null)
			{
				if(CheckElements(item,
					ActionElementEnum.InputFilename))
				{
					//	Load the document if the filename was specified.
					content = File.ReadAllText(item.InputFiles[0].FullName);
					doc = new SvgDocumentItem(content);
					SvgToolsUtil.ApplyTransforms(doc.Document);
					SvgToolsUtil.RoundAllValues(doc.Document, 0);
					item.WorkingSvg = doc;
					Trace.WriteLine($" Working document: {item.InputFiles[0].Name}",
						$"{MessageImportanceEnum.Info}");
					areas = ImpliedFormDesign.EnumerateControls(doc);
					if(areas?.Count > 0)
					{
						ControlAreaCollection.Dump(areas, 1);
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ImpliedDesignToAvaloniaXaml																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Translate the provided implied form design file to Avalonia AXAML.
		/// </summary>
		/// <param name="item">
		/// Reference to the item to be intialized.
		/// </param>
		private static void ImpliedDesignToAvaloniaXaml(SvgActionItem item)
		{
			string content = "";
			SvgDocumentItem doc = null;
			ImpliedFormDesignAXaml formDesign = null;

			if(item != null)
			{
				if(CheckElements(item,
					ActionElementEnum.InputFilename |
					ActionElementEnum.OutputFilename))
				{
					//	Load the document if the filename was specified.
					content = File.ReadAllText(item.InputFiles[0].FullName);
					doc = new SvgDocumentItem(content);
					HtmlDocument.RecalculateAbsoluteIndex(doc.Document);
					SvgToolsUtil.ApplyTransforms(doc.Document);
					SvgToolsUtil.RoundAllValues(doc.Document, 0);
					item.WorkingSvg = doc;
					Trace.WriteLine($" Working document: {item.InputFiles[0].Name}",
						$"{MessageImportanceEnum.Info}");
					formDesign = new ImpliedFormDesignAXaml(doc);
					content = formDesign.ToXaml();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* InitializeFilenames																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Initialize values for working at this and child levels.
		/// </summary>
		/// <param name="item">
		/// Reference to the item to be intialized.
		/// </param>
		/// <remarks>
		///	<para>When preparing the object for use:</para>
		///	<list type="bullet">
		///	<item>All input files at a level should be read from a single
		///	reference source. Check for an inputs collection.</item>
		///	<item>If blank, check for filename and add to inputs
		///	collection.</item>
		///	<item>If blank, check for foldername and add to inputs
		///	collection.</item>
		///	<item>All output files at a level should be written from a
		///	single reference source. Check for the Output.</item>
		///	<item>If blank, check for the output filename.</item>
		///	<item>If blank, check for the output foldername.</item>
		///	</list>
		///	<para>
		///	In this version, the conversion is made on every action at every level.
		///	</para>
		/// </remarks>
		private static void InitializeFilenames(SvgActionItem item)
		{
			if(item != null)
			{
				//	Input filenames.
				if(item.mInputNames.Count == 0 &&
					(item.mInputFilename?.Length > 0 ||
					item.mInputFolderName?.Length > 0))
				{
					//	The input names collection was not specified, but either a
					//	filename or foldername were provided.
					if(item.mInputFilename?.Length > 0)
					{
						//	An input filename was provided at this level.
						item.mInputNames.AddRange(
							ResolveWildcards(
								GetPropertyByName(item,
									nameof(WorkingPath)), item.mInputFilename));
						//item.mInputNames.Add(item.mInputFilename);
						item.mInputFilename = "";
					}
					if(item.mInputFolderName?.Length > 0)
					{
						//	An input foldername was provided at this level.
						item.mInputNames.Add(item.mInputFolderName);
						item.mInputFiles.Add(new FileInfo(item.mInputFolderName));
						//	DEP20240225.1102 - I don't remember the original reason
						//	for clearing this variable. Its raw value is needed in
						//	directory deletion routine.
						//	Be aware this may need to be uncommented.
						//item.mInputFolderName = "";
					}
				}
				//	Output filenames.
				if((item.mOutputName == null || item.mOutputName.Length == 0) &&
					(item.mOutputFilename?.Length > 0 ||
					item.mOutputFolderName?.Length > 0))
				{
					//	The output name was not specified and either an output filename
					//	or output foldername are present.
					if(item.mOutputFilename?.Length > 0)
					{
						//	An output filename was provided at this level.
						item.mOutputName = item.mOutputFilename;
					}
					else if(item.mOutputFolderName?.Length > 0)
					{
						//	An output folder name was provided at this level.
						item.mOutputName = item.mOutputFolderName;
					}
				}
				//	In this version, all child items are processed as they are
				//	encountered.
				////	Process all child levels.
				//foreach(FileActionItem actionItem in item.mActions)
				//{
				//	InitializeFilenames(actionItem);
				//}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* InitializeProperties																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Initialize the public properties list of this class so they can be
		/// used repeatedly with minimal overhead.
		/// </summary>
		private static void InitializeProperties()
		{
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
			PropertyInfo[] properties = null;

			if(mPublicProperties.Count == 0)
			{
				//	Only initialize once.
				properties = typeof(SvgActionItem).GetProperties(bindingFlags);
				foreach(PropertyInfo propertyInfoItem in properties)
				{
					mPublicProperties.Add(propertyInfoItem);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* NormalizeValue																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Normalize the filename, using the values of any local properties
		/// necessary.
		/// </summary>
		/// <param name="item">
		/// Reference to the item from which variables will be resolved.
		/// </param>
		/// <param name="value">
		/// Value to normalize.
		/// </param>
		/// <returns>
		/// Fully normalized version of the provided filename.
		/// </returns>
		private static string NormalizeValue(SvgActionItem item, string value)
		{
			MatchCollection matches = null;
			List<NameValueItem> replacements = new List<NameValueItem>();
			string result = "";

			if(value?.Length > 0)
			{
				result = value;
				matches = Regex.Matches(result, ResourceMain.rxEmbeddedFieldName);
				if(matches.Count > 0)
				{
					foreach(Match matchItem in matches)
					{
						if(!replacements.Exists(x =>
							x.Name == GetValue(matchItem, "field")))
						{
							replacements.Add(new NameValueItem()
							{
								Name = GetValue(matchItem, "field"),
								Value = GetPropertyByName(item, GetValue(matchItem, "name"))
							});
						}
					}
					foreach(NameValueItem replaceItem in replacements)
					{
						result = result.Replace(replaceItem.Name, replaceItem.Value);
					}
					//	Run at least one more time after having made replacements.
					result = NormalizeValue(item, result);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OpenWorkingSvg																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Open the working SVG file to allow multiple operations to be completed
		/// in the same session.
		/// </summary>
		/// <param name="item">
		/// Reference to the action item containing information about the file to
		/// open.
		/// </param>
		private static void OpenWorkingSvg(SvgActionItem item)
		{
			string content = "";
			SvgDocumentItem doc = null;

			if(item != null)
			{
				if(CheckElements(item,
					ActionElementEnum.InputFilename))
				{
					//	Load the document if the filename was specified.
					content = File.ReadAllText(item.InputFiles[0].FullName);
					doc = new SvgDocumentItem(content);
					item.WorkingSvg = doc;
					Trace.WriteLine($" Working document: {item.InputFiles[0].Name}",
						$"{MessageImportanceEnum.Info}");
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* PurgeDefs																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Remove unreferenced elements from the defs section.
		/// </summary>
		/// <param name="item">
		/// Reference to the action item specifying the file to be activated.
		/// </param>
		private static void PurgeDefs(SvgActionItem item)
		{
			string content = "";
			SvgDocumentItem doc = null;

			if(item != null)
			{
				doc = GetSpecifiedOrWorking(item);
				if(doc != null)
				{
					SvgToolsUtil.PurgeDefs(doc.Document);
					if(doc.IsLocal)
					{
						//	Per-file mode is active.
						content = doc.Document.Html;
						File.WriteAllText(item.OutputFile.FullName, content);
						Trace.WriteLine($" File written: {item.OutputFile.Name}",
							$"{MessageImportanceEnum.Info}");
					}
				}
				else
				{
					Trace.WriteLine($" Error: Input files were not specified.",
						$"{MessageImportanceEnum.Err}");
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RoundAllValues																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Round all values in the loaded or specified file to a given decimal
		/// point precision.
		/// </summary>
		/// <param name="item">
		/// Reference to the action item to process.
		/// </param>
		private static void RoundAllValues(SvgActionItem item)
		{
			string content = "";
			SvgDocumentItem doc = null;
			int precision = 0;

			if(item != null)
			{
				doc = GetSpecifiedOrWorking(item);
				if(doc != null)
				{
					precision = GetPrecision(item);
					SvgToolsUtil.RoundAllValues(doc.Document, precision);
					if(doc.IsLocal)
					{
						//	Per-file mode is active.
						content = doc.Document.Html;
						File.WriteAllText(item.OutputFile.FullName, content);
						Trace.WriteLine($" File written: {item.OutputFile.Name}",
							$"{MessageImportanceEnum.Info}");
					}
				}
				else
				{
					Trace.WriteLine($" Error: Input files were not specified.",
						$"{MessageImportanceEnum.Err}");
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RunActions																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Run all of the unmuted actions in the collection.
		/// </summary>
		/// <param name="actions">
		/// Reference to a collection of file actions to run.
		/// </param>
		/// <returns>
		/// Reference to the asynchronous task that was launched.
		/// </returns>
		private static async Task RunActions(SvgActionCollection actions)
		{
			if(actions?.Count > 0)
			{
				foreach(SvgActionItem actionItem in actions)
				{
					if(!actionItem.Options.Exists(x => x.Name.ToLower() == "mute"))
					{
						await actionItem.Run();
						if(actionItem.Stop)
						{
							Trace.WriteLine("Batch stopped...",
								$"{MessageImportanceEnum.Info}");
							break;
						}
					}
					else
					{
						Trace.WriteLine(
							$"Action {actionItem.Action} is muted...",
							$"{MessageImportanceEnum.Info}");
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RunSequence																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Run the specified sequence.
		/// </summary>
		/// <param name="item">
		/// Reference to the file action that specifies the sequence to run.
		/// </param>
		/// <remarks>
		/// This method loads the Actions collection from the steps found in
		/// the referenced sequence.
		/// </remarks>
		private static async void RunSequence(SvgActionItem item)
		{
			string name = "";
			SequenceItem sequence = null;

			if(item != null)
			{
				name = GetPropertyByName(item, "SequenceName");
				if(name?.Length > 0)
				{
					Trace.WriteLine($" {name}", $"{MessageImportanceEnum.Info}");
					sequence =
						item.Sequences.FirstOrDefault(x => x.SequenceName == name);
					if(sequence != null)
					{
						//	Copy all of the actions.
						foreach(SvgActionItem actionItem in sequence.Actions)
						{
							item.Actions.Add(DeepCopy(actionItem));
						}
						item.Actions.Parent = item;
						//	Run each action.
						await RunActions(item.Actions);
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SaveWorkingSvg																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Save the working SVG file to the specified output file.
		/// </summary>
		/// <param name="item">
		/// Reference to the item where the file action is defined.
		/// </param>
		private static void SaveWorkingSvg(SvgActionItem item)
		{
			string content = "";

			if(item != null)
			{
				if(CheckElements(item, ActionElementEnum.OutputFilename))
				{
					if(item.WorkingSvg?.Document != null)
					{
						content = item.WorkingSvg.Document.Html;
						File.WriteAllText(item.OutputFile.FullName, content);
						Trace.WriteLine($" SVG file written: {item.OutputFile.Name}",
							$"{MessageImportanceEnum.Info}");
					}
					else
					{
						Trace.WriteLine($" Error: No working SVG file is open...",
							$"{MessageImportanceEnum.Err}");
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetWorkingImage																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the working image to the one specified by the user property
		/// ImageName.
		/// </summary>
		/// <param name="item">
		/// Reference to the action item specifying the file to be activated.
		/// </param>
		private static void SetWorkingImage(SvgActionItem item)
		{
			BitmapInfoItem imageInfo = null;
			string imageName = "";

			WorkingImage = null;
			if(item != null)
			{
				imageName = GetPropertyByName(item, "ImageName");
				if(imageName?.Length > 0)
				{
					//	Image was specified.
					Trace.WriteLine($" Working Image: {imageName}",
						$"{MessageImportanceEnum.Info}");
					imageInfo = Images.FirstOrDefault(x => x.Name == imageName);
					if(imageInfo != null)
					{
						WorkingImage = imageInfo;
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SizeImage																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Scale the working image to a new size to the dimensions found in the
		/// Width and Height user properties.
		/// </summary>
		/// <param name="item">
		/// Reference to the action item describing the new size to use on
		/// the working image.
		/// </param>
		private static void SizeImage(SvgActionItem item)
		{
			SKBitmap bitmap = null;
			int height = 0;
			SKRect targetRect = SKRect.Empty;
			int width = 0;

			if(item != null && WorkingImage != null)
			{
				//	The item and the working image are both present.
				width = ToInt(GetPropertyByName(item, "Width"));
				height = ToInt(GetPropertyByName(item, "Height"));
				if(width > 0 && height > 0)
				{
					//	Dimensions were supplied.
					Trace.WriteLine($" {width}, {height}",
						$"{MessageImportanceEnum.Info}");
					targetRect = new SKRect(0, 0, width, height);
					bitmap = new SKBitmap((int)targetRect.Width, (int)targetRect.Height);
					DrawBitmap(WorkingImage.Bitmap, bitmap, targetRect);
					WorkingImage.Bitmap.Dispose();
					WorkingImage.Bitmap = bitmap;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SortSymbols																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Sort the symbols in the defs section of the specified SVG.
		/// </summary>
		/// <param name="item">
		/// Reference to the action item to process.
		/// </param>
		private static void SortSymbols(SvgActionItem item)
		{
			string content = "";
			HtmlNodeItem defs = null;
			SvgDocumentItem doc = null;
			List<HtmlNodeItem> symbols = null;

			if(item != null)
			{
				doc = GetSpecifiedOrWorking(item);
				if(doc != null)
				{
					defs = doc.Document.Nodes.FindMatch(x => x.NodeType == "defs");
					if(defs != null)
					{
						symbols = defs.Nodes.FindMatches(x => x.NodeType == "symbol").
							OrderBy(y => y.Attributes.GetValue("id")).ToList();
						if(symbols.Count > 0)
						{
							foreach(HtmlNodeItem symbolItem in symbols)
							{
								//HtmlNodeCollection.AbsorbPretextBlanks(defs.Nodes, symbolItem);
								defs.Nodes.Remove(symbolItem);
								defs.Nodes.Add(symbolItem);
							}
							if(doc.IsLocal)
							{
								//	Per-file mode is active.
								content = doc.Document.Html;
								File.WriteAllText(item.OutputFile.FullName, content);
								Trace.WriteLine($" File written: {item.OutputFile.Name}",
									$"{MessageImportanceEnum.Info}");
							}
						}
					}
				}
				else
				{
					Trace.WriteLine($" Error: Input files were not specified.",
						$"{MessageImportanceEnum.Err}");
				}
			}
		}
		//*-----------------------------------------------------------------------*

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
		/// Create a new instance of the SvgActionItem Item.
		/// </summary>
		public SvgActionItem()
		{
			InitializeProperties();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Action																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Action">Action</see>.
		/// </summary>
		private SvgActionTypeEnum mAction = SvgActionTypeEnum.None;
		/// <summary>
		/// Get/Set the action associated with this entry.
		/// </summary>
		/// <remarks>
		/// This property is non-inheritable.
		/// </remarks>
		[JsonConverter(typeof(StringEnumConverter))]
		public SvgActionTypeEnum Action
		{
			get { return mAction; }
			set { mAction = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Actions																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Actions">Actions</see>.
		/// </summary>
		private SvgActionCollection mActions = new SvgActionCollection();
		/// <summary>
		/// Get a reference to the collection of child SVG actions.
		/// </summary>
		/// <remarks>
		/// This property is non-inheritable.
		/// </remarks>
		public SvgActionCollection Actions
		{
			get { return mActions; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Base																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Base">Base</see>.
		/// </summary>
		private string mBase = null;
		/// <summary>
		/// Get/Set the base number or filename pattern of the source or target
		/// files, depending upon the action.
		/// </summary>
		/// <remarks>
		/// This property is inheritable.
		/// </remarks>
		public string Base
		{
			get
			{
				string result = mBase;

				if(result == null)
				{
					if(mParent != null)
					{
						result = mParent.GetBase();
					}
					else
					{
						result = "";
					}
				}
				return result;
			}
			set { mBase = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Conditions																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Conditions">Conditions</see>.
		/// </summary>
		private ConditionCollection mConditions = new ConditionCollection();
		/// <summary>
		/// Get a reference to the collection of conditions assigned to this
		/// action.
		/// </summary>
		/// <remarks>
		/// This property is not inheritable. However, properties from parent
		/// levels are retrieved when calling the GetConditions function.
		/// </remarks>
		public ConditionCollection Conditions
		{
			get { return mConditions; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ConfigFilename																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ConfigFilename">ConfigFilename</see>.
		/// </summary>
		private string mConfigFilename = "";
		/// <summary>
		/// Get/Set the path and filename of the configuration file for this
		/// action.
		/// </summary>
		/// <remarks>
		/// This property is non-inheritable.
		/// </remarks>
		public string ConfigFilename
		{
			get { return mConfigFilename; }
			set { mConfigFilename = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Count																																	*
		//*-----------------------------------------------------------------------*
		private float mCount = float.MinValue;
		/// <summary>
		/// Get/Set the count associated with the current action.
		/// </summary>
		/// <remarks>
		/// This property is inheritable.
		/// </remarks>
		public float Count
		{
			get
			{
				float result = mCount;

				if(result == float.MinValue)
				{
					if(mParent != null)
					{
						result = mParent.GetCount();
					}
					else
					{
						result = 0f;
					}
				}
				return result;
			}
			set { mCount = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	CurrentFile																														*
		//*-----------------------------------------------------------------------*
		private FileInfo mCurrentFile = null;
		/// <summary>
		/// Get/Set a reference to the current active file in-use.
		/// </summary>
		/// <remarks>
		/// This property is inheritable.
		/// </remarks>
		[JsonIgnore]
		public FileInfo CurrentFile
		{
			get
			{
				FileInfo result = mCurrentFile;

				if(result == null)
				{
					if(mParent != null)
					{
						result = mParent.GetCurrentFile();
					}
				}
				return result;
			}
			set { mCurrentFile = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	DateTimeValue																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="DateTimeValue">DateTimeValue</see>.
		/// </summary>
		private DateTime mDateTimeValue = DateTime.MinValue;
		/// <summary>
		/// Get/Set the date and time associated with the current action.
		/// </summary>
		/// <remarks>
		/// <para>This property is inheritable.</para>
		/// <para>Corresponds with the command-line parameter 'DateTime'.</para>
		/// </remarks>
		public DateTime DateTimeValue
		{
			get
			{
				DateTime result = mDateTimeValue;

				if(result == DateTime.MinValue && mParent != null)
				{
					result = mParent.GetDateTimeValue();
				}
				return result;
			}
			set { mDateTimeValue = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* DeepCopy																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a new instance of the provided item.
		/// </summary>
		/// <param name="item">
		/// Reference to the item to be copied.
		/// </param>
		/// <returns>
		/// Reference to a completely new instance of the file action item
		/// provided by the caller.
		/// </returns>
		public static SvgActionItem DeepCopy(SvgActionItem item)
		{
			string content = "";
			SvgActionItem result = null;

			if(item != null)
			{
				content = JsonConvert.SerializeObject(item);
				result = JsonConvert.DeserializeObject<SvgActionItem>(content);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Digits																																*
		//*-----------------------------------------------------------------------*
		private int mDigits = int.MinValue;
		/// <summary>
		/// Get/Set the number of digits associated with the current action.
		/// </summary>
		/// <remarks>
		/// This property is inheritable.
		/// </remarks>
		public int Digits
		{
			get
			{
				int result = mDigits;

				if(result == int.MinValue)
				{
					if(mParent != null)
					{
						result = mParent.GetDigits();
					}
					else
					{
						result = 0;
					}
				}
				return result;
			}
			set { mDigits = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetConditions																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a collection of conditions defined at the caller's item level
		/// and at all of its parents.
		/// </summary>
		/// <param name="item">
		/// Reference to the file action item to inspect.
		/// </param>
		/// <returns>
		/// Reference to a collection of all conditions defined at the current and
		/// baser levels.
		/// </returns>
		public static ConditionCollection GetConditions(SvgActionItem item)
		{
			ConditionCollection conditions = null;
			ConditionCollection result = new ConditionCollection();

			if(item != null)
			{
				if(item.Parent != null && item.Parent.Parent != null)
				{
					conditions = GetConditions(item.Parent.Parent);
					foreach(ConditionItem conditionItem in conditions)
					{
						result.Add(conditionItem);
					}
				}
				//	Write the local items last.
				foreach(ConditionItem conditionItem in item.Conditions)
				{
					result.Add(conditionItem);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetOptionByName																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the option specified by name from this or a parent entity.
		/// </summary>
		/// <param name="item">
		/// Reference to the item for which the option will be found.
		/// </param>
		/// <param name="optionName">
		/// Name of the option to retrieve.
		/// </param>
		/// <returns>
		/// Reference to the specified option, if found. Otherwise, null.
		/// </returns>
		public static SvgActionOptionItem GetOptionByName(SvgActionItem item,
			string optionName)
		{
			SvgActionOptionItem result = null;

			if(item != null && optionName?.Length > 0)
			{
				result = item.Options.FirstOrDefault(x =>
					x.Name.ToLower() == optionName.ToLower());
				if(result == null && item.mParent != null)
				{
					result =
						SvgActionCollection.GetOptionByName(item.mParent, optionName);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetPropertyByName																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the user property specified by name from this or a parent
		/// entity.
		/// </summary>
		/// <param name="item">
		/// Reference to the item for which the property will be retrieved.
		/// </param>
		/// <param name="propertyName">
		/// Name of the property to retrieve.
		/// </param>
		/// <param name="resolveVariables">
		/// Value indicating whether to resolve variables on this call.
		/// </param>
		/// <returns>
		/// Reference to the specified property, if found. Otherwise, null.
		/// </returns>
		public static string GetPropertyByName(SvgActionItem item,
			string propertyName, bool resolveVariables = true)
		{
			PropertyInfo propertySystem = null;
			NameValueItem propertyUser = null;
			object propertyValue = null;
			string result = "";

			if(item != null && propertyName?.Length > 0)
			{
				propertySystem =
					mPublicProperties.FirstOrDefault(x => x.Name.ToLower() ==
					propertyName.ToLower());
				if(propertySystem != null)
				{
					//	Built-in property.
					propertyValue = propertySystem.GetValue(item);
					if(propertyValue != null)
					{
						result = propertyValue.ToString();
					}
				}
				else
				{
					//	User property.
					propertyUser = item.Properties.FirstOrDefault(x =>
						x.Name.ToLower() == propertyName.ToLower());
					if(propertyUser != null)
					{
						result = propertyUser.Value;
					}
					else if(item.mParent != null && item.mParent.Parent != null)
					{
						result = GetPropertyByName(item.mParent.Parent,
							propertyName, false);
					}
				}
				if(result.Length > 0 && resolveVariables)
				{
					result = NormalizeValue(item, result);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetSpecifiedOrWorking																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a reference either to the SVG specified in the local file
		/// arguments or the previously loaded working document.
		/// </summary>
		/// <param name="item">
		/// Reference to the action item within which the file arguments will
		/// be found.
		/// </param>
		/// <returns>
		/// Reference to the SVG document found, if successul. Otherwise, null.
		/// </returns>
		private static SvgDocumentItem GetSpecifiedOrWorking(SvgActionItem item)
		{
			string content = "";
			SvgDocumentItem doc = null;

			if(item != null)
			{
				if(CheckElements(item,
					ActionElementEnum.InputFilename |
					ActionElementEnum.OutputFilename,
					includeInherited: false, quiet: true))
				{
					//	Just load the document if the filenames were specified.
					content = File.ReadAllText(item.InputFiles[0].FullName);
					doc = new SvgDocumentItem(content);
				}
				else
				{
					doc = item.WorkingSvg;
				}
			}
			return doc;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Images																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Images">Images</see>.
		/// </summary>
		private static BitmapInfoCollection mImages = new BitmapInfoCollection();
		/// <summary>
		/// Get a reference to the collection of images in this session.
		/// </summary>
		[JsonIgnore]
		public static BitmapInfoCollection Images
		{
			get { return mImages; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	InputDir																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="InputDir">InputDir</see>.
		/// </summary>
		private DirectoryInfo mInputDir = null;
		/// <summary>
		/// Get/Set the internal, calculated input directory.
		/// </summary>
		/// <remarks>
		/// This property is non-inerhitable.
		/// </remarks>
		[JsonIgnore]
		public DirectoryInfo InputDir
		{
			get { return mInputDir; }
			set { mInputDir = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	InputFilename																													*
		//*-----------------------------------------------------------------------*
		private string mInputFilename = null;
		/// <summary>
		/// Get/Set the input path and filename of the input file.
		/// </summary>
		/// <remarks>
		/// <para>This property is inheritable.</para>
		/// <para>Corresponds with the command-line parameter 'InFile'.</para>
		/// </remarks>
		public string InputFilename
		{
			get
			{
				string result = mInputFilename;

				if(result == null)
				{
					if(mParent != null)
					{
						result = mParent.GetInputFilename();
					}
					else
					{
						result = "";
					}
				}
				return result;
			}
			set { mInputFilename = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	InputFiles																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="InputFiles">InputFiles</see>.
		/// </summary>
		private List<FileInfo> mInputFiles = new List<FileInfo>();
		/// <summary>
		/// Get a reference to the collection of file information used as input in
		/// this session.
		/// </summary>
		/// <remarks>
		/// This property is inheritable.
		/// </remarks>
		[JsonIgnore]
		public List<FileInfo> InputFiles
		{
			get
			{
				List<FileInfo> result = mInputFiles;

				if(result.Count == 0 && mParent != null)
				{
					result = mParent.GetInputFiles();
				}
				return result;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	InputFolderName																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="InputFolderName">InputFolderName</see>.
		/// </summary>
		private string mInputFolderName = null;
		/// <summary>
		/// Get/Set the path and folder name of the input for this action.
		/// </summary>
		/// <remarks>
		/// <para>This property is inheritable.</para>
		/// <para>Corresponds with the command-line parameter 'InFolder'.</para>
		/// </remarks>
		public string InputFolderName
		{
			get
			{
				string result = mInputFolderName;

				if(result == null)
				{
					if(mParent != null)
					{
						result = mParent.GetInputFolderName();
					}
					else
					{
						result = "";
					}
				}
				return result;
			}
			set { mInputFolderName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	InputNames																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="InputNames">InputNames</see>.
		/// </summary>
		private List<string> mInputNames = new List<string>();
		/// <summary>
		/// Get a reference to the list of filenames or foldernames with
		/// or without wildcards. This parameter can be specified multiple times
		/// on the command line with different values to load multiple input files.
		/// </summary>
		/// <remarks>
		/// <para>This property is inheritable.</para>
		/// <para>Corresponds with the command-line parameter 'Inputs'.</para>
		/// </remarks>
		public List<string> InputNames
		{
			get
			{
				List<string> result = mInputNames;

				if(result.Count == 0 && mParent != null)
				{
					//	If the local list is not overridden, then default to the
					//	parent.
					result = mParent.GetInputNames();
				}
				return result;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsOutputLocal																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the output filenames are local at
		/// this level.
		/// </summary>
		/// <returns>
		/// True if an output filename has been specified at this level.
		/// </returns>
		public bool IsOutputLocal()
		{
			return (mOutputName?.Length > 0);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Message																																*
		//*-----------------------------------------------------------------------*
		private string mMessage = "";
		/// <summary>
		/// Get/Set a message to be displayed when this action is run.
		/// </summary>
		public string Message
		{
			get { return mMessage; }
			set { mMessage = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Options																																*
		//*-----------------------------------------------------------------------*
		private SvgActionOptionCollection mOptions =
			new SvgActionOptionCollection();
		/// <summary>
		/// Get a reference to the collection of options assigned to this action.
		/// </summary>
		/// <remarks>
		/// This property is not inheritable. However, options from parent levels
		/// are retrieved when calling the GetOptionByName function.
		/// </remarks>
		public SvgActionOptionCollection Options
		{
			get { return mOptions; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	OutputFile																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="OutputFile">OutputFile</see>.
		/// </summary>
		private FileInfo mOutputFile = null;
		/// <summary>
		/// Get/Set the internal, calculated output file.
		/// </summary>
		/// <remarks>
		/// This property is non-inheritable.
		/// </remarks>
		[JsonIgnore]
		public FileInfo OutputFile
		{
			get { return mOutputFile; }
			set { mOutputFile = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	OutputDir																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="OutputDir">OutputDir</see>.
		/// </summary>
		private DirectoryInfo mOutputDir = null;
		/// <summary>
		/// Get/Set the internal, calculated output directory.
		/// </summary>
		/// <remarks>
		/// This property is inheritable.
		/// </remarks>
		[JsonIgnore]
		public DirectoryInfo OutputDir
		{
			get
			{
				DirectoryInfo directory = mOutputDir;

				if(directory == null && mParent != null && mParent != null)
				{
					directory = mParent.Parent.OutputDir;
				}
				return directory;
			}
			set { mOutputDir = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	OutputFilename																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="OutputFilename">OutputFilename</see>.
		/// </summary>
		private string mOutputFilename = null;
		/// <summary>
		/// Get/Set the output path and filename for this action.
		/// </summary>
		/// <remarks>
		/// <para>This property is inheritable.</para>
		/// <para>Corresponds with the command-line parameter 'OutFile'.</para>
		/// </remarks>
		public string OutputFilename
		{
			get
			{
				string result = mOutputFilename;

				if(result == null)
				{
					if(mParent != null)
					{
						result = mOutputFilename;
					}
					else
					{
						result = "";
					}
				}
				return result;
			}
			set { mOutputFilename = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	OutputFolderName																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="OutputFolderName">OutputFolderName</see>.
		/// </summary>
		private string mOutputFolderName = null;
		/// <summary>
		/// Get/Set the output path and folder name for this action.
		/// </summary>
		/// <remarks>
		/// <para>This property is inheritable.</para>
		/// <para>Corresponds with the command-line parameter 'OutFolder'.</para>
		/// </remarks>
		public string OutputFolderName
		{
			get
			{
				string result = mOutputFolderName;

				if(result == null)
				{
					if(mParent != null)
					{
						result = mParent.GetOutputFolderName();
					}
					else
					{
						result = "";
					}
				}
				return result;
			}
			set { mOutputFolderName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	OutputName																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="OutputName">OutputName</see>.
		/// </summary>
		private string mOutputName = null;
		/// <summary>
		/// Get/Set an output pattern that allows for filenames or foldernames
		/// with or without wildcards. This parameter can be specified muliple
		/// times on the command line with different values to write to multiple
		/// output files.
		/// </summary>
		/// <remarks>
		/// <para>This property is inheritable.</para>
		/// <para>Corresponds with the command-line parameter 'Output'.</para>
		/// </remarks>
		public string OutputName
		{
			get
			{
				string result = mOutputName;

				if(result == null)
				{
					if(mParent != null)
					{
						result = mParent.GetOutputName();
					}
					else
					{
						result = "";
					}
				}
				return result;
			}
			set { mOutputName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	OutputType																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="OutputType">OutputType</see>.
		/// </summary>
		private RenderFileTypeEnum mOutputType = RenderFileTypeEnum.None;
		/// <summary>
		/// Get/Set the type of rendering to be done on the file affected by this
		/// action.
		/// </summary>
		/// <remarks>
		/// This property is inheritable.
		/// </remarks>
		public RenderFileTypeEnum OutputType
		{
			get
			{
				RenderFileTypeEnum result = mOutputType;

				if(result == RenderFileTypeEnum.None)
				{
					if(mParent != null)
					{
						result = mParent.GetOutputType();
					}
					else
					{
						result = RenderFileTypeEnum.Auto;
					}
				}
				return mOutputType;
			}
			set { mOutputType = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Parent																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Parent">Parent</see>.
		/// </summary>
		private SvgActionCollection mParent = null;
		/// <summary>
		/// Get/Set a reference to the parent of this item.
		/// </summary>
		/// <remarks>
		/// This property is non-inheritable.
		/// </remarks>
		[JsonIgnore]
		public SvgActionCollection Parent
		{
			get { return mParent; }
			set { mParent = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Pattern																																*
		//*-----------------------------------------------------------------------*
		private string mPattern = null;
		/// <summary>
		/// Get/Set a regular expression pattern for files, folders, or other
		/// appropriate strings.
		/// </summary>
		/// <remarks>
		/// This property is inheritable.
		/// </remarks>
		public string Pattern
		{
			get
			{
				string result = mPattern;

				if(result == null)
				{
					if(mParent != null)
					{
						result = mParent.GetPattern();
					}
					else
					{
						result = "";
					}
				}
				return result;
			}
			set { mPattern = value; }
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
		/// Get a reference to the collection of properties assigned to this
		/// action.
		/// </summary>
		/// <remarks>
		/// This property is not inheritable. However, properties from parent
		/// levels are retrieved when calling the GetPropertyByName function.
		/// </remarks>
		public NameValueCollection Properties
		{
			get { return mProperties; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Range																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Range">Range</see>.
		/// </summary>
		private StartEndItem mRange = null;
		/// <summary>
		/// Get/Set a reference to the start and end values of the range.
		/// </summary>
		/// <remarks>
		/// This property is inheritable.
		/// </remarks>
		public StartEndItem Range
		{
			get
			{
				StartEndItem result = mRange;

				if(result == null)
				{
					if(mParent != null)
					{
						result = mParent.GetRange();
					}
					else
					{
						result = mRange = new StartEndItem();
					}
				}
				return result;
			}
			set { mRange = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RectangleInfoList																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="RectangleInfoList">RectangleInfoList</see>.
		/// </summary>
		private static RectangleInfoCollection mRectangleInfoList =
			new RectangleInfoCollection();
		/// <summary>
		/// Get a reference to the collection of rectangle info items in this
		/// session.
		/// </summary>
		[JsonIgnore]
		public static RectangleInfoCollection RectangleInfoList
		{
			get { return mRectangleInfoList; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Run																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Run the configured action.
		/// </summary>
		public async Task Run()
		{
			//List<FileActionItem> actionItems = null;
			string content = "";
			FileInfo file = null;
			string lineNumber = "";
			Match match = null;
			string position = "";
			SvgActionItem soloItem = null;
			string sourceFilename = "";
			string targetFilename = "";
			SvgActionItem topItem = null;

			//	TODO: Create an error exit routine...
			//	Decide which errors require exit and which can just be reported.

			if(mWorkingPathLast != WorkingPath)
			{
				Trace.WriteLine($"Working Path: {WorkingPath}",
					$"{MessageImportanceEnum.Info}");
				mWorkingPathLast = WorkingPath;
			}

			Trace.WriteLine($"Action {mAction}...", $"{MessageImportanceEnum.Info}");
			if(Message?.Length > 0)
			{
				Trace.WriteLine($" {Message}", $"{MessageImportanceEnum.Info}");
			}

			if(mAction == SvgActionTypeEnum.Batch)
			{
				//	If this is a batch action, read the contents of
				//	ConfigFilename.
				if(ConfigFilename?.Length > 0)
				{
					Trace.WriteLine(
						$"Opening configuration file: {Path.GetFileName(ConfigFilename)}",
						$"{MessageImportanceEnum.Info}");
					sourceFilename = AbsolutePath(
						GetPropertyByName(this, nameof(WorkingPath)),
						GetPropertyByName(this, nameof(ConfigFilename)));
					content = File.ReadAllText(sourceFilename);
					if(content?.Length > 0)
					{
						try
						{
							topItem = JsonConvert.DeserializeObject<SvgActionItem>(content);
							if(topItem.Action == SvgActionTypeEnum.Batch)
							{
								//	All of the top item information is added to this item.
								CopyFields(topItem, this,
									skipList: new string[]
									{
									"mAction", "mConfigFilename",
									"mCurrentFile", "mParent",
									"mWorkingPath"
									});
							}
							else
							{
								//	The top item is a child of this action.
								this.Actions.Add(topItem);
							}
							this.Actions.Parent = this;
							SvgActionCollection.InitializeParent(this.Actions);
						}
						catch(Exception ex)
						{
							lineNumber = "Unknown";
							position = "Unknown";
							match = Regex.Match(ex.Message,
								ResourceMain.rxJsonErrorLinePosition);
							if(match.Success)
							{
								lineNumber = GetValue(match, "line");
								position = GetValue(match, "position");
							}
							Trace.WriteLine(
								"Error loading configuration file: " +
								$"Line: {lineNumber}, Position: {position}",
								$"{MessageImportanceEnum.Err}");
						}
					}
					else
					{
						Trace.WriteLine("Error: No configuration data loaded from: " +
							$"{sourceFilename}", $"{MessageImportanceEnum.Err}");
					}
				}
				else
				{
					Trace.WriteLine("Error: Config filename not specified...",
						$"{MessageImportanceEnum.Err}");
				}
			}
			this.Actions.Parent = this;
			//if(mParent == null)
			//{
			//	//	Initialize all levels from the top level.
			//	InitializeLevels(this);
			//}
			InitializeFilenames(this);
			//if(mAction != ActionTypeEnum.Batch)
			//{
			//	//	When this level isn't a batch, identify all folders and files
			//	//	for the action.
			//	In this version, input files can be defined at any level.
			IdentifyInputFiles(this);
			//}
			IdentifyOutputFiles(this);
			switch(mAction)
			{
				#region Removed
				//case ActionTypeEnum.AlphaConditionalAdjust:
				//	// Make adjustments to alpha values of pixels in an image matching
				//	// the specified values. Available variables are a, r, g, b.
				//	if(this.CurrentFile == null)
				//	{
				//		AlphaConditionalAdjust(this);
				//	}
				//	else
				//	{
				//		AlphaConditionalAdjustBytes(this);
				//	}
				//	break;
				//case ActionTypeEnum.AlphaMask:
				//	AlphaMask(this);
				//	break;
				//case ActionTypeEnum.AntiAliasTransparency:
				//	//	Smooth the alpha borders between transparent and non-transparent
				//	//	areas.
				//	if(this.CurrentFile == null)
				//	{
				//		AntiAliasTransparency(this);
				//	}
				//	else
				//	{
				//		AntiAliasTransparencyBytes(this);
				//	}
				//	break;
				#endregion
				case SvgActionTypeEnum.ApplyTransforms:
					ApplyTransforms(this);
					break;
				//	TODO: Work out the object and property cheat sheet, and possibly start with GTKSharp first!
				//case SvgActionTypeEnum.ArtToGtk3:
				//	break;
				//case SvgActionTypeEnum.ArtToXaml:
				//	break;
				case SvgActionTypeEnum.Batch:
					//	TODO: Allow multiple Soloed items to run.
					//	This is a file batch.
					//	Check first to see if there is a solo.
					soloItem = this.Actions.FirstOrDefault(x =>
						x.Options.Exists(y => y.Name.ToLower() == "solo"));
					if(soloItem != null)
					{
						//	Only run the solo item.
						await soloItem.Run();
					}
					else
					{
						//	Run all non-muted items.
						await RunActions(this.Actions);
					}
					if(IsOutputLocal())
					{
						switch(this.OutputType)
						{
							case RenderFileTypeEnum.RectangleInfoList:
								targetFilename =
									AbsolutePath(
										GetPropertyByName(this, nameof(WorkingPath)),
										GetPropertyByName(this, nameof(OutputName)));
								file = new FileInfo(targetFilename);
								Trace.WriteLine(
									$"Writing Rectangles to {file.Name}",
									$"{MessageImportanceEnum.Info}");
								content = JsonConvert.SerializeObject(RectangleInfoList);
								File.WriteAllText(file.FullName, content);
								break;
						}
					}
					break;
				#region Removed
				//case ActionTypeEnum.BuildPathProperty:
				//	BuildPathProperty(this);
				//	break;
				#endregion
				case SvgActionTypeEnum.CalculateTransform:
					CalculateTransform(this);
					break;
				case SvgActionTypeEnum.CleanupSvg:
					CleanupSvg(this);
					break;
				#region NotYetImplemented
				//case SvgActionTypeEnum.ClearInputFiles:
				//	ClearInputFiles(this);
				//	break;
				#endregion
				#region Removed
				//case ActionTypeEnum.ConvertFromB64:
				//	//	Convert the file from base-64 to binary.
				//	ConvertFromB64(this);
				//	break;
				//case ActionTypeEnum.ConvertToB64:
				//	//	Convert the file from binary to base-64.
				//	ConvertToB64(this);
				//	break;
				//case ActionTypeEnum.CopyNumericToRange:
				//	CopyNumericToRange(this);
				//	break;
				//case ActionTypeEnum.CopyRange:
				//	CopyRange(this);
				//	break;
				//case ActionTypeEnum.CropImage:
				//	CropImage(this);
				//	break;
				//case ActionTypeEnum.CropImageToRectangleInfoName:
				//	CropImageToRectangleInfoName(this);
				//	break;
				//case ActionTypeEnum.DelDirectoryPattern:
				//	DelDirectoryPattern(this);
				//	break;
				//case ActionTypeEnum.DeleteFile:
				//	DeleteFile(this);
				//	break;
				//case ActionTypeEnum.DelEveryX:
				//	DelEveryX(this);
				//	break;
				#endregion
				case SvgActionTypeEnum.DereferenceLinks:
					DereferenceLinks(this);
					break;
				#region Removed
				//case ActionTypeEnum.DirReformat:
				//case ActionTypeEnum.FormatDirFile:
				//	//	TODO: Format the DIR file as tab-separated values.
				//	FormatDirFile(this);
				//	break;
				//case ActionTypeEnum.DirToTsv:
				//	DirToTsv(this);
				//	break;
				#endregion
				#region NotYetImplemented
				//case SvgActionTypeEnum.DrawImage:
				//	DrawImage(this);
				//	break;
				//case SvgActionTypeEnum.FileOpenImage:
				//	FileOpenImage(this);
				//	break;
				//case SvgActionTypeEnum.FileOverlayImage:
				//	FileOverlayImage(this);
				//	break;
				//case SvgActionTypeEnum.FileSaveImage:
				//	FileSaveImage(this);
				//	break;
				#endregion
				#region Removed
				//case ActionTypeEnum.FindFiles:
				//	FindFiles(this);
				//	break;
				#endregion
				#region NotYetImplemented
				//case SvgActionTypeEnum.ForEachFile:
				//	ForEachFile(this);
				//	break;
				//case SvgActionTypeEnum.If:
				//	//	Run comparisons in this item's Actions collection.
				//	If(this);
				//	break;
				//case SvgActionTypeEnum.ImageBackground:
				//	//	Paint the specified image background color and / or image
				//	//	on the current working image.
				//	ImageBackground(this);
				//	break;
				//case SvgActionTypeEnum.ImagesClear:
				//	ImagesClear(this);
				//	break;
				#endregion
				#region Removed
				//case ActionTypeEnum.ImageSetCommonBoundary:
				//	ImageSetCommonBoundary(this);
				//	break;
				#endregion
				case SvgActionTypeEnum.ImpliedDesignEnumerateControls:
					//	Enumerate the controls in the implied form design.
					ImpliedDesignEnumerateControls(this);
					break;
				case SvgActionTypeEnum.ImpliedDesignToAvaloniaXaml:
					ImpliedDesignToAvaloniaXaml(this);
					break;
				#region Removed
				//case ActionTypeEnum.LoadRectangleInfoList:
				//	LoadRectangleInfoList(this);
				//	break;
				//case ActionTypeEnum.MoveFiles:
				//	MoveFiles(this);
				//	break;
				//case ActionTypeEnum.NonLinearEditExcel:
				//	//	Execute a non-linear editing pattern using an Excel file with
				//	//	the fields Start, Action, End, Count, X, Y, Width, Height,
				//	//	and Color.
				//	//	The input file is expected to be an Excel file.
				//	await NonLinearEditExcel(this);
				//	break;
				#endregion
				case SvgActionTypeEnum.OpenWorkingSvg:
					//	Open the working SVG file to allow multiple operations to be
					//	completed in the same session.
					OpenWorkingSvg(this);
					break;
				#region Removed
				//case ActionTypeEnum.PrefixFilenames:
				//	PrefixFilenames(this);
				//	break;
				#endregion
				case SvgActionTypeEnum.PurgeDefs:
					//	Remove unused elements from the defs section.
					PurgeDefs(this);
					break;
				#region Removed
				//case ActionTypeEnum.RemoveBackground:
				//	await RemoveBackground(this);
				//	break;
				//case ActionTypeEnum.RenameFiles:
				//	RenameFiles(this);
				//	break;
				//case ActionTypeEnum.RenumberFiles:
				//	RenumberFiles(this);
				//	break;
				//case ActionTypeEnum.RepeatInsertClip:
				//	RepeatInsertClip(this);
				//	break;
				//case ActionTypeEnum.ReplaceGreenscreen:
				//	await ReplaceGreenscreen(this);
				//	break;
				#endregion
				case SvgActionTypeEnum.RoundAllValues:
					//	Round all values.
					RoundAllValues(this);
					break;
				#region NotYetImplemented
				//case SvgActionTypeEnum.RunSequence:
				//	RunSequence(this);
				//	break;
				#endregion
				case SvgActionTypeEnum.SaveWorkingSvg:
					SaveWorkingSvg(this);
					break;
				#region NotYetImplemented
				//case SvgActionTypeEnum.SetWorkingImage:
				//	SetWorkingImage(this);
				//	break;
				//case SvgActionTypeEnum.SizeImage:
				//	SizeImage(this);
				//	break;
				#endregion
				case SvgActionTypeEnum.SortSymbols:
					SortSymbols(this);
					break;
				#region Removed
				//case ActionTypeEnum.StitchFilePatternToMp4:
				//	StitchFilePatternToMp4(this);
				//	break;
				//case ActionTypeEnum.SuffixFilenames:
				//	SuffixFilenames(this);
				//	break;
				#endregion
				default:
					Trace.WriteLine($" Error: {Action} not implemented...",
						$"{MessageImportanceEnum.Err}");
					break;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Sequences																															*
		//*-----------------------------------------------------------------------*
		private SequenceCollection mSequences = new SequenceCollection();
		/// <summary>
		/// Get a reference to the collection of sequences defined for this action.
		/// </summary>
		/// <remarks>
		/// This property is not inheritable.
		/// </remarks>
		public SequenceCollection Sequences
		{
			get
			{
				SequenceCollection result = mSequences;

				if(result.Count == 0 && mParent != null && mParent.Parent != null)
				{
					result = mParent.Parent.Sequences;
				}
				return result;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SourceDir																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="SourceDir">SourceDir</see>.
		/// </summary>
		private DirectoryInfo mSourceDir = null;
		/// <summary>
		/// Get/Set the internal, calculated source directory.
		/// </summary>
		/// <remarks>
		/// This property is non-inerhitable.
		/// </remarks>
		[JsonIgnore]
		public DirectoryInfo SourceDir
		{
			get { return mSourceDir; }
			set { mSourceDir = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SourceFolderName																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="SourceFolderName">SourceFolderName</see>.
		/// </summary>
		private string mSourceFolderName = null;
		/// <summary>
		/// Get/Set the path and folder name of the data source for this action.
		/// </summary>
		/// <remarks>
		/// <para>This property is inheritable.</para>
		/// <para>Corresponds with the command-line parameter 'InFolder'.</para>
		/// </remarks>
		public string SourceFolderName
		{
			get
			{
				string result = mSourceFolderName;

				if(result == null)
				{
					if(mParent != null)
					{
						result = mParent.GetSourceFolderName();
					}
					else
					{
						result = "";
					}
				}
				return result;
			}
			set { mSourceFolderName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Stop																																	*
		//*-----------------------------------------------------------------------*
		private bool mStop = false;
		/// <summary>
		/// Get/Set a value indicating whether the process should be stopped.
		/// </summary>
		[JsonIgnore]
		public bool Stop
		{
			get { return mStop; }
			set
			{
				mStop = value;
				if(mParent?.Parent != null)
				{
					mParent.Parent.Stop = value;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Text																																	*
		//*-----------------------------------------------------------------------*
		private string mText = null;
		/// <summary>
		/// Get/Set the text of the current action.
		/// </summary>
		/// <remarks>
		/// This property is inheritable.
		/// </remarks>
		public string Text
		{
			get
			{
				string result = mText;

				if(result == null)
				{
					if(mParent != null)
					{
						result = mParent.GetText();
					}
					else
					{
						result = "";
					}
				}
				return result;
			}
			set { mText = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	WorkingImage																													*
		//*-----------------------------------------------------------------------*
		private static BitmapInfoItem mWorkingImage = null;
		/// <summary>
		/// Get/Set a reference to the current working image in this session.
		/// </summary>
		[JsonIgnore]
		public static BitmapInfoItem WorkingImage
		{
			get { return mWorkingImage; }
			set { mWorkingImage = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	WorkingPath																														*
		//*-----------------------------------------------------------------------*
		private string mWorkingPath = null;
		/// <summary>
		/// Get/Set the working path for operations in this instance.
		/// </summary>
		/// <remarks>
		/// <para>This property is inheritable.</para>
		/// <para>Corresponds with the command-line parameter 'WorkingPath'.</para>
		/// </remarks>
		public string WorkingPath
		{
			get
			{
				string result = mWorkingPath;

				if(result == null)
				{
					if(mParent != null)
					{
						result = mParent.GetWorkingPath();
					}
					else
					{
						result = "";
					}
				}
				return result;
			}
			set
			{
				mWorkingPath = value;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	WorkingSvg																														*
		//*-----------------------------------------------------------------------*
		private SvgDocumentItem mWorkingSvg = null;
		/// <summary>
		/// Get/Set the working SVG file for operations in this instance.
		/// </summary>
		/// <remarks>
		/// <para>This property is inheritable.</para>
		/// </remarks>
		public SvgDocumentItem WorkingSvg
		{
			get
			{
				SvgDocumentItem result = mWorkingSvg;

				if(result == null)
				{
					if(mParent != null)
					{
						result = mParent.GetWorkingSvg();
					}
				}
				return result;
			}
			set
			{
				if(mInputFilename?.Length > 0 ||
					mParent == null || mParent.Parent == null)
				{
					mWorkingSvg = value;
					if(mWorkingSvg != null)
					{
						mWorkingSvg.IsLocal = false;
					}
				}
				else if(string.IsNullOrEmpty(mInputFilename) &&
					mParent?.Parent != null)
				{
					mParent.Parent.WorkingSvg = value;
				}
			}
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
