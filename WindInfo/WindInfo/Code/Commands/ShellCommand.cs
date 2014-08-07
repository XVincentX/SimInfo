using Coding4Fun.Toolkit.Controls;
using Microsoft.Phone.Shell;
using StringResources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Tiles;

namespace WindInfo.Code
{
    public class ShellCommand : ICommand
    {
        public ShellCommand()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(null, EventArgs.Empty);
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var filename = parameter.ToString();
            var number = Path.GetFileNameWithoutExtension(filename).Split('_')[2];

            if (ShellTile.ActiveTiles.Any(st => st.NavigationUri.ToString().Contains(number)))
            {
                new MessagePrompt() { Title = "SimInfo", Message = AppResources.TileAlreadyPinned }.Show();
                return;
            }

            var baseDir = Path.GetDirectoryName(filename);

            var smallpath = Path.Combine(baseDir, string.Format("{0}_{1}_{2}.jpg", 159, 159, number));
            var normalpath = Path.Combine(baseDir, string.Format("{0}_{1}_{2}.jpg", 336, 336, number));
            //var widepath = Path.Combine(baseDir, string.Format("{0}_{1}_{2}.jpg", 691, 336, number));

            StandardTileData tileData = new StandardTileData
            {
                Title = " ",
                //  WideBackgroundImage = new Uri("isostore:" + widepath, UriKind.Absolute),
                BackgroundImage = new Uri("isostore:" + normalpath, UriKind.Absolute),
                //SmallBackgroundImage = new Uri("isostore:" + smallpath, UriKind.Absolute),
                BackBackgroundImage = new Uri("isostore:" + normalpath, UriKind.Absolute),
                // WideBackBackgroundImage = new Uri("isostore:" + widepath, UriKind.Absolute),
            };


            string tileUri = string.Concat("/DataPage.xaml?number=", number);
            ShellTile.Create(new Uri(tileUri, UriKind.Relative), tileData, false);

        }
    }

    public class CanExecuteChangedEventArgs : EventArgs
    {
        public bool NewState { get; set; }
    }
}
