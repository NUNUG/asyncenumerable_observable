using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Demo
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Fibonacci from AsyncEnumerable");
			await foreach(int number in FibonacciAsync(10))
			{
				Console.WriteLine(number);
			}

			Console.WriteLine("\nFibonacci from Observable");

			var observable = FibonacciObservable(10);

			using var subscription = observable
			.Subscribe(value => Console.WriteLine(value));

			await observable;

			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
		}

		public static async IAsyncEnumerable<int> FibonacciAsync(int max)
		{
			int next = 1;
			int current = 0;
			int count = 0;

			while (count++ < max)
			{
				await Task.Delay(200);
				yield return current;

				int temp = current;
				current = next;
				next = temp + next;
			}
		}

		public static IObservable<int> FibonacciObservable(int max)
		{
			int current = 0;
			int next = 1;

			return Observable.Range(0, max)
			.Select(count =>
			{
				int value = current;

				current = next;
				next = value + current;

				return value;
			});
		}
	}
}
