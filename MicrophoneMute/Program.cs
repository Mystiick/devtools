using System;
using NAudio.CoreAudioApi;

ConsoleColor defaultColor = Console.ForegroundColor;

using var mmEnum = new MMDeviceEnumerator();
MMDeviceCollection devices = mmEnum.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active | DeviceState.Disabled);

// Print out an indexed list of audio devices
for (int i = 0; i < devices.Count; i++)
{
    try
    {
        using var device = devices[i];
        Console.Write($"{i}) {device.FriendlyName} - Muted: ");
        Console.ForegroundColor = device.AudioEndpointVolume.Mute ? ConsoleColor.Red : ConsoleColor.Green;
        Console.Write(device.AudioEndpointVolume.Mute);
    }
    catch
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"Device {i} Failed");
    }

    // Reset console and continue on with the loops
    Console.ForegroundColor = defaultColor;
    Console.WriteLine();
}

// Get input from user
int choice = -1;
do
{
    Console.Write($"Choose a device to toggle (0 - {devices.Count-1}): ");
} while (!int.TryParse(Console.ReadLine(), out choice) || choice > devices.Count - 1 || choice < 0);

// Actually mute the device
using var endpoint = devices[choice].AudioEndpointVolume;
endpoint.Mute = !endpoint.Mute;

Console.WriteLine($"Toggled {devices[choice].FriendlyName} to Muted: {endpoint.Mute}");