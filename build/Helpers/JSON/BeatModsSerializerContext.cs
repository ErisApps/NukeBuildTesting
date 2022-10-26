using System.Collections.Generic;
using System.Text.Json.Serialization;
using Models.BeatMods;

namespace Helpers.JSON
{
	[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]
	[JsonSerializable(typeof(List<string>))]
	[JsonSerializable(typeof(Dictionary<string, List<string>>))]
	[JsonSerializable(typeof(List<Mod>))]
	internal partial class BeatModsSerializerContext : JsonSerializerContext
	{
	}
}
