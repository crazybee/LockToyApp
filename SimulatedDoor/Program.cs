// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub device SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/main/iothub/device/samples

using System.Text;
using System.Text.Json;
using Microsoft.Azure.Devices.Client;
using CommandLine;
using ToyContracts;

namespace SimulatedDoor
{
    /// <summary>
    /// This sample illustrates the very basics of a device app sending telemetry. For a more comprehensive device app sample, please see
    /// <see href="https://github.com/Azure-Samples/azure-iot-samples-csharp/tree/main/iot-hub/Samples/device/DeviceReconnectionSample"/>.
    /// </summary>
    internal class Program
    {
        private static DoorStatus doorStatus = new DoorStatus();
        private static async Task Main(string[] args)
        {
            Parameters parameters = null;
            ParserResult<Parameters> result = Parser.Default.ParseArguments<Parameters>(args)
                .WithParsed(parsedParams => parameters = parsedParams)
                .WithNotParsed(errors => Environment.Exit(1));

            Console.WriteLine("IoT Hub Quickstarts #1 - Simulated device.");

            // Connect to the IoT hub using the MQTT protocol by default
            using var deviceClient = DeviceClient.CreateFromConnectionString(parameters.PrimaryConnectionString, parameters.TransportType);

            // Run the telemetry loop
            await ReceiveLoop(deviceClient);
            await deviceClient.CloseAsync();
            Console.WriteLine("Device simulator finished.");
        }

        // Async method to send simulated telemetry
        private static async Task ReceiveLoop(DeviceClient deviceClient)
        {     
            try
            {
                while (true)
                {     
                    Message receivedMessage = await deviceClient.ReceiveAsync();
                    if (receivedMessage != null)
                    {
                        var msgString = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Received message: {0}",msgString);
                        Console.ResetColor();
                        var receivedMsgObj = JsonSerializer.Deserialize<DoorOpRequest>(msgString);
                        doorStatus.Operation = receivedMsgObj?.Command ?? "na";
                        doorStatus.UserName = receivedMsgObj?.UserName ?? "na";
                        doorStatus.UserId = receivedMsgObj?.UserId.ToString() ?? "na";
                        doorStatus.Status = "ok";

                        Console.WriteLine($"Door operation request received from user {doorStatus.UserName} with operation {doorStatus.Operation} current status is {doorStatus.Status}");
                        await deviceClient.CompleteAsync(receivedMessage);

                    }
                    else 
                    {
                        continue;
                    }
                }
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
            } 
        }
       
    }
}
