# Recursive Line Command Extraction (RLCE) Framework

A novel C# framework for efficient data extraction from large text files using AI-powered line command extraction.

## Overview

The RLCE framework enables a unique approach to working with large text files:

1. **Send** large text files to an AI with line numbers
2. **Receive** back commands describing which lines to extract (not the full data)
3. **Reconstruct** the selected data from those commands

This approach can be more efficient than having the AI return full data, especially for:
- Very large files where only specific sections are needed
- Scenarios where network bandwidth is limited
- Cases where you want to preserve exact original formatting

## Features

✅ **Modular Design**: Separate classes for data preparation, parsing, and reconstruction
✅ **Recursive Segments**: Support for nested/hierarchical data extraction
✅ **Streaming Support**: Process large files without loading everything into memory
✅ **JSON-Based**: Uses standard JSON format for AI responses
✅ **Production-Ready**: Comprehensive error handling and validation
✅ **Well-Documented**: Extensive code comments and examples

## Architecture

### Core Components

#### 1. **Models** (`Models/`)
- `LineRange.cs`: Represents a range of lines (StartLine, EndLine)
- `SegmentCommand.cs`: Contains extraction commands (FirstLine, Ranges, LastLine, NestedSegments)

#### 2. **Services** (`Services/`)
- `DataSender.cs`: Prepares data with line numbers, handles AI communication
- `AIResponseParser.cs`: Parses JSON responses into SegmentCommand objects
- `DataReconstructor.cs`: Recursively reconstructs data from commands

### Workflow

```
┌─────────────────┐
│  Original File  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  DataSender     │  Adds line numbers: "1: Content..."
│  .PrepareData() │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   Send to AI    │  AI analyzes and returns JSON commands
│   (API Call)    │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ AIResponseParser│  Parses JSON into SegmentCommand objects
│  .ParseResponse()│
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│DataReconstructor│  Extracts referenced lines from original
│  .Reconstruct() │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Reconstructed   │
│     Output      │
└─────────────────┘
```

## Installation

### Prerequisites
- .NET 10.0 or later
- Newtonsoft.Json 13.0.3 (installed via NuGet)

### Building the Project

```bash
cd "Recursive Line Command Extraction"
dotnet restore
dotnet build
```

### Running the Demo

```bash
dotnet run
```

## Usage Examples

### Basic Usage

```csharp
using Recursive_Line_Command_Extraction.Models;
using Recursive_Line_Command_Extraction.Services;

// Step 1: Prepare data with line numbers
var sender = new DataSender();
string numberedData = sender.PrepareDataWithLineNumbers("input.txt");

// Step 2: Send to AI (implement your AI integration here)
// string aiResponse = await sender.SendToAIAsync(numberedData);

// For demonstration, use a sample response:
string aiResponse = @"{
    ""FirstLine"": ""=== Important Sections ==="",
    ""Ranges"": [
        { ""StartLine"": 5, ""EndLine"": 10 },
        { ""StartLine"": 25, ""EndLine"": 30 }
    ],
    ""LastLine"": ""=== End ===""
}";

// Step 3: Parse AI response
var parser = new AIResponseParser();
var segment = parser.ParseResponse(aiResponse);

// Step 4: Reconstruct data
var reconstructor = new DataReconstructor();
string result = reconstructor.Reconstruct(segment, "input.txt");

// Step 5: Use or save the result
File.WriteAllText("output.txt", result);
```

### Nested Segments Example

```csharp
var nestedCommand = new SegmentCommand
{
    FirstLine = "=== Main Section ===",
    Ranges = new List<LineRange>
    {
        new LineRange { StartLine = 1, EndLine = 5 }
    },
    NestedSegments = new List<SegmentCommand>
    {
        new SegmentCommand
        {
            FirstLine = "  -- Subsection --",
            Ranges = new List<LineRange>
            {
                new LineRange { StartLine = 10, EndLine = 15 }
            },
            LastLine = "  -- End Subsection --"
        }
    },
    LastLine = "=== End Main Section ==="
};

var reconstructor = new DataReconstructor();
string result = reconstructor.Reconstruct(nestedCommand, "input.txt");
```

### Streaming for Large Files

```csharp
var sender = new DataSender();

// Option 1: Stream to file (no memory overhead)
sender.PrepareDataWithLineNumbersStreaming("huge_file.txt", "numbered.txt");

// Option 2: Stream as enumerable (process on-the-fly)
var numberedLines = sender.PrepareDataWithLineNumbersStreamingEnumerable("huge_file.txt");
foreach (var line in numberedLines)
{
    // Process each line as it's read
    await SendLineToAI(line);
}
```

## JSON Response Format

The AI should return JSON in this format:

