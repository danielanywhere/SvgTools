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
	//*	MessageSendEventArgs																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Event arguments for handling a MessageSent event.
	/// </summary>
	public class MessageSendEventArgs : EventArgs
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
		//*	Handled																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Handled">Handled</see>.
		/// </summary>
		private bool mHandled = false;
		/// <summary>
		/// Get/Set a value indicating whether this event has been handled.
		/// </summary>
		public bool Handled
		{
			get { return mHandled; }
			set { mHandled = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Importance																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Importance">Importance</see>.
		/// </summary>
		private MessageImportanceEnum mImportance = MessageImportanceEnum.None;
		/// <summary>
		/// Get/Set the importance of the message.
		/// </summary>
		public MessageImportanceEnum Importance
		{
			get { return mImportance; }
			set { mImportance = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	MessageText																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="MessageText">MessageText</see>.
		/// </summary>
		private string mMessageText = "";
		/// <summary>
		/// Get/Set the text of the message.
		/// </summary>
		public string MessageText
		{
			get { return mMessageText; }
			set { mMessageText = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
