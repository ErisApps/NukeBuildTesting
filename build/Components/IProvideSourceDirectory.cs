using Nuke.Common;
using Nuke.Common.IO;

namespace Components;

public interface IProvideSourceDirectory : INukeBuild
{
	[Parameter]
	AbsolutePath SourceDirectory => TryGetValue(() => SourceDirectory) ?? RootDirectory;
}