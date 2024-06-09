using System.Text.Json;
using Pulumi;
using SysEnvironment = System.Environment;

namespace Deploy.Infrastructure.Configuration;

public static class ConfigReader
{
    public static DeploymentConfig LoadConfig(string? configsDirectory)
    {
        var stack = Deployment.Instance.StackName;
        var configsPath = GetConfigsDirectoryPath(configsDirectory);
        var environments = LoadConfigPart<List<DeploymentEnvironment>>(configsPath, "deployment-environments.json");
        var tags = LoadConfigPart<DefaultTags>(configsPath, "default-tags.json");

        var deploymentEnv = environments.SingleOrDefault(x => x.Name == stack);

        if (deploymentEnv is null)
        {
            throw new Exception($"Unable to find deployment environment '{stack}'");
        }

        return new DeploymentConfig(stack)
        {
            AllDeploymentEnvironments = environments,
            DeploymentEnvironment = deploymentEnv,
            DefaultResourceTags = tags.Tags,
        };
    }

    private static T LoadConfigPart<T>(string workDir, string fileName)
    {
        var fullPath = Path.Combine(workDir, fileName);
        var text = File.ReadAllText(fullPath);

        return JsonSerializer.Deserialize<T>(text, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        })!;
    }

    private static string LoadConfigPartAsString(string workDir, string fileName)
    {
        var fullPath = Path.Combine(workDir, fileName);
        return File.ReadAllText(fullPath);
    }

    private static string GetConfigsDirectoryPath(string? configsDirectory)
    {
        var directorySplit = configsDirectory?.Split("/", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
        var folderName = directorySplit.FirstOrDefault() ?? "configs";

        var configsFolderPath = FindConfigsFolder(folderName);
        // we have found the folder, no need to go further
        if (directorySplit.Length <= 1)
        {
            return configsFolderPath;
        }

        // the sub-folder that we're looking for is the last one in the path split above
        var subFolderWithConfigs = directorySplit.Last();
        var index = 1;
        var currentPath = configsFolderPath;
        while (index < directorySplit.Length)
        {
            var directories = Directory.GetDirectories(currentPath);
            var path = directories.FirstOrDefault(x => x.EndsWith(subFolderWithConfigs));

            if (!string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            currentPath = Path.Combine(configsFolderPath, directorySplit[index]);
            index++;
        }

        return string.Empty;
    }

    private static string FindConfigsFolder(string folderName)
    {
        var workingDirectory = SysEnvironment.CurrentDirectory;
        var directories = Directory.GetDirectories(workingDirectory);

        var configsFolderPath = directories.FirstOrDefault(x => x.EndsWith(folderName));
        if (!string.IsNullOrWhiteSpace(configsFolderPath))
        {
            return configsFolderPath;
        }

        var parent = Directory.GetParent(workingDirectory);
        if (parent is null)
        {
            throw new Exception($"Unable to resolve deploy directory, was looking for {folderName}. " +
                                $"Make sure to create directory called '{folderName}' at the repo root level.");
        }

        directories = Directory.GetDirectories(parent.FullName);
        configsFolderPath = directories.FirstOrDefault(x => x.EndsWith(folderName));
        while (string.IsNullOrWhiteSpace(configsFolderPath))
        {
            parent = Directory.GetParent(parent.FullName);
            if (parent is null)
            {
                break;
            }

            directories = Directory.GetDirectories(parent.FullName);
            configsFolderPath = directories.FirstOrDefault(x => x.EndsWith(folderName));
        }

        if (string.IsNullOrWhiteSpace(configsFolderPath))
        {
            throw new Exception($"Unable to resolve deploy directory, was looking for {folderName}. " +
                                $"Make sure to create directory called '{folderName}' at the repo root level.");
        }

        return configsFolderPath;
    }
}
