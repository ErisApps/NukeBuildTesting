using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions("Build",
	GitHubActionsImage.UbuntuLatest,
	AutoGenerate = true,
	OnPushBranches = new[] { "master" },
	InvokedTargets = new[] { nameof(Compile) },
	OnPullRequestBranches = new[] { "master" })]
class Build : NukeBuild
{
	/// Support plugins are available for:
	///   - JetBrains ReSharper        https://nuke.build/resharper
	///   - JetBrains Rider            https://nuke.build/rider
	///   - Microsoft VisualStudio     https://nuke.build/visualstudio
	///   - Microsoft VSCode           https://nuke.build/vscode
	public static int Main() => Execute<Build>(x => x.Compile);

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")] readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Secret] [Parameter(Name = "SIRA_SERVER_CODE")] readonly string SiraServerCode;

	[Solution(GenerateProjects = true)] readonly Solution Solution;

	Target Clean => _ => _
		.Before(Restore)
		.Executes(() =>
		{
			DotNetClean(settings => settings
				.SetConfiguration(Solution.Configurations.)
				.SetProject(Solution));
		});

	Target Restore => _ => _
		.Executes(() =>
		{
			DotNetRestore(settings => settings
				.SetProjectFile(Solution));
		});

	Target Compile => _ => _
		.DependsOn(Restore)
		.Executes(() => DotNetBuild(settings => settings
			.SetConfiguration(Configuration)
			.SetProjectFile(Solution)
			.EnableNoRestore()
		));
}