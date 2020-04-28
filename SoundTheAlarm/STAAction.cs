using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SoundTheAlarm {
    public class STAAction {

        private Dictionary<string, float> managedSettlements;
        private Settlement settlementToTrack;

        public void Initialize() {
            managedSettlements = new Dictionary<string, float>();
        }

        // Action method fired once the VilageBeingRaided event fires
        public void DisplayVillageRaid(Village v) {
            RemoveExpiredFromManagedSettlements(v.Settlement);
            if (Hero.MainHero != null) {
                if (Hero.MainHero.IsAlive) {
                    if (ShouldAlertForSettlement(v.Settlement)) {
                        if (!managedSettlements.ContainsKey(v.Settlement.Name.ToString())) {
                            managedSettlements.Add(v.Settlement.Name.ToString(), Campaign.CurrentTime);

                            TextObject text = GameTexts.FindText("str_sta_alarm_village_attack_message", null);
                            text.SetTextVariable("VILLAGE", v.Settlement.Name.ToString());
                            TextObject attacker = new TextObject("", null);
                            attacker.SetTextVariable("PARTY", v.Settlement.LastAttackerParty.Name);
                            attacker.SetTextVariable("NAME", v.Settlement.LastAttackerParty.LeaderHero.Name);
                            attacker.SetTextVariable("GENDER", v.Settlement.LastAttackerParty.LeaderHero.IsFemale ? 1 : 0);
                            attacker.SetTextVariable("FACTION", v.Settlement.LastAttackerParty.LeaderHero.MapFaction.Name);
                            text.SetTextVariable("ATTACKER", attacker); 

                            TextObject header = GameTexts.FindText("str_sta_alarm_village_attack_title", null);
                            header.SetTextVariable("ICON", "{=!}<img src=\"Icons\\Food@2x\">");

                            string title = header.ToString();
                            string display = text.ToString();
                            string track = GameTexts.FindText("str_sta_ui_track", null).ToString();
                            string close = GameTexts.FindText("str_sta_ui_close", null).ToString();
                                                        

                            settlementToTrack = v.Settlement;
                            InformationManager.ShowInquiry(new InquiryData(title, display, true, true, track, close, new Action(Track), null, ""), STALibrary.Instance.STAConfiguration.PauseGameOnPopup);
                            if (STALibrary.Instance.STAConfiguration.EnableDebugMessages)
                                InformationManager.DisplayMessage(new InformationMessage("STALibrary: " + display, new Color(1.0f, 0.0f, 0.0f)));
                        }
                    }
                }
            }
        }

        // Action method fired once the VillageBecomeNormal event fires
        public void FinalizeVillageRaid(Village v) {
            RemoveExpiredFromManagedSettlements(v.Settlement);
        }

        // Action method fired once the OnSiegeEventStartedEvent event fires
        public void DisplaySiege(SiegeEvent e) {
            if (Hero.MainHero != null) {
                if (Hero.MainHero.IsAlive) {
                    if (ShouldAlertForSettlement(e.BesiegedSettlement)) {
                        if(e.BesiegedSettlement.IsCastle && STALibrary.Instance.STAConfiguration.EnableCastlePopup) {
                            if (!managedSettlements.ContainsKey(e.BesiegedSettlement.Name.ToString())) {
                                managedSettlements.Add(e.BesiegedSettlement.Name.ToString(), 0.0f);

                                TextObject text = GameTexts.FindText("str_sta_alarm_castle_attack_message", null);
                                text.SetTextVariable("CASTLE", e.BesiegedSettlement.Name.ToString());
                                TextObject attacker = new TextObject("", null);
                                attacker.SetTextVariable("PARTY", e.BesiegedSettlement.LastAttackerParty.Name);
                                attacker.SetTextVariable("NAME", e.BesiegedSettlement.LastAttackerParty.LeaderHero.Name);
                                attacker.SetTextVariable("GENDER", e.BesiegedSettlement.LastAttackerParty.LeaderHero.IsFemale ? 1 : 0);
                                attacker.SetTextVariable("FACTION", e.BesiegedSettlement.LastAttackerParty.LeaderHero.MapFaction.Name);
                                text.SetTextVariable("ATTACKER", attacker);

                                TextObject header = GameTexts.FindText("str_sta_alarm_castle_attack_title", null);
                                header.SetTextVariable("ICON", "{=!}<img src=\"MapOverlay\\Settlement\\icon_wall\">"); 

                                string title = header.ToString();
                                string display = text.ToString();
                                string track = GameTexts.FindText("str_sta_ui_track", null).ToString();
                                string close = GameTexts.FindText("str_sta_ui_close", null).ToString();

                                settlementToTrack = e.BesiegedSettlement;
                                InformationManager.ShowInquiry(new InquiryData(title, display, true, true, track, close, new Action(Track), null, ""), STALibrary.Instance.STAConfiguration.PauseGameOnPopup);
                                if (STALibrary.Instance.STAConfiguration.EnableDebugMessages)
                                    InformationManager.DisplayMessage(new InformationMessage("STALibrary: " + display, new Color(1.0f, 0.0f, 0.0f)));
                            }
                        } else if(e.BesiegedSettlement.IsTown && STALibrary.Instance.STAConfiguration.EnableTownPopup) {
                            managedSettlements.Add(e.BesiegedSettlement.Name.ToString(), 0.0f);

                            TextObject text = GameTexts.FindText("str_sta_alarm_town_attack_message", null);
                            text.SetTextVariable("TOWN", e.BesiegedSettlement.Name.ToString());
                            TextObject attacker = new TextObject("", null);
                            attacker.SetTextVariable("PARTY", e.BesiegedSettlement.LastAttackerParty.Name);
                            attacker.SetTextVariable("NAME", e.BesiegedSettlement.LastAttackerParty.LeaderHero.Name);
                            attacker.SetTextVariable("GENDER", e.BesiegedSettlement.LastAttackerParty.LeaderHero.IsFemale ? 1 : 0);
                            attacker.SetTextVariable("FACTION", e.BesiegedSettlement.LastAttackerParty.LeaderHero.MapFaction.Name);
                            text.SetTextVariable("ATTACKER", attacker);

                            TextObject header = GameTexts.FindText("str_sta_alarm_town_attack_title", null);
                            header.SetTextVariable("ICON", "{=!}<img src=\"MapOverlay\\Settlement\\icon_walls_lvl1\">");

                            string title = header.ToString();
                            string display = text.ToString();
                            string track = GameTexts.FindText("str_sta_ui_track", null).ToString();
                            string close = GameTexts.FindText("str_sta_ui_close", null).ToString();

                            settlementToTrack = e.BesiegedSettlement;
                            InformationManager.ShowInquiry(new InquiryData(title, display, true, true, track, close, new Action(Track), null, ""), STALibrary.Instance.STAConfiguration.PauseGameOnPopup);
                            if (STALibrary.Instance.STAConfiguration.EnableDebugMessages)
                                InformationManager.DisplayMessage(new InformationMessage("STALibrary: " + display, new Color(1.0f, 0.0f, 0.0f)));
                        }
                    }
                }
            }
        }

        // Action method fired once the OnSiegeEventEndedEvent event fires
        public void FinalizeSiege(SiegeEvent e) {
            if (managedSettlements.ContainsKey(e.BesiegedSettlement.Name.ToString())) {
                managedSettlements.Remove(e.BesiegedSettlement.Name.ToString());
                if (STALibrary.Instance.STAConfiguration.EnableDebugMessages)
                    InformationManager.DisplayMessage(new InformationMessage("STALibrary: Removed " + e.BesiegedSettlement.Name.ToString() + " from managed settlements list", new Color(1.0f, 0.0f, 0.0f)));
            }
        }

        // Action method fired once two empires declare war
        public void OnDeclareWar(IFaction faction1, IFaction faction2) {

            TextObject text = GameTexts.FindText("str_sta_alarm_war_message", null);
            text.SetTextVariable("LEADER", faction1.Leader.Name);
            text.SetTextVariable("IS_FEMALE", faction1.Leader.IsFemale ? 1 : 0);
            text.SetTextVariable("FACTION1", faction1.Name.ToString());
            text.SetTextVariable("FACTION2", faction2.Name.ToString());

            TextObject header = GameTexts.FindText("str_sta_alarm_war_title", null);
            header.SetTextVariable("ICON", "{=!}<img src=\"Icons\\Party@2x\">");

            string title = header.ToString();
            string display = text.ToString();
            string ok = GameTexts.FindText("str_sta_ui_ok", null).ToString();
            string close = GameTexts.FindText("str_sta_ui_close", null).ToString();

            InformationManager.ShowInquiry(new InquiryData(title, display, true, false, ok, close, null, null, ""), STALibrary.Instance.STAConfiguration.PauseGameOnPopup);
            if (STALibrary.Instance.STAConfiguration.EnableDebugMessages)
                InformationManager.DisplayMessage(new InformationMessage("STALibrary: " + display, new Color(1.0f, 0.0f, 0.0f)));
        }

        // Action method fired once two empires declare peace
        public void OnDeclarePeace(IFaction faction1, IFaction faction2) {

            TextObject text = GameTexts.FindText("str_sta_alarm_peace_message", null);
            text.SetTextVariable("LEADER", faction1.Leader.Name.ToString());
            text.SetTextVariable("IS_FEMALE", faction1.Leader.IsFemale ? 1 : 0);
            text.SetTextVariable("FACTION1", faction1.Name.ToString());
            text.SetTextVariable("FACTION2", faction2.Name.ToString());

            TextObject header = GameTexts.FindText("str_sta_alarm_peace_title", null);
            header.SetTextVariable("ICON", "{=!}<img src=\"Icons\\Morale@2x\">");

            string title = header.ToString();
            string display = text.ToString();
            string ok = GameTexts.FindText("str_sta_ui_ok", null).ToString();
            string close = GameTexts.FindText("str_sta_ui_close", null).ToString();

            InformationManager.ShowInquiry(new InquiryData(title, display, true, false, ok, close, null, null, ""), STALibrary.Instance.STAConfiguration.PauseGameOnPopup);
            if (STALibrary.Instance.STAConfiguration.EnableDebugMessages)
                InformationManager.DisplayMessage(new InformationMessage("STALibrary: " + display, new Color(1.0f, 0.0f, 0.0f)));
        }

        // Action method fired once the user clicks 'Track' on the popup
        public void Track() {
            Campaign.Current.VisualTrackerManager.RegisterObject(settlementToTrack);
            if (STALibrary.Instance.STAConfiguration.EnableDebugMessages)
                InformationManager.DisplayMessage(new InformationMessage("STALibrary: Tracking " + settlementToTrack.Name.ToString(), new Color(1.0f, 0.0f, 0.0f)));
        }

        // Check if the alert should fire (thanks to iPherian for submitting pull request that fixed alert not showing if you are not the king)
        private bool ShouldAlertForSettlement(Settlement settlement) {
            return settlement.MapFaction.Leader == Hero.MainHero || settlement.OwnerClan.Leader == Hero.MainHero;
        }

        // We ignore certain settlements when alerting for a period of time after user first alerted. This removes a settlement which has expired in that way from the list.
        private void RemoveExpiredFromManagedSettlements(Settlement settlement) {
            if (managedSettlements.ContainsKey(settlement.Name.ToString())) {
                float time;
                if (!managedSettlements.TryGetValue(settlement.Name.ToString(), out time)) {
                    return;
                }
                if (Campaign.CurrentTime > time + STALibrary.Instance.STAConfiguration.TimeToRemoveVillageFromList) {
                    managedSettlements.Remove(settlement.Name.ToString());
                    if (STALibrary.Instance.STAConfiguration.EnableDebugMessages) 
                        InformationManager.DisplayMessage(new InformationMessage("STALibrary: Removed " + settlement.Name.ToString() + " from managed settlements list", new Color(1.0f, 0.0f, 0.0f)));
                }
        
                if (STALibrary.Instance.STAConfiguration.EnableDebugMessages)
                    InformationManager.DisplayMessage(new InformationMessage("STALibrary: " + settlement.Name.ToString() + " count is at " + ((time + STALibrary.Instance.STAConfiguration.TimeToRemoveVillageFromList) - Campaign.CurrentTime), new Color(1.0f, 0.0f, 0.0f)));
            }
        }
    }
}
