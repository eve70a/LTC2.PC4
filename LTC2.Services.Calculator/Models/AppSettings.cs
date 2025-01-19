using System;

namespace LTC2.Services.Calculator.Models
{
    public class AppSettings
    {
        private string _multiSportFolder;

        public string Name { get; set; }
        
        public string MultiSportFolder
        {
            get
            {
                return _multiSportFolder == null ? null : Environment.ExpandEnvironmentVariables(_multiSportFolder);
            }
            set
            {
                _multiSportFolder = value;
            }
        }
    }
}
