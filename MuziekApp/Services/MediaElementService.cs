using MuziekApp.ViewModels;
using Plugin.Maui.Audio;
using System.Diagnostics;
using System.Net.Http;
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
        private readonly object _lock = new();
        private MemoryStream? _currentStream;

#if WINDOWS
        private static MediaPlayer? _mediaPlayer;
#endif

        private List<PlaylistSong> _playlist = new();
        private int _currentIndex = -1;

        public string CurrentTitle { get; private set; } = "";
        public string CurrentArtist { get; private set; } = "";

        // Events voor ViewModel
        public event Action<string, string>? OnSongChanged;
        public event Func<Task>? OnSongEnded;

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
            _mediaPlayer.MediaEnded += async (s, e) =>
            {
                if (OnSongEnded != null)
                    await OnSongEnded.Invoke();
            };
#endif
        }

        public void SetPlaylist(IEnumerable<PlaylistSong> songs)
        {
            _playlist = songs.ToList();
            _currentIndex = -1;
        }

        public async Task PlayAsync(string url, string title = "", string artist = "")
        {
            try
            {
                lock (_lock)
                {
                    Stop_NoLock();
                }

                // geef Android native tijd om resources vrij te geven
                await Task.Delay(100);

                CurrentTitle = title;
                CurrentArtist = artist;
                Debug.WriteLine($"[PLAYER] Playing: {title} - {artist}");
                OnSongChanged?.Invoke(CurrentTitle, CurrentArtist);

                if (DeviceInfo.Platform == DevicePlatform.WinUI)
                {
#if WINDOWS
                    _mediaPlayer.Source = MediaSource.CreateFromUri(new Uri(url));
                    _mediaPlayer.Volume = 1.0;
                    _mediaPlayer.Play();
#endif
                }
                else
                {
                    using var httpClient = new HttpClient();

                    // Controleer status vóór downloaden
                    HttpResponseMessage response;
                    try
                    {
                        response = await httpClient.GetAsync(url);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[PLAYER] HTTP request failed: {ex}");
                        return;
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine($"[PLAYER] HTTP {(int)response.StatusCode} {response.ReasonPhrase} for URL: {url}");
                        return;
                    }

                    var data = await response.Content.ReadAsByteArrayAsync();

                    _currentStream?.Dispose();
                    _currentStream = new MemoryStream(data);

                    lock (_lock)
                    {
                        _player = _audioManager.CreatePlayer(_currentStream);
                        _player.PlaybackEnded += async (s, e) =>
                        {
                            Debug.WriteLine("[PLAYER] Playback ended");
                            if (OnSongEnded != null)
                                await MainThread.InvokeOnMainThreadAsync(OnSongEnded);
                        };
                        _player.Play();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PLAYER] Fout bij PlayAsync: {ex}");
            }
        }

        public async Task PlayNext()
        {
            if (_playlist.Count == 0) return;

            _currentIndex++;
            if (_currentIndex >= _playlist.Count) _currentIndex = 0;

            var song = _playlist[_currentIndex];
            await PlayAsync(song.AudioUrl, song.Title, song.Artist);
        }

        public async Task PlayPrevious()
        {
            if (_playlist.Count == 0) return;

            _currentIndex--;
            if (_currentIndex < 0) _currentIndex = _playlist.Count - 1;

            var song = _playlist[_currentIndex];
            await PlayAsync(song.AudioUrl, song.Title, song.Artist);
        }

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
                lock (_lock)
                {
                    if (_player == null) return;
                    if (_player.IsPlaying) _player.Pause();
                    else _player.Play();
                }
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                Stop_NoLock();
            }
        }

        private void Stop_NoLock()
        {
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
#if WINDOWS
                _mediaPlayer?.Pause();
#endif
            }
            else
            {
                if (_player != null)
                {
                    Debug.WriteLine("[PLAYER] Stopping current player...");
                    _player.Stop();
                    _player.Dispose();
                    _player = null;
                }
            }
        }

        public TimeSpan Duration =>
#if WINDOWS
            _mediaPlayer != null ? _mediaPlayer.NaturalDuration : TimeSpan.Zero;
#else
            _player != null ? TimeSpan.FromSeconds(_player.Duration) : TimeSpan.Zero;
#endif

        public TimeSpan Position =>
#if WINDOWS
            _mediaPlayer?.Position ?? TimeSpan.Zero;
#else
            _player != null ? TimeSpan.FromSeconds(_player.CurrentPosition) : TimeSpan.Zero;
#endif

        public void Seek(TimeSpan position)
        {
#if WINDOWS
            if (_mediaPlayer != null)
                _mediaPlayer.Position = position;
#else
            lock (_lock)
            {
                _player?.Seek(position.TotalSeconds);
            }
#endif
        }
    }

    public record PlaylistSong(string Title, string Artist, string AudioUrl);
}
