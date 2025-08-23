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
