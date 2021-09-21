using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Essentials;
using Microsoft.Maui.Graphics;
using Plugin.AudioRecorder;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MauiAudioRecorder
{
    public partial class MainPage : ContentPage
    {
        private readonly AudioRecorderService audioRecorderService = new AudioRecorderService();

        private readonly AudioPlayer audioPlayer = new AudioPlayer();


        public MainPage()
        {
            InitializeComponent();
            RecordButton.BackgroundColor = Color.FromArgb("#561ECC");
            PlayButton.IsEnabled = false;
            PlayButton.BackgroundColor = Color.FromArgb("#C8C8C3");
            ShareButton.IsEnabled = false;
            ShareButton.BackgroundColor = Color.FromArgb("#C8C8C3");

        }

        private async void OnRecordClicked(object sender, EventArgs e)
        {
            await ValidatePermissionsAsync().ConfigureAwait(false);

            if (audioRecorderService.IsRecording)
            {
                RecordButton.Text = "Start Recording";
                await audioRecorderService.StopRecording();
                Progress.Text = "Recording is available";
                if (!string.IsNullOrEmpty(audioRecorderService.GetAudioFilePath()))
                {
                    PlayButton.IsEnabled = true;
                    PlayButton.BackgroundColor = Color.FromArgb("#561ECC");
                    ShareButton.IsEnabled = true;
                    ShareButton.BackgroundColor = Color.FromArgb("#561ECC");
                }
                //audioPlayer.Play(audioRecorderService.GetAudioFilePath());
            }
            else
            {
                audioRecorderService.FilePath = Path.Combine(FileSystem.AppDataDirectory, $"{Guid.NewGuid().ToString().Substring(0, 5)}.wav");
                await audioRecorderService.StartRecording();
                RecordButton.Text = "Stop Recording";
                Progress.Text = "Waiting for recording";
                PlayButton.IsEnabled = false;
                PlayButton.BackgroundColor = Color.FromArgb("#C8C8C3");
                ShareButton.IsEnabled = false;
                ShareButton.BackgroundColor = Color.FromArgb("#C8C8C3");
            }

        }

        private async void OnShareButtonClicked(object sender, EventArgs e)
        {
            var sourcePath = audioRecorderService.GetAudioFilePath();

            if (string.IsNullOrEmpty(sourcePath))
                return;
            
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Sharing Recorded Audio File",
                File = new ShareFile(sourcePath)
            });
        }

        private async void OnPlayButtonClicked(object sender, EventArgs e)
        {
            var sourcePath = audioRecorderService.GetAudioFilePath();

            if (string.IsNullOrEmpty(sourcePath))
                return;

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(sourcePath)
            });

        }



        private async Task ValidatePermissionsAsync()
        {
            var status = await Permissions.RequestAsync<Permissions.Microphone>();
            
            if (status != PermissionStatus.Granted)
            {
                Progress.Text = "Microphone permission not granted";
            }

            status = await Permissions.RequestAsync<Permissions.StorageRead>();

            if (status != PermissionStatus.Granted)
            {
                Progress.Text = "StorageRead permission not granted";
            }

            status = await Permissions.RequestAsync<Permissions.StorageWrite>();

            if (status != PermissionStatus.Granted)
            {
                Progress.Text = "StorageWrite permission not granted";
            }

            status = await Permissions.RequestAsync<Permissions.Speech>();

            if (status != PermissionStatus.Granted)
            {
                Progress.Text = "Speech permission not granted";
            }

        }
    }
}
