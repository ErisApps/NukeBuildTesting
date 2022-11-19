using System.IO.Compression;
using System.Net.Http;
using Helpers;
using Nuke.Common;
using Serilog;

namespace Components;

interface IDownloadGameRefs : IProvideRefsDirectory
{
	private const string PROJECT_SIRA_CDN_URL = "https://cdn.project-sira.tech/gate";

	private static string? GameVersion => IDeserializeManifest.Manifest?.GameVersion;

	[Secret]
	[Parameter(Name = "SIRA_SERVER_CODE")]
	string? SiraServerCode => TryGetValue(() => SiraServerCode);

	Target DownloadGameRefs => _ => _
		.DependsOn<IDeserializeManifest>(x => x.DeserializeManifest)
		.Requires(() => !string.IsNullOrWhiteSpace(SiraServerCode))
		.OnlyWhenDynamic(() => !string.IsNullOrWhiteSpace(GameVersion))
		.Executes(async () =>
		{
			var urlBuilder = new UrlBuilder(PROJECT_SIRA_CDN_URL)
				.AppendQueryParam("code", SiraServerCode!)
				.AppendQueryParam("id", IDeserializeManifest.Manifest!.GameVersion);

			Log.Information("Downloading game refs for version {GameVersion}", GameVersion);
			using var httpClient = new HttpClient();
			using var gameRefsResponse = await httpClient.GetAsync(urlBuilder, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
			gameRefsResponse.EnsureSuccessStatusCode();

			Log.Information("Extracting game refs to {ExtractionPath}", RefsDirectory);
			await using var gameRefsStream = await gameRefsResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
			using var zipArchive = new ZipArchive(gameRefsStream);
			zipArchive.ExtractToDirectory(RefsDirectory);

			Log.Information("Finished extracting game refs");
		});
}