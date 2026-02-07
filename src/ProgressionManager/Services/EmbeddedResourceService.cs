using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using ProgressionManager.Services.Interfaces;

namespace ProgressionManager.Services;

public class EmbeddedResourceService : IEmbeddedResourceService
{
    private readonly Assembly _assembly = Assembly.GetExecutingAssembly();
    private readonly string _resourcePrefix = "ProgressionManager.Data.";

    public string LoadResourceAsString(string resourceName)
    {
        var fullResourceName = _resourcePrefix + resourceName;
        using var stream = _assembly.GetManifestResourceStream(fullResourceName);

        if (stream == null)
        {
            throw new InvalidOperationException($"Could not find embedded resource: {resourceName}");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public T? LoadResourceAsJson<T>(string resourceName, JsonSerializerOptions? options = null)
    {
        var json = LoadResourceAsString(resourceName);

        options ??= new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<T>(json, options);
    }
}

