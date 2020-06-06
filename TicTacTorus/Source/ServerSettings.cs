namespace TicTacTorus.Source
{
    /*
        Server settings, should be read from config file at the beginning. 
    */
    public struct ServerSettings
    {
        //the base url of the server. please read from here everywhere you use it.
        public string WebsiteDomain;

        //limit, enforced in the lobby
        public int MaxGridSize;

        //ram usage limit in KiB, enforced by not allowing to open a new lobby and limiting the grid size, if (max_ram - ram > max_grid).
        public long MaxRamKiB;
        
        //persistent storage limit in KiB, enforced by not offering to save replay or allowing to create accounts
        public long MaxDiskKiB;

    }
}