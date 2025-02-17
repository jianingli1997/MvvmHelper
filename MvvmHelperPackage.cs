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
        ["Xaml"],
        ["HierSingleSelectionName:.xaml$"])]
    // [ProvideUIContextRule("660bc25f-3a95-48e6-ac0d-a8b8eda0fbce",
    //    "Supported Files",
    //     "CSharp | VisualBasic",
    //     ["CSharp", "VisualBasic"],
    //     ["HierSingleSelectionName:.cs$", "HierSingleSelectionName:.vb$"])]
    public sealed class MvvmHelperPackage : ToolkitPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.RegisterCommandsAsync();
        }
    }
}