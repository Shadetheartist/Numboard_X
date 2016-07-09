﻿using System;
using System.Collections.Generic;
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
		private void SetVolume(object sender, RoutedEventArgs e)
		{
			var menuItem = ((FrameworkElement)sender as MenuItem);
			var value = Int32.Parse(Regex.Match((string)menuItem.Header, @"\d+").Value);

			ContextMenu cm = menuItem.CommandParameter as ContextMenu;
			if (cm != null)
			{
				NumboardButton button = cm.PlacementTarget as NumboardButton;
				button.Volume = ((double)value) / 100;
				HaveChangesBeenMade = true;

				Helpers.GetLogicalChildCollection<MenuItem>(menuItem.Parent).ForEach(mi => mi.IsChecked = false);
				menuItem.IsChecked = true;
			}

		}

		private void SetButtonSource(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

			if (files.Count() != 1) return;

			var file = files[0];

			if (!Helpers.isValidFormat(file))
			{
                System.Windows.MessageBox.Show("File format (" + System.IO.Path.GetExtension(file)  + ") is invalid. This application supports " + String.Join(", ", Helpers.validFormats.ToArray()));
                return;
			}
			var button = (NumboardButton)sender;

			button.SetSource(file);

			HaveChangesBeenMade = true;
		}

	}


	public class NumboardButton : Button
	{
		public string DefaultValue { get; set; }
		public static readonly DependencyProperty DefaultValueProperty =
		  DependencyProperty.Register("DefaultValue", typeof(string), typeof(NumboardButton), new FrameworkPropertyMetadata(null));

		public string Source { get; set; }
		public static readonly DependencyProperty SourceProperty =
		  DependencyProperty.Register("Source", typeof(string), typeof(NumboardButton));

		public double? Volume { get; set; }
		public static readonly DependencyProperty VolumeProperty =
		  DependencyProperty.Register("Volume", typeof(double), typeof(NumboardButton));

		public Keys Hotkey { get; set; }
		public static readonly DependencyProperty HotkeyProperty =
		  DependencyProperty.Register("Hotkey", typeof(Keys), typeof(NumboardButton), new FrameworkPropertyMetadata(null));

		public void SetSource(string source)
		{
			this.Source = source;

			if (source == null)
			{
				this.Content = new TextBlock
				{
					Text = GetDefaultValue(this),
					TextWrapping = TextWrapping.Wrap
				};
				return;
			};

			var friendlyName = System.IO.Path.GetFileNameWithoutExtension(source);
			this.Content = new TextBlock
			{
				Text = friendlyName,
				TextWrapping = TextWrapping.Wrap
			};
		}

		public static string GetDefaultValue(UIElement element)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			return (string)element.GetValue(DefaultValueProperty);
		}
		public static void SetDefaultValue(UIElement element, string value)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			element.SetValue(DefaultValueProperty, value);
		}

		public static Keys GetHotkey(UIElement element)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			return (Keys)element.GetValue(HotkeyProperty);
		}

		public static void SetHotkey(UIElement element, string value)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			element.SetValue(HotkeyProperty, value);
		}
	}
}
