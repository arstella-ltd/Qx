using System.Collections.Generic;

namespace Qx.Models;

internal class LicenseInfo
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string License { get; set; } = string.Empty;
    public Dictionary<string, string> Dependencies { get; set; } = new();
}

internal class LicenseData
{
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string License { get; set; } = string.Empty;
    public string ProjectUrl { get; set; } = string.Empty;
}
