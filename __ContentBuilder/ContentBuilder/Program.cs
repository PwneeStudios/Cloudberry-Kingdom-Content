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

		//static Dictionary<string, string> CustomParams = new Dictionary<string,string>()
		//{
		//    // Uncompressed, with mipmaps
		//    { "Xbox_A",             "-u8888" },
		//    { "Xbox_B",             "-u8888" },
		//    { "Xbox_Dir",           "-u8888" },
		//    { "Xbox_LB",            "-u8888" },
		//    { "Xbox_LT",            "-u8888" },
		//    { "Xbox_RB",            "-u8888" },
		//    { "Xbox_RT",            "-u8888" },
		//    { "Xbox_Start",         "-u8888" },
		//    { "Xbox_X",             "-u8888" },
		//    { "Xbox_Y",             "-u8888" },
		//    { "Grobold_Japanese",   "-u8888" },
		//    { "Grobold_Western",    "-u8888" },

		//    // DXT1, no mipmaps
		//    { "Castle_Backdrop_2",      "-dxt1c -nomipmap" },
		//    { "castle_wall",            "-dxt1c -nomipmap" },
		//    { "castle_wall_p2",         "-dxt1c -nomipmap" },
		//    { "cave_backdrop",          "-dxt1c -nomipmap" },
		//    { "cave_backdrop_p2",       "-dxt1c -nomipmap" },
		//    { "cloud_castle_layer5",    "-dxt1c -nomipmap" },
		//    { "cloud_castle_layer5_p2", "-dxt1c -nomipmap" },
		//    { "hills_backdrop",         "-dxt1c -nomipmap" },
		//    { "hills_backdrop_p2",      "-dxt1c -nomipmap" },
		//    { "sea_backdrop",           "-dxt1c -nomipmap" },
		//    { "sea_backdrop_p2",        "-dxt1c -nomipmap" },
		//    { "Title_Blur",             "-dxt1c -nomipmap" },

		//    // dxt5, no mipmaps
		//    { "cave_bottom_1_p2_trim1",     "-dxt5 -nomipmap" },
		//    { "cave_bottom_1_p2_trim2",     "-dxt5 -nomipmap" },
		//    { "cave_bottom_1_trim1",        "-dxt5 -nomipmap" },
		//    { "cave_bottom_1_trim2",        "-dxt5 -nomipmap" },
		//    { "cave_bottom_2_p1",           "-dxt5 -nomipmap" },
		//    { "cave_bottom_2_p2",           "-dxt5 -nomipmap" },
		//    { "cloud_castle_layer4",        "-dxt5 -nomipmap" },
		//    { "door_castle_1",              "-dxt5 -nomipmap" },
		//    { "door_castle_2",              "-dxt5 -nomipmap" },
		//    { "door_cave_1",                "-dxt5 -nomipmap" },
		//    { "door_cave_2",                "-dxt5 -nomipmap" },
		//    { "door_cloud_1",               "-dxt5 -nomipmap" },
		//    { "door_cloud_2",               "-dxt5 -nomipmap" },
		//    { "door_forest_1",              "-dxt5 -nomipmap" },
		//    { "door_forest_2",              "-dxt5 -nomipmap" },
		//    { "door_hills_1",               "-dxt5 -nomipmap" },
		//    { "door_hills_2",               "-dxt5 -nomipmap" },
		//    { "door_sea_1",                 "-dxt5 -nomipmap" },
		//    { "door_sea_2",                 "-dxt5 -nomipmap" },
		//    { "forest_backhills_p2_trim",   "-dxt5 -nomipmap" },
		//    { "forest_backhills_trim",      "-dxt5 -nomipmap" },
		//    { "forest_clouds",              "-dxt5 -nomipmap" },
		//    { "forest_foretrees",           "-dxt5 -nomipmap" },
		//    { "forest_foretrees_p2",        "-dxt5 -nomipmap" },
		//    { "forest_mid_p2_trim",         "-dxt5 -nomipmap" },
		//    { "forest_mid_trim",            "-dxt5 -nomipmap" },
		//    { "forest_sky",                 "-dxt5 -nomipmap" },
		//    { "hills_backcastles_p2_trim",  "-dxt5 -nomipmap" },
		//    { "hills_backcastles_trim",     "-dxt5 -nomipmap" },
		//    { "hills_backhills",            "-dxt5 -nomipmap" },
		//    { "hills_backhills2_p2_trim",   "-dxt5 -nomipmap" },
		//    { "hills_backhills2_trim",      "-dxt5 -nomipmap" },
		//    { "hills_backhills_p2",         "-dxt5 -nomipmap" },
		//    { "hills_clouds",               "-dxt5 -nomipmap" },
		//    { "hills_hill1",                "-dxt5 -nomipmap" },
		//    { "hills_hill2",                "-dxt5 -nomipmap" },
		//    { "hills_hillandtree",          "-dxt5 -nomipmap" },
		//    { "hills_plants_1",             "-dxt5 -nomipmap" },
		//    { "hills_plants_2",             "-dxt5 -nomipmap" },
		//    { "hills_plants_3",             "-dxt5 -nomipmap" },
		//    { "hills_plants_4",             "-dxt5 -nomipmap" },
		//    { "hills_plants_5",             "-dxt5 -nomipmap" },
		//    { "hills_plants_6",             "-dxt5 -nomipmap" },
		//    { "hills_rock",                 "-dxt5 -nomipmap" },
		//    { "Pillar_Castle_1000",         "-dxt5 -nomipmap" },
		//    { "Pillar_Castle_600",          "-dxt5 -nomipmap" },
		//    { "Pillar_Cave_1000",           "-dxt5 -nomipmap" },
		//    { "Pillar_Cave_600",            "-dxt5 -nomipmap" },
		//    { "Pillar_Cloud_1000",          "-dxt5 -nomipmap" },
		//    { "Pillar_Cloud_600",           "-dxt5 -nomipmap" },
		//    { "Pillar_Forest_1000",         "-dxt5 -nomipmap" },
		//    { "Pillar_Forest_600",          "-dxt5 -nomipmap" },
		//    { "Pillar_Hills_1000",          "-dxt5 -nomipmap" },
		//    { "Pillar_Hills_600",           "-dxt5 -nomipmap" },
		//    { "Pillar_Sea_1000",            "-dxt5 -nomipmap" },
		//    { "Pillar_Sea_600",             "-dxt5 -nomipmap" },
		//    { "sea_water_1",                "-dxt5 -nomipmap" },
		//    { "sea_water_2",                "-dxt5 -nomipmap" },
		//    { "Wall_Castle",                "-dxt5 -nomipmap" },
		//    { "Wall_Cave",                  "-dxt5 -nomipmap" },
		//    { "Wall_Cloud",                 "-dxt5 -nomipmap" },
		//    { "Wall_Forest",                "-dxt5 -nomipmap" },
		//    { "Wall_Hills",                 "-dxt5 -nomipmap" },
		//    { "Wall_Sea",                   "-dxt5 -nomipmap" },

		//    // DXT5, no mipmaps
		//    { "castle_lava",    "-dxt5 -nomipmap" },

		//    // Uncompressed, no alpha, no mipmaps
		//    { "CharSelect",         "-u888 -nomipmap" },
		//    { "Scene_Kobbler",      "-u888 -nomipmap" },
		//    { "Scene_Kobbler_Blur", "-u888 -nomipmap" },
		//    { "Scene_Princess",     "-u888 -nomipmap" },
		//    { "Title_Screen",       "-u888 -nomipmap" },

		//};


