using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using System.IO;

using System.Diagnostics;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using System.Data;
using System.Data.OleDb;

namespace ContentBuilder
{
    class ContentBuilder
    {
		static bool args_RedoAll = false;
		static bool args_RedoDDS = false;
		static bool args_RedoList = false;


		// Content directories
		const string ContentPath_Source = @"C:\Users\Ezra\Desktop\Dir\Pwnee\CK\Source\Content\";
		const string ContentPath_Source_Temp = @"C:\Users\Ezra\Desktop\Dir\Pwnee\CK\Source\Content\__premult\";

		const string ContentPath_WiiU = @"C:\Users\Ezra\Desktop\Dir\Pwnee\CK\Source\CloudberryKingdomPort\Cloudberry-Kingdom-Port\ContentWiiU\";
		const string ContentPath_PS3 = @"C:\Users\Ezra\Desktop\Dir\Pwnee\CK\Source\CloudberryKingdomPort\Cloudberry-Kingdom-Port\ContentPS3\";
		const string ContentPath_Xbox = @"C:\Users\Ezra\Desktop\Dir\Pwnee\CK\Source\Cloudberry Kingdom\Cloudberry Kingdom\Content\";
		const string ContentPath_PC_CPlusPlus = @"C:\Users\Ezra\Desktop\Dir\Pwnee\CK\Source\CloudberryKingdomPort\Cloudberry-Kingdom-Port\Content\";
		const string ContentPath_PC = @"C:\Users\Ezra\Desktop\Dir\Pwnee\CK\Source\Cloudberry Kingdom\Cloudberry Kingdom\ContentPC\";

		const string LoadListPath = @"C:\Users\Ezra\Desktop\Dir\Pwnee\CK\Source\CloudberryKingdomPort\Cloudberry-Kingdom-Port\Game\ResourceList\Resources_Art.h";

		// File types to copy besides textures
		static HashSet<string> FileTypes = new HashSet<string>()          { ".txt", ".tsv", ".fnt", ".smo", ".wav" };
		static HashSet<string> RemovableFileTypes = new HashSet<string>() { ".txt", ".tsv", ".fnt", ".smo", ".wav",
																				".png", ".dds", ".gtf", ".gtx", ".bmp" };
        // DDS tool
        const string Path_nvdxt = @"C:\Program Files (x86)\NVIDIA Corporation\DDS Utilities\nvdxt.exe";
        const string Params_nvdxt = @"-file ""{0}"" -output ""{1}"" ";

        // PS3 GTF tool
        const string Path_dds2gtf = @"C:\usr\local\cell\host-win32\bin\dds2gtf.exe";
        const string Params_dds2gtf = @"-o ""{0}"" ""{1}""";

        // WiiU GTX tool
        const string Path_TexConv2 = @"C:\Pwnee\cafe_sdk\system\bin\win32\TexConv2.exe";
        const string Params_TexConv2 = @"-i ""{0}"" -o ""{1}""";

		// WiiU Shader Compiler
		const string Path_gshConverter = @"C:\Pwnee\cafe_sdk\system\bin\win32\gshCompile.exe";
		const string Params_gshConverter = @"-v ""{0}"" -p ""{1}"" -o ""{2}""";

		// PS3 Shader Compiler
		static string ShaderSourceDir = Path.Combine(ContentPath_Source, "Shaders");
		static string Path_sce_cgc = @"sce-cgc";
		static string Params_sce_cgc_vertex = @"-I """ + ShaderSourceDir + @""" -p sce_vp_rsx -o ""{1}"" -e {2} ""{0}""";
		static string Params_sce_cgc_pixel =  @"-I """ + ShaderSourceDir + @""" -p sce_fp_rsx -o ""{1}"" -e {2} ""{0}""";
		const string Path_sce_cgcstrip = "sce-cgcstrip";
		const string Params_sce_cgcstrip = @"-param -o ""{1}"" ""{0}""";
		const string Path_cgnv2elf = "cgnv2elf";
		const string Params_cgnv2elf = @"--no-unref --no-sem  ""{0}"" ""{1}""";

