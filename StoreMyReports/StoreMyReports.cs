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

using KSP.UI.Screens.Flight.Dialogs;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace StoreMyReports
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class StoreMyReports : MonoBehaviour
    {
        private static string configFilePath = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, "json");

        [SerializeField]
        private bool discardDuplicates = true;

        private bool isExperimentsResultDialogOpen;

        /// <summary>
        /// Gets the current instance of the object.
        /// </summary>
        public static StoreMyReports Instance { get; private set; }

        /// <summary>
        /// Gets or sets whether to automatically discard duplicates.
        /// </summary>
        public bool DiscardDuplicates
        {
            get { return discardDuplicates; }
            set { discardDuplicates = value; }
        }

        private void Awake()
        {
            // assign current instance
            if (Instance == null)
            {
                Instance = this;
            }

            // allow only one instance
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void OnDisable()
        {
            // save configuration
            File.WriteAllText(configFilePath, JsonUtility.ToJson(this, true));
        }

        private void OnEnable()
        {
            // load configuration
            if (File.Exists(configFilePath))
            {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(configFilePath), this);
            }
        }

        private void OnExperimentsResultDialogClosed()
        {
            if (FlightGlobals.ActiveVessel != null)
            {
                // get all container modules on the vessel
                List<ModuleScienceContainer> containers = FlightGlobals.ActiveVessel.FindPartModulesImplementing<ModuleScienceContainer>();

                // iterate over the containers
                for (int containerIndex = 0; containerIndex < containers.Count; containerIndex++)
                {
                    ModuleScienceContainer container = containers[containerIndex];

                    // get all the experiment modules attached to the same part as the container
                    List<ModuleScienceExperiment> experiments = container.part.FindModulesImplementing<ModuleScienceExperiment>();

                    // iterate over the experiments
                    for (int experimentIndex = 0; experimentIndex < experiments.Count; experimentIndex++)
                    {
                        ModuleScienceExperiment experiment = experiments[experimentIndex];

                        // check that experiment has available data
                        if (experiment.GetScienceCount() > 0)
                        {
                            // get both the container and experiment data for duplicate checking
                            ScienceData[] containerDataArray = container.GetData();
                            ScienceData[] experimentDataArray = experiment.GetData();

                            // iterate over the experiment data
                            foreach (ScienceData experimentData in experimentDataArray)
                            {
                                bool allowDataTransfer = true;

                                // check for duplicates in the container data
                                foreach (ScienceData containerData in containerDataArray)
                                {
                                    if (containerData.subjectID == experimentData.subjectID)
                                    {
                                        allowDataTransfer = false;

                                        // discard duplicates
                                        if (discardDuplicates)
                                        {
                                            experiment.DumpData(experimentData);
                                        }
                                    }
                                }

                                // transfer data from experiment to container
                                if (allowDataTransfer)
                                {
                                    experiment.DumpData(experimentData);
                                    container.AddData(experimentData);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Update()
        {
            // check experiments result dialog has closed on this frame
            if (isExperimentsResultDialogOpen && ExperimentsResultDialog.Instance == null)
            {
                OnExperimentsResultDialogClosed();
            }

            // update experiments result dialog open state
            isExperimentsResultDialogOpen = (ExperimentsResultDialog.Instance != null);
        }
    }
}