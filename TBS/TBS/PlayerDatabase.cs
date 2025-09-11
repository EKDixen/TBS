using Microsoft.Data.Sqlite;

public class PlayerDatabase
{
    private string connectionString = "Data Source=game.db";

    public PlayerDatabase()
    {
        CreateTable();
    }

    private void CreateTable()
    {
        using var con = new SqliteConnection(connectionString);
        con.Open();

        string sql = @"CREATE TABLE IF NOT EXISTS Players (
                        PlayerName TEXT PRIMARY KEY,
                        Data TEXT
                       )";

        using var cmd = new SqliteCommand(sql, con);
        cmd.ExecuteNonQuery();
    }

    public void SavePlayer(Player p)
    {
        using var con = new SqliteConnection(connectionString);
        con.Open();

        string json = Serializer.ToJson(p);

        string sql = "REPLACE INTO Players (PlayerName, Data) VALUES (@name, @data)";

        using var cmd = new SqliteCommand(sql, con);
        cmd.Parameters.AddWithValue("@name", p.playerName);
        cmd.Parameters.AddWithValue("@data", json);
        cmd.ExecuteNonQuery();
    }

    public Player? LoadPlayer(string username, string password)
    {
        using var con = new SqliteConnection(connectionString);
        con.Open();

        string sql = "SELECT Data FROM Players WHERE PlayerName = @name";
        using var cmd = new SqliteCommand(sql, con);
        cmd.Parameters.AddWithValue("@name", username);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            string json = reader.GetString(0);
            var player = Serializer.FromJson<Player>(json);

            if (player.password == password)
                return player;
        }

        return null;
    }
}
