using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace shahash
{
    internal class Program
    {
        public enum Algorithms
        {
            md5, sha1, sha256, sha384, sha512
        }

        public static HashAlgorithm GetHashAlgorithm(Algorithms algorithmChoice)
        {
            switch (algorithmChoice)
            {
                case Algorithms.sha1:
                    return SHA1Managed.Create();

                case Algorithms.sha256:
                    return SHA256Managed.Create();

                case Algorithms.sha384:
                    return SHA384Managed.Create();

                case Algorithms.sha512:
                    return SHA512Managed.Create();

                default:
                    return MD5.Create();
            }
        }

        private static void ComputeHash(String filename, HashAlgorithm hashAlgorithm, String hashAlgorithmName)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filename);

                // Create a fileStream for the file.
                using (FileStream fileStream = fileInfo.Open(FileMode.Open))
                {
                    // Be sure it's positioned to the beginning of the stream.
                    fileStream.Position = 0;
                    // Compute the hash of the fileStream.
                    byte[] hashValue = hashAlgorithm.ComputeHash(fileStream);

                    StringBuilder stringBuilder = new StringBuilder();

                    // Write the hash value to the Console.
                    foreach (byte b in hashValue)
                    {
                        stringBuilder.AppendFormat("{0,2:x2}", b);
                    }

                    String hash = stringBuilder.ToString();
                    // Write the name of the file to the Console.
                    Console.WriteLine(fileInfo.Name + " " + hashAlgorithmName + ": " + hash);

                    System.Windows.Clipboard.SetData(DataFormats.Text, hash);

                    // Close the file.
                    fileStream.Close();
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(String.Format("Error: {0:s} could not be found.", filename));
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Error: {0:s} could not be processed: {1:s}", filename, e.Message));
            }
        }

        private static void Help()
        {
            const String help = "shahash <algorithm:<md5|sha1>|sha256|sha384|sha512> <filename>";

            Console.WriteLine(help);
        }

        [STAThread] // for clipboard access
        private static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                ComputeHash(args[0], GetHashAlgorithm(Algorithms.md5), Algorithms.md5.ToString());
            }
            else if (args.Length == 2)
            {
                if (!Enum.IsDefined(typeof(Algorithms), args[0]))
                {
                    Help();
                    return;
                }
                else
                {
                    Algorithms algorithmChoice = (Algorithms)Enum.Parse(typeof(Algorithms), args[0]);
                    ComputeHash(args[1], GetHashAlgorithm(algorithmChoice), algorithmChoice.ToString());
                    return;
                }
            }
            else
            {
                Help();
                return;
            }
        }
    }
}