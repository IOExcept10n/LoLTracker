using System;
using System.Threading.Tasks;
using AsyncImageLoader.Loaders;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Diagnostics;

namespace LoLTracker.Services
{
    internal class IconsService(RiotApi api)
    {
        private static readonly Bitmap AllyIcon = Bitmap.DecodeToWidth(AssetLoader.OpenAndGetAssembly(new Uri("avares://LoLTracker/Assets/ally.jpg")).stream, 512);
        private static readonly Bitmap EnemyIcon = Bitmap.DecodeToWidth(AssetLoader.OpenAndGetAssembly(new Uri("avares://LoLTracker/Assets/enemy.jpg")).stream, 512);

        private static readonly BaseWebImageLoader loader = new DiskCachedWebImageLoader();

        private string? version;

        public void InitializeVersion(string version)
        {
            this.version = version;
        }

        public async Task<Bitmap> GetAllyIconAsync(PlayerStats stats)
        {
            Guard.IsNotNull(version);
            string? iconUrl = await api.GetChampionIconLinkAsync(stats.ChampionId, version);
            if (iconUrl == null)
                return AllyIcon;
            return await loader.ProvideImageAsync(iconUrl) ?? AllyIcon;
        }

        public async Task<Bitmap> GetEnemyIconAsync(PlayerStats stats)
        {
            Guard.IsNotNull(version);
            string? iconUrl = await api.GetChampionIconLinkAsync(stats.ChampionId, version);
            if (iconUrl == null)
                return EnemyIcon;
            return await loader.ProvideImageAsync(iconUrl) ?? EnemyIcon;
        }
    }
}
