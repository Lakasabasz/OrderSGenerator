namespace SetupCardTool;

public class Root
{
    public List<Setup> Connections { get; set; }
}

public class Setup
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Database { get; set; }
    public string DbUser { get; set; }
    public string DbPassword { get; set; }
    public int LocationId { get; set; }
    public string SetupName { get; set; }
}