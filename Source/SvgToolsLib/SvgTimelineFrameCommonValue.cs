using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	SvgTimelineFrameCommonValueCollection																		*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of SvgTimelineFrameCommonValueItem Items.
	/// </summary>
	public class SvgTimelineFrameCommonValueCollection :
		List<SvgTimelineFrameCommonValueItem>
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
		//*	ApplyValues																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Assign the shared common properties for the referenced object on the
		/// current frame.
		/// </summary>
		/// <param name="keyframe">
		/// Reference to the keyframe whose per-object common values will be
		/// copied.
		/// </param>
		public void ApplyValues(SvgTimelineKeyframeItem keyframe)
		{
			SvgTimelineFrameCommonValueItem item = null;

			if(keyframe?.ObjectName?.Length > 0 &&
				(keyframe.ActiveFields &
				(SvgTimelineKeyframeFieldEnum.OriginX |
				SvgTimelineKeyframeFieldEnum.OriginY)) !=
				SvgTimelineKeyframeFieldEnum.None)
			{
				item = this.FirstOrDefault(x => x.ObjectName == keyframe.ObjectName);
				if(item == null)
				{
					item = new SvgTimelineFrameCommonValueItem()
					{
						ObjectName = keyframe.ObjectName
					};
					this.Add(item);
				}
				if((keyframe.ActiveFields & SvgTimelineKeyframeFieldEnum.OriginX) !=
					SvgTimelineKeyframeFieldEnum.None)
				{
					item.FloatProperties.SetValue("OriginX", keyframe.OriginX);
				}
				if((keyframe.ActiveFields & SvgTimelineKeyframeFieldEnum.OriginY) !=
					SvgTimelineKeyframeFieldEnum.None)
				{
					item.FloatProperties.SetValue("OriginY", keyframe.OriginY);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FloatValueExists																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified common value associated
		/// with the named object exists in this list.
		/// </summary>
		/// <param name="objectName">
		/// Name of the object to which the value will be associated.
		/// </param>
		/// <param name="propertyName">
		/// Name of the property to search for.
		/// </param>
		/// <returns>
		/// True if the specified property is associated with the object name.
		/// Otherwise, false.
		/// </returns>
		public bool FloatValueExists(string objectName, string propertyName)
		{
			bool result = false;

			if(objectName?.Length > 0 && propertyName?.Length > 0)
			{
				result = this.Exists(x =>
					string.Equals(x.ObjectName, objectName,
						StringComparison.OrdinalIgnoreCase) &&
					x.FloatProperties.Exists(y =>
					string.Equals(y.Name, propertyName,
						StringComparison.OrdinalIgnoreCase)));
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetFloatValue																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the float value representing the specified common property value
		/// associated with the named object in this list.
		/// </summary>
		/// <param name="objectName">
		/// Name of the object to which the value will be associated.
		/// </param>
		/// <param name="propertyName">
		/// Name of the property to search for.
		/// </param>
		/// <returns>
		/// Value of the common float property found for the object name, if
		/// found. Otherwise, 0.
		/// </returns>
		public float GetFloatValue(string objectName, string propertyName)
		{
			SvgTimelineFrameCommonValueItem item = null;
			NameValueFloatItem property = null;
			float result = 0f;

			if(objectName?.Length > 0 && propertyName?.Length > 0)
			{
				item = this.FirstOrDefault(x =>
					string.Equals(x.ObjectName, objectName,
						StringComparison.OrdinalIgnoreCase) &&
					x.FloatProperties.Exists(y =>
					string.Equals(y.Name, propertyName,
						StringComparison.OrdinalIgnoreCase)));
				if(item != null)
				{
					property = item.FloatProperties.FirstOrDefault(x =>
						string.Equals(x.Name, propertyName,
							StringComparison.OrdinalIgnoreCase));
					if(property != null)
					{
						result = property.Value;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	SvgTimelineFrameCommonValueItem																					*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about a set of values shared by a single object for a given
	/// output frame.
	/// </summary>
	public class SvgTimelineFrameCommonValueItem
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
		//*	FloatProperties																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="FloatProperties">FloatProperties</see>.
		/// </summary>
		private NameValueFloatCollection mFloatProperties =
			new NameValueFloatCollection();
		/// <summary>
		/// Get a reference to the collection of float properties defined for this
		/// frame.
		/// </summary>
		public NameValueFloatCollection FloatProperties
		{
			get { return mFloatProperties; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ObjectName																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Private member for <see cref="ObjectName">ObjectName</see>.
		/// </summary>
		private string mObjectName = "";
		/// <summary>
		/// Get/Set the name of the object being updated.
		/// </summary>
		public string ObjectName
		{
			get { return mObjectName; }
			set { mObjectName = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
