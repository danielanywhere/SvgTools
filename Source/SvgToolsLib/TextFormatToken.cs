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
using System.Text;

using static SvgToolsLib.SvgToolsUtil;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	TextFormatTokenCollection																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of TextFormatTokenItem Items.
	/// </summary>
	public class TextFormatTokenCollection : List<TextFormatTokenItem>
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


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	TextFormatTokenItem																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Token for formatting text Text.
	/// </summary>
	public class TextFormatTokenItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* PopFormat																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Pop settings from the sequence stack until the specified format item
		/// can be removed.
		/// </summary>
		/// <param name="format">
		/// The format to pop.
		/// </param>
		private void PopFormat(TextFormatTypeEnum format)
		{
			TextFormatTypeEnum peek = TextFormatTypeEnum.None;

			while(mSequence.Contains(format))
			{
				peek = mSequence[^1];
				switch(peek)
				{
					case TextFormatTypeEnum.Bold:
						this.mBold = false;
						this.mBoldChanged = true;
						break;
					case TextFormatTypeEnum.Color:
						this.mColor = "";
						this.mColorChanged = true;
						break;
					case TextFormatTypeEnum.FontName:
						this.mFontName = "";
						this.mFontNameChanged = true;
						break;
					case TextFormatTypeEnum.FontSize:
						this.mFontSize = 0;
						this.mFontSizeChanged = true;
						break;
					case TextFormatTypeEnum.Italic:
						this.mItalic = false;
						this.mItalicChanged = true;
						break;
					case TextFormatTypeEnum.Underline:
						this.mUnderline = false;
						this.mUnderlineChanged = true;
						break;
				}
				mSequence.RemoveAt(mSequence.Count - 1);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* PushFormat																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Push the specified format setting onto the stack, if necessary.
		/// </summary>
		/// <param name="format">
		/// The format to push.
		/// </param>
		private void PushFormat(TextFormatTypeEnum format)
		{
			if(!mSequence.Contains(format))
			{
				mSequence.Add(format);
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
		/// Create a new instance of the TextFormatTokenItem item.
		/// </summary>
		public TextFormatTokenItem()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new instance of the TextFormatTokenItem item.
		/// </summary>
		/// <param name="previousItem">
		/// Reference to a previous item that will be used to set this item's
		/// initial properties.
		/// </param>
		public TextFormatTokenItem(TextFormatTokenItem previousItem)
		{
			if(previousItem != null)
			{
				this.Bold = previousItem.Bold;
				this.Color = previousItem.Color;
				this.FontName = previousItem.FontName;
				this.FontSize = previousItem.FontSize;
				this.Italic = previousItem.Italic;
				this.Underline = previousItem.Underline;
				this.mSequence.Clear();
				this.mSequence.AddRange(previousItem.mSequence);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Bold																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Bold">Bold</see>.
		/// </summary>
		private bool mBold = false;
		/// <summary>
		/// Get/Set a value indicating whether the text style is bold.
		/// </summary>
		public bool Bold
		{
			get { return mBold; }
			set
			{
				mBoldChanged = (mBold != value);
				mBold = value;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	BoldChanged																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="BoldChanged">BoldChanged</see>.
		/// </summary>
		private bool mBoldChanged = false;
		/// <summary>
		/// Get/Set a value indicating whether the value of the Bold property has
		/// changed.
		/// </summary>
		public bool BoldChanged
		{
			get { return mBoldChanged; }
			set { mBoldChanged = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Changed																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Changed">Changed</see>.
		/// </summary>
		private bool mChanged = false;
		/// <summary>
		/// Get/Set a value indicating whether any of the formatting properties on
		/// this item have changed.
		/// </summary>
		public bool Changed
		{
			get
			{
				return mBoldChanged ||
					mChanged ||
					mColorChanged ||
					mFontNameChanged ||
					mFontSizeChanged ||
					mItalicChanged ||
					mUnderlineChanged;
			}
			set
			{
				if(!value)
				{
					mBoldChanged =
						mColorChanged =
						mFontNameChanged =
						mFontSizeChanged =
						mItalicChanged =
						mUnderlineChanged = false;
				}
				mChanged = value;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Color																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Color">Color</see>.
		/// </summary>
		private string mColor = "#000000";
		/// <summary>
		/// Get/Set the fill color for the text.
		/// </summary>
		public string Color
		{
			get { return mColor; }
			set
			{
				mColorChanged = (mColor != value);
				mColor = value;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ColorChanged																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ColorChanged">ColorChanged</see>.
		/// </summary>
		private bool mColorChanged = false;
		/// <summary>
		/// Get/Set a value indicating whether the value of the color property has
		/// changed.
		/// </summary>
		public bool ColorChanged
		{
			get { return mColorChanged; }
			set { mColorChanged = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FontName																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="FontName">FontName</see>.
		/// </summary>
		private string mFontName = "";
		/// <summary>
		/// Get/Set the name of the font.
		/// </summary>
		public string FontName
		{
			get { return mFontName; }
			set
			{
				mFontNameChanged = (mFontName != value);
				mFontName = value;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FontNameChanged																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="FontNameChanged">FontNameChanged</see>.
		/// </summary>
		private bool mFontNameChanged = false;
		/// <summary>
		/// Get/Set a value indicating whether the value of the FontName property
		/// has changed.
		/// </summary>
		public bool FontNameChanged
		{
			get { return mFontNameChanged; }
			set { mFontNameChanged = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FontSize																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="FontSize">FontSize</see>.
		/// </summary>
		private int mFontSize = 18;
		/// <summary>
		/// Get/Set the font size, in pixels.
		/// </summary>
		public int FontSize
		{
			get { return mFontSize; }
			set
			{
				mFontSizeChanged = (mFontSize != value);
				mFontSize = value;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FontSizeChanged																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="FontSizeChanged">FontSizeChanged</see>.
		/// </summary>
		private bool mFontSizeChanged = false;
		/// <summary>
		/// Get/Set a value indicating whether the value of the FontSize property
		/// has changed.
		/// </summary>
		public bool FontSizeChanged
		{
			get { return mFontSizeChanged; }
			set { mFontSizeChanged = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetActiveTextFormat																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the active text format token for the specified source node.
		/// </summary>
		/// <param name="node">
		/// Reference to the source node to inspect.
		/// </param>
		/// <returns>
		/// Reference to a newly created text format token representing the active
		/// styles.
		/// </returns>
		public static TextFormatTokenItem GetActiveTextFormat(HtmlNodeItem node)
		{
			TextFormatTokenItem result = new TextFormatTokenItem();
			string text = "";

			if(node != null)
			{
				text = GetActiveStyleCombined(node, "font-weight");
				if(text.Length > 0 && text.ToLower() == "bold")
				{
					result.mBold = true;
				}
				text = GetActiveStyleCombined(node, "fill");
				if(text.Length > 0)
				{
					result.mColor = text;
				}
				text = GetActiveStyleCombined(node, "font-family");
				if(text.Length > 0)
				{
					result.mFontName = text;
				}
				text = GetActiveStyleCombined(node, "font-size");
				if(text.Length > 0)
				{
					result.mFontSize = (int)SvgToolsUtil.EstimateFontHeight(text);
				}
				text = GetActiveStyleCombined(node, "font-style");
				if(text.Length > 0 && text.ToLower() == "italic")
				{
					result.mItalic = true;
				}
				text = GetActiveStyleCombined(node, "text-decoration");
				if(text.Length > 0 && text.ToLower() == "underline")
				{
					result.mUnderline = true;
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the active text format token for the specified source node.
		/// </summary>
		/// <param name="node">
		/// Reference to the source node to inspect.
		/// </param>
		/// <param name="comparison">
		/// Reference to a format with which to compare the new item, to set the
		/// changed fields on the new item.
		/// </param>
		/// <returns>
		/// Reference to a newly created text format token representing the active
		/// styles. Changed fields will be set for any properties whose values
		/// don't match those on the comparison item.
		/// </returns>
		public static TextFormatTokenItem GetActiveTextFormat(HtmlNodeItem node,
			TextFormatTokenItem comparison)
		{
			TextFormatTokenItem result = new TextFormatTokenItem();
			string text = "";

			if(node != null)
			{
				text = GetActiveStyleCombined(node, "font-weight");
				if(text.Length > 0 && text.ToLower() == "bold")
				{
					result.mBold = true;
				}
				result.mBoldChanged = (result.mBold != comparison?.mBold);
				text = GetActiveStyleCombined(node, "fill");
				if(text.Length > 0)
				{
					result.mColor = text;
				}
				result.mColorChanged = (result.mColor != comparison?.mColor);
				text = GetActiveStyleCombined(node, "font-family");
				if(text.Length > 0)
				{
					result.mFontName = text;
				}
				result.mFontNameChanged = (result.mFontName != comparison?.mFontName);
				text = GetActiveStyleCombined(node, "font-size");
				if(text.Length > 0)
				{
					result.mFontSize = (int)SvgToolsUtil.EstimateFontHeight(text);
				}
				result.mFontSizeChanged = (result.mFontSize != comparison?.mFontSize);
				text = GetActiveStyleCombined(node, "font-style");
				if(text.Length > 0 && text.ToLower() == "italic")
				{
					result.mItalic = true;
				}
				result.mItalicChanged = (result.mItalic != comparison?.mItalic);
				text = GetActiveStyleCombined(node, "text-decoration");
				if(text.Length > 0 && text.ToLower() == "underline")
				{
					result.mUnderline = true;
				}
				result.mUnderlineChanged =
					(result.mUnderline != comparison?.mUnderline);
				//	Maintain the same sequence for the previous items.
				foreach(TextFormatTypeEnum formatTypeItem in comparison.mSequence)
				{
					switch(formatTypeItem)
					{
						case TextFormatTypeEnum.Bold:
							if(result.mBold)
							{
								result.mSequence.Add(TextFormatTypeEnum.Bold);
							}
							break;
						case TextFormatTypeEnum.Color:
							if(result.mColor.Length > 0)
							{
								result.mSequence.Add(TextFormatTypeEnum.Color);
							}
							break;
						case TextFormatTypeEnum.FontName:
							if(result.mFontName.Length > 0)
							{
								result.mSequence.Add(TextFormatTypeEnum.FontName);
							}
							break;
						case TextFormatTypeEnum.FontSize:
							if(result.mFontSize > 0f)
							{
								result.mSequence.Add(TextFormatTypeEnum.FontSize);
							}
							break;
						case TextFormatTypeEnum.Italic:
							if(result.mItalic)
							{
								result.mSequence.Add(TextFormatTypeEnum.Italic);
							}
							break;
						case TextFormatTypeEnum.Underline:
							if(result.mUnderline)
							{
								result.mSequence.Add(TextFormatTypeEnum.Underline);
							}
							break;
					}
				}
				if(result.mBold && !result.mSequence.Contains(TextFormatTypeEnum.Bold))
				{
					result.mSequence.Add(TextFormatTypeEnum.Bold);
				}
				if(result.mColor.Length > 0 &&
					!result.mSequence.Contains(TextFormatTypeEnum.Color))
				{
					result.mSequence.Add(TextFormatTypeEnum.Color);
				}
				if(result.mFontName.Length > 0 &&
					!result.mSequence.Contains(TextFormatTypeEnum.FontName))
				{
					result.mSequence.Add(TextFormatTypeEnum.FontName);
				}
				if(result.mFontSize > 0f &&
					!result.mSequence.Contains(TextFormatTypeEnum.FontSize))
				{
					result.mSequence.Add(TextFormatTypeEnum.FontSize);
				}
				if(result.mItalic &&
					!result.mSequence.Contains(TextFormatTypeEnum.Italic))
				{
					result.mSequence.Add(TextFormatTypeEnum.Italic);
				}
				if(result.mUnderline &&
					!result.mSequence.Contains(TextFormatTypeEnum.Underline))
				{
					result.mSequence.Add(TextFormatTypeEnum.Underline);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Italic																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Italic">Italic</see>.
		/// </summary>
		private bool mItalic = false;
		/// <summary>
		/// Get/Set a value indicating whether the text is italic.
		/// </summary>
		public bool Italic
		{
			get { return mItalic; }
			set
			{
				mItalicChanged = (mItalic != value);
				mItalic = value;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ItalicChanged																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ItalicChanged">ItalicChanged</see>.
		/// </summary>
		private bool mItalicChanged = false;
		/// <summary>
		/// Get/Set a value indicating whether the value of the Italic property has
		/// changed.
		/// </summary>
		public bool ItalicChanged
		{
			get { return mItalicChanged; }
			set { mItalicChanged = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Sequence																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Sequence">Sequence</see>.
		/// </summary>
		private List<TextFormatTypeEnum> mSequence =
			new List<TextFormatTypeEnum>();
		/// <summary>
		/// Get a reference to the list of formats in the order they have been
		/// activated.
		/// </summary>
		public List<TextFormatTypeEnum> Sequence
		{
			get { return mSequence; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Underline																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Underline">Underline</see>.
		/// </summary>
		private bool mUnderline = false;
		/// <summary>
		/// Get/Set a value indicating whether the text is underlined.
		/// </summary>
		public bool Underline
		{
			get { return mUnderline; }
			set
			{
				mUnderlineChanged = (mUnderline != value);
				mUnderline = value;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	UnderlineChanged																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="UnderlineChanged">UnderlineChanged</see>.
		/// </summary>
		private bool mUnderlineChanged = false;
		/// <summary>
		/// Get/Set a value indicating whether the value of the Underline property
		/// has changed.
		/// </summary>
		public bool UnderlineChanged
		{
			get { return mUnderlineChanged; }
			set { mUnderlineChanged = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* UpdateChanged																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Compare the source and target tokens and update the changed state of
		/// the properties in the target item.
		/// </summary>
		/// <param name="source">
		/// Reference to the source item to compare.
		/// </param>
		/// <param name="target">
		/// Reference to the target item to compare and update.
		/// </param>
		public static void UpdateChanged(TextFormatTokenItem source,
			TextFormatTokenItem target)
		{
			if(source != null && target != null)
			{
				target.Changed = false;
				target.mBoldChanged = (source.mBold != target.mBold);
				target.mColorChanged = (source.mColor != target.mColor);
				target.mFontNameChanged = (source.mFontName != target.mFontName);
				target.mFontSizeChanged = (source.mFontSize != target.mFontSize);
				target.mItalicChanged = (source.mItalic != target.mItalic);
				target.mUnderlineChanged = (source.mUnderline != target.mUnderline);
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
