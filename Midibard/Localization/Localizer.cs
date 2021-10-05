﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard
{
	static class LocalizerExtension
	{
		internal static string Localize(this string message) => MidiBard.Localizer.Localize(message);
		internal static string Localize(this string format, params object[] objects) => string.Format(MidiBard.Localizer.Localize(format), objects);


	}
	class Localizer
	{
		public UILang Language;
		private Dictionary<string, string> zh = new();
		private Dictionary<string, string> en = new();
		public Localizer(UILang language)
		{
			Language = language;
			LoadZh();
		}
		public string Localize(string message)
		{
			if (message == null) return null;
			if (Language == UILang.CN) return zh.ContainsKey(message) ? zh[message] : message;
			if (Language == UILang.EN) return en.ContainsKey(message) ? en[message] : message;
			return message;
		}
		private void LoadZh()
		{
			zh.Add("Import midi file.", "导入MIDI文件");
			zh.Add("Clear Playlist", "清空播放列表");
			zh.Add("UI Language", "界面语言");
			zh.Add("Help", "常见问题");

			zh.Add("Change the UI Language.", "改变界面语言");
			zh.Add("Ensemble Mode Running", "合奏模式运行中");
			zh.Add("Ensemble Mode Preparing", "合奏模式准备小节");
			zh.Add("Import midi files to start performing!", "导入一些MIDI文件来开始演奏！");

			zh.Add($"tracks in playlist.", "首乐曲在播放列表中。");
			zh.Add($"Playing: ", "正在播放：");
			zh.Add($"track in playlist.", "首乐曲在播放列表中。");

			zh.Add($"Playmode: ", "播放模式：");
			zh.Add("Single", "单曲播放（单曲结束后停止）");
			zh.Add("ListOrdered", "列表顺序（列表结束后停止）");
			zh.Add("ListRepeat", "列表循环");
			zh.Add("SingleRepeat", "单曲循环");
			zh.Add("Random", "随机播放");

			zh.Add("Toggle player control panel", "演奏控制面板");
			zh.Add("Toggle settings panel", "播放器设置面板");
			zh.Add("Toggle mini player", "切换迷你播放器");
			zh.Add("Track Selection. \nMidiBard will only perform enabled tracks.\nLeft click to enable/disable a track, Right click to solo it.",
				"音轨选择。\r\nMIDIBARD只会演奏被选中的音轨。\n左键单击选择/取消选择音轨，右键单击Solo该音轨。");
			zh.Add("Track", "音轨");
			zh.Add($"notes)", "音符)");
			zh.Add("Transpose", "全音轨移调");
			zh.Add("Octave+", "升高八度");
			zh.Add("Octave-", "降低八度");
			zh.Add("Add 1 octave(+12 semitones) to all notes.", "对将要演奏的所有音符升高八度（加12半音）");
			zh.Add("Subtract 1 octave(-12 semitones) to all notes.", "对将要演奏的所有音符降低八度（减12半音）");
			zh.Add("Reset##note", "重置音高");
			zh.Add("Auto adapt notes", "自适应音高");
			zh.Add("Adapt high/low pitch notes which are out of range\r\ninto 3 octaves we can play",
				"对超出演奏范围的音符自动升/降八度直至其可以被演奏。");
			zh.Add("Progress", "演奏进度");
			zh.Add("Speed", "演奏速度");
			zh.Add("Monitor ensemble", "监控合奏");
			zh.Add("Auto start ensemble when entering in-game party ensemble mode.", "在游戏内的合奏助手运行时自动开始同步合奏。");
			zh.Add("Auto Confirm Ensemble Ready Check", "合奏准备自动确认");
			zh.Add("Playlist size", "播放列表大小");
			zh.Add("Play list rows number.", "播放列表窗口同时显示的行数");
			zh.Add("Player width", "播放器宽度");
			zh.Add("Player window max width.", "播放器窗口最大宽度");
			zh.Add("Auto open MidiBard", "自动打开MIDIBARD");
			zh.Add("Open MidiBard window automatically when entering performance mode", "在进入演奏模式时自动打开MIDIBARD窗口。");
			zh.Add("Import in progress...", "正在导入...");
			zh.Add("Select a mid file", "选择MID文件");
			zh.Add("Instrument", "乐器选择");
			zh.Add("Auto switch instrument", "自动切换乐器");
			zh.Add("Auto transpose", "自动移调");
			zh.Add("Auto switch instrument on demand. If you need this, \nplease add #instrument name# before file name.\nE.g. #harp#demo.mid",
				"根据要求自动切换乐器。如果需要自动切换乐器，请在文件开头添加 #乐器名#。\n例如：#鲁特琴#demo.mid");
			zh.Add("Auto transpose notes on demand. If you need this, \nplease add #transpose number# before file name.\nE.g. #-12#demo.mid",
				"根据要求自动移调。如果需要自动移调，请在文件开头添加 #要移调的半音数量#。\n例如：#-12#demo.mid");

			zh.Add("Transpose, measured by semitone. \nRight click to reset.", "移调，以半音数计算。\n点击+或-键升高或降低一个八度，右键点击来将它重置回0。");
			zh.Add("Set the speed of events playing. 1 means normal speed.\nFor example, to play events twice slower this property should be set to 0.5.\nRight Click to reset back to 1.",
				"设置Midi事件的播放速度倍数。\n例如将其设为0.5会使播放速度减半。\n右键点击来将它重置回1。");
			zh.Add("Set the playing progress. \nRight click to restart current playback.", "乐曲播放进度，右键点击回到乐曲开头。");
			zh.Add("Select current instrument. \nRight click to quit performance mode.", "设置和切换当前乐器，右键点击会退出演奏模式。");
			zh.Add("Override guitar tones", "自动电吉他音色");
			zh.Add("Listening input device: ", "正在监听MIDI输入：");
			zh.Add("Input Device", "输入设备");
			zh.Add("Choose external midi input device. right click to reset.", "选择当前的外部midi输入设备，例如虚拟midi接口或midi键盘。\n右键点击来停止使用外部输入。");
			zh.Add("Double click to clear playlist.", "双击来清空播放列表");
			zh.Add("Search playlist", "搜索播放列表");
			zh.Add("Assign different guitar tones for each midi tracks.", "为每个Midi轨道分别指定电吉他音色。");
			zh.Add("Theme color", "主题颜色");
			zh.Add("Enter to search", "输入开始搜索");
			zh.Add("Delay", "间隔时间");
			zh.Add("Delay time before play next track.", "在连续播放时每首乐曲播放结束后的等待时间。");
			zh.Add("Midibard auto performance only supports 37-key layout.\nPlease consider switching in performance settings.", "Midibard自动演奏仅支持37键布局。\n请考虑在操作设置中切换。");
			zh.Add("Transpose per track", "分音轨移调");
			zh.Add("Transpose per track, right click to reset all tracks' transpose offset back to zero.", "启用分音轨移调，右键点击将全部音轨的移调偏移重置回0。");
			zh.Add("Auto restart listening", "自动恢复监听");
			zh.Add("Auto listening new device", "自动开始监听");
			zh.Add("Try restart listening last used midi device on plugin start.", "在插件启动时尝试监听最后使用过的MIDI输入设备。");
			zh.Add("Auto start listening new midi input device when idle.", "尝试自动对新连接的MIDI设备开始监听。");
			zh.Add("Assign different guitar tones for each midi tracks", "为每个音轨指定不同的电吉他音色。");

		}
	}
}