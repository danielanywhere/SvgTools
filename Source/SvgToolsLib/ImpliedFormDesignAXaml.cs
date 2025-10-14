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

using Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
					targetNode.Attributes.SetAttribute("x:Name", attribute.Value);
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
			HtmlNodeItem result = null;
			int state = 0;
			string text = "";

			//	TODO: !1 - Stopped here...
			//	TODO: Implement AXAML version of RenderOutputNode.
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
								break;
							case 3:
								//	Text and images.
								break;
						}
						break;
					case ImpliedDesignIntentEnum.CheckBox:
						break;
					case ImpliedDesignIntentEnum.ComboBox:
						break;
					case ImpliedDesignIntentEnum.FlowPanel:
						break;
					case ImpliedDesignIntentEnum.Form:
						break;
					case ImpliedDesignIntentEnum.FormInformation:
						break;
					case ImpliedDesignIntentEnum.Grid:
						break;
					case ImpliedDesignIntentEnum.GridView:
						break;
					case ImpliedDesignIntentEnum.GroupBox:
						break;
					case ImpliedDesignIntentEnum.HorizontalGrid:
						break;
					case ImpliedDesignIntentEnum.HorizontalScrollPanel:
						break;
					case ImpliedDesignIntentEnum.HorizontalStackPanel:
						break;
					case ImpliedDesignIntentEnum.Image:
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
