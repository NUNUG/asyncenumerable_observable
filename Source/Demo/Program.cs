using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Demo
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Getting random numbers from a server");
			await foreach(int number in GetRandomNumbers().Take(10))
			{
				Console.WriteLine(number);
			}

			Console.WriteLine("\nStarting random number observable");
			var observable = StartGetRandomNumbers();

			using var sub = observable.Subscribe(i => Console.WriteLine(i));
			await observable;

			Console.Write("Press any key to continue...");
			Console.ReadKey();
		}

		public static async IAsyncEnumerable<int> GetRandomNumbers()
		{
			while (true)
			{
				string number = await "https://randommer.io/Number".PostUrlEncodedAsync(new Dictionary<string, string>
				{
					{ "Min", "1" },
					{ "Max", "100" }
				})
				.ReceiveString();

				yield return int.Parse(number);
			}
		}

		public static IObservable<int> StartGetRandomNumbers()
		{
			var subject = new Subject<int>();

			// Here we have some background task that is getting our data. This could be a connected peripheral,
			// a websocket, or just something that gets data periodically.
			Task.Run(async () =>
			{
				for (int i = 0; i < 10; ++i)
				{
					string number = await "https://randommer.io/Number".PostUrlEncodedAsync(new Dictionary<string, string>
					{
						{ "Min", "1" },
						{ "Max", "100" }
					})
					.ReceiveString();

					int value = int.Parse(number);
					subject.OnNext(value);

					await Task.Delay(TimeSpan.FromMilliseconds(200));
				}

				subject.OnCompleted();
				subject.Dispose();
			});

			return subject;
		}
	}
}
