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
using System.Text.RegularExpressions;
using static SvgToolsLibrary.SvgToolsUtil;

namespace SvgToolsLibrary
{
	//*-------------------------------------------------------------------------*
	//*	TransformCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of TransformItem Items.
	/// </summary>
	public class TransformCollection : List<TransformItem>
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
		//	When translating X or Y, TranslatePoint(Left, Top) should be used.
		//	When translating Width or Height,
		//	 TranslatePoint(Right, Bottom) - Location should be used.
		//	When translating X2 or Y2, TranslatePoint(X2, Y2) should be used.
		//	When translating CX or CY, TranslatePoint(CX, CY) should be used.
		//	TODO: Get all values from the current to highest levels.

		////*-----------------------------------------------------------------------*
		////* ConvertShapeToGlobal																									*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Convert the local shape coordinates to a global value.
		///// </summary>
		///// <param name="transforms">
		///// Reference to the local collection of transforms for the current level.
		///// </param>
		///// <param name="shape">
		///// Reference to information about the shape to back transform.
		///// </param>
		//public static void ConvertShapeToGlobal(TransformCollection transforms,
		//	ShapeInfo shape)
		//{
		//	if(transforms?.Count > 0 && shape != null)
		//	{

		//	}
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////* GetGlobalHeight																												*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Return the fully translated global version of the caller's local
		///// height.
		///// </summary>
		///// <param name="transforms">
		///// Reference to the collection of transforms configured for the current
		///// level.
		///// </param>
		///// <param name="localShape">
		///// The shape containing the height for which the global value will be calculated.
		///// </param>
		///// <returns>
		///// The global representation of the local value.
		///// </returns>
		//public static float GetGlobalHeight(TransformCollection transforms,
		//	float localHeight)
		//{
		//	//	By default, the local value is assumed to be final.
		//	float result = localHeight;

		//	if(transforms?.Count > 0 && Math.Abs(localHeight) > float.Epsilon)
		//	{
		//		//	Height is affected by matrix, scale, and skew Y.
		//		foreach(TransformItem transformItem in transforms)
		//		{
		//			switch(transformItem.TransformType)
		//			{
		//				case TransformTypeEnum.Matrix:
		//					break;
		//				case TransformTypeEnum.Scale:
		//					//	Scale: 0 - Width, 1 - Height.
		//					if(transformItem.Parameters.Count > 1)
		//					{
		//						result *= transformItem.Parameters[1];
		//					}
		//					break;
		//				case TransformTypeEnum.SkewY:
		//					//	When the object is skewed by a Y rotation, the X coordinates
		//					//	remain fixed but the Y coordinates change to meet the center
		//					//	ray of the object, adjusted by the specified angle.
		//					//	The angle specified in Y skew represents the angle of a right
		//					//	triangle. The adjacent side is the width of the shape / 2.
		//					//	The opposite side of that triangle represents
		//					//	1/2 of the additional height added to the shape.
		//					break;
		//			}
		//		}
		//	}
		//	if(transforms.mParentSet != null)
		//	{
		//		result = GetGlobalHeight(transforms.mParentSet, result);
		//	}
		//	return result;
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////* GetGlobalRotation																											*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Return the fully translated global version of the caller's local
		///// rotation.
		///// </summary>
		///// <param name="transforms">
		///// Reference to the collection of transforms configured for the current
		///// level.
		///// </param>
		///// <param name="localRotation">
		///// The rotation for which the global value will be calculated.
		///// </param>
		///// <returns>
		///// The global representation of the local value.
		///// </returns>
		//public static float GetGlobalRotation(TransformCollection transforms,
		//	float localRotation)
		//{
		//	//	By default, the local value is assumed to be final.
		//	float result = localRotation;

		//	if(transforms?.Count > 0 && Math.Abs(localRotation) > float.Epsilon)
		//	{
		//		//	Rotation is affected by matrix and rotation.
		//		foreach(TransformItem transformItem in transforms)
		//		{
		//			switch(transformItem.TransformType)
		//			{
		//				case TransformTypeEnum.Matrix:
		//					break;
		//				case TransformTypeEnum.Rotate:
		//					if(transformItem.Parameters.Count > 0)
		//					{
		//						result = localRotation + transformItem.Parameters[0];
		//					}
		//					break;
		//			}
		//		}
		//		if(transforms.mParentSet != null)
		//		{
		//			result = GetGlobalRotation(transforms.mParentSet, result);
		//		}
		//	}
		//	return result;
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////* GetGlobalWidth																												*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Return the fully translated global version of the caller's local width.
		///// </summary>
		///// <param name="transforms">
		///// Reference to the collection of transforms configured for the current
		///// level.
		///// </param>
		///// <param name="localWidth">
		///// The local width for which the global value will be found.
		///// </param>
		///// <returns>
		///// The global representation of the local value.
		///// </returns>
		//public static float GetGlobalWidth(TransformCollection transforms,
		//	float localWidth)
		//{
		//	//	By default, the local value is assumed to be final.
		//	float result = localWidth;

		//	if(transforms?.Count > 0 && Math.Abs(localWidth) > float.Epsilon)
		//	{
		//		//	Width is affected by matrix, scale, and skew X.
		//		foreach(TransformItem transformItem in transforms)
		//		{
		//			switch(transformItem.TransformType)
		//			{
		//				case TransformTypeEnum.Matrix:
		//					break;
		//				case TransformTypeEnum.Scale:
		//					break;
		//				case TransformTypeEnum.SkewX:
		//					break;
		//			}
		//		}
		//	}
		//	return result;
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////* GetGlobalX																														*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Return the fully translated global version of the caller's local X
		///// coordinate.
		///// </summary>
		///// <param name="transforms">
		///// Reference to the collection of transforms configured for the current
		///// level.
		///// </param>
		///// <param name="localX">
		///// The local X coordinate for which the global value will be found.
		///// </param>
		///// <returns>
		///// The global representation of the local value.
		///// </returns>
		//public static float GetGlobalX(TransformCollection transforms,
		//	float localX)
		//{
		//	//	By default, the local value is assumed to be final.
		//	float result = localX;

