using Nuke.Common;
using Nuke.Common.IO;

namespace Components
{
	public interface IProvideRefsDirectoryPath : INukeBuild
	{
		[Parameter]
		AbsolutePath RefsDirectory => TryGetValue(() => RefsDirectory) ?? RootDirectory / "Refs";
	}
}