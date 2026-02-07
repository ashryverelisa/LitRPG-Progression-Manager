using System.Text.Json;

namespace ProgressionManager.Services.Interfaces;

public interface IEmbeddedResourceService
{
    public string LoadResourceAsString(string resourceName);
    public T? LoadResourceAsJson<T>(string resourceName, JsonSerializerOptions? options = null);
}


