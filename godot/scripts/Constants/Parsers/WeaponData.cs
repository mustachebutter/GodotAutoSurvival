using System;
using System.Collections.Generic;
using Godot;

public struct WeaponData
{
    public string WeaponId { get; set; }
    public string WeaponName { get; set; }
    public string WeaponDescription { get; set; }
	public float Damage { get; set; }
    public WeaponTypes WeaponType { get; set; }
    public DamageTypes DamageType { get; set; }
    public float AttackSpeed { get; set; }
    public float Speed { get; set; }
	public string AnimationName { get; set; }
    public PackedScene ProjectileScene { get; set; }
}

public class WeaponDamageData
{
    public string WeaponId { get; set; }
    public int MainLevel { get; set; }
    public DamageTypes DamageType { get; set; }
	public UpgradableObject Damage { get; set; }
    public UpgradableObject AttackSpeed { get; set; }
    public UpgradableObject Speed { get; set; }
}

public static class WeaponDataParser
{
    public static List<WeaponData> ParseData(string path)
    {
        var projectiles = new List<WeaponData>();
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if(file == null)
            LoggingUtils.Error($"Failed to parse file {path}");
        
        // Skip the first header line
        file.GetLine();
        try
        {
            LoggingUtils.Info("WeaponData Parsing!!!", isBold: true, fontSize: 20);
            while(!file.EofReached())
            {
                string[] content = file.GetCsvLine("\t");
                LoggingUtils.Info($"Parsing {content.Length} column(s)");
                if (content.Length == 1)
                {
                    continue;
                }

                var projectileData = new WeaponData
                {
                    WeaponId = content[0],
                    WeaponName = content[1],
                    WeaponDescription = content[2],
                    Damage = float.Parse(content[3]),
                    DamageType = Enum.Parse<DamageTypes>(content[4].Split(".")[1]),
                    WeaponType = Enum.Parse<WeaponTypes>(content[5].Split(".")[1]),
                    AttackSpeed = float.Parse(content[6]),
                    Speed = float.Parse(content[7]),
                    AnimationName = content[8],
                    ProjectileScene = GD.Load<PackedScene>(content[9]),
                };
                LoggingUtils.Info($"{projectileData.WeaponId}, {projectileData.AnimationName}");
                projectiles.Add(projectileData);
            }


            if(projectiles.Count == 0)
                LoggingUtils.Error("Wasn't able to parsed any projectile data!");
    
            return projectiles;
        }
        catch(FormatException ex)
        {
            LoggingUtils.Error($"Failed to parse column for projectile: {ex}");
        }
        catch(IndexOutOfRangeException ex)
        {
            LoggingUtils.Error($"The number of parsed columns doesn't match the expected for projectile: {ex}");
        }
        catch(ArgumentNullException ex)
        {
            LoggingUtils.Error($"Tried to parse a null value: {ex}");
        }


        return null;
    }

    public static List<WeaponDamageData> ParseDamageData(string path)
    {
        var damageDatas = new List<WeaponDamageData>();
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if(file == null)
            LoggingUtils.Error($"Failed to parse file {path}");
        
        // Skip the first header line
        file.GetLine();
        try
        {
            LoggingUtils.Info("WeaponDAMAGEData Parsing!!!", isBold: true, fontSize: 20);
            string currentWeaponId = "";

            while(!file.EofReached())
            {
                string[] content = file.GetCsvLine("\t");
                LoggingUtils.Info($"Parsing {content.Length} column(s)");
                if (content.Length == 1)
                {
                    continue;
                }

                if (content[0] != "")
                {
                    currentWeaponId = content[0];
                }

                var projectileDamageData = new WeaponDamageData
                {
                    WeaponId = currentWeaponId,
                    MainLevel = 1,
                    DamageType = Enum.Parse<DamageTypes>(content[1].Split(".")[1]),
                    Damage = new UpgradableObject { Name = "Damage", Level = 1, Value = float.Parse(content[2]) },
                    AttackSpeed = new UpgradableObject { Name = "AttackSpeed", Level = 1, Value = float.Parse(content[3]) },
                    Speed = new UpgradableObject { Name = "Speed", Level = 1, Value = float.Parse(content[4]) },
                };
                
                LoggingUtils.Info($"{projectileDamageData.WeaponId}, {projectileDamageData.DamageType}");
                damageDatas.Add(projectileDamageData);
            }


            if(damageDatas.Count == 0)
                LoggingUtils.Error("Wasn't able to parsed any projectile DAMAGE data!");
    
            return damageDatas;
        }
        catch(FormatException ex)
        {
            LoggingUtils.Error($"Failed to parse column for projectile damage data: {ex}");
        }
        catch(IndexOutOfRangeException ex)
        {
            LoggingUtils.Error($"The number of parsed columns doesn't match the expected for projectile damage data: {ex}");
        }
        catch(ArgumentNullException ex)
        {
            LoggingUtils.Error($"Tried to parse a null value: {ex}");
        }


        return null;
    }
}