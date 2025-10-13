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
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* ControlTypes																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// List of control types used in implied form design.
		/// </summary>
		private static string[] mControlTypes = new string[]
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
		public static void EnumerateControls(SvgDocumentItem doc)
		{
			ControlAreaItem area = null;
			ControlAreaCollection areas = null;
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
								ProcessNodeZOrder(layerItem, areaItem.FrontAreas);
							}
						}
					}
					ControlAreaCollection.Dump(areas, indent);
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
						if(node.NodeType.ToLower() == "image")
						{
							result = ImpliedDesignIntentEnum.Image;
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

	}
	//*-------------------------------------------------------------------------*


}
