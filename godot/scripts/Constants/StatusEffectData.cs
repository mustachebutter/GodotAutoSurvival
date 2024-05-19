using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public struct StatusEffectData
{
    public string StatusEffectId { get; set; }
    public string StatusEffectName { get; set; }
    public string StatusEffectDesc { get; set; }
    public string VisualEffectName { get; set; }
    public bool IsStackable { get; set; }
    public int NumberOfStacks { get; set; }
    public float Duration { get; set; }
	public float Damage { get; set; }
    public DamageTypes DamageType { get; set; }
	public float TickPerEverySecond { get; set; }
}

public static class StatusEffectDataParser
{
    public static List<StatusEffectData> ParseData(string path)
    {
        var statusEffects = new List<StatusEffectData>();
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if(file == null)
            GD.PrintErr($"Failed to parse file {path}");
        
        // Skip the first header line
        file.GetLine();
        try
        {
            while(!file.EofReached())
            {
                string[] content = file.GetCsvLine("\t");
                
                if (content.Length == 0)
                {
                    continue;
                }

                var statusEffectData = new StatusEffectData
                {
                    StatusEffectId = content[0],
                    StatusEffectName = content[1],
                    StatusEffectDesc = content[2],
                    VisualEffectName = content[3],
                    IsStackable = bool.Parse(content[4]),
                    NumberOfStacks = int.Parse(content[5]),
                    Duration = float.Parse(content[6]),
                    Damage = float.Parse(content[7]),
                    DamageType = Enum.Parse<DamageTypes>(content[8].Split(".")[1]),
                    TickPerEverySecond = float.Parse(content[9]),
                };
                GD.Print(statusEffectData.StatusEffectName);
                statusEffects.Add(statusEffectData);
            }


            if(statusEffects.Count == 0)
                GD.PrintErr("Wasn't able to parsed any status effect data!");
    
            return statusEffects;
        }
        catch(FormatException ex)
        {
            GD.PrintErr($"Failed to parse column for status effect: {ex}");
        }
        catch(IndexOutOfRangeException ex)
        {
            GD.PrintErr($"The number of parsed columns doesn't match the expected for status effect: {ex}");
        }
        catch(ArgumentNullException ex)
        {
            GD.PrintErr($"Tried to parse a null value: {ex}");
        }


        return null;
    }
}