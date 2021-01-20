/*
 * SSA2SRT Converter.
 * Licensed under MIT License.
 * Copyright © 2021 Pavel Chaimardanov.
 */
using SSA2SRT.Model.Utils;
using System;
using System.Text;
using System.Collections.Generic;

namespace SSA2SRT.Model
{
	/// <summary>
	/// Represents a converter from the SSA/ASS to SRT.
	/// </summary>
	public static class SSA2SRTConverter
	{
		/// <summary>
		/// Converts the SSA/ASS subtitles to the SRT subtitles.
		/// </summary>
		/// <param name="data"> SSA/ASS subtitles or/and ZIP32/64 files with SSA/ASS subtitles. </param>
		/// <param name="settings"> Settings for the converter. </param>
		/// <returns> SRT subtitles or/and ZIP32/64 files with SRT subtitles. </returns>
		/// <exception cref="ArgumentException">
        /// The exception that is thrown when argumen is null.
        /// </exception>
		/// <exception cref="InvalidOperationException">
        /// The exception that is thrown when subtitle is invalid.
        /// </exception>
		public static IEnumerable<SSA2SRTConverterData> Convert(IEnumerable<SSA2SRTConverterData> data, SSA2SRTConverterSettings settings)
		{
			if (data == null)
			{
				throw new ArgumentException("Data is null");
			}
			
			if (settings== null)
			{
				throw new ArgumentException("Settings is null");
			}
			
			bool ignoreErrors = settings.IgnoreExceptions.GetValueOrDefault();
			int index = 0;
			
			IReadOnlyDataStorer inputStorer;
			IWriteOnlyDataStorer outputStorer;
			string valueName, convertedName;
			byte[] valueData, convertedData;
			
			foreach(var value in data)
			{
				convertedData = null;
				
				valueName = value.Name;
				valueData = value.Data;
				
				if (valueName == null)
				{
					throw new ArgumentException(string.Format("Key with index {0} is null.", index));
				}
				
				if(valueData == null)
				{
					throw new ArgumentException(string.Format("Data with the name {0} is null.", valueName));
				}
				
				convertedName = settings.NameConverter(valueName);
				
				try
				{
					inputStorer = ZipReadOnlyStorer.FromData(valueData);
					outputStorer = new ZipWriteOnlyStorer(true, (inputStorer as ZipReadOnlyStorer).IsZip64);
					
					ConvertFromStorer(inputStorer, outputStorer, settings);
					convertedData = outputStorer.Close();
				}
				catch
				{
					try
					{
						convertedData = ConvertFromSubtitle(valueData, valueName, settings);
					}
					catch
					{
						if (!ignoreErrors)
						{
							throw;
						}
					}
				}
				
				if (convertedData != null)
				{
					yield return new SSA2SRTConverterData(convertedName, convertedData);
				}
				
				index++;
			}
		}
		
