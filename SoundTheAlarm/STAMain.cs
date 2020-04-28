using System;
using System.Windows.Forms;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

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
                    "STALibrary:\n" +
                    "Villages(" + STALibrary.Instance.STAConfiguration.EnableVillagePopup + ")\n" +
                    "Castles(" + STALibrary.Instance.STAConfiguration.EnableCastlePopup + ")\n" +
                    "Towns(" + STALibrary.Instance.STAConfiguration.EnableTownPopup + ")\n" +
                    "Wars(" + STALibrary.Instance.STAConfiguration.EnableWarPopup + ")\n" +
                    "Peace(" + STALibrary.Instance.STAConfiguration.EnablePeacePopup + ")\n" +
                    "PauseOnPopup(" + STALibrary.Instance.STAConfiguration.PauseGameOnPopup + ")\n" +
                    "TimeToRemove(" + STALibrary.Instance.STAConfiguration.TimeToRemoveVillageFromList + ")\n",
                    new Color(1.0f, 0.0f, 0.0f)));
        }

        // Initialize Sound The Alarm once the save has been loaded
        public override void OnGameLoaded(Game game, object initializerObject) {
            base.OnGameLoaded(game, initializerObject);
            try {
                STALibrary.Instance.STAAction.Initialize();
                if(STALibrary.Instance.STAConfiguration.EnableVillagePopup) {
                    CampaignEvents.VillageBeingRaided.AddNonSerializedListener(this, new Action<Village>(STALibrary.Instance.STAAction.DisplayVillageRaid));
                    CampaignEvents.VillageBecomeNormal.AddNonSerializedListener(this, new Action<Village>(STALibrary.Instance.STAAction.FinalizeVillageRaid));
                }
                if (STALibrary.Instance.STAConfiguration.EnableCastlePopup || STALibrary.Instance.STAConfiguration.EnableTownPopup) {
                    CampaignEvents.OnSiegeEventStartedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(STALibrary.Instance.STAAction.DisplaySiege));
                    CampaignEvents.OnSiegeEventEndedEvent.AddNonSerializedListener(this, new Action<SiegeEvent>(STALibrary.Instance.STAAction.FinalizeSiege));
                }
                if (STALibrary.Instance.STAConfiguration.EnableWarPopup) {
                    CampaignEvents.WarDeclared.AddNonSerializedListener(this, new Action<IFaction, IFaction>(STALibrary.Instance.STAAction.OnDeclareWar));
                }
                if (STALibrary.Instance.STAConfiguration.EnablePeacePopup) {
                    CampaignEvents.MakePeace.AddNonSerializedListener(this, new Action<IFaction, IFaction>(STALibrary.Instance.STAAction.OnDeclarePeace));
                }
                game.GameTextManager.LoadGameTexts(BasePath.Name + $"Modules/SoundTheAlarm/ModuleData/module_strings.xml");
            } catch (Exception ex) {
                MessageBox.Show("An error has occurred whilst initialising Sound The Alarm:\n\n" + ex.Message + "\n\n" + ex.StackTrace);
            }
        }
    }
}
