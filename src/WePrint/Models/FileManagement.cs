namespace WePrint.Models
{
    public readonly struct file_entry
    {
        public file_entry(string name, string uri, long length)
        {
            this.name = name;
            location = uri;
            this.length = length;
        }

        public string name { get; }

        public string location { get; }

        public long length { get; }
    }
}