		/// <summary>
		/// Converts the SSA/ASS subtitles in the storer to the SRT subtitles.
		/// </summary>
		/// <param name="inputStorer"> Input storer. </param>
		/// <param name="outputStorer"> Output storer. The storer contains converted data. </param>
		/// <param name="settings"> Settings for the converter. </param>
		private static void ConvertFromStorer(IReadOnlyDataStorer inputStorer, IWriteOnlyDataStorer outputStorer, SSA2SRTConverterSettings settings)
		{
			IReadOnlyList<IDataStorerEntry> entries = inputStorer.Entries;
			int count = entries.Count;
			bool ignoreErrors = settings.IgnoreExceptions.GetValueOrDefault();
			
			IDataStorerEntry entry;
			string name;
			
			for (int i = 0; i < count; i++)
			{
				entry = entries[i];
				name = entry.Path;
				
				if (!name.EndsWith(".ass", StringComparison.OrdinalIgnoreCase) 
				    && !name.EndsWith(".ssa", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				
				try
				{
					outputStorer.Add(
						settings.NameConverter(name),
						ConvertFromSubtitle(inputStorer.Read(entry), name, settings));
				}
				catch
				{
					if (!ignoreErrors)
					{
						throw;
					}
				}
			}
		}
		
		/// <summary>
		/// Converts the SSA/ASS subtitle to the SRT subtitle.
		/// </summary>
		/// <param name="data"> Array of the bytes that represents the SSA/ASS subtitle. </param>
		/// <param name="name"> Name of the SSA/ASS subtitle. </param>
		/// <param name="settings"> Settings for the converter. </param>
		/// <returns> Array of the bytes that represents the SRT subtitle. </returns>
        /// <exception cref="InvalidOperationException">
        /// The exception that is thrown when subtitle is invalid.
        /// </exception>
		private static byte[] ConvertFromSubtitle(byte[] data, string name, SSA2SRTConverterSettings settings)
		{
			int startIndex = -1;
			int endIndex = -1;
			int textIndex = -1;
			string text = "";
			
			Encoding detectedEncoding = StringUtils.DetectTextEncoding(data, out text);
			IEnumerator<string> linesEnumerator = StringUtils.SplitLines(text).GetEnumerator();
			
			ReadIndexes(name, linesEnumerator, out startIndex, out endIndex, out textIndex);
			string converted = Convert(linesEnumerator, startIndex, endIndex, textIndex, settings.MinShowTime);
			
			Encoding outputEncoding = settings.OutputEncoding ?? detectedEncoding;
			
			byte[] preambleBytes = outputEncoding.GetPreamble();
			int preambleLength = preambleBytes.Length;
			int bytesCount = outputEncoding.GetByteCount(converted);
			
			byte[] output = new byte[preambleLength + bytesCount];
			
			Buffer.BlockCopy(preambleBytes, 0, output, 0, preambleLength);
			outputEncoding.GetBytes(converted, 0, converted.Length, output, preambleLength);
			
			return output;
		}
		
		/// <summary>
		/// Reads the indexes that associated with positions of the the start time, end time, and text. 
		/// </summary>
		/// <param name="name"> Name of the SSA/ASS subtitle. </param>
		/// <param name="lineEnumerator"> Enumerator for the lines. </param>
		/// <param name="startIndex"> Index that associated with the start time. </param>
		/// <param name="endIndex"> Index that associated with the end time. </param>
		/// <param name="textIndex"> Index that associated with the text. </param>
		/// <exception cref="InvalidOperationException">
        /// The exception that is thrown when subtitle is invalid.
        /// </exception>
		private static void ReadIndexes(string name, IEnumerator<string> lineEnumerator, out int startIndex, out int endIndex, out int textIndex)
		{
			int length = 0;
			int index = 0;
			string currentLine = "";
			string previousLine = "";
			IEnumerable<string> values;
			
			startIndex = -1;
			endIndex = -1;
			textIndex = -1;
			
			while(lineEnumerator.MoveNext())
			{
				currentLine = lineEnumerator.Current;
				length = currentLine.Length;
				
				if(length > 7 && currentLine.StartsWith("Format:", StringComparison.Ordinal) && previousLine.StartsWith("[Events]",  StringComparison.Ordinal))
				{
					index = 0;
					values = StringUtils.Split(currentLine, 7, length - 7, ',');
					
					foreach(var value in values)
					{
						if(value.Contains("Start"))
						{
							startIndex = index;
						}
						
						if(value.Contains("End"))
						{
							endIndex = index;
						}
						
						if(value.Contains("Text"))
						{
							textIndex = index;
						}
						
						index++;
					}
					
					break;
				}
				
				previousLine = currentLine;
			}
			
			if(startIndex == -1 || endIndex == -1 || textIndex == -1)
			{
				throw new InvalidOperationException(string.Format("SSA/SRT subtitle with name {0} is invalid.", name));
			}
		}
		
		/// <summary>
		/// Converts the lines of the SSA/ASS subtitle to the SRT subtitle.
		/// </summary>
		/// <param name="lineEnumerator"> Enumerator for the lines. </param>
		/// <param name="startIndex"> Index that associated with the start time. </param>
		/// <param name="endIndex"> Index that associated with the end time. </param>
		/// <param name="textIndex"> Index that associated with the text. </param>
		/// <param name="minShowTime"> Minimal time to show. If null - as defined in the subtitle. </param>
		/// <returns> String that represents a SRT subtitle. </returns>
		private static string Convert(IEnumerator<string> lineEnumerator, int startIndex, int endIndex, int textIndex, double? minShowTime)
		{
			StringBuilder converted = new StringBuilder();
			
			int lineIndex = 1;
			int valueIndex = 0;
			string currentLine = "";
			string start = "", end = "";
			IEnumerable<string> values;
			int maxIndex = Math.Max(startIndex, endIndex);
			
			bool notFirstLine = false;
			
			DateTime startTime, endTime;
			TimeSpan delta;
			
			double minTime = minShowTime != null ? minShowTime.Value : 0;
			TimeSpan minShowTimeDelta = TimeSpan.FromSeconds(minTime);
			StringBuilder subtitleBuilder = new StringBuilder();
			
			while(lineEnumerator.MoveNext())
			{
				currentLine = lineEnumerator.Current;
				
				if(currentLine.StartsWith("Dialogue: ", StringComparison.Ordinal))
				{
					values = StringUtils.Split(currentLine, 10, currentLine.Length - 10, ',');
					valueIndex = 0;
					
					// Gets the start time, end time, subtitle text
					foreach(var value in values)
					{
						if(valueIndex == startIndex)
						{
							start = value;
						}
						
						if(valueIndex == endIndex)
						{
							end = value;
						}
						
						if(++valueIndex > maxIndex)
						{
							break;
						}
					}
					
					// Writes the subtitle
					
					if(DateTime.TryParse(start, out startTime) && DateTime.TryParse(end, out endTime))
					{
						subtitleBuilder = GetSubtitleText(currentLine, textIndex);
						
						if(subtitleBuilder.Length != 0)
						{
							// Time correction
							delta = endTime - startTime;
							if(delta.TotalSeconds < minTime) 
							{
								endTime = startTime + minShowTimeDelta;
							}
							
							if(notFirstLine)
							{
								// New lines between subtitles
								converted.AppendLine();
								converted.AppendLine();
								converted.AppendLine();
							}
							else
							{
								notFirstLine = true;
							}
							
							// 1st line - line number
							converted.AppendLine((lineIndex++).ToString());
							
							// 2nd line - time
							
							// Start
							converted.Append(startTime.Hour);
							converted.Append(":");
							converted.Append(startTime.Minute);
							converted.Append(":");
							converted.Append(startTime.Second);
							converted.Append(",");
							converted.Append(startTime.Millisecond);
							
							// From --> To
							converted.Append(" --> ");
							
							// End
							
							converted.Append(endTime.Hour);
							converted.Append(":");
							converted.Append(endTime.Minute);
							converted.Append(":");
							converted.Append(endTime.Second);
							converted.Append(",");
							converted.Append(endTime.Millisecond);
							
							// New line
							converted.AppendLine();
							
							// Other lines - subtitle text
							converted.Append(subtitleBuilder.ToString());
						}
					}
				}
			}
			
			return converted.ToString();
		}
		
		/// <summary>
		/// Gets the builder of the text of the subtitle (without special chars).
		/// </summary>
		/// <param name="line"> The line. </param>
		/// <param name="textIndex"> Index that associated with the text. </param>
		/// <returns> Builder of the text of the subtitle. </returns>
		private static unsafe StringBuilder GetSubtitleText(string line, int textIndex)
		{
			StringBuilder subtitleBuilder = new StringBuilder();
			
			if(line.Contains(@"\p0") || line.Contains(@"\p1") || line.Contains(@"\p2"))
			{
				return subtitleBuilder;
			}
			
			int length = line.Length;
			
			bool hasStartTagIndex = false;
			bool hasLeftChar = false;
			bool hasRightChar = false;
			
			int textOffset = GetTextOffset(line, textIndex);
			int expected = length - textOffset;
			char currentChar, nextChar;
			
			fixed(char* linePtr = line)
			{
				char* ptr = &linePtr[textOffset];
				
				while(expected-- > 0)
				{
					currentChar = *ptr++;
					
					if(!hasStartTagIndex && currentChar == '{')
					{
						hasStartTagIndex = true;
					}
					
					if(!hasStartTagIndex)
					{
						if(currentChar == '\\' && expected > 0)
						{
							nextChar = *ptr;
							
							if(nextChar == 'N' || nextChar == 'n' || nextChar == 'h')
							{
								hasLeftChar = subtitleBuilder.Length != 0;
								hasRightChar = expected > 1;
								
								if(hasLeftChar && hasRightChar)
								{
									if(subtitleBuilder[subtitleBuilder.Length - 1] != ' ' && *(ptr + 1) != ' ')
									{
										subtitleBuilder.Append(' ');
									}
								}
								else
								{
									if((hasLeftChar && subtitleBuilder[subtitleBuilder.Length - 1] != ' ') || (hasRightChar && *(ptr + 1) != ' '))
									{
										subtitleBuilder.Append(' ');
									}
								}
								
								ptr++;
								expected -= 1;
							}
						}
						else
						{
							subtitleBuilder.Append(currentChar);
						}
					}
					else
					{
						if(currentChar == '}')
						{
							hasStartTagIndex = false;
						}
					}
				}
			}
			
			return subtitleBuilder;
		}
		
		/// <summary>
		/// Gets an offset of the text in the line. 
		/// </summary>
		/// <param name="line"> The line. </param>
		/// <param name="textIndex"> Index that associated with the text. </param>
		/// <returns> Offset of the text in the line. </returns>
		/// <remarks> All commas in the text is a part of the text (from the specification). </remarks>
		private static unsafe int GetTextOffset(string line, int textIndex)
		{
			int commaCount = 0;
			int lengthN1 = line.Length - 1;
			int index = 0;
			
			fixed(char* linePtr = line)
			{
				char* ptr = linePtr;
				
				while(index++ != lengthN1)
				{
					if(*ptr++ == ',')
					{
						commaCount++;
					}
					
					if(commaCount == textIndex)
					{
						return index != lengthN1 ? index: index + 1;
					}
				}
			}
			
			return lengthN1;
		}
	}
}