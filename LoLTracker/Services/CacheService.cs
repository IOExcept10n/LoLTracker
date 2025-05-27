using System.Collections.Generic;
using System.Threading.Tasks;
using LoLTracker.Models;
using LoLTracker.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace LoLTracker.Services
{
    internal class CacheService(LeagueContext repository, RiotApi api)
    {
        private readonly LeagueContext repository = repository;
        private readonly RiotApi api = api;

        public async Task<MatchDto> GetMatchInfoAsync(string matchId)
        {
            var cachedMatch = await repository.Matches.FindAsync(long.Parse(matchId[3..]));
            if (cachedMatch != null)
            {
                await repository.Entry(cachedMatch)
                    .Collection(x => x.Teams)
                    .LoadAsync();
                await repository.Entry(cachedMatch)
                    .Collection(x => x.Participants)
                    .LoadAsync();
                return cachedMatch;
            }

            var matchInfo = await api.GetMatchInfo(matchId);
            repository.Matches.Add(matchInfo);
            await repository.SaveChangesAsync();

            return matchInfo;
        }

        public async Task<string[]> GetMatchIdsAsync(string puuid, int start, int count = 20) =>
            await api.GetMatchIds(puuid, start, count);

        public async Task<IEnumerable<MatchDto>> GetMatchesAsync(string puuid, int start = 0, int count = 20)
        {
            var matchIds = await GetMatchIdsAsync(puuid, start, count);
            var matches = new List<MatchDto>();

            foreach (var matchId in matchIds)
            {
                var matchInfo = await GetMatchInfoAsync(matchId);
                matches.Add(matchInfo);
            }

            return matches;
        }

        public async Task<RiotAccountDetails> GetRiotAccountDetailsAsync(string riotId)
        {
            var cachedAccount = await repository.Accounts.FirstOrDefaultAsync(a => a.RiotId == riotId);
            if (cachedAccount != null)
            {
                return cachedAccount;
            }

            var puuid = await api.GetAccountFromRiotID(riotId);

            var leagueAccountId = await api.GetLeagueSummonerId(puuid);

            var accountDetails = new RiotAccountDetails
            {
                RiotId = riotId,
                Puuid = puuid,
                LeagueAccountId = leagueAccountId
            };

            repository.Accounts.Add(accountDetails);
            await repository.SaveChangesAsync();

            return accountDetails;
        }

    }
}
