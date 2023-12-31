using SucroseImpact.Proxy;

Console.Title = "SucroseImpact | Proxy";
Console.BackgroundColor = ConsoleColor.Black;
Console.ForegroundColor = ConsoleColor.White;

string[] address = File.ReadAllLines("./address.txt");
if (String.IsNullOrEmpty(address[0]) || String.IsNullOrWhiteSpace(address[0]) || address[0].Any(Char.IsWhiteSpace)) {
    System.Environment.Exit(-1);
}

ProxyService service = new();
AppDomain.CurrentDomain.ProcessExit += (_, _) =>
{
    Console.WriteLine("Shutting down...");
    service.Shutdown();
};

Thread.Sleep(-1);