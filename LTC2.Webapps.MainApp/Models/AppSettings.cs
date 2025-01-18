using System;

namespace LTC2.Webapps.MainApp.Models
{
    public class AppSettings
    {
        private string _tilesFolder;
        private string _tempRoutesFolder;

        public string Name { get; set; }

        public string AllowedOrigins { get; set; }

        public bool ForceDetailed { get; set; }
        
        public bool UseRedirectDuringLogin  { get; set; }

        public string TilesFolder
        {
            get
            {
                return _tilesFolder == null ? null : Environment.ExpandEnvironmentVariables(_tilesFolder);
            }

            set
            {
                _tilesFolder = value;
            }
        }

        public string DefaultListenUrls { get; set; }

        public bool IsStandAlone { get; set; }

        public string TempRoutesFolder
        {
            get
            {
                return _tempRoutesFolder == null ? null : Environment.ExpandEnvironmentVariables(_tempRoutesFolder);
            }
            set
            {
                _tempRoutesFolder = value;
            }
        }
    }
}
