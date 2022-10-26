using Nuke.Common;
using Nuke.Common.IO;

namespace Components
{
	public interface IProvidePaths : INukeBuild
	{
		[Parameter]
		AbsolutePath SourceDirectory => TryGetValue(() => SourceDirectory) ?? RootDirectory / "src";

		[Parameter]
		AbsolutePath RefsDirectory => TryGetValue(() => RefsDirectory) ?? RootDirectory / "src" / "Refs";
	}
}