using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;

namespace AttackAlert
{
    public class SubModule : MBSubModuleBase
    {
        private bool loaded = false;
        private bool postLoad = false;
        private bool controlLoad = false;
        private float _currentTime;
        private Dictionary<string, bool> _managedSettlements;
        private Settlement _settlementToTrack;

        protected override void OnApplicationTick(float dt) {
            base.OnApplicationTick(dt);
            if(loaded) {
                if(Campaign.Current.GameStarted) {
                    if(!controlLoad) {
                        _currentTime = Campaign.CurrentTime;
                        controlLoad = true;
                    } else {
                        if(Campaign.CurrentTime > _currentTime + 2 && !postLoad) {
                            postLoad = true;
                        }
                    }
                    if(postLoad) {
                        if (Hero.MainHero != null) {
                            foreach (Settlement settlement in Hero.MainHero.MapFaction.Settlements) {
                                if (settlement.MapFaction.Leader == Hero.MainHero) {
                                    if (settlement.IsUnderRaid || settlement.IsUnderSiege) {
                                        if (!_managedSettlements.ContainsKey(settlement.Name.ToString())) {
                                            _managedSettlements.Add(settlement.Name.ToString(), true);
                                            string display = "";
                                            display += settlement.Name + " is under attack by " + settlement.LastAttackerParty.Name + " of the " + settlement.LastAttackerParty.LeaderHero.MapFaction.Name + "!";
                                            _settlementToTrack = settlement;
                                            InformationManager.ShowInquiry(new InquiryData("Settlement Attack Alert", display, true, true, "Track", "Close", new Action(Track), null, ""), true);
                                        }
                                    } else if (_managedSettlements.ContainsKey(settlement.Name.ToString())) {
                                        _managedSettlements.Remove(settlement.Name.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Track() {
            Campaign.Current.VisualTrackerManager.RegisterObject(_settlementToTrack);
        }

        public override void OnGameEnd(Game game) {
            base.OnGameEnd(game);
            _managedSettlements = null;
            loaded = false;
        }

        public override bool DoLoading(Game game) {
            if(!loaded)
                _managedSettlements = new Dictionary<string, bool>();
            loaded = true;
            return base.DoLoading(game);
        }
    }
}
