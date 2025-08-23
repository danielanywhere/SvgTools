using System;
using System.Collections.Generic;
using System.Text;
using Geometry;
using SkiaSharp;

namespace SvgToolsLibrary
{
	//*-------------------------------------------------------------------------*
	//*	Matrix32																																*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// 3 x 2 matrix.
	/// </summary>
	public class Matrix32
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
		//*	A																																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get/Set the value of the A item.
		/// </summary>
		public float A
		{
			get { return mValues[0]; }
			set { mValues[0] = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	AssignSequential																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Sequentially assign a list of values to the matrix.
		/// </summary>
		/// <param name="values">
		/// List of values to assign.
		/// </param>
		public void AssignSequential(List<float> values)
		{
			int count = 0;
			int index = 0;

			if(values?.Count > 0)
			{
				count = Math.Min(values.Count, 6);
				for(index = 0; index < count; index++)
				{
					mValues[index] = values[index];
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	B																																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get/Set the value of the B item.
		/// </summary>
		public float B
		{
			get { return mValues[1]; }
			set { mValues[1] = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	C																																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get/Set the value of the C item.
		/// </summary>
		public float C
		{
			get { return mValues[2]; }
			set { mValues[2] = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	D																																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get/Set the value of the D item.
		/// </summary>
		public float D
		{
			get { return mValues[3]; }
			set { mValues[3] = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	E																																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get/Set the value of the E item.
		/// </summary>
		public float E
		{
			get { return mValues[4]; }
			set { mValues[4] = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	F																																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get/Set the value of the F item.
		/// </summary>
		public float F
		{
			get { return mValues[5]; }
			set { mValues[5] = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Transform																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Transform two of the values of the caller's list based on the contents
		/// of the matrix and the list.
		/// </summary>
		/// <param name="x">
		/// X coordinate to transform.
		/// </param>
		/// <param name="y">
		/// Y coordinate to transform.
		/// </param>
		/// <returns>
		/// Floating-point point.
		/// </returns>
		public FVector2 Transform(float x, float y)
		{
			FVector2 result = new FVector2();

			//	https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/transform
			//	newX = a * x + c * y + e.
			//	newY = b * x + d * y + f.
			result.X = (mValues[0] * x) + (mValues[2] * y) + mValues[4];
			result.Y = (mValues[1] * x) + (mValues[3] * y) + mValues[5];
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Values																																*
		//*-----------------------------------------------------------------------*
		private float[] mValues = new float[6];
		/// <summary>
		/// Get the raw list of values in this matrix.
		/// </summary>
		public float[] Values
		{
			get { return mValues; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
