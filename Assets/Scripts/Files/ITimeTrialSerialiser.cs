using System.IO;
using System.Collections.Generic;

public interface ITimeTrialSerialiser {
    public bool Save(BinaryWriter writer, IEnumerable<TimeTrialResult> results);
    public IEnumerable<TimeTrialResult> Load(BinaryReader reader);
}