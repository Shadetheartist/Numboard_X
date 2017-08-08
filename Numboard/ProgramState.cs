using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System;

namespace Numboard
{
	public sealed class ProgramState
	{
		private static readonly Lazy<ProgramState> lazy = new Lazy<ProgramState>(() => new ProgramState());
		public static ProgramState Instance => lazy.Value;

		private ProgramState()
		{
		}

		public const string stateFileName = "program-state.json";

		public List<string> SaveFiles { get; set; } = new List<string>();
		public string DefaultSaveFile { get; set; }
		public int? PrimaryOutputDevice { get; set; }
		public int? SecondaryOutputDevice { get; set; }
		public double MasterVolume { get; set; } = 1;

		public void Save()
		{
			var path = AppDomain.CurrentDomain.BaseDirectory + stateFileName;

			using (var writer = new StreamWriter(path, false))
			{
				writer.Write(JsonConvert.SerializeObject(Instance));
			}
		}

		public void Load()
		{
			var path = AppDomain.CurrentDomain.BaseDirectory + stateFileName;

			if (!File.Exists(path)) {
				Save();
			}

			using (var reader = new StreamReader(path))
			{
				var loadedProgramState = JsonConvert.DeserializeObject<ProgramState>(reader.ReadToEnd());
				loadedProgramState?.CopyProperties(Instance);
			}
		}
	}
}
