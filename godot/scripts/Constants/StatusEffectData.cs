using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class StatusEffectData
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
            LoggingUtils.Error($"Failed to parse file {path}");
        
        // Skip the first header line
        file.GetLine();
        try
        {
            LoggingUtils.Info("StatusEffectData Parsing!!!", isBold: true, fontSize: 20);
            while(!file.EofReached())
            {
                string[] content = file.GetCsvLine("\t");
                
                LoggingUtils.Info($"Parsing {content.Length} column(s)");
                if (content.Length == 1)
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
                LoggingUtils.Info($"{statusEffectData.StatusEffectId}: {statusEffectData.StatusEffectName}");
                statusEffects.Add(statusEffectData);
            }


            if(statusEffects.Count == 0)
                LoggingUtils.Error("Wasn't able to parsed any status effect data!");
    
            return statusEffects;
        }
        catch(FormatException ex)
        {
            LoggingUtils.Error($"Failed to parse column for status effect: {ex}");
        }
        catch(IndexOutOfRangeException ex)
        {
            LoggingUtils.Error($"The number of parsed columns doesn't match the expected for status effect: {ex}");
        }
        catch(ArgumentNullException ex)
        {
            LoggingUtils.Error($"Tried to parse a null value: {ex}");
        }


        return null;
    }
}