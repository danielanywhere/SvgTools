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
using System.Diagnostics;
using System.IO;
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
		//* AddMenuPanels																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add the contents of matching menu panels to the provided base menu.
		/// </summary>
		/// <param name="targetNodes">
		/// Reference to the collection of HTML output nodes to which matches
		/// will be added.
		/// </param>
		/// <param name="baseMenu">
		/// Reference to the base menu item to be matched.
		/// </param>
		/// <param name="definitions">
		/// Reference to the set of definitions within which the MenuPanel objects
		/// can be found.
		/// </param>
		private static void AddMenuPanels(HtmlNodeCollection targetNodes,
			ControlAreaItem baseMenu, ControlAreaCollection definitions)
		{
			string lowerName = "";
			List<ControlAreaItem> menuItems = null;
			HtmlNodeItem node = null;
			List<ControlAreaItem> panels = null;

			if(targetNodes != null && baseMenu != null && baseMenu.Node != null &&
				definitions?.Count > 0)
			{
				lowerName = baseMenu.Node.Id.ToLower();
				foreach(ControlAreaItem definitionItem in definitions)
				{
					panels = definitionItem.FrontAreas.FindAll(x =>
						x.Intent == ImpliedDesignIntentEnum.MenuPanel &&
						x.Reference.ToLower() == lowerName);
					foreach(ControlAreaItem panelItem in panels)
					{
						//	The panel isn't directly rendered.
						menuItems = panelItem.FrontAreas.FindAll(x =>
							x.Intent == ImpliedDesignIntentEnum.MenuItem ||
							x.Intent == ImpliedDesignIntentEnum.Text);
						foreach(ControlAreaItem menuItem in menuItems)
						{
							node = new HtmlNodeItem()
							{
								NodeType = "MenuItem",
								SelfClosing = (menuItem.FrontAreas.Count == 0)
							};
							SetRenderedControlName(menuItem.Node, node);
							node.Attributes.SetAttribute(
								"Header", FormatShortcut(GetText(menuItem)));
							targetNodes.Add(node);
							AddMenuPanels(node.Nodes, menuItem, definitions);
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AppendNodes																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Append the style node structure to the target nodes collection.
		/// </summary>
		/// <param name="styleNodes">
		/// Reference to the source style nodes collection to append.
		/// </param>
		/// <param name="target">
		/// Reference to the target node collection receiving the new nodes.
		/// </param>
		private static void AppendNodes(NameValueNodesCollection styleNodes,
			HtmlNodeCollection target)
		{
			HtmlNodeItem node = null;

			if(styleNodes?.Count > 0 && target != null)
			{
				foreach(NameValueNodesItem styleItem in styleNodes)
				{
					if(styleItem.Name?.Length > 0)
					{
						node = new HtmlNodeItem()
						{
							NodeType = styleItem.Name,
							SelfClosing = (styleItem.Nodes.Count == 0)
						};
						foreach(NameValueItem propertyItem in styleItem.Properties)
						{
							if(propertyItem.Name?.Length > 0)
							{
								node.Attributes.SetAttribute(propertyItem.Name,
									propertyItem.Value);
							}
						}
						target.Add(node);
						AppendNodes(styleItem.Nodes, node.Nodes);
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AppendStyle																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Append the style node structure to the target nodes collection.
		/// </summary>
		/// <param name="extension">
		/// Reference to the source style extension to append.
		/// </param>
		/// <param name="target">
		/// Reference to the target node collection receiving the new nodes.
		/// </param>
		private static void AppendStyle(ShapeStyleExtensionItem extension,
			HtmlNodeCollection target)
		{
			HtmlNodeItem childNode = null;
			HtmlNodeItem node = null;

			if(extension != null && target != null)
			{
				node = new HtmlNodeItem()
				{
					NodeType = "Style"
				};
				if(extension.Selector.Length > 0)
				{
					node.Attributes.SetAttribute("Selector", extension.Selector);
				}
				target.Add(node);
				foreach(NameValueNodesItem settingItem in extension.Settings)
				{
					if(settingItem.Name?.Length > 0)
					{
						childNode = new HtmlNodeItem()
						{
							NodeType = "Setter",
							SelfClosing = (settingItem.Nodes.Count == 0)
						};
						childNode.Attributes.SetAttribute("Property", settingItem.Name);
						if(settingItem.Value.Length > 0 || settingItem.Nodes.Count == 0)
						{
							childNode.Attributes.SetAttribute("Value", settingItem.Value);
						}
						node.Nodes.Add(childNode);
						if(settingItem.Nodes.Count > 0)
						{
							AppendNodes(settingItem.Nodes, childNode.Nodes);
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

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
						case "dock":
							target.Attributes.SetAttribute(
								"DockPanel.Dock", attributeItem.Value);
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
		//* ApplyStyleExtensions																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Apply style extension definitions from the supplied worksheets.
		/// </summary>
		/// <param name="node">
		/// Reference to the output node to which the extensions will be applied.
		/// </param>
		private void ApplyStyleExtensions(HtmlNodeItem node)
		{
			HtmlNodeItem childNode = null;
			HtmlNodeItem containerNode = null;
			string lowerName = "";

			if(node != null && mStyleCatalog.Count > 0)
			{
				foreach(ShapeStyleExtensionListCollection listCollectionItem in
					mStyleCatalog)
				{
					foreach(ShapeStyleExtensionListItem listItem in listCollectionItem)
					{
						if(listItem.ShapeNames.Contains(node.Attributes.GetValue("x:Name"),
							StringComparer.OrdinalIgnoreCase))
						{
							foreach(ShapeStyleExtensionItem extensionItem in
								listItem.Extensions)
							{
								switch(extensionItem.ExtensionType)
								{
									case ShapeStyleExtensionType.ItemsPanel:
										lowerName = $"{node.NodeType.ToLower()}.itemspanel";
										childNode = node.Nodes.FirstOrDefault(x =>
											x.NodeType.ToLower() == lowerName);
										if(childNode == null)
										{
											childNode = new HtmlNodeItem()
											{
												NodeType = $"{node.NodeType}.ItemsPanel",
												SelfClosing = false
											};
											node.Nodes.Insert(0, childNode);
										}
										else
										{
											childNode.Nodes.Clear();
										}
										containerNode = new HtmlNodeItem()
										{
											NodeType = "ItemsPanelTemplate",
											SelfClosing = false
										};
										childNode.Nodes.Add(containerNode);
										AppendNodes(extensionItem.Settings, containerNode.Nodes);
										break;
									case ShapeStyleExtensionType.Style:
										lowerName = $"{node.NodeType.ToLower()}.styles";
										containerNode = node.Nodes.FirstOrDefault(x =>
											x.NodeType.ToLower() == lowerName);
										if(containerNode == null)
										{
											containerNode = new HtmlNodeItem()
											{
												NodeType = $"{node.NodeType}.Styles",
												SelfClosing = false
											};
											node.Nodes.Insert(0, containerNode);
										}
										AppendStyle(extensionItem, containerNode.Nodes);
										break;
									case ShapeStyleExtensionType.Template:
										break;
								}
							}
						}
					}
				}
			}
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
		//* FillTreeViewItems																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Fill a set of TreeView items from the provided collection of area
		/// references.
		/// </summary>
		/// <param name="references">
		/// Reference to a collection of control area references to follow.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the current rendering token for this level.
		/// </param>
		/// <param name="nodes">
		/// Reference to a collection of HTML nodes prepared to receive the new
		/// collection of nodes.
		/// </param>
		private void FillTreeViewItems(
			ControlReferenceCollection references, RenderTokenItem renderToken,
			HtmlNodeCollection nodes)
		{
			ControlAreaItem area = null;
			HtmlNodeItem node = null;

			if(references?.Count > 0 && nodes != null)
			{
				foreach(ControlReferenceItem referenceItem in references)
				{
					area = referenceItem.Area;
					node = RenderTreeViewItem(area, renderToken);
					if(node != null)
					{
						nodes.Add(node);
						FillTreeViewItems(referenceItem.References,
							renderToken, node.Nodes);
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderButton																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render the button output.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <param name="buttonType">
		/// The type of button to render. Default = "Button".
		/// </param>
		/// <param name="captionProperty">
		/// The name of the property used for the caption.
		/// </param>
		/// <returns>
		/// XAML node representing the Button control.
		/// </returns>
		private HtmlNodeItem RenderButton(ControlAreaItem area,
			RenderTokenItem renderToken, string buttonType = "Button",
			string captionProperty = "Content")
		{
			HtmlNodeItem childNode = null;
			RenderTokenItem childToken = null;
			HtmlNodeItem containerNode = null;
			ControlAreaItem firstArea = null;
			ControlAreaItem imageArea = null;
			OrthogonalOrientationEnum orientation = OrthogonalOrientationEnum.None;
			HtmlNodeItem result = null;
			ControlAreaItem secondArea = null;
			int state = 0;
			string text = "";
			ControlAreaItem textArea = null;

			if(area != null && buttonType?.Length > 0)
			{
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
							NodeType = buttonType,
							SelfClosing = true
						};
						result.Attributes.SetAttribute(captionProperty, text);
						SetRenderedControlName(area.Node, result);
						break;
					case 2:
						//	Images only, no text.
						imageArea = GetImageArea(area);
						result = new HtmlNodeItem()
						{
							NodeType = buttonType,
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
								NodeType = buttonType,
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
								case OrthogonalOrientationEnum.Horizontal:
								case OrthogonalOrientationEnum.None:
									containerNode.Attributes.SetAttribute(
										"Orientation", "Horizontal");
									break;
								case OrthogonalOrientationEnum.Vertical:
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
									containerNode.Nodes.Add(childNode);
								}
								secondArea =
									(imageArea == firstArea ? textArea : imageArea);
								if(secondArea != null)
								{
									childNode = RenderOutputNode(secondArea, childToken);
									if(childNode != null)
									{
										containerNode.Nodes.Add(childNode);
									}
								}
							}
						}
						break;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderCheckBox																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return a XAML node for a CheckBox control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the CheckBox control.
		/// </returns>
		private HtmlNodeItem RenderCheckBox(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			HtmlNodeItem result = null;
			string text = "";

			if(area != null)
			{
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
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderComboBox																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a ComboBox control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the CheckBox control.
		/// </returns>
		private HtmlNodeItem RenderComboBox(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			List<ControlAreaItem> areas = null;
			HtmlNodeItem childNode = null;
			HtmlNodeItem result = null;

			if(area != null)
			{
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
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderDockPanel																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a DockPanel control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <param name="childToken">
		/// Reference to the rendering token for the next level.
		/// </param>
		/// <returns>
		/// XAML node representing the DockPanel control.
		/// </returns>
		private HtmlNodeItem RenderDockPanel(ControlAreaItem area,
			RenderTokenItem renderToken, RenderTokenItem childToken)
		{
			//	In the dock panel, remember that all controls have been ordered from
			//	left to right, top to bottom, so they will need to be given placement
			//	priority through their original node's AbsoluteIndex property.

			List<HtmlNodeItem> nodes = null;
			List<ControlAreaItem> orderedAreas = null;
			HtmlNodeItem result = null;

			if(area != null)
			{
				result = new HtmlNodeItem()
				{
					NodeType = "DockPanel",
					SelfClosing = false
				};
				SetRenderedControlName(area.Node, result);
				orderedAreas =
					area.FrontAreas.FindAll(x => x.Node != null).
					OrderBy(y => y.Node.AbsoluteIndex).ToList();
				nodes = RenderOutputNodes(orderedAreas, childToken);
				foreach(HtmlNodeItem nodeItem in nodes)
				{
					result.Nodes.Add(nodeItem);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderFlowPanel																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a FlowPanel control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <param name="childToken">
		/// Reference to the rendering token for the next level.
		/// </param>
		/// <returns>
		/// XAML node representing the FlowPanel control.
		/// </returns>
		private HtmlNodeItem RenderFlowPanel(ControlAreaItem area,
			RenderTokenItem renderToken, RenderTokenItem childToken)
		{
			List<HtmlNodeItem> nodes = null;
			HtmlNodeItem result = null;

			if(area != null)
			{
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
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderForm																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a Form control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <param name="childToken">
		/// Reference to the rendering token for the next level.
		/// </param>
		/// <returns>
		/// XAML node representing the FlowPanel control.
		/// </returns>
		private HtmlNodeItem RenderForm(ControlAreaItem area,
			RenderTokenItem renderToken, RenderTokenItem childToken)
		{
			List<HtmlNodeItem> nodes = null;
			HtmlNodeItem result = null;

			if(area != null)
			{
				nodes = RenderOutputNodes(area.FrontAreas, childToken);
				foreach(HtmlNodeItem nodeItem in nodes)
				{
					result.Nodes.Add(nodeItem);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderGrid																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a Grid control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <param name="childToken">
		/// Reference to the rendering token for the next level.
		/// </param>
		/// <returns>
		/// XAML node representing the Grid control.
		/// </returns>
		private HtmlNodeItem RenderGrid(ControlAreaItem area,
			RenderTokenItem renderToken, RenderTokenItem childToken)
		{
			string attributeValue = "";
			ControlAreaItem cellArea = null;
			int colIndex = 0;
			int gridColCount = 0;
			string[] gridColDims = null;
			ControlReferenceCollection gridCols = null;
			int gridRowCount = 0;
			string[] gridRowDims = null;
			ControlReferenceCollection gridRows = null;
			HtmlNodeItem node = null;
			HtmlNodeItem result = null;
			int rowIndex = 0;

			//	TODO: Update GetColumns and GetRows to allow every non-equal space
			//	to be returned as separate columns and rows so a grid can be built
			//	where Grid.ColumnSpan and Grid.RowSpan properties are implemented.
			if(area != null)
			{
				gridRows = GetRows(area.FrontAreas);
				gridRowCount = gridRows.Count;
				gridRowDims = new string[gridRowCount];
				for(rowIndex = 0; rowIndex < gridRowCount; rowIndex++)
				{
					gridRowDims[rowIndex] = "*";
				}
				gridCols = GetColumns(area.FrontAreas);
				gridColCount = gridCols.Count;
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
				rowIndex = 0;
				foreach(ControlReferenceItem gridRowItem in gridRows)
				{
					colIndex = 0;
					foreach(ControlReferenceItem gridColItem in gridCols)
					{
						childToken.Properties.SetValue(
							"GridColumnIndex", colIndex.ToString());
						childToken.Properties.SetValue(
							"GridRowIndex", rowIndex.ToString());
						foreach(ControlReferenceItem referenceItem in
							gridColItem.References)
						{
							cellArea = referenceItem.Area;
							if(gridRowItem.References.Exists(item =>
								item.Area == cellArea))
							{
								//	The item appears in this row.
								node = RenderOutputNode(cellArea, childToken);
								if(node != null)
								{
									//	Assign the child-specified grid and column dimensions.
									if(colIndex > -1 && rowIndex > -1)
									{
										attributeValue =
											cellArea.Node.Attributes.GetValue("ColumnWidth");
										if(attributeValue?.Length > 0)
										{
											gridColDims[colIndex] = attributeValue;
										}
										attributeValue =
											cellArea.Node.Attributes.GetValue("RowHeight");
										if(attributeValue?.Length > 0)
										{
											gridRowDims[rowIndex] = attributeValue;
										}
									}
									result.Nodes.Add(node);
								}
							}
						}
						colIndex++;
					}
					rowIndex++;
				}
				result.Attributes.SetAttribute(
					"RowDefinitions", string.Join(',', gridRowDims));
				result.Attributes.SetAttribute(
					"ColumnDefinitions", string.Join(',', gridColDims));
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderGridView																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a GridView control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the GridView control.
		/// </returns>
		private HtmlNodeItem RenderGridView(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			HtmlNodeItem childNode = null;
			int colIndex = 0;
			int gridColCount = 0;
			ControlReferenceCollection gridCols = null;
			string[] gridColDims = null;
			HtmlNodeItem node = null;
			HtmlNodeItem result = null;

			if(area != null)
			{
				gridCols = GetColumns(area.FrontAreas);
				gridColCount = gridCols.Count;
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
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderGroupBox																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a GroupBox control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <param name="childToken">
		/// Reference to the rendering token for the next level.
		/// </param>
		/// <returns>
		/// XAML node representing the GroupBox control.
		/// </returns>
		private HtmlNodeItem RenderGroupBox(ControlAreaItem area,
			RenderTokenItem renderToken, RenderTokenItem childToken)
		{
			HtmlNodeItem result = null;
			ControlAreaItem textArea = null;

			if(area != null)
			{
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
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderHorizontalGrid																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a HorizontalGrid control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <param name="childToken">
		/// Reference to the rendering token for the next level.
		/// </param>
		/// <returns>
		/// XAML node representing the HorizontalGrid control.
		/// </returns>
		private HtmlNodeItem RenderHorizontalGrid(ControlAreaItem area,
			RenderTokenItem renderToken, RenderTokenItem childToken)
		{
			//	Avalonia doesn't have a horizontal grid.
			//	However, their normal grid supports columns-only operation.
			//	The direct front area items will be the only participants
			//	in a grid layout.
			string attributeValue = "";
			ControlAreaItem cellArea = null;
			int colIndex = 0;
			int gridColCount = 0;
			string[] gridColDims = null;
			ControlReferenceCollection gridCols = null;
			HtmlNodeItem node = null;
			HtmlNodeItem result = null;

			if(area != null)
			{
				gridCols = GetColumns(area.FrontAreas);
				gridColCount = gridCols.Count;
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
				colIndex = 0;
				foreach(ControlReferenceItem gridColItem in gridCols)
				{
					childToken.Properties.SetValue(
						"GridColumnIndex", colIndex.ToString());
					foreach(ControlReferenceItem referenceItem in
						gridColItem.References)
					{
						cellArea = referenceItem.Area;
						node = RenderOutputNode(cellArea, childToken);
						if(node != null)
						{
							//	Assign the child-specified column dimensions.
							attributeValue =
								cellArea.Node.Attributes.GetValue("ColumnWidth");
							if(attributeValue?.Length > 0)
							{
								gridColDims[colIndex] = attributeValue;
							}
							result.Nodes.Add(node);
						}
					}
					colIndex++;
				}
				result.Attributes.SetAttribute(
					"ColumnDefinitions", string.Join(',', gridColDims));
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderHorizontalScrollPanel																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a HorizontalScrollPanel
		/// control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <param name="childToken">
		/// Reference to the rendering token for the next level.
		/// </param>
		/// <returns>
		/// XAML node representing the HorizontalScrollPanel control.
		/// </returns>
		private HtmlNodeItem RenderHorizontalScrollPanel(ControlAreaItem area,
			RenderTokenItem renderToken, RenderTokenItem childToken)
		{
			HtmlNodeItem childNode = null;
			HtmlNodeItem result = null;

			if(area != null)
			{
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
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderHorizontalStackPanel																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a HorizontalStackPanel
		/// control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <param name="childToken">
		/// Reference to the rendering token for the next level.
		/// </param>
		/// <returns>
		/// XAML node representing the HorizontalStackPanel control.
		/// </returns>
		private HtmlNodeItem RenderHorizontalStackPanel(ControlAreaItem area,
			RenderTokenItem renderToken, RenderTokenItem childToken)
		{
			HtmlNodeItem childNode = null;
			HtmlNodeItem result = null;

			if(area != null)
			{
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
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderImage																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of an Image
		/// control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the Image control.
		/// </returns>
		private HtmlNodeItem RenderImage(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			ControlAreaItem imageArea = null;
			string imageName = "";
			HtmlNodeItem result = null;

			//	TODO: Handle inline vector-drawing from SVG assets.
			if(area != null)
			{
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
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderLabel																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a Label control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the Label control.
		/// </returns>
		private HtmlNodeItem RenderLabel(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			HtmlNodeItem result = null;

			if(area != null)
			{
				result = new HtmlNodeItem()
				{
					NodeType = "Label",
					SelfClosing = true
				};
				SetRenderedControlName(area.Node, result);
				result.Attributes.SetAttribute("Content", GetText(area));
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderListBox																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a ListBox control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the ListBox control.
		/// </returns>
		private HtmlNodeItem RenderListBox(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			string attributeValue = "";
			HtmlNodeItem childNode = null;
			HtmlNodeItem childNode2 = null;
			HtmlNodeItem childNode3 = null;
			HtmlNodeItem result = null;
			List<ControlAreaItem> segmentAreas = null;

			if(area != null)
			{
				result = new HtmlNodeItem()
				{
					NodeType = "ListBox",
					SelfClosing = false
				};
				SetRenderedControlName(area.Node, result);
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
							//	Filename syntax '/Assets/Images/{SourceFilename}'
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
									"x:Name", imageAreaItem.Node.Id);
								childNode3.Attributes.SetAttribute(
									"Stretch", "None");
								childNode3.Attributes.SetAttribute(
									"IsHitTestVisible", "False");
								attributeValue =
									imageAreaItem.Node.Attributes.GetValue("ImageFilename");
								if(attributeValue.Length == 0)
								{
									attributeValue = $"{imageAreaItem.Node.Id}.png";
								}
								if(attributeValue.Length > 0)
								{
									childNode3.Attributes.SetAttribute(
										"Source", $"/Assets/Images/{attributeValue}");
								}
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
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderListView																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a ListView control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the ListView control.
		/// </returns>
		private HtmlNodeItem RenderListView(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			string attributeValue = "";
			ControlAreaItem cellArea = null;
			HtmlNodeItem childNode = null;
			int colIndex = 0;
			//ControlReferenceItem gridCol = null;
			int gridColCount = 0;
			string[] gridColDims = null;
			ControlReferenceCollection gridCols = null;
			HtmlNodeItem node = null;
			HtmlNodeItem result = null;

			if(area != null)
			{
				gridCols = GetColumns(area.FrontAreas);
				gridColCount = gridCols.Count;
				gridColDims = new string[gridColCount];
				for(colIndex = 0; colIndex < gridColCount; colIndex++)
				{
					gridColDims[colIndex] = "*";
				}
				result = new HtmlNodeItem()
				{
					NodeType = "ListView",
					SelfClosing = false
				};
				SetRenderedControlName(area.Node, result);
				result.Attributes.SetAttribute(
					"Items", $"{{Binding {area.Node.Id}Items}}");
				if(area.FrontAreas.Count > 0)
				{
					node = new HtmlNodeItem()
					{
						NodeType = "ListBox.ItemTemplate",
						SelfClosing = false
					};
					result.Nodes.Add(node);
					childNode = new HtmlNodeItem()
					{
						NodeType = "DataTemplate",
						SelfClosing = false
					};
					node.Nodes.Add(childNode);
					node = new HtmlNodeItem()
					{
						NodeType = "Grid",
						SelfClosing = false
					};
					childNode.Nodes.Add(node);
					colIndex = 0;
					foreach(ControlReferenceItem gridColItem in gridCols)
					{
						foreach(ControlReferenceItem referenceItem in
							gridColItem.References)
						{
							cellArea = referenceItem.Area;
							if(cellArea.Intent == ImpliedDesignIntentEnum.Text)
							{
								childNode = new HtmlNodeItem()
								{
									NodeType = "TextBlock",
									SelfClosing = true
								};
								childNode.Attributes.SetAttribute("Text", GetText(cellArea));
								childNode.Attributes.SetAttribute(
									"Grid.Column", colIndex.ToString());
								attributeValue =
									cellArea.Node.Attributes.GetValue("ColumnWidth");
								if(attributeValue?.Length > 0)
								{
									gridColDims[colIndex] = attributeValue;
								}
								node.Nodes.Add(childNode);
							}
						}
						colIndex++;
					}
					node.Attributes.SetAttribute(
						"ColumnDefinitions", string.Join(',', gridColDims));
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderMenuBar																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a MenuBar control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the MenuBar control.
		/// </returns>
		private HtmlNodeItem RenderMenuBar(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			List<ControlAreaItem> baseMenuItems = null;
			HtmlNodeItem childNode = null;
			ControlAreaCollection definitions = null;
			string lowerName = "";
			HtmlNodeItem result = null;

			if(area != null)
			{
				result = new HtmlNodeItem()
				{
					NodeType = "Menu",
					SelfClosing = (area.FrontAreas.Count == 0)
				};
				SetRenderedControlName(area.Node, result);
				definitions = GetDefinitionAreas(area.FrontAreas);
				foreach(ControlAreaItem definitionItem in definitions)
				{
					baseMenuItems = definitionItem.FrontAreas.FindAll(x =>
						x.Intent == ImpliedDesignIntentEnum.MenuItem);
					foreach(ControlAreaItem menuItem in baseMenuItems)
					{
						lowerName = menuItem.Node.Id.ToLower();
						childNode = new HtmlNodeItem()
						{
							NodeType = "MenuItem",
							SelfClosing = (menuItem.FrontAreas.Count == 0)
						};
						SetRenderedControlName(menuItem.Node, childNode);
						childNode.Attributes.SetAttribute(
							"Header", FormatShortcut(GetText(menuItem)));
						result.Nodes.Add(childNode);
						AddMenuPanels(childNode.Nodes, menuItem, definitions);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderPanel																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a Panel control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <param name="childToken">
		/// Reference to the rendering token for the next level.
		/// </param>
		/// <returns>
		/// XAML node representing the Panel control.
		/// </returns>
		private HtmlNodeItem RenderPanel(ControlAreaItem area,
			RenderTokenItem renderToken, RenderTokenItem childToken)
		{
			List<HtmlNodeItem> nodes = null;
			OrthogonalOrientationEnum orientation = OrthogonalOrientationEnum.None;
			HtmlNodeItem result = null;

			//	In this version, Panel is going to auto-deduce implementation
			//	between VerticalStackPanel, HorizontalStackPanel, and Grid.
			if(area != null)
			{
				//if(area.Node.Id == "rect97")
				//{
				//	Trace.WriteLine("ImpliedFormDesignAXaml.RenderPanel: Break here...");
				//}
				orientation = GetOrientation(area.FrontAreas);
				switch(orientation)
				{
					case OrthogonalOrientationEnum.Grid:
						result = RenderGrid(area, renderToken, childToken);
						break;
					case OrthogonalOrientationEnum.Horizontal:
						result = RenderHorizontalStackPanel(area, renderToken, childToken);
						break;
					case OrthogonalOrientationEnum.Vertical:
						result = RenderVerticalStackPanel(area, renderToken, childToken);
						break;
					default:
						result = new HtmlNodeItem()
						{
							NodeType = "Panel",
							SelfClosing = false
						};
						SetRenderedControlName(area.Node, result);
						nodes = RenderOutputNodes(area.FrontAreas, childToken);
						foreach(HtmlNodeItem nodeItem in nodes)
						{
							result.Nodes.Add(nodeItem);
						}
						break;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderProgressBar																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a ProgressBar control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the ProgressBar control.
		/// </returns>
		private HtmlNodeItem RenderProgressBar(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			HtmlNodeItem result = null;

			if(area != null)
			{
				result = new HtmlNodeItem()
				{
					NodeType = "ProgressBar",
					SelfClosing = true
				};
				SetRenderedControlName(area.Node, result);
				TransferAttribute(area.Node, "Maximum", result, "Maximum", "100");
				TransferAttribute(area.Node, "Minimum", result, "Minimum", "0");
				TransferAttribute(area.Node, "Value", result, "Value", "0");
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderRadioButton																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a RadioButton control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the RadioButton control.
		/// </returns>
		private HtmlNodeItem RenderRadioButton(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			HtmlNodeItem result = null;

			if(area != null)
			{
				result = new HtmlNodeItem()
				{
					NodeType = "RadioButton",
					SelfClosing = true
				};
				SetRenderedControlName(area.Node, result);
				TransferAttribute(area.Node, "GroupName", result, "GroupName");
				TransferAttribute(area.Node, "Content", result, "Content");
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderScrollPanel																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a ScrollPanel
		/// control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <param name="childToken">
		/// Reference to the rendering token for the next level.
		/// </param>
		/// <returns>
		/// XAML node representing the ScrollPanel control.
		/// </returns>
		private HtmlNodeItem RenderScrollPanel(ControlAreaItem area,
			RenderTokenItem renderToken, RenderTokenItem childToken)
		{
			HtmlNodeItem result = null;

			if(area != null)
			{
				result = new HtmlNodeItem()
				{
					NodeType = "ScrollViewer",
					SelfClosing = false
				};
				SetRenderedControlName(area.Node, result);
				result.Attributes.SetAttribute(
					"HorizontalScrollBarVisibility", "Auto");
				result.Attributes.SetAttribute(
					"VerticalScrollBarVisibility", "Auto");
				result.Nodes.AddRange(
					RenderOutputNodes(area.FrontAreas, childToken));
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderSlider																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a Slider control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the Slider control.
		/// </returns>
		private HtmlNodeItem RenderSlider(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			HtmlNodeItem result = null;

			if(area != null)
			{
				result = new HtmlNodeItem()
				{
					NodeType = "Slider",
					SelfClosing = true
				};
				SetRenderedControlName(area.Node, result);
				TransferAttribute(area.Node, "Maximum", result, "Maximum", "100");
				TransferAttribute(area.Node, "Minimum", result, "Minimum", "0");
				TransferAttribute(area.Node, "Value", result, "Value", "0");
				TransferAttribute(area.Node, "Frequency", result, "TickFrequency");
				TransferAttribute(area.Node, "Orientation", result, "Orientation");
				TransferAttribute(area.Node, "Snap", result, "IsSnapToTickEnabled");
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderSplitPanel																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a SplitPanel
		/// control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <param name="childToken">
		/// Reference to the rendering token for the next level.
		/// </param>
		/// <returns>
		/// XAML node representing the SplitPanel control.
		/// </returns>
		private HtmlNodeItem RenderSplitPanel(ControlAreaItem area,
			RenderTokenItem renderToken, RenderTokenItem childToken)
		{
			string attributeValue = "";
			HtmlNodeItem childNode = null;
			HtmlNodeItem node = null;
			OrthogonalOrientationEnum orientation = OrthogonalOrientationEnum.None;
			HtmlNodeItem result = null;

			if(area != null)
			{
				result = new HtmlNodeItem()
				{
					NodeType = "Grid",
					SelfClosing = false
				};
				SetRenderedControlName(area.Node, result);
				if(area.FrontAreas.Count > 1)
				{
					orientation = GetOrientation(area.FrontAreas[0], area.FrontAreas[1]);
				}
				switch(orientation)
				{
					case OrthogonalOrientationEnum.Horizontal:
					case OrthogonalOrientationEnum.None:
						node = new HtmlNodeItem()
						{
							NodeType = "Grid.ColumnDefinitions",
							SelfClosing = false
						};
						attributeValue = area.Node.Attributes.GetValue("PanelASize");
						if(attributeValue.Length == 0)
						{
							attributeValue = "Auto";
						}
						childNode = new HtmlNodeItem()
						{
							NodeType = "ColumnDefinition",
							SelfClosing = true
						};
						childNode.Attributes.SetAttribute("Width", attributeValue);
						node.Nodes.Add(childNode);
						attributeValue = area.Node.Attributes.GetValue("SplitterSize");
						if(attributeValue.Length == 0)
						{
							attributeValue = "5";
						}
						childNode = new HtmlNodeItem()
						{
							NodeType = "ColumnDefinition",
							SelfClosing = true
						};
						childNode.Attributes.SetAttribute("Width", attributeValue);
						node.Nodes.Add(childNode);
						attributeValue = area.Node.Attributes.GetValue("PanelBSize");
						if(attributeValue.Length == 0)
						{
							attributeValue = "*";
						}
						childNode = new HtmlNodeItem()
						{
							NodeType = "ColumnDefinition",
							SelfClosing = true
						};
						childNode.Attributes.SetAttribute("Width", attributeValue);
						node.Nodes.Add(childNode);
						break;
					case OrthogonalOrientationEnum.Vertical:
						node = new HtmlNodeItem()
						{
							NodeType = "Grid.RowDefinitions",
							SelfClosing = false
						};
						attributeValue = area.Node.Attributes.GetValue("PanelASize");
						if(attributeValue.Length == 0)
						{
							attributeValue = "Auto";
						}
						childNode = new HtmlNodeItem()
						{
							NodeType = "RowDefinition",
							SelfClosing = true
						};
						childNode.Attributes.SetAttribute("Height", attributeValue);
						node.Nodes.Add(childNode);
						attributeValue = area.Node.Attributes.GetValue("SplitterSize");
						if(attributeValue.Length == 0)
						{
							attributeValue = "5";
						}
						childNode = new HtmlNodeItem()
						{
							NodeType = "RowDefinition",
							SelfClosing = true
						};
						childNode.Attributes.SetAttribute("Height", attributeValue);
						node.Nodes.Add(childNode);
						attributeValue = area.Node.Attributes.GetValue("PanelBSize");
						if(attributeValue.Length == 0)
						{
							attributeValue = "*";
						}
						childNode = new HtmlNodeItem()
						{
							NodeType = "RowDefinition",
							SelfClosing = true
						};
						childNode.Attributes.SetAttribute("Height", attributeValue);
						node.Nodes.Add(childNode);
						break;
				}
				result.Nodes.Add(node);
				if(area.FrontAreas.Count > 0)
				{
					switch(orientation)
					{
						case OrthogonalOrientationEnum.Horizontal:
						case OrthogonalOrientationEnum.None:
							childToken.Properties.SetValue("GridColumnIndex", "0");
							break;
						case OrthogonalOrientationEnum.Vertical:
							childToken.Properties.SetValue("GridRowIndex", "0");
							break;
					}
					result.Nodes.Add(RenderOutputNode(area.FrontAreas[0], childToken));
				}
				node = new HtmlNodeItem()
				{
					NodeType = "GridSplitter",
					SelfClosing = true
				};
				switch(orientation)
				{
					case OrthogonalOrientationEnum.Horizontal:
					case OrthogonalOrientationEnum.None:
						node.Attributes.SetAttribute("Grid.Column", "1");
						node.Attributes.SetAttribute("ResizeDirection", "Columns");
						break;
					case OrthogonalOrientationEnum.Vertical:
						node.Attributes.SetAttribute("Grid.Row", "1");
						node.Attributes.SetAttribute("ResizeDirection", "Rows");
						break;
				}
				node.Attributes.SetAttribute("Background", "DarkGray");
				node.Attributes.SetAttribute("HorizontalAlignment", "Stretch");
				node.Attributes.SetAttribute("VerticalAlignment", "Stretch");
				result.Nodes.Add(node);
				if(area.FrontAreas.Count > 1)
				{
					switch(orientation)
					{
						case OrthogonalOrientationEnum.Horizontal:
						case OrthogonalOrientationEnum.None:
							childToken.Properties.SetValue("GridColumnIndex", "2");
							break;
						case OrthogonalOrientationEnum.Vertical:
							childToken.Properties.SetValue("GridRowIndex", "2");
							break;
					}
					result.Nodes.Add(RenderOutputNode(area.FrontAreas[^1], childToken));
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderTabControl																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a TabControl
		/// control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <param name="childToken">
		/// Reference to the rendering token for the next level.
		/// </param>
		/// <returns>
		/// XAML node representing the TabControl control.
		/// </returns>
		private HtmlNodeItem RenderTabControl(ControlAreaItem area,
			RenderTokenItem renderToken, RenderTokenItem childToken)
		{
			ControlAreaCollection definitions = null;
			HtmlNodeItem node = null;
			NameValueItem property = null;
			HtmlNodeItem result = null;

			if(area != null)
			{
				result = new HtmlNodeItem()
				{
					NodeType = "TabControl",
					SelfClosing = false
				};
				SetRenderedControlName(area.Node, result);
				definitions = GetDefinitionAreas(area.FrontAreas);
				foreach(ControlAreaItem definitionItem in definitions)
				{
					node = new HtmlNodeItem()
					{
						NodeType = "TabItem",
						SelfClosing = false
					};
					SetRenderedControlName(definitionItem.Node, node);
					property = definitionItem.Properties["Header"];
					if(property?.Value?.Length > 0)
					{
						node.Attributes.SetAttribute("Header", property.Value);
					}
					node.Nodes.AddRange(
						RenderOutputNodes(definitionItem.FrontAreas, childToken));
					result.Nodes.Add(node);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderTextBlock																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a TextBlock control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the TextBlock control.
		/// </returns>
		private HtmlNodeItem RenderTextBlock(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			HtmlNodeItem result = null;

			if(area != null)
			{
				result = new HtmlNodeItem()
				{
					NodeType = "TextBlock",
					SelfClosing = true
				};
				SetRenderedControlName(area.Node, result);
				result.Attributes.SetAttribute("Text", GetText(area));
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderTextBox																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a TextBox control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the TextBox control.
		/// </returns>
		private HtmlNodeItem RenderTextBox(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			HtmlNodeItem result = null;
			string text = null;
			ControlAreaItem textArea = null;

			if(area != null)
			{
				result = new HtmlNodeItem()
				{
					NodeType = "TextBox",
					SelfClosing = true
				};
				SetRenderedControlName(area.Node, result);
				text = null;
				textArea = area.FrontAreas.FirstOrDefault(x =>
					x.Intent == ImpliedDesignIntentEnum.Text);
				if(textArea != null)
				{
					text = GetText(textArea);
					if(text.Length == 0)
					{
						text = null;
					}
				}
				TransferAttribute(area.Node, "AcceptsReturn", result, "AcceptsReturn");
				TransferAttribute(area.Node, "PasswordChar", result, "PasswordChar");
				TransferAttribute(area.Node, "Prompt", result, "Watermark");
				TransferAttribute(area.Node, "Text", result, "Text", text);
				TransferAttribute(area.Node, "TextWrapping", result, "TextWrapping");
				text = null;
				textArea = area.FrontAreas.FirstOrDefault(x =>
					x.Intent == ImpliedDesignIntentEnum.Label);
				if(textArea != null)
				{
					text = GetText(textArea);
					if(text.Length == 0)
					{
						text = null;
					}
				}
				TransferAttribute(textArea.Node, "Text",
					result, "assist:TextFieldAssist.Label", text);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderTextWithHelper																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a TextWithHelper control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the TextWithHelper control.
		/// </returns>
		private HtmlNodeItem RenderTextWithHelper(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			ControlAreaItem buttonArea = null;
			HtmlNodeItem childNode = null;
			HtmlNodeItem result = null;
			HtmlNodeItem node = null;
			string text = null;
			ControlAreaItem textArea = null;
			ControlAreaItem textBoxArea = null;

			if(area != null)
			{
				result = new HtmlNodeItem()
				{
					NodeType = "Grid",
					SelfClosing = false
				};
				SetRenderedControlName(area.Node, result);
				node = new HtmlNodeItem()
				{
					NodeType = "Grid.ColumnDefinitions",
					SelfClosing = false
				};
				childNode = new HtmlNodeItem()
				{
					NodeType = "ColumnDefinition",
					SelfClosing = true
				};
				childNode.Attributes.SetAttribute("Width", "*");
				node.Nodes.Add(childNode);
				childNode = new HtmlNodeItem()
				{
					NodeType = "ColumnDefinition",
					SelfClosing = true
				};
				childNode.Attributes.SetAttribute("Width", "Auto");
				node.Nodes.Add(childNode);
				result.Nodes.Add(node);
				textBoxArea = area.FrontAreas.FindMatch(x =>
					x.Intent == ImpliedDesignIntentEnum.TextBox);
				buttonArea = area.FrontAreas.FindMatch(x => x.Intent ==
					ImpliedDesignIntentEnum.Button);
				node = new HtmlNodeItem()
				{
					NodeType = "TextBox",
					SelfClosing = true
				};
				node.Attributes.SetAttribute("Grid.Column", "0");
				if(textBoxArea != null)
				{
					text = null;
					textArea = textBoxArea.FrontAreas.FirstOrDefault(x =>
						x.Intent == ImpliedDesignIntentEnum.Text);
					if(textArea != null)
					{
						text = GetText(textArea);
						if(text.Length == 0)
						{
							text = null;
						}
					}
					TransferAttribute(textArea.Node, "Text", node, "Text", text);
					TransferAttribute(area.Node, "Prompt", node, "Watermark");
					text = null;
					textArea = textBoxArea.FrontAreas.FirstOrDefault(x =>
						x.Intent == ImpliedDesignIntentEnum.Label);
					if(textArea != null)
					{
						text = GetText(textArea);
						if(text.Length == 0)
						{
							text = null;
						}
					}
					TransferAttribute(textArea.Node, "Text",
						node, "assist:TextFieldAssist.Label", text);
				}
				result.Nodes.Add(node);
				node = new HtmlNodeItem()
				{
					NodeType = "Button",
					SelfClosing = true
				};
				node.Attributes.SetAttribute("Grid.Column", "1");
				if(buttonArea != null)
				{
					textArea = buttonArea.FrontAreas.FirstOrDefault(x =>
						x.Intent == ImpliedDesignIntentEnum.Text);
					if(textArea != null)
					{
						text = GetText(textArea);
						if(text.Length == 0)
						{
							text = null;
						}
					}
					TransferAttribute(textArea.Node, "Content", node, "Content", text);
				}
				result.Nodes.Add(node);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderTreeView																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a TreeView control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the TreeView control.
		/// </returns>
		private HtmlNodeItem RenderTreeView(ControlAreaItem area,
			RenderTokenItem renderToken, RenderTokenItem childToken)
		{
			ControlAreaCollection definitions = null;
			List<ControlAreaItem> nodeList = null;
			HtmlNodeItem result = null;
			ControlReferenceCollection tree = null;

			if(area != null)
			{
				result = new HtmlNodeItem()
				{
					NodeType = "TreeView",
					SelfClosing = true
				};
				SetRenderedControlName(area.Node, result);
				definitions = GetDefinitionAreas(area.FrontAreas);
				nodeList = definitions.SelectMany(x => x.FrontAreas).ToList();
				tree = ControlReferenceCollection.CreateTree(nodeList);
				FillTreeViewItems(tree, childToken, result.Nodes);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderTreeViewItem																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render the output of an individual TreeViewItem.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the Button control.
		/// </returns>
		private HtmlNodeItem RenderTreeViewItem(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			HtmlNodeItem childNode = null;
			RenderTokenItem childToken = null;
			HtmlNodeItem containerNode = null;
			ControlAreaItem firstArea = null;
			ControlAreaItem imageArea = null;
			HtmlNodeItem node = null;
			OrthogonalOrientationEnum orientation = OrthogonalOrientationEnum.None;
			HtmlNodeItem result = null;
			ControlAreaItem secondArea = null;
			int state = 0;
			string text = "";
			ControlAreaItem textArea = null;

			if(area != null)
			{
				state =
					(HasImages(area) ? 0x2 : 0x0) |
					(HasText(area) ? 0x1 : 0x0);
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
							NodeType = "TreeViewItem",
							SelfClosing = true
						};
						result.Attributes.SetAttribute("Header", text);
						SetRenderedControlName(area.Node, result);
						break;
					case 2:
						//	Images only, no text.
						imageArea = GetImageArea(area);
						result = new HtmlNodeItem()
						{
							NodeType = "TreeViewItem",
							SelfClosing = false
						};
						SetRenderedControlName(area.Node, result);
						node = new HtmlNodeItem()
						{
							NodeType = "TreeViewItem.Header",
							SelfClosing = false
						};
						childNode = RenderOutputNode(imageArea, childToken);
						if(childNode != null)
						{
							node.Nodes.Add(childNode);
						}
						result.Nodes.Add(node);
						break;
					case 3:
						//	Text and images.
						imageArea = GetImageArea(area);
						textArea = GetTextArea(area);
						if(imageArea != null && textArea != null)
						{
							result = new HtmlNodeItem()
							{
								NodeType = "TreeViewItem",
								SelfClosing = false
							};
							SetRenderedControlName(area.Node, result);
							node = new HtmlNodeItem()
							{
								NodeType = "TreeViewItem.Header",
								SelfClosing = false
							};
							containerNode = new HtmlNodeItem()
							{
								NodeType = "StackPanel"
							};
							containerNode.Attributes.SetAttribute("Spacing", "5");
							node.Nodes.Add(containerNode);
							result.Nodes.Add(node);
							orientation = GetOrientation(imageArea, textArea);
							switch(orientation)
							{
								case OrthogonalOrientationEnum.Horizontal:
								case OrthogonalOrientationEnum.None:
									containerNode.Attributes.SetAttribute(
										"Orientation", "Horizontal");
									break;
								case OrthogonalOrientationEnum.Vertical:
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
									containerNode.Nodes.Add(childNode);
								}
								secondArea =
									(imageArea == firstArea ? textArea : imageArea);
								if(secondArea != null)
								{
									childNode = RenderOutputNode(secondArea, childToken);
									if(childNode != null)
									{
										containerNode.Nodes.Add(childNode);
									}
								}
							}
						}
						break;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderUpDown																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of an UpDown control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the UpDown control.
		/// </returns>
		private HtmlNodeItem RenderUpDown(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			int number = 0;
			HtmlNodeItem result = null;
			string text = null;
			ControlAreaItem textArea = null;

			if(area != null)
			{
				result = new HtmlNodeItem()
				{
					NodeType = "NumericUpDown",
					SelfClosing = true
				};
				SetRenderedControlName(area.Node, result);
				textArea = area.FrontAreas.FirstOrDefault(x =>
					x.Intent == ImpliedDesignIntentEnum.Text);
				if(textArea != null)
				{
					text = GetText(textArea);
					if(text.Length == 0)
					{
						text = null;
					}
					number = ToInt(text);
				}
				TransferAttribute(area.Node, "Minimum", result, "Minimum", "0");
				TransferAttribute(area.Node, "Maximum", result, "Maximum", "100");
				TransferAttribute(area.Node, "Value", result, "Value",
					number.ToString());
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderVerticalGrid																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a VerticalGrid.
		/// control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <param name="childToken">
		/// Reference to the rendering token for the next level.
		/// </param>
		/// <returns>
		/// XAML node representing the VerticalGrid control.
		/// </returns>
		private HtmlNodeItem RenderVerticalGrid(ControlAreaItem area,
			RenderTokenItem renderToken, RenderTokenItem childToken)
		{
			string attributeValue = "";
			ControlAreaItem cellArea = null;
			//ControlReferenceItem gridRow = null;
			int gridRowCount = 0;
			string[] gridRowDims = null;
			ControlReferenceCollection gridRows = null;
			HtmlNodeItem node = null;
			HtmlNodeItem result = null;
			int rowIndex = 0;

			if(area != null)
			{
				//	Avalonia doesn't have a vertical grid.
				//	However, their normal grid supports rows-only operation.
				//	The direct front area items will be the only participants
				//	in a grid layout.
				gridRows = GetRows(area.FrontAreas);
				gridRowCount = gridRows.Count;
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
				rowIndex = 0;
				foreach(ControlReferenceItem gridRowItem in gridRows)
				{
					foreach(ControlReferenceItem referenceItem in gridRowItem.References)
					{
						cellArea = referenceItem.Area;
						childToken.Properties.SetValue(
							"GridRowIndex", rowIndex.ToString());
						node = RenderOutputNode(cellArea, childToken);
						if(node != null)
						{
							//	Assign the child-specified row dimensions.
							attributeValue =
								cellArea.Node.Attributes.GetValue("RowHeight");
							if(attributeValue?.Length > 0)
							{
								gridRowDims[rowIndex] = attributeValue;
							}
							result.Nodes.Add(node);
						}
					}
					rowIndex++;
				}
				result.Attributes.SetAttribute(
					"RowDefinitions", string.Join(',', gridRowDims));
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderStatusBar																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a StatusBar control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the StatusBar control.
		/// </returns>
		private HtmlNodeItem RenderStatusBar(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			HtmlNodeItem node = null;
			HtmlNodeItem result = null;

			if(area != null)
			{
				result = new HtmlNodeItem()
				{
					NodeType = "Border",
					SelfClosing = false
				};
				result.Attributes.SetAttribute("Height", "30");
				node = new HtmlNodeItem()
				{
					NodeType = "TextBlock",
					SelfClosing = true
				};
				SetRenderedControlName(area.Node, node);
				node.Attributes.SetAttribute("Text", GetText(area));
				node.Attributes.SetAttribute("Margin", "10,0");
				result.Nodes.Add(node);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderToolBar																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a ToolBar control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// XAML node representing the ToolBar control.
		/// </returns>
		private HtmlNodeItem RenderToolBar(ControlAreaItem area,
			RenderTokenItem renderToken, RenderTokenItem childToken)
		{
			HtmlNodeItem childNode = null;
			HtmlNodeItem node = null;
			HtmlNodeItem result = null;

			if(area != null)
			{
				result = new HtmlNodeItem()
				{
					NodeType = "ToolBarTray",
					SelfClosing = false
				};
				node = new HtmlNodeItem()
				{
					NodeType = "ToolBar",
					SelfClosing = false
				};
				SetRenderedControlName(area.Node, node);
				foreach(ControlAreaItem areaItem in area.FrontAreas)
				{
					switch(areaItem.Intent)
					{
						case ImpliedDesignIntentEnum.Button:
							//	Process the button node.
							node.Nodes.Add(RenderOutputNode(areaItem, childToken));
							break;
						case ImpliedDesignIntentEnum.Separator:
							//	Process the separator node.
							childNode = new HtmlNodeItem()
							{
								NodeType = "Separator",
								SelfClosing = true
							};
							node.Nodes.Add(childNode);
							break;
					}
				}
				result.Nodes.Add(node);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderVerticalScrollPanel																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a VerticalScrollPanel.
		/// control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <param name="childToken">
		/// Reference to the rendering token for the next level.
		/// </param>
		/// <returns>
		/// XAML node representing the VerticalScrollPanel control.
		/// </returns>
		private HtmlNodeItem RenderVerticalScrollPanel(ControlAreaItem area,
			RenderTokenItem renderToken, RenderTokenItem childToken)
		{
			HtmlNodeItem childNode = null;
			HtmlNodeItem result = null;

			if(area != null)
			{
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
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderVerticalStackPanel																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render and return the XAML representation of a VerticalStackPanel.
		/// control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the dimensions, coordinates,
		/// source node, and intention for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <param name="childToken">
		/// Reference to the rendering token for the next level.
		/// </param>
		/// <returns>
		/// XAML node representing the VerticalStackPanel control.
		/// </returns>
		private HtmlNodeItem RenderVerticalStackPanel(ControlAreaItem area,
			RenderTokenItem renderToken, RenderTokenItem childToken)
		{
			HtmlNodeItem result = null;

			if(area != null)
			{
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
			}
			return result;
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
		private static void SetRenderedControlName(HtmlNodeItem sourceNode,
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
					//	TODO: Use (?i:(?<defaultName>{sourceNode.NodeType}\d+)) pattern.
					if(!attribute.Value.ToLower().StartsWith(
						sourceNode.NodeType.ToLower()))
					{
						//	If this control wasn't named with the default ID then
						//	apply the user-supplied name.
						targetNode.Attributes.SetAttribute("x:Name", attribute.Value);
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		////*-----------------------------------------------------------------------*
		////* FillForm																															*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Fill the form starting at the provided collection of areas.
		///// </summary>
		///// <param name="areas">
		///// Reference to the collection of control areas from which the form will
		///// be filled.
		///// </param>
		///// <param name="node">
		///// Reference to the output node.
		///// </param>
		//protected override void FillForm(ControlAreaCollection areas,
		//	HtmlNodeItem node)
		//{
		//	base.FillForm(areas, node);
		//}
		////*-----------------------------------------------------------------------*

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
			char[] charColon = new char[] { ':' };
			char[] charComma = new char[] { ',' };
			char[] charSemicolon = new char[] { ';' };
			RenderTokenItem childToken = null;
			HtmlNodeItem result = null;

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
						result = RenderButton(area, renderToken);
						break;
					case ImpliedDesignIntentEnum.CheckBox:
						//	A checkbox might have an integrated label or nothing else.
						result = RenderCheckBox(area, renderToken);
						break;
					case ImpliedDesignIntentEnum.ComboBox:
						result = RenderComboBox(area, renderToken);
						break;
					case ImpliedDesignIntentEnum.DockPanel:
						result = RenderDockPanel(area, renderToken, childToken);
						break;
					case ImpliedDesignIntentEnum.FlowPanel:
						result = RenderFlowPanel(area, renderToken, childToken);
						break;
					case ImpliedDesignIntentEnum.Form:
						//	Inner forms are not supported, but their controls can be
						//	rendered.
						result = RenderForm(area, renderToken, childToken);
						break;
					case ImpliedDesignIntentEnum.FormInformation:
						//	Form information is not rendered.
						break;
					case ImpliedDesignIntentEnum.Grid:
						//	The direct front area items will be the only participants
						//	in a grid layout.
						result = RenderGrid(area, renderToken, childToken);
						break;
					case ImpliedDesignIntentEnum.GridView:
						//	Avalonia doesn't explicitly have a GridView control, but
						//	they do have a DataGrid.
						result = RenderGridView(area, renderToken);
						break;
					case ImpliedDesignIntentEnum.GroupBox:
						result = RenderGroupBox(area, renderToken, childToken);
						break;
					case ImpliedDesignIntentEnum.HorizontalGrid:
						result = RenderHorizontalGrid(area, renderToken, childToken);
						break;
					case ImpliedDesignIntentEnum.HorizontalScrollPanel:
						result =
							RenderHorizontalScrollPanel(area, renderToken, childToken);
						break;
					case ImpliedDesignIntentEnum.HorizontalStackPanel:
						result = RenderHorizontalStackPanel(area, renderToken, childToken);
						break;
					case ImpliedDesignIntentEnum.Image:
						result = RenderImage(area, renderToken);
						break;
					case ImpliedDesignIntentEnum.Label:
						result = RenderLabel(area, renderToken);
						break;
					case ImpliedDesignIntentEnum.ListBox:
						result = RenderListBox(area, renderToken);
						break;
					case ImpliedDesignIntentEnum.ListView:
						result = RenderListView(area, renderToken);
						break;
					case ImpliedDesignIntentEnum.MenuBar:
						result = RenderMenuBar(area, renderToken);
						break;
					case ImpliedDesignIntentEnum.MenuItem:
					case ImpliedDesignIntentEnum.MenuPanel:
					case ImpliedDesignIntentEnum.None:
						//	Not rendered directly.
						break;
					case ImpliedDesignIntentEnum.Panel:
						result = RenderPanel(area, renderToken, childToken);
						break;
					case ImpliedDesignIntentEnum.PictureBox:
						result = RenderImage(area, renderToken);
						break;
					case ImpliedDesignIntentEnum.ProgressBar:
						result = RenderProgressBar(area, renderToken);
						break;
					case ImpliedDesignIntentEnum.RadioButton:
						result = RenderRadioButton(area, renderToken);
						break;
					case ImpliedDesignIntentEnum.ScrollPanel:
						result = RenderScrollPanel(area, renderToken, childToken);
						break;
					case ImpliedDesignIntentEnum.Slider:
						result = RenderSlider(area, renderToken);
						break;
					case ImpliedDesignIntentEnum.SplitPanel:
						result = RenderSplitPanel(area, renderToken, childToken);
						break;
					case ImpliedDesignIntentEnum.StaticPanel:
						//	For the time-being, this will be a normal panel. If
						//	we go ahead with statically-positioned things, we'll
						//	let you know.
						result = RenderPanel(area, renderToken, childToken);
						break;
					case ImpliedDesignIntentEnum.StatusBar:
						result = RenderStatusBar(area, renderToken);
						break;
					case ImpliedDesignIntentEnum.TabControl:
						result = RenderTabControl(area, renderToken, childToken);
						break;
					case ImpliedDesignIntentEnum.Text:
						result = RenderTextBlock(area, renderToken);
						break;
					case ImpliedDesignIntentEnum.TextBox:
						result = RenderTextBox(area, renderToken);
						break;
					case ImpliedDesignIntentEnum.TextWithHelper:
						result = RenderTextWithHelper(area, renderToken);
						break;
					case ImpliedDesignIntentEnum.ToolBar:
						result = RenderToolBar(area, renderToken, childToken);
						break;
					case ImpliedDesignIntentEnum.TreeView:
						result = RenderTreeView(area, renderToken, childToken);
						break;
					case ImpliedDesignIntentEnum.UpDown:
						result = RenderUpDown(area, renderToken);
						break;
					case ImpliedDesignIntentEnum.VerticalGrid:
						result = RenderVerticalGrid(area, renderToken, childToken);
						break;
					case ImpliedDesignIntentEnum.VerticalScrollPanel:
						result = RenderVerticalScrollPanel(area, renderToken, childToken);
						break;
					case ImpliedDesignIntentEnum.VerticalStackPanel:
						result = RenderVerticalStackPanel(area, renderToken, childToken);
						break;
				}
				if(result != null)
				{
					ApplyCommonProperties(area, result);
					ApplyTokenProperties(renderToken, result);
					ApplyStyleExtensions(result);
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
		//*	StyleCatalog																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="StyleCatalog">StyleCatalog</see>.
		/// </summary>
		private List<ShapeStyleExtensionListCollection> mStyleCatalog =
			new List<ShapeStyleExtensionListCollection>();
		/// <summary>
		/// Get a reference to the catalog shape styles used to render the output
		/// in this session.
		/// </summary>
		public List<ShapeStyleExtensionListCollection> StyleCatalog
		{
			get { return mStyleCatalog; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToXaml																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Translate the current control area content to Avalonia XAML and return
		/// the result to the caller.
		/// </summary>
		/// <param name="styleCatalog">
		/// Reference to an optional collection of style extension worksheets to
		/// load
		/// </param>
		/// <returns>
		/// The Avalonia XAML content representing the controls presented in
		/// the local control areas.
		/// </returns>
		public string ToXaml(
			List<ShapeStyleExtensionListCollection> styleCatalog = null)
		{
			HtmlNodeItem node = new HtmlNodeItem();

			mStyleCatalog.Clear();
			if(styleCatalog?.Count > 0)
			{
				foreach(ShapeStyleExtensionListCollection styleListItem in
					styleCatalog)
				{
					if(styleListItem.Count > 0)
					{
						mStyleCatalog.Add(styleListItem);
					}
				}
			}
			FillForm(mControlAreas, node);
			return node.Html;
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
