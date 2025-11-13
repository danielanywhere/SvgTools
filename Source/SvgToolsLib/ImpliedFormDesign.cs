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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ConversionCalc;
using Geometry;
using Html;
using SkiaSharp;
using static SvgToolsLib.SvgToolsUtil;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	ImpliedFormDesign																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Features and functionality for supporting the Implied Form Design
	/// technique.
	/// </summary>
	public class ImpliedFormDesign
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* FindLayersExtendingNodeId																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a collection of layers that extend the node having the specified
		/// ID.
		/// </summary>
		/// <param name="doc">
		/// Reference to the HTML document withing which to find the matching
		/// layers.
		/// </param>
		/// <param name="nodeId">
		/// The unique node ID being extended.
		/// </param>
		/// <returns>
		/// Reference to a collection containing the layer nodes extending the
		/// node with the specified ID, if found. Otherwise, an empty collection.
		/// </returns>
		private static List<HtmlNodeItem> FindLayersExtendingNodeId(
			HtmlDocument doc, string nodeId)
		{
			List<HtmlNodeItem> items = null;
			string lowerId = "";
			List<HtmlNodeItem> result = new List<HtmlNodeItem>();

			if(doc != null && nodeId?.Length > 0)
			{
				lowerId = nodeId.ToLower();
				items = doc.Nodes.FindMatches(x => x.NodeType.ToLower() == "g" &&
					x.Attributes.Exists(y =>
						y.Name.ToLower() == "inkscape:groupmode" &&
						y.Value.ToLower() == "layer") &&
					x.Attributes.Exists(y =>
						y.Name.ToLower() == "inkscape:label" &&
						(y.Value.ToLower() == lowerId ||
						y.Value.ToLower().StartsWith($"{lowerId}-"))));
				foreach(HtmlNodeItem nodeItem in items)
				{
					//Trace.WriteLine(
					//	" Found extension layer: " +
					//	HtmlAttributeCollection.GetAttributeValue(nodeItem,
					//		"inkscape:label"));
					result.Add(nodeItem);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ProcessNodeZOrder																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Process the form design Z order for the provided node.
		/// </summary>
		/// <param name="node">
		/// Reference to the node to be placed.
		/// </param>
		/// <param name="areas">
		/// Reference to the areas collection within which, the control area will
		/// be placed.
		/// </param>
		private static void ProcessNodeZOrder(HtmlNodeItem node,
			ControlAreaCollection areas)
		{
			ControlAreaItem area = null;

			if(node != null && areas != null)
			{
				foreach(HtmlNodeItem nodeItem in node.Nodes)
				{
					if(IsControl(nodeItem))
					{
						area = new ControlAreaItem()
						{
							X = GetX(nodeItem),
							Y = GetY(nodeItem),
							Width = GetWidth(nodeItem),
							Height = GetHeight(nodeItem),
							Node = nodeItem,
							Intent = GetIntent(nodeItem),
							Reference = GetReference(nodeItem)
						};
						ControlAreaCollection.PlaceInFront(area, areas);
					}
					//	Process the children of this node.
					ProcessNodeZOrder(nodeItem, areas);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		/// <summary>
		/// List of control types used in implied form design.
		/// </summary>
		protected static string[] mControlTypes = new string[]
		{
			"button", "checkbox", "combobox", "dockpanel", "gridview", "label",
			"listbox", "listview", "menubar", "menuitem", "menupanel", "picturebox",
			"progressbar", "radiobutton", "statusbar", "tabcontrol", "textbox",
			"textwithhelper", "toolbar", "trackbar", "treeview", "updown",
			"forminformation", "flowpanel", "grid", "groupbox", "horizontalgrid",
			"horizontalscrollpanel", "horizontalstackpanel", "panel", "scrollpanel",
			"splitpanel", "staticpanel", "verticalgrid", "verticalscrollpanel",
			"verticalstackpanel"
		};

		/// <summary>
		/// List of discrete control types.
		/// </summary>
		protected static string[] mDiscreteControlTypes = new string[]
		{
			"button", "checkbox", "combobox", "gridview", "label", "listbox",
			"listview", "menubar", "picturebox", "progressbar", "radiobutton",
			"statusbar", "tabcontrol", "textbox", "textwithhelper", "toolbar",
			"trackbar", "treeview", "updown"
		};

		/// <summary>
		/// List of space-occupying layout control types.
		/// </summary>
		protected static string[] mOrganizerControlTypes = new string[]
		{
			"dockpanel", "flowpanel",
			"grid", "groupbox", "horizontalgrid", "horizontalscrollpanel",
			"horizontalstackpanel", "panel", "scrollpanel", "splitpanel",
			"staticpanel", "verticalgrid", "verticalscrollpanel",
			"verticalstackpanel"
		};

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
		/// <param name="outputNode">
		/// Reference to the output node.
		/// </param>
		protected virtual void FillForm(ControlAreaCollection areas,
			HtmlNodeItem outputNode)
		{
			ControlAreaItem area = null;
			List<HtmlAttributeItem> attributes = null;
			ControlAreaCollection childAreas = null;
			ControlAreaItem formArea = null;
			ImpliedDesignIntentEnum intent = ImpliedDesignIntentEnum.None;
			List<ControlAreaItem> members = null;
			string name = "";
			HtmlNodeItem node = null;
			RenderTokenItem renderToken = new RenderTokenItem();
			string text = "";

			if(areas?.Count > 0 && outputNode != null)
			{
				//	Initialize the form.
				Clear(outputNode);
				outputNode.NodeType = "Window";
				outputNode.Attributes.SetAttribute(
					"xmlns", "https://github.com/avaloniaui");
				outputNode.Attributes.SetAttribute(
					"xmlns:x", "http://schemas.microsoft.com/winfx/2006/xaml");
				mFormWidth = Round(GetFormWidth(mSvg.Document), 0);
				mFormHeight = Round(GetFormHeight(mSvg.Document), 0);
				outputNode.Attributes.SetAttribute("Width",
					mFormWidth.ToString("0"));
				outputNode.Attributes.SetAttribute("Height",
					mFormHeight.ToString("0"));
				members = areas.FindMatches(x =>
					x.Intent == ImpliedDesignIntentEnum.FormInformation);
				foreach(ControlAreaItem memberItem in members)
				{
					if(memberItem.Node != null)
					{
						attributes = memberItem.Node.Attributes;
						foreach(HtmlAttributeItem attributeItem in attributes)
						{
							name = attributeItem.Name.ToLower();
							switch(name)
							{
								case "caption":
									//	Caption.
									outputNode.Attributes.SetAttribute(
										"Title", attributeItem.Value);
									break;
								case "projectname":
									mProjectName = attributeItem.Value;
									break;
								case "usebackgroundcolor":
									//	Value indicating whether to use background colors on
									//	dropped objects by default on this form.
									mUseBackgroundColor = ToBool(attributeItem.Value);
									break;
								case "usebordercolor":
									//	Value indicating whether to use border colors of drawing
									//	objects by default on this form.
									mUseBorderColor = ToBool(attributeItem.Value);
									break;
								case "useborderwidth":
									//	Value indicating whether to use border widths of drawing
									//	objects by default on this form.
									mUseBorderWidth = ToBool(attributeItem.Value);
									break;
								case "usecornerradius":
									//	Value indicating whether to use border corner radii of
									//	drawing objects by default on this form.
									mUseCornerRadius = ToBool(attributeItem.Value);
									break;
							}
						}
					}
				}
				//	Re-index to the base form.
				formArea =
					areas.FindMatch(x => x.Intent == ImpliedDesignIntentEnum.Form);
				if(formArea != null)
				{
					//	Finalize the caption.
					if(!outputNode.Attributes.Exists(x =>
						x.Name.ToLower() == "title" && x.Value.Length > 0))
					{
						text = GetFormCaption(formArea.Node);
						if(text.Length > 0)
						{
							outputNode.Attributes.SetAttribute("Title", text);
						}
					}
					//	Determine the initial layout.
					childAreas = formArea.FrontAreas;
					if(HasOrganizer(childAreas))
					{
						area = GetOrganizer(childAreas);
						node = RenderOutputNode(area, renderToken);
					}
					else
					{
						//	This item needs an organizer.
						intent = ImpliedDesignIntentEnum.VerticalGrid;
						node = new HtmlNodeItem()
						{
							NodeType = "rect"
						};
						node.Attributes.SetAttribute(
							"x", Round(formArea.X, 0).ToString());
						node.Attributes.SetAttribute(
							"y", Round(formArea.Y, 0).ToString());
						node.Attributes.SetAttribute(
							"width", Round(formArea.Width, 0).ToString());
						node.Attributes.SetAttribute(
							"height", Round(formArea.Height, 0).ToString());
						node.Attributes.SetAttribute(
							"Intent", intent.ToString());
						area = new ControlAreaItem()
						{
							X = formArea.X,
							Y = formArea.Y,
							Width = formArea.Width,
							Height = formArea.Height,
							Intent = intent,
							Node = node
						};
						area.FrontAreas.AddRange(childAreas);
						formArea.FrontAreas.Clear();
						formArea.FrontAreas.Add(area);
						node = RenderOutputNode(area, renderToken);
					}
					//	This item is a form and has an organizer.
					PerformLayout(area, node);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////* FillText																															*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Fill the provided builder with the text found in the specified control
		///// area.
		///// </summary>
		///// <param name="area">
		///// Reference to the area to inspect.
		///// </param>
		///// <param name="builder">
		///// Reference to the text builder to fill.
		///// </param>
		///// <param name="recursive">
		///// Value indicating whether to append text from all descendants
		///// </param>
		//private static void FillText(ControlAreaItem area, StringBuilder builder,
		//	bool recursive = true)
		//{
		//	if(area != null && builder != null)
		//	{
		//		if(area.Node?.InnerText.Length > 0)
		//		{
		//			builder.Append(ToStringFromHtml(area.Node.InnerText));
		//		}
		//	}
		//}
		////*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mConverter_ResolveBaseToValue																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Up-convert the supplied value for the provided external entry type.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Conversion event arguments.
		/// </param>
		private void mConverter_ResolveBaseToValue(object sender,
			ConversionEventArgs e)
		{
			string name = "";

			//	Relative CSS Unit Values.
			if(e != null)
			{
				name = e.Definition.Name;
				if(name == "vmax")
				{
					if(mFormWidth >= mFormHeight)
					{
						name = "vw";
					}
					else
					{
						name = "vh";
					}
				}
				else if(name == "vmin")
				{
					if(mFormWidth >= mFormHeight)
					{
						name = "vh";
					}
					else
					{
						name = "vw";
					}
				}
				switch(name)
				{
					case "ch":
						//	1ch = height in current font and size * 0.666666667.
						e.Value *= 1d /
							((double)mCurrentFontSize * 0.666666667d * e.Definition.Value);
						e.Handled = true;
						break;
					case "em":
						//	1em = 'M' in current font height.
						e.Value *= 1d / ((double)mCurrentFontSize * e.Definition.Value);
						e.Handled = true;
						break;
					case "ex":
						//	1ex = 'X' in current font height = 16px.
						e.Value *= 1d / ((double)mCurrentFontSize * e.Definition.Value);
						e.Handled = true;
						break;
					case "rem":
						//	1rem = 'M' in root font height.
						e.Value *= 1d / ((double)mRootFontSize * e.Definition.Value);
						e.Handled = true;
						break;
					case "vh":
						//	1vh = (viewport height / 100) pixels.
						e.Value *= 1d /
							(((double)mFormHeight / 100d) * e.Definition.Value);
						e.Handled = true;
						break;
					case "vw":
						//	1vw = (viewport width / 100) pixels.
						e.Value *= 1d / (((double)mFormWidth / 100d) * e.Definition.Value);
						e.Handled = true;
						break;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mConverter_ResolveValueToBase																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Down-convert the supplied value for the provided external entry type.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Conversion event arguments.
		/// </param>
		private void mConverter_ResolveValueToBase(object sender,
			ConversionEventArgs e)
		{
			string name = "";

			//	Relative CSS Unit Values.
			if(e != null)
			{
				name = e.Definition.Name;
				if(name == "vmax")
				{
					if(mFormWidth >= mFormHeight)
					{
						name = "vw";
					}
					else
					{
						name = "vh";
					}
				}
				else if(name == "vmin")
				{
					if(mFormWidth >= mFormHeight)
					{
						name = "vh";
					}
					else
					{
						name = "vw";
					}
				}
				switch(name)
				{
					case "ch":
						//	1ch = height in current font and size * 0.666666667.
						e.Value *=
							((double)mCurrentFontSize * 0.666666667d * e.Definition.Value);
						e.Handled = true;
						break;
					case "em":
						//	1em = 'M' in current font height.
						e.Value *= ((double)mCurrentFontSize * e.Definition.Value);
						e.Handled = true;
						break;
					case "ex":
						//	1ex = 'X' in current font height = 16px.
						e.Value *= ((double)mCurrentFontSize * e.Definition.Value);
						e.Handled = true;
						break;
					case "rem":
						//	1rem = 'M' in root font height.
						e.Value *= ((double)mRootFontSize * e.Definition.Value);
						e.Handled = true;
						break;
					case "vh":
						//	1vh = (viewport height / 100) pixels.
						e.Value *= (((double)mFormHeight / 100d) * e.Definition.Value);
						e.Handled = true;
						break;
					case "vw":
						//	1vw = (viewport width / 100) pixels.
						e.Value *= (((double)mFormWidth / 100d) * e.Definition.Value);
						e.Handled = true;
						break;
				}
			}
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
		protected virtual void PerformLayout(ControlAreaItem area,
			HtmlNodeItem outputNode)
		{

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
		protected virtual HtmlNodeItem RenderOutputNode(ControlAreaItem area,
			RenderTokenItem renderToken)
		{
			return null;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderOutputNodes																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a collection of output nodes that will be added to a layout
		/// panel or area.
		/// </summary>
		/// <param name="areas">
		/// Reference to the list of control areas containing the dimensions,
		/// coordinates, source node, and intentions for the output.
		/// </param>
		/// <param name="renderToken">
		/// Reference to the rendering state token provided by the parent.
		/// </param>
		/// <returns>
		/// Reference to a list of output HTML nodes, in the active dialect, that
		/// properly represent the caller's supplied control areas, if
		/// legitimate. Otherwise, an empty list.
		/// </returns>
		protected virtual List<HtmlNodeItem> RenderOutputNodes(
			List<ControlAreaItem> areas, RenderTokenItem renderToken)
		{
			HtmlNodeItem node = null;
			List<HtmlNodeItem> nodes = new List<HtmlNodeItem>();

			if(areas?.Count > 0)
			{
				foreach(ControlAreaItem areaItem in areas)
				{
					switch(areaItem.Intent)
					{
						case ImpliedDesignIntentEnum.Button:
						case ImpliedDesignIntentEnum.CheckBox:
						case ImpliedDesignIntentEnum.ComboBox:
						case ImpliedDesignIntentEnum.FlowPanel:
						case ImpliedDesignIntentEnum.Grid:
						case ImpliedDesignIntentEnum.GridView:
						case ImpliedDesignIntentEnum.GroupBox:
						case ImpliedDesignIntentEnum.HorizontalGrid:
						case ImpliedDesignIntentEnum.HorizontalScrollPanel:
						case ImpliedDesignIntentEnum.HorizontalStackPanel:
						case ImpliedDesignIntentEnum.Image:
						case ImpliedDesignIntentEnum.Label:
						case ImpliedDesignIntentEnum.ListBox:
						case ImpliedDesignIntentEnum.ListView:
						case ImpliedDesignIntentEnum.MenuBar:
						case ImpliedDesignIntentEnum.Panel:
						case ImpliedDesignIntentEnum.PictureBox:
						case ImpliedDesignIntentEnum.ProgressBar:
						case ImpliedDesignIntentEnum.RadioButton:
						case ImpliedDesignIntentEnum.ScrollPanel:
						case ImpliedDesignIntentEnum.Slider:
						case ImpliedDesignIntentEnum.SplitPanel:
						case ImpliedDesignIntentEnum.StaticPanel:
						case ImpliedDesignIntentEnum.StatusBar:
						case ImpliedDesignIntentEnum.TabControl:
						case ImpliedDesignIntentEnum.Text:
						case ImpliedDesignIntentEnum.TextBox:
						case ImpliedDesignIntentEnum.TextWithHelper:
						case ImpliedDesignIntentEnum.ToolBar:
						case ImpliedDesignIntentEnum.TreeView:
						case ImpliedDesignIntentEnum.UpDown:
						case ImpliedDesignIntentEnum.VerticalGrid:
						case ImpliedDesignIntentEnum.VerticalScrollPanel:
						case ImpliedDesignIntentEnum.VerticalStackPanel:
							node = RenderOutputNode(areaItem, renderToken);
							if(node != null)
							{
								nodes.Add(node);
							}
							break;
						case ImpliedDesignIntentEnum.Definitions:
						case ImpliedDesignIntentEnum.Form:
							//	These intents are skipped and their children are processed.
							nodes.AddRange(
								RenderOutputNodes(areaItem.FrontAreas, renderToken));
							break;
						case ImpliedDesignIntentEnum.FormInformation:
						case ImpliedDesignIntentEnum.MenuItem:
						case ImpliedDesignIntentEnum.MenuPanel:
						case ImpliedDesignIntentEnum.None:
							//	These intents are not rendered directly.
							break;
					}
				}
			}
			return nodes;
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new instance of the ImpliedFormDesign item.
		/// </summary>
		public ImpliedFormDesign()
		{
			ConversionDomainItem domain = null;

			mConverter = new ConversionCalc.Converter();

			mConverter.ResolveBaseToValue += mConverter_ResolveBaseToValue;
			mConverter.ResolveValueToBase += mConverter_ResolveValueToBase;

			//	Add relative Css Units to the Distance domain.
			domain = mConverter.Data.Domains.FirstOrDefault(x =>
				x.DomainName == "Distance");
			if(domain != null)
			{
				//	All of the CSS relative conversions in this example are
				//	based in pixels.
				domain.Conversions.AddRange(new ConversionDefinitionItem[]
				{
					new ConversionDefinitionItem()
					{
						EntryType = ConversionDefinitionEntryType.External,
						Name = "ch",
						Value = 0.002645833d
					},
					new ConversionDefinitionItem()
					{
						EntryType = ConversionDefinitionEntryType.External,
						Name = "em",
						Value = 0.002645833d
					},
					new ConversionDefinitionItem()
					{
						EntryType = ConversionDefinitionEntryType.External,
						Name = "ex",
						Value = 0.002645833d
					},
					new ConversionDefinitionItem()
					{
						EntryType = ConversionDefinitionEntryType.External,
						Name = "rem",
						Value = 0.002645833d
					},
					new ConversionDefinitionItem()
					{
						EntryType = ConversionDefinitionEntryType.External,
						Name = "vh",
						Value = 0.002645833d
					},
					new ConversionDefinitionItem()
					{
						EntryType = ConversionDefinitionEntryType.External,
						Name = "vmax",
						Value = 0.002645833d
					},
					new ConversionDefinitionItem()
					{
						EntryType = ConversionDefinitionEntryType.External,
						Name = "vmin",
						Value = 0.002645833d
					},
					new ConversionDefinitionItem()
					{
						EntryType = ConversionDefinitionEntryType.External,
						Name = "vw",
						Value = 0.002645833d
					}
				});
			}
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new instance of the ImpliedFormDesign item.
		/// </summary>
		public ImpliedFormDesign(SvgDocumentItem svgDocument) : this()
		{
			if(svgDocument != null)
			{
				mSvg = svgDocument;
				mControlAreas = EnumerateControls(svgDocument);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AddUserStyle																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add a user style abbreviation to the output nodes collection.
		/// </summary>
		/// <param name="nodes">
		/// Reference to the collection of nodes to which the style will be
		/// appended.
		/// </param>
		/// <param name="styleValue">
		/// The abbreviated user style value to append.
		/// </param>
		public static void AddUserStyle(HtmlNodeCollection nodes,
			string styleValue)
		{
			char[] charColon = new char[] { ':' };
			char[] charComma = new char[] { ',' };
			char[] charSemicolon = new char[] { ';' };
			HtmlNodeItem childNode = null;
			HtmlNodeItem node = null;
			string selector = "";
			string[] setters = new string[0];
			string[] values = null;

			if(nodes != null && styleValue?.Length > 0)
			{
				values = ToStringFromHtml(styleValue).Split(charSemicolon);
				if(values.Length > 0)
				{
					//selector = values[0].Replace("&quot;", "\"");
					selector = values[0];
					if(values.Length > 1)
					{
						setters = values[1].Split(charComma);
					}
					node = new HtmlNodeItem()
					{
						NodeType = "Style",
						SelfClosing = false
					};
					if(selector.Length > 0)
					{
						node.Attributes.SetAttribute("Selector",
							GetValue(selector, ResourceMain.rxNameEqualsValue, "value"));
					}
					foreach(string setterItem in setters)
					{
						childNode = new HtmlNodeItem()
						{
							NodeType = "Setter",
							SelfClosing = true
						};
						values = setterItem.Split(charColon);
						if(values.Length > 0)
						{
							childNode.Attributes.SetAttribute("Property", values[0]);
							if(values.Length > 1)
							{
								childNode.Attributes.SetAttribute("Value", values[1]);
							}
						}
						node.Nodes.Add(childNode);
					}
					nodes.Add(node);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Caption																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Caption">Caption</see>.
		/// </summary>
		protected string mCaption = "";
		/// <summary>
		/// Get/Set the caption of this form.
		/// </summary>
		public string Caption
		{
			get { return mCaption; }
			set { mCaption = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ControlAreas																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ControlAreas">ControlAreas</see>.
		/// </summary>
		protected ControlAreaCollection mControlAreas =
			new ControlAreaCollection();
		/// <summary>
		/// Get a reference to the collection of control areas currently active
		/// for this instance.
		/// </summary>
		public ControlAreaCollection ControlAreas
		{
			get { return mControlAreas; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ControlTypes																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get a reference to an array of control type names.
		/// </summary>
		public static string[] ControlTypes
		{
			get { return mControlTypes; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Converter																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Converter">Converter</see>.
		/// </summary>
		protected ConversionCalc.Converter mConverter =
			new ConversionCalc.Converter();
		/// <summary>
		/// Get a reference to the conversion calculator for this instance.
		/// </summary>
		public ConversionCalc.Converter Converter
		{
			get { return mConverter; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	CurrentFontSize																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="CurrentFontSize">CurrentFontSize</see>.
		/// </summary>
		protected float mCurrentFontSize = 16f;
		/// <summary>
		/// Get/Set the current font size, in pixels.
		/// </summary>
		/// <remarks>
		/// The default for this property is 16px (12pt).
		/// </remarks>
		public float CurrentFontSize
		{
			get { return mCurrentFontSize; }
			set { mCurrentFontSize = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* EnumerateControls																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Enumerate the controls on the form presented in the caller's SVG
		/// document.
		/// </summary>
		/// <param name="doc">
		/// Reference to the SVG document to enumerate.
		/// </param>
		/// <returns>
		/// Reference to a control area collection where all of the recognizable
		/// control references have been organized with the intended z-order, if
		/// found. Otherwise, null.
		/// </returns>
		/// <remarks>
		/// The X,Y placement order of the controls is not yet sorted at this
		/// stage.
		/// </remarks>
		public static ControlAreaCollection EnumerateControls(SvgDocumentItem doc)
		{
			ControlAreaItem area = null;
			ControlAreaCollection areas = null;
			MatchCollection assignments = null;
			HtmlAttributeItem attribute = null;
			List<ControlAreaItem> flatAreas = null;
			int indent = 1;
			List<HtmlNodeItem> layers = null;
			HtmlNodeItem nodeForm = null;

			if(doc != null)
			{
				//	Get the form.
				nodeForm = doc.Document.Nodes.FindMatch(x =>
					x.Attributes.Exists(y =>
						y.Name.ToLower() == "inkscape:groupmode" &&
						y.Value.ToLower() == "layer") &&
					x.Attributes.Exists(y =>
						y.Name.ToLower() == "inkscape:label" &&
						y.Value.ToLower().StartsWith("form")));
				if(nodeForm != null)
				{
					Trace.WriteLine(
						$" Form: {nodeForm.Id} " +
						$"({ImpliedFormDesign.GetFormCaption(nodeForm)})");
					indent++;
					areas = new ControlAreaCollection();
					area = new ControlAreaItem()
					{
						X = 0f,
						Y = 0f,
						Width = GetFormWidth(doc.Document),
						Height = GetFormHeight(doc.Document),
						Node = nodeForm,
						Intent = ImpliedDesignIntentEnum.Form
					};
					areas.Add(area);
					ProcessNodeZOrder(nodeForm, areas);
					flatAreas = ControlAreaCollection.GetFlatList(areas);
					foreach(ControlAreaItem areaItem in flatAreas)
					{
						if(areaItem.Node != null &&
							areaItem.Intent != ImpliedDesignIntentEnum.Form &&
							areaItem.Intent != ImpliedDesignIntentEnum.None)
						{
							layers = FindLayersExtendingNodeId(doc.Document,
								areaItem.Node.Id);
							foreach(HtmlNodeItem layerItem in layers)
							{
								area = new ControlAreaItem()
								{
									Intent = ImpliedDesignIntentEnum.Definitions
								};
								//	Assignments are in inkscape:label
								attribute = layerItem.Attributes["inkscape:label"];
								if(attribute != null)
								{
									assignments = Regex.Matches(attribute.Value,
										ResourceMain.rxNodeExtensionAssignment);
									foreach(Match matchItem in assignments)
									{
										area.Properties.SetValue(
											GetValue(matchItem, "name"),
											GetValue(matchItem, "value"));
									}
								}
								areaItem.FrontAreas.Add(area);
								ProcessNodeZOrder(layerItem, area.FrontAreas);
							}
						}
					}
				}
			}
			if(areas?.Count > 0)
			{
				ControlAreaCollection.SortPosition(areas);
			}
			return areas;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FormHeight																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="FormHeight">FormHeight</see>.
		/// </summary>
		protected float mFormHeight = 1080f;
		/// <summary>
		/// Get/Set the height of the current form.
		/// </summary>
		public float FormHeight
		{
			get { return mFormHeight; }
			set { mFormHeight = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FormatShortcut																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the caller's text with any shortcut key symbols consistently
		/// formatted.
		/// </summary>
		/// <param name="textWithShortcut">
		/// The text that might contain a shortcut decorator.
		/// </param>
		/// <returns>
		/// A version of the caller's text where the shortcut decorator has been
		/// formatted to be consistent, if legitimate. Otherwise, an empty string.
		/// </returns>
		public static string FormatShortcut(string textWithShortcut)
		{
			string result = "";

			if(textWithShortcut?.Length > 0)
			{
				result = textWithShortcut.Replace('&', '_');
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FormWidth																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="FormWidth">FormWidth</see>.
		/// </summary>
		private float mFormWidth = 1920f;
		/// <summary>
		/// Get/Set the width of the current form.
		/// </summary>
		public float FormWidth
		{
			get { return mFormWidth; }
			set { mFormWidth = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetColumn																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a reference to the column found at the specified location.
		/// </summary>
		/// <param name="areas">
		/// Reference to the collection of reference areas to inspect.
		/// </param>
		/// <param name="x">
		/// Location whose column will be identified.
		/// </param>
		/// <returns>
		/// Reference to the column at the specified position, if found.
		/// Otherwise, null.
		/// </returns>
		public static ControlReferenceItem GetColumn(
			ControlReferenceCollection areas, float x)
		{
			ControlReferenceItem result = null;

			foreach(ControlReferenceItem areaItem in areas)
			{
				if(areaItem.Left <= x && areaItem.Right >= x)
				{
					result = areaItem;
					break;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetColumns																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a collection of implied columns found at the current control
		/// layer.
		/// </summary>
		/// <param name="areas">
		/// Reference to the collection of areas to inspect.
		/// </param>
		/// <returns>
		/// Reference to a collection of columns found at the current control
		/// layer, if any controls were present. Otherwise, an empty collection.
		/// </returns>
		public static ControlReferenceCollection GetColumns(
			ControlAreaCollection areas)
		{
			FArea intersection = null;
			ControlReferenceItem reference = null;
			List<ControlReferenceItem> result = new List<ControlReferenceItem>();

			foreach(ControlAreaItem areaItem in areas)
			{
				if(areaItem.Intent != ImpliedDesignIntentEnum.Definitions)
				{
					intersection = null;
					foreach(ControlReferenceItem columnItem in result)
					{
						intersection = GetIntersectingArea(areaItem, columnItem);
						if(intersection != null)
						{
							columnItem.Left = Math.Min(columnItem.Left, areaItem.Left);
							columnItem.Right = Math.Max(columnItem.Right, areaItem.Right);
							columnItem.References.Add(areaItem);
							break;
						}
					}
					if(intersection == null)
					{
						reference = new ControlReferenceItem()
						{
							X = areaItem.X,
							Y = float.MinValue / 2f,
							Width = areaItem.Width,
							Height = float.MaxValue
						};
						reference.References.Add(areaItem);
						result.Add(reference);
					}
				}
				else
				{
					result.AddRange(GetColumns(areaItem.FrontAreas));
				}
			}
			return new ControlReferenceCollection(result.OrderBy(x => x.X).ToList());
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetDefinitionAreas																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a reference to the collection of areas serving as definition
		/// data for the set.
		/// </summary>
		/// <param name="areas">
		/// Reference to a collection of areas to inspect.
		/// </param>
		/// <returns>
		/// Reference to a collection of control areas that set the definition
		/// for the set. If one or more Definitions intents was found, the
		/// collection will contain the children of those items. Otherwise,
		/// the supplied areas collection will be returned. If the supplied areas
		/// collection was blank an empty collection is returned.
		/// </returns>
		public static ControlAreaCollection GetDefinitionAreas(
			ControlAreaCollection areas)
		{
			ControlAreaCollection result = new ControlAreaCollection();

			if(areas?.Count > 0)
			{
				result.AddRange(areas.FindMatches(x =>
					x.Intent == ImpliedDesignIntentEnum.Definitions));
				if(result.Count == 0)
				{
					result = areas;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetFirstArea																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the first of two areas in the orientation given.
		/// </summary>
		/// <param name="area1">
		/// Reference to the first area to review.
		/// </param>
		/// <param name="area2">
		/// Reference to the second area to review.
		/// </param>
		/// <param name="orientation">
		/// Rectilinear orientation at which to apply the test.
		/// </param>
		/// <returns>
		/// Reference to the first of two areas in the specified orientation, if
		/// found. Otherwise, a reference to the first provided item.
		/// </returns>
		public static ControlAreaItem GetFirstArea(ControlAreaItem area1,
			ControlAreaItem area2, RectilinearOrientationEnum orientation)
		{
			float center1 = 0f;
			float center2 = 0f;
			ControlAreaItem result = area1;

			if(area1 != null && area2 != null)
			{
				switch(orientation)
				{
					case RectilinearOrientationEnum.Horizontal:
						center1 = area1.X + (area1.Width / 2f);
						center2 = area2.X + (area2.Width / 2f);
						break;
					case RectilinearOrientationEnum.Vertical:
						center1 = area1.Y + (area1.Height / 2f);
						center2 = area2.Y + (area2.Height / 2f);
						break;
				}
				if(center1 > center2)
				{
					result = area2;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetFlatList																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a flat list representation of all of the areas in the provided
		/// tree.
		/// </summary>
		/// <param name="areas">
		/// Reference to the collection of areas to flatten.
		/// </param>
		/// <returns>
		/// Reference to a flat collection of areas found in the caller's
		/// collection.
		/// </returns>
		public static List<ControlAreaItem> GetFlatList(
			ControlAreaCollection areas)
		{
			List<ControlAreaItem> result = null;

			if(areas?.Count > 0)
			{
				result = areas.FindMatches(x =>
					x.Intent != ImpliedDesignIntentEnum.None);
			}
			if(result == null)
			{
				result = new List<ControlAreaItem>();
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetFormCaption																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the implied form design form caption from either the
		/// label or the Caption attribute.
		/// </summary>
		/// <param name="node">
		/// Reference to the node to check for identifying elements.
		/// </param>
		/// <returns>
		/// The caption found for the form node.
		/// </returns>
		public static string GetFormCaption(HtmlNodeItem node)
		{
			HtmlAttributeItem attribute = null;
			string caption = "";
			string label = "";

			if(node != null &&
				node.Attributes.Exists(x =>
					x.Name.ToLower() == "inkscape:groupmode" &&
					x.Value.ToLower() == "layer") &&
				node.Attributes.Exists(x =>
					x.Name.ToLower() == "inkscape:label" &&
					x.Value.ToLower().StartsWith("form")))
			{
				label = node.Attributes["inkscape:label"].Value;
				if(label.ToLower().StartsWith("form-") &&
					label.Length > 5)
				{
					//	Caption is available in the label.
					caption = label.Substring(5);
				}
				else
				{
					attribute = node.Attributes["caption"];
					if(attribute != null)
					{
						caption = attribute.Value;
					}
				}
			}
			return caption;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetFormHeight																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the height of the form.
		/// </summary>
		/// <param name="doc">
		/// Reference to the HTML document to search.
		/// </param>
		/// <returns>
		/// The height of the form, if found. Otherwise, 0.
		/// </returns>
		public static float GetFormHeight(HtmlDocument doc)
		{
			HtmlAttributeItem attribute = null;
			float result = 0f;
			HtmlNodeItem node = null;

			if(doc != null)
			{
				node = doc.Nodes.FindMatch(x => x.NodeType.ToLower() == "svg");
				if(node != null)
				{
					attribute = node.Attributes["height"];
					if(attribute != null && attribute.Value?.Length > 0)
					{
						result = (float)ToInt(attribute.Value);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetFormWidth																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the width of the form.
		/// </summary>
		/// <param name="doc">
		/// Reference to the HTML document to search.
		/// </param>
		/// <returns>
		/// The width of the form, if found. Otherwise, 0.
		/// </returns>
		public static float GetFormWidth(HtmlDocument doc)
		{
			HtmlAttributeItem attribute = null;
			float result = 0f;
			HtmlNodeItem node = null;

			if(doc != null)
			{
				node = doc.Nodes.FindMatch(x => x.NodeType.ToLower() == "svg");
				if(node != null)
				{
					attribute = node.Attributes["width"];
					if(attribute != null && attribute.Value?.Length > 0)
					{
						result = (float)ToInt(attribute.Value);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetHeight																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the height of the specified node.
		/// </summary>
		/// <param name="node">
		/// Reference to the node to check.
		/// </param>
		/// <returns>
		/// The height of the specified node.
		/// </returns>
		public static float GetHeight(HtmlNodeItem node)
		{
			BoundingObjectItem bounds = null;
			float result = 0f;

			if(node != null)
			{
				bounds = CalcBounds(node);
				if(bounds != null)
				{
					result = bounds.GetHeight();
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetImageArea																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the nearest image area to the provided control area.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area to search for an image type.
		/// </param>
		/// <returns>
		/// Reference to the first image-type control area, either at the
		/// provided area, or one of its descendants, if found. Otherwise, null.
		/// </returns>
		public static ControlAreaItem GetImageArea(ControlAreaItem area)
		{
			ControlAreaItem result = null;

			if(area != null)
			{
				if(area.Intent == ImpliedDesignIntentEnum.Image ||
					area.Intent == ImpliedDesignIntentEnum.PictureBox)
				{
					result = area;
				}
				else
				{
					foreach(ControlAreaItem areaItem in area.FrontAreas)
					{
						result = GetImageArea(areaItem);
						if(result != null)
						{
							break;
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetImageName																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the name representing the nearest image node.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area to search for an image.
		/// </param>
		/// <returns>
		/// The name of the image, as it should appear in the assets folder.
		/// </returns>
		public static string GetImageName(ControlAreaItem area)
		{
			ControlAreaItem image = null;
			string result = "";

			if(area != null)
			{
				image = GetImageArea(area);
				if(image != null)
				{
					result = GetImageName(image.Node);
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the name representing the image node.
		/// </summary>
		/// <param name="node">
		/// Reference to an image-like HTML node.
		/// </param>
		/// <returns>
		/// The name of the image, as it should appear in the assets folder.
		/// </returns>
		public static string GetImageName(HtmlNodeItem node)
		{
			HtmlAttributeItem attribute = null;
			bool bFilenameFound = false;
			string extension = "";
			string result = "";

			if(node != null)
			{
				attribute = node.Attributes["xlink:href"];
				if(attribute != null && attribute.Value?.Length > 0)
				{
					if(attribute.Value.StartsWith("data:"))
					{
						extension = Between(attribute.Value, "/", ";");
					}
					attribute = node.Attributes["id"];
					if(attribute?.Value?.Length > 0)
					{
						if(extension.Length == 0)
						{
							extension = "png";
						}
						result = $"{attribute.Value}.{extension}";
						bFilenameFound = true;
					}
				}
				else
				{
					attribute = node.Attributes["id"];
					if(attribute?.Value?.Length > 0)
					{
						result = $"{attribute.Value}.png";
						bFilenameFound = true;
					}
				}
				if(!bFilenameFound)
				{
					result = $"{Guid.NewGuid().ToString("D")}.png";
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetIntent																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the interpreted intent of the caller's node.
		/// </summary>
		/// <param name="node">
		/// Reference to the HTML node to inspect.
		/// </param>
		/// <returns>
		/// The implied form design intent of the provided node, if it could be
		/// found. Otherwise, none.
		/// </returns>
		public static ImpliedDesignIntentEnum GetIntent(HtmlNodeItem node)
		{
			HtmlAttributeItem attribute = null;
			ImpliedDesignIntentEnum intent = ImpliedDesignIntentEnum.None;
			string label = "";
			Match match = null;
			string nodeType = "";
			ImpliedDesignIntentEnum result = ImpliedDesignIntentEnum.None;
			string text = "";

			if(node != null)
			{
				//if(node.NodeType.ToLower() == "image")
				//{
				//	Debug.WriteLine("GetIntent: Break here...");
				//}
				if(!IsLayer(node))
				{
					//	If the node has a label, then that intent overrides the intent
					//	attribute.
					text = GetLabel(node);
					if(text.Length == 0)
					{
						attribute = node.Attributes["intent"];
						if(attribute != null && attribute.Value?.Length > 0)
						{
							text = attribute.Value;
						}
					}
					if(text.Length > 0)
					{
						match = Regex.Match(text, ResourceMain.rxIntentWithLabel);
						if(match.Success)
						{
							label = GetValue(match, "intent");
							if(Enum.TryParse<ImpliedDesignIntentEnum>(
								label, true, out intent))
							{
								result = intent;
							}
						}
					}
					if(result == ImpliedDesignIntentEnum.None)
					{
						nodeType = node.NodeType.ToLower();
						switch(nodeType)
						{
							case "image":
								result = ImpliedDesignIntentEnum.Image;
								break;
							case "text":
								result = ImpliedDesignIntentEnum.Text;
								break;
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetLabel																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the label content of the caller's node.
		/// </summary>
		/// <param name="node">
		/// Reference to the node to inspect.
		/// </param>
		/// <returns>
		/// The content of the supplied node's label, if found. Otherwise, an
		/// empty string.
		/// </returns>
		public static string GetLabel(HtmlNodeItem node)
		{
			HtmlAttributeItem attribute = null;
			string result = "";

			if(node != null)
			{
				attribute = node.Attributes["inkscape:label"];
				if(attribute != null && attribute.Value?.Length > 0)
				{
					result = attribute.Value;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetOrganizer																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the first organizer found at the current level of the provided
		/// control area collection.
		/// </summary>
		/// <param name="areas">
		/// Reference to the collection of areas to enumerate.
		/// </param>
		/// <returns>
		/// Reference to the first control area serving as a
		/// space-occupying area organizer, if found. Otherwise, null.
		/// </returns>
		public static ControlAreaItem GetOrganizer(ControlAreaCollection areas)
		{
			ControlAreaItem result = null;

			if(areas?.Count > 0)
			{
				result = areas.FirstOrDefault(x => IsOrganizerControl(x.Node));
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetOrientation																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the rectilinear orientation of the relationship between two
		/// areas.
		/// </summary>
		/// <param name="area1">
		/// Reference to the first control area for which the direction will be
		/// found.
		/// </param>
		/// <param name="area2">
		/// Reference to the second control area for which the direction will be
		/// found.
		/// </param>
		/// <param name="defaultOrientation">
		/// Default orientation if no specific answer is found.
		/// </param>
		/// <returns>
		/// The orientation between the two objects, if found. Otherwise, None.
		/// </returns>
		public static RectilinearOrientationEnum GetOrientation(
			ControlAreaItem area1, ControlAreaItem area2,
			RectilinearOrientationEnum defaultOrientation =
				RectilinearOrientationEnum.None)
		{
			float angle = 0f;
			FVector2 center1 = null;
			FVector2 center2 = null;
			RectilinearOrientationEnum result = defaultOrientation;

			if(area1 != null && area2 != null)
			{
				center1 = new FVector2(
					area1.X + (area1.Width / 2f),
					area1.Y + (area1.Height / 2f));
				center2 = new FVector2(
					area2.X + (area2.Width / 2f),
					area2.Y + (area2.Height / 2f));
				angle = Trig.RadToDeg(Trig.GetLineAngle(center1, center2));
				if(angle <= 45f ||
					(angle >= 135 && angle <= 225) ||
					angle >= 315)
				{
					result = RectilinearOrientationEnum.Horizontal;
				}
				else
				{
					result = RectilinearOrientationEnum.Vertical;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetReference																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the interpreted reference of the caller's node.
		/// </summary>
		/// <param name="node">
		/// Reference to the HTML node to inspect.
		/// </param>
		/// <returns>
		/// The implied form design intent reference of the provided node, if it
		/// could be found. Otherwise, none.
		/// </returns>
		public static string GetReference(HtmlNodeItem node)
		{
			HtmlAttributeItem attribute = null;
			Match match = null;
			string result = "";
			string text = "";

			if(node != null)
			{
				if(!IsLayer(node))
				{
					//	If the node has a label, then that intent overrides the intent
					//	attribute.
					text = GetLabel(node);
					if(text.Length == 0)
					{
						attribute = node.Attributes["intent"];
						if(attribute != null && attribute.Value?.Length > 0)
						{
							text = attribute.Value;
						}
					}
					if(text.Length > 0)
					{
						match = Regex.Match(text, ResourceMain.rxIntentWithLabel);
						if(match.Success)
						{
							result = GetValue(match, "reference");
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetRow																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a reference to the row found at the specified location.
		/// </summary>
		/// <param name="areas">
		/// Reference to the collection of reference areas to inspect.
		/// </param>
		/// <param name="y">
		/// Location whose row will be identified.
		/// </param>
		/// <returns>
		/// Reference to the row at the specified position, if found.
		/// Otherwise, null.
		/// </returns>
		public static ControlReferenceItem GetRow(
			ControlReferenceCollection areas, float y)
		{
			ControlReferenceItem result = null;

			foreach(ControlReferenceItem areaItem in areas)
			{
				if(areaItem.Top <= y && areaItem.Bottom >= y)
				{
					result = areaItem;
					break;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetRows																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a collection of implied rows found at the current control
		/// layer.
		/// </summary>
		/// <param name="areas">
		/// Reference to the collection of areas to inspect.
		/// </param>
		/// <returns>
		/// Reference to a collection of rows found at the current control
		/// layer, if any controls were present. Otherwise, an empty collection.
		/// </returns>
		public static ControlReferenceCollection GetRows(
			ControlAreaCollection areas)
		{
			FArea intersection = null;
			ControlReferenceItem reference = null;
			List<ControlReferenceItem> result = new List<ControlReferenceItem>();

			foreach(ControlAreaItem areaItem in areas)
			{
				if(areaItem.Intent != ImpliedDesignIntentEnum.Definitions)
				{
					intersection = null;
					foreach(ControlReferenceItem rowItem in result)
					{
						intersection = GetIntersectingArea(areaItem, rowItem);
						if(intersection != null)
						{
							rowItem.Top = Math.Min(rowItem.Top, areaItem.Top);
							rowItem.Bottom = Math.Max(rowItem.Bottom, areaItem.Bottom);
							rowItem.References.Add(areaItem);
							break;
						}
					}
					if(intersection == null)
					{
						reference = new ControlReferenceItem()
						{
							X = float.MinValue / 2f,
							Y = areaItem.Y,
							Width = float.MaxValue,
							Height = areaItem.Y
						};
						reference.References.Add(areaItem);
						result.Add(reference);
					}
				}
				else
				{
					result.AddRange(GetRows(areaItem.FrontAreas));
				}
			}
			return new ControlReferenceCollection(result.OrderBy(y => y.Y).ToList());
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetSegmentAreas																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a list of distinct segment areas, which are overlapping areas
		/// shared by multiple objects or distinct areas occupied by individual
		/// items.
		/// </summary>
		/// <param name="areas">
		/// Reference to the collection of areas to be analyzed.
		/// </param>
		/// <returns>
		/// Reference to a list of distinct, non-intersecting areas found within
		/// the caller's area and descendents.
		/// </returns>
		public static List<ControlAreaItem> GetSegmentAreas(
			ControlAreaCollection areas)
		{
			ControlAreaItem area = null;
			ControlAreaItem areaCopy = null;
			ControlAreaItem definition = null;
			List<ControlAreaItem> definitions = null;
			FArea farea = null;
			List<FArea> fareas = new List<FArea>();
			List<ControlAreaItem> flatList = null;
			List<ControlAreaItem> result = new List<ControlAreaItem>();

			if(areas?.Count > 0)
			{
				definitions = GetDefinitionAreas(areas);
				flatList = GetFlatList(areas);
				flatList.RemoveAll(x =>
					x.Intent == ImpliedDesignIntentEnum.Definitions);
				foreach(ControlAreaItem areaItem in flatList)
				{
					//	Group all overlapping areas.
					area = result.FirstOrDefault(x =>
						FArea.HasIntersection(areaItem, x));
					if(area == null)
					{
						//	This area didn't overlap any others.
						area = new ControlAreaItem(areaItem);
						result.Add(area);
					}
					areaCopy = new ControlAreaItem(areaItem);
					area.FrontAreas.Add(areaCopy);
					//	Transfer all associated user properties to the
					//	first level areas of each definition.
					definition = definitions.FirstOrDefault(x =>
						x.FrontAreas.Contains(areaItem));
					if(definition?.Properties.Count > 0)
					{
						NameValueCollection.TransferValues(
							definition.Properties, areaCopy.Properties);
					}
				}
				//	Set the boundaries for the area from the constituents.
				foreach(ControlAreaItem areaItem in result)
				{
					fareas.Clear();
					fareas.AddRange(areaItem.FrontAreas);
					farea = FArea.BoundingBox(fareas);
					areaItem.X = farea.X;
					areaItem.Y = farea.Y;
					areaItem.Width = farea.Width;
					areaItem.Height = farea.Height;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetText																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return all of the text found in the provided control area and its
		/// descendants.
		/// </summary>
		/// <param name="area">
		/// Reference to the area to inspect.
		/// </param>
		/// <returns>
		/// The text found in the provided area.
		/// </returns>
		public static string GetText(ControlAreaItem area)
		{
			string result = "";

			result = ToStringFromHtml(ControlAreaItem.GetInnerText(area));
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetTextArea																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the first text object found at the provided area or one of its
		/// descendants.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area at which to begin searching.
		/// </param>
		/// <returns>
		/// Reference to the first located text type control area, if found.
		/// Otherwise, null.
		/// </returns>
		public static ControlAreaItem GetTextArea(ControlAreaItem area)
		{
			ControlAreaItem result = null;

			if(area != null)
			{
				if(area.Intent == ImpliedDesignIntentEnum.Text ||
					area.Intent == ImpliedDesignIntentEnum.Label)
				{
					result = area;
				}
				else
				{
					foreach(ControlAreaItem areaItem in area.FrontAreas)
					{
						result = GetTextArea(areaItem);
						if(result != null)
						{
							break;
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetTextDefinitions																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the text-type nodes found in the definitions collection and its
		/// descendants.
		/// </summary>
		/// <param name="areas">
		/// Reference to a collection of areas to search for text definition nodes.
		/// </param>
		/// <returns>
		/// Reference to a list of control areas found in the local and descendant
		/// collections, if found. Otherwise, an empty list.
		/// </returns>
		public static List<ControlAreaItem> GetTextDefinitions(
			ControlAreaCollection areas)
		{
			ControlAreaCollection definitionAreas = null;
			List<ControlAreaItem> result = null;

			if(areas?.Count > 0)
			{
				definitionAreas = GetDefinitionAreas(areas);
				if(definitionAreas.Count > 0)
				{
					result = definitionAreas.FindMatches(x =>
						x.Node != null &&
						(x.Intent == ImpliedDesignIntentEnum.Label ||
						x.Intent == ImpliedDesignIntentEnum.Text));
				}
			}
			else
			{
				result = new List<ControlAreaItem>();
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////* GetTextLocal																													*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Return all of the text found directly in the provided control area.
		///// </summary>
		///// <param name="area">
		///// Reference to the area to inspect.
		///// </param>
		///// <returns>
		///// The text found in the provided area.
		///// </returns>
		//public static string GetTextLocal(ControlAreaItem area)
		//{
		//	StringBuilder builder = new StringBuilder();

		//	if(area != null && area.Node != null)
		//	{
		//		FillText(area, builder, false);
		//	}
		//	return builder.ToString();
		//}
		////*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetUserControlStyles																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return an array of control style definitions for a user control.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the source node to inspect.
		/// </param>
		/// <returns>
		/// An array of style definitions, if found. Otherwise, an empty array.
		/// </returns>
		public static string[] GetUserControlStyles(ControlAreaItem area)
		{
			Match match = null;
			string[] result = new string[0];
			List<string> values = new List<string>();

			if(area?.Node != null)
			{
				foreach(HtmlAttributeItem attributeItem in area.Node.Attributes)
				{
					match = Regex.Match(attributeItem.Name,
						ResourceMain.rxControlStyleName);
					if(match.Success && match.Length == attributeItem.Name.Length)
					{
						values.Add(attributeItem.Value);
					}
				}
				if(values.Count > 0)
				{
					result = values.ToArray();
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetUserValue																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the specified user property.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area containing the source node to inspect.
		/// </param>
		/// <param name="propertyName">
		/// Name of the property to retrieve.
		/// </param>
		/// <returns>
		/// Value of the specified user property, if found. Otherwise, a blank
		/// string.
		/// </returns>
		public static string GetUserValue(ControlAreaItem area,
			string propertyName)
		{
			HtmlAttributeItem attribute = null;
			string result = "";

			if(area?.Node != null && propertyName?.Length > 0)
			{
				attribute = area.Node.Attributes[propertyName];
				if(attribute != null)
				{
					result = attribute.Value;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetWidth																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the width of the specified node.
		/// </summary>
		/// <param name="node">
		/// Reference to the node to check.
		/// </param>
		/// <returns>
		/// The width of the specified node.
		/// </returns>
		public static float GetWidth(HtmlNodeItem node)
		{
			BoundingObjectItem bounds = null;
			float result = 0f;

			if(node != null)
			{
				bounds = CalcBounds(node);
				if(bounds != null)
				{
					result = bounds.GetWidth();
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetX																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the X coordinate of the specified node.
		/// </summary>
		/// <param name="node">
		/// Reference to the node to check.
		/// </param>
		/// <returns>
		/// The X coordinate of the specified node.
		/// </returns>
		public static float GetX(HtmlNodeItem node)
		{
			HtmlAttributeItem attribute = null;
			BoundingObjectItem bounds = null;
			string nodeType = "";
			float result = 0f;
			HtmlNodeItem span = null;
			List<HtmlNodeItem> spans = null;
			string textAnchor = "start";
			float width = 0f;
			float x = 0f;

			if(node != null)
			{
				nodeType = node.NodeType.ToLower();
				switch(nodeType)
				{
					case "g":
						bounds = CalcBounds(node);
						if(bounds != null)
						{
							result = bounds.MinX;
						}
						break;
					case "text":
						spans = node.Nodes.FindAll(x => x.NodeType.ToLower() == "tspan" &&
							x.Attributes.Exists(y => y.Name.ToLower() == "x"));
						if(spans.Count > 0)
						{
							span = spans[0];
							attribute = span.Attributes["x"];
							if(attribute != null)
							{
								//	Raw.
								x = (float)ToInt(attribute.Value);
								textAnchor =
									GetActiveStyle(span, "text-anchor", textAnchor);
								width = GetWidth(span);
								switch(textAnchor)
								{
									case "end":
										result = x - width;
										break;
									case "middle":
										result = x - (width / 2f);
										break;
									case "start":
									default:
										result = x;
										break;
								}
							}
						}
						break;
					case "tspan":
						span = node;
						attribute = span.Attributes["x"];
						if(attribute != null)
						{
							//	Raw.
							x = (float)ToInt(attribute.Value);
							textAnchor =
								GetActiveStyle(span, "text-anchor", textAnchor);
							width = GetWidth(span);
							switch(textAnchor)
							{
								case "end":
									result = x - width;
									break;
								case "middle":
									result = x - (width / 2f);
									break;
								case "start":
								default:
									result = x;
									break;
							}
						}
						break;
					default:
						attribute = node.Attributes["x"];
						if(attribute != null)
						{
							result = (float)ToInt(attribute.Value);
						}
						break;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetY																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the Y coordinate of the specified node.
		/// </summary>
		/// <param name="node">
		/// Reference to the node to check.
		/// </param>
		/// <returns>
		/// The Y coordinate of the specified node.
		/// </returns>
		public static float GetY(HtmlNodeItem node)
		{
			HtmlAttributeItem attribute = null;
			BoundingObjectItem bounds = null;
			string dominantBaseline = "alphabetic";
			float height = 0f;
			string nodeType = "";
			float result = 0f;
			HtmlNodeItem span = null;
			List<HtmlNodeItem> spans = null;
			float y = 0f;

			if(node != null)
			{
				nodeType = node.NodeType.ToLower();
				switch(nodeType)
				{
					case "g":
						bounds = CalcBounds(node);
						if(bounds != null)
						{
							result = bounds.MinY;
						}
						break;
					case "text":
						spans = node.Nodes.FindAll(x => x.NodeType.ToLower() == "tspan" &&
							x.Attributes.Exists(y => y.Name.ToLower() == "y"));
						if(spans.Count > 0)
						{
							span = spans[0];
							attribute = span.Attributes["y"];
							if(attribute != null)
							{
								//	Raw.
								y = (float)ToInt(attribute.Value);
								dominantBaseline =
									GetActiveStyle(span, "dominant-baseline", dominantBaseline);
								height = GetHeight(span);
								switch(dominantBaseline)
								{
									case "central":
									case "middle":
										result = y - (height / 2f);
										break;
									case "hanging":
									case "text-before-edge":
										result = y;
										break;
									case "alphabetic":
									default:
										result = y - height;
										break;
								}
							}
						}
						break;
					case "tspan":
						span = node;
						attribute = span.Attributes["y"];
						if(attribute != null)
						{
							//	Raw.
							y = (float)ToInt(attribute.Value);
							dominantBaseline =
								GetActiveStyle(span, "dominant-baseline", dominantBaseline);
							height = GetHeight(span);
							switch(dominantBaseline)
							{
								case "central":
								case "middle":
									result = y - (height / 2f);
									break;
								case "hanging":
								case "text-before-edge":
									result = y;
									break;
								case "alphabetic":
								default:
									result = y - height;
									break;
							}
						}
						break;
					default:
						attribute = node.Attributes["y"];
						if(attribute != null)
						{
							result = (float)ToInt(attribute.Value);
						}
						break;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* HasImages																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified collection or its
		/// decendants have images.
		/// </summary>
		/// <param name="areas">
		/// Reference to the control area collection to inspect.
		/// </param>
		/// <returns>
		/// True if one or more images are found in within the supplied tree.
		/// Otherwise, false.
		/// </returns>
		public static bool HasImages(ControlAreaCollection areas)
		{
			bool result = false;

			if(areas?.Count > 0)
			{
				result =
					areas.FindMatches(x =>
						x.Intent == ImpliedDesignIntentEnum.Image ||
						x.Intent == ImpliedDesignIntentEnum.PictureBox).Count > 0;
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return a value indicating whether the specified item or its decendants
		/// have images.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area to inspect.
		/// </param>
		/// <returns>
		/// True if one or more images are found in within the supplied tree.
		/// Otherwise, false.
		/// </returns>
		public static bool HasImages(ControlAreaItem area)
		{
			bool result = false;

			if(area != null)
			{
				result =
					(area.Intent == ImpliedDesignIntentEnum.Image ||
					area.Intent == ImpliedDesignIntentEnum.PictureBox);
				if(!result && area.FrontAreas.Count > 0)
				{
					result = HasImages(area.FrontAreas);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* HasIntent																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the caller's node has an intent.
		/// </summary>
		/// <param name="node">
		/// Reference to the node to be tested.
		/// </param>
		/// <returns>
		/// True if the node has an intent. Otherwise, false.
		/// </returns>
		public static bool HasIntent(HtmlNodeItem node)
		{
			bool result = false;

			if(node != null)
			{
				if((node.Attributes.Exists(x => x.Name.ToLower() == "intent" &&
					mControlTypes.Contains(LeftOf(x.Value.ToLower(), "-")))) ||
					(!IsLayer(node) &&
					node.Attributes.Exists(x => x.Name.ToLower() == "inkscape:label" &&
					mControlTypes.Contains(LeftOf(x.Value.ToLower(), "-")))))
				{
					result = true;
				}
				//if((node.Attributes.Exists(x => x.Name.ToLower() == "intent" &&
				//	x.Value?.Length > 0)) ||
				//	(!IsLayer(node) &&
				//	node.Attributes.Exists(x => x.Name.ToLower() == "inkscape:label" &&
				//	x.Value?.Length > 0)))
				//{
				//	result = true;
				//}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* HasOrganizer																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether there is a full-area organizer at the
		/// current level of the area collection.
		/// </summary>
		/// <param name="areas">
		/// Reference to the collection of control areas to review.
		/// </param>
		/// <returns>
		/// True if the current level of the provided control area collection
		/// contains an unchallenged area organizer. Otherwise, false.
		/// </returns>
		public static bool HasOrganizer(ControlAreaCollection areas)
		{
			ControlAreaItem discrete = null;
			ControlAreaItem organizer = null;
			bool result = false;

			if(areas?.Count > 0)
			{
				organizer = areas.FirstOrDefault(x => IsOrganizerControl(x.Node));
				if(organizer != null)
				{
					discrete = areas.FirstOrDefault(x => IsDiscreteControl(x.Node));
				}
				result = (organizer != null && discrete == null);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* HasText																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified collection or its
		/// decendants have text.
		/// </summary>
		/// <param name="areas">
		/// Reference to the control area collection to inspect.
		/// </param>
		/// <returns>
		/// True if one or more text instances are found in within the supplied
		/// tree. Otherwise, false.
		/// </returns>
		public static bool HasText(ControlAreaCollection areas)
		{
			bool result = false;

			if(areas?.Count > 0)
			{
				result =
					areas.FindMatches(x =>
						x.Intent == ImpliedDesignIntentEnum.Text ||
						x.Intent == ImpliedDesignIntentEnum.Label).Count > 0;
			}
			return result;
		}
		/// <summary>
		/// Return a value indicating whether the specified area or its
		/// decendants have text.
		/// </summary>
		/// <param name="area">
		/// Reference to the control area to inspect.
		/// </param>
		/// <returns>
		/// True if one or more text instances are found in within the supplied
		/// tree. Otherwise, false.
		/// </returns>
		public static bool HasText(ControlAreaItem area)
		{
			bool result = false;

			if(area != null)
			{
				result =
					(area.Intent == ImpliedDesignIntentEnum.Text ||
					area.Intent == ImpliedDesignIntentEnum.Label);
				if(!result && area.FrontAreas.Count > 0)
				{
					result = HasText(area.FrontAreas);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsControl																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the provided node is a control
		/// definition.
		/// </summary>
		/// <param name="node">
		/// Reference to the node to test.
		/// </param>
		/// <returns>
		/// True if the provided node represents a control definition. Otherwise,
		/// false.
		/// </returns>
		public static bool IsControl(HtmlNodeItem node)
		{
			bool result = false;

			if(node != null && (
				node.NodeType.ToLower() == "image" ||
				node.NodeType.ToLower() == "text" ||
				(!IsLayer(node) && HasIntent(node))))
			{
				result = true;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsDiscreteControl																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the provided node represents a
		/// discrete control.
		/// </summary>
		/// <param name="node">
		/// Reference to the node to inspect.
		/// </param>
		/// <returns>
		/// True if the provided node represents a discrete control. Otherwise,
		/// false.
		/// </returns>
		public static bool IsDiscreteControl(HtmlNodeItem node)
		{
			ImpliedDesignIntentEnum intent = ImpliedDesignIntentEnum.None;
			bool result = false;

			if(node != null)
			{
				intent = GetIntent(node);
				result = mDiscreteControlTypes.Contains(intent.ToString().ToLower());
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsFormLayer																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified node represents the
		/// main form layer.
		/// </summary>
		/// <param name="node">
		/// Reference to the node to test.
		/// </param>
		/// <returns>
		/// True if the provided node is the main form layer. Otherwise, false.
		/// </returns>
		public static bool IsFormLayer(HtmlNodeItem node)
		{
			bool result = false;

			if(node != null &&
				node.NodeType.ToLower() == "g" &&
				node.Attributes.Exists(x =>
					x.Name.ToLower() == "inkscape:groupmode" &&
					x.Value.ToLower() == "layer") &&
				node.Attributes.Exists(x =>
						x.Name.ToLower() == "inkscape:label" &&
						x.Value.ToLower().StartsWith("form")))
			{
				result = true;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsLayer																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified node represents a
		/// drawing layer.
		/// </summary>
		/// <param name="node">
		/// Reference to the node to test.
		/// </param>
		/// <returns>
		/// True if the provided node is a drawing layer. Otherwise, false.
		/// </returns>
		public static bool IsLayer(HtmlNodeItem node)
		{
			bool result = false;

			if(node != null &&
				node.NodeType.ToLower() == "g" &&
				node.Attributes.Exists(x =>
					x.Name.ToLower() == "inkscape:groupmode" &&
					x.Value.ToLower() == "layer"))
			{
				result = true;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsOrganizerControl																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified node is an area
		/// organizer.
		/// </summary>
		/// <param name="node">
		/// Reference to the node to inspect.
		/// </param>
		/// <returns>
		/// True if the node represents a space-occupying organizer. Otherwise,
		/// false.
		/// </returns>
		public static bool IsOrganizerControl(HtmlNodeItem node)
		{
			ImpliedDesignIntentEnum intent = ImpliedDesignIntentEnum.None;
			bool result = false;

			if(node != null)
			{
				intent = GetIntent(node);
				result = mOrganizerControlTypes.Contains(intent.ToString().ToLower());
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ProjectName																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ProjectName">ProjectName</see>.
		/// </summary>
		protected string mProjectName = "ImpliedFormDesignProject";
		/// <summary>
		/// Get/Set the name of the project of which this form is a member.
		/// </summary>
		public string ProjectName
		{
			get { return mProjectName; }
			set { mProjectName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	RootFontSize																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="RootFontSize">RootFontSize</see>.
		/// </summary>
		protected float mRootFontSize = 16f;
		/// <summary>
		/// Get/Set the root font size, in pixels.
		/// </summary>
		/// <remarks>
		/// The default for this property is 16px (12pt).
		/// </remarks>
		public float RootFontSize
		{
			get { return mRootFontSize; }
			set { mRootFontSize = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetControlStyles																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the user-defined control styles on the target node.
		/// </summary>
		/// <param name="styleCollectionName">
		/// Name of the style collection to create if there are styles.
		/// </param>
		/// <param name="area">
		/// The area whose source node will be inspected for custom control styles.
		/// </param>
		/// <param name="node">
		/// Reference to the target node to which the styles will be output.
		/// </param>
		public static void SetControlStyles(string styleCollectionName,
			ControlAreaItem area, HtmlNodeItem node)
		{
			string[] controlStyleValues = null;
			HtmlNodeItem result = null;

			if(styleCollectionName?.Length > 0 && area?.Node != null && node != null)
			{
				controlStyleValues = GetUserControlStyles(area);
				if(controlStyleValues.Length > 0)
				{
					result = new HtmlNodeItem()
					{
						NodeType = styleCollectionName,
						SelfClosing = false
					};
					foreach(string styleItem in controlStyleValues)
					{
						AddUserStyle(result.Nodes, styleItem);
					}
					node.Nodes.Add(result);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Svg																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Svg">Svg</see>.
		/// </summary>
		private SvgDocumentItem mSvg = null;
		/// <summary>
		/// Get/Set a reference to the active SVG document for this instance.
		/// </summary>
		public SvgDocumentItem Svg
		{
			get { return mSvg; }
			set { mSvg = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TransferAttribute																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Transfer the attribute value from the source to the target object.
		/// </summary>
		/// <param name="source">
		/// Reference to the source node.
		/// </param>
		/// <param name="sourceAttributeName">
		/// Name of the source attribute to read.
		/// </param>
		/// <param name="target">
		/// Reference to the target node.
		/// </param>
		/// <param name="targetAttributeName">
		/// Name of the target attribute to write.
		/// </param>
		/// <param name="defaultValue">
		/// Optional default value to set if the source is not found.
		/// </param>
		public void TransferAttribute(
			HtmlNodeItem source, string sourceAttributeName,
			HtmlNodeItem target, string targetAttributeName,
			string defaultValue = null)
		{
			HtmlAttributeItem attribute = null;

			if(source != null && target != null &&
				sourceAttributeName?.Length > 0 && targetAttributeName?.Length > 0)
			{
				attribute = source.Attributes[sourceAttributeName];
				if(attribute != null)
				{
					if(attribute.Value?.Length > 0)
					{
						target.Attributes.SetAttribute(targetAttributeName,
							attribute.Value);
					}
					else
					{
						target.Attributes.SetAttribute(targetAttributeName, "");
					}
				}
				else if(defaultValue != null)
				{
					target.Attributes.SetAttribute(targetAttributeName, defaultValue);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TransferUnitAttribute																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Transfer the CSS unit attribute value from the source area's
		/// attributes to the target area.
		/// </summary>
		/// <param name="source">
		/// Reference to the source node.
		/// </param>
		/// <param name="sourceAttributeName">
		/// Name of the source attribute to read.
		/// </param>
		/// <param name="target">
		/// Reference to the target node.
		/// </param>
		/// <param name="targetAttributeName">
		/// Name of the target attribute to write.
		/// </param>
		public void TransferUnitAttribute(
			HtmlNodeItem source, string sourceAttributeName,
			HtmlNodeItem target, string targetAttributeName)
		{
			HtmlAttributeItem attribute = null;
			Match match = null;
			string measure = "";
			string number = "";
			string targetValue = "0";

			if(source != null && target != null &&
				sourceAttributeName?.Length > 0 && targetAttributeName?.Length > 0)
			{
				attribute = source.Attributes[sourceAttributeName];
				if(attribute != null && attribute.Value?.Length > 0)
				{
					match = Regex.Match(attribute.Value,
						ResourceMain.rxCssNumberWithMeasure);
					if(match.Success)
					{
						number = GetValue(match, "number");
						measure = GetValue(match, "measure");
						if(number.Length > 0)
						{
							if(measure.Length > 0 && measure.ToLower() != "px")
							{
								targetValue =
									mConverter.Convert("Distance", ToDouble(number),
										measure, "px").ToString("0.###");
							}
							else
							{
								//	Value is already in pixels.
								targetValue = ToFloat(number).ToString("0.###");
							}
						}
						target.Attributes.SetAttribute(targetAttributeName, targetValue);
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	UseBackgroundColor																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for
		/// <see cref="UseBackgroundColor">UseBackgroundColor</see>.
		/// </summary>
		protected bool mUseBackgroundColor = true;
		/// <summary>
		/// Get/Set a value indicating whether to use background colors on dropped
		/// objects by default on this form.
		/// </summary>
		public bool UseBackgroundColor
		{
			get { return mUseBackgroundColor; }
			set { mUseBackgroundColor = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	UseBorderColor																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="UseBorderColor">UseBorderColor</see>.
		/// </summary>
		protected bool mUseBorderColor = true;
		/// <summary>
		/// Get/Set a value indicating whether to use border colors of drawing
		/// objects by default on this form.
		/// </summary>
		public bool UseBorderColor
		{
			get { return mUseBorderColor; }
			set { mUseBorderColor = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	UseBorderWidth																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="UseBorderWidth">UseBorderWidth</see>.
		/// </summary>
		protected bool mUseBorderWidth = true;
		/// <summary>
		/// Get/Set a value indicating whether to use border widths of drawing
		/// objects by default on this form.
		/// </summary>
		public bool UseBorderWidth
		{
			get { return mUseBorderWidth; }
			set { mUseBorderWidth = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	UseCornerRadius																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="UseCornerRadius">UseCornerRadius</see>.
		/// </summary>
		protected bool mUseCornerRadius = true;
		/// <summary>
		/// Get/Set a value indicating whether to use border corner radii of
		/// drawing objects by default on this form.
		/// </summary>
		public bool UseCornerRadius
		{
			get { return mUseCornerRadius; }
			set { mUseCornerRadius = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*


}