namespace ContentBuilder
{
    class Program
    {
		// Content directories
		const string ContentPath_Source = @"C:\Users\Ezra\Desktop\Dir\Pwnee\CK\Source\Content\";
		const string ContentPath_Source_Temp = @"C:\Users\Ezra\Desktop\Dir\Pwnee\CK\Source\Content\__premult\";

		const string ContentPath_CPlusPlus = @"C:\Users\Ezra\Desktop\Dir\Pwnee\CK\Source\CloudberryKingdomPort\Cloudberry-Kingdom-Port\Content\";
		const string ContentPath_WiiU = @"C:\Users\Ezra\Desktop\Dir\Pwnee\CK\Source\CloudberryKingdomPort\Cloudberry-Kingdom-Port\ContentWiiU\";
		const string ContentPath_PS3 = @"C:\Users\Ezra\Desktop\Dir\Pwnee\CK\Source\CloudberryKingdomPort\Cloudberry-Kingdom-Port\ContentPS3\";
		const string ContentPath_Xbox = @"C:\Users\Ezra\Desktop\Dir\Pwnee\CK\Source\Cloudberry Kingdom\Cloudberry Kingdom\Content\";

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
		static string ShaderSourceDir = Path.Combine(ContentPath_Source, "Shader");
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

#elif defined(CAFE)

{1}

#endif

#endif
";

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
		static string ShaderPS3Dir = Path.Combine(ContentPath_PS3, "Shader");
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
			{ "DXT5", "dxt5" },
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

			public AssetInfo_Single Xbox, PS3, WiiU;
			public string Path;
		}

		static List<AssetInfo> TextureAssets = new List<AssetInfo>(1000);
		static HashSet<string> XboxFiles = new HashSet<string>(),
							   PS3Files = new HashSet<string>(),
							   WiiUFiles = new HashSet<string>();

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

        static void Main(string[] args)
        {
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
					CopyIfNewer(file, ChangeRootDirectory(file, ContentPath_Source, ContentPath_CPlusPlus));

				// For Xbox
				if (extension == ".fx")
					CopyIfNewer(file, ChangeRootDirectory(file, ContentPath_Source, ContentPath_Xbox));

				// For PS3
				if (extension == ".fx")
					CompilePS3Shader(file);
			}

			// Get texture asset list
			string proj = Path.Combine(ContentPath_Source, @"Proj.xlsx");
			bool UpdateLoadList = Date(LoadListPath) < Date(proj);
			string connection = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties=Excel 12.0;", proj);

