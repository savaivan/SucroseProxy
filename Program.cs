using SucroseProxy;

Console.Title = "SucroseImpact | Proxy";
Console.ResetColor();

if (File.Exists("./address.txt"))
{
    string mentallilness = File.ReadAllText("./address.txt");
    if (String.IsNullOrEmpty(mentallilness) || String.IsNullOrWhiteSpace(mentallilness))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        throw new Exception("Invalid address!");
    }

    string[] address = File.ReadAllLines("./address.txt");
    if (String.IsNullOrEmpty(address[0]) || String.IsNullOrWhiteSpace(address[0]) || address[0].Any(Char.IsWhiteSpace))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        throw new Exception("Invalid address!");
    }

    try
    {
        Uri url = new($"http://" + address[0] + "/");
    }
    catch
    {
        Console.ForegroundColor = ConsoleColor.Red;
        throw;
    }
}
else
{
    StreamWriter addressfile = new StreamWriter("./address.txt");
    addressfile.WriteLine("127.0.0.1:21000");
    addressfile.Close();
    addressfile.Dispose();
}

ProxyService service = new();

AppDomain.CurrentDomain.ProcessExit += (_, _) =>
{
    Console.WriteLine("Shutting down...");
    service.Shutdown();
};

Thread.Sleep(-1);