		//	if(transforms?.Count > 0 && Math.Abs(localX) > float.Epsilon)
		//	{
		//		//	X is affected by matrix, scale, and translation.
		//		foreach(TransformItem transformItem in transforms)
		//		{
		//			switch(transformItem.TransformType)
		//			{
		//				case TransformTypeEnum.Matrix:
		//					break;
		//				case TransformTypeEnum.Scale:
		//					break;
		//				case TransformTypeEnum.Translate:
		//					break;
		//			}
		//		}
		//	}
		//	return result;
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////* GetGlobalY																														*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Return the fully translated global version of the caller's local Y
		///// coordinate.
		///// </summary>
		///// <param name="transforms">
		///// Reference to the collection of transforms configured for the current
		///// level.
		///// </param>
		///// <param name="localY">
		///// The local Y coordinate for which the global value will be found.
		///// </param>
		///// <returns>
		///// The global representation of the local value.
		///// </returns>
		//public static float GetGlobalY(TransformCollection transforms,
		//	float localY)
		//{
		//	//	By default, the local value is assumed to be final.
		//	float result = localY;

		//	if(transforms?.Count > 0 && Math.Abs(localY) > float.Epsilon)
		//	{
		//		//	Y is affected by matrix, scale, and translation.
		//		foreach(TransformItem transformItem in transforms)
		//		{
		//			switch(transformItem.TransformType)
		//			{
		//				case TransformTypeEnum.Matrix:
		//					break;
		//				case TransformTypeEnum.Scale:
		//					break;
		//				case TransformTypeEnum.Translate:
		//					break;
		//			}
		//		}
		//	}
		//	return result;
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	ParentSet																															*
		////*-----------------------------------------------------------------------*
		//private TransformCollection mParentSet = null;
		///// <summary>
		///// Get/Set a reference to the parent transform collection.
		///// </summary>
		//public TransformCollection ParentSet
		//{
		//	get { return mParentSet; }
		//	set { mParentSet = value; }
		//}
		////*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Parse																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Parse the content of a transform attribute and return a populated
		/// collection of transform items.
		/// </summary>
		/// <param name="transformContent">
		/// The value of the SVG transform attribute to parse.
		/// </param>
		/// <returns>
		/// Reference to a newly created collection of transform descriptors
		/// representing the SVG attribute value.
		/// </returns>
		public static TransformCollection Parse(string transformContent)
		{
			string name = "";
			MatchCollection parameterMatches = null;
			TransformCollection result = new TransformCollection();
			TransformItem transform = null;
			MatchCollection transformMatches = null;
			TransformTypeEnum transformType = TransformTypeEnum.None;

			if(transformContent?.Length > 0)
			{
				transformMatches = Regex.Matches(transformContent,
					ResourceMain.rxFindSvgTransforms);
				foreach(Match transformMatchItem in transformMatches)
				{
					transform = new TransformItem();
					name = GetValue(transformMatchItem, "name").ToLower();
					if(Enum.TryParse<TransformTypeEnum>(name, true, out transformType))
					{
						transform.TransformType = transformType;
					}
					parameterMatches = Regex.Matches(
						GetValue(transformMatchItem, "params"),
						ResourceMain.rxFindSvgTransformParams);
					foreach(Match parameterMatchItem in parameterMatches)
					{
						transform.Parameters.Add(
							ToFloat(GetValue(parameterMatchItem, "param")));
					}
					//result.Add(transform);
					//	Transforms are applied in a last-to-first order.
					result.Insert(0, transform);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of this collection.
		/// </summary>
		/// <returns>
		/// The string representation of items in this collection.
		/// </returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			string text = "";

			foreach(TransformItem transformItem in this)
			{
				text = transformItem.ToString();
				if(text.Length > 0)
				{
					if(builder.Length > 0)
					{
						builder.Append(' ');
					}
					builder.Append(text);
				}
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	TransformItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Tracking information for an individual transform level.
	/// </summary>
	public class TransformItem
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
		//*	Parameters																														*
		//*-----------------------------------------------------------------------*
		private List<float> mParameters = new List<float>();
		/// <summary>
		/// Get a reference to the list of parameters for this transform.
		/// </summary>
		public List<float> Parameters
		{
			get { return mParameters; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of this transform.
		/// </summary>
		/// <returns>
		/// The string representation of this item.
		/// </returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			int index = 0;

			if(mTransformType != TransformTypeEnum.None)
			{
				builder.Append(mTransformType.ToString().ToLower());
				builder.Append('(');
				foreach(float floatItem in mParameters)
				{
					if(index > 0)
					{
						builder.Append(',');
					}
					builder.Append(floatItem);
					index++;
				}
				builder.Append(')');
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	Transforms																														*
		////*-----------------------------------------------------------------------*
		//private TransformCollection mTransforms = null;
		///// <summary>
		///// Get a reference to the collection of transforms found under this item.
		///// </summary>
		//public TransformCollection Transforms
		//{
		//	get { return mTransforms; }
		//}
		////*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TransformType																													*
		//*-----------------------------------------------------------------------*
		private TransformTypeEnum mTransformType = TransformTypeEnum.None;
		/// <summary>
		/// Get/Set the transformation type applicable to this entry.
		/// </summary>
		public TransformTypeEnum TransformType
		{
			get { return mTransformType; }
			set { mTransformType = value; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
