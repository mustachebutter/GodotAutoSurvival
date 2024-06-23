using System;
using System.Collections.Generic;
using Godot;

public struct ProjectileData
{
    public string ProjectileId { get; set; }
    public string ProjectileName { get; set; }
    public string ProjectileDescription { get; set; }
	public float Damage { get; set; }
    public DamageTypes DamageType { get; set; }
    public float Speed { get; set; }
	public string AnimationName { get; set; }
    public PackedScene ProjectileScene { get; set; }
}

public static class ProjectileDataParser
{
    public static List<ProjectileData> ParseData(string path)
    {
        var projectiles = new List<ProjectileData>();
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if(file == null)
            GD.PrintErr($"Failed to parse file {path}");
        
        GD.Print(file);
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

                var projectileData = new ProjectileData
                {
                    ProjectileId = content[0],
                    ProjectileName = content[1],
                    ProjectileDescription = content[2],
                    Damage = float.Parse(content[3]),
                    DamageType = Enum.Parse<DamageTypes>(content[4].Split(".")[1]),
                    Speed = float.Parse(content[5]),
                    AnimationName = content[6],
                    ProjectileScene = GD.Load<PackedScene>(content[7]),
                };

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