namespace LoLTracker.Services
{
    public class MatchReport(in TeamStats allyTeam, in TeamStats enemyTeam)
    {
        public double WinProbability { get; set; }

        public TeamStats AllyTeam { get; set; } = allyTeam;

        public TeamStats EnemyTeam { get; set; } = enemyTeam;
    }
}