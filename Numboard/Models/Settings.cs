using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Numboard
{
	public partial class MainWindow
	{
		public int SelectedPrimaryOutputDevice
		{
			get
			{
				var valueMenuItem = Helpers.GetLogicalChildCollection<ValueMenuItem>(PrimaryOutputDevice)
					.Where(i => i.IsChecked == true)
					.FirstOrDefault();

				//if there is none checked, make the first one checked
				if (valueMenuItem == null)
				{
					valueMenuItem = Helpers.GetLogicalChildCollection<ValueMenuItem>(PrimaryOutputDevice).FirstOrDefault();

					//if that fails, then there are no audio devices, we might want to throw an exception
					if (valueMenuItem != null)
					{
						valueMenuItem.IsChecked = true;
					}
					else
					{
						//exception instead?
						return 0;
					}
				}

				return (int)valueMenuItem.Value;
			}
		}

		public int SelectedSecondaryOutputDevice
		{
			get
			{
				var valueMenuItem = Helpers.GetLogicalChildCollection<ValueMenuItem>(SecondaryOutputDevice)
					.Where(i => i.IsChecked == true)
					.FirstOrDefault();

				//if there is none checked, make the first one checked
				if (valueMenuItem == null)
				{
					valueMenuItem = Helpers.GetLogicalChildCollection<ValueMenuItem>(SecondaryOutputDevice).FirstOrDefault();

					//if that fails, then there are no audio devices, we might want to throw an exception
					if (valueMenuItem != null)
					{
						valueMenuItem.IsChecked = true;
					}
					else
					{
						//exception instead?
						return 0;
					}
				}

				return (int)valueMenuItem.Value;
            }
		}

		public double MasterVolume
		{
			get { return MasterVolumeSlider.Value; }
			set
			{
				MasterVolumeSlider.Value = value;
			}
		}

		public double PrimaryVolume
		{
			get { return PrimaryDeviceVolumeSlider.Value; }
			set
			{
				PrimaryDeviceVolumeSlider.Value = value;
			}
		}

		public double SecondaryVolume
		{
			get { return SecondaryDeviceVolumeSlider.Value; }
			set
			{
				SecondaryDeviceVolumeSlider.Value = value;
			}
		}

		public bool HaveChangesBeenMade { get; set; }

		public string SaveFilePath { get; set; }

		public Keys StopAllKey = Keys.Decimal;

		public Key StopSingleKey = Key.NumPad0;
	}
}
