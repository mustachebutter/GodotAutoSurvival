using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class AugmentRateData
{
    public int Level { get; set; }
    public int CommonRate { get; set; }
    public int RareRate { get; set; }
    public int EpicRate { get; set; }
    public int LegendaryRate { get; set; }
    public int MythicRate { get; set; }
    public AugmentRateData DeepCopy()
    {
        return new AugmentRateData
        {
            Level = this.Level,
            CommonRate = this.CommonRate,
            RareRate = this.RareRate,
            EpicRate = this.EpicRate,
            LegendaryRate = this.LegendaryRate,
            MythicRate = this.MythicRate,
        };
    }
}

public static class AugmentRateDataParser
{
    public static List<AugmentRateData> ParseData(string path)
    {
        var augmentRates = new List<AugmentRateData>();
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if(file == null)
            LoggingUtils.Error($"Failed to parse file {path}");
        
        // Skip the first header line
        string[] keys = file.GetCsvLine("\t");
        try
        {
            LoggingUtils.Info("AugmentRateData Parsing!!!", isBold: true, fontSize: 20);
            while(!file.EofReached())
            {
                string[] content = file.GetCsvLine("\t");
                LoggingUtils.Info($"Parsing {content.Length} column(s)");
                if (content.Length == 1)
                {
                    continue;
                }

                var ar = new AugmentRateData
                {
                    Level = int.Parse(content[0]),
                    CommonRate = int.Parse(content[1]),
                    RareRate = int.Parse(content[2]),
                    EpicRate = int.Parse(content[3]),
                    LegendaryRate = int.Parse(content[4]),
                    MythicRate = int.Parse(content[5]),
                };
                
                augmentRates.Add(ar);
            }

            if(augmentRates.Count == 0)
                LoggingUtils.Error("Wasn't able to parsed any augment rate data!");
    
            return augmentRates;
        }
        catch(FormatException ex)
        {
            LoggingUtils.Error($"Failed to parse column for augment rate: {ex}");
        }
        catch(IndexOutOfRangeException ex)
        {
            LoggingUtils.Error($"The number of parsed columns doesn't match the expected for augment rate: {ex}");
        }
        catch(ArgumentNullException ex)
        {
            LoggingUtils.Error($"Tried to parse a null value: {ex}");
        }

        return null;
    }
}