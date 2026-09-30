/*
 * Copyright (c). 2026 Daniel Patterson, MCSD (danielanywhere).
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
#define Y_UP_POSITIVE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Geometry;
using Html;

using static SvgToolsLib.SvgToolsUtil;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	SvgPlasmaPlotter																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// G-code generator compatible with plasma plotting.
	/// </summary>
	public class SvgPlasmaPlotter
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* AdjustOffsetInside																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Adjust the tool offset to the inside of the shape.
		/// </summary>
		/// <param name="plotter">
		/// Reference to the plotter configuration for this session.
		/// </param>
		private static void AdjustOffsetInside(SvgPlasmaPlotter plotter)
		{
			WindingOrientationEnum winding = WindingOrientationEnum.None;

			if(plotter?.mLines.Count > 0)
			{
				winding = SvgPlotLineCollection.GetWindingDirection(
					plotter.mLines, plotter.mYUpPositive);
				//	Clockwise paths run to the right of the line.
				//	Counterclockwise paths run to the left of the line.
				switch(winding)
				{
					case WindingOrientationEnum.Clockwise:
						AdjustOffsetRight(plotter);
						break;
					case WindingOrientationEnum.CounterClockwise:
						AdjustOffsetLeft(plotter);
						break;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AdjustOffsetLeft																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Adjust the tool offset to the left of the line of travel.
		/// </summary>
		/// <param name="plotter">
		/// Reference to the plotter configuration for this session.
		/// </param>
		private static void AdjustOffsetLeft(SvgPlasmaPlotter plotter)
		{
			List<SvgPlotLineItem> lines = null;

			if(plotter != null)
			{
				lines = SvgPlotLineCollection.OffsetPolygon(plotter.mLines,
					plotter.mKerf / 2f, LineOffsetType.Left, plotter.mYUpPositive,
					plotter.mPrecision);
				plotter.mLines.Clear();
				plotter.mLines.AddRange(lines);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AdjustOffsetOutside																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Adjust the tool offset to the outside of the shape.
		/// </summary>
		/// <param name="plotter">
		/// Reference to the plotter configuration for this session.
		/// </param>
		private static void AdjustOffsetOutside(SvgPlasmaPlotter plotter)
		{
			WindingOrientationEnum winding = WindingOrientationEnum.None;

			if(plotter?.mLines.Count > 0)
			{
				winding = SvgPlotLineCollection.GetWindingDirection(
					plotter.mLines, plotter.mYUpPositive);
				//	Clockwise paths run to the left of the line.
				//	Counterclockwise paths run to the right of the line.
				switch(winding)
				{
					case WindingOrientationEnum.Clockwise:
						AdjustOffsetLeft(plotter);
						break;
					case WindingOrientationEnum.CounterClockwise:
						AdjustOffsetRight(plotter);
						break;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AdjustOffsetRight																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Adjust the tool offset to the right of the line of travel.
		/// </summary>
		/// <param name="plotter">
		/// Reference to the plotter configuration for this session.
		/// </param>
		private static void AdjustOffsetRight(SvgPlasmaPlotter plotter)
		{
			List<SvgPlotLineItem> lines = null;

			if(plotter != null)
			{
				lines = SvgPlotLineCollection.OffsetPolygon(plotter.mLines,
					plotter.mKerf / 2f, LineOffsetType.Right, plotter.mYUpPositive,
					plotter.mPrecision);
				plotter.mLines.Clear();
				plotter.mLines.AddRange(lines);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FlattenShapes																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Flatten all of the shapes in the caller's path.
		/// </summary>
		/// <param name="plotter">
		/// Reference to the plotter configuration for the current session.
		/// </param>
		/// <param name="actions">
		/// The collection of actions to plot.
		/// </param>
		private static void FlattenShapes(SvgPlasmaPlotter plotter,
			SvgPathActionCollection actions)
		{
			FArea area = null;
			bool bContinue = true;
			FVector2 center = null;
			//SvgPlotLineItem currentItem = null;
			FEllipse ellipse = null;
			//SvgPlotLineItem firstItem = null;
			float length = 0f;
			FVector2 point = null;
			List<FVector2> points = null;
			//SvgPlotLineItem prevItem = null;
			int sampleCount = 0;
			ItemTracker tracker = new ItemTracker();

			if(plotter != null && actions != null)
			{
				tracker.Lines = plotter.Lines;
				plotter.mLines.Clear();
				if(actions?.Count > 0 &&
					actions[0].ActionType != SvgPathActionType.Move)
				{
					tracker.CurrentItem = new SvgPlotLineItem()
					{
						ToolDown = false
					};
					tracker.Next();
				}
				foreach(SvgPathActionItem actionItem in actions)
				{
					switch(actionItem.ActionType)
					{
						case SvgPathActionType.ClosePath:
							SvgPlotLineCollection.RemoveEqual(tracker.Lines,
								plotter.mPrecision);
							if(tracker.FirstItem != null &&
								!tracker.Lines.Contains(tracker.FirstItem))
							{
								tracker.FirstItem =
									tracker.Lines.FirstOrDefault(x => x.ToolDown == true);
							}
							if(tracker.FirstItem != null && tracker.PreviousItem != null)
							{
								if(!VectorEquals(
									tracker.PreviousItem.End, tracker.FirstItem.Start))
								{
									tracker.CurrentItem = new SvgPlotLineItem()
									{
										Start = new FVector2(tracker.PreviousItem.End),
										End = new FVector2(tracker.FirstItem.Start),
										ToolDown = true
									};
								}
								else
								{
									//	Assure that the last item is registered properly.
									tracker.PreviousItem.End.X = tracker.FirstItem.Start.X;
									tracker.PreviousItem.End.Y = tracker.FirstItem.Start.Y;
								}
							}
							break;
						case SvgPathActionType.CubicBezier:
						case SvgPathActionType.ShorthandCubicCurve:
							if(actionItem is SvgPathCubicBezierActionItem cubicBezier &&
								plotter.mPrecision != 0f)
							{
								area = Bezier.GetCubicBoundingBox(
									cubicBezier.Start,
									cubicBezier.Control1,
									cubicBezier.Control2,
									cubicBezier.End,
									100);
								length = (float)(Math.Sqrt(
										(double)((area.Width * area.Width) + (area.Height * area.Height))
									));
								sampleCount = (int)(length / plotter.mPrecision);
								points = Bezier.GetCubicCurvePointsEquidistant(
									cubicBezier.Start,
									cubicBezier.Control1,
									cubicBezier.Control2,
									cubicBezier.End,
									sampleCount);
								foreach(FVector2 pointItem in points)
								{
									tracker.CurrentItem = new SvgPlotLineItem()
									{
										Start = new FVector2(tracker.PreviousItem.End),
										End = new FVector2(pointItem.X, pointItem.Y),
										ToolDown = true
									};
									tracker.Next();
								}
							}
							break;
						case SvgPathActionType.EllipticalArc:
							if(actionItem is SvgPathArcActionItem arc)
							{
								if(TryGetArcCenter(arc.Start, arc.End,
									arc.Radius, arc.Sweep, arc.IsLargeArc, out center))
								{
									ellipse = new FEllipse()
									{
										Center = center,
										RadiusX = arc.Radius.X,
										RadiusY = arc.Radius.Y,
										Rotation = arc.XAxisRotation
									};
									length = (arc.Radius.X + arc.Radius.Y) / 2f;
									sampleCount = (int)(length / plotter.mPrecision);
									point = GetEllipticalArcAngles(ellipse, arc.Start, arc.End);
									points = FEllipse.GetVerticesInArc(ellipse, sampleCount,
										point.X, point.Y - point.X);
									foreach(FVector2 pointItem in points)
									{
										tracker.CurrentItem = new SvgPlotLineItem()
										{
											Start = new FVector2(tracker.PreviousItem.End),
											End = new FVector2(pointItem.X, pointItem.Y),
											ToolDown = true
										};
										tracker.Next();
									}
								}
							}
							break;
						case SvgPathActionType.HorizontalLine:
							if(actionItem is SvgPathHorizontalActionItem horzLine)
							{
								tracker.CurrentItem = new SvgPlotLineItem()
								{
									Start = new FVector2(tracker.PreviousItem.End),
									End = new FVector2(horzLine.End.X, horzLine.End.Y),
									ToolDown = true
								};
							}
							break;
						case SvgPathActionType.Line:
							if(actionItem is SvgPathLineActionItem normalLine)
							{
								tracker.CurrentItem = new SvgPlotLineItem()
								{
									Start = new FVector2(tracker.PreviousItem.End),
									End = new FVector2(normalLine.End.X, normalLine.End.Y),
									ToolDown = true
								};
							}
							break;
						case SvgPathActionType.Move:
							if(actionItem is SvgPathMoveActionItem move)
							{
								tracker.CurrentItem = new SvgPlotLineItem()
								{
									Start = new FVector2(tracker.PreviousItem.End),
									End = new FVector2(move.End.X, move.End.Y),
									ToolDown = false
								};
							}
							break;
						case SvgPathActionType.None:
							break;
						case SvgPathActionType.QuadraticBezier:
						case SvgPathActionType.ShorthandQuadraticCurve:
							if(actionItem is SvgPathQuadraticBezierActionItem quadBezier)
							{
								area = Bezier.GetQuadraticBoundingBox(
									quadBezier.Start,
									quadBezier.Control,
									quadBezier.End,
									100);
								length = (float)(Math.Sqrt(
										(double)((area.Width * area.Width) + (area.Height * area.Height))
									)) * plotter.mPrecision;
								if(length != 0f)
								{
									sampleCount = (int)(1f / length);
								}
								else
								{
									sampleCount = 32;
								}
								points = Bezier.GetQuadraticCurvePointsEquidistant(
									quadBezier.Start,
									quadBezier.Control,
									quadBezier.End,
									sampleCount);
								foreach(FVector2 pointItem in points)
								{
									tracker.CurrentItem = new SvgPlotLineItem()
									{
										Start = new FVector2(tracker.PreviousItem.End),
										End = new FVector2(pointItem.X, pointItem.Y),
										ToolDown = true
									};
									tracker.Next();
								}
							}
							break;
						case SvgPathActionType.VerticalLine:
							if(actionItem is SvgPathVerticalActionItem vertLine)
							{
								tracker.CurrentItem = new SvgPlotLineItem()
								{
									Start = new FVector2(tracker.PreviousItem.End),
									End = new FVector2(vertLine.End.X, vertLine.End.Y),
									ToolDown = true
								};
							}
							break;
					}
					tracker.Next();
				}
				SvgPlotLineCollection.RemoveEqual(plotter.mLines, plotter.mPrecision);
				SvgPlotLineCollection.RemoveDuplicate(plotter.mLines,
					plotter.mPrecision);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GenerateGCodeFromShape																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Retrieve the g-code content that will cut the provided path.
		/// </summary>
		/// <param name="plotter">
		/// Reference to the plotter configuration for the current session.
		/// </param>
		/// <param name="pathSide">
		/// The side to offset the tool so the kerf aligns as desired with the
		/// line.
		/// </param>
		/// <returns>
		/// Generated g-code for the collection.
		/// </returns>
		private static string GenerateGCodeFromShape(
			SvgPlasmaPlotter plotter,
			PathSideEnum pathSide)
		{
			StringBuilder builder = new StringBuilder();
			FVector2 start = null;

			if(plotter != null)
			{
				switch(pathSide)
				{
					case PathSideEnum.Auto:
					case PathSideEnum.None:
					case PathSideEnum.RightOfTravel:
						AdjustOffsetRight(plotter);
						break;
					case PathSideEnum.LeftOfTravel:
						AdjustOffsetLeft(plotter);
						break;
					case PathSideEnum.Center:
						break;
					case PathSideEnum.InsideShape:
						AdjustOffsetInside(plotter);
						break;
					case PathSideEnum.OutsideShape:
						AdjustOffsetOutside(plotter);
						break;
				}
				if(plotter.mLines.Count > 0)
				{
					if(!plotter.mNegativeValuesPresent)
					{
						plotter.mNegativeValuesPresent = plotter.mLines.Exists(x =>
							x.Start.X < 0f ||
							x.Start.Y < 0f ||
							x.End.X < 0f ||
							x.End.Y < 0f);
					}
					start = plotter.mLines[0].Start;
					if(start.X != plotter.mCursor.X ||
						start.Y != plotter.mCursor.Y)
					{
						ToolMoveRapid(plotter, builder);
					}
					foreach(SvgPlotLineItem lineItem in plotter.mLines)
					{
						if(lineItem.ToolDown)
						{
							ToolDraw(plotter, lineItem.End, builder);
						}
						else
						{
							ToolMoveRapid(plotter, lineItem.End, builder);
						}
					}
					if(plotter.mToolDown)
					{
						ToolRaise(plotter, builder);
					}
				}
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ItemTracker																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Local item tracker.
		/// </summary>
		private class ItemTracker
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
			//*	_Constructor																													*
			//*-----------------------------------------------------------------------*
			/// <summary>
			/// Create a new instance of the ItemTracker item.
			/// </summary>
			public ItemTracker()
			{
				mPreviousItem = new SvgPlotLineItem()
				{
					Start = new FVector2(),
					End = new FVector2(),
					ToolDown = false
				};
			}
			//*-----------------------------------------------------------------------*

			//*-----------------------------------------------------------------------*
			//*	CurrentItem																														*
			//*-----------------------------------------------------------------------*
			/// <summary>
			/// Private member for <see cref="CurrentItem">CurrentItem</see>.
			/// </summary>
			private SvgPlotLineItem mCurrentItem = null;
			/// <summary>
			/// Get/Set a reference to the current item.
			/// </summary>
			public SvgPlotLineItem CurrentItem
			{
				get { return mCurrentItem; }
				set
				{
					mCurrentItem = value;
					if(mCurrentItem?.ToolDown == true && mFirstItem == null)
					{
						mFirstItem = mCurrentItem;
					}
				}
			}
			//*-----------------------------------------------------------------------*

			//*-----------------------------------------------------------------------*
			//*	FirstItem																															*
			//*-----------------------------------------------------------------------*
			/// <summary>
			/// Private member for <see cref="FirstItem">FirstItem</see>.
			/// </summary>
			private SvgPlotLineItem mFirstItem = null;
			/// <summary>
			/// Get/Set a reference to the first item in the path.
			/// </summary>
			public SvgPlotLineItem FirstItem
			{
				get { return mFirstItem; }
				set { mFirstItem = value; }
			}
			//*-----------------------------------------------------------------------*

			//*-----------------------------------------------------------------------*
			//*	Lines																																	*
			//*-----------------------------------------------------------------------*
			/// <summary>
			/// Private member for <see cref="Lines">Lines</see>.
			/// </summary>
			private SvgPlotLineCollection mLines = new SvgPlotLineCollection();
			/// <summary>
			/// Get/Set a reference to the collection of lines being generated for
			/// this instance.
			/// </summary>
			public SvgPlotLineCollection Lines
			{
				get { return mLines; }
				set { mLines = value; }
			}
			//*-----------------------------------------------------------------------*

			//*-----------------------------------------------------------------------*
			//* Next																																	*
			//*-----------------------------------------------------------------------*
			/// <summary>
			/// Increment the current line references.
			/// </summary>
			public void Next()
			{
				if(mCurrentItem != null)
				{
					if(mCurrentItem.ToolDown)
					{
						mLines.Add(mCurrentItem);
					}
					if(mCurrentItem.ToolDown == true && mFirstItem == null)
					{
						mFirstItem = mCurrentItem;
					}
					mPreviousItem = mCurrentItem;
				}
				mCurrentItem = null;
			}
			//*-----------------------------------------------------------------------*

			//*-----------------------------------------------------------------------*
			//*	PreviousItem																													*
			//*-----------------------------------------------------------------------*
			/// <summary>
			/// Private member for <see cref="PreviousItem">PreviousItem</see>.
			/// </summary>
			private SvgPlotLineItem mPreviousItem = null;
			/// <summary>
			/// Get/Set a reference to the previously visited item.
			/// </summary>
			public SvgPlotLineItem PreviousItem
			{
				get { return mPreviousItem; }
				set { mPreviousItem = value; }
			}
			//*-----------------------------------------------------------------------*

		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ReverseAxisY																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Reverse the Y-axis values of the lines in the supplied collection.
		/// </summary>
		/// <param name="document">
		/// Reference to the document being processed.
		/// </param>
		/// <param name="plotter">
		/// Reference to the plotter whose lines will be enumerated.
		/// </param>
		private static void ReverseAxisY(SvgDocumentItem document,
			SvgPlasmaPlotter plotter)
		{
			float height = 0f;
			SvgPlotLineCollection lines = null;

			if(document != null && plotter?.mLines.Count > 0)
			{
				lines = plotter.mLines;
				height = document.GetHeightmm();
				foreach(SvgPlotLineItem lineItem in lines)
				{
					lineItem.Start.Y = height - lineItem.Start.Y;
					lineItem.End.Y = height - lineItem.End.Y;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToolActivate																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Lower and activate the tool.
		/// </summary>
		/// <param name="plotter">
		/// Reference to the plotter configuration.
		/// </param>
		/// <param name="builder">
		/// Reference to the string builder to be appended to.
		/// </param>
		private static void ToolActivate(SvgPlasmaPlotter plotter,
			StringBuilder builder)
		{
			if(plotter != null && builder != null)
			{
				builder.AppendLine(
					$"G0 Z{plotter.mPierceHeight:0.000} ; Ready beam...");
				builder.AppendLine($"M3 ; Activate.");
				plotter.mBeamOn = true;
				builder.AppendLine(
					$"G4 S{plotter.mPierceDelay:0.###} ; Wait for puncture.");
				builder.AppendLine(
					$"G1 Z{plotter.mCutHeight:0.000} F{plotter.mFeedRate:0.000}");
				plotter.mCursor.Z = plotter.mCutHeight;
				plotter.mToolDown = true;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToolDraw																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Draw a single line with the tool.
		/// </summary>
		/// <param name="plotter">
		/// Reference to the plotter configuration.
		/// </param>
		/// <param name="endPoint">
		/// Reference to the end point to be reached.
		/// </param>
		/// <param name="builder">
		/// Reference to the string builder to be appended to.
		/// </param>
		private static void ToolDraw(SvgPlasmaPlotter plotter,
			FVector2 endPoint, StringBuilder builder)
		{
			if(plotter != null && endPoint != null && builder != null)
			{
				if(!plotter.mToolDown)
				{
					ToolActivate(plotter, builder);
				}
				builder.AppendLine($"G1 X{endPoint.X:0.000} Y{endPoint.Y:0.000}");
				plotter.mCursor.X = endPoint.X;
				plotter.mCursor.Y = endPoint.Y;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToolMoveRapid																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Rapid travel to the starting coordinate of the provided line.
		/// </summary>
		/// <param name="plotter">
		/// Reference to the plotter configuration.
		/// </param>
		/// <param name="endPoint">
		/// Reference to the end point to be reached.
		/// </param>
		/// <param name="builder">
		/// Reference to the string builder to be appended to.
		/// </param>
		private static void ToolMoveRapid(SvgPlasmaPlotter plotter,
			FVector2 endPoint, StringBuilder builder)
		{
			if(plotter != null && endPoint != null && builder != null)
			{
				if(plotter.mToolDown)
				{
					ToolRaise(plotter, builder);
				}
				builder.AppendLine($"G0 X{endPoint.X:0.000} Y{endPoint.Y:0.000}");
				plotter.mCursor.X = endPoint.X;
				plotter.mCursor.Y = endPoint.Y;
			}
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Rapid travel to the starting coordinate of the queue.
		/// </summary>
		/// <param name="plotter">
		/// Reference to the plotter configuration.
		/// </param>
		/// <param name="builder">
		/// Reference to the string builder to be appended to.
		/// </param>
		private static void ToolMoveRapid(SvgPlasmaPlotter plotter,
			StringBuilder builder)
		{
			FVector2 start = null;

			if(plotter?.mLines.Count > 0 && builder != null)
			{
				if(plotter.mToolDown)
				{
					ToolRaise(plotter, builder);
				}
				start = plotter.mLines[0].Start;
				builder.AppendLine(
					$"G0 X{start.X:0.000} Y{start.Y:0.000} ; Position.");
				plotter.mCursor.X = start.X;
				plotter.mCursor.Y = start.Y;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToolRaise																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raise the tool.
		/// </summary>
		/// <param name="plotter">
		/// Reference to the plotter configuration.
		/// </param>
		/// <param name="builder">
		/// Reference to the string builder to be appended to.
		/// </param>
		private static void ToolRaise(SvgPlasmaPlotter plotter,
			StringBuilder builder)
		{
			if(plotter != null && builder != null)
			{
				if(plotter.mBeamOn)
				{
					if(plotter.mOffDelay != 0f)
					{
						builder.AppendLine($"G4 S{plotter.mOffDelay:0.###} ; Off delay.");
					}
					builder.AppendLine("M5 ; Turn off beam.");
					plotter.mBeamOn = false;
				}
				builder.AppendLine(
					$"G0 Z{plotter.mSafeHeight:0.000} ; Raise the tool.");
				plotter.mCursor.Z = plotter.mSafeHeight;
				plotter.mToolDown = false;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* VectorEquals																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the values of vector a are
		/// equal to the values of vector b, within tolerance.
		/// </summary>
		/// <param name="a">
		/// Reference to the first vector to compare.
		/// </param>
		/// <param name="b">
		/// Reference to the second vector to compare.
		/// </param>
		/// <returns>
		/// True if the two vectors are equal within tolerance. Otherwise, false.
		/// </returns>
		private static bool VectorEquals(FVector2 a, FVector2 b)
		{
			bool result = false;

			result = (a != null && b != null &&
				(float)Math.Abs(b.X - a.X) < 0.001f &&
					(float)Math.Abs(b.Y - a.Y) < 0.001f);
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	BeamOn																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="BeamOn">BeamOn</see>.
		/// </summary>
		private bool mBeamOn = false;
		/// <summary>
		/// Get/Set a value indicating whether the beam is currently turned on.
		/// </summary>
		public bool BeamOn
		{
			get { return mBeamOn; }
			set { mBeamOn = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Cursor																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Cursor">Cursor</see>.
		/// </summary>
		private FVector3 mCursor = new FVector3();
		/// <summary>
		/// Get/Set a reference to the current tool location.
		/// </summary>
		public FVector3 Cursor
		{
			get { return mCursor; }
			set { mCursor = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	CutHeight																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="CutHeight">CutHeight</see>.
		/// </summary>
		private float mCutHeight = 1.5f;
		/// <summary>
		/// Get/Set the height at which the continuous cut is made.
		/// </summary>
		public float CutHeight
		{
			get { return mCutHeight; }
			set { mCutHeight = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	DefaultPathSide																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="DefaultPathSide">DefaultPathSide</see>.
		/// </summary>
		private PathSideEnum mDefaultPathSide = PathSideEnum.Auto;
		/// <summary>
		/// Get/Set the default tool path orientation side.
		/// </summary>
		public PathSideEnum DefaultPathSide
		{
			get { return mDefaultPathSide; }
			set { mDefaultPathSide = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FeedRate																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="FeedRate">FeedRate</see>.
		/// </summary>
		private float mFeedRate = 1000f;
		/// <summary>
		/// Get/Set the feed rate of the cutting path, in active units.
		/// </summary>
		public float FeedRate
		{
			get { return mFeedRate; }
			set { mFeedRate = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GenerateGCode																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Generate the g-code plot content from the supplied document.
		/// </summary>
		/// <param name="plotter">
		/// Reference to the active plotter configuration.
		/// </param>
		/// <param name="document">
		/// Reference to the SVG document to analyze.
		/// </param>
		/// <returns>
		/// G-code of the first found path.
		/// </returns>
		/// <remarks>
		/// This version assumes metric numbering with a millimeter base.
		/// </remarks>
		public static string GenerateGCode(SvgPlasmaPlotter plotter,
			SvgDocumentItem document)
		{
			SvgPathActionItem action = null;
			SvgPathActionCollection actionGroup = null;
			List<SvgPathActionCollection> actionGroups =
				new List<SvgPathActionCollection>();
			SvgPathActionCollection actions = null;
			StringBuilder builder = new StringBuilder();
			int count = 0;
			HtmlDocument doc = null;
			int index = 0;
			List<HtmlNodeItem> paths = null;
			PlotPointsFCollection plotPoints = null;
			PathSideEnum side = PathSideEnum.None;

			builder.AppendLine("G90 ; Absolute positioning.");
			builder.AppendLine("G21 ; Metric units (mm).");
			if(plotter != null && document != null)
			{
				ToolRaise(plotter, builder);
				doc = document.Document;
				paths = doc.Nodes.FindMatches(x => x.NodeType == "path");
				foreach(HtmlNodeItem pathItem in paths)
				{
					actionGroup = new SvgPathActionCollection();
					actionGroups.Clear();
					plotter.mLines.Clear();
					plotPoints =
						PlotPointsFCollection.Parse(pathItem.Attributes.GetValue("d"));
					if(plotPoints?.Count > 0)
					{
						actionGroups.Add(actionGroup);
						actions = SvgPathActionCollection.FromPlotPoints(plotPoints);
						foreach(SvgPathActionItem actionItem in actions)
						{
							actionGroup.Add(actionItem);
							if(actionItem.ActionType == SvgPathActionType.ClosePath)
							{
								//	Closed shape. Start a new path.
								actionGroup = new SvgPathActionCollection();
								actionGroups.Add(actionGroup);
							}
						}
						if(actionGroup.Count == 0)
						{
							actionGroups.RemoveAt(actionGroups.Count - 1);
						}
						//foreach(SvgPathActionCollection actionGroupItem in actionGroups)
						//{
						//	SvgPathActionCollection.Trim(actionGroupItem);
						//}
						count = actionGroups.Count;
						switch(plotter.DefaultPathSide)
						{
							case PathSideEnum.Auto:
							case PathSideEnum.None:
								//	Draw all of the inside cuts first.
								for(index = 1; index < count; index++)
								{
									actionGroup = actionGroups[index];
									switch(plotter.DefaultPathSide)
									{
										case PathSideEnum.Auto:
										case PathSideEnum.None:
											side = (SvgPathActionCollection.IsClosed(actionGroup) ?
												PathSideEnum.InsideShape : PathSideEnum.RightOfTravel);
											break;
										default:
											side = plotter.DefaultPathSide;
											break;
									}
									FlattenShapes(plotter, actionGroup);
									if(plotter.mYUpPositive)
									{
										ReverseAxisY(document, plotter);
									}
									builder.Append(
										GenerateGCodeFromShape(plotter,
											PathSideEnum.InsideShape));
								}
								//	Draw the outside cut.
								if(count > 0)
								{
									actionGroup = actionGroups[0];
									FlattenShapes(plotter, actionGroup);
									if(plotter.mYUpPositive)
									{
										ReverseAxisY(document, plotter);
									}
									builder.Append(
										GenerateGCodeFromShape(plotter,
											PathSideEnum.OutsideShape));
								}
								break;
							case PathSideEnum.Center:
								for(index = 0; index < count; index ++)
								{
									actionGroup = actionGroups[index];
									FlattenShapes(plotter, actionGroup);
									if(plotter.mYUpPositive)
									{
										ReverseAxisY(document, plotter);
									}
									builder.Append(
										GenerateGCodeFromShape(plotter,
											PathSideEnum.Center));
								}
								break;
							case PathSideEnum.InsideShape:
								for(index = 0; index < count; index++)
								{
									actionGroup = actionGroups[index];
									FlattenShapes(plotter, actionGroup);
									if(plotter.mYUpPositive)
									{
										ReverseAxisY(document, plotter);
									}
									builder.Append(
										GenerateGCodeFromShape(plotter,
											PathSideEnum.InsideShape));
								}
								break;
							case PathSideEnum.OutsideShape:
								for(index = 0; index < count; index++)
								{
									actionGroup = actionGroups[index];
									FlattenShapes(plotter, actionGroup);
									if(plotter.mYUpPositive)
									{
										ReverseAxisY(document, plotter);
									}
									builder.Append(
										GenerateGCodeFromShape(plotter,
											PathSideEnum.OutsideShape));
								}
								break;
							case PathSideEnum.RightOfTravel:
								for(index = 0; index < count; index++)
								{
									actionGroup = actionGroups[index];
									FlattenShapes(plotter, actionGroup);
									if(plotter.mYUpPositive)
									{
										ReverseAxisY(document, plotter);
									}
									builder.Append(
										GenerateGCodeFromShape(plotter,
											PathSideEnum.RightOfTravel));
								}
								break;
						}
					}
				}
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Kerf																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Kerf">Kerf</see>.
		/// </summary>
		private float mKerf = 2f;
		/// <summary>
		/// Get/Set the cut width of the tool.
		/// </summary>
		public float Kerf
		{
			get { return mKerf; }
			set { mKerf = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Lines																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Lines">Lines</see>.
		/// </summary>
		private SvgPlotLineCollection mLines = new SvgPlotLineCollection();
		/// <summary>
		/// Get a reference to the collection of straight lines present in the
		/// shape.
		/// </summary>
		public SvgPlotLineCollection Lines
		{
			get { return mLines; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	NegativeValuesPresent																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="NegativeValuesPresent">
		/// NegativeValuesPresent</see>.
		/// </summary>
		private bool mNegativeValuesPresent = false;
		/// <summary>
		/// Get/Set a value indicating whether negative values were found during
		/// parsing.
		/// </summary>
		public bool NegativeValuesPresent
		{
			get { return mNegativeValuesPresent; }
			set { mNegativeValuesPresent = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	OffDelay																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="OffDelay">OffDelay</see>.
		/// </summary>
		private float mOffDelay = 0.5f;
		/// <summary>
		/// Get/Set the delay to wait at the end of the cut path before turning
		/// off the beam, in seconds.
		/// </summary>
		public float OffDelay
		{
			get { return mOffDelay; }
			set { mOffDelay = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	PierceDelay																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="PierceDelay">PierceDelay</see>.
		/// </summary>
		private float mPierceDelay = 0.5f;
		/// <summary>
		/// Get/Set the delay to wait between the pierce signal and the plunge to
		/// cut, in seconds.
		/// </summary>
		public float PierceDelay
		{
			get { return mPierceDelay; }
			set { mPierceDelay = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	PierceHeight																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="PierceHeight">PierceHeight</see>.
		/// </summary>
		private float mPierceHeight = 3;
		/// <summary>
		/// Get/Set the height of the tool at which the plasma will pierce the
		/// material.
		/// </summary>
		public float PierceHeight
		{
			get { return mPierceHeight; }
			set { mPierceHeight = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Precision																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="Precision">Precision</see>.
		/// </summary>
		private float mPrecision = 0.1f;
		/// <summary>
		/// Get/Set the precision at which to create straight lines from the
		/// complex shapes.
		/// </summary>
		public float Precision
		{
			get { return mPrecision; }
			set { mPrecision = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SafeHeight																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="SafeHeight">SafeHeight</see>.
		/// </summary>
		private float mSafeHeight = 20f;
		/// <summary>
		/// Get/Set the safe height of the tool.
		/// </summary>
		public float SafeHeight
		{
			get { return mSafeHeight; }
			set { mSafeHeight = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ToolDown																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ToolDown">ToolDown</see>.
		/// </summary>
		private bool mToolDown = false;
		/// <summary>
		/// Get/Set a value indicating whether the tool is currently down (active).
		/// </summary>
		public bool ToolDown
		{
			get { return mToolDown; }
			set { mToolDown = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	UnitSystem																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="UnitSystem">UnitSystem</see>.
		/// </summary>
		private UnitSystemEnum mUnitSystem = UnitSystemEnum.Metric;
		/// <summary>
		/// Get/Set the unit system to be used on the job.
		/// </summary>
		public UnitSystemEnum UnitSystem
		{
			get { return mUnitSystem; }
			set { mUnitSystem = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	YUpPositive																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="YUpPositive">YUpPositive</see>.
		/// </summary>
#if NO_Y_UP_POSITIVE
		private bool mYUpPositive = false;
#else
		private bool mYUpPositive = true;
#endif
		/// <summary>
		/// Get/Set a value indicating whether the value on the Y-axis increases
		/// in an upward direction, such as in the case of on a physical plotter
		/// bed. If false, Y increases in a downward direction, such as in the
		/// case of a page on a visual editor.
		/// </summary>
		/// <remarks>
		/// In development mode, this values defaults to false.
		/// In production mode, this value defaults to true.
		/// </remarks>
		public bool YUpPositive
		{
			get { return mYUpPositive; }
			set { mYUpPositive = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
