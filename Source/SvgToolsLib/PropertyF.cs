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
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	PropertyFCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of PropertyFItem Items.
	/// </summary>
	public class PropertyFCollection : List<PropertyFItem>
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
		//* GetValue																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the property having the specified name.
		/// </summary>
		/// <param name="name">
		/// Name of the property to retrieve.
		/// </param>
		/// <returns>
		/// Value of the specified property, if found. Otherwise, 0.
		/// </returns>
		public float GetValue(string name)
		{
			string nameLower = "";
			PropertyFItem prop = null;
			float result = 0f;

			if(name?.Length > 0)
			{
				nameLower = name.ToLower();
				prop = this.FirstOrDefault(x => x.Name.ToLower() == nameLower);
				if(prop != null)
				{
					result = prop.Value;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetUnchanged																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the contents of this collection to unchanged.
		/// </summary>
		public void SetUnchanged()
		{
			foreach(PropertyFItem propertyItem in this)
			{
				propertyItem.Changed = false;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SetValue																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the value on the specified property, creating it if necessary.
		/// </summary>
		/// <param name="name">
		/// Name of the property.
		/// </param>
		/// <param name="value">
		/// Value to set on the property.
		/// </param>
		public void SetValue(string name, float value)
		{
			PropertyFItem property = null;

			if(name?.Length > 0)
			{
				property = this.FirstOrDefault(x => x.Name == name);
				if(property == null)
				{
					property = new PropertyFItem()
					{
						Name = name
					};
					this.Add(property);
				}
				property.Value = value;
			}
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	PropertyFItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual floating point value property.
	/// </summary>
	public class PropertyFItem : INotifyPropertyChanged
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* NotifyPropertyChanged																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Called by the Set accessor of each property to raise the
		/// PropertyChanged event.
		/// </summary>
		/// <param name="propertyName">
		/// Name of the property changed. Inferred for all native properties.
		/// </param>
		private void NotifyPropertyChanged(
			[CallerMemberName] String propertyName = "")
		{
			mChanged = true;
			PropertyChanged?.Invoke(this,
				new PropertyChangedEventArgs(propertyName));
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	Changed																																*
		//*-----------------------------------------------------------------------*
		private bool mChanged = false;
		/// <summary>
		/// Get/Set a value indicating whether this value has been changed since
		/// the last check.
		/// </summary>
		public bool Changed
		{
			get { return mChanged; }
			set { mChanged = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Name																																	*
		//*-----------------------------------------------------------------------*
		private string mName = "";
		/// <summary>
		/// Get/Set the name of the property.
		/// </summary>
		public string Name
		{
			get { return mName; }
			set
			{
				bool bChanged = (mName != value);

				mName = value;
				if(bChanged)
				{
					NotifyPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* PropertyChanged																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Fired when the value of a property has changd.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Value																																	*
		//*-----------------------------------------------------------------------*
		private float mValue = 0f;
		/// <summary>
		/// Get/Set the value of the property.
		/// </summary>
		public float Value
		{
			get { return mValue; }
			set
			{
				bool bChanged = (mValue != value);

				mValue = value;
				if(bChanged)
				{
					NotifyPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
