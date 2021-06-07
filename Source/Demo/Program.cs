using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Demo
{
	class Program
	{
		static async Task Main(string[] args)
		{
			await FizzBuzz();
			await FromEvent();

			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
		}

		public static async Task FizzBuzz()
		{
			var observable = Observable.Range(1, 100);

			var by3 = observable.Where(i => i % 3 == 0).Select(i => "Fizz");
			var by5 = observable.Where(i => i % 5 == 0).Select(i => "Buzz");
			var number = observable.Where(i => i % 5 != 0 && i % 3 != 0).Select(i => i.ToString());

			var specialCases = by3.Merge(by5);
			var collection = number.Merge(by3).Merge(by5)
			.ToEnumerable();

			Console.WriteLine(string.Join(",", collection));
		}

		public static async Task FromEvent()
		{
			var tester = new Tester();
			var observable = Observable.FromEventPattern<TesterEventArgs>(d => tester.OnNotify += d, d => tester.OnNotify -= d);
			observable.Subscribe(args => Console.WriteLine($"Event fired. Id: {args.EventArgs.Id}; Name: {args.EventArgs.Name}"), () => Console.WriteLine("Subscription ended."));

			tester.Start();
			await Task.Delay(TimeSpan.FromSeconds(3));
			tester.End();
		}
	}

	public class Tester
	{
		public event EventHandler<TesterEventArgs> OnNotify;
		protected CancellationTokenSource CancellationSource { get; set; }

		public void Start()
		{
			if (CancellationSource != null)
				CancellationSource.Cancel();

			CancellationSource = new CancellationTokenSource();
			Task.Run(async () =>
			{
				int count = 0;
				var local = CancellationSource;

				while (!local.IsCancellationRequested)
				{
					OnNotify?.Invoke(this, new TesterEventArgs { Id = count, Name = $"Event {count}" });
					count++;
					await Task.Delay(500);
				}
			});
		}

		public void End()
		{
			CancellationSource.Cancel();
			CancellationSource = null;
		}
	}

	public class TesterEventArgs
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}
