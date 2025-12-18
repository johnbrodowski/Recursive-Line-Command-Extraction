using Recursive_Line_Command_Extraction.Models;
using Recursive_Line_Command_Extraction.Services;

namespace Recursive_Line_Command_Extraction
{
    /// <summary>
    /// Demonstration of the Recursive Line Command Extraction (RLCE) Framework.
    ///
    /// This framework enables efficient data extraction by:
    /// 1. Sending large text files to an AI with line numbers
    /// 2. Receiving back commands describing which lines to extract
    /// 3. Reconstructing the data from those commands
    ///
    /// The AI returns commands instead of full data, which can be more efficient
    /// for large files or when only specific sections are needed.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Recursive Line Command Extraction (RLCE) Framework Demo ===\n");

            try
            {
                // Run the demonstration
                RunBasicExample();
                RunNestedExample();
                RunStreamingExample();

                Console.WriteLine("\n=== All demonstrations completed successfully! ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Demonstrates basic usage of the RLCE framework with a simple file.
        /// </summary>
        static void RunBasicExample()
        {
            Console.WriteLine("--- Basic Example ---");

            // Step 1: Create sample input file
            string inputFile = "sample_input.txt";
            CreateSampleInputFile(inputFile);
            Console.WriteLine($"✓ Created sample input file: {inputFile}");

            // Step 2: Prepare data with line numbers
            var sender = new DataSender();
            string numberedData = sender.PrepareDataWithLineNumbers(inputFile);
            Console.WriteLine("\n✓ Prepared data with line numbers:");
            Console.WriteLine(numberedData);

            // Save numbered data for inspection
            string numberedFile = "numbered_output.txt";
            sender.SaveNumberedData(inputFile, numberedFile);
            Console.WriteLine($"✓ Saved numbered data to: {numberedFile}");

            // Step 3: Simulate AI response (in real usage, this comes from AI API)
            string aiResponse = CreateSampleAIResponse();
            Console.WriteLine("\n✓ Sample AI response (JSON):");
            Console.WriteLine(aiResponse);

            // Step 4: Parse AI response
            var parser = new AIResponseParser();
            var segmentCommand = parser.ParseResponse(aiResponse);
            Console.WriteLine("\n✓ Parsed AI response into SegmentCommand");

            // Step 5: Reconstruct data from commands
            var reconstructor = new DataReconstructor();
            string reconstructed = reconstructor.Reconstruct(segmentCommand, inputFile);
            Console.WriteLine("\n✓ Reconstructed data:");
            Console.WriteLine(reconstructed);

            // Step 6: Save reconstructed data
            string outputFile = "reconstructed_output.txt";
            File.WriteAllText(outputFile, reconstructed);
            Console.WriteLine($"✓ Saved reconstructed data to: {outputFile}\n");
        }

        /// <summary>
        /// Demonstrates nested segment handling.
        /// </summary>
        static void RunNestedExample()
        {
            Console.WriteLine("--- Nested Segments Example ---");

            // Create a more complex input file
            string inputFile = "nested_sample.txt";
            CreateNestedSampleInputFile(inputFile);
            Console.WriteLine($"✓ Created nested sample file: {inputFile}");

            // Create nested segment command
            var nestedCommand = new SegmentCommand
            {
                FirstLine = "=== Document Start ===",
                Ranges = new List<LineRange>
                {
                    new LineRange { StartLine = 1, EndLine = 3 }
                },
                NestedSegments = new List<SegmentCommand>
                {
                    new SegmentCommand
                    {
                        FirstLine = "  --- Nested Section ---",
                        Ranges = new List<LineRange>
                        {
                            new LineRange { StartLine = 5, EndLine = 7 }
                        },
                        LastLine = "  --- End Nested Section ---"
                    }
                },
                LastLine = "=== Document End ==="
            };

            // Reconstruct with nested segments
            var reconstructor = new DataReconstructor();
            string reconstructed = reconstructor.Reconstruct(nestedCommand, inputFile);
            Console.WriteLine("\n✓ Reconstructed data with nested segments:");
            Console.WriteLine(reconstructed);

            // Save result
            string outputFile = "nested_output.txt";
            File.WriteAllText(outputFile, reconstructed);
            Console.WriteLine($"✓ Saved to: {outputFile}\n");
        }

        /// <summary>
        /// Demonstrates streaming approach for large files.
        /// </summary>
        static void RunStreamingExample()
        {
            Console.WriteLine("--- Streaming Example (for large files) ---");

            // Create a larger sample file
            string inputFile = "large_sample.txt";
            CreateLargeSampleFile(inputFile, 1000); // Create file with 1000 lines
            Console.WriteLine($"✓ Created large sample file: {inputFile} (1000 lines)");

            // Use streaming approach to add line numbers
            var sender = new DataSender();
            string numberedFile = "large_numbered.txt";
            sender.PrepareDataWithLineNumbersStreaming(inputFile, numberedFile);
            Console.WriteLine($"✓ Processed large file with streaming approach");
            Console.WriteLine($"✓ Saved numbered data to: {numberedFile}");

            // Demonstrate enumerable streaming (doesn't create intermediate file)
            Console.WriteLine("\n✓ Using enumerable streaming (no intermediate file):");
            var numberedLines = sender.PrepareDataWithLineNumbersStreamingEnumerable(inputFile);
            var firstTenLines = numberedLines.Take(10).ToList();
            Console.WriteLine("First 10 lines:");
            foreach (var line in firstTenLines)
            {
                Console.WriteLine(line);
            }

            Console.WriteLine("\n✓ Streaming approach completed successfully\n");
        }

        /// <summary>
        /// Creates a simple sample input file for demonstration.
        /// </summary>
        static void CreateSampleInputFile(string filePath)
        {
            var content = @"This is line 1 of the sample file.
Line 2 contains some important data.
Line 3 has more information.
Line 4 is part of the content.
Line 5 continues the data.
Line 6 contains additional details.
Line 7 has relevant information.
Line 8 is near the end.
Line 9 is the penultimate line.
Line 10 is the last line of the file.";

            File.WriteAllText(filePath, content);
        }

        /// <summary>
        /// Creates a sample file for nested segment demonstration.
        /// </summary>
        static void CreateNestedSampleInputFile(string filePath)
        {
            var content = @"Header line 1
Header line 2
Header line 3
Separator
Nested content line 1
Nested content line 2
Nested content line 3
Separator
Footer line 1
Footer line 2";

            File.WriteAllText(filePath, content);
        }

        /// <summary>
        /// Creates a large sample file for streaming demonstration.
        /// </summary>
        static void CreateLargeSampleFile(string filePath, int lineCount)
        {
            using var writer = new StreamWriter(filePath);
            for (int i = 1; i <= lineCount; i++)
            {
                writer.WriteLine($"This is line {i} of the large sample file with some data.");
            }
        }

        /// <summary>
        /// Creates a sample AI response in JSON format.
        /// In real usage, this would come from an actual AI API call.
        /// </summary>
        static string CreateSampleAIResponse()
        {
            var segment = new SegmentCommand
            {
                FirstLine = "=== Extracted Content ===",
                Ranges = new List<LineRange>
                {
                    new LineRange { StartLine = 2, EndLine = 4 },
                    new LineRange { StartLine = 7, EndLine = 9 }
                },
                LastLine = "=== End of Extracted Content ==="
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(segment, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// Example of how to integrate with an actual AI API (not implemented).
        /// This shows the intended workflow when connected to a real AI service.
        /// </summary>
        static async Task RunWithRealAIExample()
        {
            Console.WriteLine("--- Real AI Integration Example (Template) ---");

            string inputFile = "sample_input.txt";
            var sender = new DataSender();

            // Prepare prompt for AI
            string prompt = @"Please analyze this document and extract the most important sections.
Return your response as a JSON SegmentCommand with the following structure:
- FirstLine: A header describing the extraction
- Ranges: Array of {StartLine, EndLine} objects referencing important line ranges
- LastLine: A footer for the extraction
- NestedSegments: (optional) Nested segment commands for hierarchical extraction

Only include line ranges that contain important information.";

            try
            {
                // This would make an actual API call to an AI service
                // string aiResponse = await sender.SendFileToAIAsync(inputFile, prompt);

                // For now, we simulate with a placeholder
                Console.WriteLine("Note: Real AI integration requires API key and endpoint configuration.");
                Console.WriteLine("See DataSender.SendToAIAsync() method for integration details.");
            }
            catch (NotImplementedException)
            {
                Console.WriteLine("AI API integration not yet implemented.");
            }
        }
    }
}
