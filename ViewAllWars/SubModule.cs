using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.InputSystem;

namespace ViewAllWars
{
    public class SubModule : MBSubModuleBase {

        protected override void OnApplicationTick(float dt) {
            base.OnApplicationTick(dt);
            if(Campaign.Current != null) {
                if (Campaign.Current.GameStarted) {
                    if (InputKey.Home.IsPressed()) {
                        string message = "Warring Empires Across Calradia\n\n";
                        int count = 0;

                        foreach (Kingdom kingdom in Kingdom.All) {
                            if (kingdom != null) {
                                count++;
                                message += count + ". " + kingdom.Name + " versus ";
                                int warCount = 0;
                                foreach (Kingdom kingdom2 in from w in Kingdom.All orderby w.Name.ToString() select w) {
                                    if (kingdom2 != null && !kingdom.Name.Equals(kingdom2.Name)) {
                                        if (kingdom.IsAtWarWith(kingdom2)) {
                                            message += kingdom2.Name + " and ";
                                            warCount++;
                                        }
                                    }
                                }
                                if (warCount > 0)
                                    message = message.Substring(0, message.Length - 5);
                                else
                                    message = message.Substring(0, message.Length - 8) + " is at peace.";
                                message += "\n\n";
                            }
                        }

                        InformationManager.ShowInquiry(new InquiryData("View All Wars", message, true, false, "Ok", "", null, null, ""), false);
                    }
                }
            }
            
        }
    }
}
