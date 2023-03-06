using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FFMpegConcat
{
    class Program
    {
        static void Main(string[] args)
        {
            var dir = Directory.EnumerateDirectories(args.Length > 0 ? args[0] : "E:\\Course");

            foreach (var d in dir)
            {
                //转码或拷贝课程
                var scos = Directory.EnumerateDirectories(d, "sco*").ToList();
                scos.SortNatural(x => x);
                if (scos.Any())
                {
                    Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}\\Output\\{new DirectoryInfo(d).Name}");

                    try
                    {
                        File.Copy($"{d}\\imgs\\banner.png", $"{Directory.GetCurrentDirectory()}\\Output\\{new DirectoryInfo(d).Name}\\{new DirectoryInfo(d).Name}.png");
                    }
                    catch (Exception) { }

                    try
                    {
                        File.Copy($"{d}\\course\\course.doc", $"{Directory.GetCurrentDirectory()}\\Output\\{new DirectoryInfo(d).Name}\\course.doc");
                    }
                    catch (Exception) { }

                    try
                    {
                        File.Copy($"{d}\\teacher\\teacher.doc", $"{Directory.GetCurrentDirectory()}\\Output\\{new DirectoryInfo(d).Name}\\teacher.doc");
                    }
                    catch (Exception) { }

                    try
                    {
                        File.Copy($"{d}\\imsmanifest.xml", $"{Directory.GetCurrentDirectory()}\\Output\\{new DirectoryInfo(d).Name}\\imsmanifest.xml");
                    }
                    catch (Exception) { }

                    try
                    {
                        File.Copy($"{d}\\draft.zip", $"{Directory.GetCurrentDirectory()}\\Output\\{new DirectoryInfo(d).Name}\\draft.zip");
                    }
                    catch (Exception) { }

                    if (scos.Count() > 1)
                    {
                        using (var result = new StreamWriter(new FileStream($"concat.txt", FileMode.Create, FileAccess.Write)))
                        {
                            foreach (var sco in scos)
                            {
                                if (File.Exists($"{sco}\\1_H.mp4"))
                                {
                                    result.WriteLine($"file '{sco.Replace("\\", "/")}/1_H.mp4'");
                                }
                                else
                                {
                                    result.WriteLine($"file '{sco.Replace("\\", "/")}/1.mp4'");
                                }
                            }
                        }

                        var p = new Process();
                        p.StartInfo.WorkingDirectory = d;
                        p.StartInfo.FileName = Directory.GetCurrentDirectory() + "\\ffmpeg.exe";
                        p.StartInfo.Arguments = $"-f concat -safe 0 -i {Directory.GetCurrentDirectory()}\\concat.txt -c copy {Directory.GetCurrentDirectory()}\\Output\\{new DirectoryInfo(d).Name}\\{new DirectoryInfo(d).Name}.mp4";
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.RedirectStandardInput = false;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.Start();
                        Console.WriteLine(p.StandardOutput.ReadToEnd());
                    }
                    else
                    {
                        if (File.Exists($"{d}\\sco1\\1_H.mp4"))
                        {
                            File.Copy($"{d}\\sco1\\1_H.mp4", $"{Directory.GetCurrentDirectory()}\\Output\\{new DirectoryInfo(d).Name}\\{new DirectoryInfo(d).Name}.mp4");
                        }
                        else
                        {
                            File.Copy($"{d}\\sco1\\1.mp4", $"{Directory.GetCurrentDirectory()}\\Output\\{new DirectoryInfo(d).Name}\\{new DirectoryInfo(d).Name}.mp4");
                        }
                    }

                    //转换成纯音频
                    var y = new Process();
                    y.StartInfo.WorkingDirectory = d;
                    y.StartInfo.FileName = Directory.GetCurrentDirectory() + "\\ffmpeg.exe";
                    y.StartInfo.Arguments = $"-i {Directory.GetCurrentDirectory()}\\Output\\{new DirectoryInfo(d).Name}\\{new DirectoryInfo(d).Name}.mp4 -f mp3 {Directory.GetCurrentDirectory()}\\Output\\{new DirectoryInfo(d).Name}\\{new DirectoryInfo(d).Name}.mp3";
                    y.StartInfo.UseShellExecute = false;
                    y.StartInfo.RedirectStandardInput = false;
                    y.StartInfo.RedirectStandardOutput = true;
                    y.Start();
                    Console.WriteLine(y.StandardOutput.ReadToEnd());
                }
            }

        }
    }

    public static class ListExt
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string lhs, string rhs);

        //Version for lists of any type.
        public static void SortNatural<T>(this List<T> self, Func<T, string> stringSelector)
        {
            self.Sort((lhs, rhs) => StrCmpLogicalW(stringSelector(lhs), stringSelector(rhs)));
        }

        //Simpler version for List<string>
        public static void SortNatural(this List<string> self)
        {
            self.Sort(StrCmpLogicalW);
        }
    }
}
