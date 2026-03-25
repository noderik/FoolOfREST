internal class ApiKey{
    static public void Generate(int keySize = 20){
        string? envKey = Environment.GetEnvironmentVariable("APIKEY");
        if (envKey != null)
        {
            _key = envKey;
            return;
        }
        Random rand = new();
        for (int i = 0; i < keySize; i++){
            _key += Convert.ToChar(33+rand.NextInt64(93));
        }
        string path = Path.Join(Path.GetTempPath(), "ApiKey");
        using (StreamWriter sw = File.CreateText(path)){
            sw.Write(_key);
        }
    }

    static private string? _key;
    static public string? Key{get => _key;}
}
