namespace Recursive_Line_Command_Extraction.Models
{
    /// <summary>
    /// Represents a range of lines in the original file.
    /// This is used to specify which consecutive lines should be extracted.
    /// </summary>
    public class LineRange
    {
        /// <summary>
        /// The starting line number (1-based index in the original file).
        /// </summary>
        public int StartLine { get; set; }

        /// <summary>
        /// The ending line number (1-based index in the original file), inclusive.
        /// </summary>
        public int EndLine { get; set; }

        /// <summary>
        /// Validates that the line range is valid.
        /// </summary>
        public bool IsValid()
        {
            return StartLine > 0 && EndLine >= StartLine;
        }

        public override string ToString()
        {
            return $"Lines {StartLine}-{EndLine}";
        }
    }
}
