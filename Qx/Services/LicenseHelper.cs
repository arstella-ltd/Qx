using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Qx.Models;

namespace Qx.Services;

internal interface ILicenseHelper
{
    Task<Dictionary<string, LicenseData>> GetLicenseInfoAsync();
    Task<LicenseInfo> GetVersionInfoAsync();
    Task GenerateThirdPartyNoticesAsync(string filePath);
}

internal class LicenseHelper : ILicenseHelper
{
    private static Dictionary<string, LicenseData>? _cachedLicenses;
    private static LicenseInfo? _cachedVersionInfo;

    private static readonly Dictionary<string, LicenseData> _embeddedLicenses = new()
    {
        ["Qx"] = new LicenseData
        {
            Name = "Qx",
            Version = GetAssemblyVersion(),
            License = "MIT License\n\nCopyright (c) 2025 Arstella Ltd.\n\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.",
            ProjectUrl = "https://github.com/lifeast/Qx"
        },
        ["OpenAI"] = new LicenseData
        {
            Name = "OpenAI",
            Version = "2.3.0",
            License = "MIT License\n\nCopyright (c) 2024 Roger Pincombe\n\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.",
            ProjectUrl = "https://github.com/openai/openai-dotnet"
        },
        ["System.CommandLine"] = new LicenseData
        {
            Name = "System.CommandLine",
            Version = "2.0.0-beta6.25358.103",
            License = "MIT License\n\nCopyright (c) .NET Foundation and Contributors\n\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.",
            ProjectUrl = "https://github.com/dotnet/command-line-api"
        },
        ["Microsoft.Extensions.Configuration"] = new LicenseData
        {
            Name = "Microsoft.Extensions.Configuration",
            Version = "9.0.7",
            License = "MIT License\n\nCopyright (c) .NET Foundation and Contributors\n\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.",
            ProjectUrl = "https://github.com/dotnet/runtime"
        },
        ["Microsoft.Extensions.Configuration.EnvironmentVariables"] = new LicenseData
        {
            Name = "Microsoft.Extensions.Configuration.EnvironmentVariables",
            Version = "9.0.7",
            License = "MIT License\n\nCopyright (c) .NET Foundation and Contributors\n\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.",
            ProjectUrl = "https://github.com/dotnet/runtime"
        },
        ["Microsoft.Extensions.Configuration.Json"] = new LicenseData
        {
            Name = "Microsoft.Extensions.Configuration.Json",
            Version = "9.0.7",
            License = "MIT License\n\nCopyright (c) .NET Foundation and Contributors\n\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.",
            ProjectUrl = "https://github.com/dotnet/runtime"
        },
        ["Microsoft.Extensions.DependencyInjection"] = new LicenseData
        {
            Name = "Microsoft.Extensions.DependencyInjection",
            Version = "9.0.7",
            License = "MIT License\n\nCopyright (c) .NET Foundation and Contributors\n\nPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:\n\nThe above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.\n\nTHE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.",
            ProjectUrl = "https://github.com/dotnet/runtime"
        }
    };

    public Task<Dictionary<string, LicenseData>> GetLicenseInfoAsync()
    {
        _cachedLicenses ??= new Dictionary<string, LicenseData>(_embeddedLicenses);
        return Task.FromResult(_cachedLicenses);
    }

    public Task<LicenseInfo> GetVersionInfoAsync()
    {
        if (_cachedVersionInfo == null)
        {
            _cachedVersionInfo = new LicenseInfo
            {
                Name = "Qx",
                Version = GetAssemblyVersion(),
                License = "MIT",
                Dependencies = GetDetectedDependencies()
            };
        }

        return Task.FromResult(_cachedVersionInfo);
    }

    public async Task GenerateThirdPartyNoticesAsync(string filePath)
    {
        string content = GenerateNoticesContent();
        await File.WriteAllTextAsync(filePath, content).ConfigureAwait(false);
    }

    private static string GetAssemblyVersion()
    {
        try
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version? version = assembly.GetName().Version;
            return version?.ToString(3) ?? "0.1.0";
        }
        catch (NotSupportedException)
        {
            // Can occur in certain runtime environments
            return "0.1.0";
        }
        catch (System.Security.SecurityException)
        {
            // Can occur if permission to access assembly info is denied
            return "0.1.0";
        }
    }

    private static Dictionary<string, string> GetDetectedDependencies()
    {
        try
        {
            System.Reflection.Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            Dictionary<string, string> dependencies = new Dictionary<string, string>();

            Dictionary<string, string> knownDependencies = new Dictionary<string, string>
            {
                { "OpenAI", "MIT" },
                { "System.CommandLine", "MIT" },
                { "Microsoft.Extensions.Configuration", "MIT" },
                { "Microsoft.Extensions.Configuration.EnvironmentVariables", "MIT" },
                { "Microsoft.Extensions.Configuration.Json", "MIT" },
                { "Microsoft.Extensions.DependencyInjection", "MIT" }
            };

            foreach (System.Reflection.Assembly assembly in loadedAssemblies)
            {
                string? name = assembly.GetName().Name;
                if (name != null && knownDependencies.TryGetValue(name, out string? value))
                {
                    dependencies[name] = value;
                }
            }

            foreach (KeyValuePair<string, string> dep in knownDependencies)
            {
                dependencies[dep.Key] = dep.Value;
            }

            return dependencies;
        }
        catch (NotSupportedException)
        {
            // Can occur in certain runtime environments
            return GetDefaultDependencies();
        }
        catch (System.Security.SecurityException)
        {
            // Can occur if permission to access assembly info is denied
            return GetDefaultDependencies();
        }
    }

    private static Dictionary<string, string> GetDefaultDependencies()
    {
        return new Dictionary<string, string>
        {
            { "OpenAI", "MIT" },
            { "System.CommandLine", "MIT" },
            { "Microsoft.Extensions.Configuration", "MIT" },
            { "Microsoft.Extensions.Configuration.EnvironmentVariables", "MIT" },
            { "Microsoft.Extensions.Configuration.Json", "MIT" },
            { "Microsoft.Extensions.DependencyInjection", "MIT" }
        };
    }

    private static string GenerateNoticesContent()
    {
        string content = "THIRD-PARTY SOFTWARE NOTICES AND INFORMATION\n";
        content += "==============================================\n\n";
        content += "This file contains third-party software notices and/or additional terms for licensed third-party software components included within Qx.\n\n";

        foreach (KeyValuePair<string, LicenseData> license in _embeddedLicenses.Where(l => l.Key != "Qx"))
        {
            content += $"-------------------------------------------------------------------------------\n";
            content += $"{license.Value.Name} v{license.Value.Version}\n";
            content += $"Project: {license.Value.ProjectUrl}\n";
            content += $"-------------------------------------------------------------------------------\n";
            content += $"{license.Value.License}\n\n";
        }

        return content;
    }
}
