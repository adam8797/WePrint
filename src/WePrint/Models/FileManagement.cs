namespace WePrint.Models
{
    public readonly struct FileEntry
    {
        public FileEntry(string name, string uri, long length)
        {
            Name = name;
            Location = uri;
            Length = length;
        }

        public string Name { get; }

        public string Location { get; }

        public long Length { get; }
    }
}
