using DeadEndKeyRemover;
using Microsoft.Win32;
using System.Runtime.InteropServices;

string default_uninstallKeyName = "SOFTWARE\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall";

Console.WriteLine("Welcome to Dead-End Key Remover App");

if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    Console.WriteLine("Detected non-windows OS");
} else
{
    Console.WriteLine($"\nEnter the Registry path to the Uninstall Keys or press enter for Default: \n({default_uninstallKeyName})");
    string? registryPathInput = Console.ReadLine();
    string uninstallKeyName = string.IsNullOrEmpty(registryPathInput) ? default_uninstallKeyName : registryPathInput;

    Console.WriteLine($"\nEnter filter for display name, this is the name of the app as it appears under Apps & Features: ");
    string? displayNameFilterInput = Console.ReadLine();
    string displayNameFilter = string.IsNullOrEmpty(displayNameFilterInput) ? string.Empty : displayNameFilterInput;

    RegistryClient registryClient = new RegistryClient(uninstallKeyName);
    string[] deadEndKeys = registryClient.GetDeadEndKeys(displayNameFilter);

    if (deadEndKeys.Length == 0)
    {
        Console.WriteLine("\nZero dead end keys were found");
    } else
    {
        Console.WriteLine("\nThe following Display Names have been found to be Dead End keys within the Registry:-");
        foreach (string k in deadEndKeys)
        {
            Console.WriteLine(k);
        }
        Console.WriteLine("\nConfirm removal of these keys (y / n): ");
        string removalResponse = Console.ReadLine() ?? string.Empty;
        if (removalResponse.Equals("y", StringComparison.CurrentCultureIgnoreCase))
        {
            registryClient.RemoveSubKeys(deadEndKeys);
            Console.WriteLine("\nDead End keys removed");
        }
        else
        {
            Console.WriteLine("\nRemoval canceled");
        }
    }    
}

Console.Write("\nPress any key to exit");
Console.ReadKey();
