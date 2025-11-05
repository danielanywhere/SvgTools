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
using Geometry;
using Html;

using static SvgToolsLib.SvgToolsUtil;

namespace SvgToolsLib
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
		//* ApplyCommonProperties																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the common properties on the target node from the source area.
		/// </summary>
		/// <param name="source">
		/// Reference to the drawing source area whose HTML node might contain
		/// common attributes to place on the target.
		/// </param>
		/// <param name="target">
		/// Referenec to the target HTML node whose attributes can be updated
		/// by common property values.
		/// </param>
		private static void ApplyCommonProperties(ControlAreaItem source,
			HtmlNodeItem target)
		{
			string lowerName = "";
			HtmlNodeItem node = null;

			if(source?.Node != null && target != null)
			{
				node = source.Node;
				foreach(HtmlAttributeItem attributeItem in node.Attributes)
				{
					lowerName = attributeItem.Name.ToLower();
					switch(lowerName)
					{
						case "background":
							target.Attributes.SetAttribute(
								"Background", attributeItem.Value);
							break;
						case "borderbrush":
							target.Attributes.SetAttribute(
								"BorderBrush", attributeItem.Value);
							break;
						case "borderthickness":
							target.Attributes.SetAttribute(
								"BorderThickness", attributeItem.Value);
							break;
						case "margin":
							target.Attributes.SetAttribute(
								"Margin",
								GetCommaDelimitedIntegerNumber(attributeItem.Value));
							break;
						case "padding":
							target.Attributes.SetAttribute(
								"Padding",
								GetCommaDelimitedIntegerNumber(attributeItem.Value));
							break;
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ApplyPreemptiveProperties																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Apply any applicable properties to an outer shell object, returning
		/// either the outer object, if created, or the original supplied object
		/// if no changes were made.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area for which the preemptive change will
		/// be applied.
		/// </param>
		/// <param name="node">
		/// Reference to the node in focus.
		/// </param>
		/// <returns>
		/// Reference to the original supplied output node, if no changes were
		/// made, or the new encapsulating object, if a preemptive shell was
		/// required.
		/// </returns>
		private static HtmlNodeItem ApplyPreemptiveProperties(ControlAreaItem area,
			HtmlNodeItem node)
		{
			HtmlNodeItem border = null;
			string lowerName = "";
			HtmlNodeItem result = node;
			HtmlNodeItem sourceNode = null;

			if(area?.Node != null && node != null)
			{
				sourceNode = area.Node;
				foreach(HtmlAttributeItem attributeItem in sourceNode.Attributes)
				{
					lowerName = attributeItem.Name.ToLower();
					switch(lowerName)
					{
						case "boxshadow":
						case "cornerradius":
							if(border == null)
							{
								border = new HtmlNodeItem()
								{
									NodeType = "Border",
									SelfClosing = false
								};
							}
							break;
					}
					if(border != null)
					{
						break;
					}
				}
				if(border != null)
				{
					foreach(HtmlAttributeItem attributeItem in sourceNode.Attributes)
					{
						lowerName = attributeItem.Name.ToLower();
						switch(lowerName)
						{
							case "boxshadow":
								border.Attributes.SetAttribute(
									"BoxShadow", attributeItem.Value);
								break;
							case "cornerradius":
								border.Attributes.SetAttribute(
									"CornerRadius", attributeItem.Value);
								break;
						}
					}
					result = border;
					result.Nodes.Add(node);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ApplyTokenProperties																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Apply properties from the rendering token to the current output node.
		/// </summary>
		/// <param name="renderToken">
		/// Reference to the rendering token containing information to apply
		/// on the output node.
		/// </param>
		/// <param name="node">
		/// Reference to the output XAML node.
		/// </param>
		private static void ApplyTokenProperties(RenderTokenItem renderToken,
			HtmlNodeItem node)
		{
			if(renderToken != null && node != null)
			{
				foreach(NameValueItem propertyItem in renderToken.Properties)
				{
					switch(propertyItem.Name)
					{
						case "GridColumnIndex":
							node.Attributes.SetAttribute("Grid.Column", propertyItem.Value);
							break;
						case "GridRowIndex":
							node.Attributes.SetAttribute("Grid.Row", propertyItem.Value);
							break;
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

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
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// Reference to the output HTML node, in the active dialect, that
		/// properly represents the caller's supplied control area, if
		/// legitimate. Otherwise, null.
		/// </returns>
		protected override HtmlNodeItem RenderOutputNode(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			List<ControlAreaItem> areas = null;
			string attributeValue = "";
			char[] charColon = new char[] { ':' };
			char[] charComma = new char[] { ',' };
			char[] charSemicolon = new char[] { ';' };
			HtmlNodeItem childNode = null;
			RenderTokenItem childToken = null;
			HtmlNodeItem childNode2 = null;
			HtmlNodeItem childNode3 = null;
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
			List<HtmlNodeItem> nodes = null;
			RectilinearOrientationEnum orientation = RectilinearOrientationEnum.None;
			HtmlNodeItem result = null;
			int rowIndex = 0;
			ControlAreaItem secondArea = null;
			List<ControlAreaItem> segmentAreas = null;
			int state = 0;
			string text = "";
			ControlAreaItem textArea = null;

			//	TODO: Implement AXAML version of RenderOutputNode.
			//	TODO: Allow coordinates to be added if control is placed on panel.
			if(area != null)
			{
				if(renderToken != null)
				{
					childToken = RenderTokenItem.DeepCopyWithRemove(renderToken,
						"Dock", "GridColumnIndex", "GridRowIndex");
				}
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
								childNode = RenderOutputNode(imageArea, childToken);
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
										childNode = RenderOutputNode(firstArea, childToken);
										if(childNode != null)
										{
											result.Nodes.Add(childNode);
										}
										secondArea =
											(imageArea == firstArea ? textArea : imageArea);
										if(secondArea != null)
										{
											childNode = RenderOutputNode(secondArea, childToken);
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
						nodes = RenderOutputNodes(area.FrontAreas, childToken);
						foreach(HtmlNodeItem nodeItem in nodes)
						{
							result.Nodes.Add(nodeItem);
						}
						break;
					case ImpliedDesignIntentEnum.Form:
						//	Inner forms are not supported, but their controls can be
						//	rendered.
						nodes = RenderOutputNodes(area.FrontAreas, childToken);
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
							colIndex = GetColumnIndex(area.FrontAreas, areaItem.X);
							rowIndex = GetRowIndex(area.FrontAreas, areaItem.Y);
							if(colIndex > -1)
							{
								childToken.Properties.SetValue(
									"GridColumnIndex", colIndex.ToString());
							}
							if(rowIndex > -1)
							{
								childToken.Properties.SetValue(
									"GridRowIndex", rowIndex.ToString());
							}
							node = RenderOutputNode(areaItem, childToken);
							if(node != null)
							{
								//	Assign the child-specified grid and column dimensions.
								if(colIndex > -1 && rowIndex > -1)
								{
									attributeValue =
										areaItem.Node.Attributes.GetValue("ColumnWidth");
									if(attributeValue?.Length > 0)
									{
										gridColDims[colIndex] = attributeValue;
									}
									attributeValue =
										areaItem.Node.Attributes.GetValue("RowHeight");
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
								result.Nodes.Add(RenderOutputNode(areaItem, childToken));
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
							colIndex = GetColumnIndex(area.FrontAreas, areaItem.X);
							if(colIndex > -1)
							{
								childToken.Properties.SetValue(
									"GridColumnIndex", colIndex.ToString());
							}
							node = RenderOutputNode(areaItem, childToken);
							if(node != null)
							{
								//	Assign the child-specified column dimensions.
								if(colIndex > -1)
								{
									attributeValue =
										areaItem.Node.Attributes.GetValue("ColumnWidth");
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
						result = new HtmlNodeItem()
						{
							NodeType = "ScrollViewer",
							SelfClosing = false
						};
						SetRenderedControlName(area.Node, result);
						result.Attributes.SetAttribute(
							"HorizontalScrollBarVisibility", "Auto");
						result.Attributes.SetAttribute(
							"VerticalScrollBarVisibility", "Disabled");
						childNode = new HtmlNodeItem()
						{
							NodeType = "StackPanel",
							SelfClosing = false
						};
						childNode.Attributes.SetAttribute("Orientation", "Horizontal");
						childNode.Attributes.SetAttribute("Spacing", "10");
						childNode.Nodes.AddRange(
							RenderOutputNodes(area.FrontAreas, childToken));
						result.Nodes.Add(childNode);
						break;
					case ImpliedDesignIntentEnum.HorizontalStackPanel:
						result = new HtmlNodeItem()
						{
							NodeType = "StackPanel",
							SelfClosing = false
						};
						SetRenderedControlName(area.Node, result);
						result.Attributes.SetAttribute("Orientation", "Horizontal");
						result.Attributes.SetAttribute("Spacing", "10");
						result.Nodes.AddRange(
							RenderOutputNodes(area.FrontAreas, childToken));
						result.Nodes.Add(childNode);
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
						result = new HtmlNodeItem()
						{
							NodeType = "Label",
							SelfClosing = true
						};
						SetRenderedControlName(imageArea.Node, result);
						result.Attributes.SetAttribute("Content", GetText(area));
						break;
					case ImpliedDesignIntentEnum.ListBox:
						result = new HtmlNodeItem()
						{
							NodeType = "ListBox",
							SelfClosing = false
						};
						SetRenderedControlName(imageArea.Node, node);
						result.Attributes.SetAttribute("HorizontalAlignment", "Stretch");
						//	Items panel.
						childNode = new HtmlNodeItem()
						{
							NodeType = "ListBox.ItemsPanel",
							SelfClosing = false
						};
						childNode2 = new HtmlNodeItem()
						{
							NodeType = "ItemsPanelTemplate",
							SelfClosing = false
						};
						childNode3 = new HtmlNodeItem()
						{
							NodeType = "StackPanel",
							SelfClosing = true
						};
						childNode3.Attributes.SetAttribute("Orientation", "Vertical");
						childNode3.Attributes.SetAttribute(
							"HorizontalAlignment", "Stretch");
						childNode2.Nodes.Add(childNode3);
						childNode.Nodes.Add(childNode2);
						result.Nodes.Add(childNode);

						SetControlStyles("ListBox.Styles", area, result);

						//	ListBox items can be shown directly over the control or upon
						//	a node extension layer. In the case that one or more node
						//	extension layers exist for this control, there may be one
						//	or more Definitions-type areas in the area.FrontAreas
						//	collection, and in that case, there might also be
						//	one or more properties on the Definitions-type area that
						//	should be applied to the target elements created as a result
						//	of that definition.

						//	Whether or not multiple sources exist, the number of list
						//	items is defined as the count of objects in common spaces
						//	over the control or its extended layer.
						segmentAreas = GetSegmentAreas(area.FrontAreas);
						foreach(ControlAreaItem areaItem in segmentAreas)
						{
							childNode = new HtmlNodeItem()
							{
								NodeType = "ListBoxItem",
								SelfClosing = false
							};
							if(areaItem.FrontAreas.Count > 0)
							{
								if(areaItem.FrontAreas.Count == 1 &&
									areaItem.FrontAreas[0].Intent ==
									ImpliedDesignIntentEnum.Text)
								{
									//	This is a single text entry.
									childNode.Attributes.SetAttribute(
										"Content", GetText(areaItem));
								}
								else if(areaItem.FrontAreas[0].Intent ==
									ImpliedDesignIntentEnum.Text)
								{
									//	Multiple text entries.
									childNode2 = new HtmlNodeItem()
									{
										NodeType = "StackPanel",
										SelfClosing = false
									};
									childNode2.Attributes.SetAttribute(
										"Orientation", "Vertical");
									foreach(ControlAreaItem textAreaItem in areaItem.FrontAreas)
									{
										childNode3 = new HtmlNodeItem()
										{
											NodeType = "TextBlock",
											SelfClosing = false
										};
										foreach(NameValueItem nameValueItem in
											textAreaItem.Properties)
										{
											childNode3.Attributes.SetAttribute(
												nameValueItem.Name, nameValueItem.Value);
										}
										childNode2.Nodes.Add(childNode3);
									}
									childNode.Nodes.Add(childNode2);
								}
								else if(areaItem.FrontAreas[0].Intent ==
									ImpliedDesignIntentEnum.Image)
								{
									//	One or more image entries.
									//	TODO: Add support for Image filename in Source property.
									//	Filename syntax '/Assets/Images/{filename}'
									childNode2 = new HtmlNodeItem()
									{
										NodeType = "Grid",
										SelfClosing = false
									};
									childNode2.Attributes.SetAttribute(
										"Width", areaItem.FrontAreas[0].Width.ToString());
									childNode2.Attributes.SetAttribute(
										"Height", areaItem.FrontAreas[0].Height.ToString());
									foreach(ControlAreaItem imageAreaItem in areaItem.FrontAreas)
									{
										childNode3 = new HtmlNodeItem()
										{
											NodeType = "Image",
											SelfClosing = true,
										};
										childNode3.Attributes.SetAttribute(
											"x:Name", imageArea.Node.Id);
										childNode3.Attributes.SetAttribute(
											"Stretch", "None");
										childNode3.Attributes.SetAttribute(
											"IsHitTestVisible", "False");
										foreach(NameValueItem nameValueItem in
											imageAreaItem.Properties)
										{
											childNode3.Attributes.SetAttribute(
												nameValueItem.Name, nameValueItem.Value);
										}
										childNode2.Nodes.Add(childNode3);
									}
									childNode.Nodes.Add(childNode2);
								}
							}
							result.Nodes.Add(childNode);
						}
						break;
					case ImpliedDesignIntentEnum.ListView:
						//	TODO: !1 - Stopped here...
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
							rowIndex = GetRowIndex(area.FrontAreas, areaItem.Y);
							if(rowIndex > -1)
							{
								childToken.Properties.SetValue(
									"GridRowIndex", rowIndex.ToString());
							}
							node = RenderOutputNode(areaItem, childToken);
							if(node != null)
							{
								//	Assign the child-specified row dimensions.
								if(rowIndex > -1)
								{
									attributeValue =
										areaItem.Node.Attributes.GetValue("RowHeight");
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
						result = new HtmlNodeItem()
						{
							NodeType = "ScrollViewer",
							SelfClosing = false
						};
						SetRenderedControlName(area.Node, result);
						result.Attributes.SetAttribute(
							"VerticalScrollBarVisibility", "Auto");
						result.Attributes.SetAttribute(
							"HorizontalScrollBarVisibility", "Disabled");
						childNode = new HtmlNodeItem()
						{
							NodeType = "StackPanel",
							SelfClosing = false
						};
						childNode.Attributes.SetAttribute("Orientation", "Vertical");
						childNode.Attributes.SetAttribute("Spacing", "10");
						childNode.Nodes.AddRange(
							RenderOutputNodes(area.FrontAreas, childToken));
						result.Nodes.Add(childNode);
						break;
					case ImpliedDesignIntentEnum.VerticalStackPanel:
						result = new HtmlNodeItem()
						{
							NodeType = "StackPanel",
							SelfClosing = false
						};
						SetRenderedControlName(area.Node, result);
						result.Attributes.SetAttribute("Orientation", "Vertical");
						result.Attributes.SetAttribute("Spacing", "10");
						result.Nodes.AddRange(
							RenderOutputNodes(area.FrontAreas, childToken));
						result.Nodes.Add(childNode);
						break;
				}
				if(result != null)
				{
					ApplyCommonProperties(area, result);
					ApplyTokenProperties(renderToken, result);
					result = ApplyPreemptiveProperties(area, result);
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
