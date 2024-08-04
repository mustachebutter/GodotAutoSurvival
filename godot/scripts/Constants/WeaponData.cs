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

public static class WeaponDataParser
{
    public static List<WeaponData> ParseData(string path)
    {
        var projectiles = new List<WeaponData>();
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
                GD.Print(content.Length);
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
                GD.Print($"{projectileData.WeaponId}, {projectileData.AnimationName}, {projectileData.ProjectileScene}");
                projectiles.Add(projectileData);
            }


            if(projectiles.Count == 0)
                GD.PrintErr("Wasn't able to parsed any projectile data!");
    
            return projectiles;
        }
        catch(FormatException ex)
        {
            GD.PrintErr($"Failed to parse column for projectile: {ex}");
        }
        catch(IndexOutOfRangeException ex)
        {
            GD.PrintErr($"The number of parsed columns doesn't match the expected for projectile: {ex}");
        }
        catch(ArgumentNullException ex)
        {
            GD.PrintErr($"Tried to parse a null value: {ex}");
        }


        return null;
    }
}