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

using System.Reflection;
using UnityEngine;

namespace StoreMyReports
{
    public class ConfigDialog : MonoBehaviour
    {
        private UISkinDef guiSkin;
        private Callback onClose;
        private PopupDialog popupDialog;
        private Rect screenRect;
        private Config tempConfig;

        public PopupDialog PopupDialog
        {
            get { return popupDialog; }
        }

        /// <summary>
        /// Creates a new config dialog window.
        /// </summary>
        public static ConfigDialog Create(GameObject gameObject, Callback onClose)
        {
            ConfigDialog configDialog = gameObject.AddComponent<ConfigDialog>();
            configDialog.guiSkin = UISkinManager.GetSkin("MiniSettingsSkin");
            configDialog.screenRect = new Rect(0.5f, 0.5f, 400f, 150f);
            configDialog.tempConfig = Config.Clone();
            configDialog.onClose = onClose;

            return configDialog;
        }

        /// <summary>
        /// Closes the dialog window.
        /// </summary>
        public void Close()
        {
            popupDialog.Dismiss();
        }

        /// <summary>
        /// Applies the configuration changes and closes the dialog window.
        /// </summary>
        private void Accept()
        {
            Apply();
            Close();
        }

        /// <summary>
        /// Applies the configuration changes.
        /// </summary>
        private void Apply()
        {
            Config.ApplyConfig(tempConfig);
        }

        /// <summary>
        /// Gets the dialog GUI for use with the stock popup dialog system.
        /// </summary>
        private DialogGUIBase[] GetDialogGUI()
        {
            // layout containing the settings gui objects
            DialogGUIVerticalLayout settingsLayout = new DialogGUIVerticalLayout(0f, 0f, 0f, new RectOffset(5, 25, 5, 5), TextAnchor.UpperLeft,
                new DialogGUIToggleButton(tempConfig.discardDuplicates, "Automatically Discard Duplicates", value => tempConfig.discardDuplicates = value, h: 30f),
                new DialogGUIToggleButton(tempConfig.saveExperimentsResultDialogPosition, "Save Experiements Result Dialog Position", value => tempConfig.saveExperimentsResultDialogPosition = value, h: 30f)
            );

            // scroll list for containing the settings layout
            DialogGUIScrollList settingsScrollList = new DialogGUIScrollList(-Vector2.one, false, true, settingsLayout);

            // layout containing the bottom gui objects
            DialogGUIBase bottomLayout = new DialogGUIHorizontalLayout
            (TextAnchor.MiddleLeft,

                // version
                new DialogGUIFlexibleSpace(),
                new DialogGUILabel($"<color=#eee><i>v{Assembly.GetExecutingAssembly().GetName().Version}</i></color>"),
                new DialogGUIFlexibleSpace(),

                // buttons
                new DialogGUIButton("Apply", Apply, 80f, 30f, false),
                new DialogGUIButton("Accept", Accept, 80f, 30f, false),
                new DialogGUIButton("Close", Close, 80f, 30f, false)
            );

            return new DialogGUIBase[2] { settingsScrollList, bottomLayout };
        }

        private void OnDestroy()
        {
            // invoke the closing callback that was passed in at creation
            if (onClose != null)
            {
                onClose.Invoke();
            }
        }

        private void Start()
        {
            // create stock popup dialog
            popupDialog = PopupDialog.SpawnPopupDialog(new MultiOptionDialog("Store My Reports", string.Empty, "Store My Reports - Configuration", guiSkin, screenRect, GetDialogGUI()), false, guiSkin);

            // destroy this component when the popup is destroyed
            popupDialog.onDestroy.AddListener(() => Destroy(this));
        }
    }
}