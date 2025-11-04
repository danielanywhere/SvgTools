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
using System.Linq;
using System.Text;

using Html;

namespace SvgToolsLibrary
{
	//*-------------------------------------------------------------------------*
	//*	ImpliedFormDesignAXaml																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Features and functionality for supporting
	/// Avalonia XAML Implied Form Design translation.
	/// </summary>
	public class ImpliedFormDesignAXaml : ImpliedFormDesign
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* SetRenderedControlName																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The the name of the rendered control from the source control's ID.
		/// </summary>
		/// <param name="sourceNode">
		/// Reference to the source design node.
		/// </param>
		/// <param name="targetNode">
		/// Reference to the target rendered node.
		/// </param>
		private void SetRenderedControlName(HtmlNodeItem sourceNode,
			HtmlNodeItem targetNode)
		{
			HtmlAttributeItem attribute = null;

			if(sourceNode != null && targetNode != null)
			{
				attribute = sourceNode.Attributes.FirstOrDefault(x =>
					x.Name.ToLower() == "id" &&
					x.Value?.Length > 0);
				if(attribute != null)
				{
					if(!attribute.Value.ToLower().StartsWith(
						sourceNode.NodeType.ToLower()))
					{
						targetNode.Attributes.SetAttribute("x:Name", attribute.Value);
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* FillForm																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Fill the form starting at the provided collection of areas.
		/// </summary>
		/// <param name="areas">
		/// Reference to the collection of control areas from which the form will
		/// be filled.
		/// </param>
		/// <param name="node">
		/// Reference to the output node.
		/// </param>
		protected override void FillForm(ControlAreaCollection areas,
			HtmlNodeItem node)
		{
			base.FillForm(areas, node);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* PerformLayout																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Perform layout at the current and descending levels.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area to be processed.
		/// </param>
		/// <param name="outputNode">
		/// Reference to the last active output node to which child items will
		/// be appended.
		/// </param>
		protected override void PerformLayout(ControlAreaItem area,
			HtmlNodeItem outputNode)
		{
			//	TODO: Implement AXAML version of PerformLayout.
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderOutputNode																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render one or more elements to generate the output node that properly
		/// represents the current area presented by the caller.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <returns>
		/// Reference to the output HTML node, in the active dialect, that
		/// properly represents the caller's supplied control area, if
		/// legitimate. Otherwise, null.
		/// </returns>
		protected override HtmlNodeItem RenderOutputNode(ControlAreaItem area)
		{
			List<ControlAreaItem> areas = null;
			HtmlAttributeItem attribute = null;
			string attributeValue = "";
			HtmlNodeItem childNode = null;
			int colIndex = 0;
			HtmlNodeItem containerNode = null;
			ControlAreaItem firstArea = null;
			int gridColCount = 0;
			string[] gridColDims = null;
			int gridRowCount = 0;
			string[] gridRowDims = null;
			ControlAreaItem imageArea = null;
			string imageName = "";
			HtmlNodeItem node = null;
			int nodeIndex = 0;
			List<HtmlNodeItem> nodes = null;
			RectilinearOrientationEnum orientation = RectilinearOrientationEnum.None;
			HtmlNodeItem result = null;
			int rowIndex = 0;
			ControlAreaItem secondArea = null;
			int state = 0;
			string text = "";
			ControlAreaItem textArea = null;

			//	TODO: Implement AXAML version of RenderOutputNode.
			//	TODO: Allow coordinates to be added if control is placed on panel.
			if(area != null)
			{
				switch(area.Intent)
				{
					case ImpliedDesignIntentEnum.Button:
						state =
							(HasImages(area.FrontAreas) ? 0x2 : 0x0) |
							(HasText(area.FrontAreas) ? 0x1 : 0x0);
						switch(state)
						{
							case 0:
								//	No text, no images.
								break;
							case 1:
								//	Text only, no images.
								text = GetText(area);
								result = new HtmlNodeItem()
								{
									NodeType = "Button",
									SelfClosing = true
								};
								result.Attributes.SetAttribute("Content", text);
								SetRenderedControlName(area.Node, result);
								break;
							case 2:
								//	Images only, no text.
								imageArea = GetImageArea(area);
								result = new HtmlNodeItem()
								{
									NodeType = "Button",
									SelfClosing = false
								};
								SetRenderedControlName(area.Node, result);
								childNode = RenderOutputNode(imageArea);
								if(childNode != null)
								{
									result.Nodes.Add(childNode);
								}
								break;
							case 3:
								//	Text and images.
								imageArea = GetImageArea(area);
								textArea = GetTextArea(area);
								if(imageArea != null && textArea != null)
								{
									result = new HtmlNodeItem()
									{
										NodeType = "Button",
										SelfClosing = false
									};
									SetRenderedControlName(area.Node, result);
									containerNode = new HtmlNodeItem()
									{
										NodeType = "StackPanel"
									};
									containerNode.Attributes.SetAttribute("Spacing", "5");
									result.Nodes.Add(containerNode);
									orientation = GetOrientation(imageArea, textArea);
									switch(orientation)
									{
										case RectilinearOrientationEnum.Horizontal:
										case RectilinearOrientationEnum.None:
											containerNode.Attributes.SetAttribute(
												"Orientation", "Horizontal");
											break;
										case RectilinearOrientationEnum.Vertical:
											containerNode.Attributes.SetAttribute(
												"Orientation", "Vertical");
											break;
									}
									firstArea = GetFirstArea(imageArea, textArea, orientation);
									if(firstArea != null)
									{
										childNode = RenderOutputNode(firstArea);
										if(childNode != null)
										{
											result.Nodes.Add(childNode);
										}
										secondArea =
											(imageArea == firstArea ? textArea : imageArea);
										if(secondArea != null)
										{
											childNode = RenderOutputNode(secondArea);
											if(childNode != null)
											{
												result.Nodes.Add(childNode);
											}
										}
									}
								}
								break;
						}
						break;
					case ImpliedDesignIntentEnum.CheckBox:
						//	A checkbox might have an integrated label or nothing else.
						result = new HtmlNodeItem()
						{
							NodeType = "CheckBox",
							SelfClosing = true
						};
						if(area.Node?.NodeType == "g" &&
							HasText(area.FrontAreas))
						{
							//	This is a checkbox with a label.
							text = GetText(area);
							result.Attributes.SetAttribute("Content", text);
						}
						SetRenderedControlName(area.Node, result);
						break;
					case ImpliedDesignIntentEnum.ComboBox:
						result = new HtmlNodeItem()
						{
							NodeType = "ComboBox",
							SelfClosing = false
						};
						SetRenderedControlName(area.Node, result);
						areas = GetTextDefinitions(area.FrontAreas);
						foreach(ControlAreaItem areaItem in areas)
						{
							childNode = new HtmlNodeItem()
							{
								NodeType = "ComboBoxItem",
								SelfClosing = false
							};
							if(areaItem.Node != null)
							{
								SetRenderedControlName(areaItem.Node, childNode);
								childNode.Text = GetText(areaItem);
							}
							result.Nodes.Add(childNode);
						}
						break;
					case ImpliedDesignIntentEnum.FlowPanel:
						result = new HtmlNodeItem()
						{
							NodeType = "FlowPanel",
							SelfClosing = false
						};
						SetRenderedControlName(area.Node, result);
						nodes = RenderOutputNodes(area.FrontAreas);
						foreach(HtmlNodeItem nodeItem in nodes)
						{
							result.Nodes.Add(nodeItem);
						}
						break;
					case ImpliedDesignIntentEnum.Form:
						//	Inner forms are not supported, but their controls can be
						//	rendered.
						nodes = RenderOutputNodes(area.FrontAreas);
						foreach(HtmlNodeItem nodeItem in nodes)
						{
							result.Nodes.Add(nodeItem);
						}
						break;
					case ImpliedDesignIntentEnum.FormInformation:
						//	Form information is not rendered.
						break;
					case ImpliedDesignIntentEnum.Grid:
						//	The direct front area items will be the only participants
						//	in a grid layout.
						gridRowCount = GetRowCount(area.FrontAreas);
						gridRowDims = new string[gridRowCount];
						for(rowIndex = 0; rowIndex < gridRowCount; rowIndex++)
						{
							gridRowDims[rowIndex] = "*";
						}
						gridColCount = GetColumnCount(area.FrontAreas);
						gridColDims = new string[gridColCount];
						for(colIndex = 0; colIndex < gridColCount; colIndex ++)
						{
							gridColDims[colIndex] = "*";
						}
						result = new HtmlNodeItem()
						{
							NodeType = "Grid",
							SelfClosing = false
						};
						SetRenderedControlName(area.Node, result);
						foreach(ControlAreaItem areaItem in area.FrontAreas)
						{
							node = RenderOutputNode(areaItem);
							if(node != null)
							{
								//	Assign the child-specified grid and column dimensions.
								colIndex = GetColumnIndex(area.FrontAreas, areaItem.X);
								rowIndex = GetRowIndex(area.FrontAreas, areaItem.Y);
								if(colIndex > -1 && rowIndex > -1)
								{
									attributeValue = node.Attributes.GetValue("ColumnWidth");
									if(attributeValue?.Length > 0)
									{
										gridColDims[colIndex] = attributeValue;
									}
									attributeValue = node.Attributes.GetValue("RowHeight");
									if(attributeValue?.Length > 0)
									{
										gridRowDims[rowIndex] = attributeValue;
									}
								}
								result.Nodes.Add(node);
							}
						}
						result.Attributes.SetAttribute(
							"RowDefinitions", string.Join(',', gridRowDims));
						result.Attributes.SetAttribute(
							"ColumnDefinitions", string.Join(',', gridColDims));
						break;
					case ImpliedDesignIntentEnum.GridView:
						//	Avalonia doesn't explicitly have a GridView control, but
						//	they do have a DataGrid.
						gridColCount = GetColumnCount(area.FrontAreas);
						gridColDims = new string[gridColCount];
						for(colIndex = 0; colIndex < gridColCount; colIndex++)
						{
							gridColDims[colIndex] = "*";
						}
						result = new HtmlNodeItem()
						{
							NodeType = "DataGrid",
							SelfClosing = false
						};
						SetRenderedControlName(area.Node, result);
						result.Attributes.SetAttribute("AutoGenerateColumns", "False");
						result.Attributes.SetAttribute("CanUserSortItems", "True");
						if(area.FrontAreas.Count > 0)
						{
							node = new HtmlNodeItem()
							{
								NodeType = "DataGrid.Columns",
								SelfClosing = false
							};
							result.Nodes.Add(node);
							foreach(ControlAreaItem areaItem in area.FrontAreas)
							{
								childNode = new HtmlNodeItem()
								{
									NodeType = "DataGridTextColumn",
									SelfClosing = true
								};
								childNode.Attributes.SetAttribute("Header", GetText(areaItem));
								node.Nodes.Add(childNode);
							}
						}
						break;
					case ImpliedDesignIntentEnum.GroupBox:
						textArea = GetTextArea(area);
						result = new HtmlNodeItem()
						{
							NodeType = "HeaderedContentControl",
							SelfClosing = false
						};
						SetRenderedControlName(area.Node, result);
						if(textArea != null)
						{
							result.Attributes.SetAttribute("Header", GetText(textArea));
						}
						foreach(ControlAreaItem areaItem in area.FrontAreas)
						{
							if(areaItem != textArea)
							{
								result.Nodes.Add(RenderOutputNode(areaItem));
							}
						}
						break;
					case ImpliedDesignIntentEnum.HorizontalGrid:
						//	Avalonia doesn't have a horizontal grid.
						//	However, their normal grid supports columns-only operation.
						//	The direct front area items will be the only participants
						//	in a grid layout.
						gridColCount = GetColumnCount(area.FrontAreas);
						gridColDims = new string[gridColCount];
						for(colIndex = 0; colIndex < gridColCount; colIndex++)
						{
							gridColDims[colIndex] = "*";
						}
						result = new HtmlNodeItem()
						{
							NodeType = "Grid",
							SelfClosing = false
						};
						SetRenderedControlName(area.Node, result);
						foreach(ControlAreaItem areaItem in area.FrontAreas)
						{
							node = RenderOutputNode(areaItem);
							if(node != null)
							{
								//	Assign the child-specified column dimensions.
								colIndex = GetColumnIndex(area.FrontAreas, areaItem.X);
								if(colIndex > -1)
								{
									attributeValue = node.Attributes.GetValue("ColumnWidth");
									if(attributeValue?.Length > 0)
									{
										gridColDims[colIndex] = attributeValue;
									}
								}
								result.Nodes.Add(node);
							}
						}
						result.Attributes.SetAttribute(
							"ColumnDefinitions", string.Join(',', gridColDims));
						break;
					case ImpliedDesignIntentEnum.HorizontalScrollPanel:
						//	TODO: !1 - Stopped here...
						break;
					case ImpliedDesignIntentEnum.HorizontalStackPanel:
						break;
					case ImpliedDesignIntentEnum.Image:
						imageArea = GetImageArea(area);
						if(imageArea != null)
						{
							imageName = GetImageName(imageArea.Node);
						}
						if(imageName.Length > 0)
						{
							result = new HtmlNodeItem()
							{
								NodeType = "Image",
								SelfClosing = true
							};
							SetRenderedControlName(imageArea.Node, result);
							result.Attributes.SetAttribute("Source",
								$"avares://{mProjectName}/Assets/{imageName}");
							TransferUnitAttribute(imageArea.Node, "width", result, "Width");
							TransferUnitAttribute(imageArea.Node, "height", result, "Height");
						}
						break;
					case ImpliedDesignIntentEnum.Label:
						break;
					case ImpliedDesignIntentEnum.ListBox:
						break;
					case ImpliedDesignIntentEnum.ListView:
						break;
					case ImpliedDesignIntentEnum.MenuBar:
						break;
					case ImpliedDesignIntentEnum.MenuPanel:
						break;
					case ImpliedDesignIntentEnum.None:
						break;
					case ImpliedDesignIntentEnum.Panel:
						break;
					case ImpliedDesignIntentEnum.PictureBox:
						break;
					case ImpliedDesignIntentEnum.ProgressBar:
						break;
					case ImpliedDesignIntentEnum.RadioButton:
						break;
					case ImpliedDesignIntentEnum.ScrollPanel:
						break;
					case ImpliedDesignIntentEnum.SplitPanel:
						break;
					case ImpliedDesignIntentEnum.StaticPanel:
						break;
					case ImpliedDesignIntentEnum.StatusBar:
						break;
					case ImpliedDesignIntentEnum.TabControl:
						break;
					case ImpliedDesignIntentEnum.Text:
						break;
					case ImpliedDesignIntentEnum.TextBox:
						break;
					case ImpliedDesignIntentEnum.TextWithHelper:
						break;
					case ImpliedDesignIntentEnum.ToolBar:
						break;
					case ImpliedDesignIntentEnum.TrackBar:
						break;
					case ImpliedDesignIntentEnum.TreeView:
						break;
					case ImpliedDesignIntentEnum.UpDown:
						break;
					case ImpliedDesignIntentEnum.VerticalGrid:
						//	Avalonia doesn't have a vertical grid.
						//	However, their normal grid supports rows-only operation.
						//	The direct front area items will be the only participants
						//	in a grid layout.
						gridRowCount = GetRowCount(area.FrontAreas);
						gridRowDims = new string[gridRowCount];
						for(rowIndex = 0; rowIndex < gridRowCount; rowIndex++)
						{
							gridRowDims[rowIndex] = "*";
						}
						result = new HtmlNodeItem()
						{
							NodeType = "Grid",
							SelfClosing = false
						};
						SetRenderedControlName(area.Node, result);
						foreach(ControlAreaItem areaItem in area.FrontAreas)
						{
							node = RenderOutputNode(areaItem);
							if(node != null)
							{
								//	Assign the child-specified row dimensions.
								rowIndex = GetRowIndex(area.FrontAreas, areaItem.Y);
								if(rowIndex > -1)
								{
									attributeValue = node.Attributes.GetValue("RowHeight");
									if(attributeValue?.Length > 0)
									{
										gridRowDims[rowIndex] = attributeValue;
									}
								}
								result.Nodes.Add(node);
							}
						}
						result.Attributes.SetAttribute(
							"RowDefinitions", string.Join(',', gridRowDims));
						break;
					case ImpliedDesignIntentEnum.VerticalScrollPanel:
						break;
					case ImpliedDesignIntentEnum.VerticalStackPanel:
						break;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*


		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new instance of the ImpliedFormDesignAXaml item.
		/// </summary>
		public ImpliedFormDesignAXaml() : base()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new instance of the ImpliedFormDesignAXaml item.
		/// </summary>
		/// <param name="svgDocument">
		/// Reference to the SVG document to be parsed.
		/// </param>
		public ImpliedFormDesignAXaml(SvgDocumentItem svgDocument) :
			base(svgDocument)
		{
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToXaml																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Translate the current control area content to Avalonia XAML and return
		/// the result to the caller.
		/// </summary>
		/// <returns>
		/// The Avalonia XAML content representing the controls presented in
		/// the local control areas.
		/// </returns>
		public string ToXaml()
		{
			HtmlNodeItem node = new HtmlNodeItem();

			FillForm(mControlAreas, node);
			return node.Html;
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
