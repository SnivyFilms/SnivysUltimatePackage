using System.Reflection;

namespace VVUP.CustomRoles.API.ExternalTeams
{
    public class UiuCheck : ExternalTeamChecker
    {
        public override void Init(Assembly assembly)
        {
            PluginEnabled = true;

            Type mainClass = assembly.GetType("UIURescueSquad.UIURescueSquad");
            Instance = mainClass.GetField("Instance").GetValue(null);
            FieldInfo = mainClass.GetField("IsSpawnable");
        }
    }
}