		// LoadList template
		const string LoadListTemplate_SingleBuild = @"
const wchar_t *TEXTURE_PATHS[] = {{
{0}
}};

const int TEXTURE_WIDTHS[] = {{
{1}
}};

const int TEXTURE_HEIGHTS[] = {{
{2}
}};";
		const string LoadListTemplate = @"
#ifndef _RESOURCES_ART_H_
#define _RESOURCES_ART_H_

#include <vector>
#include <string>

#if defined(PS3)

{0}

{3}

#elif defined(CAFE)

{1}

#else

{2}

#endif

#endif
";

		const string LoadListTemplate_VideoMemory = @"
const bool VIDEO_MEMORY[] = {{
{0}
}};";

        public static List<string> GetFiles(string path, bool IncludeSubdirectories)
        {
            List<string> files = new List<string>();
            files.AddRange(Directory.GetFiles(path));

            if (IncludeSubdirectories)
            {
                string[] dir = Directory.GetDirectories(path);
                for (int i = 0; i < dir.Length; i++)
                    files.AddRange(GetFiles(dir[i], IncludeSubdirectories));
            }

            return files;
        }

        static Bitmap Load(string path)
        {
            return new Bitmap(path);
        }

        static byte Multiply(byte x, byte y)
        {
            return (byte)( 255f * ( (float)x / 255f ) * ( (float)y / 255f ) + .5f );
        }

        static void ConvertToPremultiplied(string source, string dest)
        {
			Directory.CreateDirectory(Path.GetDirectoryName(dest));

			var bitmap = Load(source);

            for (int i = 0; i < bitmap.Width; i++)
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color clr = bitmap.GetPixel(i, j);

                    Color premult = Color.FromArgb( clr.A,
                                                    Multiply(clr.R, clr.A),
                                                    Multiply(clr.G, clr.A),
                                                    Multiply(clr.B, clr.A) );
                    bitmap.SetPixel(i, j, premult );

                }

			bitmap.Save(dest, ImageFormat.Png);
        }

		static string SourcePath(string path) { return Path.Combine(ContentPath_Source, path + ".png"); }
        static string TempPremultPath(string path) { return Path.Combine( ContentPath_Source_Temp, path + ".png" ); }
		static string TempDDSPath(string path) { return Path.Combine(ContentPath_Source_Temp, path + ".dds"); }
		static string GetPath_Xbox_DDS(string path) { return Path.Combine(ContentPath_Xbox, path + ".dds"); }
		static string GetPath_WiiU_GTX(string path) { return Path.Combine(ContentPath_WiiU, path + ".gtx"); }
		static string GetPath_PS3_GTF(string path) { return Path.Combine(ContentPath_PS3, path   + ".gtf"); }
		static string GetPath_PC_CPlusPlus_PNG(string path) { return Path.Combine(ContentPath_PC_CPlusPlus, path + ".png"); }
		static string GetPath_PC_DDS(string path) { return Path.Combine(ContentPath_PC, path + ".dds"); }

		static string RunCommand(string Executable, string Arguments)
		{
			// Start the child process.
			Process p = new Process();

			// Redirect the output stream of the child process.
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.FileName = Executable;
			p.StartInfo.Arguments = Arguments;

			p.Start();

			string output = p.StandardOutput.ReadToEnd();
			p.WaitForExit();

			return output;
		}

        static void ConvertToDds(AssetInfo_Single info, string source, string dest)
        {
			Directory.CreateDirectory(Path.GetDirectoryName(dest));

			string Params = info.GetDssFormat();
			RunCommand(Path_nvdxt, string.Format(Params_nvdxt + Params, source, dest));
        }

