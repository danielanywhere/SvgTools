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

namespace SvgToolsLib
{
	//*-------------------------------------------------------------------------*
	//*	SvgActionTypeEnum																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of available actions for SVGs.
	/// </summary>
	public enum SvgActionTypeEnum
	{
		/// <summary>
		/// No action specified or unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// Animate an SVG from the provided timeline.
		/// </summary>
		AnimateTimeline,
		/// <summary>
		/// Apply all appropriate transformations in the document.
		/// </summary>
		ApplyTransforms,
		///// <summary>
		///// Convert the SVG UI Design Art to GTK3.
		///// </summary>
		//ArtToGtk3,
		///// <summary>
		///// Convert the SVG UI Design Art to XAML.
		///// </summary>
		//ArtToXaml,
		/// <summary>
		/// Read a configuration file to load a batch of SVG actions.
		/// </summary>
		Batch,
		/// <summary>
		/// Change the content of the specified image by id and filename.
		/// </summary>
		ChangeImage,
		/// <summary>
		/// Clean up the specified SVG file by dereferencing links,
		/// applying transforms, purging unused defs, and rounding all
		/// values.
		/// </summary>
		CleanupSvg,
		/// <summary>
		/// Solve an individual transform for the provided properties.
		/// </summary>
		CalculateTransform,
		///// <summary>
		///// Clear the input files collection.
		///// </summary>
		//ClearInputFiles,
		/// <summary>
		/// Find objects used by reference through href, such as through the use
		/// element, create local instances of those objects contained by the user,
		/// and remove the href attributes.
		/// </summary>
		DereferenceLinks,
		///// <summary>
		///// Draw the image specified by ImageName onto the working image at the
		///// location specified by user properties Left and Top.
		///// </summary>
		//DrawImage,
		///// <summary>
		///// Open the image file specified in the current input file. Name it
		///// in the local images collection with the name specified in the
		///// user property ImageName.
		///// </summary>
		//FileOpenImage,
		///// <summary>
		///// Open each image from the range and place the image specified in
		///// InputFilename at the options specified by Left, Top, Width, and Height.
		///// </summary>
		//FileOverlayImage,
		///// <summary>
		///// Save the working image to the currently specified OutputFile.
		///// </summary>
		//FileSaveImage,
		///// <summary>
		///// Run the Actions collection of the action through all of the files
		///// currently loaded in the InputFiles collection, setting the
		///// CurrentFile property for each pass.
		///// </summary>
		//ForEachFile,
		///// <summary>
		///// Run one or more conditions to determine whether the sub-actions
		///// of the action should be run.
		///// </summary>
		//If,
		///// <summary>
		///// Set the background color of the working image, overlaying the
		///// previous contents on the new background.
		///// </summary>
		//ImageBackground,
		///// <summary>
		///// Clear all images from the Images collection.
		///// </summary>
		//ImagesClear,
		/// <summary>
		/// Enumerate the controls in the specified implied design file.
		/// </summary>
		ImpliedDesignEnumerateControls,
		/// <summary>
		/// Translate the provided implied form design file to Avalonia AXAML.
		/// </summary>
		ImpliedDesignToAvaloniaXaml,
		/// <summary>
		/// Open the working SVG file to allow multiple operations to be completed
		/// in the same session.
		/// </summary>
		OpenWorkingSvg,
		/// <summary>
		/// Remove unused elements from the defs section.
		/// </summary>
		PurgeDefs,
		/// <summary>
		/// Round all values to the precision supplied in the 'Precision'
		/// user property.
		/// </summary>
		RoundAllValues,
		///// <summary>
		///// Run the sequence specified in the 'SequenceName' user property.
		///// </summary>
		//RunSequence,
		/// <summary>
		/// Save the current working SVG to the specified output filename.
		/// </summary>
		SaveWorkingSvg,
		///// <summary>
		///// Set the current working image to the one with the local name
		///// found in the user property ImageName.
		///// </summary>
		//SetWorkingImage,
		///// <summary>
		///// Scale the image to a new size, as specified in user properties
		///// Width and Height.
		///// </summary>
		//SizeImage,
		/// <summary>
		/// Sort the symbols alphabetically in the defs section.
		/// </summary>
		SortSymbols,
		/// <summary>
		/// Output a manifest of the objects, and optionally, the properties of
		/// a XAML file.
		/// </summary>
		XamlManifest,
		/// <summary>
		/// Merge the contents of two or more XAML files, producing a single
		/// output file.
		/// </summary>
		XamlMergeContents
	}
	//*-------------------------------------------------------------------------*

}
