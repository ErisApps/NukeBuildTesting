using BeatSaberModdingTools.Nuke.Components;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[ShutdownDotNetAfterServerBuild]
partial class Build : NukeBuild, IClean, IDeserializeManifest, IDownloadGameRefs, IDownloadBeatModsDependencies
{
	/// Support plugins are available for:
	///   - JetBrains ReSharper        https://nuke.build/resharper
	///   - JetBrains Rider            https://nuke.build/rider
	///   - Microsoft VisualStudio     https://nuke.build/visualstudio
	///   - Microsoft VSCode           https://nuke.build/vscode
	public static int Main() => Execute<Build>(x => x.Compile);

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")] readonly Configuration Configuration = IsLocalBuild ? Configuration.Release : Configuration.Release;

	[Solution(GenerateProjects = true)] readonly Solution Solution;

	[GitVersion] readonly GitVersion GitVersion;

	Target IClean.Clean => _ => _
		.Inherit<IClean>()
		.Executes(() =>
		{
			DotNetClean(s => s.SetProject(Solution.MorePrecisePlayerHeight));
		});

	Target GrabRefs => _ => _
		.After(RestorePackages)
		.OnlyWhenStatic(() => IsServerBuild)
		.WhenSkipped(DependencyBehavior.Skip)
		.DependsOn<IDownloadGameRefs>()
		.DependsOn<IDownloadBeatModsDependencies>();

	Target RestorePackages => _ => _
		.DependsOn<IClean>()
		.Executes(() => DotNetRestore(settings => settings.SetProjectFile(Solution.MorePrecisePlayerHeight)));

	Target Compile => _ => _
		.DependsOn(RestorePackages)
		.DependsOn(GrabRefs)
		.Executes(() =>
		{
			DotNetBuild(settings => settings
				.EnableNoRestore()
				.SetProjectFile(Solution.MorePrecisePlayerHeight)
				.SetConfiguration(Configuration)
				.SetVersion(GitVersion.FullSemVer)
				.SetAssemblyVersion(GitVersion.AssemblySemVer)
				.SetFileVersion(GitVersion.AssemblySemFileVer)
				.SetInformationalVersion(GitVersion.InformationalVersion));
		});
}