        static void ConvertToGtf(string source, string dest)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dest));

			RunCommand(Path_dds2gtf, string.Format(Params_dds2gtf, dest, source));
        }

        static void ConvertToGtx(string source, string dest)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dest));

			RunCommand(Path_TexConv2, string.Format(Params_TexConv2, source, dest));
        }

		static void CompileWiiUShader(string PixelShaderPath, string VertexShaderPath)
		{
			string dest = Path.Combine(ContentPath_WiiU, "Shaders", Path.GetFileNameWithoutExtension(PixelShaderPath) + ".gsh");
			Directory.CreateDirectory(Path.GetDirectoryName(dest));

			RunCommand(Path_gshConverter, string.Format(Params_gshConverter, VertexShaderPath, PixelShaderPath, dest));
		}

		static Regex FindVertexShaderEntry = new Regex(@"VertexShader = compile VERTEX_SHADER (?<name>.*)\(\)");
		static Regex FindPixelShaderEntry = new Regex(@"PixelShader = compile PIXEL_SHADER (?<name>.*)\(\)");
		static string ShaderPS3Dir = Path.Combine(ContentPath_PS3, "Shaders");
		static void CompilePS3Shader(string ShaderPath)
		{
			string text = File.ReadAllText(ShaderPath);

			string pixel_entry  = FindPixelShaderEntry.Match(text).Groups["name"].Value;
			string vertex_entry = FindVertexShaderEntry.Match(text).Groups["name"].Value;

			string dest = Path.Combine(ShaderPS3Dir, Path.GetFileNameWithoutExtension(ShaderPath));
			Directory.CreateDirectory(Path.GetDirectoryName(dest));

			string dest_vertex = dest + ".vpo";
			string dest_pixel = dest + ".fpo";

			string dest_vertex_temp1 = dest + ".vpo_1";
			string dest_pixel_temp1 = dest + ".fpo_1";

			string dest_vertex_temp2 = dest + ".vpo_2";
			string dest_pixel_temp2 = dest + ".fpo_2";

			DateTime SourceDate = Date(ShaderPath);

			if (Date(dest + ".vpo") < SourceDate)
			{
				RunCommand(Path_sce_cgc, string.Format(Params_sce_cgc_vertex, ShaderPath, dest_vertex_temp1, vertex_entry));
				RunCommand(Path_sce_cgcstrip, string.Format(Params_sce_cgcstrip, dest_vertex_temp1, dest_vertex_temp2));
 				RunCommand(Path_cgnv2elf, string.Format(Params_cgnv2elf, dest_vertex_temp2, dest_vertex));
				File.Delete(dest_vertex_temp1);
				File.Delete(dest_vertex_temp2);
			}

			if (Date(dest + ".fpo") < SourceDate)
			{
				RunCommand(Path_sce_cgc, string.Format(Params_sce_cgc_pixel, ShaderPath, dest_pixel_temp1, pixel_entry));
				RunCommand(Path_sce_cgcstrip, string.Format(Params_sce_cgcstrip, dest_pixel_temp1, dest_pixel_temp2));
				RunCommand(Path_cgnv2elf, string.Format(Params_cgnv2elf, dest_pixel_temp2, dest_pixel));
				File.Delete(dest_pixel_temp1);
				File.Delete(dest_pixel_temp2);
			}
		}

		static Dictionary<string, string> StrToDssParams = new Dictionary<string, string>()
		{
			{ "Raw Rgba" , "-u8888" },
			{ "Raw Rgb, no alpha", "-u888" },
			{ "DXT1", "-dxt1c" },
			{ "DXT3", "-dxt3" },
			{ "DXT5", "-dxt5" },
			{ "R5G6B5", "-u565" },
		};

		struct AssetInfo_Single
		{
			public bool Include;
			public string Format;
			public bool Mipmap;

			public string GetDssFormat()
			{
				return StrToDssParams[ Format ] + ( Mipmap ? "" : " -nomipmap" );
			}
		}

		struct AssetInfo
		{
			public int Width, Height;

			public AssetInfo_Single Xbox, PS3, WiiU, PC;
			public string FilePath;

			public bool VideoMemory;

			public int GetWidth()
			{
				if (Width > 0) return Width;

				Width = new Bitmap(Path.Combine(ContentPath_Source, FilePath + ".png")).Width;
				
				return Width;
			}
		}

		static List<AssetInfo> TextureAssets = new List<AssetInfo>(1000);
		static HashSet<string> XboxFiles = new HashSet<string>(),
							   PS3Files = new HashSet<string>(),
							   WiiUFiles = new HashSet<string>(),
							   PCFiles = new HashSet<string>();

		static DateTime Date(string file)
		{
			return File.GetLastWriteTime(file);
		}

		static bool Equals(string s1, string s2)
		{
			return string.Compare(s1, s2, true) != 0;
		}

		static string GetFilePathWithoutExtension(string path)
		{
			return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
		}

		static void RemoveFiles(string Root, HashSet<string> ValidRelativePaths)
		{
			if (!Directory.Exists(Root)) return;

			List<string> Files = GetFiles(Root, true);
			foreach (var file in Files)
			{
				string extension = Path.GetExtension(file).ToLower();
				if (RemovableFileTypes.Contains(extension))
				{
					string relative_path = file.Replace(Root, "");
					string ext = Path.GetExtension(file);
					if (FileTypes.Contains(ext) && !ValidRelativePaths.Contains(relative_path) ||
						!FileTypes.Contains(ext) && !ValidRelativePaths.Contains(GetFilePathWithoutExtension(relative_path)))
					{
						File.Delete(file);
					}
				}
			}
		}

		static void Copy(string source, string dest)
		{
			if (File.Exists(dest))
			{
				File.Delete(dest);
			}

			Directory.CreateDirectory(Path.GetDirectoryName(dest));
			File.Copy(source, dest);
		}

		static void CopyIfNewer(string source, string dest)
		{
			if (Date(dest) < Date(source))
			{
				Copy(source, dest);
			}
		}

		static string ChangeRootDirectory(string path, string root_original, string root_new)
		{
			return Path.Combine(root_new, path.Replace(root_original, ""));
		}

		static void ParseAsset(ref AssetInfo_Single dest, ref AssetInfo_Single _default, object cell1, object cell2, object cell3)
		{
			dest.Include = (string)cell1 == " " ? _default.Include : (string)cell1 == "Include";
			dest.Format  = (string)cell2 == " " ? _default.Format  : (string)cell2;
			dest.Mipmap  = (string)cell3 == " " ? _default.Mipmap  : (string)cell3 == "Yes";
		}

		static byte[] Flip(byte[] b, int start, int n)
		{
			byte[] flipped = new byte[n];
			for (int i = 0; i < n; i++)
				flipped[i] = b[start + n - 1 - i];
			return flipped;
		}

		static uint BitConvert_UInt32(byte[] b, int start)
		{
			return BitConverter.ToUInt32(Flip(b, start, 4), 0);
		}

		static UInt16 BitConvert_UInt16(byte[] b, int start)
		{
			return BitConverter.ToUInt16(Flip(b, start, 2), 0);
		}

		static void Swap(ref byte b1, ref byte b2)
		{
			byte temp = b2;
			b2 = b1;
			b1 = temp;
		}

		static void PadPngWidth(string source, string dest)
		{
			var bitmap = new Bitmap(source);

			var padded = new Bitmap(bitmap.Width + 1, bitmap.Height);
			
			for (int i = 0; i < bitmap.Width; i++)
				for (int j = 0; j < bitmap.Height; j++)
					padded.SetPixel(i, j, bitmap.GetPixel(i, j));

			for (int j = 0; j < bitmap.Height; j++)
				padded.SetPixel(bitmap.Width, j, Color.Transparent);

			padded.Save(dest);
		}

		static void ChangeGtfWidth(string path_gtf, int new_width)
		{
			byte[] b = File.ReadAllBytes(path_gtf);

			//uint Version = BitConvert_UInt32(b, 0);
			//uint Size = BitConvert_UInt32(b, 4);
			//uint NumTexture = BitConvert_UInt32(b, 8);

			//uint Id = BitConvert_UInt32(b, 12);
			//uint OffsetToTex = BitConvert_UInt32(b, 16);
			//uint TextureSize = BitConvert_UInt32(b, 20);

			//byte format = b[24];
			//byte mipmap = b[25];
			//byte dimension = b[26];
			//byte cubemap = b[27];
			//uint remap = BitConvert_UInt32(b, 28);
			UInt16 width = BitConvert_UInt16(b, 32);
			//UInt16 height = BitConvert_UInt16(b, 34);

			//UInt16 depth = BitConvert_UInt16(b, 36);
			//byte location = b[38];
			//byte _padding = b[39];

			//uint pitch = BitConvert_UInt32(b, 40);
			//uint offset = BitConvert_UInt32(b, 44);

			// Set width
			BitConverter.GetBytes((UInt16)new_width).CopyTo(b, 32);
			Swap(ref b[32], ref b[33]);

			//width = BitConvert_UInt16(b, 32);
			//height = BitConvert_UInt16(b, 34);

			File.WriteAllBytes(path_gtf, b);
		}

		static void Main(string[] args)
        {
			if (args.Length > 0 && args[0] == "1") { args_RedoAll = true; Console.WriteLine("Arguments: Rebuilding all. (Will take a while)"); }
			if (args.Length > 1 && args[1] == "1") { args_RedoDDS = true; Console.WriteLine("Arguments: Rebuild DDS files. (Will take a while)"); }
			if (args.Length > 2 && args[2] == "1") { args_RedoList = true; Console.WriteLine("Arguments: Rebuild texture list for C++."); }

			// Compiler shaders
			List<string> ShaderFiles = GetFiles(Path.Combine(ContentPath_Source, "Shaders"), true);
			foreach (var file in ShaderFiles)
			{
				string extension = Path.GetExtension(file).ToLower();

				// For WiiU
				if (extension == ".ps")
					CompileWiiUShader(file, Path.ChangeExtension(file, ".vs"));

				// For PC C++
				if (extension == ".ps" || extension == ".vs")
					CopyIfNewer(file, ChangeRootDirectory(file, ContentPath_Source, ContentPath_PC_CPlusPlus));

				// For Xbox
				if (extension == ".fx")
					CopyIfNewer(file, ChangeRootDirectory(file, ContentPath_Source, ContentPath_Xbox));

				// For PS3
				if (extension == ".fx")
					CompilePS3Shader(file);
			}

			// Convert language excel file to unicode tsv
			ConvertMasterLocalization();

			// Get texture asset list
			string proj = Path.Combine(ContentPath_Source, @"Proj.xlsx");
			bool UpdateLoadList = args_RedoAll || args_RedoList || Date(LoadListPath) < Date(proj);
			string connection = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=Excel 12.0;", proj);

			var adapter = new OleDbDataAdapter("SELECT * FROM [Files$]", connection);
			var ds = new DataSet();

			adapter.Fill(ds);

			var table = ds.Tables["Table"];
			var data = table.AsEnumerable();

			foreach (var d in data)
			{
				AssetInfo asset = new AssetInfo();

				asset.FilePath = (string)d.ItemArray[15];
				asset.FilePath = asset.FilePath.Replace('/', '\\');

				// Get defaults
				AssetInfo_Single Default = new AssetInfo_Single();
				ParseAsset(ref Default, ref Default, d.ItemArray[12], d.ItemArray[13], d.ItemArray[14]);
				asset.VideoMemory = ((string)d.ItemArray[16]) == "Yes";

				// Xbox
				ParseAsset(ref asset.Xbox, ref Default, d.ItemArray[0], d.ItemArray[1], d.ItemArray[2]);
				if (asset.Xbox.Include) XboxFiles.Add(asset.FilePath);

				// PS3
				ParseAsset(ref asset.PS3, ref Default, d.ItemArray[3], d.ItemArray[4], d.ItemArray[5]);
				if (asset.PS3.Include) PS3Files.Add(asset.FilePath);

				// WiiU
				ParseAsset(ref asset.WiiU, ref Default, d.ItemArray[6], d.ItemArray[7], d.ItemArray[8]);
				if (asset.WiiU.Include) WiiUFiles.Add(asset.FilePath);

				// PC
				ParseAsset(ref asset.PC, ref Default, d.ItemArray[9], d.ItemArray[10], d.ItemArray[11]);
				if (asset.PC.Include) PCFiles.Add(asset.FilePath);

				if (UpdateLoadList)
				{
					var size = Load(Path.Combine(ContentPath_Source, asset.FilePath + ".png")).Size;
					asset.Width = size.Width;
					asset.Height = size.Height;
				}

				TextureAssets.Add(asset);
			}

			// Make texture list
			if (UpdateLoadList)
			{
				string TextureList_PS3 = "";
				string TextureList_Width_PS3 = "";
				string TextureList_Height_PS3 = "";
				string TextureList_VideoMemory = "";
				foreach (var file in TextureAssets)
				{
					if (file.PS3.Include)
					{
						TextureList_PS3 += string.Format("L\"{0}\",\n", file.FilePath.Replace("\\", "/"));
						TextureList_Width_PS3 += string.Format("{0},\n", file.Width);
						TextureList_Height_PS3 += string.Format("{0},\n", file.Height);
						TextureList_VideoMemory += string.Format("{0},\n", file.VideoMemory ? "true" : "false" );
					}
				}
				TextureList_PS3 = string.Format(LoadListTemplate_SingleBuild, TextureList_PS3, TextureList_Width_PS3, TextureList_Height_PS3);
				TextureList_VideoMemory = string.Format(LoadListTemplate_VideoMemory, TextureList_VideoMemory);

				string TextureList_WiiU = "";
				string TextureList_Width_WiiU = "";
				string TextureList_Height_WiiU = "";
				foreach (var file in TextureAssets)
				{
					if (file.WiiU.Include)
					{
						TextureList_WiiU += string.Format("L\"{0}\",\n", file.FilePath.Replace("\\", "/"));
						TextureList_Width_WiiU += string.Format("{0},\n", file.Width);
						TextureList_Height_WiiU += string.Format("{0},\n", file.Height);
					}
				}
				TextureList_WiiU = string.Format(LoadListTemplate_SingleBuild, TextureList_WiiU, TextureList_Width_WiiU, TextureList_Height_WiiU);

				string TextureList_PC = "";
				string TextureList_Width_PC = "";
				string TextureList_Height_PC = "";
				foreach (var file in TextureAssets)
				{
					if (file.PC.Include)
					{
						TextureList_PC += string.Format("L\"{0}\",\n", file.FilePath.Replace("\\", "/"));
						TextureList_Width_PC += string.Format("{0},\n", file.Width);
						TextureList_Height_PC += string.Format("{0},\n", file.Height);
					}
				}
				TextureList_PC = string.Format(LoadListTemplate_SingleBuild, TextureList_PC, TextureList_Width_PC, TextureList_Height_PC);

				string TextureList = string.Format(LoadListTemplate, TextureList_PS3, TextureList_WiiU, TextureList_PC, TextureList_VideoMemory);
				File.WriteAllText(LoadListPath, TextureList);
			}

			// Get non-texture assets
			List<string> Files = GetFiles(ContentPath_Source, true);
			foreach (var file in Files)
			{
				if (file.Contains("__ContentBuilder")) continue;

				string extension = Path.GetExtension(file).ToLower();
				if (FileTypes.Contains(extension))
				{
					string relative_path = file.Replace(ContentPath_Source, "");

					string xbox_dest = Path.Combine(ContentPath_Xbox, relative_path);
					CopyIfNewer(file, xbox_dest);
					XboxFiles.Add(relative_path);

					string wiiu_dest = Path.Combine(ContentPath_WiiU, relative_path);
					CopyIfNewer(file, wiiu_dest);
					WiiUFiles.Add(relative_path);

					string ps3_dest = Path.Combine(ContentPath_PS3, relative_path);
					CopyIfNewer(file, ps3_dest);
					PS3Files.Add(relative_path);

					string pc_dest = Path.Combine(ContentPath_PC, relative_path);
					CopyIfNewer(file, pc_dest);
					string pc_cplusplus_dest = Path.Combine(ContentPath_PC_CPlusPlus, relative_path);
					CopyIfNewer(file, pc_cplusplus_dest);
					PCFiles.Add(relative_path);
				}
			}

			// Remove files in destination folders that don't exist anymore.
			RemoveFiles(ContentPath_Xbox, XboxFiles);
			RemoveFiles(ContentPath_PS3, PS3Files);
			RemoveFiles(ContentPath_WiiU, WiiUFiles);
			RemoveFiles(ContentPath_PC_CPlusPlus, PCFiles);
			RemoveFiles(ContentPath_PC, PCFiles);

			// Convert and place all texture assets
            foreach (var asset in TextureAssets)
            {
				string source = SourcePath(asset.FilePath);
                string temp_premult = TempPremultPath(asset.FilePath);
				string temp_dds = TempDDSPath(asset.FilePath);
				DateTime source_date = Date(source);

                string path_xbox_dds = GetPath_Xbox_DDS(asset.FilePath);
				string path_ps3_gtf =  GetPath_PS3_GTF(asset.FilePath);
				string path_wiiu_gtx = GetPath_WiiU_GTX(asset.FilePath);
				string path_pc_cplusplus_png = GetPath_PC_CPlusPlus_PNG(asset.FilePath);
				string path_pc_dds = GetPath_PC_DDS(asset.FilePath);

				bool Cascade = false;
				if (args_RedoAll || Date(temp_premult) < Date(source))
                {
					ConvertToPremultiplied(source, temp_premult);

					Cascade = true;
                }

				if (asset.Xbox.Include)
				if (args_RedoAll || args_RedoDDS || Cascade || Date(path_xbox_dds) < source_date)
                {
                    ConvertToDds(asset.Xbox, temp_premult, temp_dds);
					Copy(temp_dds, path_xbox_dds);
                }

				if (asset.PC.Include)
				if (args_RedoAll || Cascade || Date(path_pc_cplusplus_png) < source_date)
				{
					Copy(temp_premult, path_pc_cplusplus_png);
				}
                
				if (asset.PC.Include)
				if (args_RedoAll || args_RedoDDS || Cascade || Date(path_pc_dds) < source_date)
				{
					ConvertToDds(asset.PC, temp_premult, temp_dds);
					Copy(temp_dds, path_pc_dds);
				}

				if (asset.PS3.Include)
				if (args_RedoAll || args_RedoDDS || Cascade || Date(path_ps3_gtf) < source_date)
                {
					string temp_premult_ps3 = temp_premult;

					// If the PNG is uncompressed RGBA and doesn't have an even width, pad the width
					bool pad = false;
					if (asset.PS3.Format == "Raw Rgba" && asset.GetWidth() % 2 == 1) pad = true;
					
					if (pad)
					{
						temp_premult_ps3 = Path.Combine(Path.GetDirectoryName(temp_premult), Path.GetFileNameWithoutExtension(temp_premult) + " (width padded).png");
						PadPngWidth(temp_premult, temp_premult_ps3);
					}

					ConvertToDds(asset.PS3, temp_premult_ps3, temp_dds);
                    ConvertToGtf(temp_dds, path_ps3_gtf);

					if (pad)
					{
						ChangeGtfWidth(path_ps3_gtf, asset.Width);
					}
                }

				if (asset.WiiU.Include)
				if (args_RedoAll || args_RedoDDS || Cascade || Date(path_wiiu_gtx) < source_date)
                {
					ConvertToDds(asset.WiiU, temp_premult, temp_dds);
                    ConvertToGtx(temp_dds, path_wiiu_gtx);
                }
            }

            Console.WriteLine("Done!");
        }

		private static void ConvertMasterLocalization()
		{
			string MasterPath = Path.Combine(ContentPath_Source, Path.Combine("Localization", "Translation Master.xlsx"));
			string TsvPath = Path.Combine(ContentPath_Source, Path.Combine("Localization", "Localization.tsv"));
			string TsvCppPath = Path.Combine(ContentPath_Source, Path.Combine("Localization", "LocalizationCpp.tsv"));
			
			bool UpdateLanguageFile = args_RedoAll || Date(TsvPath) < Date(MasterPath);
			if (!UpdateLanguageFile) return;

			var t = File.ReadAllBytes(MasterPath);
			var connection = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=Excel 12.0;", MasterPath);

			var adapter = new OleDbDataAdapter("SELECT * FROM [Localization$]", connection);
			var ds = new DataSet();

			adapter.Fill(ds);

			var table = ds.Tables["Table"];
			var data = table.AsEnumerable();

			string text = "";
			foreach (var d in data)
			{
				if (d.ItemArray.Length > 0 && d.ItemArray[0] is DBNull) continue;

				for (int i = 1; i < d.ItemArray.Length; i++)
					text += (string)d.ItemArray[i] + '\t';
				text += '\n';
			}

			File.WriteAllText(TsvPath, text);

			// Convert for printf
			text = text.Replace("{0}", "%ws");
			text = text.Replace("{1}", "%ws");
			File.WriteAllText(TsvCppPath, text);
		}
    }
}
