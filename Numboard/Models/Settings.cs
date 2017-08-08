using System.Linq;
using System.Windows.Input;

namespace Numboard
{
    public partial class MainWindow
    {
        public int SelectedPrimaryOutputDevice
        {
            get
            {
                var menuItems = Helpers.GetLogicalChildCollection<ValueMenuItem>(PrimaryOutputDevice);

                var valueMenuItem = menuItems.FirstOrDefault(i => i.IsChecked);

                //if there is none checked, make the first one chekced
                if (valueMenuItem != null)
                {
                    return (int) valueMenuItem.Value;
                }

                valueMenuItem = menuItems.FirstOrDefault(i => (int) i.Value != -1);

                //if that fails, then there are no audio devices, we might want to throw an exception
                if (valueMenuItem != null)
                {
                    valueMenuItem.IsChecked = true;
                }
                else
                {
                    return -1;
                }

                return (int) valueMenuItem.Value;
            }
        }

        public int SelectedSecondaryOutputDevice
        {
            get
            {
                Helpers.GetLogicalChildCollection<ValueMenuItem>(SecondaryOutputDevice);
                var valueMenuItem = Helpers
                    .GetLogicalChildCollection<ValueMenuItem>(SecondaryOutputDevice)
                    .Where(i => (int) i.Value != -1)
                    .FirstOrDefault(i => i.IsChecked);

                //if there is none checked, make the first one checked
                if (valueMenuItem != null)
                {
                    return (int) valueMenuItem.Value;
                }

                valueMenuItem = Helpers.GetLogicalChildCollection<ValueMenuItem>(SecondaryOutputDevice).FirstOrDefault();

                //if that fails, then there are no audio devices, we might want to throw an exception
                if (valueMenuItem != null)
                {
                    valueMenuItem.IsChecked = true;
                }
                else
                {
                    return -1;
                }

                return (int) valueMenuItem.Value;
            }
        }

        public double MasterVolume
        {
            get => MasterVolumeSlider.Value;
            set => MasterVolumeSlider.Value = value;
        }

        public double PrimaryVolume
        {
            get => PrimaryDeviceVolumeSlider.Value;
            set => PrimaryDeviceVolumeSlider.Value = value;
        }

        public double SecondaryVolume
        {
            get => SecondaryDeviceVolumeSlider.Value;
            set => SecondaryDeviceVolumeSlider.Value = value;
        }

        public bool HaveChangesBeenMade { get; set; }

        public string SaveFilePath { get; set; }

        public Keys StopAllKey = Keys.Decimal;

        public Key StopSingleKey = Key.NumPad0;
    }
}
