namespace Wallets.Api.Extensions;

public static class ServicesExtensions
{
    public static void LoadEnv(this ConfigurationManager configuration, string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException(".env not found by path: " + path);
        foreach (var line in File.ReadAllLines(path))
        {
            var keyValue = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
            if (keyValue.Length == 2)
                Environment.SetEnvironmentVariable(keyValue[0], keyValue[1]);
        }
        configuration.AddEnvironmentVariables();
    }
}