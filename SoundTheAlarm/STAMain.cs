using System;
using System.Windows.Forms;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using HarmonyLib;

namespace SoundTheAlarm {
    public class STAMain : MBSubModuleBase {

        // Read XML data once the SubModule has been loaded
        protected override void OnSubModuleLoad() {
            base.OnSubModuleLoad();
            string text = BasePath.Name + "Modules/SoundTheAlarm/ModuleData/SoundTheAlarm.config.xml";
            STALibrary.Instance.LoadXML(text);
        }

        // Method run during the initial movie on game startup
        protected override void OnBeforeInitialModuleScreenSetAsRoot() {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            if (STALibrary.Instance.STAConfiguration.EnableDebugMessages)
                InformationManager.DisplayMessage(new InformationMessage(
                    "STALibrary: " +
                    "Fiefs(" + STALibrary.Instance.STAConfiguration.EnableFiefPopup + ") " +
                    "Wars(" + STALibrary.Instance.STAConfiguration.EnableWarPopup + ") " +
                    "Peace(" + STALibrary.Instance.STAConfiguration.EnablePeacePopup + ")",
                    new Color(1.0f, 0.0f, 0.0f)));
        }

        // Initialize Sound The Alarm once the save has been loaded
        public override void OnGameLoaded(Game game, object initializerObject) {
            base.OnGameLoaded(game, initializerObject);
            try {
                STALibrary.Instance.STAAction.Initialize();
                if(STALibrary.Instance.STAConfiguration.EnableFiefPopup) {
                    CampaignEvents.VillageBeingRaided.AddNonSerializedListener(this, new Action<Village>(STALibrary.Instance.STAAction.DisplayVillageRaid));
                    CampaignEvents.VillageBecomeNormal.AddNonSerializedListener(this, new Action<Village>(STALibrary.Instance.STAAction.FinalizeVillageRaid));
                    CampaignEvents.OnSiegeEventStartedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(STALibrary.Instance.STAAction.DisplaySiege));
                    CampaignEvents.OnSiegeEventEndedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(STALibrary.Instance.STAAction.FinalizeSiege));
                }
                if (STALibrary.Instance.STAConfiguration.EnableWarPopup) {
                    CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction>(STALibrary.Instance.STAAction.OnDeclareWar));
                }
                if (STALibrary.Instance.STAConfiguration.EnablePeacePopup) {
                    CampaignEvents.MakePeace.AddNonSerializedListener(this, new Action<IFaction, IFaction>(STALibrary.Instance.STAAction.OnDeclarePeace));
                }
            } catch (Exception ex) {
                MessageBox.Show("An error has occurred whilst initialising Sound The Alarm:\n\n" + ex.Message + "\n\n" + ex.StackTrace);
            }
        }
        
        // Below is the alternate method using the Harmony library to patch the relative methods
        //protected override void OnSubModuleLoad() {
        //    base.OnSubModuleLoad();
        //    try {
        //        HourlyTickPatch.Initialize();

        //        Harmony harmony = new Harmony("mod.bannerlord.alpine");
        //        harmony.PatchAll();

        //    } catch(Exception ex) {
        //        MessageBox.Show("An error has occurred whilst initialising Sound The Alarm:\n\n" + ex.Message + "\n\n" + ex.StackTrace);
        //    }
        //}

    }
}
