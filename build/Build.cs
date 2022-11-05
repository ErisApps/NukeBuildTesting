using Components;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions("Build",
	GitHubActionsImage.UbuntuLatest,
	AutoGenerate = false,
	OnPushBranches = new[] { "main" },
	InvokedTargets = new[] { nameof(Compile) },
	PublishArtifacts = true,
	OnPullRequestBranches = new[] { "main" },
	CacheKeyFiles = new string[0],
	ImportSecrets = new[] { "SIRA_SERVER_CODE" })]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild, IClean, IDeserializeManifest, IDownloadGameRefs, IDownloadBeatModsDependencies
{
	/// Support plugins are available for:
	///   - JetBrains ReSharper        https://nuke.build/resharper
	///   - JetBrains Rider            https://nuke.build/rider
	///   - Microsoft VisualStudio     https://nuke.build/visualstudio
	///   - Microsoft VSCode           https://nuke.build/vscode
	public static int Main() => Execute<Build>(x => x.Compile);

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")] readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Solution(GenerateProjects = true)] readonly Solution Solution;

	[CI] readonly GitHubActions GitHubActions;

	Target IDownloadGameRefs.DownloadGameRefs => _ => _
		.TryAfter<IClean>()
		.Inherit<IDownloadGameRefs>();

	Target RestorePackages => _ => _
		.DependsOn<IClean>()
		.Executes(() => DotNetRestore(settings => settings.SetProjectFile(Solution.MorePrecisePlayerHeight)));

	Target Compile => _ => _
		.DependsOn(RestorePackages)
		.DependsOn<IDownloadGameRefs>()
		.DependsOn<IDownloadBeatModsDependencies>()
		.Executes(() =>
		{
			DotNetBuild(settings => settings
				.SetConfiguration(Configuration)
				.SetProjectFile(Solution.MorePrecisePlayerHeight)
				.EnableNoRestore());
		});
}