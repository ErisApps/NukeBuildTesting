using Nuke.Common;
using Nuke.Common.IO;

namespace Components;

public interface IProvideSourceDirectory : INukeBuild
{
	[Parameter("Path to the source directory")]
	AbsolutePath SourceDirectory => TryGetValue(() => SourceDirectory) ?? RootDirectory;
}