using BoardManager;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using TestWrapper;

const string DATASET = "PLANE ALTITUDE";

// Create a Dummy Board
Board testBoard1 = new()
{
    Name = "Test",
    OperatingSystem = OperatingSystems.ARDUINO.ToString(),
    Outputs = new() { "PLANE ALTITUDE" },

};

// Initialize the board
Boards bd = new();
string result = bd.Add(testBoard1);

// Get the Port we have allocatated.
Dictionary<string, string>? data = JsonSerializer.Deserialize<Dictionary<string, string>>(result);
if (data == null)
{
    Console.WriteLine("Wrapper did not pick up a Port");
    Environment.Exit(1);
}

Console.WriteLine("Connection");

// Loop until end read and write data
for ( int i = 0; i < 10000 ; i++)
{
    // Set the output data
    // Dictionary<string, string> tempdata = new() { { DATASET, $"{i}" } };
    Dictionary<string, string> tempdata = new() { { DATASET, "1" } };

    bd.SetOutputData(tempdata);

    // Get the input data
    foreach( var b in bd.GetChangedInputData() )
    {
        Console.WriteLine( b.Key+ ": " + b.Value ); 
    }
    // Hang about a little
    Thread.Sleep(1000) ;
}
