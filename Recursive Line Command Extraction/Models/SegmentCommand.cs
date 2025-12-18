namespace Recursive_Line_Command_Extraction.Models
{
    /// <summary>
    /// Represents a segment command returned by the AI.
    /// A segment contains:
    /// - The first line (actual text content)
    /// - One or more ranges of lines (references to original line numbers)
    /// - The last line (actual text content)
    /// - Optional nested segments following the same pattern
    /// </summary>
    public class SegmentCommand
    {
        /// <summary>
        /// The actual text content of the first line of this segment.
        /// </summary>
        public string? FirstLine { get; set; }

        /// <summary>
        /// List of line ranges that should be extracted from the original file.
        /// These ranges reference line numbers from the numbered source file.
        /// </summary>
        public List<LineRange> Ranges { get; set; } = new List<LineRange>();

        /// <summary>
        /// The actual text content of the last line of this segment.
        /// </summary>
        public string? LastLine { get; set; }

        /// <summary>
        /// Optional nested segments that follow the same pattern.
        /// This allows for hierarchical/recursive data extraction.
        /// </summary>
        public List<SegmentCommand>? NestedSegments { get; set; }

        /// <summary>
        /// Validates that the segment command has the minimum required data.
        /// </summary>
        public bool IsValid()
        {
            // At minimum, we need ranges or nested segments
            return (Ranges != null && Ranges.Any()) ||
                   (NestedSegments != null && NestedSegments.Any());
        }
    }
}
