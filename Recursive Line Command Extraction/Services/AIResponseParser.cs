using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Recursive_Line_Command_Extraction.Models;

namespace Recursive_Line_Command_Extraction.Services
{
    /// <summary>
    /// Parses AI JSON responses into SegmentCommand objects.
    /// Handles both simple and nested segment structures.
    /// </summary>
    public class AIResponseParser
    {
        /// <summary>
        /// Parses a JSON string containing segment commands into a SegmentCommand object.
        /// Supports recursive parsing of nested segments.
        /// </summary>
        /// <param name="jsonResponse">JSON string from AI response</param>
        /// <returns>Parsed SegmentCommand object</returns>
        public SegmentCommand ParseResponse(string jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                throw new ArgumentException("JSON response cannot be null or empty", nameof(jsonResponse));
            }

            try
            {
                var segment = JsonConvert.DeserializeObject<SegmentCommand>(jsonResponse);

                if (segment == null)
                {
                    throw new InvalidOperationException("Failed to deserialize JSON response");
                }

                ValidateSegmentCommand(segment);
                return segment;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to parse AI response as JSON: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Parses a JSON string that may contain multiple segment commands.
        /// </summary>
        /// <param name="jsonResponse">JSON string containing array of segments</param>
        /// <returns>List of parsed SegmentCommand objects</returns>
        public List<SegmentCommand> ParseMultipleSegments(string jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                throw new ArgumentException("JSON response cannot be null or empty", nameof(jsonResponse));
            }

            try
            {
                var segments = JsonConvert.DeserializeObject<List<SegmentCommand>>(jsonResponse);

                if (segments == null)
                {
                    throw new InvalidOperationException("Failed to deserialize JSON response");
                }

                foreach (var segment in segments)
                {
                    ValidateSegmentCommand(segment);
                }

                return segments;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to parse AI response as JSON: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Attempts to parse a JSON response that could be either a single segment or an array.
        /// </summary>
        /// <param name="jsonResponse">JSON string from AI</param>
        /// <returns>List of SegmentCommand objects (single item if input was a single segment)</returns>
        public List<SegmentCommand> ParseFlexible(string jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                throw new ArgumentException("JSON response cannot be null or empty", nameof(jsonResponse));
            }

            try
            {
                var token = JToken.Parse(jsonResponse);

                if (token is JArray)
                {
                    return ParseMultipleSegments(jsonResponse);
                }
                else if (token is JObject)
                {
                    var segment = ParseResponse(jsonResponse);
                    return new List<SegmentCommand> { segment };
                }
                else
                {
                    throw new InvalidOperationException("JSON response must be either an object or an array");
                }
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to parse AI response: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Validates a segment command to ensure it has the required data.
        /// Recursively validates nested segments.
        /// </summary>
        /// <param name="segment">The segment to validate</param>
        private void ValidateSegmentCommand(SegmentCommand segment)
        {
            if (segment == null)
            {
                throw new ArgumentNullException(nameof(segment));
            }

            // Check that we have at least ranges or nested segments
            bool hasRanges = segment.Ranges != null && segment.Ranges.Any();
            bool hasNestedSegments = segment.NestedSegments != null && segment.NestedSegments.Any();

            if (!hasRanges && !hasNestedSegments)
            {
                throw new InvalidOperationException(
                    "Segment must have at least one range or nested segment");
            }

            // Validate all ranges
            if (segment.Ranges != null)
            {
                foreach (var range in segment.Ranges)
                {
                    if (range == null || !range.IsValid())
                    {
                        throw new InvalidOperationException(
                            $"Invalid line range: {range}");
                    }
                }
            }

            // Recursively validate nested segments
            if (segment.NestedSegments != null)
            {
                foreach (var nested in segment.NestedSegments)
                {
                    ValidateSegmentCommand(nested);
                }
            }
        }

        /// <summary>
        /// Creates a sample JSON response for testing purposes.
        /// This demonstrates the expected format from the AI.
        /// </summary>
        /// <returns>Sample JSON string</returns>
        public static string GetSampleResponse()
        {
            var sample = new SegmentCommand
            {
                FirstLine = "Start of document",
                Ranges = new List<LineRange>
                {
                    new LineRange { StartLine = 1, EndLine = 5 },
                    new LineRange { StartLine = 10, EndLine = 15 }
                },
                LastLine = "End of document",
                NestedSegments = new List<SegmentCommand>
                {
                    new SegmentCommand
                    {
                        FirstLine = "Nested section start",
                        Ranges = new List<LineRange>
                        {
                            new LineRange { StartLine = 20, EndLine = 25 }
                        },
                        LastLine = "Nested section end"
                    }
                }
            };

            return JsonConvert.SerializeObject(sample, Formatting.Indented);
        }
    }
}
