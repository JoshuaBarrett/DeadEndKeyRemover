using DeadEndKeyRemover;
using Microsoft.Win32;
using System.Runtime.InteropServices;

string default_uninstallKeyName = "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall";

Console.WriteLine("Welcome to Dead-End Key Remover App\n");

if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    Console.WriteLine("Detected non-windows OS");
} else
{
    Console.WriteLine($"Enter the Registry path to the Uninstall Keys or press enter for Default: \n({default_uninstallKeyName})");
    string uninstallKeyName = Console.ReadLine() ?? default_uninstallKeyName;

    Console.WriteLine($"Enter filter for display name, this is the name of the app as it appears under Apps & Features: ");
    string displayNameFilter = Console.ReadLine() ?? string.Empty;

    RegistryClient registryClient = new RegistryClient(uninstallKeyName);
    string[] deadEndKeys = registryClient.GetDeadEndKeys(displayNameFilter);

    Console.WriteLine("The following Display Names have been found to be Dead End keys within the Registry:-");
    foreach(string k in deadEndKeys)
    {
        Console.WriteLine(k);
    }
    Console.WriteLine("Confirm removal of these keys (y / n): ");
    string removalResponse = Console.ReadLine() ?? string.Empty;
    if (removalResponse.Equals("y", StringComparison.CurrentCultureIgnoreCase))
    {
        registryClient.RemoveSubKeys(deadEndKeys);
        Console.WriteLine("Dead End keys removed");
    } else
    {
        Console.WriteLine("Removal canceled");
    }
}

Console.Write("\nPress any key to exit");
Console.ReadKey();
