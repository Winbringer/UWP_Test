using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;


namespace VictoremMediaApp
{

    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<StorageFile> files = null;
        private ushort currentFile = 0;
        private DisplayRequest appDisplayRequest = null;
        private IRandomAccessStream stream = null;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            this.media.AudioCategory = AudioCategory.GameMedia;
            media.MediaEnded += On_MediaEnded;
        }

        private async void On_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (files != null && files.Count() > 0)
            {
                currentFile++;
                await PlayCurrent();
            }
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                BackgroundMediaPlayer.SendMessageToBackground(new ValueSet());
            });
        }

        private async void openButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            folderPicker.FileTypeFilter.Add("*");
            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                var currentMediaFile = await GetFiles(folder);

                if (null != currentMediaFile && currentMediaFile.Count() > 0)
                {
                    files = currentMediaFile.Where(x => !string.IsNullOrWhiteSpace(x.ContentType) && media.CanPlayType(x.ContentType) != MediaCanPlayResponse.NotSupported).ToList();
                    currentFile = 0;
                    await PlayCurrent();
                }
            }
        }

        async Task<IEnumerable<StorageFile>> GetFiles(StorageFolder folder)
        {
            var folders = await folder.GetFoldersAsync();
            var files = await folder.GetFilesAsync();
            foreach (var f in folders)
            {
                var fl = await GetFiles(f);
                if (fl != null && fl.Count() > 0)
                    files = files.Concat(fl).ToList();
            }

            return files.ToArray();
        }

        private async void nextButton_Click(object sender, RoutedEventArgs e)
        {

            currentFile++;
            await PlayCurrent();

        }

        private async void prevButton_Click(object sender, RoutedEventArgs e)
        {

            if (currentFile > 0) currentFile--;
            await PlayCurrent();

        }

        private async void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Windows.UI.Popups.MessageDialog("Удалить проигрываемый файл с диска?");
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Да"));
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Нет"));

            dialog.DefaultCommandIndex = 1;
            var result = await dialog.ShowAsync();
            if (result.Label == "Да")
            {

                if (files != null && files.Count() > 0)
                {

                    await files[currentFile].DeleteAsync();
                    files.RemoveAt(currentFile);
                    await PlayCurrent();
                }
                var d = new Windows.UI.Popups.MessageDialog("Файл удален.");
                await d.ShowAsync();
            }
        }

        private async Task PlayCurrent()
        {
            if (files != null && files.Count() > 0)
            {
                if (currentFile >= files.Count()) currentFile = 0;
                if (files[currentFile] != null)
                {
                    try
                    {
                        stream?.Dispose();
                        stream = await files[currentFile].OpenAsync(Windows.Storage.FileAccessMode.Read);
                        var f = await files[currentFile].Properties.GetMusicPropertiesAsync();
                        nameM.Text = $"Title - {f?.Title} : {f?.Subtitle}";
                        autorM.Text = "Artist - " + f?.Artist + " : " + (f != null && f.Writers != null && f.Writers.Count > 0 ? f.Writers[0] : "") + " Файлов в плейлисте: " + files.Count;
                        albumM.Text = "Album - " + f?.Album;
                        nuberM.Text = currentFile.ToString();
                        media.SetSource(stream, files[currentFile].ContentType);
                        media.Play();
                    }
                    catch (Exception e)
                    {

                        await new Windows.UI.Popups.MessageDialog(e.Message).ShowAsync();
                    }
                }
                else
                {
                    currentFile++;
                }
            }
        }

        private void media_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            MediaElement mediaElement = sender as MediaElement;
            if (mediaElement != null && mediaElement.IsAudioOnly == false)
            {
                if (mediaElement.CurrentState == Windows.UI.Xaml.Media.MediaElementState.Playing)
                {
                    if (appDisplayRequest == null)
                    {
                        // This call creates an instance of the DisplayRequest object. 
                        appDisplayRequest = new DisplayRequest();
                        appDisplayRequest.RequestActive();
                    }
                }
                else // CurrentState is Buffering, Closed, Opening, Paused, or Stopped. 
                {
                    if (appDisplayRequest != null)
                    {
                        // Deactivate the display request and set the var to null.
                        appDisplayRequest.RequestRelease();
                        appDisplayRequest = null;
                    }
                }
            }
        }
    }
}
