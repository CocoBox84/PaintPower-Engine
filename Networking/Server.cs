using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace PaintPower.Networking;

// Networking class for the PaintPower Engine.
// Mainly to be used for the 'Coco xPaint Project', but it will lie
// here in the engine because it's open source. So anyone can create their own server.
public class Server
{
    //*--- Domain security. ---*//
    private static List<Domain> AllowedDomainsList = new List<Domain>();
    public void AllowDomain(Domain domain) => AllowedDomainsList.Add(domain);
    public bool IsDomainAllowed(Domain domain) => AllowedDomainsList.Contains(domain);

    public void RemoveDomain(Domain domain) { AllowedDomainsList.Remove(domain); }

    public Domain CurrentDomain = new Domain("paint-website.onrender.com");

    public void closeAllConnections() {
        AllowedDomainsList.Clear();
    }

    // Create, register, and add defualt domains.
    public void loadDefaultDomains() {
        
        // Clear old list
        AllowedDomainsList.Clear();

        // Create Coco links, Paint links, random links, and more!
        // Creating a custom server? Make a issue on Github and i'll add it here!
        Domain d1 = new Domain("xpaint.cocoink.ink");
        Domain d2 = new Domain("paint.cocoink.ink");
        Domain d3 = new Domain("127.0.0.1:5500/f/xPaint");
        Domain d4 = new Domain("127.0.0.1:5000/f/xPaint");
        Domain d5 = new Domain("127.0.0.1:3000/f/xPaint");
        Domain d6 = new Domain("127.0.0.1:8000");
        Domain d7 = new Domain("localhost:5500");
        Domain d8 = new Domain("localhost:5000");
        Domain d9 = new Domain("localhost:8000");
        Domain d10 = new Domain("localhost:3000");
        Domain d11 = new Domain("github.com");
        Domain d12 = new Domain("render.com");
        Domain d13 = new Domain("paint-website.onrender.com");
        Domain d14 = new Domain("paintpower.cocoink.ink");
        Domain d15 = new Domain("cocoink.ink");
        Domain d16 = new Domain("cocoink.ink/f/xPaint");
        Domain d17 = new Domain("cocoink.ink/f/Paint");
        Domain d18 = new Domain("cocoink.ink/f/PaintPower");
        Domain d19 = new Domain("negro.org");
        Domain d20 = new Domain("example.com");

        // Add to list
        AllowDomain(d1); AllowDomain(d2); AllowDomain(d3); AllowDomain(d4); AllowDomain(d5);
        AllowDomain(d6); AllowDomain(d7); AllowDomain(d8); AllowDomain(d9); AllowDomain(d10);
        AllowDomain(d11); AllowDomain(d12); AllowDomain(d13); AllowDomain(d14); AllowDomain(d15);
        AllowDomain(d16); AllowDomain(d17); AllowDomain(d18); AllowDomain(d19); AllowDomain(d20);

        setActiveDomain(d3);
    }

    public void setActiveDomain(Domain domain)
    {
        CurrentDomain = domain;
    }

    //*--- Networking ---*//
    public void InitServer()
    {
        loadDefaultDomains();
        checkConnection();
    }

    public bool checkConnection() { return true; }

    public async Task<string> GetFromServer()
    {
        var domain = CurrentDomain;
        if (domain == null) throw new ArgumentNullException(nameof(domain));
        if (!IsDomainAllowed(domain)) throw new UnauthorizedAccessException("Domain not allowed");
        return await Net.PerformGetRequest(URLifyer.URLify(domain));
    }

    public Server() {
    }
}