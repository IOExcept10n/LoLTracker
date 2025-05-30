using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using LoLTracker.Models.Dto;
using LoLTracker.Utilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Splat;

namespace LoLTracker.Services
{
    internal class RiotApi(IConfiguration configuration) : IEnableLogger
    {
        private const string ApiKey = "api_key";

        private readonly ApiWorker worker = new(new());
        private readonly string key =
            Environment.GetEnvironmentVariable("ApiKey") ??
            configuration["ApiKey"] ??
            ThrowHelper.ThrowInvalidOperationException<string?>("Couldn't find an API key for the application.");

        private Dictionary<string, string> championIcons = null;

        /// <summary>
        /// Gets the encrypted <see langword="Riot PUUID"/> from the Riot ID.
        /// </summary>
        /// <param name="riotId">Player nickname in format <see langword="Nickname#Tag"/></param>
        /// <returns>An encrypted PUUID that can be used to access some API methods.</returns>
        public async Task<string> GetAccountFromRiotID(string riotId)
        {
            var parse = riotId.Split('#');
            (string name, string tagline) = (parse[0], parse[1]);
            string data = $"{name}/{tagline}";
            dynamic response = await worker.BuildRequest(Endpoints.EuAddress)
                                           .WithEndpoint(Endpoints.AccountByRiotID)
                                           .WithEndpoint(data)
                                           .WithParams(ApiKey, key)
                                           .CallAsync();
            return response.puuid;
        }

        /// <summary>
        /// Gets the unique ID for the LoL account.
        /// </summary>
        /// <param name="puuid">User PUUID, you can get it using <see cref="GetAccountFromRiotID(string)"/>.</param>
        /// <returns>An ID for the LoL account.</returns>
        public async Task<string> GetLeagueSummonerId(string puuid)
        {
            dynamic response = await worker.BuildRequest(Endpoints.RuAddress)
                                           .WithEndpoint(Endpoints.AccountByPUUID)
                                           .WithEndpoint(puuid)
                                           .WithParams(ApiKey, key)
                                           .CallAsync();
            return response.accountId;
        }

        /// <summary>
        /// Asynchronously gets the main matches info from Riot API.
        /// </summary>
        /// <param name="puuid">Player PUUID.</param>
        /// <param name="start">Starting index for search. You can use it to skip the last matches.</param>
        /// <param name="count">Maximal count of matches to get.</param>
        /// <returns>An asynchronous enumerable with all loaded matches info.</returns>
        public async IAsyncEnumerable<MatchDto> GetMatches(string puuid, int start = 0, int count = 20)
        {
            string[] matchIds = await GetMatchIds(puuid, start, count);
            foreach (var match in matchIds)
            {
                yield return await GetMatchInfo(match);
            }
        }

        /// <summary>
        /// Gets the info for a single match by its ID.
        /// </summary>
        /// <param name="id">ID of the match to get the info.</param>
        public async Task<MatchDto> GetMatchInfo(string id)
        {
            var response = await worker.BuildRequest(Endpoints.EuAddress)
                                       .WithEndpoint(Endpoints.MatchById)
                                       .WithEndpoint(id)
                                       .WithParams(ApiKey, key)
                                       .CallAsync();
            return response["info"]!.ToObject<MatchDto>()!;
        }

        /// <summary>
        /// Gets the IDs for matches of the specified player.
        /// </summary>
        /// <param name="puuid">Player ID to get matches to.</param>
        /// <param name="start">Starting index to get matches from.</param>
        /// <param name="count">Count of matches to get.</param>
        /// <returns>An array with IDs of the matches for the specified player.</returns>
        public async Task<string[]> GetMatchIds(string puuid, int start = 0, int count = 20)
        {
            var response = await worker.BuildRequest(Endpoints.EuAddress)
                                       .WithEndpoint(Endpoints.MatchesByPlayer)
                                       .WithEndpoint($"{puuid}/ids")
                                       .WithParams(ApiKey, key)
                                       .WithParams("start", start)
                                       .WithParams("count", count)
                                       .CallAsync();

            return [.. from x in response select x.ToString()];
        }

        private async Task<Dictionary<string, string>> GetChampionsDataAsync(string gameVersion)
        {
            dynamic response = await worker.BuildRequest(Endpoints.DataDragon.Api)
                               .WithEndpoint(Endpoints.DataDragon.Resources)
                               .WithEndpoint($"{gameVersion}/")
                               .WithEndpoint(Endpoints.DataDragon.ChampionsEndpoint)
                               .CallAsync();
            Dictionary<string, string> result = [];
            foreach (dynamic champion in response.data)
            {
                result.Add(champion.key, $"{Endpoints.DataDragon.ApiUrl}{Endpoints.DataDragon.Resources}{gameVersion}/{Endpoints.DataDragon.IconsEndpoint}{champion.image.full}");
            }
            return result;
        }

        public async Task<string?> GetChampionIconLinkAsync(int championId, string gameVersion)
        {
            championIcons ??= await GetChampionsDataAsync(gameVersion);
            championIcons.TryGetValue(championId.ToString(), out var result);
            return result;
        }

        private static class Endpoints
        {
            public const string ApiUrl = "api.riotgames.com/";
            public const string RuServer = "ru";
            public const string EuServer = "europe";
            public const string AccountByPUUID = "lol/summoner/v4/summoners/by-puuid/";
            public const string AccountByRiotID = "riot/account/v1/accounts/by-riot-id/";
            public const string EntriesBySummoner = "lol/league/v4/entries/by-summoner/";
            public const string MatchesByPlayer = "lol/match/v5/matches/by-puuid/";
            public const string MatchById = "lol/match/v5/matches/";

            public static class DataDragon 
            {
                public const string ApiUrl = "https://ddragon.leagueoflegends.com/";
                public const string Resources = "cdn/";
                public const string ChampionsEndpoint = "data/en_US/champion.json";
                public const string IconsEndpoint = "img/champion/";

                public static Uri Api = new(ApiUrl);
            }

            public static readonly Uri RuAddress = new($"https://{RuServer}.{ApiUrl}");
            public static readonly Uri EuAddress = new($"https://{EuServer}.{ApiUrl}");
        }
    }
}
