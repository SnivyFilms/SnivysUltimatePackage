using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.CreditTags.Features;
using Exiled.Events.EventArgs.Player;
using MEC;

namespace VVUP.CreditTags
{
    public class EventHandlers
    {
        public Plugin Plugin;
        public EventHandlers(Plugin plugin) => Plugin = plugin;
        public void OnVerified(VerifiedEventArgs ev)
        {
            if (Plugin.Instance.EventHandlers == null)
                return;

            if (ev.Player == null)
                return;
            
            if (ev.Player.DoNotTrack && !Plugin.Instance.Config.IgnoreDntFlag)
            {
                Log.Debug($"VVUP: Player {ev.Player.Nickname} has DoNotTrack enabled, skipping credit tag assignment.");
                return;
            }
                
            Timing.CallDelayed(0.5f, () =>
            {
                if (HasCreditTag(ev.Player.UserId))
                {
                    SetRank(ev.Player, false);
                }
            });
        }

        private static readonly List<Rank> CreditRanks = new List<Rank>()
        {
            new Rank("VVUP Main Developer", "magenta", "FF0090"),
            new Rank("VVUP Contributor", "aqua", "00FFFF"),
            new Rank("VVUP Plugin Tester", "yellow", "FFFF00"),
        };

        private static readonly Dictionary<string, int> ContributorIds = new Dictionary<string, int>()
        {
            { "76561198050637807@steam", 0 }, //Snivy
            { "76561198836489233@steam", 1 }, //Vretu
            { "76561198987608062@steam", 2 }, //Yourfate420
            { "76561197971457827@steam", 1}, //PJ
            { "76561199150506472@steam", 1}, //Mr. Baguetter
            
        };
        public static bool HasCreditTag(string userId) => ContributorIds.ContainsKey(userId);

        public bool SetRank(Player player, bool forced = false)
        {
            if (!ContributorIds.TryGetValue(player.UserId, out int rankIndex))
                return false;

            Rank rank = CreditRanks[rankIndex];

            bool canReciveCreditRank = forced ||
                                       (((string.IsNullOrEmpty(player.RankName) &&
                                          string.IsNullOrEmpty(player.ReferenceHub.serverRoles.HiddenBadge)) ||
                                         Plugin.Instance.Config.BadgeOverride) && player.GlobalBadge is null);
            if (!canReciveCreditRank)
                return false;

            player.RankName = rank.Name;
            player.RankColor = rank.Color;
            Log.Debug($"VVUP: Applied credit tag '{rank.Name}' to {player.Nickname}");
            return true;
        }
    }
    
    public class Rank
    {
        public string Name { get; }
        public string Color { get; }
        public string TextColor { get; }

        public Rank(string name, string color, string textColor)
        {
            Name = name;
            Color = color;
            TextColor = textColor;
        }
    }
}