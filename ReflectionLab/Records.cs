namespace ReflectionLab;

public readonly record struct PlayerData(string Id, int Level);

public record GuildData
{
    public string Name { get; set; } = "";
    public int MaxMembers { get; set; }
}
