namespace Husa.Uploader.Desktop.ViewModels
{
    using System.IO;
    using System.Text.Json;
    using System.Text.Json.Nodes;

    public abstract class ViewModelHelpers
    {
        public static void UpdateAppSetting(string key, object newValue)
        {
            try
            {
                string appSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                string json = File.ReadAllText(appSettingsPath);
                JsonNode jsonNode = JsonNode.Parse(json);
                string[] keys = key.Split(':');
                JsonNode currentNode = jsonNode;
                for (int i = 0; i < keys.Length - 1; i++)
                {
                    string currentKey = keys[i];

                    if (currentNode[currentKey] == null)
                    {
                        currentNode[currentKey] = new JsonObject();
                    }

                    currentNode = currentNode[currentKey];
                }

                string finalKey = keys[^1];

                switch (newValue)
                {
                    case bool boolVal:
                        currentNode[finalKey] = boolVal;
                        break;
                    case int intVal:
                        currentNode[finalKey] = intVal;
                        break;
                    case string strVal:
                        currentNode[finalKey] = strVal;
                        break;
                    default:
                        currentNode[finalKey] = JsonValue.Create(newValue);
                        break;
                }

                var jsonToWrite = jsonNode.ToJsonString(new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = false,
                });

                File.WriteAllText(appSettingsPath, jsonToWrite);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating app setting: {ex.Message}");
                throw;
            }
        }
    }
}
