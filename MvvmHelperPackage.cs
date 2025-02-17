global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using System.Runtime.InteropServices;
using System.Threading;

namespace MvvmHelper
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.MvvmHelperString)]
    [ProvideUIContextRule("660bc25f-3a95-48e6-ac0d-a8b8eda0fbce", "XAML Files Only", "XAML",
        ["Xaml","Xaml.cs"],
        ["HierSingleSelectionName:.xaml$", "!HierSingleSelectionName:.xaml.cs$"],500)]
    public sealed class MvvmHelperPackage : ToolkitPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.RegisterCommandsAsync();
        }
    }
}