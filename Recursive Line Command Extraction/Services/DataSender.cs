using System.Text;

namespace Recursive_Line_Command_Extraction.Services
{
    /// <summary>
    /// Handles preparing data for sending to the AI by adding line numbers.
    /// Also provides methods to send data to AI (with placeholder for actual API integration).
    /// </summary>
    public class DataSender
    {
        /// <summary>
        /// Adds line numbers to each line of the input file.
        /// Format: "LineNumber: Content"
        /// </summary>
        /// <param name="inputFilePath">Path to the input file</param>
        /// <returns>String with numbered lines</returns>
        public string PrepareDataWithLineNumbers(string inputFilePath)
        {
            if (!File.Exists(inputFilePath))
            {
                throw new FileNotFoundException($"Input file not found: {inputFilePath}");
            }

            var lines = File.ReadAllLines(inputFilePath);
            var numberedLines = new StringBuilder();

            for (int i = 0; i < lines.Length; i++)
            {
                numberedLines.AppendLine($"{i + 1}: {lines[i]}");
            }

            return numberedLines.ToString();
        }

        /// <summary>
        /// Saves the numbered data to a file for inspection or sending to AI.
        /// </summary>
        /// <param name="inputFilePath">Path to the input file</param>
        /// <param name="outputFilePath">Path where numbered output should be saved</param>
        public void SaveNumberedData(string inputFilePath, string outputFilePath)
        {
            var numberedData = PrepareDataWithLineNumbers(inputFilePath);
            File.WriteAllText(outputFilePath, numberedData);
        }

        /// <summary>
        /// Streaming approach for very large files without loading all lines into memory.
        /// This method processes the file line by line and writes numbered output directly.
        /// </summary>
        /// <param name="inputFilePath">Path to the input file</param>
        /// <param name="outputFilePath">Path where numbered output should be saved</param>
        public void PrepareDataWithLineNumbersStreaming(string inputFilePath, string outputFilePath)
        {
            if (!File.Exists(inputFilePath))
            {
                throw new FileNotFoundException($"Input file not found: {inputFilePath}");
            }

            using var reader = new StreamReader(inputFilePath);
            using var writer = new StreamWriter(outputFilePath);

            int lineNumber = 1;
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                writer.WriteLine($"{lineNumber}: {line}");
                lineNumber++;
            }
        }

        /// <summary>
        /// Streaming approach that returns an enumerable for processing large files
        /// without loading everything into memory.
        /// </summary>
        /// <param name="inputFilePath">Path to the input file</param>
        /// <returns>Enumerable of numbered lines</returns>
        public IEnumerable<string> PrepareDataWithLineNumbersStreamingEnumerable(string inputFilePath)
        {
            if (!File.Exists(inputFilePath))
            {
                throw new FileNotFoundException($"Input file not found: {inputFilePath}");
            }

            return PrepareDataIterator(inputFilePath);
        }

        private static IEnumerable<string> PrepareDataIterator(string inputFilePath)
        {
            using var reader = new StreamReader(inputFilePath);
            int lineNumber = 1;
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                yield return $"{lineNumber}: {line}";
                lineNumber++;
            }
        }

        /// <summary>
        /// Placeholder method for sending numbered data to AI.
        /// In a real implementation, this would make an API call to an AI service.
        /// </summary>
        /// <param name="numberedData">The data with line numbers</param>
        /// <param name="prompt">Optional prompt to send to the AI</param>
        /// <returns>JSON response from AI containing segment commands</returns>
        public async Task<string> SendToAIAsync(string numberedData, string? prompt = null)
        {
            // TODO: Implement actual AI API integration
            // This is a placeholder that would be replaced with actual API calls
            // For example, using OpenAI API, Anthropic Claude API, etc.

            await Task.Delay(100); // Simulate API call delay

            throw new NotImplementedException(
                "This method should be implemented with actual AI API integration. " +
                "The AI should analyze the numbered data and return JSON segment commands.");

            // Example implementation structure:
            /*
            var client = new HttpClient();
            var requestBody = new
            {
                model = "gpt-4",
                messages = new[]
                {
                    new { role = "system", content = "You are a data extraction assistant..." },
                    new { role = "user", content = prompt ?? "Extract relevant segments..." },
                    new { role = "user", content = numberedData }
                }
            };

            var response = await client.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);
            var result = await response.Content.ReadAsStringAsync();
            return result;
            */
        }

        /// <summary>
        /// Sends data from a file to AI with automatic numbering.
        /// </summary>
        /// <param name="inputFilePath">Path to the input file</param>
        /// <param name="prompt">Optional prompt to send to the AI</param>
        /// <returns>JSON response from AI</returns>
        public async Task<string> SendFileToAIAsync(string inputFilePath, string? prompt = null)
        {
            var numberedData = PrepareDataWithLineNumbers(inputFilePath);
            return await SendToAIAsync(numberedData, prompt);
        }
    }
}
