using System.Text.RegularExpressions;
using MetadataExtractor;


namespace SortPictures
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 2)
                {
                    string input = args[0];
                    string outDir = args[1];
                    DirectoryInfo inputDir = new DirectoryInfo(input);
                    string extensions = Properties.Resources.ExtensionsToSort;

                    Console.WriteLine("inputDir: " + input);
                    Console.WriteLine("OutDir: " + outDir);
                    Console.WriteLine("Files considered: " + extensions);



                    string[] allfiles = System.IO.Directory.GetFiles(input, "*.*", SearchOption.AllDirectories);
                   

                    Regex regandroid = new Regex("^\\d{8}_.*");

                    int count = 0;
                    foreach (string filename in allfiles)
                    {
                        FileInfo fi = new FileInfo(filename);
                        if (extensions.Contains(fi.Extension.ToLower()))
                        {
                            if (++count % 10 == 0)
                                Console.WriteLine(string.Format("Files processed {0}/{1}", count, allfiles.Count()));

                            // Default date, LastWriteTime
                            DateTime fileDate = fi.LastWriteTime;


                            //20120612_124502.jpg Android naming scheme
                            if (regandroid.IsMatch(fi.Name))
                            {
                                int year = int.Parse(fi.Name.Substring(0, 4));
                                int month = int.Parse(fi.Name.Substring(4, 2));
                                int day = int.Parse(fi.Name.Substring(6, 2));

                                fileDate = new DateTime(year, month, day);
                            }
                            else //Try Exif Data
                            { 
                                if(fi.Extension == ".jpg" || fi.Extension == ".png")
                                {
                                    DateTime ans = GetDate(fi);
                                    if(ans != DateTime.MinValue)
                                    {
                                        fileDate = ans;
                                    }
                                }
                            }
                            string yearPath = Path.Combine(outDir, fileDate.Year.ToString());
                            string monthPath = Path.Combine(yearPath, fileDate.Month.ToString("00") + " - " + fileDate.ToString("MMMM"));
                            string destPath = Path.Combine(monthPath, fi.Name.TrimStart('_'));

                            if (!System.IO.Directory.Exists(yearPath))
                                System.IO.Directory.CreateDirectory(yearPath);
                            if (!System.IO.Directory.Exists(monthPath))
                                System.IO.Directory.CreateDirectory(monthPath);

                            if (!File.Exists(destPath))
                            {
                                fi.MoveTo(destPath);
                            }
                            else
                            {
                                // Duplicated file, do nothing
                                if(fi.FullName != destPath)
                                    Console.WriteLine(string.Format($"File {fi.FullName} exists at {destPath}"));
                            }
                        }
                    }
                    Console.WriteLine(string.Format("{0} files considered", count));
                    Console.WriteLine("That's all folks");
                }
                else
                {
                    Console.WriteLine("Sorts pictures from a single directory (drop out of the camera) to a directory tree (MainPicDir / Year / Month).");
                    Console.WriteLine("Sortpicture.exe <input directory> <output directory>");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.ReadLine();
            }
        }

        public static DateTime GetDate(FileInfo f)
        {
            var directories = ImageMetadataReader.ReadMetadata(f.FullName);

            foreach (var directory in directories)
                foreach (var tag in directory.Tags)
                {
                    if(tag.Name.Contains("Date/Time"))
                    {
                        var exifdate = tag.Description;

                        int year = int.Parse(exifdate.Substring(0, 4));
                        int month = int.Parse(exifdate.Substring(5, 2));
                        int day = int.Parse(exifdate.Substring(8, 2));

                        return new DateTime(year, month, day);
                    }
                }

            return DateTime.MinValue;
        }

    }
}
