using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;
using VVUP.ServerEvents.ServerEventsConfigs;
using VVUP.ServerEvents.ServerEventsEventHandlers;
using Server = Exiled.Events.Handlers.Server;

namespace VVUP.ServerEvents
{
    public class Plugin : Plugin<ServerEventsMasterConfig>
    {
        public override PluginPriority Priority { get; } = PluginPriority.Low;
        public static Plugin Instance;
        public override string Name { get; } = "VVUP: Server Events";
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVUP.SE";
        public override Version Version { get; } = new Version(3, 3, 0);
        public override Version RequiredExiledVersion { get; } = new Version(9, 9, 2);

        public static int ActiveEvent = 0;
        public ServerEventsMainEventHandler ServerEventsMainEventHandler;

        public override void OnEnabled()
        {
            if (!Loader.Plugins.Any(plugin => plugin.Prefix == "VVUP.Base"))
            {
                Log.Error("VVUP SE: Base Plugin is not present, disabling module");
                base.OnDisabled();
                return;
            }

            Instance = this;
            ServerEventsMainEventHandler = new ServerEventsMainEventHandler(this);
            Server.RoundStarted += ServerEventsMainEventHandler.OnRoundStart;
            Server.RoundEnded += ServerEventsMainEventHandler.OnEndingRound;
            Server.WaitingForPlayers += ServerEventsMainEventHandler.OnWaitingForPlayers;
            Base.Plugin.Instance.VvupSe = true;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Base.Plugin.Instance.VvupSe = false;
            Server.RoundStarted -= ServerEventsMainEventHandler.OnRoundStart;
            Server.RoundEnded -= ServerEventsMainEventHandler.OnEndingRound;
            Server.WaitingForPlayers -= ServerEventsMainEventHandler.OnWaitingForPlayers;
            ServerEventsMainEventHandler = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}