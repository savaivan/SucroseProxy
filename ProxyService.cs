using System.Net;
using System.Net.Security;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;

namespace SucroseProxy;
internal class ProxyService
{
    string[] address = File.ReadAllLines("./address.txt");

    private static readonly string[] s_redirectDomains =
    {
        ".yuanshen.com",
        ".hoyoverse.com",
        ".mihoyo.com",
        ".yuanshen.com:12401",
        ".zenlesszonezero.com",
        ".honkaiimpact3.com",
        ".bhsr.com",
        ".starrails.com"
    };

    private readonly ProxyServer _server;

    public ProxyService()
    {
        _server = new ProxyServer();
        _server.CertificateManager.EnsureRootCertificate();

        _server.BeforeRequest += BeforeRequest;
        _server.ServerCertificateValidationCallback += OnCertValidation;

        ExplicitProxyEndPoint endPoint = new(IPAddress.Any, 8080, true);
        endPoint.BeforeTunnelConnectRequest += BeforeTunnelConnectRequest;

        _server.AddEndPoint(endPoint);
        _server.Start();

        _server.SetAsSystemHttpProxy(endPoint);
        _server.SetAsSystemHttpsProxy(endPoint);
    }

    public void Shutdown()
    {
        _server.Stop();
        _server.Dispose();
    }

    private Task BeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs args)
    {
        string hostname = args.HttpClient.Request.RequestUri.Host;
        args.DecryptSsl = ShouldRedirect(hostname);

        return Task.CompletedTask;
    }

    private Task OnCertValidation(object sender, CertificateValidationEventArgs args)
    {
        if (args.SslPolicyErrors == SslPolicyErrors.None)
            args.IsValid = true;

        return Task.CompletedTask;
    }

    private Task BeforeRequest(object sender, SessionEventArgs args)
    {
        string hostname = args.HttpClient.Request.RequestUri.Host;

        if (ShouldRedirect(hostname))
        {
            string requestUrl = args.HttpClient.Request.Url;
            Uri local = new($"http://" + address[0] + "/");

            string replacedUrl = new UriBuilder(requestUrl)
            {
                Scheme = local.Scheme,
                Host = local.Host,
                Port = local.Port
            }.Uri.ToString();
            
            args.HttpClient.Request.Url = replacedUrl;
			
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nRedirecting");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(requestUrl);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("=>");

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(replacedUrl);
        }

        return Task.CompletedTask;
    }

    private static bool ShouldRedirect(string hostname)
    {
        foreach (string domain in s_redirectDomains)
        {
            if (hostname.EndsWith(domain))
                return true;
        }

        return false;
    }
}
