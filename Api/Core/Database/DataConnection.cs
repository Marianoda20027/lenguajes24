namespace core;

public class DataConnection
{
    public string ConnectionString {get;private set;}
    public static DataConnection Instance;
    public static void Init(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException($"{nameof(connectionString)} is required.");

        DataConnection.Instance = new DataConnection(connectionString);
    }

    private DataConnection (string connectionString)
    {
        ConnectionString = connectionString;
    }
}