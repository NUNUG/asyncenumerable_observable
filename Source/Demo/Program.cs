using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Demo
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var device = new BluetoothDevice();
			
			var random = new Random();
			byte[] file = new byte[20000];
			random.NextBytes(file);
			var mockFile = new MemoryStream(file);

			Console.WriteLine("Writing mock file to fake Bluetooth device");
			await foreach(var update in DoWorkAsync(device, mockFile))
			{
				if (update.State == DeviceOperationState.Initialized)
				{
					// Need to wait 1 second while the device sets up
					await Task.Delay(1000);
				}

				Console.WriteLine($"Updating device is {update.Progress * 100} finished. {update.Message}");
			}

			await Task.Delay(1000);
			Console.WriteLine("Update is complete.");

			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
		}

		public static async IAsyncEnumerable<(DeviceOperationState State, double Progress, string Message)> DoWorkAsync(BluetoothDevice device, Stream file)
		{
			yield return (DeviceOperationState.Ready, 0, "Starting operation...");
			await device.Initialize();

			yield return (DeviceOperationState.Initialized, 0.1, "Initialization complete. Starting transfer...");
			await foreach(double progress in TransferFileAsync(device, file))
			{
				yield return (DeviceOperationState.Transfer, (progress * 0.5) + 0.1, "Transfering file...");
			}

			yield return (DeviceOperationState.Transfer, 0.65, "Verifying file...");
			await device.Verify();
			yield return (DeviceOperationState.Rebooting, 1, "Operation complete. Device is rebooting.");
		}

		public static async IAsyncEnumerable<double> TransferFileAsync(BluetoothDevice device, Stream file)
		{
			int bytesWritten = 0;
			file.Seek(0, SeekOrigin.Begin);
			
			while (bytesWritten < file.Length)
			{
				int bytesToWrite = (int)Math.Min(512, file.Length - bytesWritten);
				byte[] chunk = new byte[bytesToWrite];

				file.Read(chunk, 0, bytesToWrite);

				await device.Write(chunk);
				bytesWritten += bytesToWrite;

				yield return (double)bytesWritten / file.Length;
			}

		}
	}

	public enum DeviceOperationState
	{
		Ready,
		Initialized,
		Transfer,
		Rebooting
	}
}
