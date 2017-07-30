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

using KSP.UI.Screens;
using UnityEngine;

namespace StoreMyReports
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class ConfigButton : MonoBehaviour
    {
        private ApplicationLauncherButton appLauncherButton;
        private Texture2D configButtonTexture;
        private ConfigDialog configDialog;

        public void DismissConfigDialog()
        {
            if (configDialog != null)
            {
                configDialog.Close();
            }
        }

        private void Awake()
        {
            // get the application launcher button texture from the game database
            configButtonTexture = GameDatabase.Instance.GetTexture(@"StoreMyReports/ConfigButton", false);
        }

        private void CreateConfigDialog()
        {
            if (configDialog == null)
            {
                configDialog = ConfigDialog.Create(gameObject, OnConfigDialogDestroy);
            }
        }

        private void OnApplicationLauncherReady()
        {
            if (ApplicationLauncher.Instance != null)
            {
                // create application launcher button
                appLauncherButton = ApplicationLauncher.Instance.AddModApplication(CreateConfigDialog, DismissConfigDialog, null, null, null, null, ApplicationLauncher.AppScenes.ALWAYS, configButtonTexture);
            }
        }

        private void OnApplicationLauncherUnreadifying(GameScenes gameScene)
        {
            if (appLauncherButton != null)
            {
                // remove button from the application launcher
                ApplicationLauncher.Instance.RemoveModApplication(appLauncherButton);
                appLauncherButton = null;
            }
        }

        private void OnConfigDialogDestroy()
        {
            if (appLauncherButton != null)
            {
                // set the application launcher button to false but don't fire any button changed events
                appLauncherButton.SetFalse(false);
            }
        }

        private void OnDisable()
        {
            // unsubscribe to application launcher events
            GameEvents.onGUIApplicationLauncherReady.Remove(OnApplicationLauncherReady);
            GameEvents.onGUIApplicationLauncherUnreadifying.Remove(OnApplicationLauncherUnreadifying);
        }

        private void OnEnable()
        {
            // subscribe to application launcher events
            GameEvents.onGUIApplicationLauncherReady.Add(OnApplicationLauncherReady);
            GameEvents.onGUIApplicationLauncherUnreadifying.Add(OnApplicationLauncherUnreadifying);
        }
    }
}