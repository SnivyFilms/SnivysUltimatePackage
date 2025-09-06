using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using PlayerRoles;

namespace VVUP.Base
{
    public static class GetRandomSpectator
    {
        public static Player GetSpectator()
        {
            // Get a list of players with the Spectator role
            Log.Debug("VVUP: Getting a list of players who are spectators");
            List<Player> spectators = Player.List.Where(p => p.Role == RoleTypeId.Spectator).ToList();

            // If there are no spectators, return null
            Log.Debug("VVUP: Checking if there is any spectators");
            if (spectators.Count == 0)
                return null;

            // Select a random spectator
            Log.Debug("VVUP: Selecting a random spectator");
            int index = GetRandomNumber.GetRandomInt(spectators.Count);
            return spectators[index];
        }
    }
}