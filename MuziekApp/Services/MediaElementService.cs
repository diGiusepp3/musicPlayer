using Plugin.Maui.Audio;
#if WINDOWS
using Windows.Media.Playback;
using Windows.Media.Core;
#endif

namespace MuziekApp.Services
{
    public class MediaElementService
    {
        private static MediaElementService? _current;
        public static MediaElementService Current => _current ??= new MediaElementService();

        private readonly IAudioManager _audioManager;
        private IAudioPlayer? _player;

#if WINDOWS
        private static MediaPlayer? _mediaPlayer;
#endif

        public string CurrentTitle { get; private set; } = "";
        public string CurrentArtist { get; private set; } = "";

        public bool IsPlaying =>
#if WINDOWS
            _mediaPlayer?.CurrentState == MediaPlayerState.Playing;
#else
            _player?.IsPlaying ?? false;
#endif

        private MediaElementService()
        {
            _audioManager = AudioManager.Current;
#if WINDOWS
            _mediaPlayer ??= new MediaPlayer();
#endif
        }

        /// <summary>
        /// Speelt een nummer af vanaf een URL en slaat titel/artist op.
        /// </summary>
        public async Task PlayAsync(string url, string title = "", string artist = "")
        {
            Stop();
            CurrentTitle = title;
            CurrentArtist = artist;

            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
#if WINDOWS
                try
                {
                    // Download naar cache
                    var fileName = Path.GetFileName(url);
                    var localPath = Path.Combine(FileSystem.CacheDirectory, fileName);

                    using var http = new HttpClient();
                    var data = await http.GetByteArrayAsync(url);
                    await File.WriteAllBytesAsync(localPath, data);

                    // Windows MediaPlayer
                    _mediaPlayer.Source = MediaSource.CreateFromUri(new Uri(localPath));
                    _mediaPlayer.Volume = 1.0;
                    _mediaPlayer.Play();

                    Console.WriteLine($"[MiniPlayer] Playing local file: {localPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[MiniPlayer] Windows playback error: " + ex.Message);
                }
#endif
            }
            else
            {
                using var httpClient = new HttpClient();
                var data = await httpClient.GetByteArrayAsync(url);
                var stream = new MemoryStream(data);
                _player = _audioManager.CreatePlayer(stream);
                _player.Play();
            }
        }

        /// <summary>
        /// Speelt af of pauzeert afhankelijk van huidige status.
        /// </summary>
        public void TogglePlayPause()
        {
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
#if WINDOWS
                if (_mediaPlayer == null) return;
                if (_mediaPlayer.CurrentState == MediaPlayerState.Playing)
                    _mediaPlayer.Pause();
                else
                    _mediaPlayer.Play();
#endif
            }
            else
            {
                if (_player == null) return;
                if (_player.IsPlaying) _player.Pause();
                else _player.Play();
            }
        }

        /// <summary>
        /// Stop het huidige nummer.
        /// </summary>
        public void Stop()
        {
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
#if WINDOWS
                _mediaPlayer?.Pause();
#endif
            }
            else
            {
                _player?.Stop();
                _player = null;
            }
        }

        /// <summary>
        /// Totale lengte van het huidige nummer.
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
#if WINDOWS
                return _mediaPlayer != null ? _mediaPlayer.NaturalDuration : TimeSpan.Zero;

#else
                return _player?.Duration ?? TimeSpan.Zero;
#endif
            }
        }

        /// <summary>
        /// Huidige positie van het nummer.
        /// </summary>
        public TimeSpan Position
        {
            get
            {
#if WINDOWS
                return _mediaPlayer?.Position ?? TimeSpan.Zero;
#else
                return _player?.CurrentPosition ?? TimeSpan.Zero;
#endif
            }
        }

        /// <summary>
        /// Spring naar een specifieke tijd in het nummer.
        /// </summary>
        /// <param name="position">Nieuwe positie in seconden.</param>
        public void Seek(TimeSpan position)
        {
#if WINDOWS
            if (_mediaPlayer != null)
                _mediaPlayer.Position = position;
#else
            if (_player != null)
                _player.Seek(position);
#endif
        }
    }
}
