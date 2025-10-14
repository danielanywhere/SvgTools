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

using Geometry;
using Html;

using static SvgToolsLibrary.SvgToolsUtil;

namespace SvgToolsLibrary
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
							Intent = GetIntent(nodeItem)
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
			"button", "checkbox", "combobox", "gridview", "label", "listbox",
			"listview", "menubar", "picturebox", "progressbar", "radiobutton",
			"statusbar", "tabcontrol", "textbox", "textwithhelper", "toolbar",
			"trackbar", "treeview", "updown", "forminformation", "flowpanel",
			"grid", "groupbox", "horizontalgrid", "horizontalscrollpanel",
			"horizontalstackpanel", "panel", "scrollpanel", "splitpanel",
			"staticpanel", "verticalgrid", "verticalscrollpanel",
			"verticalstackpanel",
			"menupanel"
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
			"flowpanel",
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
				outputNode.Attributes.SetAttribute("Width",
					GetFormWidth(mSvg.Document).ToString("0"));
				outputNode.Attributes.SetAttribute("Height",
					GetFormHeight(mSvg.Document).ToString("0"));
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
				//	Finalize the caption.
				if(!outputNode.Attributes.Exists(x =>
					x.Name.ToLower() == "title" && x.Value.Length > 0))
				{
					text = GetFormCaption(mSvg.Document);
					if(text.Length > 0)
					{
						outputNode.Attributes.SetAttribute("Title", text);
					}
				}
				//	Re-index to the base form.
				formArea =
					areas.FindMatch(x => x.Intent == ImpliedDesignIntentEnum.Form);
				if(formArea != null)
				{
					//	Determine the initial layout.
					childAreas = formArea.FrontAreas;
					if(HasOrganizer(childAreas))
					{
						area = GetOrganizer(childAreas);
						node = RenderOutputNode(area);
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
						node = RenderOutputNode(area);
					}
					//	This item is a form and has an organizer.
					PerformLayout(area, node);
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
		/// <returns>
		/// Reference to the output HTML node, in the active dialect, that
		/// properly represents the caller's supplied control area, if
		/// legitimate. Otherwise, null.
		/// </returns>
		protected virtual HtmlNodeItem RenderOutputNode(ControlAreaItem area)
		{
			return null;
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
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new instance of the ImpliedFormDesign item.
		/// </summary>
		public ImpliedFormDesign(SvgDocumentItem svgDocument)
		{
			if(svgDocument != null)
			{
				mSvg = svgDocument;
				mControlAreas = EnumerateControls(svgDocument);
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
			List<ControlAreaItem> flatAreas = null;
			int indent = 1;
			List<HtmlNodeItem> layers = null;
			HtmlNodeItem nodeForm = null;

			//	TODO: Sort the controls at each level as:
			//	left to right, then top to bottom.
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
								ProcessNodeZOrder(layerItem, areaItem.FrontAreas);
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
		//* FillText																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Fill the provided builder with the text found in the specified control
		/// area.
		/// </summary>
		/// <param name="area">
		/// Reference to the area to inspect.
		/// </param>
		/// <param name="builder">
		/// Reference to the text builder to fill.
		/// </param>
		public static void FillText(ControlAreaItem area, StringBuilder builder)
		{
			if(area != null && builder != null)
			{
				if((area.Intent == ImpliedDesignIntentEnum.Text ||
					area.Intent == ImpliedDesignIntentEnum.Label) &&
					area.Node.Text.Length > 0)
				{
					if(builder.Length > 0 &&
						!WhitespaceCharacters.Contains(builder[builder.Length - 1]) &&
						!WhitespaceCharacters.Contains(area.Node.Text[0]))
					{
						builder.Append(' ');
					}
					builder.Append(area.Node.Text);
				}
				foreach(ControlAreaItem areaItem in area.FrontAreas)
				{
					FillText(areaItem, builder);
				}
			}
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
				label = node.Attributes.FirstOrDefault(x =>
					x.Name.ToLower() == "inkscape:label").Value;
				if(label.ToLower().StartsWith("form-") &&
					label.Length > 5)
				{
					//	Caption is available in the label.
					caption = label.Substring(5);
				}
				else
				{
					attribute = node.Attributes.FirstOrDefault(x =>
						x.Name.ToLower() == "caption");
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
					attribute = node.Attributes.FirstOrDefault(x =>
						x.Name.ToLower() == "height");
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
					attribute = node.Attributes.FirstOrDefault(x =>
						x.Name.ToLower() == "width");
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
			string nodeType = "";
			ImpliedDesignIntentEnum result = ImpliedDesignIntentEnum.None;

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
					label = GetLabel(node);
					if(label.Length == 0)
					{
						attribute = node.Attributes.FirstOrDefault(x =>
							x.Name.ToLower() == "intent");
						if(attribute != null && attribute.Value?.Length > 0)
						{
							label = attribute.Value;
						}
					}
					if(label.Length > 0)
					{
						if(Enum.TryParse<ImpliedDesignIntentEnum>(
							LeftOf(label, "-"), true, out intent))
						{
							result = intent;
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
				attribute = node.Attributes.FirstOrDefault(x =>
					x.Name.ToLower() == "inkscape:label");
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
			StringBuilder builder = new StringBuilder();

			if(area != null && area.Node != null)
			{
				FillText(area, builder);
			}
			return builder.ToString();
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
							attribute =
								span.Attributes.FirstOrDefault(x => x.Name.ToLower() == "x");
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
						attribute =
							span.Attributes.FirstOrDefault(x => x.Name.ToLower() == "x");
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
						attribute =
							node.Attributes.FirstOrDefault(x => x.Name.ToLower() == "x");
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
							attribute =
								span.Attributes.FirstOrDefault(x => x.Name.ToLower() == "y");
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
						attribute =
							span.Attributes.FirstOrDefault(x => x.Name.ToLower() == "y");
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
						attribute =
							node.Attributes.FirstOrDefault(x => x.Name.ToLower() == "y");
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
