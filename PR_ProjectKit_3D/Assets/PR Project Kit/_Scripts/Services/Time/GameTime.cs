using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameTime {
	public const string MASTER_CHANNEL = "MASTER";
	private const string WARNING_REUSED_CHANNEL_NAME = "[Game Time] Warning! Multiple channels using name \"%1\". Please ensure all registered channels use unique names.";

	private static Dictionary<string, TimeChannel> channels = new Dictionary<string, TimeChannel> {
		{ MASTER_CHANNEL, new TimeChannel(MASTER_CHANNEL, null, 1) }
	};

	public static void RegisterChannel(TimeChannel newChannel) {

		if (channels.ContainsKey(newChannel.Name)) {
			Debug.LogWarning(string.Format(WARNING_REUSED_CHANNEL_NAME, newChannel.Name));
			return;
		}

		channels.Add(newChannel.Name, newChannel);
	}

	public static void RegisterChannel(string name, string parent = MASTER_CHANNEL, float defaultTimeScale = 1) {
		RegisterChannel(new TimeChannel(name, parent, defaultTimeScale));
	}

	public static bool DoesChannelExist(string name) {
		return channels.ContainsKey(name);
	}

	public static float UnscaledDeltaTime => Time.unscaledDeltaTime;
	public static float FixedUnscaledDeltaTime => Time.fixedUnscaledDeltaTime;

	public static float GetDeltaTime(string channel) {
		return Time.deltaTime * GetTimeScale(channel);
	}

	public static float GetFixedDeltaTime(string channel) {
		return Time.fixedDeltaTime * GetTimeScale(channel);
	}

	public static float GetTimeScale(string channel) {
		if (string.IsNullOrEmpty(channels[channel].Parent)) return channels[channel].TimeScale;
		return channels[channel].TimeScale * GetTimeScale(channels[channel].Parent);
	}
}
