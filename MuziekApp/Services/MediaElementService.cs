using MuziekApp.ViewModels;
using Plugin.Maui.Audio;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
#if WINDOWS
using Windows.Media.Core;
using Windows.Media.Playback;
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
        private Stream? _currentStream;   // Houd de HTTP-stream vast

#if WINDOWS
        private static MediaPlayer? _mediaPlayer;
#endif

        private List<PlaylistSong> _playlist = new();
        private int _currentIndex = -1;

        public string CurrentTitle { get; private set; } = "";
        public string CurrentArtist { get; private set; } = "";

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

                // Kort wachten zodat native audio-bronnen worden vrijgegeven
                await Task.Delay(100);

                CurrentTitle = title;
                CurrentArtist = artist;
                Debug.WriteLine($"[PLAYER] Playing: {title} - {artist}");
                OnSongChanged?.Invoke(title, artist);

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

                    // Open een HTTP-stream (streaming, niet volledige buffer)
                    Stream netStream;
                    try
                    {
                        netStream = await httpClient.GetStreamAsync(url);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[PLAYER] Kan stream niet openen: {ex.Message}");
                        return;
                    }

                    // Vervang oude stream
                    _currentStream?.Dispose();
                    _currentStream = netStream;

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

            _currentIndex = (_currentIndex + 1) % _playlist.Count;
            var song = _playlist[_currentIndex];
            await PlayAsync(song.AudioUrl, song.Title, song.Artist);
        }

        public async Task PlayPrevious()
        {
            if (_playlist.Count == 0) return;

            _currentIndex = (_currentIndex - 1 + _playlist.Count) % _playlist.Count;
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
                    if (_player.IsPlaying)
                        _player.Pause();
                    else
                        _player.Play();
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
                _currentStream?.Dispose();
                _currentStream = null;
            }
        }

        public TimeSpan Duration =>
#if WINDOWS
            _mediaPlayer?.NaturalDuration ?? TimeSpan.Zero;
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
