using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
                targetFileName = fileName + "ViewModel.cs";
            }

            // Create ViewModel file asynchronously
            await CreateViewModelFileAsync(targetPath, targetFileName, fileName);

            Project project = await VS.Solutions.GetActiveProjectAsync();
            if (project != null)
            {
                // Add the newly created ViewModel file to the project
                await AddExistingFiles(activeItem, new[] { targetPath +"\\"+ targetFileName });
            }
        }

        public static async Task<IEnumerable<PhysicalFile>> AddExistingFiles(SolutionItem solutionItem, params string[] filePaths)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            solutionItem.GetItemInfo(out IVsHierarchy hierarchy, out uint itemId, out _);

            VSADDRESULT[] result = new VSADDRESULT[filePaths.Count()];
            IVsProject ip = (IVsProject)hierarchy;

            // Ensure that the directory structure exists
            string targetDirectory = Path.GetDirectoryName(filePaths[0]);
            Directory.CreateDirectory(targetDirectory); // This will automatically handle directory creation if it doesn't exist

            // Add files to the project
            ErrorHandler.ThrowOnFailure(ip.AddItem(itemId, VSADDITEMOPERATION.VSADDITEMOP_LINKTOFILE, string.Empty,
                (uint)filePaths.Count(), filePaths, IntPtr.Zero, result));

            List<PhysicalFile> files = new();

            foreach (string filePath in filePaths)
            {
                PhysicalFile? file = await PhysicalFile.FromFileAsync(filePath);
                if (file != null)
                {
                    files.Add(file);
                }
            }

            return files;
        }

        private static async Task CreateViewModelFileAsync(string filePath, string fileName, string viewName)
        {
            string path = Path.Combine(filePath, fileName);

            // Ensure the target directory exists
            Directory.CreateDirectory(filePath);

            if (!File.Exists(path))
            {
                // Use FileStream to create the file and immediately close it to avoid locks
                using (FileStream fs = File.Create(path))
                {
                    // Optionally write a template header or content here
                    byte[] content = System.Text.Encoding.UTF8.GetBytes("// Generated ViewModel for " + viewName);
                    await fs.WriteAsync(content, 0, content.Length);
                }
            }
        }
    }
}
