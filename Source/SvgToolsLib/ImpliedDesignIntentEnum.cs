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

namespace SvgToolsLibrary
{
	//*-------------------------------------------------------------------------*
	//*	ImpliedDesignIntentEnum																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of the possible intents for an implied design control
	/// definition.
	/// </summary>
	public enum ImpliedDesignIntentEnum
	{
		/// <summary>
		/// No intent specified or unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// A visual area on the form that can be clicked to initiate an action.
		/// </summary>
		Button,
		/// <summary>
		/// A control that toggles its state when clicked.
		/// </summary>
		CheckBox,
		/// <summary>
		/// A drop-down list that lets users either select an item from a
		/// predefined set or type in their own input.
		/// </summary>
		ComboBox,
		/// <summary>
		/// A layout container that arranges child elements sequentially,
		/// horizontally or vertically, wrapping them as needed.
		/// </summary>
		FlowPanel,
		/// <summary>
		/// The base form upon which all of the other controls will be stacked.
		/// </summary>
		Form,
		/// <summary>
		/// A virtual control that structures additional general information about
		/// the form upon which it is placed, like design implication styles,
		/// dialog behavior, etc.
		/// </summary>
		FormInformation,
		/// <summary>
		/// A layout container that generally occupies the parent's available space
		/// and arranges child elements into rows and columns.
		/// </summary>
		Grid,
		/// <summary>
		/// A table-like control that displays data in rows and columns, making it
		/// easy to view, sort, and edit structured information.
		/// </summary>
		GridView,
		/// <summary>
		/// A container that visually frames a set of related controls under a
		/// labeled border.
		/// </summary>
		GroupBox,
		/// <summary>
		/// A layout container, typically occupying the parent's available space,
		/// that arranges child elements in a horizontal sequence across defined
		/// columns.
		/// </summary>
		HorizontalGrid,
		/// <summary>
		/// A layout container that arranges child elements in a horizontal line
		/// and enables scrolling when the content exceeds the visible width.
		/// </summary>
		HorizontalScrollPanel,
		/// <summary>
		/// A layout container that arranges child elements in a single horizontal
		/// line.
		/// </summary>
		HorizontalStackPanel,
		/// <summary>
		/// A simple text element used to display static information or
		/// instructions to guide users within a form.
		/// </summary>
		Label,
		/// <summary>
		/// A simple control that displays a vertical list of selectable items.
		/// </summary>
		ListBox,
		/// <summary>
		/// A flexible control that displays a list of items with customizable
		/// layouts.
		/// </summary>
		ListView,
		/// <summary>
		/// A horizontal strip of drop-down menus that organizes application
		/// commands into categories like File, Edit, or Help, giving users
		/// quick access to key features.
		/// </summary>
		MenuBar,
		/// <summary>
		/// A flexible layout surface that allows designers to position child
		/// controls at exact coordinates and sizes.
		/// </summary>
		Panel,
		/// <summary>
		/// A simple visual control used to display images, like icons, photos, or
		/// graphics, within a form, making it easy to add visual context or
		/// decoration.
		/// </summary>
		PictureBox,
		/// <summary>
		/// A visual indicator that shows the completion status of a task or
		/// process.
		/// </summary>
		ProgressBar,
		/// <summary>
		/// A selection control that lets users choose one option from a group.
		/// </summary>
		RadioButton,
		/// <summary>
		/// A layout container that enables vertical and/or horizontal scrolling
		/// when its child content exceeds the visible area.
		/// </summary>
		ScrollPanel,
		/// <summary>
		/// A layout container that divides its space into resizable sections,
		/// either horizontally or vertically aligned, and typically with a
		/// draggable splitter, allowing users to adjust the relative size of
		/// content panes dynamically.
		/// </summary>
		SplitPanel,
		/// <summary>
		/// A flexible layout surface that allows designers to position child
		/// controls at exact coordinates and sizes.
		/// </summary>
		StaticPanel,
		/// <summary>
		/// A horizontal panel typically placed at the bottom of a form that
		/// displays brief messages, progress updates, or contextual information.
		/// </summary>
		StatusBar,
		/// <summary>
		/// A container that organizes content into multiple tabs.
		/// </summary>
		TabControl,
		/// <summary>
		/// An input field that lets users enter or edit text, essential for
		/// collecting information like names, passwords, or comments in a form.
		/// </summary>
		TextBox,
		/// <summary>
		/// A composite control that combines a TextBox with a button, typically
		/// labeled with an ellipsis ("..."), to let users enter text manually or
		/// launch a helper dialog for guided input.
		/// </summary>
		TextWithHelper,
		/// <summary>
		/// A horizontal or vertical strip of buttons and controls that gives
		/// users quick access to frequently used commands, tools, or actions
		/// within an application.
		/// </summary>
		ToolBar,
		/// <summary>
		/// A slider control that lets users select a value from a continuous or
		/// discrete range by dragging a thumb along a track.
		/// </summary>
		TrackBar,
		/// <summary>
		/// A hierarchical control that displays nested items in a collapsible
		/// tree structure.
		/// </summary>
		TreeView,
		/// <summary>
		/// A numeric input control that lets users adjust a value by clicking up
		/// or down arrows.
		/// </summary>
		UpDown,
		/// <summary>
		/// A layout container, typically occupying the parent's available space,
		/// that arranges child elements in a vertical sequence across defined
		/// rows.
		/// </summary>
		VerticalGrid,
		/// <summary>
		/// A layout container that stacks child elements vertically and enables
		/// scrolling when the content exceeds the visible height.
		/// </summary>
		VerticalScrollPanel,
		/// <summary>
		/// A layout container that arranges child elements in a single vertical
		/// line.
		/// </summary>
		VerticalStackPanel,
		/// <summary>
		/// A group of definitions for the current control.
		/// </summary>
		Definitions,
		/// <summary>
		/// A menu panel containing child entries for an individual menu item.
		/// </summary>
		MenuPanel,
		/// <summary>
		/// An image resource.
		/// </summary>
		Image,
		/// <summary>
		/// General text that can be interpreted by context.
		/// </summary>
		Text
	}
	//*-------------------------------------------------------------------------*


}
