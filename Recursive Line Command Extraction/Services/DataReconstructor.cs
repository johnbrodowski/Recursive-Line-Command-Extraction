using System.Text;
using Recursive_Line_Command_Extraction.Models;

namespace Recursive_Line_Command_Extraction.Services
{
    /// <summary>
    /// Reconstructs data from SegmentCommand objects by extracting referenced lines
    /// from the original file. Supports recursive reconstruction of nested segments.
    /// </summary>
    public class DataReconstructor
    {
        private readonly Dictionary<int, string> _lineCache;

        public DataReconstructor()
        {
            _lineCache = new Dictionary<int, string>();
        }

        /// <summary>
        /// Loads the original file into memory for fast line lookups.
        /// Lines are indexed starting at 1 to match the line numbering used in commands.
        /// </summary>
        /// <param name="originalFilePath">Path to the original source file</param>
        public void LoadOriginalFile(string originalFilePath)
        {
            if (!File.Exists(originalFilePath))
            {
                throw new FileNotFoundException($"Original file not found: {originalFilePath}");
            }

            _lineCache.Clear();
            var lines = File.ReadAllLines(originalFilePath);

            for (int i = 0; i < lines.Length; i++)
            {
                _lineCache[i + 1] = lines[i]; // 1-based indexing
            }
        }

        /// <summary>
        /// Reconstructs data from a single segment command.
        /// </summary>
        /// <param name="segment">The segment command to reconstruct</param>
        /// <param name="originalFilePath">Path to the original file (if not already loaded)</param>
        /// <returns>Reconstructed text content</returns>
        public string Reconstruct(SegmentCommand segment, string? originalFilePath = null)
        {
            if (segment == null)
            {
                throw new ArgumentNullException(nameof(segment));
            }

            // Load file if not already loaded and path is provided
            if (_lineCache.Count == 0 && !string.IsNullOrEmpty(originalFilePath))
            {
                LoadOriginalFile(originalFilePath);
            }

            if (_lineCache.Count == 0)
            {
                throw new InvalidOperationException(
                    "Original file must be loaded before reconstruction. " +
                    "Call LoadOriginalFile() or provide originalFilePath parameter.");
            }

            var result = new StringBuilder();

            // Add first line if provided
            if (!string.IsNullOrEmpty(segment.FirstLine))
            {
                result.AppendLine(segment.FirstLine);
            }

            // Process line ranges
            if (segment.Ranges != null)
            {
                foreach (var range in segment.Ranges)
                {
                    AppendRange(result, range);
                }
            }

            // Recursively process nested segments
            if (segment.NestedSegments != null)
            {
                foreach (var nested in segment.NestedSegments)
                {
                    var nestedContent = Reconstruct(nested);
                    result.Append(nestedContent);
                }
            }

            // Add last line if provided
            if (!string.IsNullOrEmpty(segment.LastLine))
            {
                result.AppendLine(segment.LastLine);
            }

            return result.ToString();
        }

        /// <summary>
        /// Reconstructs data from multiple segment commands.
        /// </summary>
        /// <param name="segments">List of segment commands</param>
        /// <param name="originalFilePath">Path to the original file (if not already loaded)</param>
        /// <returns>Reconstructed text content from all segments</returns>
        public string ReconstructMultiple(List<SegmentCommand> segments, string? originalFilePath = null)
        {
            if (segments == null || !segments.Any())
            {
                throw new ArgumentException("Segments list cannot be null or empty", nameof(segments));
            }

            // Load file if needed
            if (_lineCache.Count == 0 && !string.IsNullOrEmpty(originalFilePath))
            {
                LoadOriginalFile(originalFilePath);
            }

            var result = new StringBuilder();

            foreach (var segment in segments)
            {
                result.Append(Reconstruct(segment));
                result.AppendLine(); // Add separator between segments
            }

            return result.ToString();
        }

        /// <summary>
        /// Reconstructs data and saves it to a file.
        /// </summary>
        /// <param name="segment">The segment command to reconstruct</param>
        /// <param name="originalFilePath">Path to the original file</param>
        /// <param name="outputFilePath">Path where reconstructed data should be saved</param>
        public void ReconstructToFile(SegmentCommand segment, string originalFilePath, string outputFilePath)
        {
            var reconstructed = Reconstruct(segment, originalFilePath);
            File.WriteAllText(outputFilePath, reconstructed);
        }

        /// <summary>
        /// Reconstructs multiple segments and saves to a file.
        /// </summary>
        /// <param name="segments">List of segment commands</param>
        /// <param name="originalFilePath">Path to the original file</param>
        /// <param name="outputFilePath">Path where reconstructed data should be saved</param>
        public void ReconstructMultipleToFile(List<SegmentCommand> segments, string originalFilePath, string outputFilePath)
        {
            var reconstructed = ReconstructMultiple(segments, originalFilePath);
            File.WriteAllText(outputFilePath, reconstructed);
        }

        /// <summary>
        /// Appends a range of lines from the cache to the result.
        /// </summary>
        /// <param name="result">StringBuilder to append to</param>
        /// <param name="range">The line range to extract</param>
        private void AppendRange(StringBuilder result, LineRange range)
        {
            if (!range.IsValid())
            {
                throw new ArgumentException($"Invalid line range: {range}");
            }

            for (int lineNum = range.StartLine; lineNum <= range.EndLine; lineNum++)
            {
                if (_lineCache.TryGetValue(lineNum, out var line))
                {
                    result.AppendLine(line);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Line {lineNum} not found in original file. " +
                        $"File has {_lineCache.Count} lines.");
                }
            }
        }

        /// <summary>
        /// Gets a specific line from the loaded file.
        /// </summary>
        /// <param name="lineNumber">Line number (1-based)</param>
        /// <returns>The line content</returns>
        public string? GetLine(int lineNumber)
        {
            return _lineCache.TryGetValue(lineNumber, out var line) ? line : null;
        }

        /// <summary>
        /// Clears the internal line cache to free memory.
        /// </summary>
        public void ClearCache()
        {
            _lineCache.Clear();
        }

        /// <summary>
        /// Gets statistics about the currently loaded file.
        /// </summary>
        /// <returns>Dictionary with stats (line count, etc.)</returns>
        public Dictionary<string, object> GetStats()
        {
            return new Dictionary<string, object>
            {
                { "TotalLines", _lineCache.Count },
                { "IsLoaded", _lineCache.Count > 0 }
            };
        }
    }
}
