using Nuke.Common;
using static Nuke.Common.IO.FileSystemTasks;

namespace Components
{
	public interface IClean : IProvideRefsDirectoryPath
	{
		Target Clean => _ => _
			.Executes(() =>
			{
				EnsureCleanDirectory(RefsDirectory);
			});
	}
}