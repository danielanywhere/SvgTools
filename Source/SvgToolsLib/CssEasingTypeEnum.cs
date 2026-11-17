/*
 * Copyright (c). 2025-2026 Daniel Patterson, MCSD (danielanywhere).
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

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	CssEasingTypeEnum																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of recognized easing types.
	/// </summary>
	public enum CssEasingTypeEnum
	{
		/// <summary>
		/// No entry specified or type unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// The value is one or the other of two choices.
		/// </summary>
		Discrete,
		/// <summary>
		/// The value is a linear interpolation between two values.
		/// </summary>
		Linear,
		/// <summary>
		/// The value follows an ease-in sine pattern.
		/// </summary>
		EaseInSine,
		/// <summary>
		/// The value follows an ease-out sine pattern.
		/// </summary>
		EaseOutSine,
		/// <summary>
		/// The value follows an ease-in, ease-out sine pattern.
		/// </summary>
		EaseInOutSine,
		/// <summary>
		/// The value follows an ease-in quadratic (x^2) curve pattern.
		/// </summary>
		EaseInQuad,
		/// <summary>
		/// The value follows an ease-out quadratic (x^2) curve pattern.
		/// </summary>
		EaseOutQuad,
		/// <summary>
		/// The value follows an ease-in, ease-out quadratic (x^2) curve pattern.
		/// </summary>
		EaseInOutQuad,
		/// <summary>
		/// The value follows an ease-in cubic (x^3) curve pattern.
		/// </summary>
		EaseInCubic,
		/// <summary>
		/// The value follows an ease-out cubic (x^3) curve pattern.
		/// </summary>
		EaseOutCubic,
		/// <summary>
		/// The value follows an ease-in, ease-out cubic (x^3) curve pattern.
		/// </summary>
		EaseInOutCubic,
		/// <summary>
		/// The value follows an ease-in quartic (x^4) curve pattern.
		/// </summary>
		EaseInQuart,
		/// <summary>
		/// The value follows an ease-out quartic (x^4) curve pattern.
		/// </summary>
		EaseOutQuart,
		/// <summary>
		/// The value follows an ease-in, ease-out quartic (x^4) curve pattern.
		/// </summary>
		EaseInOutQuart,
		/// <summary>
		/// The value follows an ease-in quintic (x^5) curve pattern.
		/// </summary>
		EaseInQuint,
		/// <summary>
		/// The value follows an ease-out quintic (x^5) curve pattern.
		/// </summary>
		EaseOutQuint,
		/// <summary>
		/// The value follows an ease-in, ease-out quintic (x^5) curve pattern.
		/// </summary>
		EaseInOutQuint,
		/// <summary>
		/// The value follows an ease-in exponential curve pattern.
		/// </summary>
		EaseInExpo,
		/// <summary>
		/// The value follows an ease-out exponential curve pattern.
		/// </summary>
		EaseOutExpo,
		/// <summary>
		/// The value follows an ease-in, ease-out exponential curve pattern.
		/// </summary>
		EaseInOutExpo,
		/// <summary>
		/// The value follows an ease-in circular curve pattern.
		/// </summary>
		EaseInCirc,
		/// <summary>
		/// The value follows an ease-out circular curve pattern.
		/// </summary>
		EaseOutCirc,
		/// <summary>
		/// The value follows an ease-in, ease-out circular curve pattern.
		/// </summary>
		EaseInOutCirc,
		/// <summary>
		/// The value follows an ease-in back-tracking curve pattern.
		/// </summary>
		EaseInBack,
		/// <summary>
		/// The value follows an ease-out back-tracking curve pattern.
		/// </summary>
		EaseOutBack,
		/// <summary>
		/// The value follows an ease-in, ease-out back-tracking curve pattern.
		/// </summary>
		EaseInOutBack,
		/// <summary>
		/// The value follows an ease-in elastic curve pattern.
		/// </summary>
		EaseInElastic,
		/// <summary>
		/// The value follows an ease-out elastic curve pattern.
		/// </summary>
		EaseOutElastic,
		/// <summary>
		/// The value follows an ease-in, ease-out elastic curve pattern.
		/// </summary>
		EaseInOutElastic,
		/// <summary>
		/// The value follows an ease-in bounce curve pattern.
		/// </summary>
		EaseInBounce,
		/// <summary>
		/// The value follows an ease-out bounce curve pattern.
		/// </summary>
		EaseOutBounce,
		/// <summary>
		/// The value follows an ease-in, ease-out bounce curve pattern.
		/// </summary>
		EaseInOutBounce
	}
	//*-------------------------------------------------------------------------*

}
