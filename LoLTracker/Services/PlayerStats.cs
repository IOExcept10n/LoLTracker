namespace LoLTracker.Services
{
    public class PlayerStats
    {
        public double Efficiency { get; set; }

        public string ChampionName { get; set; } = string.Empty;
        public int ChampionId { get; set; } = 0;

        public string PlayerName { get; set; } = string.Empty;
        public int MatchCount { get; set; }
    }
}