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
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using SvgToolsLib;

using static SvgToolsLib.SvgToolsUtil;

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
		/// A value greater than zero if the provided argument equals the specified
		/// system-agnostic key value. Otherwise, zero.
		/// </returns>
		private static int AgnosticArgEqual(string argumentValue, string keyValue)
		{
			int result = 0;

			if(argumentValue?.Length > 0 && keyValue?.Length > 0)
			{
				result = ((argumentValue == $"/{keyValue}" ||
					argumentValue == $"--{keyValue}") ? argumentValue.Length : 0);
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
		/// A value equal to the length of the keyvalue plus the prefix on the
		/// argument value, if the provided argument starts with the specified
		/// system-agnostic key value. Otherwise, zero.
		/// </returns>
		private static int AgnosticArgStart(string argumentValue, string keyValue)
		{
			string prefix = "";
			int result = 0;

			if(argumentValue?.Length > 0 && keyValue?.Length > 0)
			{
				if(argumentValue.StartsWith('/'))
				{
					prefix = "/";
				}
				else if(argumentValue.StartsWith("--"))
				{
					prefix = "--";
				}
				if(prefix.Length > 0)
				{
					result = ((argumentValue.StartsWith($"/{keyValue}") ||
						argumentValue.StartsWith($"--{keyValue}")) ?
						$"{prefix}{keyValue}".Length : 0);
				}
			}
			return result;
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
			int keyLength = 0;
			string lowerArg = "";   //	Current Lowercase Argument.
			StringBuilder message = new StringBuilder();
			NameValueCollection nameValues = null;
			Program prg = new Program();  //	Initialized instance.

			ConsoleTraceListener consoleListener = new ConsoleTraceListener();
			Trace.Listeners.Add(consoleListener);

			Console.WriteLine("SvgTools.exe");
			prg.mActionItem = new SvgActionItem()
			{
				StyleWorksheets = new System.Collections.Generic.List<string>()
			};

			foreach(string arg in args)
			{
				lowerArg = arg.ToLower();
				key = "?";
				keyLength = AgnosticArgEqual(lowerArg, key);
				if(keyLength > 0)
				{
					bShowHelp = true;
					continue;
				}
				key = "action:";
				keyLength = AgnosticArgStart(lowerArg, key);
				if(keyLength > 0)
				{
					if(Enum.TryParse<SvgActionTypeEnum>(arg.Substring(keyLength),
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
				keyLength = AgnosticArgEqual(lowerArg, key);
				if(keyLength > 0)
				{
					bShowCommand = true;
					continue;
				}
				key = "configfile:";
				keyLength = AgnosticArgStart(lowerArg, key);
				if(keyLength > 0)
				{
					prg.ActionItem.ConfigFilename = arg.Substring(keyLength);
					continue;
				}
				key = "infile:";
				keyLength = AgnosticArgStart(lowerArg, key);
				if(keyLength > 0)
				{
					prg.ActionItem.InputNames.Add(arg.Substring(keyLength));
					//prg.ActionItem.InputFilename = arg.Substring(keyLength);
					continue;
				}
				key = "option:";
				keyLength = AgnosticArgStart(lowerArg, key);
				if(keyLength > 0)
				{
					prg.ActionItem.Options.Add(arg.Substring(keyLength));
					continue;
				}
				key = "outfile:";
				keyLength = AgnosticArgStart(lowerArg, key);
				if(keyLength > 0)
				{
					prg.ActionItem.OutputFilename = arg.Substring(keyLength);
					continue;
				}
				key = "properties:";
				keyLength = AgnosticArgStart(lowerArg, key);
				if(keyLength > 0)
				{
					try
					{
						nameValues = JsonConvert.DeserializeObject<NameValueCollection>(
							arg.Substring(keyLength));
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
				key = "styleworksheet:";
				keyLength = AgnosticArgStart(lowerArg, key);
				if(keyLength > 0)
				{
					prg.ActionItem.StyleWorksheets.Add(arg.Substring(keyLength));
				}
				key = "testprecision:";
				keyLength = AgnosticArgStart(lowerArg, key);
				if(keyLength > 0)
				{
					TestPrecision(arg.Substring(keyLength));
					bActivity = true;
					continue;
				}
				key = "text:";
				keyLength = AgnosticArgStart(lowerArg, key);
				if(keyLength > 0)
				{
					prg.ActionItem.Text = arg.Substring(keyLength);
					continue;
				}
				key = "wait";
				keyLength = AgnosticArgEqual(lowerArg, key);
				if(keyLength > 0)
				{
					prg.mWaitAfterEnd = true;
					continue;
				}
				key = "workingpath:";
				keyLength = AgnosticArgStart(lowerArg, key);
				if(keyLength > 0)
				{
					prg.ActionItem.WorkingPath = arg.Substring(keyLength);
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
