using System;

class MapList
{
    private Dictionary<string, string> maps;
    public bool valid;


    public MapList()
    {
        maps = new Dictionary<string, string>();
        valid = true;
    }

    public string getKeyString(string command)
    {
        string s;
        if (maps.TryGetValue(command, out s))
            return s;
        else
            return "";
    }

    public bool addMapping(string command, string s)
    {
        if (maps.ContainsKey(command))
        {
            return false;
        }
        else
        {
            maps.Add(command, s);
            return true;
        }
    }