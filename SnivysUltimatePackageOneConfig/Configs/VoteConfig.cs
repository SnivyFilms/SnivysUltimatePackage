﻿namespace SnivysUltimatePackageOneConfig.Configs
{
    public class VoteConfig
    {
        public bool IsEnabled { get; set; } = true;

        public ushort MapBroadcastTime { get; set; } = 10;

        public string StartVoteMapBroadcast { get; set; } =
            @"<size=30>A vote has started!. Use .vote in player console [~]. Prompt: %prompt%, \nVote: %option1% %option2% %option3% %option4% %option5%</size>";
        public string EndVoteMapBroadcast { get; set; } = @"<size=30>The vote has ended!\n The results are: %results%</size>";
        public float VoteDuration { get; set; } = 30f;
    }
}