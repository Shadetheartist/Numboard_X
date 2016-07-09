using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NAudio.FileFormats.Map
{
    /// <summary>
    /// Represents a Cakewalk Drum Map file (.map)
    /// </summary>
    public class CakewalkMapFile
    {
        private int mapEntryCount;
        private readonly List<CakewalkDrumMapping> drumMappings;
        private MapBlockHeader fileHeader1;
        private MapBlockHeader fileHeader2;
        private MapBlockHeader mapNameHeader;
        private MapBlockHeader outputs1Header;
        private MapBlockHeader outputs2Header;
        private MapBlockHeader outputs3Header;
        int outputs1Count;
        int outputs2Count;
        int outputs3Count;

        private string mapName;

        /// <summary>
        /// Parses a Cakewalk Drum Map file
        /// </summary>
        /// <param name="filename">Path of the .map file</param>
        public CakewalkMapFile(string filename)
        {
            using (var reader = new BinaryReader(File.OpenRead(filename),Encoding.Unicode))
            {
                drumMappings = new List<CakewalkDrumMapping>();
                ReadMapHeader(reader);
                for (int n = 0; n < mapEntryCount; n++)
                {
                    drumMappings.Add(ReadMapEntry(reader));
                }
                ReadMapName(reader);
                ReadOutputsSection1(reader);
                if (reader.BaseStream.Position == reader.BaseStream.Length)
                    return;
                ReadOutputsSection2(reader);
                if (reader.BaseStream.Position == reader.BaseStream.Length)
                    return;
                ReadOutputsSection3(reader);
                System.Diagnostics.Debug.Assert(reader.BaseStream.Position == reader.BaseStream.Length);
            }
        }

        /// <summary>
        /// The drum mappings in this drum map
        /// </summary>
        public List<CakewalkDrumMapping> DrumMappings
        {
            get { return drumMappings; }
        }


        private void ReadMapHeader(BinaryReader reader)
        {
            fileHeader1 = MapBlockHeader.Read(reader);
            fileHeader2 = MapBlockHeader.Read(reader);
            mapEntryCount = reader.ReadInt32();
        }

        private CakewalkDrumMapping ReadMapEntry(BinaryReader reader)
        {
            var mapping = new CakewalkDrumMapping();
            reader.ReadInt32(); // unknown
            mapping.InNote = reader.ReadInt32();
            reader.ReadInt32(); // unknown
            reader.ReadInt32(); // unknown
            reader.ReadInt32(); // unknown
            reader.ReadInt32(); // unknown
            reader.ReadInt32(); // unknown
            reader.ReadInt32(); // unknown
            mapping.VelocityScale = reader.ReadSingle();
            mapping.Channel = reader.ReadInt32();
            mapping.OutNote = reader.ReadInt32();
            mapping.OutPort = reader.ReadInt32();
            mapping.VelocityAdjust = reader.ReadInt32();
            char[] name = reader.ReadChars(32);
            int nameLength;
            for (nameLength = 0; nameLength < name.Length; nameLength++)
            {
                if (name[nameLength] == 0)
                    break;
            }
            mapping.NoteName = new string(name, 0, nameLength);
            return mapping;
        }


        private void ReadMapName(BinaryReader reader)
        {
            mapNameHeader = MapBlockHeader.Read(reader);
            char[] name = reader.ReadChars(34);
            int nameLength;
            for (nameLength = 0; nameLength < name.Length; nameLength++)
            {
                if (name[nameLength] == 0)
                    break;
            }
            mapName = new string(name,0,nameLength);
            reader.ReadBytes(98); // unknown
        }

        private void ReadOutputsSection1(BinaryReader reader)
        {
            outputs1Header = MapBlockHeader.Read(reader);
            outputs1Count = reader.ReadInt32();
            for (int n = 0; n < outputs1Count; n++)
            {
                reader.ReadBytes(20); // data
            }
        }

        private void ReadOutputsSection2(BinaryReader reader)
        {
            outputs2Header = MapBlockHeader.Read(reader);
            outputs2Count = reader.ReadInt32();
            for (int n = 0; n < outputs2Count; n++)
            {
                reader.ReadBytes(24); // data
            }
        }
        
        private void ReadOutputsSection3(BinaryReader reader)
        {
            outputs3Header = MapBlockHeader.Read(reader);
            if (outputs3Header.Length > 0)
            {
                outputs3Count = reader.ReadInt32();
                for (int n = 0; n < outputs3Count; n++)
                {
                    reader.ReadBytes(36); // data
                }
            }
        }

        /// <summary>
        /// Describes this drum map
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("FileHeader1: {0}\r\n", fileHeader1);
            sb.AppendFormat("FileHeader2: {0}\r\n", fileHeader2);
            sb.AppendFormat("MapEntryCount: {0}\r\n", mapEntryCount);
            foreach (var mapping in drumMappings)
            {
                sb.AppendFormat("   Map: {0}\r\n", mapping);
            }
            sb.AppendFormat("MapNameHeader: {0}\r\n", mapNameHeader);
            sb.AppendFormat("MapName: {0}\r\n", mapName);
            sb.AppendFormat("Outputs1Header: {0} Count: {1}\r\n", outputs1Header, outputs1Count);
            sb.AppendFormat("Outputs2Header: {0} Count: {1}\r\n", outputs2Header, outputs2Count);
            sb.AppendFormat("Outputs3Header: {0} Count: {1}\r\n", outputs3Header, outputs3Count);
            return sb.ToString();
        }
    }


}
