using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.InputSystem;

namespace ViewAllWars
{
    public class SubModule : MBSubModuleBase {

        private bool loaded = false;

        protected override void OnApplicationTick(float dt) {
            base.OnApplicationTick(dt);
            if(loaded) {
                if(Campaign.Current.GameMode == CampaignGameMode.Campaign) {
                    if (Campaign.Current.GameStarted) {
                        if (InputKey.Home.IsPressed()) {
                            string message = "Warring Empires Across Calradia\n\n";
                            int count = 0;

                            foreach (Kingdom kingdom in Kingdom.All) {
                                if (kingdom != null) {
                                    count++;
                                    message += count + ". " + kingdom.Name + " versus ";
                                    foreach (Kingdom kingdom2 in from w in Kingdom.All orderby w.Name.ToString() select w) {
                                        if (kingdom2 != null && !kingdom.Name.Equals(kingdom2.Name)) {
                                            if (kingdom.IsAtWarWith(kingdom2)) {
                                                message += kingdom2.Name + " and ";
                                            }
                                        }
                                    }
                                    message = message.Substring(0, message.Length - 5);
                                    message += "\n\n";
                                }
                            }

                            InformationManager.ShowInquiry(new InquiryData("View All Wars", message, true, false, "Ok", "", null, null, ""), false);
                        }
                    }
                }
            }
        }

        public override void OnGameEnd(Game game) {
            base.OnGameEnd(game);
            loaded = false;
        }

        public override bool DoLoading(Game game) {
            loaded = true;
            return base.DoLoading(game);
        }
    }
}
