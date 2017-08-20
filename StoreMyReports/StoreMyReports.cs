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
using UnityEngine;

namespace StoreMyReports
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class StoreMyReports : MonoBehaviour
    {
        private Vector3 experimentsResultDialogPosition;
        private bool isExperimentsResultDialogOpen;

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
                                        if (Config.DiscardDuplicates)
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

        private void OnExperimentsResultDialogOpened()
        {
            if (Config.SaveExperimentsResultDialogPosition)
            {
                // set experiments result dialog position to stored position
                ExperimentsResultDialog.Instance.transform.position = experimentsResultDialogPosition;
            }
        }

        private void Update()
        {
            // check experiments result dialog has closed on this frame
            if (isExperimentsResultDialogOpen && ExperimentsResultDialog.Instance == null)
            {
                OnExperimentsResultDialogClosed();
            }
            if (ExperimentsResultDialog.Instance != null)
            {
                // check experiments result dialog has opened on this frame
                if (isExperimentsResultDialogOpen == false)
                {
                    OnExperimentsResultDialogOpened();
                }

                // update experiments result dialog stored position
                experimentsResultDialogPosition = ExperimentsResultDialog.Instance.transform.position;
            }

            // update experiments result dialog open state
            isExperimentsResultDialogOpen = (ExperimentsResultDialog.Instance != null);
        }
    }
}