using System.IO;

namespace MvvmHelper.Commands
{
    [Command(PackageIds.GenerateViewModelCommand)]
    internal sealed class GenerateViewModelCommand : BaseCommand<GenerateViewModelCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            SolutionItem activeItem = await VS.Solutions.GetActiveItemAsync();
            string fullPath = string.Empty;
            string targetFileName = string.Empty;
            string targetPath = string.Empty;
            string filePath = activeItem?.FullPath;
            string fileName = string.Empty;
            if (filePath != null)
            {
                fullPath = Path.GetDirectoryName(filePath);
            }

            if (fullPath != null)
            {
                targetPath = fullPath.Replace("Views", "ViewModels");
            }

            if (activeItem != null)
            {
                fileName = Path.GetFileNameWithoutExtension(filePath);
                targetFileName =fileName + "ViewModel.cs";
            }

            CreateViewModelFileAsync(targetPath, targetFileName,fileName);
            Project project = await VS.Solutions.GetActiveProjectAsync();

            if (project != null) await project.AddExistingFilesAsync(new[]{targetPath+targetFileName});

        }

        private static void CreateViewModelFileAsync(string filePath, string fileName,string viewName)
        {
            if (!File.Exists(filePath+fileName))
            {
                File.Create(filePath + fileName);
            }

        }
    }
}