using System;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using SvgToolsLibrary;
using static SvgToolsLibrary.SvgToolsUtil;

namespace SvgToolsApp
{
	//*-------------------------------------------------------------------------*
	//*	Program																																	*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Main console application instance for SvgTools.
	/// </summary>
	public class Program
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* AgnosticArgEqual																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Test to see if the argument equals the specified system-agnostic
		/// key value.
		/// </summary>
		/// <param name="argumentValue">
		/// The argument to review.
		/// </param>
		/// <param name="keyValue">
		/// The key value to test for a match.
		/// </param>
		/// <returns>
		/// True if the provided argument equals the specified system-agnostic
		/// key value.
		/// </returns>
		private static bool AgnosticArgEqual(string argumentValue, string keyValue)
		{
			bool result = false;

			if(argumentValue?.Length > 0 && keyValue?.Length > 0)
			{
				result = (argumentValue == $"/{keyValue}" ||
					argumentValue == $"--{keyValue}");
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AgnosticArgStart																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Test to see if the argument begins with the specified system-agnostic
		/// key value.
		/// </summary>
		/// <param name="argumentValue">
		/// The argument to review.
		/// </param>
		/// <param name="keyValue">
		/// The key value to test for a match.
		/// </param>
		/// <returns>
		/// True if the provided argument starts with the specified system-agnostic
		/// key value.
		/// </returns>
		private static bool AgnosticArgStart(string argumentValue, string keyValue)
		{
			bool result = false;

			if(argumentValue?.Length > 0 && keyValue?.Length > 0)
			{
				result = (argumentValue.StartsWith($"/{keyValue}") ||
					argumentValue.StartsWith($"--{keyValue}"));
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mActionItem_MessageSent																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// A message has been sent from the action system.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Message send event arguments.
		/// </param>
		private static void mActionItem_MessageSent(object sender,
			MessageSendEventArgs e)
		{
			if(!e.Handled)
			{
				switch(e.Importance)
				{
					case MessageImportanceEnum.Error:
						Console.Write("!> ");
						break;
					case MessageImportanceEnum.Information:
						Console.Write("i> ");
						break;
					case MessageImportanceEnum.None:
						Console.Write("?> ");
						break;
					case MessageImportanceEnum.Warning:
						Console.Write("W> ");
						break;
				}
				Console.WriteLine(e.MessageText);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TestPrecision																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Test the precision of a number formatted as {number},{precision}
		/// </summary>
		/// <param name="value">
		/// The comma-delimited combination of the number to test, and the
		/// decimal point precision to apply.
		/// </param>
		private static void TestPrecision(string value)
		{
			string[] elements = null;
			string number = "";
			int precision = 0;

			if(value?.Length > 0)
			{
				elements = value.Split(',',
					StringSplitOptions.RemoveEmptyEntries |
					StringSplitOptions.TrimEntries);
				if(elements.Length > 0)
				{
					number = elements[0];
				}
				if(elements.Length > 1)
				{
					precision = ToInt(elements[1]);
				}
				Console.Write($"Result of {number} limited to {precision} digits is ");
				Console.WriteLine(SvgToolsUtil.SetPrecision(number, precision));
			}
			else
			{
				Console.WriteLine("Please supply a value as {number},{precision}");
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
		//*	_Main																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Configure and run the application.
		/// </summary>
		public static async Task Main(string[] args)
		{
			SvgActionTypeEnum action = SvgActionTypeEnum.None;
			bool bActivity = false;
			bool bShowCommand = false;
			bool bShowHelp = false; //	Flag - Explicit Show Help.
			StringBuilder builder = new StringBuilder();
			string key = "";        //	Current Parameter Key.
			string lowerArg = "";   //	Current Lowercase Argument.
			StringBuilder message = new StringBuilder();
			NameValueCollection nameValues = null;
			Program prg = new Program();  //	Initialized instance.

			Console.WriteLine("SvgTools.exe");
			prg.mActionItem = new SvgActionItem();
			prg.mActionItem.MessageSent += mActionItem_MessageSent;

			foreach(string arg in args)
			{
				lowerArg = arg.ToLower();
				key = "/?";
				if(lowerArg == key)
				{
					bShowHelp = true;
					continue;
				}
				key = "action:";
				if(AgnosticArgStart(lowerArg, key))
				{
					if(Enum.TryParse<SvgActionTypeEnum>(arg.Substring(key.Length),
						true, out action))
					{
						if(action != SvgActionTypeEnum.None)
						{
							prg.ActionItem.Action = action;
							bActivity = true;
						}
						else
						{
							message.Append("Error: No action specified...");
							bShowHelp = true;
							break;
						}
					}
					continue;
				}
				key = "commandline";
				if(AgnosticArgEqual(lowerArg, key))
				{
					bShowCommand = true;
					continue;
				}
				key = "configfile:";
				if(AgnosticArgStart(lowerArg, key))
				{
					prg.ActionItem.ConfigFilename = arg.Substring(key.Length);
					continue;
				}
				key = "infile:";
				if(AgnosticArgStart(lowerArg, key))
				{
					prg.ActionItem.InputFilename = arg.Substring(key.Length);
					continue;
				}
				key = "option:";
				if(AgnosticArgStart(lowerArg, key))
				{
					prg.ActionItem.Options.Add(arg.Substring(key.Length));
					continue;
				}
				key = "outfile:";
				if(AgnosticArgStart(lowerArg, key))
				{
					prg.ActionItem.OutputFilename = arg.Substring(key.Length);
					continue;
				}
				key = "properties:";
				if(AgnosticArgStart(lowerArg, key))
				{
					try
					{
						nameValues = JsonConvert.DeserializeObject<NameValueCollection>(
							arg.Substring(key.Length));
						foreach(NameValueItem propertyItem in nameValues)
						{
							prg.mActionItem.Properties.Add(propertyItem);
						}
					}
					catch(Exception ex)
					{
						Console.WriteLine($"Error parsing properties: {ex.Message}");
						bShowHelp = true;
					}
					continue;
				}
				key = "testprecision:";
				if(AgnosticArgStart(lowerArg, key))
				{
					TestPrecision(arg.Substring(key.Length));
					bActivity = true;
					continue;
				}
				key = "text:";
				if(AgnosticArgStart(lowerArg, key))
				{
					prg.ActionItem.Text = arg.Substring(key.Length);
					continue;
				}
				key = "wait";
				if(AgnosticArgEqual(lowerArg, key))
				{
					prg.mWaitAfterEnd = true;
					continue;
				}
				key = "workingpath:";
				if(AgnosticArgStart(lowerArg, key))
				{
					prg.ActionItem.WorkingPath = arg.Substring(key.Length);
					continue;
				}
			}
			if(bShowCommand)
			{
				Clear(builder);
				builder.Append("Command:");
				foreach(string argItem in args)
				{
					builder.Append(' ');
					builder.Append(argItem);
				}
			}
			if(!bShowHelp && !bActivity)
			{
				message.AppendLine(
					"Please specify an action or a stand-alone activity.");
				bShowHelp = true;
			}
			if(bShowHelp)
			{
				//	Display Syntax.
				Console.WriteLine(message.ToString() + "\r\n" + ResourceMain.Syntax);
			}
			else
			{
				//	Run the configured application.
				await prg.Run();
			}
			if(prg.mWaitAfterEnd)
			{
				Console.WriteLine("Press [Enter] to exit...");
				Console.ReadLine();
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ActionItem																														*
		//*-----------------------------------------------------------------------*
		private SvgActionItem mActionItem = null;
		/// <summary>
		/// Get/Set the file action item associated with this session.
		/// </summary>
		public SvgActionItem ActionItem
		{
			get { return mActionItem; }
			set { mActionItem = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Run																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Run the configured application.
		/// </summary>
		public async Task Run()
		{
			if(mActionItem.Action != SvgActionTypeEnum.None)
			{
				await mActionItem.Run();
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	WaitAfterEnd																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="WaitAfterEnd">WaitAfterEnd</see>.
		/// </summary>
		private bool mWaitAfterEnd = false;
		/// <summary>
		/// Get/Set a value indicating whether to wait for user keypress after
		/// processing has completed.
		/// </summary>
		public bool WaitAfterEnd
		{
			get { return mWaitAfterEnd; }
			set { mWaitAfterEnd = value; }
		}

	}
	//*-------------------------------------------------------------------------*


}
