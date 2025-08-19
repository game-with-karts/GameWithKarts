using UnityEngine;
using System;
using System.IO;
public class RaceSettingsFileHandler
{
    private string path = Application.persistentDataPath;
    public int Save(RaceSettings settings, string name) {
        FileStream file = new($"{path}/{name}.gwk", FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);

        writer.Write(settings.numberOfLaps);

        writer.Close();
        file.Close();
        return 0;
    }

    public int Load(string name, out RaceSettings settings) {
        settings = new();
        FileStream file = new($"{path}/{name}.gwk", FileMode.Open);
        BinaryReader reader = new(file);

        settings.numberOfLaps = reader.ReadByte();
        return 0;
    }
}