### Simple Segment
```json
{
  "FirstLine": "Optional header text",
  "Ranges": [
    { "StartLine": 1, "EndLine": 5 },
    { "StartLine": 10, "EndLine": 15 }
  ],
  "LastLine": "Optional footer text"
}
```

### Nested Segments
```json
{
  "FirstLine": "=== Main Document ===",
  "Ranges": [
    { "StartLine": 1, "EndLine": 3 }
  ],
  "NestedSegments": [
    {
      "FirstLine": "--- Subsection ---",
      "Ranges": [
        { "StartLine": 10, "EndLine": 20 }
      ],
      "LastLine": "--- End Subsection ---"
    }
  ],
  "LastLine": "=== End Document ==="
}
```

### Multiple Segments
```json
[
  {
    "FirstLine": "First segment",
    "Ranges": [{ "StartLine": 1, "EndLine": 10 }],
    "LastLine": "End first"
  },
  {
    "FirstLine": "Second segment",
    "Ranges": [{ "StartLine": 20, "EndLine": 30 }],
    "LastLine": "End second"
  }
]
```

## AI Integration

To integrate with an actual AI service, implement the `SendToAIAsync` method in `DataSender.cs`:

```csharp
public async Task<string> SendToAIAsync(string numberedData, string? prompt = null)
{
    // Example with OpenAI
    var client = new HttpClient();
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

    var request = new
    {
        model = "gpt-4",
        messages = new[]
        {
            new {
                role = "system",
                content = "You are a data extraction assistant. Return SegmentCommand JSON."
            },
            new {
                role = "user",
                content = $"{prompt}\n\n{numberedData}"
            }
        }
    };

    var response = await client.PostAsJsonAsync(
        "https://api.openai.com/v1/chat/completions",
        request
    );

    return await response.Content.ReadAsStringAsync();
}
```

## API Reference

### DataSender

| Method | Description |
|--------|-------------|
| `PrepareDataWithLineNumbers(string path)` | Adds line numbers to file content |
| `SaveNumberedData(string input, string output)` | Saves numbered data to file |
| `PrepareDataWithLineNumbersStreaming(...)` | Streaming version for large files |
| `SendToAIAsync(string data, string? prompt)` | Sends data to AI (requires implementation) |

### AIResponseParser

| Method | Description |
|--------|-------------|
| `ParseResponse(string json)` | Parses single SegmentCommand from JSON |
| `ParseMultipleSegments(string json)` | Parses array of SegmentCommands |
| `ParseFlexible(string json)` | Auto-detects single or multiple segments |

### DataReconstructor

| Method | Description |
|--------|-------------|
| `LoadOriginalFile(string path)` | Loads file into memory for reconstruction |
| `Reconstruct(SegmentCommand, string? path)` | Reconstructs data from command |
| `ReconstructMultiple(List<SegmentCommand>, string? path)` | Reconstructs multiple segments |
| `ReconstructToFile(...)` | Reconstructs and saves to file |

## Use Cases

### 1. **Document Summarization**
Send a large document to AI, receive back commands for the most important sections, reconstruct summary.

### 2. **Code Extraction**
Extract specific functions or classes from large codebases based on AI analysis.

### 3. **Log Analysis**
Identify and extract relevant log entries from massive log files.

### 4. **Data Cleaning**
AI identifies clean/valid data sections, returns commands to extract only those.

### 5. **Hierarchical Document Processing**
Process documents with nested structure (chapters → sections → paragraphs).

## Performance Considerations

- **Memory**: Use streaming methods for files > 100MB
- **Line Numbers**: 1-based indexing matches human-readable format
- **Caching**: DataReconstructor caches loaded file for multiple reconstructions
- **Validation**: All ranges and segments are validated before processing

## Error Handling

The framework includes comprehensive error handling:

```csharp
try
{
    var reconstructor = new DataReconstructor();
    var result = reconstructor.Reconstruct(segment, "input.txt");
}
catch (FileNotFoundException ex)
{
    // Handle missing file
}
catch (InvalidOperationException ex)
{
    // Handle invalid segment commands or line ranges
}
catch (ArgumentException ex)
{
    // Handle invalid arguments
}
```

## Testing

The included `Program.cs` demonstrates three test scenarios:

1. **Basic Example**: Simple line range extraction
2. **Nested Example**: Hierarchical segment processing
3. **Streaming Example**: Large file handling (1000+ lines)

Run all tests with:
```bash
dotnet run
```

## Contributing

Contributions are welcome! Areas for enhancement:

- Additional AI service integrations (OpenAI, Anthropic, Azure, etc.)
- Performance optimizations
- Additional output formats
- CLI interface
- Unit tests

## License

See LICENSE.txt for details.

## Support

For issues or questions, please file an issue on the project repository.

---

**Created with Claude Code** - A novel approach to efficient AI-powered data extraction