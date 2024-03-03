using SucroseImpactProxy;

Console.Title = "SucroseImpact | Proxy";
Console.BackgroundColor = ConsoleColor.Black;
Console.ForegroundColor = ConsoleColor.White;

string address = File.ReadAllText("./address.txt");
if (String.IsNullOrEmpty(address) || String.IsNullOrWhiteSpace(address) || address.Any(Char.IsWhiteSpace))
{
    Console.ForegroundColor = ConsoleColor.Red;
    throw new Exception("Invalid address!");
}

ProxyService service = new();
AppDomain.CurrentDomain.ProcessExit += (_, _) =>
{
    Console.WriteLine("Shutting down...");
    service.Shutdown();
};

Thread.Sleep(-1);
