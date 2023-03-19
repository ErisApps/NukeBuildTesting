using BeatSaberModdingTools.Nuke.Components;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
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
			EnsureCleanDirectory(ArtifactsDirectory);
		});

	Target GrabRefs => _ => _
		.After(RestorePackages)
		.OnlyWhenStatic(() => false)
		.WhenSkipped(DependencyBehavior.Skip)
		.DependsOn<IDownloadGameRefs>()
		.DependsOn<IDownloadBeatModsDependencies>();

	Target RestorePackages => _ => _
		.DependsOn<IClean>()
		.Executes(() => DotNetRestore(settings => settings.SetProjectFile(Solution.MorePrecisePlayerHeight)));

	AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

	Target Compile => _ => _
		.DependsOn(RestorePackages)
		.DependsOn(GrabRefs)
		.Produces(ArtifactsDirectory / "*.zip")
		.Executes(() =>
		{
			DotNetBuild(settings => settings
				.EnableNoRestore()
				.SetProjectFile(Solution.MorePrecisePlayerHeight)
				.SetConfiguration(Configuration)
				.SetVersion(GitVersion.FullSemVer)
				.SetAssemblyVersion(GitVersion.AssemblySemVer)
				.SetFileVersion(GitVersion.AssemblySemFileVer)
				.SetInformationalVersion(GitVersion.InformationalVersion)
				.SetProperty("ZipDestinationDirectory", ArtifactsDirectory));
		});
}