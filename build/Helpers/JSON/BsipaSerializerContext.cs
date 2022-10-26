using System.Text.Json.Serialization;
using Models.BSIPA;

namespace Helpers.JSON
{
	[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]
	[JsonSerializable(typeof(PluginManifest))]
	internal partial class BsipaSerializerContext : JsonSerializerContext
	{
	}
}
