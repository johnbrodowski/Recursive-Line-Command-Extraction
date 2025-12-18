# RLCE Framework - Quick Start Guide

Get started with the Recursive Line Command Extraction framework in 5 minutes!

## Prerequisites

- .NET 10.0 SDK installed
- Text editor or IDE (Visual Studio, VS Code, Rider, etc.)

## Installation

```bash
# Clone or download the repository
cd "Recursive Line Command Extraction"

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the demo
dotnet run
```

## Your First RLCE Program

### 1. Create a sample file

Create `mydata.txt`:
```
Line 1: First item
Line 2: Important data
Line 3: More content
Line 4: Critical information
Line 5: Additional details
Line 6: Extra content
Line 7: Key findings
Line 8: More details
Line 9: Conclusion
Line 10: End
```

### 2. Write a simple extraction program

```csharp
using Recursive_Line_Command_Extraction.Models;
using Recursive_Line_Command_Extraction.Services;

// Step 1: Load and number your data
var sender = new DataSender();
sender.SaveNumberedData("mydata.txt", "numbered.txt");
Console.WriteLine("Created numbered file");

// Step 2: Create a segment command (normally this comes from AI)
var command = new SegmentCommand
{
    FirstLine = "=== Extracted Data ===",
    Ranges = new List<LineRange>
    {
        new LineRange { StartLine = 2, EndLine = 4 },
        new LineRange { StartLine = 7, EndLine = 9 }
    },
    LastLine = "=== End ==="
};

// Step 3: Reconstruct the data
var reconstructor = new DataReconstructor();
string result = reconstructor.Reconstruct(command, "mydata.txt");

// Step 4: See the results
Console.WriteLine(result);
File.WriteAllText("output.txt", result);
```

### 3. Run it

```bash
dotnet run
```

### 4. Check the output

Your `output.txt` will contain:
```
=== Extracted Data ===
Line 2: Important data
Line 3: More content
Line 4: Critical information
Line 7: Key findings
Line 8: More details
Line 9: Conclusion
=== End ===
```

## Understanding the Workflow

### The Three Main Steps

1. **Prepare** - Add line numbers to your file
   ```csharp
   var sender = new DataSender();
   string numbered = sender.PrepareDataWithLineNumbers("input.txt");
   ```

2. **Get Commands** - Receive extraction commands (from AI or manually created)
   ```csharp
   // From AI (when implemented):
   string json = await sender.SendToAIAsync(numbered, "Extract important sections");

   // Or manually for testing:
   var command = new SegmentCommand { /* ... */ };
   ```

3. **Reconstruct** - Extract the specified lines
   ```csharp
   var reconstructor = new DataReconstructor();
   string output = reconstructor.Reconstruct(command, "input.txt");
   ```

## Common Patterns

### Pattern 1: Extract Multiple Ranges

Extract lines 1-5 and 20-25:
```csharp
var command = new SegmentCommand
{
    Ranges = new List<LineRange>
    {
        new LineRange { StartLine = 1, EndLine = 5 },
        new LineRange { StartLine = 20, EndLine = 25 }
    }
};
```

### Pattern 2: Nested Extraction

Extract with hierarchical structure:
```csharp
var command = new SegmentCommand
{
    FirstLine = "=== Main ===",
    Ranges = new List<LineRange>
    {
        new LineRange { StartLine = 1, EndLine = 3 }
    },
    NestedSegments = new List<SegmentCommand>
    {
        new SegmentCommand
        {
            FirstLine = "  -- Subsection --",
            Ranges = new List<LineRange>
            {
                new LineRange { StartLine = 10, EndLine = 15 }
            }
        }
    },
    LastLine = "=== End ==="
};
```

### Pattern 3: Parse JSON Response

When you get JSON from an AI:
```csharp
string jsonFromAI = /* AI response */;
var parser = new AIResponseParser();
var command = parser.ParseResponse(jsonFromAI);
var reconstructor = new DataReconstructor();
string output = reconstructor.Reconstruct(command, "original.txt");
```

### Pattern 4: Streaming for Large Files

For files larger than 100MB:
```csharp
var sender = new DataSender();

// Stream to file
sender.PrepareDataWithLineNumbersStreaming("huge.txt", "numbered.txt");

// Or stream as enumerable
var lines = sender.PrepareDataWithLineNumbersStreamingEnumerable("huge.txt");
foreach (var line in lines.Take(100))
{
    Console.WriteLine(line);
}
```

## Next Steps

1. **See the full demo**: Run `dotnet run` to see all examples
2. **Check the examples**: Look in the `Examples/` folder for JSON samples
3. **Read the docs**: Open `README.md` for complete documentation
4. **Integrate AI**: Implement `SendToAIAsync()` in `DataSender.cs`

## Common Issues

### Issue: "File not found"
**Solution**: Use absolute paths or ensure your working directory is correct
```csharp
string fullPath = Path.GetFullPath("myfile.txt");
```

### Issue: "Invalid line range"
**Solution**: Ensure StartLine <= EndLine and both are > 0
```csharp
var range = new LineRange { StartLine = 5, EndLine = 10 };
if (!range.IsValid()) {
    Console.WriteLine("Invalid range!");
}
```

### Issue: "Line number not found"
**Solution**: Verify your line numbers match the original file
```csharp
var reconstructor = new DataReconstructor();
reconstructor.LoadOriginalFile("input.txt");
var stats = reconstructor.GetStats();
Console.WriteLine($"File has {stats["TotalLines"]} lines");
```

## Need Help?

- **Examples**: Check the `Examples/` folder
- **Full Documentation**: See `README.md`
- **Code Comments**: All classes are well-documented
- **Demo Program**: Run `dotnet run` to see working examples

## Pro Tips

1. **Reuse reconstructor**: Load the file once, reconstruct multiple times
   ```csharp
   var reconstructor = new DataReconstructor();
   reconstructor.LoadOriginalFile("data.txt");

   string result1 = reconstructor.Reconstruct(command1);
   string result2 = reconstructor.Reconstruct(command2);
   ```

2. **Validate before reconstructing**:
   ```csharp
   if (command.IsValid()) {
       var result = reconstructor.Reconstruct(command, "input.txt");
   }
   ```

3. **Use flexible parsing**:
   ```csharp
   // Handles both single segment and array
   var segments = parser.ParseFlexible(jsonResponse);
   ```

Happy extracting! 🚀
