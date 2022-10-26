using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;

namespace Components
{
	public interface IClean : INukeBuild
	{
		[Parameter] AbsolutePath SourceDirectory => TryGetValue(() => SourceDirectory) ?? RootDirectory / "src";
		[Parameter] AbsolutePath RefsDirectory => TryGetValue(() => RefsDirectory) ?? RootDirectory / "src" / "Refs";

		Target Clean => _ => _
			.Executes(() =>
			{
				SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(EnsureCleanDirectory);
				EnsureCleanDirectory(RefsDirectory);
			});
	}
}