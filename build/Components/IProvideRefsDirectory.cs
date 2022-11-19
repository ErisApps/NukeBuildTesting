using Nuke.Common;
using Nuke.Common.IO;

namespace Components
{
	public interface IProvideRefsDirectory : IProvideSourceDirectory
	{
		[Parameter]
		AbsolutePath RefsDirectory => TryGetValue(() => RefsDirectory) ?? SourceDirectory / "Refs";
	}
}