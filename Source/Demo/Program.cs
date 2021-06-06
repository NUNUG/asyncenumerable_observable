using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Fibonacci from AsyncEnumerable");
			await foreach(int number in Fibonacci(10))
			{
				Console.WriteLine(number);
			}
		}

		public static async IAsyncEnumerable<int> Fibonacci(int max)
		{
			int current = 1;
			int previous = 0;
			int count = 0;

			while(count++ < max)
			{
				await Task.Delay(200);
				yield return previous;

				int temp = previous;
				previous = current;
				current = temp + current;
			}
		}
	}
}
