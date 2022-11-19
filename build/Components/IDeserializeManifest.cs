using System.IO;
using System.Text.Json;
using Helpers.JSON;
using Models.BSIPA;
using Nuke.Common;
using Nuke.Common.IO;
using Serilog;

namespace Components;

interface IDeserializeManifest : IProvideSourceDirectory
{
	static PluginManifest? Manifest { get; set; }

	[Parameter]
	AbsolutePath ManifestPath => TryGetValue(() => ManifestPath) ?? SourceDirectory / "manifest.json";

	Target DeserializeManifest => _ => _
		.TryAfter<IClean>()
		.Requires(() => !string.IsNullOrWhiteSpace(ManifestPath))
		.Executes(async () =>
		{
			Assert.FileExists(ManifestPath);

			Log.Information("Deserializing manifest");
			await using var fileStream = File.OpenRead(ManifestPath);
			Manifest = await JsonSerializer.DeserializeAsync(fileStream, BsipaSerializerContext.Default.PluginManifest).ConfigureAwait(false);
			Manifest.NotNull();

			Log.Information("Deserialized manifest | Name {Name} | Version {Version} | GameVersion {GameVersion}", Manifest!.Name, Manifest.Version, Manifest.GameVersion);
		});
}