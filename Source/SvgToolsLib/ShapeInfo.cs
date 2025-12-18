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
using System.Linq;
using System.Text;

using Html;
using static SvgToolsLib.SvgToolsUtil;
using System.Text.RegularExpressions;
using Geometry;
using System.Diagnostics;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	ShapeInfoCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ShapeInfoItem Items.
	/// </summary>
	public class ShapeInfoCollection : List<ShapeInfoItem>
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
		//*	Add																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add a shape information item to the collection.
		/// </summary>
		/// <param name="item">
		/// Reference to the item to be added to the collection.
		/// </param>
		public new void Add(ShapeInfoItem item)
		{
			if(item != null)
			{
				item.Parent = this;
				base.Add(item);
			}
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////* ApplyTransforms																												*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Apply transforms on all of the objects in the structure of shapes.
		///// </summary>
		///// <param name="shapes">
		///// Collection of shapes containing values to back-transform.
		///// </param>
		//public static void ApplyTransforms(ShapeInfoCollection shapes)
		//{
		//	if(shapes?.Count > 0)
		//	{
		//		foreach(ShapeInfoItem shapeItem in shapes)
		//		{
		//			ShapeInfoItem.ApplyTransforms(shapeItem);
		//		}
		//	}
		//}
		////*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	CreateShapes																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Assign shape information to the specified nodes and all of their
		/// children.
		/// </summary>
		/// <param name="nodes">
		/// Reference to the collection of HTML nodes to activate.
		/// </param>
		/// <returns>
		/// A newly created and filled shape information collection.
		/// </returns>
		public static ShapeInfoCollection CreateShapes(HtmlNodeCollection nodes)
		{
			ShapeInfoItem shape = null;
			ShapeInfoCollection shapes = new ShapeInfoCollection();

			if(nodes?.Count > 0)
			{
				foreach(HtmlNodeItem nodeItem in nodes)
				{
					shape = ShapeInfoItem.CreateShapes(nodeItem);
					if(shape != null)
					{
						shapes.Add(shape);
					}
				}
			}
			return shapes;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Parent																																*
		//*-----------------------------------------------------------------------*
		private ShapeInfoItem mParent = null;
		/// <summary>
		/// Get/Set a reference to the parent item of this collection.
		/// </summary>
		public ShapeInfoItem Parent
		{
			get { return mParent; }
			set { mParent = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RemoveTransforms																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Remove the applicable transforms from all nodes and their children.
		/// </summary>
		/// <param name="shapes">
		/// Reference to the collection os shape information item from which the
		/// consumed transforms will be removed.
		/// </param>
		public static void RemoveTransforms(ShapeInfoCollection shapes)
		{
			if(shapes?.Count > 0)
			{
				foreach(ShapeInfoItem shapeItem in shapes)
				{
					ShapeInfoItem.RemoveTransforms(shapeItem);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* UpdateNodes																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Update all associated HTML node attributes from property values found
		/// in this collection of shapes.
		/// </summary>
		/// <param name="shapes">
		/// Reference to the collection of shape information items associated with
		/// HTML nodes.
		/// </param>
		public static void UpdateNodes(ShapeInfoCollection shapes)
		{
			if(shapes?.Count > 0)
			{
				foreach(ShapeInfoItem shapeItem in shapes)
				{
					ShapeInfoItem.UpdateNodes(shapeItem);
				}
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ShapeInfoItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Scratchpad information about a shape that can be referred to when
	/// calculating transformations and other transient information about shapes.
	/// </summary>
	public class ShapeInfoItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		/// <summary>
		/// Properties that identify dimensional comprehensions.
		/// </summary>
		private static string[] mDimensionalProperties = new string[]
		{
			"width", "height", "r", "rx", "ry"
		};
		/// <summary>
		/// Properties that exhibit horizontal direction comprehensions.
		/// </summary>
		private static string[] mHorizontalProperties = new string[]
		{
			"x", "cx", "x1", "x2", "width", "rx", "fx"
		};
		/// <summary>
		/// Properties that identify list comprehensions.
		/// </summary>
		private static string[] mListProperties = new string[] { "d", "points" };
		/// <summary>
		/// Properties that identify location compresensions.
		/// </summary>
		private static string[] mLocationProperties = new string[]
		{
			"x", "y", "cx", "cy", "fx", "fy", "x1", "y1", "x2", "y2"
		};
		/// <summary>
		/// Non directional properties.
		/// </summary>
		private static string[] mNonDirectionalProperties = new string[]
		{
			"r"
		};
		/// <summary>
		/// Properties that exhibit vertical direction comprehensions.
		/// </summary>
		private static string[] mVerticalProperties = new string[]
		{
			"y", "cy", "y1", "y2", "height", "ry", "fy"
		};

		//*-----------------------------------------------------------------------*
		//* AssignProperties																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Assign the correct properties from the node to the shape, depending
		/// on 
		/// </summary>
		/// <param name="node">
		/// Reference to the node from which the source values will be retrieved.
		/// </param>
		/// <param name="shape">
		/// Reference to the shape containing the properties collections to
		/// be updated.
		/// </param>
		/// <param name="propertyNames">
		/// Reference to an array of property names to be assigned.
		/// </param>
		private static void AssignProperties(HtmlNodeItem node,
			ShapeInfoItem shape, string[] propertyNames)
		{
			HtmlAttributeItem attrib = null;
			List<string> calculatedPropertyNames = new List<string>();

			if(node != null && shape?.mShapeType != ShapeInfoTypeEnum.None &&
				propertyNames?.Length > 0)
			{
				foreach(string propertyNameItem in propertyNames)
				{
					attrib = node.Attributes.FirstOrDefault(x =>
						x.Name == propertyNameItem);
					//	The following section was commented to keep an item
					//	from having attributes that override another element's
					//	value.
					//if(attrib == null)
					//{
					//	//	If the attribute didn't exist, then create it if it is
					//	//	dimensional or locational.
					//	if(mDimensionalProperties.Contains(propertyNameItem) ||
					//		mLocationProperties.Contains(propertyNameItem))
					//	{
					//		attrib = new HtmlAttributeItem();
					//		attrib.Name = propertyNameItem;
					//		attrib.Value = "0";
					//	}
					//}
					if(attrib != null)
					{
						switch(attrib.Name)
						{
							case "d":
								shape.mPlotPoints =
									PlotPointsFCollection.Parse(attrib.Value);
								break;
							case "points":
								shape.mPoints = ParsePoints(attrib.Value);
								break;
							default:
								//	In this version, a numeric value can be normal,
								//	or can be a percentage. In the case this item is
								//	a percentage and has an href, check that item for
								//	width and height. Otherwise, use the first nearest
								//	parents where the width and height are specified.
								if(attrib.Value?.Length > 0 && attrib.Value.EndsWith("%"))
								{
									//	This item is a percentage property save for last.
									calculatedPropertyNames.Add(attrib.Name);
									shape.mProperties.SetValue(propertyNameItem, 0f);
								}
								else
								{
									shape.mProperties.SetValue(propertyNameItem,
										ToFloat(attrib.Value));
								}
								break;
						}
					}
				}
				foreach(string propertyNameItem in calculatedPropertyNames)
				{
					attrib = node.Attributes.FirstOrDefault(x =>
						x.Name == propertyNameItem);
					if(attrib != null)
					{
						shape.mProperties.SetValue(propertyNameItem,
							ToFloat(attrib.Value, shape, propertyNameItem));
					}
				}
				shape.mProperties.SetUnchanged();
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FindShape																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Find the first shape within the structure that matches the
		/// caller-specified expression.
		/// </summary>
		/// <param name="baseItem">
		/// The base item to match in the current context.
		/// </param>
		/// <param name="match">
		/// Predicate expression to match.
		/// </param>
		/// <returns>
		/// Reference to the shape information item found within the structure,
		/// if found. Otherwise, null.
		/// </returns>
		private static ShapeInfoItem FindShape(ShapeInfoItem baseItem,
			Predicate<ShapeInfoItem> match)
		{
			ShapeInfoItem result = null;

			if(baseItem != null && match != null)
			{
				if(match.Invoke(baseItem))
				{
					//	The referenced item was a match.
					result = baseItem;
				}
				else
				{
					foreach(ShapeInfoItem shapeItem in baseItem.mShapes)
					{
						result = FindShape(shapeItem, match);
						if(result != null)
						{
							break;
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetBoundingBoxX1Y1X2Y2																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the bounding box of the shape as x1, y1, x2, y1 coordinates,
		/// from the underlying node of the provided shape.
		/// </summary>
		/// <param name="shape">
		/// Reference to the shape to inspect.
		/// </param>
		/// <returns>
		/// Reference to a newly created HTML attribute collection containing
		/// the specified attributes, if found. Otherwise, null.
		/// </returns>
		/// <remarks>
		/// In this function, the shape's properties are used by default. When
		/// a shape property is not found, the associated node is checked for
		/// an attribute of the same name, and that value is used if found.
		/// </remarks>
		private static HtmlAttributeCollection GetBoundingBoxX1Y1X2Y2(
			ShapeInfoItem shape)
		{
			BoundingObjectItem bounds = null;
			float cx = 0f;
			float cy = 0f;
			float height = 0f;
			HtmlNodeItem node = null;
			float r = 0f;
			float rx = 0f;
			float ry = 0f;
			HtmlAttributeCollection result = null;
			float width = 0f;
			float x = 0f;
			float y = 0f;

			if(shape != null && shape.mNode != null)
			{
				if(shape.mShapeType != ShapeInfoTypeEnum.None)
				{
					result = new HtmlAttributeCollection();
					node = shape.mNode;
				}
				switch(shape.mShapeType)
				{
					case ShapeInfoTypeEnum.Circle:
					case ShapeInfoTypeEnum.Radial:
						//	cx, cy, r.
						cx = ToFloat(GetShapeOrNodeValue(shape, "cx"));
						cy = ToFloat(GetShapeOrNodeValue(shape, "cy"));
						r = ToFloat(GetShapeOrNodeValue(shape, "r"));
						result.Add("x1", $"{cx - r}");
						result.Add("y1", $"{cy - r}");
						result.Add("x2", $"{cx + r}");
						result.Add("y2", $"{cy + r}");
						break;
					case ShapeInfoTypeEnum.Ellipse:
						//	cx, cy, rx, ry.
						cx = ToFloat(GetShapeOrNodeValue(shape, "cx"));
						cy = ToFloat(GetShapeOrNodeValue(shape, "cy"));
						rx = ToFloat(GetShapeOrNodeValue(shape, "rx"));
						ry = ToFloat(GetShapeOrNodeValue(shape, "ry"));
						result.Add("x1", $"{cx - rx}");
						result.Add("y1", $"{cy - ry}");
						result.Add("x2", $"{cx + rx}");
						result.Add("y2", $"{cy + ry}");
						break;
					case ShapeInfoTypeEnum.Line:
						//	x1, y1, x2, y2.
						result.Add("x1",
							ToFloat(GetShapeOrNodeValue(shape, "x1")).ToString());
						result.Add("y1",
							ToFloat(GetShapeOrNodeValue(shape, "y1")).ToString());
						result.Add("x2",
							ToFloat(GetShapeOrNodeValue(shape, "x2")).ToString());
						result.Add("y2",
							ToFloat(GetShapeOrNodeValue(shape, "y2")).ToString());
						break;
					case ShapeInfoTypeEnum.Location:
						//	Any kind of node with x and y values.
						result.Add("x1",
							ToFloat(GetShapeOrNodeValue(shape, "x")).ToString());
						result.Add("y1",
							ToFloat(GetShapeOrNodeValue(shape, "y")).ToString());
						result.Add("x2",
							ToFloat(GetShapeOrNodeValue(shape, "x")).ToString());
						result.Add("y2",
							ToFloat(GetShapeOrNodeValue(shape, "y")).ToString());
						break;
					case ShapeInfoTypeEnum.Path:
						//	d.
						bounds = CalcBounds(shape.mPlotPoints);
						result.Add("x1", bounds.MinX.ToString());
						result.Add("y1", bounds.MinY.ToString());
						result.Add("x2", bounds.MaxX.ToString());
						result.Add("y2", bounds.MaxY.ToString());
						break;
					case ShapeInfoTypeEnum.Poly:
						//	points.
						bounds = CalcBounds(shape.mPoints);
						result.Add("x1", bounds.MinX.ToString());
						result.Add("y1", bounds.MinY.ToString());
						result.Add("x2", bounds.MaxX.ToString());
						result.Add("y2", bounds.MaxY.ToString());
						break;
					case ShapeInfoTypeEnum.Rect:
						//	x, y, width, height.
						x = ToFloat(GetShapeOrNodeValue(shape, "x"));
						y = ToFloat(GetShapeOrNodeValue(shape, "y"));
						width = ToFloat(GetShapeOrNodeValue(shape, "width"));
						height = ToFloat(GetShapeOrNodeValue(shape, "height"));
						result.Add("x1", $"{x}");
						result.Add("y1", $"{y}");
						result.Add("x2", $"{x + width}");
						result.Add("y2", $"{y + height}");
						break;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetReferenceDimension																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the primary reference dimension value for the speccified shape.
		/// </summary>
		/// <param name="shape">
		/// Reference to the shape containing the dimension to find.
		/// </param>
		/// <returns>
		/// The value of the primary reference dimension for the caller's shape.
		/// </returns>
		private static float GetReferenceDimension(ShapeInfoItem shape)
		{
			BoundingObjectItem bounds = null;
			float result = 0f;

			if(shape != null)
			{
				switch(shape.mShapeType)
				{
					case ShapeInfoTypeEnum.Circle:
					case ShapeInfoTypeEnum.Radial:
						//	r.
						result = shape.mProperties.GetValue("r");
						break;
					case ShapeInfoTypeEnum.Ellipse:
						//	rx, ry.
						result = (shape.mProperties.GetValue("rx") +
							shape.mProperties.GetValue("ry")) / 2f;
						break;
					case ShapeInfoTypeEnum.Line:
						//	x1, y1, x2, y2.
						result = ((shape.mProperties.GetValue("x2") -
							shape.mProperties.GetValue("x1")) +
							(shape.Properties.GetValue("y2") -
							shape.mProperties.GetValue("y1"))) / 2f;
						break;
					case ShapeInfoTypeEnum.Path:
						bounds = CalcBounds(shape.mPlotPoints);
						result = (bounds.GetWidth() +
							bounds.GetHeight()) / 2f;
						break;
					case ShapeInfoTypeEnum.Poly:
						bounds = CalcBounds(shape.mPoints);
						result = (bounds.GetWidth() +
							bounds.GetHeight()) / 2f;
						break;
					case ShapeInfoTypeEnum.Rect:
						//	width, height.
						result = (shape.mProperties.GetValue("width") +
							shape.mProperties.GetValue("height")) / 2f;
						break;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetRoot																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the root node of the supplied item.
		/// </summary>
		/// <param name="shape">
		/// Reference to the shape to be compared.
		/// </param>
		/// <returns>
		/// Root node in the structure referred to by the supplied node, if found.
		/// Otherwise, null.
		/// </returns>
		private static ShapeInfoItem GetRoot(ShapeInfoItem shape)
		{
			ShapeInfoItem result = null;

			if(shape != null)
			{
				if(shape != null && shape.mParent != null &&
					shape.mParent.Parent != null)
				{
					result = GetRoot(shape.mParent.Parent);
				}
				else
				{
					result = shape;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetShapeOrNodeValue																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the specified property on the provided shape, if
		/// found, falling back to the value of the attribute of the same name
		/// on the associated node when not found on the shape.
		/// </summary>
		/// <param name="shape">
		/// Reference to the shape to be inspected for the property.
		/// </param>
		/// <param name="propertyName">
		/// Name of the shape property or node attribute to return.
		/// </param>
		/// <returns>
		/// Value of the shape property or node attribute found, if found.
		/// Otherwise, an empty string.
		/// </returns>
		private static string GetShapeOrNodeValue(ShapeInfoItem shape,
			string propertyName)
		{
			string propertyNameLower = "";
			string result = "";

			if(shape != null && propertyName?.Length > 0)
			{
				propertyNameLower = propertyName.ToLower();
				if(shape.mProperties.Exists(x =>
					x.Name.ToLower() == propertyNameLower))
				{
					result = shape.mProperties.GetValue(propertyName).ToString();
				}
				else if(shape.mNode != null &&
					shape.mNode.Attributes.Exists(x =>
						x.Name.ToLower() == propertyNameLower))
				{
					result = shape.mNode.Attributes.GetValue(propertyName);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ParsePoints																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a list of floating point points from the caller's value.
		/// </summary>
		/// <param name="value">
		/// String containing one or more points, separated by string or comma.
		/// </param>
		/// <returns>
		/// Reference to a newly created list of floating point points.
		/// </returns>
		private static List<FVector2> ParsePoints(string value)
		{
			int count = 0;
			int index = 0;
			Match match = null;
			MatchCollection matches = null;
			FVector2 point = null;
			List<FVector2> points = new List<FVector2>();
			string text = "";

			if(value?.Length > 0)
			{
				matches = Regex.Matches(value, ResourceMain.rxFindSvgTransformParams);
				count = matches.Count;
				for(index = 0; index < count; index += 2)
				{
					point = new FVector2();
					match = matches[index];
					text = GetValue(match, "param");
					if(IsNumeric(text))
					{
						point.X = ToFloat(text);
					}
					if(index + 1 < count)
					{
						match = matches[index + 1];
						text = GetValue(match, "param");
						if(IsNumeric(text))
						{
							point.Y = ToFloat(text);
						}
					}
					points.Add(point);
				}
			}
			return points;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TransformMatrixCircle																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Perform matrix transformation on the provided circle.
		/// </summary>
		/// <param name="shape">
		/// Reference to the rectangular shape to be transformed, having cx, cy and
		/// r properties.
		/// </param>
		/// <param name="transform">
		/// Reference to the transform containing the 6 values representing the
		/// member values of a 3x2 matrix.
		/// </param>
		private static void TransformMatrixCircle(ShapeInfoItem shape,
			TransformItem transform)
		{
			float cx = 0f;
			float cy = 0f;
			Matrix32 m32 = null;
			FVector2 point = null;
			float r = 0f;
			float x1 = 0f;
			float x2 = 0f;
			float y1 = 0f;
			float y2 = 0f;

			if(shape != null && transform != null)
			{
				m32 = new Matrix32();
				m32.AssignSequential(transform.Parameters);
				cx = shape.mProperties.GetValue("cx");
				cy = shape.mProperties.GetValue("cy");
				r = shape.mProperties.GetValue("r");
				x1 = cx - r;
				y1 = cy - r;
				x2 = cx + r;
				y2 = cy + r;
				point = m32.Transform(x1, y1);
				x1 = point.X;
				y1 = point.Y;
				point = m32.Transform(x2, y2);
				x2 = point.X;
				y2 = point.Y;
				r = (x2 - x1) / 2f;
				cx = x1 + r;
				cy = y1 + r;
				shape.mProperties.SetValue("cx", cx);
				shape.mProperties.SetValue("cy", cy);
				shape.mProperties.SetValue("r", r);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TransformMatrixEllipse																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Perform a matrix transformation on the provided ellipse.
		/// </summary>
		/// <param name="shape">
		/// Reference to the rectangular shape to be transformed, having cx, cy,
		/// rx, and ry properties.
		/// </param>
		/// <param name="transform">
		/// Reference to the transform containing the 6 values representing the
		/// member values of a 3x2 matrix.
		/// </param>
		private static void TransformMatrixEllipse(ShapeInfoItem shape,
			TransformItem transform)
		{
			float cx = 0f;
			float cy = 0f;
			Matrix32 m32 = null;
			FVector2 point = null;
			float rx = 0f;
			float ry = 0f;
			float x1 = 0f;
			float x2 = 0f;
			float y1 = 0f;
			float y2 = 0f;

			if(shape != null && transform != null)
			{
				m32 = new Matrix32();
				m32.AssignSequential(transform.Parameters);
				cx = shape.mProperties.GetValue("cx");
				cy = shape.mProperties.GetValue("cy");
				rx = shape.mProperties.GetValue("rx");
				ry = shape.mProperties.GetValue("ry");
				x1 = cx - rx;
				y1 = cy - ry;
				x2 = cx + rx;
				y2 = cy + ry;
				point = m32.Transform(x1, y1);
				x1 = point.X;
				y1 = point.Y;
				point = m32.Transform(x2, y2);
				x2 = point.X;
				y2 = point.Y;
				rx = (x2 - x1) / 2f;
				ry = (y2 - y1) / 2f;
				cx = x1 + rx;
				cy = y1 + ry;
				shape.mProperties.SetValue("cx", cx);
				shape.mProperties.SetValue("cy", cy);
				shape.mProperties.SetValue("rx", rx);
				shape.mProperties.SetValue("ry", ry);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TransformMatrixLine																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Perform a matrix transformation on the provided line.
		/// </summary>
		/// <param name="shape">
		/// Reference to the rectangular shape to be transformed, having x1, y1,
		/// x2, and y2 properties.
		/// </param>
		/// <param name="transform">
		/// Reference to the transform containing the 6 values representing the
		/// member values of a 3x2 matrix.
		/// </param>
		private static void TransformMatrixLine(ShapeInfoItem shape,
			TransformItem transform)
		{
			Matrix32 m32 = null;
			FVector2 point = null;
			float x1 = 0f;
			float x2 = 0f;
			float y1 = 0f;
			float y2 = 0f;

			if(shape != null && transform != null)
			{
				m32 = new Matrix32();
				m32.AssignSequential(transform.Parameters);
				x1 = shape.mProperties.GetValue("x1");
				y1 = shape.mProperties.GetValue("y1");
				x2 = shape.mProperties.GetValue("x2");
				y2 = shape.mProperties.GetValue("y2");
				point = m32.Transform(x1, y1);
				x1 = point.X;
				y1 = point.Y;
				point = m32.Transform(x2, y2);
				x2 = point.X;
				y2 = point.Y;
				shape.mProperties.SetValue("x1", x1);
				shape.mProperties.SetValue("y1", y1);
				shape.mProperties.SetValue("x2", x2);
				shape.mProperties.SetValue("y2", y2);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TransformMatrixLocation																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Perform a matrix transformation on the provided location.
		/// </summary>
		/// <param name="shape">
		/// Reference to the rectangular shape to be transformed, having x and y
		/// properties.
		/// </param>
		/// <param name="transform">
		/// Reference to the transform containing the 6 values representing the
		/// member values of a 3x2 matrix.
		/// </param>
		private static void TransformMatrixLocation(ShapeInfoItem shape,
			TransformItem transform)
		{
			Matrix32 m32 = null;
			FVector2 point = null;
			float x = 0f;
			float y = 0f;

			if(shape != null && transform != null)
			{
				m32 = new Matrix32();
				m32.AssignSequential(transform.Parameters);
				x = shape.mProperties.GetValue("x");
				y = shape.mProperties.GetValue("y");
				point = m32.Transform(x, y);
				x = point.X;
				y = point.Y;
				shape.mProperties.SetValue("x", x);
				shape.mProperties.SetValue("y", y);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TransformMatrixPoly																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Perform a matrix transform on the points in the specified point list.
		/// </summary>
		/// <param name="shape">
		/// Reference to the shape containing the points to be transformed, each
		/// having x and y properties.
		/// </param>
		/// <param name="transform">
		/// Reference to the transform containing the 6 values representing the
		/// member values of a 3x2 matrix.
		/// </param>
		private static void TransformMatrixPoly(ShapeInfoItem shape,
			TransformItem transform)
		{
			Matrix32 m32 = null;
			FVector2 point = null;

			if(shape != null && transform != null)
			{
				m32 = new Matrix32();
				m32.AssignSequential(transform.Parameters);
				foreach(FVector2 pointItem in shape.mPoints)
				{
					point = m32.Transform(pointItem.X, pointItem.Y);
					shape.mProperties.SetValue("x", point.X);
					shape.mProperties.SetValue("y", point.Y);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TransformMatrixRadial																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Perform matrix transformation on the provided radial shape.
		/// </summary>
		/// <param name="shape">
		/// Reference to the radial shape to be transformed, having cx, cy, r,
		/// fx, and fy properties.
		/// </param>
		/// <param name="transform">
		/// Reference to the transform containing the 6 values representing the
		/// member values of a 3x2 matrix.
		/// </param>
		private static void TransformMatrixRadial(ShapeInfoItem shape,
			TransformItem transform)
		{
			float cx = 0f;
			float cy = 0f;
			float fx = 0f;
			float fy = 0f;
			Matrix32 m32 = null;
			FVector2 point = null;
			float r = 0f;
			float x1 = 0f;
			float x2 = 0f;
			float y1 = 0f;
			float y2 = 0f;

			if(shape != null && transform != null)
			{
				m32 = new Matrix32();
				m32.AssignSequential(transform.Parameters);
				fx = shape.mProperties.GetValue("fx");
				fy = shape.mProperties.GetValue("fy");
				point = m32.Transform(fx, fy);
				fx = point.X;
				fy = point.Y;
				cx = shape.mProperties.GetValue("cx");
				cy = shape.mProperties.GetValue("cy");
				r = shape.mProperties.GetValue("r");
				x1 = cx - r;
				y1 = cy - r;
				x2 = cx + r;
				y2 = cy + r;
				point = m32.Transform(x1, y1);
				x1 = point.X;
				y1 = point.Y;
				point = m32.Transform(x2, y2);
				x2 = point.X;
				y2 = point.Y;
				r = (x2 - x1) / 2f;
				cx = x1 + r;
				cy = y1 + r;
				shape.mProperties.SetValue("fx", fx);
				shape.mProperties.SetValue("fy", fy);
				shape.mProperties.SetValue("cx", cx);
				shape.mProperties.SetValue("cy", cy);
				shape.mProperties.SetValue("r", r);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TransformMatrixRect																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Perform a matrix transform on the specified rectangle shape.
		/// </summary>
		/// <param name="shape">
		/// Reference to the rectangular shape to be transformed, having x, y,
		/// width, and height properties.
		/// </param>
		/// <param name="transform">
		/// Reference to the transform containing the 6 values representing the
		/// member values of a 3x2 matrix.
		/// </param>
		private static void TransformMatrixRect(ShapeInfoItem shape,
			TransformItem transform)
		{
			Matrix32 m32 = null;
			FVector2 point = null;
			float x1 = 0f;
			float x2 = 0f;
			float y1 = 0f;
			float y2 = 0f;

			if(shape != null && transform != null)
			{
				m32 = new Matrix32();
				m32.AssignSequential(transform.Parameters);
				x1 = shape.mProperties.GetValue("x");
				y1 = shape.mProperties.GetValue("y");
				x2 = shape.mProperties.GetValue("width") + x1;
				y2 = shape.mProperties.GetValue("height") + y1;
				point = m32.Transform(x1, y1);
				x1 = point.X;
				y1 = point.Y;
				point = m32.Transform(x2, y2);
				x2 = point.X;
				y2 = point.Y;
				if(shape.mProperties.Exists(x => x.Name == "x"))
				{
					shape.mProperties.SetValue("x", x1);
				}
				if(shape.mProperties.Exists(x => x.Name == "y"))
				{
					shape.mProperties.SetValue("y", y1);
				}
				if(shape.mProperties.Exists(x => x.Name == "width"))
				{
					shape.mProperties.SetValue("width", x2 - x1);
				}
				if(shape.mProperties.Exists(x => x.Name == "height"))
				{
					shape.mProperties.SetValue("height", y2 - y1);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* TransformShape																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Transform the values in the individual shape by the provided
		/// transforms.
		/// </summary>
		/// <param name="shape">
		/// Reference to the shape whose values will be transformed.
		/// </param>
		/// <param name="transforms">
		/// Reference to the collection of transforms to apply to the shape's
		/// properties.
		/// </param>
		private static void TransformShape(ShapeInfoItem shape,
			TransformCollection transforms)
		{
			bool bChanged = false;
			Matrix32 m32 = null;
			Match match = null;
			string measure = "";
			float mult = 1f;
			string name = "";
			float number = 0f;
			FVector2 point = null;
			int pointCount = 0;
			int pointIndex = 0;
			List<PropertyFItem> properties = null;
			string style = "";
			string text = "";
			float vn = 0f;
			float vo = 0f;
			float tx = 0f;
			float ty = 0f;

			if(shape != null && transforms?.Count > 0)
			{
				//	Transform basic properties and general points.
				//	Get the original reference dimension.
				//if(shape.mNode.Attributes.Exists(x =>
				//	x.Name == "id" && x.Value == "img2"))
				//{
				//	Console.WriteLine("ShapeInfoItem.TransformShape: Break here...");
				//}
				vo = GetReferenceDimension(shape);
				foreach(TransformItem transformItem in transforms)
				{
					if(transformItem.Parameters.Count > 0)
					{
						switch(transformItem.TransformType)
						{
							case TransformTypeEnum.Matrix:
								switch(shape.mShapeType)
								{
									case ShapeInfoTypeEnum.Circle:
										TransformMatrixCircle(shape, transformItem);
										break;
									case ShapeInfoTypeEnum.Ellipse:
										TransformMatrixEllipse(shape, transformItem);
										break;
									case ShapeInfoTypeEnum.Line:
										TransformMatrixLine(shape, transformItem);
										break;
									case ShapeInfoTypeEnum.Location:
										TransformMatrixLocation(shape, transformItem);
										break;
									case ShapeInfoTypeEnum.Poly:
										TransformMatrixPoly(shape, transformItem);
										break;
									case ShapeInfoTypeEnum.Radial:
										TransformMatrixRadial(shape, transformItem);
										break;
									case ShapeInfoTypeEnum.Rect:
										TransformMatrixRect(shape, transformItem);
										break;
								}
								break;
							case TransformTypeEnum.Rotate:
								break;
							case TransformTypeEnum.Scale:
								//	Base properties.
								tx = transformItem.Parameters[0];
								ty = 1f;
								if(transformItem.Parameters.Count > 1)
								{
									ty = transformItem.Parameters[1];
								}
								properties =
									shape.mProperties.FindAll(x =>
										mLocationProperties.Contains(x.Name) ||
										mDimensionalProperties.Contains(x.Name));
								foreach(PropertyFItem propertyItem in properties)
								{
									if(mHorizontalProperties.Contains(propertyItem.Name))
									{
										//	This is a horizontal value.
										propertyItem.Value *= tx;
									}
									if(mVerticalProperties.Contains(propertyItem.Name) &&
										transformItem.Parameters.Count > 1)
									{
										//	This is a vertical value.
										propertyItem.Value *= ty;
									}
								}
								//	Transform point list.
								pointCount = shape.mPoints.Count;
								for(pointIndex = 0; pointIndex < pointCount; pointIndex++)
								{
									point = shape.mPoints[pointIndex];
									point.X *= tx;
									point.Y *= tx;
									shape.mPoints[pointIndex] = point;
								}
								break;
							case TransformTypeEnum.SkewX:
								break;
							case TransformTypeEnum.SkewY:
								break;
							case TransformTypeEnum.Translate:
								//	Base properties.
								tx = transformItem.Parameters[0];
								ty = 0f;
								if(transformItem.Parameters.Count > 1)
								{
									ty = transformItem.Parameters[1];
								}
								properties =
									shape.mProperties.FindAll(x =>
										mLocationProperties.Contains(x.Name));
								foreach(PropertyFItem propertyItem in properties)
								{
									if(mHorizontalProperties.Contains(propertyItem.Name))
									{
										//	This is a horizontal location.
										propertyItem.Value += tx;
									}
									if(mVerticalProperties.Contains(propertyItem.Name) &&
										transformItem.Parameters.Count > 1)
									{
										//	This is a vertical location.
										propertyItem.Value += ty;
									}
								}
								//	Transform point list.
								pointCount = shape.mPoints.Count;
								for(pointIndex = 0; pointIndex < pointCount; pointIndex++)
								{
									point = shape.mPoints[pointIndex];
									point.X += tx;
									point.Y += tx;
									shape.mPoints[pointIndex] = point;
								}
								break;
						}
					}
				}
				//	Transform plot points.
				if(shape.mPlotPoints.Count > 0)
				{
					//	Convert all plot points to absolute values.
					PlotPointsFCollection.ConvertToAbsolute(shape.mPlotPoints);
					PlotPointsFCollection.Transform(shape.mPlotPoints, transforms);
				}
				//	Transform secondary values.
				vn = GetReferenceDimension(shape);
				//if(HtmlNodeItem.GetId(shape.mNode) == "text3477")
				//{
				//	Console.WriteLine("TransformShape. Break here...");
				//}
				if(shape.mNode != null &&
					(vo != vn || shape.mNode.NodeType == "text"))
				{
					if(vo != vn)
					{
						//	The reference dimension has changed.
						mult = (vo != 0f ? vn / vo : 0f);
						if(shape.mNode.Attributes.Exists(x => x.Name == "style"))
						{
							//	Style attribute is present on the shape.
							style = GetAttributeValue(shape.mNode, "style");
							//	Stroke width.
							name = "stroke-width";
							if(style.IndexOf(name) > -1)
							{
								//	Stroke width is present.
								shape.mNode.Attributes.SetStyle(name, (ToFloat(
									shape.mNode.Attributes.GetStyle(name)) * mult).ToString());
							}
						}
					}
					else if(shape.mNode.NodeType == "text" &&
						shape.mNode.Attributes.Exists(x => x.Name == "style") &&
						transforms.Exists(x =>
							x.TransformType == TransformTypeEnum.Matrix ||
							x.TransformType == TransformTypeEnum.Scale))
					{
						bChanged = false;
						style = GetAttributeValue(shape.mNode, "style");
						//	Font size.
						name = "font-size";
						if(style.IndexOf(name) > -1)
						{
							//	Font size is present.
							text = shape.mNode.Attributes.GetStyle(name);
							match = Regex.Match(text, ResourceMain.rxCssNumberWithMeasure);
							if(match.Success)
							{
								number = ToFloat(GetValue(match, "number"));
								measure = GetValue(match, "measure");
								if(measure == "vh" ||
									measure == "vw" ||
									measure == "%")
								{
									//	Only a specific measure gets converted with
									//	transformation.
									number = 0f;
								}
							}
							if(number != 0f)
							{
								//	In the case of text, the font size can be changed on
								//	Scale and Matrix transformations.
								foreach(TransformItem transformItem in transforms)
								{
									//	Transforms are already ordered in the reverse order for
									//	use in a loop.
									switch(transformItem.TransformType)
									{
										case TransformTypeEnum.Matrix:
											tx = ty = number;
											m32 = new Matrix32();
											m32.AssignSequential(transformItem.Parameters);
											//	Remove all translation, leaving only scale.
											m32.Values[4] = 0f;
											m32.Values[5] = 0f;
											point = m32.Transform(tx, ty);
											mult = ((point.X + point.Y) / 2f) / number;
											number *= mult;
											bChanged = true;
											break;
										case TransformTypeEnum.Scale:
											tx = ty = number;
											if(transformItem.Parameters.Count > 0)
											{
												tx = number * transformItem.Parameters[0];
											}
											if(transformItem.Parameters.Count > 1)
											{
												ty = number * transformItem.Parameters[1];
											}
											mult = ((tx + ty) / 2f) / number;
											number *= mult;
											bChanged = true;
											break;
									}
								}
							}
							//	After transformation, store the value back into the
							//	style.
							if(number != 0f && bChanged)
							{
								shape.mNode.Attributes.SetStyle(name, $"{number}{measure}");
								ScaleStyle(shape.mNode.Nodes, "font-size", mult);
							}
						}
					}
				}
				//	Apply downward.
				foreach(ShapeInfoItem shapeItem in shape.mShapes)
				{
					TransformShape(shapeItem, transforms);
				}
			}
		}
		////*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		///// <summary>
		///// Transform the specified shape named by the caller using the transforms
		///// of the existing shape and its ancestors.
		///// </summary>
		///// <param name="targetShapeName">
		///// Name of the shape to transform.
		///// </param>
		///// <param name="refShape">
		///// Reference to the shape whose transforms will be used.
		///// </param>
		//private static void TransformShape(string targetShapeName,
		//	ShapeInfoItem refShape)
		//{
		//	ShapeInfoItem shape = null;

		//	if(targetShapeName?.Length > 0 && refShape != null)
		//	{
		//		//	The object name was found.
		//		shape = ShapeInfoItem.GetRoot(refShape);
		//		if(shape != null)
		//		{
		//			//	The root object was found.
		//			shape = ShapeInfoItem.FindShape(shape,
		//				x => HtmlNodeItem.GetId(x.mNode) == targetShapeName);
		//			if(shape != null)
		//			{
		//				//	The remote item was found.
		//				if(shape.mNode != null)
		//				{
		//					//	Transform the values in the remote item from
		//					//	the local transforms.
		//					TransformShape(shape, refShape.mTransforms);
		//				}
		//			}
		//		}
		//	}
		//}
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
		/// Create a new instance of the ShapeInfoItem Item.
		/// </summary>
		public ShapeInfoItem()
		{
			mShapes = new ShapeInfoCollection();
			mShapes.Parent = this;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ApplyTransforms																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Back-transform the supplied object and all of its dependend objects.
		/// </summary>
		/// <param name="shape">
		/// Reference to the shape information object at which to begin
		/// back-transformation.
		/// </param>
		/// <remarks>
		/// <para>There is a universal bug in SVG for the 'use' element where
		/// although the local x and y attributes are observed, the local
		/// width and height attributes of the same element are completely
		/// ignored. As a result, any non-transform change in the output size
		/// of the resource must be done at the resource.</para>
		/// <para>Unfortunately, the SVG scale function also has an effect on the
		/// object's position. When using the scale function, remember that
		/// the SVG method of scaling is to adjust each internal point's relative
		/// position according to the scaling factor, instead of the object's
		/// dimensions.</para>
		/// </remarks>
		public static void ApplyTransforms(ShapeInfoItem shape)
		{
			if(shape != null)
			{
				//	Transform the values in this item from the local transforms.
				//	Apply outermost transforms first, working inward toward root.
				foreach(ShapeInfoItem shapeItem in shape.mShapes)
				{
					ApplyTransforms(shapeItem);
				}
				TransformShape(shape, shape.mTransforms);
			}
		}
		////*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		///// <summary>
		///// Apply all reference transformations in the hierarchy to the target
		///// shape and its descendent structure.
		///// </summary>
		///// <param name="targetShape">
		///// Reference to the shape structure to be updated.
		///// </param>
		///// <param name="refShape">
		///// Reference to the transform hierarchy to be applied.
		///// </param>
		//public static void ApplyTransforms(ShapeInfoItem targetShape,
		//	ShapeInfoItem refShape)
		//{
		//	if(targetShape != null && refShape != null)
		//	{
		//		//	Apply outermost transforms first, working inward toward root.
		//		TransformShape(targetShape, refShape.mTransforms);
		//	}
		//}
		////*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		///// <summary>
		///// Apply all transformations to the specified shape and its descendent
		///// structure, using the transform hierarchy of the provided reference
		///// shape.
		///// </summary>
		///// <param name="targetShapeName">
		///// Name of the shape at which to start appling transforms.
		///// </param>
		///// <param name="refShape">
		///// Reference shape from which all of the transforms will be applied.
		///// </param>
		//public static void ApplyTransforms(string targetShapeName,
		//	ShapeInfoItem refShape)
		//{
		//	ShapeInfoItem shape = null;

		//	if(targetShapeName?.Length > 0 && refShape != null)
		//	{
		//		//	The object name was found.
		//		shape = ShapeInfoItem.GetRoot(refShape);
		//		if(shape != null)
		//		{
		//			//	The root object was found.
		//			shape = ShapeInfoItem.FindShape(shape,
		//				x => HtmlNodeItem.GetId(x.mNode) == targetShapeName);
		//			if(shape != null)
		//			{
		//				//	The remote item was found.
		//				ApplyTransforms(shape, refShape);
		//			}
		//		}
		//	}
		//}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	CreateShapes																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Assign shape information to the specified node and all of its children.
		/// </summary>
		/// <param name="node">
		/// Reference to the HTML node to activate.
		/// </param>
		public static ShapeInfoItem CreateShapes(HtmlNodeItem node)
		{
			HtmlAttributeItem attrib = null;
			bool bFound = false;
			string[] propertyNames = Array.Empty<string>();
			ShapeInfoItem shape = null;
			List<string> texts = new List<string>();

			if(node != null && node.NodeType.Length > 0)
			{
				shape = new ShapeInfoItem();
				//if(HtmlNodeItem.GetId(node) == "btnSepTools")
				//{
				//	Console.WriteLine("CreateShapes. Break here...");
				//}
				shape.mNode = node;
				//	Transform is available on any of 16 node types.
				switch(node.NodeType)
				{
					case "a":
					case "circle":
					case "clippath":
					case "defs":
					case "ellipse":
					case "foreignObject":
					case "g":
					case "image":
					case "line":
					case "path":
					case "polygon":
					case "polyline":
					case "rect":
					case "switch":
					case "text":
					case "use":
						attrib = node.Attributes.FirstOrDefault(x =>
							x.Name == "transform");
						if(attrib != null)
						{
							shape.mTransforms = TransformCollection.Parse(attrib.Value);
						}
						break;
					case "linearGradient":
					case "radialGradient":
						attrib = node.Attributes.FirstOrDefault(x =>
							x.Name == "gradientTransform");
						if(attrib != null)
						{
							shape.mTransforms = TransformCollection.Parse(attrib.Value);
						}
						break;
				}
				bFound = true;
				switch(node.NodeType)
				{
					case "a":
					case "clippath":
					case "defs":
					case "g":
					case "switch":
						//	No properties except transform.
						shape.mShapeType = ShapeInfoTypeEnum.None;
						break;
					case "circle":
						//	Properties: cx, cy, r.
						shape.mShapeType = ShapeInfoTypeEnum.Circle;
						propertyNames = new string[] { "cx", "cy", "r" };
						break;
					case "ellipse":
						//	Properties: cx, cy, rx, ry.
						shape.mShapeType = ShapeInfoTypeEnum.Ellipse;
						propertyNames = new string[] { "cx", "cy", "rx", "ry" };
						break;
					case "foreignObject":
					case "image":
					case "pattern":
					case "rect":
					case "use":
						//	Properties: x, y, width, height.
						shape.mShapeType = ShapeInfoTypeEnum.Rect;
						propertyNames = new string[] { "x", "y", "width", "height" };
						break;
					case "line":
					case "linearGradient":
						//	Properties: x1, y1, x2, y2.
						shape.mShapeType = ShapeInfoTypeEnum.Line;
						propertyNames = new string[] { "x1", "y1", "x2", "y2" };
						break;
					case "path":
						//	Properties: d.
						shape.mShapeType = ShapeInfoTypeEnum.Path;
						propertyNames = new string[] { "d" };
						break;
					case "polygon":
					case "polyline":
						//	Properties: points.
						shape.mShapeType = ShapeInfoTypeEnum.Poly;
						propertyNames = new string[] { "points" };
						break;
					case "radialGradient":
						//	Properties: cx, cy, r, fx, fy.
						shape.mShapeType = ShapeInfoTypeEnum.Radial;
						propertyNames = new string[] { "cx", "cy", "r", "fx", "fy" };
						break;
					case "text":
						//	Properties: x, y.
						//	Special case. Text parameters are only used when x and y are
						//	present, and tspan is not contained.
						if(node.Attributes.Exists(x => x.Name.ToLower() == "x") &&
							node.Attributes.Exists(y => y.Name.ToLower() == "y") &&
							node.Nodes.Count == 0)
						{
							shape.mShapeType = ShapeInfoTypeEnum.Location;
							propertyNames = new string[] { "x", "y" };
						}
						else
						{
							//	Any transform in this item can still be referenced.
							shape.mShapeType = ShapeInfoTypeEnum.None;
							propertyNames = Array.Empty<string>();
						}
						break;
					case "tspan":
						//	Properties: x, y.
						//	Special case. TSpan parameters are only used when x or y
						//	are present.
						texts.Clear();
						if(node.Attributes.Exists(x => x.Name.ToLower() == "x"))
						{
							texts.Add("x");
						}
						if(node.Attributes.Exists(y => y.Name.ToLower() == "y"))
						{
							texts.Add("y");
						}
						if(texts.Count > 0)
						{
							shape.mShapeType = ShapeInfoTypeEnum.Location;
							propertyNames = texts.ToArray();
						}
						else
						{
							shape.mShapeType = ShapeInfoTypeEnum.None;
							propertyNames = Array.Empty<string>();
						}
						break;
					default:
						bFound = false;
						break;
				}
				//	If properties were found, then assign them.
				if(bFound && propertyNames.Length > 0)
				{
					//if(node.Attributes.Exists(x =>
					//	x.Name == "id" && x.Value == "img2"))
					//{
					//	Console.WriteLine("ShapeInfoItem.CreateShapes: Break here...");
					//}
					AssignProperties(node, shape, propertyNames);
				}
				//	Drill down.
				if(node.Nodes.Count > 0)
				{
					shape.mShapes = ShapeInfoCollection.CreateShapes(node.Nodes);
					shape.mShapes.Parent = shape;
				}
			}
			return shape;
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	CenterX																																*
		////*-----------------------------------------------------------------------*
		//private float mCenterX = 0f;
		///// <summary>
		///// Get/Set the center X coordinate.
		///// </summary>
		//public float CenterX
		//{
		//	get { return mCenterX; }
		//	set { mCenterX = value; }
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	CenterY																																*
		////*-----------------------------------------------------------------------*
		//private float mCenterY = 0f;
		///// <summary>
		///// Get/Set the center Y coordinate.
		///// </summary>
		//public float CenterY
		//{
		//	get { return mCenterY; }
		//	set { mCenterY = value; }
		//}
		////*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FindGlobal																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a reference to the first shape in the tree with the specified
		/// ID.
		/// </summary>
		/// <param name="startShape">
		/// Arbitrary shape from which the search will start.
		/// </param>
		/// <param name="id">
		/// Shape ID to search for.
		/// </param>
		/// <returns>
		/// Reference to the ShapeInfoItem with the specified ID, if found.
		/// Otherwise, null;
		/// </returns>
		public static ShapeInfoItem FindGlobal(ShapeInfoItem startShape, string id)
		{
			ShapeInfoItem current = null;
			ShapeInfoItem result = null;

			if(startShape != null)
			{
				current = GetRoot(startShape);
				if(id.StartsWith("#"))
				{
					result = FindShape(current,
						x => x.mNode.Attributes.Exists(y =>
						y.Name == "id" && y.Value == id.Substring(1, id.Length - 1)));
				}
				else
				{
					//	TODO: ShapeInfoItem. Load shape from the Internet.
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	Height																																*
		////*-----------------------------------------------------------------------*
		//private float mHeight = 0f;
		///// <summary>
		///// Get/Set the height of the object.
		///// </summary>
		//public float Height
		//{
		//	get { return mHeight; }
		//	set { mHeight = value; }
		//}
		////*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Node																																	*
		//*-----------------------------------------------------------------------*
		private HtmlNodeItem mNode = null;
		/// <summary>
		/// Get/Set a reference to the HTML node backing this shape.
		/// </summary>
		public HtmlNodeItem Node
		{
			get { return mNode; }
			set { mNode = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Parent																																*
		//*-----------------------------------------------------------------------*
		private ShapeInfoCollection mParent = null;
		/// <summary>
		/// Get/Set a reference to the parent collection of this shape.
		/// </summary>
		public ShapeInfoCollection Parent
		{
			get { return mParent; }
			set { mParent = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	PlotPoints																														*
		//*-----------------------------------------------------------------------*
		private PlotPointsFCollection mPlotPoints = new PlotPointsFCollection();
		/// <summary>
		/// Get a reference to the plot points associated with this shape.
		/// </summary>
		public PlotPointsFCollection PlotPoints
		{
			get { return mPlotPoints; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Points																																*
		//*-----------------------------------------------------------------------*
		private List<FVector2> mPoints = new List<FVector2>();
		/// <summary>
		/// Get a reference to the list of points used for line-drawing.
		/// </summary>
		public List<FVector2> Points
		{
			get { return mPoints; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Properties																														*
		//*-----------------------------------------------------------------------*
		private PropertyFCollection mProperties = new PropertyFCollection();
		/// <summary>
		/// Get a reference to the collection of floating point property values
		/// assigned to this shape.
		/// </summary>
		public PropertyFCollection Properties
		{
			get { return mProperties; }
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	Radius																																*
		////*-----------------------------------------------------------------------*
		//private float mRadius = 0f;
		///// <summary>
		///// Get/Set the radius of the object.
		///// </summary>
		//public float Radius
		//{
		//	get { return mRadius; }
		//	set { mRadius = value; }
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	RadiusX																																*
		////*-----------------------------------------------------------------------*
		//private float mRadiusX = 0f;
		///// <summary>
		///// Get/Set the X radius of the object.
		///// </summary>
		//public float RadiusX
		//{
		//	get { return mRadiusX; }
		//	set { mRadiusX = value; }
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	RadiusY																																*
		////*-----------------------------------------------------------------------*
		//private float mRadiusY = 0f;
		///// <summary>
		///// Get/Set the Y radius of the object.
		///// </summary>
		//public float RadiusY
		//{
		//	get { return mRadiusY; }
		//	set { mRadiusY = value; }
		//}
		////*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RemoveTransforms																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Remove the applicable transforms from this node and all children.
		/// </summary>
		/// <param name="shape">
		/// Reference to the shape information item from which the consumed
		/// transforms will be removed.
		/// </param>
		public static void RemoveTransforms(ShapeInfoItem shape)
		{
			int count = 0;
			int index = 0;
			TransformItem transform = null;

			if(shape != null)
			{
				count = shape.mTransforms.Count;
				for(index = 0; index < count; index++)
				{
					transform = shape.mTransforms[index];
					switch(transform.TransformType)
					{
						case TransformTypeEnum.Matrix:
						case TransformTypeEnum.Scale:
						case TransformTypeEnum.Translate:
							shape.mTransforms.RemoveAt(index);
							index--;
							count--;
							break;
					}
				}
				//	Drill down.
				if(shape.mShapes.Count > 0)
				{
					ShapeInfoCollection.RemoveTransforms(shape.mShapes);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Shapes																																*
		//*-----------------------------------------------------------------------*
		private ShapeInfoCollection mShapes = null;
		/// <summary>
		/// Get/Set a reference to the collection of shapes contained by this item.
		/// </summary>
		public ShapeInfoCollection Shapes
		{
			get { return mShapes; }
			set { mShapes = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShapeType																															*
		//*-----------------------------------------------------------------------*
		private ShapeInfoTypeEnum mShapeType = ShapeInfoTypeEnum.None;
		/// <summary>
		/// Get/Set the type of shape represented by this instance.
		/// </summary>
		public ShapeInfoTypeEnum ShapeType
		{
			get { return mShapeType; }
			set { mShapeType = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Transforms																														*
		//*-----------------------------------------------------------------------*
		private TransformCollection mTransforms = new TransformCollection();
		/// <summary>
		/// Get a reference to the collection of transforms defined on this item.
		/// </summary>
		public TransformCollection Transforms
		{
			get { return mTransforms; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* UpdateNodes																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Update all associated HTML node attributes from property values found
		/// in this shape.
		/// </summary>
		/// <param name="shape">
		/// Reference to the shape information item associated with an HTML
		/// node or has children associated with HTML nodes.
		/// </param>
		public static void UpdateNodes(ShapeInfoItem shape)
		{
			float angle = 0f;
			float angleResult = 0f;
			FArea box = null;
			//StringBuilder builder = new StringBuilder();
			HtmlAttributeCollection coords = null;
			float cx = 0f;
			float cy = 0f;
			float dx = 0f;
			float dxl = 0f;
			float dy = 0f;
			float dyl = 0f;
			HtmlNodeItem dependentNode = null;
			ShapeInfoItem dependentShape = null;
			float height = 0f;
			string id = "";
			List<FVector2> intersections = null;
			float length = 0f;
			FLine line = null;
			HtmlNodeItem node = null;
			string text = "";
			float width = 0f;
			float x1 = 0f;
			float x2 = 0f;
			float y1 = 0f;
			float y2 = 0f;

			if(shape != null)
			{
				//	Any kind of shape can have a "transform" property.
				if(shape.mNode != null)
				{
					node = shape.mNode;
					//if(node.Attributes.Exists(x => x.Name == "id" &&
					//	x.Value == "rect912"))
					//{
					//	Console.WriteLine("ShapeInfoItem.UpdateNodes: Break here...");
					//}
					if(node.Attributes.Exists(x => x.Name == "transform"))
					{
						text = shape.mTransforms.ToString();
						if(text.Length > 0)
						{
							node.Attributes.SetAttribute("transform", text);
						}
						else
						{
							node.Attributes.RemoveAll(x => x.Name == "transform");
						}
					}
					else if(node.Attributes.Exists(x =>
						x.Name == "gradientTransform"))
					{
						text = shape.mTransforms.ToString();
						if(text.Length > 0)
						{
							node.Attributes.SetAttribute("gradientTransform", text);
						}
						else
						{
							node.Attributes.RemoveAll(x =>
								x.Name == "gradientTransform");
						}
					}
				}
				if(node != null)
				{
					id = node.Id;
					//	Gradients are a special case.
					//	In this era, they are single-use objects completely dependent
					//	upon their dependent object. Their coordinates will be updated
					//	to correspond with the dependent object's settings, in relation
					//	to the gradient travel.
					switch(node.NodeType)
					{
						case "linearGradient":
							x1 = ToFloat(node.Attributes["x1"].Value);
							y1 = ToFloat(node.Attributes["y1"].Value);
							x2 = ToFloat(node.Attributes["x2"].Value);
							y2 = ToFloat(node.Attributes["y2"].Value);
							angle = Geometry.Trig.GetLineAngle(x1, y1, x2, y2);
							dependentNode = HtmlNodeItem.GetRoot(node);
							if(dependentNode != null)
							{
								//	Retrieve the first dependent.
								dependentNode = dependentNode.Nodes.FindMatch(x =>
									x.Attributes["style"].Value?.IndexOf($"url(#{id})") > -1 &&
									!x.NodeType.EndsWith("Gradient") &&
									!HtmlNodeItem.HasAncestorNodeType(x, "defs"));
								if(dependentNode != null)
								{
									dependentShape = ShapeInfoItem.GetRoot(shape);
									if(dependentShape != null)
									{
										dependentShape =
											ShapeInfoItem.FindShape(dependentShape,
												x => x.Node?.Id == dependentNode.Id);
									}
								}
								if(dependentShape != null)
								{
									coords = GetBoundingBoxX1Y1X2Y2(dependentShape);
									if(coords?.Count > 0)
									{
										x1 = ToFloat(coords["x1"].Value);
										y1 = ToFloat(coords["y1"].Value);
										x2 = ToFloat(coords["x2"].Value);
										y2 = ToFloat(coords["y2"].Value);
										width = x2 - x1;
										height = y2 - y1;
										cx = (x1 + x2) / 2f;
										cy = (y1 + y2) / 2f;
										dx = (float)Math.Cos((double)angle);
										dy = (float)Math.Sin((double)angle);
										length = Trig.GetLineHypFromAdjOpp(width, height);
										dxl = dx * length;
										dyl = dy * length;
										line = new FLine(cx - dxl, cy - dyl, cx + dxl, cy + dyl);
										box = new FArea(x1, y1, width, height);
										intersections = FArea.GetIntersections(box, line);
										if(intersections.Count == 2)
										{
											x1 = intersections[0].X;
											y1 = intersections[0].Y;
											x2 = intersections[1].X;
											y2 = intersections[1].Y;
											angleResult = Trig.GetLineAngle(x1, y1, x2, y2);
											if(Math.Abs(angleResult - angle) > GeometryUtil.HalfPi)
											{
												x1 = intersections[1].X;
												y1 = intersections[1].Y;
												x2 = intersections[0].X;
												y2 = intersections[0].Y;
											}
										}
										node.Attributes.SetAttribute("x1", x1.ToString());
										node.Attributes.SetAttribute("y1", y1.ToString());
										node.Attributes.SetAttribute("x2", x2.ToString());
										node.Attributes.SetAttribute("y2", y2.ToString());
									}
								}
							}
							break;
						case "radialGradient":
							//	TODO: Update the positioning of the radial gradient.
							break;
						default:
							switch(shape.mShapeType)
							{
								case ShapeInfoTypeEnum.Circle:
								//	cx, cy, r.
								case ShapeInfoTypeEnum.Ellipse:
								//	cx, cy, rx, ry.
								case ShapeInfoTypeEnum.Line:
								//	x1, y1, x2, y2.
								case ShapeInfoTypeEnum.Location:
								//	Any kind of node with x and y values.
								case ShapeInfoTypeEnum.Radial:
								//	cx, cy, r, fx, fy.
								case ShapeInfoTypeEnum.Rect:
									//	x, y, width, height.
									foreach(PropertyFItem propertyItem in shape.mProperties)
									{
										if(propertyItem.Changed)
										{
											shape.mNode.Attributes.SetAttribute(
												propertyItem.Name,
												propertyItem.Value.ToString());
										}
									}
									break;
								case ShapeInfoTypeEnum.Path:
									//	d.
									shape.mNode.Attributes.SetAttribute("d",
										shape.mPlotPoints.ToString());
									break;
								case ShapeInfoTypeEnum.Poly:
									//	points.
									//Clear(builder);
									//foreach(FVector2 pointItem in shape.mPoints)
									//{
									//	if(builder.Length > 0)
									//	{
									//		builder.Append(' ');
									//	}
									//	builder.Append(pointItem.X);
									//	builder.Append(',');
									//	builder.Append(pointItem.Y);
									//}
									//if(builder.Length > 0)
									//{
									//	shape.mNode.Attributes.SetAttribute("points",
									//		builder.ToString());
									//}
									shape.mNode.Attributes.SetAttribute("points",
										GetPolyPointsPath(shape.mPoints));
									break;
							}
							break;
					}
				}
				//	Drill down.
				ShapeInfoCollection.UpdateNodes(shape.Shapes);
			}
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	Width																																	*
		////*-----------------------------------------------------------------------*
		//private float mWidth = 0f;
		///// <summary>
		///// Get/Set the width of the object.
		///// </summary>
		//public float Width
		//{
		//	get { return mWidth; }
		//	set { mWidth = value; }
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	X																																			*
		////*-----------------------------------------------------------------------*
		//private float mX = 0f;
		///// <summary>
		///// Get/Set the X coordinate.
		///// </summary>
		//public float X
		//{
		//	get { return mX; }
		//	set { mX = value; }
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	X1																																		*
		////*-----------------------------------------------------------------------*
		//private float mX1 = 0f;
		///// <summary>
		///// Get/Set the x1 coordinate.
		///// </summary>
		//public float X1
		//{
		//	get { return mX1; }
		//	set { mX1 = value; }
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	X2																																		*
		////*-----------------------------------------------------------------------*
		//private float mX2 = 0f;
		///// <summary>
		///// Get/Set the x2 coordinate.
		///// </summary>
		//public float X2
		//{
		//	get { return mX2; }
		//	set { mX2 = value; }
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	Y																																			*
		////*-----------------------------------------------------------------------*
		//private float mY = 0f;
		///// <summary>
		///// Get/Set the Y coordinate.
		///// </summary>
		//public float Y
		//{
		//	get { return mY; }
		//	set { mY = value; }
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	Y1																																		*
		////*-----------------------------------------------------------------------*
		//private float mY1 = 0f;
		///// <summary>
		///// Get/Set the y1 coordinate.
		///// </summary>
		//public float Y1
		//{
		//	get { return mY1; }
		//	set { mY1 = value; }
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	Y2																																		*
		////*-----------------------------------------------------------------------*
		//private float mY2 = 0f;
		///// <summary>
		///// Get/Set the y2 coordinate.
		///// </summary>
		//public float Y2
		//{
		//	get { return mY2; }
		//	set { mY2 = value; }
		//}
		////*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
