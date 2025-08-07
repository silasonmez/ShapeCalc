using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace InputApi.Services
{
    public interface IEndpointSelector
    {
        string Next();
    }

    public class RoundRobinEndpointSelector : IEndpointSelector
    {
        private readonly List<string> _targets;
        private int _index = -1;
        
        private readonly object _lock = new object();

        public RoundRobinEndpointSelector()
        {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            var json = File.ReadAllText(configPath);
            var root = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
            _targets = root?["ComputeApiTargets"] ?? throw new Exception("Targets missing in config.");
        }

        public string Next()
        {
            lock (_lock)
            {
                _index = (_index + 1) % _targets.Count;
                return _targets[_index];
            }
        }
    }
}
