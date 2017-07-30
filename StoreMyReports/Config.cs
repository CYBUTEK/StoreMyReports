/*

    Store My Reports (a mod for Kerbal Space Program)

    Copyright (C) 2017 CYBUTEK

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace StoreMyReports
{
    [Serializable]
    public class Config
    {
        public bool discardDuplicates;

        private static Config currentConfig;
        private static string filePath = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, "json");

        static Config()
        {
            // check if a configuration file exists
            if (File.Exists(filePath))
            {
                // load configuration from disk
                currentConfig = JsonUtility.FromJson<Config>(File.ReadAllText(filePath));
            }
            else
            {
                // use the default configuration
                currentConfig = new Config();
            }
        }

        /// <summary>
        /// Gets or sets whether to automatically discard duplicates.
        /// </summary>
        public static bool DiscardDuplicates
        {
            get { return currentConfig.discardDuplicates; }
        }

        /// <summary>
        /// Applies a new configuration and saves to disk.
        /// </summary>
        public static void ApplyConfig(Config newConfig)
        {
            // check that the new config exists
            if (newConfig != null)
            {
                // replace current with new
                currentConfig = newConfig;

                // save configuration to disk
                File.WriteAllText(filePath, JsonUtility.ToJson(currentConfig, true));
            }
        }

        /// <summary>
        /// Creates a clone of the current configuration.
        /// </summary>
        public static Config Clone()
        {
            return new Config
            {
                discardDuplicates = currentConfig.discardDuplicates
            };
        }
    }
}