			var adapter = new OleDbDataAdapter("SELECT * FROM [Files$]", connection);
			var ds = new DataSet();

			adapter.Fill(ds);

			var table = ds.Tables["Table"];
			var data = table.AsEnumerable();

			foreach (var d in data)
			{
				AssetInfo asset = new AssetInfo();

				asset.Path = (string)d.ItemArray[9];
				asset.Path = asset.Path.Replace('/', '\\');
				
				asset.Xbox.Include			= (string)d.ItemArray[0] == "Include";
				asset.Xbox.Format			= (string)d.ItemArray[1];
				asset.Xbox.Mipmap			= (string)d.ItemArray[2] == "Yes";
				if (asset.Xbox.Include) XboxFiles.Add(asset.Path);

				asset.PS3.Include			= (string)d.ItemArray[3] == "Include";
				asset.PS3.Format			= (string)d.ItemArray[4];
				asset.PS3.Mipmap			= (string)d.ItemArray[5] == "Yes";
				if (asset.PS3.Include) PS3Files.Add(asset.Path);

				asset.WiiU.Include			= (string)d.ItemArray[6] == "Include";
				asset.WiiU.Format			= (string)d.ItemArray[7];
				asset.WiiU.Mipmap			= (string)d.ItemArray[8] == "Yes";
				if (asset.WiiU.Include) WiiUFiles.Add(asset.Path);

				if (UpdateLoadList)
				{
					var size = Load(Path.Combine(ContentPath_Source, asset.Path + ".png")).Size;
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
				foreach (var file in TextureAssets)
				{
					if (file.PS3.Include)
					{
						TextureList_PS3 += string.Format("L\"{0}\",\n", file.Path);
						TextureList_Width_PS3 += string.Format("{0},\n", file.Width);
						TextureList_Height_PS3 += string.Format("{0},\n", file.Height);
					}
				}
				TextureList_PS3 = string.Format(LoadListTemplate_SingleBuild, TextureList_PS3, TextureList_Width_PS3, TextureList_Height_PS3);

				string TextureList_WiiU = "";
				string TextureList_Width_WiiU = "";
				string TextureList_Height_WiiU = "";
				foreach (var file in TextureAssets)
				{
					if (file.WiiU.Include)
					{
						TextureList_WiiU += string.Format("L\"{0}\",\n", file.Path);
						TextureList_Width_WiiU += string.Format("{0},\n", file.Width);
						TextureList_Height_WiiU += string.Format("{0},\n", file.Height);
					}
				}
				TextureList_WiiU = string.Format(LoadListTemplate_SingleBuild, TextureList_WiiU, TextureList_Width_WiiU, TextureList_Height_WiiU);

				string TextureList = string.Format(LoadListTemplate, TextureList_PS3, TextureList_WiiU);
				File.WriteAllText(LoadListPath, TextureList);
			}

			// Get non-texture assets
			List<string> Files = GetFiles(ContentPath_Source, true);
			foreach (var file in Files)
			{
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
				}
			}

			// Remove files in destination folders that don't exist anymore.
			RemoveFiles(ContentPath_Xbox, XboxFiles);
			RemoveFiles(ContentPath_PS3, PS3Files);
			RemoveFiles(ContentPath_WiiU, WiiUFiles);

			// Convert and place all texture assets
            foreach (var asset in TextureAssets)
            {
				string source = SourcePath(asset.Path);
                string temp_premult = TempPremultPath(asset.Path);
				string temp_dds = TempDDSPath(asset.Path);
				DateTime source_date = Date(source);

                string path_xbox_dds = GetPath_Xbox_DDS(asset.Path);
				string path_ps3_gtf =  GetPath_PS3_GTF(asset.Path);
				string path_wiiu_gtx = GetPath_WiiU_GTX(asset.Path);

				bool Cascade = false;
                if ( Date(temp_premult) < Date(source) )
                {
					ConvertToPremultiplied(source, temp_premult);

					Cascade = true;
                }
                
				if (asset.Xbox.Include)
				if (Cascade || Date(path_xbox_dds) < source_date)
                {
                    ConvertToDds(asset.Xbox, temp_premult, temp_dds);
					Copy(temp_dds, path_xbox_dds);
                }

				if (asset.PS3.Include)
				if (Cascade || Date(path_ps3_gtf) < source_date)
                {
					if (asset.PS3.GetDssFormat() != asset.Xbox.GetDssFormat() || !File.Exists(temp_dds))
						ConvertToDds(asset.PS3, temp_premult, temp_dds);

                    ConvertToGtf(temp_dds, path_ps3_gtf);
                }

				if (asset.WiiU.Include)
				if (Cascade || Date(path_wiiu_gtx) < source_date)
                {
					if (asset.WiiU.GetDssFormat() != asset.PS3.GetDssFormat() || !File.Exists(temp_dds))
						ConvertToDds(asset.WiiU, temp_premult, temp_dds);

                    ConvertToGtx(temp_dds, path_wiiu_gtx);
                }
            }

            Console.WriteLine("Done!");
        }
    }
}
