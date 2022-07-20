using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Logging;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards;
using Melanchall.DryWetMidi.Tools;
using MidiBard.Control.CharacterControl;
using MidiBard.Control.MidiControl.PlaybackInstance;
using MidiBard.DalamudApi;
using MidiBard.IPC;
using MidiBard.Managers.Ipc;
using MidiBard.Util;
using static MidiBard.MidiBard;

namespace MidiBard.Control.MidiControl;

public static class FilePlayback
{
    private static readonly Regex regex = new Regex(@"^#.*?([-|+][0-9]+).*?#", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static BardPlayback GetPlaybackObject(MidiFile midifile, string path)
    {
        PluginLog.Debug($"[LoadPlayback] -> {path} START");
        var stopwatch = Stopwatch.StartNew();
        var playback = BardPlayback.GetBardPlayback(midifile, path);
        playback.InterruptNotesOnStop = true;
        playback.TrackNotes = true;
        playback.TrackProgram = true;
        playback.Speed = config.playSpeed;
        playback.Finished += Playback_Finished;
        PluginLog.Debug($"[LoadPlayback] -> {path} OK! in {stopwatch.Elapsed.TotalMilliseconds} ms");
        return playback;
    }






    public static DateTime? waitUntil { get; set; } = null;
    public static DateTime? waitStart { get; set; } = null;
    public static bool isWaiting => waitUntil != null && DateTime.Now < waitUntil;

    public static float waitProgress
    {
        get
        {
            float valueTotalMilliseconds = 1;
            if (isWaiting)
            {
                try
                {
                    if (waitUntil != null)
                        if (waitStart != null)
                            valueTotalMilliseconds = 1 -
                                                     (float)((waitUntil - DateTime.Now).Value.TotalMilliseconds /
                                                             (waitUntil - waitStart).Value.TotalMilliseconds);
                }
                catch (Exception e)
                {
                    PluginLog.Error(e, "error when get current wait progress");
                }
            }

            return valueTotalMilliseconds;
        }
    }

    private static void Playback_Finished(object sender, EventArgs e)
    {
        Task.Run(async () =>
        {
            try
            {
                if (MidiBard.AgentMetronome.EnsembleModeRunning)
                    return;
                if (!PlaylistManager.FilePathList.Any())
                    return;
                if (MidiBard.SlaveMode)
                    return;

                PerformWaiting(MidiBard.config.secondsBetweenTracks);
                if (needToCancel)
                {
                    needToCancel = false;
                    return;
                }

                switch ((PlayMode)MidiBard.config.PlayMode)
                {
                    case PlayMode.Single:
                        break;

                    case PlayMode.SingleRepeat:
                        CurrentPlayback.MoveToStart();
                        MidiPlayerControl.DoPlay();
                        break;

                    case PlayMode.ListOrdered:
                        if (PlaylistManager.CurrentPlaying + 1 < PlaylistManager.FilePathList.Count)
                        {
                            if (await LoadPlayback(PlaylistManager.CurrentPlaying + 1, true))
                            {
                            }
                        }

                        break;

                    case PlayMode.ListRepeat:
                        if (PlaylistManager.CurrentPlaying + 1 < PlaylistManager.FilePathList.Count)
                        {
                            if (await LoadPlayback(PlaylistManager.CurrentPlaying + 1, true))
                            {
                            }
                        }
                        else
                        {
                            if (await LoadPlayback(0, true))
                            {
                            }
                        }

                        break;

                    case PlayMode.Random:

                        if (PlaylistManager.FilePathList.Count == 1)
                        {
                            CurrentPlayback.MoveToStart();
                            break;
                        }

                        try
                        {
                            var r = new Random();
                            int nexttrack;
                            do
                            {
                                nexttrack = r.Next(0, PlaylistManager.FilePathList.Count);
                            } while (nexttrack == PlaylistManager.CurrentPlaying);

                            if (await LoadPlayback(nexttrack, true))
                            {
                            }
                        }
                        catch (Exception exception)
                        {
                            PluginLog.Error(exception, "error when random next");
                        }

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception exception)
            {
                PluginLog.Error(exception, "Unexpected exception when Playback finished.");
            }
        });
    }

    //internal static async Task<bool> LoadPlayback(string path, bool startPlaying = false, bool switchInstrument = true)
    //{
    //    MidiFile midiFile = await PlaylistManager.LoadMidiFile(path);
    //    if (midiFile == null)
    //    {
    //        ImGuiUtil.AddNotification(NotificationType.Error, "Error when reading Midi file");
    //        return false;
    //    }
    //    else
    //    {
    //        CurrentPlayback = await Task.Run(() =>
    //        {
    //            CurrentPlayback?.Dispose();
    //            CurrentPlayback = null;

    //            return GetPlaybackObject(midiFile, Path.GetFileNameWithoutExtension(path));
    //        });
    //        Ui.RefreshPlotData();
    //        PlaylistManager.CurrentPlaying = -1;
    //        BardPlayDevice.Instance.ResetChannelStates();
    //        return true;
    //    }
    //}


    internal static async Task<bool> LoadPlayback(int index, bool startPlaying = false, bool switchInstrument = true)
    {
        if (index < 0)
        {
            try
            {
                await SwitchInstrument.SwitchTo(0);
            }
            catch (Exception e)
            {
                PluginLog.Warning(e.ToString());
            }

            CurrentPlayback = null;
            return true;
        }

        var wasPlaying = IsPlaying;
        MidiFile midiFile = await PlaylistManager.LoadMidiFile(index);
        if (midiFile == null)
        {
            // delete file if can't be loaded(likely to be deleted locally)
            PluginLog.Debug($"[LoadPlayback] removing {index}");
            //PluginLog.Debug($"[LoadPlayback] removing {PlaylistManager.FilePathList[index].path}");
            PlaylistManager.FilePathList.RemoveAt(index);
            return false;
        }
        else
        {
            CurrentPlayback = await Task.Run(() =>
                {
                    CurrentPlayback?.Dispose();
                    CurrentPlayback = null;
                    var playback = GetPlaybackObject(midiFile, PlaylistManager.FilePathList[index].path);
                    return playback;
                });
            Ui.RefreshPlotData();
            PlaylistManager.CurrentPlaying = index;
            BardPlayDevice.Instance.ResetChannelStates();

            if (switchInstrument)
            {
                try
                {
                    var songName = PlaylistManager.FilePathList[index].fileName;
                    await SwitchInstrument.WaitSwitchInstrumentForSong(songName);
                }
                catch (Exception e)
                {
                    PluginLog.Warning(e.ToString());
                }
            }


            string[] pathArray = PlaylistManager.FilePathList[index].path.Split("\\");
            string LrcPath = "";
            string fileName = Path.GetFileNameWithoutExtension(PlaylistManager.FilePathList[index].path) + ".lrc";
            for (int i = 0; i < pathArray.Length - 1; i++)
            {
                LrcPath += pathArray[i];
                LrcPath += "\\";
            }
 
            LrcPath += fileName;
            Lrc lrc = Lrc.InitLrc(LrcPath);
            MidiPlayerControl.LrcTimeStamps = Lrc._lrc.LrcWord.Keys.ToList();
#if DEBUG
            PluginLog.LogVerbose($"Title: {lrc.Title}, Artist: {lrc.Artist}, Album: {lrc.Album}, LrcBy: {lrc.LrcBy}, Offset: {lrc.Offset}");
            foreach(var pair in lrc.LrcWord)
            {
                PluginLog.LogVerbose($"{pair.Key}, {pair.Value}");
            }

#endif
            if (switchInstrument && (wasPlaying || startPlaying))
            {
                MidiPlayerControl.DoPlay();
            }

            return true;
        }
    }

    private static bool needToCancel { get; set; } = false;

    internal static void PerformWaiting(float seconds)
    {
        waitStart = DateTime.Now;
        waitUntil = DateTime.Now.AddSeconds(seconds);
        while (DateTime.Now < waitUntil)
        {
            Thread.Sleep(10);
        }

        waitStart = null;
        waitUntil = null;
    }

    internal static void CancelWaiting()
    {
        waitStart = null;
        waitUntil = null;
        needToCancel = true;
    }

    internal static void StopWaiting()
    {
        waitStart = null;
        waitUntil = null;
    }
}