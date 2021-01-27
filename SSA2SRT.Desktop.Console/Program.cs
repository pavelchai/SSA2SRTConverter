/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using System;
using System.IO;
using SSA2SRT.Model;
using System.Linq;
using System.Collections.Generic;

namespace ASS2SRT
{
	class Program
	{
		/// <summary>
		/// Entry point function.
		/// </summary>
		/// <param name="args"> Args. </param>
		public static void Main(string[] args)
		{
			Console.BackgroundColor = ConsoleColor.Blue;
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("SSS/ASS to SRT Subtitle converter");
			Console.WriteLine();
			Console.ResetColor();

			Console.WriteLine("1) Select the input directory with the .ssa/.ass subtitles or/and ZIP32/64 storages with these files");
			string inputPath = ReadPath();
			
			Console.WriteLine("2) Select the output directory");
			string outputPath = ReadPath();

			SSA2SRTConverterSettings settings = new SSA2SRTConverterSettings()
			{
				NameConverter = n => ConvertName(n, outputPath),
			};

			IEnumerable<SSA2SRTConverterData> input = Directory.EnumerateFiles(inputPath).
				Where(f => 
				      f.EndsWith(".ass", StringComparison.OrdinalIgnoreCase) || 
				      f.EndsWith(".ssa", StringComparison.OrdinalIgnoreCase) || 
				      f.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)).
				Select(f => new SSA2SRTConverterData(f, File.ReadAllBytes(f)));
			
			IEnumerable<SSA2SRTConverterData> output = SSA2SRTConverter.Convert(input, settings);

			foreach (var converted in output)
			{
				File.WriteAllBytes(converted.Name, converted.Data);
			}
		}
		
		/// <summary>
		/// Converts a name of the file.
		/// </summary>
		/// <param name="fillName"> Full name of the file. </param>
		/// <param name="outputPath"> Output path. </param>
		/// <returns> Converted name. </returns>
		private static string ConvertName(string fillName, string outputPath)
		{
			if (File.Exists(fillName))
			{
				string name = Path.GetFileNameWithoutExtension(fillName);
				string ex = Path.GetExtension(fillName);
				
				if (!ex.Equals(".zip", StringComparison.OrdinalIgnoreCase))
				{
					return Path.Combine(outputPath, string.Concat(name, ".srt"));
				}
				else
				{
					return Path.Combine(outputPath, string.Concat(name, ".converted.zip"));
				}
			}
			else
			{
				return fillName.Replace(".ass", ".srt").Replace(".ssa", ".srt").Replace(".zip", ".converted.zip");
			}
		}
		
		/// <summary>
		/// Reads the path from console.
		/// </summary>
		/// <returns> The path. </returns>
		private static string ReadPath()
		{
			string path;
			
			while (true)
			{
				Console.Write("Directory: ");
				path = Console.ReadLine();
				
				if (Directory.Exists(path))
				{
					Console.WriteLine();
					return path;
				}
				else
				{
					Console.WriteLine("Path <{0}> isn't exist", path);
					Console.WriteLine();
				}
			}
		}
	}
}