using System;
using System.Windows.Forms;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using HarmonyLib;

namespace SoundTheAlarm {

    // This class is currently not in use unless the current method turns out to not work
    [HarmonyPatch(typeof(SettlementVariablesBehavior), "HourlyTick")]
    public class HourlyTickPatch {

        private static Dictionary<string, bool> _managedSettlements;
        private static Settlement _settlementToTrack;

        public static void Initialize() {
            _managedSettlements = new Dictionary<string, bool>();
        }

        private static bool Prefix(SettlementVariablesBehavior __instance) {
            bool result = false;
            try {
                foreach(Settlement settlement in Settlement.FindAll((Settlement x) => (x.IsVillage || x.IsFortification || x.IsTown || x.IsCastle) && x.LastAttackerParty != null)) {
                    if(settlement.IsUnderRaid || settlement.IsUnderSiege) {
                        if (Hero.MainHero != null) {
                            if (Hero.MainHero.IsAlive) {
                                if (settlement.MapFaction.Leader == Hero.MainHero) {
                                    if (!_managedSettlements.ContainsKey(settlement.Name.ToString())) {
                                        _managedSettlements.Add(settlement.Name.ToString(), true);
                                        string display =
                                                settlement.Name.ToString() +
                                                " is under attack by " +
                                                settlement.LastAttackerParty.Name.ToString() +
                                                " of the " +
                                                settlement.LastAttackerParty.LeaderHero.MapFaction.Name.ToString() +
                                                "!"
                                            ;
                                        _settlementToTrack = settlement;
                                        InformationManager.ShowInquiry(new InquiryData("Sound The Alarm", display, true, true, "Track", "Close", new Action(Track), null, ""), true);
                                    }
                                }
                            }
                        }
                    } else if (_managedSettlements.ContainsKey(settlement.Name.ToString())) {
                        _managedSettlements.Remove(settlement.Name.ToString());
                    }

                    if (settlement.IsVillage) {
                        if(settlement.Party.MapEvent == null) {
                            settlement.PassedHoursAfterLastThreat--;
                        }
                    } else if(settlement.IsFortification && settlement.Party.MapEvent == null && !settlement.IsUnderSiege) {
                        settlement.PassedHoursAfterLastThreat--;
                    }
                    if(settlement.PassedHoursAfterLastThreat == 0) {
                        settlement.LastAttackerParty = null;
                    }
                }
            } catch (Exception ex) {
                result = true;
                MessageBox.Show("An error occurred while trying to get HourlyTick. Reverting to original...\n\n" + ex.Message + "\n\n" + ex.StackTrace);
            }
            return result;
        }

        private static void Track() {
            Campaign.Current.VisualTrackerManager.RegisterObject(_settlementToTrack);
        }
    }
}
