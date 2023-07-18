using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;

namespace MinecraftJarLocator
{
    class Program
    {
        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.
        public static void ProcessDirectory(string targetDirectory)
        {
            try
            {
                // Process the list of files found in the directory.
                string[] fileEntries = Directory.GetFiles(targetDirectory);
                foreach (string fileName in fileEntries)
                {
                    try
                    {
                        Program.ProcessFile(fileName);
                    }
                    catch (Exception ex)
                    {

                    }
                }

                // Recurse into subdirectories of this directory.
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach (string subdirectory in subdirectoryEntries)
                    ProcessDirectory(subdirectory);
            } catch (Exception ex)
            {

            }
        }

        static bool stuffWasFound = false;

        public static string GetMD5(string filename)
        {
            using (var md5 = MD5.Create())
                using (var stream = File.OpenRead(filename))
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "");

        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string path)
        {
            try
            {
                using (ZipArchive zip = ZipFile.Open(path, ZipArchiveMode.Read))
                {
                    var mojangSigned = zip.GetEntry("META-INF/MOJANG_C.SF") != null;

                    var manifest = zip.GetEntry("META-INF/MANIFEST.MF");
                    if (mojangSigned)
                    {

                        var launcherEntry = zip.GetEntry("net/minecraft/Launcher.class");

                        if (launcherEntry != null)
                        {
                            string md5 = GetMD5(path);
                            Console.WriteLine("launcher " + manifest.LastWriteTime.ToString() + " MD5 " + md5 + " @ " + path);
                            stuffWasFound = true;
                            return;
                        }

                        var clientEntry = zip.GetEntry("terrain.png");
                        if (clientEntry != null)
                        {
                            string md5 = GetMD5(path);
                            Console.WriteLine("client " + manifest.LastWriteTime.ToString() + " MD5 " + md5 + " @ " + path);
                            stuffWasFound = true;
                            return;
                        }

                        var lzmaEntry = zip.GetEntry("LZMA/LzmaInputStream.class");
                        if (lzmaEntry != null)
                        {
                            //string md5 = GetMD5(path);
                            //Console.WriteLine("lzma " + manifest.LastWriteTime.ToString() + " MD5 " + md5 + " @ " + path);
                            //stuffWasFound = true;
                            return;
                        }

                        var lwjglAppletUtilEntry = zip.GetEntry("org/lwjgl/util/applet/AppletLoader.class");
                        if (lwjglAppletUtilEntry != null)
                        {
                            //string md5 = GetMD5(path);
                            //Console.WriteLine("lwjgl_applet_util " + manifest.LastWriteTime.ToString() + " MD5 " + md5 + " @ " + path);
                            //stuffWasFound = true;
                            return;
                        }

                        Console.WriteLine("unknown " + manifest.LastWriteTime.ToString() + " MD5 " + GetMD5(path) + " @ " + path);
                        stuffWasFound = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Minecraft Jar Locator v0.0.1 by craftycodie");
            Console.WriteLine();


            if (args.Length < 1)
            {
                Console.WriteLine("Please run this program in command prompt with the path of your choice like so:");
                Console.WriteLine("MinecraftJarLocator.exe \"C:\\Documents and Settings\\User\\Application Data\\Sun\\Java\\Deployment\\\"");
                Console.WriteLine();
                Console.WriteLine("Happy Hunting!");
                Console.WriteLine();
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return;
            }

            string cachePath = args[0];

            Console.WriteLine("Checking for Minecraft Jar files in " + cachePath);
            Console.WriteLine("This might take some time...");
            Console.WriteLine();

            ProcessDirectory(cachePath);

            if (stuffWasFound)
            {
                Console.WriteLine("Minecraft jars were found! Please share this output with Omniarchive.");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
