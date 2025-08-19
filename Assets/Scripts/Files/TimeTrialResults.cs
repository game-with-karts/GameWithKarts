using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public struct TimeTrialResult {
    public int track;
    public uint lap1;
    public uint lap2;
    public uint lap3;
}

public sealed class TimeTrial {
    public const int VERSION = 0;
    private static readonly ITimeTrialSerialiser serialiser = new TimeTrialSerializerV0();
    private static readonly string path = Application.persistentDataPath + $"/times_{VERSION}";
    private static List<TimeTrialResult> results = new();
    public static bool Save() {
        BinaryWriter writer = new(File.Open(path, FileMode.OpenOrCreate));
        return serialiser.Save(writer, results);
    }

    public static TimeTrialResult? GetFromTrack(int track) {
        TimeTrialResult? result;
        try {
            result = results.Single(r => r.track == track);
        }
        catch (InvalidOperationException) {
            result = null;
        }
        return result;
    }

    public static void AddRecord(int track, uint lap1, uint lap2, uint lap3) => AddRecord(new() {
        track = track,
        lap1 = lap1,
        lap2 = lap2,
        lap3 = lap3
    });

    public static void AddRecord(TimeTrialResult result) {
        if (results.Any(r => r.track == result.track)) {
            results = results.Select(r => {
                if (r.track == result.track) {
                    r.lap1 = result.lap1;
                    r.lap2 = result.lap2;
                    r.lap3 = result.lap3;
                };
                return r;
            }).ToList();
            return;
        }
        results.Add(result);
    }

    public static IEnumerable<TimeTrialResult> LoadAll() {
        BinaryReader reader;
        int? version;
        try {
            reader = new(File.Open(path, FileMode.Open));
            version = reader.ReadInt32();
        }
        catch (FileNotFoundException) {
            reader = null;
            version = null;
        }


        ITimeTrialSerialiser serialiser = version switch {
            >= 0 => new TimeTrialSerializerV0(),
            _ => null,
        };
        return serialiser?.Load(reader);
    }
} 