using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace SortPictures
{
    public static class Program
    {
        static IEnumerable<FileInfo> RecursiveGetFiles(this DirectoryInfo input)
        {
            return DirDeployExplore(input);
        }

        static public Stack<FileInfo> DirDeployExplore(DirectoryInfo dSrcDir)
        {
            try
            {
                Stack<DirectoryInfo> stackDirs = new Stack<DirectoryInfo>();
                Stack<FileInfo> stackPaths = new Stack<FileInfo>();

                stackDirs.Push(dSrcDir);
                while (stackDirs.Count > 0)
                {
                    DirectoryInfo currentDir = (DirectoryInfo)stackDirs.Pop();

                    //Process .\files
                    foreach (FileInfo fileInfo in currentDir.GetFiles())
                    {
                        stackPaths.Push(fileInfo);
                    }

                    //Process Subdirectories
                    foreach (DirectoryInfo diNext in currentDir.GetDirectories())
                        stackDirs.Push(diNext);
                }

                return stackPaths;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                return null;
            }
        }


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

                    IEnumerable<FileInfo> srcPics = inputDir.RecursiveGetFiles();

                    Regex regandroid = new Regex("\\d{8}_.*");

                    int count = 0;
                    foreach (FileInfo fi in srcPics)
                    {
                        if (extensions.Contains(fi.Extension.ToLower()))
                        {
                            if (++count % 10 == 0)
                                Console.WriteLine(string.Format("Files processed {0}/{1}", count, srcPics.Count()));

                            // Default date, LastWriteTime
                            DateTime fileDate = fi.LastWriteTime;


                            //20120612_124502.jpg

                            if (regandroid.IsMatch(fi.Name))
                            {
                                int year = int.Parse(fi.Name.Substring(0, 4));
                                int month = int.Parse(fi.Name.Substring(3, 2));
                                int day = int.Parse(fi.Name.Substring(5, 2));

                                fileDate = new DateTime(year, month, day);
                            }

                            string yearPath = Path.Combine(outDir, fileDate.Year.ToString());
                            string monthPath = Path.Combine(yearPath, fileDate.Month.ToString("00") + " - " + fileDate.ToString("MMMM"));
                            string destPath = Path.Combine(monthPath, fi.Name.TrimStart('_'));

                            if (!Directory.Exists(yearPath))
                                Directory.CreateDirectory(yearPath);
                            if (!Directory.Exists(monthPath))
                                Directory.CreateDirectory(monthPath);

                            if (!File.Exists(destPath))
                            {
                                fi.MoveTo(destPath);
                            }
                            else
                            {
                                // Duplicated file
                                fi.Delete();
                            }
                        }
                    }
                    Console.WriteLine(string.Format("{0} files moved", count));
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
    }
}
