using NAudio;
using NAudio.Wave;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Numboard
{
	public partial class MainWindow
	{
		private List<PlayingStream> PlayingStreams;
		
		public void Play(object sender, MouseButtonEventArgs e)
		{
			var button = (NumboardButton)sender;
			
			if (button?.Source == null)
			{
				return;
			}

			if (!File.Exists(button.Source))
			{
				MessageBox.Show("Cannot Find file '" + button.Source + "'. It was probably moved or deleted.");
				return;
			}

			var volume = button.Volume ?? 1;

			//primary ouput
			var primaryReader = new AudioFileReader(button.Source);
			primaryReader.Volume = (float)(volume * MasterVolume * PrimaryVolume);

			var primaryWaveOut = new WaveOut();
			primaryWaveOut.DeviceNumber = SelectedPrimaryOutputDevice;

			//secondary ouput
			var secondaryReader = new AudioFileReader(button.Source);
			secondaryReader.Volume = (float)(volume * MasterVolume * SecondaryVolume);

			var secondaryWaveOut = new WaveOut();
			secondaryWaveOut.DeviceNumber = SelectedSecondaryOutputDevice;

			primaryWaveOut.PlaybackStopped += StopStream;

			try
			{
				//we always want to init so we dont have to deal with dispose in a weird way
				primaryWaveOut.Init(primaryReader);

				//-1 = 'none'
				if (SelectedPrimaryOutputDevice != -1)
					primaryWaveOut.Play();

				secondaryWaveOut.Init(secondaryReader);

				//we don't want to play both to the same audio device, it creates a chorus effect
				if (SelectedPrimaryOutputDevice != SelectedSecondaryOutputDevice)
				{
					if (SelectedSecondaryOutputDevice != -1)
						secondaryWaveOut.Play();
				}

				if (button.Volume == null)
				{
					button.Volume = 1;
				}

				PlayingStreams.Add(new PlayingStream { Button = button, ButtonVolumeWhenClicked = (double)button.Volume, PrimaryReader = primaryReader, SecondaryReader = secondaryReader, PrimaryWaveOut = primaryWaveOut, SecondaryWaveOut = secondaryWaveOut });

			}
			catch (MmException ex)
			{
				primaryReader.Dispose();
				primaryWaveOut.Dispose();
				secondaryReader.Dispose();
				secondaryWaveOut.Dispose();

				if (ex.Result == MmResult.MemoryAllocationError)
				{
					StopAllStreams();
				}
			}
		}

		private void StopStream(object sender, StoppedEventArgs e)
		{
			PlayingStreams.Where(ps => ps.PrimaryWaveOut.Equals((WaveOut)sender)).ToList().ForEach(ps => ps.Dispose());
			PlayingStreams.RemoveAll(ps => ps.PrimaryWaveOut.Equals((WaveOut)sender));
		}

		private void StopStream(NumboardButton button)
		{
			PlayingStreams.Where(ps => ps.Button.Equals(button)).ToList().ForEach(ps => ps.Dispose());
			PlayingStreams.RemoveAll(ps => ps.Button.Equals(button));
		}

		private void StopAllStreams()
		{
			PlayingStreams.ForEach(ps => { ps.Dispose(); });
			PlayingStreams = new List<PlayingStream>();
		}

		private void StopAllStreams(object sender, MouseButtonEventArgs e)
		{
			StopAllStreams();
		}
	}
}
