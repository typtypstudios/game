public interface ISaveBackend
{
    void Save(string key, string data);
    string Load(string key);
    bool Exists(string key);
    void Delete(string key);
}
