﻿using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Numboard
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			this.Closed += ClosingEventHandler;
			HaveChangesBeenMade = false;
			PlayingStreams = new List<PlayingStream>();

			ProgramState.Instance.Load();

			InitializeInputDevices();
			RememberPreviousSettings();
			PopulateFileList();
			//Is it ski season yet?
			Loaded += (s, e) =>
			{

				foreach (var item in FindVisualChildren<NumboardButton>(this))
				{
					var key = NumboardButton.GetHotkey(item);
					item.Hotkey = key;
					if (key != Keys.None)
					{
						Action<HotKey, NumboardButton> action = (k, sbb) =>
						{
							if (key == StopAllKey)
							{
								StopAllStreams();
							}
							if (!Keyboard.IsKeyDown(StopSingleKey))
							{
								Play(sbb, null);
							}
							else
							{
								StopStream(sbb);
							}
						};

						var hotkey = new HotKey(ModifierKeys.None, key, this, item);
						hotkey.HotKeyPressed += action;

						hotkey = new HotKey(ModifierKeys.Control, key, this, item);
						hotkey.HotKeyPressed += action;
					}
				}
			};
		}

		private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
		{
			if (depObj != null)
			{
				for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
				{
					var child = VisualTreeHelper.GetChild(depObj, i);
					if (child != null && child is T)
					{
						yield return (T)child;
					}

					foreach (var childOfChild in FindVisualChildren<T>(child))
					{
						yield return childOfChild;
					}
				}
			}
		}


		private void ClosingEventHandler(object sender, EventArgs e)
		{

			if (HaveChangesBeenMade)
			{
				if (MessageBox.Show("Save changes?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					Save(null, null);
				}
			}

			ProgramState.Instance.MasterVolume = MasterVolume;
			ProgramState.Instance.Save();

			StopAllStreams();

		}

		private void RememberPreviousSettings()
		{

			MasterVolume = ProgramState.Instance.MasterVolume;

			var primaryOut = ProgramState.Instance.PrimaryOutputDevice;

			if (primaryOut != null)
			{
				var deviceMenuItem = Helpers.GetLogicalChildCollection<ValueMenuItem>(PrimaryOutputDevice).
					Where(i => i.Value.Equals(primaryOut))
					.FirstOrDefault();

				if (deviceMenuItem != null)
				{
					deviceMenuItem.IsChecked = true;
				}
				else
				{
					MessageBox.Show("Could not find your old Primary Output Device");
				}
			}

			var secondaryOut = ProgramState.Instance.SecondaryOutputDevice;
			if (secondaryOut != null)
			{
				var deviceMenuItem = Helpers.GetLogicalChildCollection<ValueMenuItem>(SecondaryOutputDevice)
					.Where(i => i.Value.Equals(secondaryOut))
					.FirstOrDefault();

				if (deviceMenuItem != null)
				{
					deviceMenuItem.IsChecked = true;
				}
				else
				{
					MessageBox.Show("Could not find your old Secondary Output Device");
				}
			}


			var lastFile = ProgramState.Instance.DefaultSaveFile;
			if (lastFile != null)
			{
				if (!File.Exists(lastFile))
				{
					return;
				}

				var saveFile = new StreamReader(lastFile);
				var reader = new JsonTextReader(saveFile);
				var parsedData = new JsonSerializer().Deserialize(reader);
				var saveData = JsonConvert.DeserializeObject<List<SaveData>>((string)parsedData);
				ApplySaveData(saveData);
				SaveFilePath = lastFile;
			}
		}

		private void CheckInputDevices(object sender, MouseEventArgs e)
		{
			//-1 becuase of the 'None' item
			if (PrimaryOutputDevice.Items.Count - 1 == WaveOut.DeviceCount)
			{
				return;
			}

			var primary = SelectedPrimaryOutputDevice;
			var secondary = SelectedSecondaryOutputDevice;

			PrimaryOutputDevice.Items.Clear();
			SecondaryOutputDevice.Items.Clear();

			InitializeInputDevices();

			foreach (ValueMenuItem item in PrimaryOutputDevice.Items)
			{
				item.IsChecked = (int)item.Value == primary;
			}

			foreach (ValueMenuItem item in SecondaryOutputDevice.Items)
			{
				item.IsChecked = (int)item.Value == secondary;
			}

		}
		private void InitializeInputDevices()
		{
			var tempValueMenuItem = new ValueMenuItem();
			tempValueMenuItem.Header = "None";
			tempValueMenuItem.Value = -1;
			tempValueMenuItem.Click += SetDeviceId;
			tempValueMenuItem.IsCheckable = true;
			PrimaryOutputDevice.Items.Add(tempValueMenuItem);

			tempValueMenuItem = new ValueMenuItem();
			tempValueMenuItem.Header = "None";
			tempValueMenuItem.Value = -1;
			tempValueMenuItem.Click += SetDeviceId;
			tempValueMenuItem.IsCheckable = true;
			SecondaryOutputDevice.Items.Add(tempValueMenuItem);

			for (var deviceNumber = 0; deviceNumber < WaveOut.DeviceCount; deviceNumber++)
			{
				var capabilities = WaveOut.GetCapabilities(deviceNumber);

				tempValueMenuItem = new ValueMenuItem();
				tempValueMenuItem.Header = capabilities.ProductName;
				tempValueMenuItem.Value = deviceNumber;
				tempValueMenuItem.Click += SetDeviceId;
				tempValueMenuItem.IsCheckable = true;
				PrimaryOutputDevice.Items.Add(tempValueMenuItem);

				tempValueMenuItem = new ValueMenuItem();
				tempValueMenuItem.Header = capabilities.ProductName;
				tempValueMenuItem.Value = deviceNumber;
				tempValueMenuItem.Click += SetDeviceId;
				tempValueMenuItem.IsCheckable = true;
				SecondaryOutputDevice.Items.Add(tempValueMenuItem);
			}


		}

		private void SetDeviceId(object sender, RoutedEventArgs e)
		{
			var menuItem = (ValueMenuItem)sender;
			var value = (int)menuItem.Value;
			Helpers.GetLogicalChildCollection<ValueMenuItem>(menuItem.Parent).ForEach(i => i.IsChecked = false);
			menuItem.IsChecked = true;

			var selectedDevice = ((FrameworkElement)menuItem.Parent).Name;

			if (selectedDevice == PrimaryOutputDevice.Name)
			{
				ProgramState.Instance.PrimaryOutputDevice = (int?)menuItem.Value;
			}
			else
			{
				ProgramState.Instance.SecondaryOutputDevice = (int?)menuItem.Value;
			}

			ProgramState.Instance.Save();
		}

		private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (PlayingStreams != null)
			{
				foreach (var stream in PlayingStreams)
				{
					stream.PrimaryReader.Volume = (float)Math.Log(1 + MasterVolume * PrimaryVolume * stream.ButtonVolumeWhenClicked);
					stream.SecondaryReader.Volume = (float)Math.Log(1 + MasterVolume * SecondaryVolume * stream.ButtonVolumeWhenClicked);
				}
			}
		}

		private void FileList_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var listbox = (ListBox)sender;

			var item = ItemsControl.ContainerFromElement(listbox, e.OriginalSource as DependencyObject) as ListBoxItem;
			if (item != null)
			{
				var fileListItem = (FileListItem)item.Content;
				if (!File.Exists(fileListItem.FilePath))
				{
					RemoveFileFromFileList(fileListItem.FilePath);
					MessageBox.Show("Could not find that save");
					return;
				}
				LoadFromPath(fileListItem.FilePath);
			}
		}

		private void FileList_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				var files = (string[])e.Data.GetData(DataFormats.FileDrop);
				foreach (var file in files)
				{
					if (new FileInfo(file).Extension == ".nbs")
					{
						AddFileToFileList(file);
					}
				}
			}
		}
	}

}
