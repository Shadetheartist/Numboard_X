using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numboard
{
	class PlayingStream : IDisposable
	{
		public NumboardButton Button { get; set; }
		public double ButtonVolumeWhenClicked { get; set; }

		public AudioFileReader PrimaryReader { get; set; }
		public AudioFileReader SecondaryReader { get; set; }
		public WaveOut PrimaryWaveOut { get; set; }
		public WaveOut SecondaryWaveOut { get; set; }

		public void Dispose()
		{
			PrimaryReader.Dispose();
			SecondaryReader.Dispose();
			PrimaryWaveOut.Dispose();
			SecondaryWaveOut.Dispose();
		}
	}
}
