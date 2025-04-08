using System.Text.Json;

namespace LabProject.Utilities
{
    public  class Utils
    {
        
        private static readonly Lazy<Utils> instance = new Lazy<Utils>(() => new Utils());
   
        public static Utils Instance => instance.Value;

        private Utils() { }

        public string ExportToJson<T>(IEnumerable<T> data)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            return JsonSerializer.Serialize(data, options);
        }

    
        public void ExportToJsonFile<T>(IEnumerable<T> data, string filePath)
        {
            string json = ExportToJson(data);
            File.WriteAllText(filePath, json);
        }
    }
}
