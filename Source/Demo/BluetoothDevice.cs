﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
	public class BluetoothDevice
	{
		protected int DataCount { get; set; }

		public BluetoothDevice()
		{
			DataCount = 0;
		}

		public async Task Initialize()
		{
			await Task.Delay(100);
		}

		public async Task Write(byte[] data)
		{
			DataCount += data.Length;
			await Task.Delay((int)(data.Length * 1.5));
		}

		public async Task<bool> Verify()
		{
			await Task.Delay(DataCount);
			return true;
		}
	}
}
