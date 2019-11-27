﻿using System;
using scopely.msgpacksharp.tests;
using scopely.msgpacksharp;
using System.IO;

namespace PerfTest
{
	class MainClass
	{
		private const int numMessagesToTest = 10000;
		private static AnimalMessage testMsg;

		public static void Main(string[] args)
		{
			testMsg = AnimalMessage.CreateTestMessage();
			TimeTest(TestCli, "Serialize and Deserialize with Official CLI");
		}

		private static void TimeTest(Action test, string testName)
		{
			DateTime start = DateTime.Now;
			test();
			DateTime end = DateTime.Now;
			TimeSpan diff = end.Subtract(start);
			double numPerSecond = (double)numMessagesToTest / diff.TotalSeconds;
			Console.Out.WriteLine(testName + ":\n\t{0:n0} messages per second", numPerSecond);
		}

		private static void TestCli()
		{
			var serializer = MsgPack.Serialization.MessagePackSerializer.Create<AnimalMessage>();
			for (int i = 0; i < numMessagesToTest; i++)
			{
				byte[] buffer = null;
				using (MemoryStream outStream = new MemoryStream())
				{	
					serializer.Pack(outStream, testMsg);
					buffer = outStream.ToArray();
				}
				using (MemoryStream stream = new MemoryStream(buffer))
				{
#pragma warning disable 0219
					var deserialized = serializer.Unpack(stream);
#pragma warning restore 0219
				}
			}
		}
	}
}
