using System.IO;
using System.Text;
using Microsoft.VisualStudio.Shell;
using System.Threading.Tasks;
using Microsoft.VisualStudio;

namespace MvvmHelper.Commands
{
    // Command to generate a ViewModel from a View file
    [Command(PackageIds.GenerateViewModelCommand)]
    internal sealed class GenerateViewModelCommand : BaseCommand<GenerateViewModelCommand>
    {
        // Execute the command asynchronously
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            SolutionItem activeItem = await VS.Solutions.GetActiveItemAsync();
            if (activeItem == null || string.IsNullOrEmpty(activeItem.FullPath))
                throw new InvalidOperationException("No active item selected.");

            Project project = await VS.Solutions.GetActiveProjectAsync();

            string viewModelFilePath = GetViewModelFilePath(activeItem.FullPath);

            await CreateViewModelFileAsync(viewModelFilePath, project);

            if (project != null)
                await project.AddExistingFilesAsync(viewModelFilePath);
            
        }

        // Generate the ViewModel file path
        private static string GetViewModelFilePath(string viewFilePath)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(viewFilePath);
            string viewModelFileName = $"{fileNameWithoutExtension}ViewModel.cs";

            string directoryPath = Path.GetDirectoryName(viewFilePath);
            string viewModelDirectory = directoryPath.Replace("Views", "ViewModels");

            Directory.CreateDirectory(viewModelDirectory);

            return Path.Combine(viewModelDirectory, viewModelFileName);
        }

        private static async Task CreateViewModelFileAsync(string filePath, Project project)
        {
           
            if (!File.Exists(filePath))
            {
                (string className, string nameSpace) value = GetClassNameAndNamespace(filePath, project);
                string className = value.className;
                string nameSpace = value.nameSpace;

                // Create the ViewModel file content
                string content = $@"
using System;

namespace {nameSpace}
{{
    public class {className}
    {{
        public {className}() 
        {{
            // Constructor
            Console.WriteLine(""{className} has been instantiated!"");
        }}

        public void Greet()
        {{
            Console.WriteLine(""Hello from {className}!"");
        }}
    }}
}}";

                using StreamWriter writer = new(filePath, false, Encoding.UTF8);
                await writer.WriteAsync(content);
            }
        }

        private static (string className, string nameSpace) GetClassNameAndNamespace(string filePath, Project project)
        {
            string projectRoot = Path.GetDirectoryName(project.FullPath);

            string projectName = Path.GetFileNameWithoutExtension(project.FullPath);

            string relativePath = GetRelativePath(projectRoot, filePath);

            string className = Path.GetFileNameWithoutExtension(filePath);

            string namespaceName = Path.GetDirectoryName(relativePath)?.Replace("\\", ".");

            namespaceName = $"{projectName}.{namespaceName}";

            return (className, namespaceName);
        }

        // Manually calculate the relative path between two file paths
        private static string GetRelativePath(string fromPath, string toPath)
        {
            Uri fromUri = new(fromPath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
            Uri toUri = new(toPath);
            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            return Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', Path.DirectorySeparatorChar);
        }

    }
}
