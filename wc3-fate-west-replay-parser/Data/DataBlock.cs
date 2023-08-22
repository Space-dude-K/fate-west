namespace wc3_fate_west_parser_replay_parser.Data
{
    public class DataBlock
    {
        public ushort CompressedDataBlockSize { get; set; } //Excluding header
        public ushort DecompressedDataBlockSize { get; set; } //Should always be 8k
        public uint CheckSum { get; set; } // Presumably checksum
        public byte[] CompressedDataBlockBytes { get; set; } //Compressed using ZLib
        public byte[] DecompressedDataBlockBytes { get; set; }
    }
}