using Microsoft.EntityFrameworkCore;
using LoLTracker.Models.Dto;

namespace LoLTracker.Models
{
    public class LeagueContext : DbContext
    {
        public DbSet<TeamDto> Teams { get; set; }
        public DbSet<BanDto> Bans { get; set; }
        public DbSet<ParticipantDto> Participants { get; set; }
        public DbSet<MatchDto> Matches { get; set; }
        public DbSet<RiotAccountDetails> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeamDto>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<BanDto>()
                .HasKey(b => new { b.ChampionId, b.PickTurn });

            // Настройка отношений, если необходимо
            modelBuilder.Entity<TeamDto>()
                .HasMany(t => t.Bans)
                .WithOne()
                .HasForeignKey(b => b.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ParticipantDto>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<MatchDto>()
                .HasKey(m => m.GameId);
            modelBuilder.Entity<MatchDto>()
                .HasMany(m => m.Participants)
                .WithOne(p => p.Match)
                .HasForeignKey(p => p.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RiotAccountDetails>()
                .HasKey(r => r.Puuid);
            modelBuilder.Entity<RiotAccountDetails>()
                .HasIndex(x => x.RiotId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=cache.db");
        }
    }

}
