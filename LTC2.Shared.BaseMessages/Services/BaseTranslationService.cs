using LTC2.Shared.BaseMessages.Interfaces;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using LTC2.Shared.Utils.Generic;

namespace LTC2.Shared.BaseMessages.Services
{
    public class BaseTranslationService : IBaseTranslationService
    {
        private ConcurrentDictionary<string, string> _messages;

        private string _defaultLanguage;

        public BaseTranslationService()
        {
            _defaultLanguage = "nl";

            CurrentLanguage = _defaultLanguage;

            LoadMessageFromFile(_defaultLanguage);
        }

        public string CurrentLanguage
        {
            get;

            private set;
        }

        public void LoadMessagesForLanguage(string language)
        {
            LoadMessageFromFile(language);
        }

        public void MergeMessagesForLanguage(string extension)
        {
            MergeMessageFromFile(CurrentLanguage, extension);
        }

        public string GetMessage(string message)
        {
            var id = message.StartsWith('#') ? message.Substring(1) : message;

            if (_messages.ContainsKey(id))
            {
                return _messages[id];
            }

            return message;
        }

        public string GetMessage(string message, List<string> parameters)
        {
            var msg = GetMessage(message);

            var count = 0;

            foreach (var param in parameters)
            {
                msg = msg.Replace($"%{count}%", parameters[count]);

                count++;
            }

            return msg;
        }

        public string GetMessage(string message, string parameter)
        {
            var parameters = new List<string>()
            {
                parameter
            };

            return GetMessage(message, parameters);
        }

        protected void LoadMessageFromFile(string language)
        {
            var processModule = Process.GetCurrentProcess().MainModule;
            var languageFolder = Path.Combine(Path.GetDirectoryName(processModule?.FileName), "Resources");
            var appId = FileUtils.GetConfigFileAddition();
            
            var languageFile = Path.Combine(languageFolder, $"messages{appId}.{language}.json");
            var content = File.ReadAllText(languageFile);

            _messages = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(content);
        }

        protected void MergeMessageFromFile(string language, string extension)
        {
            var processModule = Process.GetCurrentProcess().MainModule;
            var languageFolder = Path.Combine(Path.GetDirectoryName(processModule?.FileName), "Resources");
            var appId = FileUtils.GetConfigFileAddition();

            var languageFile = Path.Combine(languageFolder, $"messages{appId}.{language}.{extension}.json");

            if (File.Exists(languageFile))
            {
                var content = File.ReadAllText(languageFile);
                var messages = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(content);

                foreach (var message in messages)
                {
                    _messages.AddOrUpdate(message.Key, message.Value, (key, oldValue) => message.Value);
                }
            }
        }

    }
}
