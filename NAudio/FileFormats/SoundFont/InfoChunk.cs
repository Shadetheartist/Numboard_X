using System;
using System.IO;

namespace NAudio.SoundFont 
{
	/// <summary>
	/// A soundfont info chunk
	/// </summary>
	public class InfoChunk 
	{
		//private RiffChunk chunk;	
		private SFVersion verSoundFont;
		private string waveTableSoundEngine;
		private string bankName;
		private string dataROM;
		private string creationDate;
		private string author;
		private string targetProduct;
		private string copyright;
		private string comments;
		private string tools; // typically ToolUsedToCreate n.nn:MostRecentToolUsedForModification n.nn
		private SFVersion verROM;

		internal InfoChunk(RiffChunk chunk) 
		{
			bool ifilPresent = false;
			bool isngPresent = false;
			bool INAMPresent = false;
			if(chunk.ReadChunkID() != "INFO") 
			{
				throw new InvalidDataException("Not an INFO chunk");
			}
			//this.chunk = chunk;
			RiffChunk c;
			while((c = chunk.GetNextSubChunk()) != null) 
			{
				switch(c.ChunkID) 
				{
				case "ifil":
					ifilPresent = true;
					verSoundFont = (SFVersion) c.GetDataAsStructure(new SFVersionBuilder());
					break;
				case "isng":
					isngPresent = true;
					waveTableSoundEngine = c.GetDataAsString();
					break;
				case "INAM":
					INAMPresent = true;
					bankName = c.GetDataAsString();
					break;
				case "irom":
					dataROM = c.GetDataAsString();
					break;
				case "iver":
					verROM = (SFVersion) c.GetDataAsStructure(new SFVersionBuilder());
					break;
				case "ICRD":
					creationDate = c.GetDataAsString();
					break;
				case "IENG":
					author = c.GetDataAsString();
					break;
				case "IPRD":
					targetProduct = c.GetDataAsString();
					break;
				case "ICOP":
					copyright = c.GetDataAsString();
					break;
				case "ICMT":
					comments = c.GetDataAsString();
					break;
				case "ISFT":
					tools = c.GetDataAsString();
					break;
				default:
					throw new InvalidDataException(String.Format("Unknown chunk type {0}",c.ChunkID));
				}
			}
			if(!ifilPresent) 
			{
                throw new InvalidDataException("Missing SoundFont version information");
			}
			if(!isngPresent) 
			{
                throw new InvalidDataException("Missing wavetable sound engine information");
			}
			if(!INAMPresent) 
			{
                throw new InvalidDataException("Missing SoundFont name information");
			}
		}

		/// <summary>
		/// SoundFont Version
		/// </summary>
		public SFVersion SoundFontVersion 
		{
			get 
			{
				return verSoundFont;
			}
		}

		/// <summary>
		/// WaveTable sound engine
		/// </summary>
		public string WaveTableSoundEngine 
		{
			get 
			{
				return waveTableSoundEngine;
			}
			set 
			{
				// TODO: check format
				waveTableSoundEngine = value;
			}
		}

		/// <summary>
		/// Bank name
		/// </summary>
		public string BankName 
		{
			get 
			{
				return bankName;
			}
			set 
			{
				// TODO: check format
				bankName = value;
			}
		}

		/// <summary>
		/// Data ROM
		/// </summary>
		public string DataROM 
		{
			get 
			{
				return dataROM;
			}
			set {
				// TODO: check format
				dataROM = value;
			}
		}

		/// <summary>
		/// Creation Date
		/// </summary>
		public string CreationDate 
		{
			get 
			{
				return creationDate;
			}
			set 
			{
				// TODO: check format
				creationDate = value;
			}
		}

		/// <summary>
		/// Author
		/// </summary>
		public string Author 
		{
			get 
			{
				return author;
			}
			set 
			{
				// TODO: check format
				author = value;
			}
		}

		/// <summary>
		/// Target Product
		/// </summary>
		public string TargetProduct 
		{
			get 
			{
				return targetProduct;
			}
			set 
			{
				// TODO: check format
				targetProduct = value;
			}
		}

		/// <summary>
		/// Copyright
		/// </summary>
		public string Copyright 
		{
			get 
			{
				return copyright;
			}
			set 
			{
				// TODO: check format
				copyright = value;
			}
		}

		/// <summary>
		/// Comments
		/// </summary>
		public string Comments 
		{
			get 
			{
				return comments;
			}
			set 
			{
				// TODO: check format
				comments = value;
			}
		}

		/// <summary>
		/// Tools
		/// </summary>
		public string Tools 
		{
			get 
			{
				return tools;
			}
			set 
			{
				// TODO: check format
				tools = value;
			}
		}

		/// <summary>
		/// ROM Version
		/// </summary>
		public SFVersion ROMVersion 
		{
			get 
			{
				return verROM;
			}
			set 
			{
				verROM = value;
			}
		}

		/// <summary>
		/// <see cref="Object.ToString"/>
		/// </summary>
		public override string ToString() 
		{
			return String.Format("Bank Name: {0}\r\nAuthor: {1}\r\nCopyright: {2}\r\nCreation Date: {3}\r\nTools: {4}\r\nComments: {5}\r\nSound Engine: {6}\r\nSoundFont Version: {7}\r\nTarget Product: {8}\r\nData ROM: {9}\r\nROM Version: {10}",
				BankName,
				Author,
				Copyright,
				CreationDate,
				Tools,
				"TODO-fix comments",//Comments,
				WaveTableSoundEngine,
				SoundFontVersion,
				TargetProduct,
				DataROM,
				ROMVersion);
		}
	}

} // end of namespace