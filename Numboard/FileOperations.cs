using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Numboard
{
	public partial class MainWindow
	{

		private void New(object sender, RoutedEventArgs e)
		{
			var saveData = JsonConvert.DeserializeObject<List<SaveData>>((string)GetNewDataJson());
			ApplySaveData(saveData);
			SaveFilePath = null;
			HaveChangesBeenMade = true;
		}

		private void Load(object sender, RoutedEventArgs e)
		{
			if (HaveChangesBeenMade)
			{
				if (MessageBox.Show("Save changes?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					Save(this, e);
				}
			}

			var dialog = new OpenFileDialog();
			dialog.Filter = "Numboard Save File (*.nbs)|*.nbs";
			dialog.RestoreDirectory = true;

			if (dialog.ShowDialog() == true)
			{
				StreamReader saveFile = new StreamReader(dialog.FileName);
				JsonTextReader reader = new JsonTextReader(saveFile);
				object parsedData = new JsonSerializer().Deserialize(reader);
				var saveData = JsonConvert.DeserializeObject<List<SaveData>>((string)parsedData);
				ApplySaveData(saveData);
				HaveChangesBeenMade = false;
				ProgramState.Instance.DefaultSaveFile = dialog.FileName;
				ProgramState.Instance.Save();

				AddFileToFileList(dialog.FileName);
			}

		}

		private void LoadFromPath(string path)
		{
			if (!File.Exists(path)) return;
			if (HaveChangesBeenMade)
			{
				if (MessageBox.Show("Save changes?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					Save(this, null);
				}
			}

			StreamReader saveFile = new StreamReader(path);
			JsonTextReader reader = new JsonTextReader(saveFile);
			object parsedData = new JsonSerializer().Deserialize(reader);
			var saveData = JsonConvert.DeserializeObject<List<SaveData>>((string)parsedData);
			ApplySaveData(saveData);
			HaveChangesBeenMade = false;

			ProgramState.Instance.DefaultSaveFile = path;
            ProgramState.Instance.Save();

			AddFileToFileList(path);
		}

		private void Save(object sender, RoutedEventArgs e)
		{
			if (SaveFilePath != null)
			{
				string json = JsonConvert.SerializeObject(GetSaveDataJson());
				File.WriteAllText(SaveFilePath, json);
				HaveChangesBeenMade = false;
			}
			else
			{
				SaveAs(sender, e);
			}
		}

		private void SaveAs(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog();
			dialog.FileName = "save.nbs";
			dialog.DefaultExt = ".nbs";
			dialog.Filter = "Numboard Save File (*.nbs)|*.nbs";
			if (dialog.ShowDialog() == true)
			{
				string json = JsonConvert.SerializeObject(GetSaveDataJson());
				File.WriteAllText(dialog.FileName, json);
				HaveChangesBeenMade = false;

				ProgramState.Instance.DefaultSaveFile = dialog.FileName;
				ProgramState.Instance.Save();

				AddFileToFileList(dialog.FileName);
				SaveFilePath = dialog.FileName;
			}
		}

		private void ApplySaveData(List<SaveData> saveData)
		{
			foreach (var item in saveData)
			{
				var button = (NumboardButton)this.FindName(item.Name);
				button.SetSource(item.Source);
				button.Volume = item.Volume;

				//sets the checked property on context menu
				if (button.Volume != null)
				{
					if (button.ContextMenu != null)
					{
						Helpers.GetLogicalChildCollection<MenuItem>(button.ContextMenu).ForEach(mi => mi.IsChecked = (Int32.Parse(Regex.Match((string)mi.Header, @"\d+").Value)) == (int)(button.Volume * 100));
					}
				}
			}
		}

		private string GetSaveDataJson()
		{
			var buttons = Helpers.GetLogicalChildCollection<NumboardButton>(this);
			var saveData = buttons.Select(b => new SaveData
			{
				Name = b.Name,
				Source = b.Source,
				Hotkey = NumboardButton.GetHotkey(b).ToString(),
				Volume = b.Volume
			});
			return JsonConvert.SerializeObject(saveData.ToArray());
		}

		private string GetNewDataJson()
		{
			var buttons = Helpers.GetLogicalChildCollection<NumboardButton>(this);
			var saveData = buttons.Select(b => new SaveData
			{
				Name = b.Name,
				Source = null,
				Hotkey = NumboardButton.GetHotkey(b).ToString(),
				Volume = 1
			});
			return JsonConvert.SerializeObject(saveData.ToArray());
		}

		private void PopulateFileList()
		{
            ProgramState.Instance.SaveFiles.ForEach(f =>
			{
				if (!File.Exists(f))
				{
					RemoveFileFromFileList(f);
				}
				else
				{
					FileList.Items.Add(new FileListItem { FileName = Path.GetFileNameWithoutExtension(new FileInfo(f).Name), FilePath = f });
				}
			});
		}

		private void AddFileToFileList(string fileName)
		{
			if (!File.Exists(fileName))
			{
				return;
			}

			//dont add dupes
			if (ProgramState.Instance.SaveFiles.FirstOrDefault(f => f == fileName) != null)
			{
				return;
			}

			ProgramState.Instance.SaveFiles.Add(fileName);
			ProgramState.Instance.Save();

			FileList.Items.Add(new FileListItem { FileName = Path.GetFileNameWithoutExtension(new FileInfo(fileName).Name), FilePath = fileName });
		}

		private void RemoveFileFromFileList(string fileName)
		{
			ProgramState.Instance.SaveFiles.Remove(fileName);
			ProgramState.Instance.Save();

			for (var i = 0; i < FileList.Items.Count; i++)
			{
				if ((FileList.Items[i] as FileListItem).FilePath == fileName)
				{
					FileList.Items.RemoveAt(i);
					break;
				}
			}
		}
	}
}
