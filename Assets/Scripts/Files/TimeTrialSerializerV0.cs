using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public sealed class TimeTrialSerializerV0 : ITimeTrialSerialiser {

    public IEnumerable<TimeTrialResult> Load(BinaryReader reader) {
        int count = reader.Read();
        TimeTrialResult temp;
        for (int i = 0; i < count; i++) {
            temp = new();
            try {
                temp.track = reader.Read();
                temp.lap1 = reader.ReadUInt32();
                temp.lap2 = reader.ReadUInt32();
                temp.lap3 = reader.ReadUInt32();
            }
            catch {
                yield break;
            }
            yield return temp;
        }
        reader.Close();
    }

    public bool Save(BinaryWriter writer, IEnumerable<TimeTrialResult> results) {
        try {
            writer.Write(TimeTrial.VERSION);
            writer.Write(results.Count());
            foreach (TimeTrialResult result in results) {
                writer.Write(result.track);
                writer.Write(result.lap1);
                writer.Write(result.lap2);
                writer.Write(result.lap3);
            }
        }
        catch {
            return false;
        }
        finally {
            writer.Close();
        }
        return true;
    }
}