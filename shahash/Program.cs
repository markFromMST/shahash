using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace shahash
{
    internal class ShaHash
    {
        /// <summary>
        /// Supported hash algorithms.
        /// </summary>
        private enum Algorithms
        {
            md5, sha1, sha256, sha384, sha512
        }

        /// <summary>
        /// Given the algorithm choice, return an instance of the algorithm.
        /// </summary>
        /// <param name="algorithmChoice">Choice of algorithm from ShaHash.Algorithms enum.</param>
        /// <returns>HashAlgorithm instance.</returns>
        private static HashAlgorithm GetHashAlgorithm(Algorithms algorithmChoice)
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


        /// <summary>
        /// Computes the hash of the file with the specified instance of hash algorithm.
        /// </summary>
        /// <param name="filename">filename of file to hash.</param>
        /// <param name="hashAlgorithm">hash algorithm instance to compute the hash.</param>
        /// <returns>Computed hash as String.</returns>
        private static String ComputeHash(String filename, HashAlgorithm hashAlgorithm)
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

                    // Close the file.
                    fileStream.Close();

                    return stringBuilder.ToString();
                }
            }
            catch (FileNotFoundException)
            {
                throw new Exception(String.Format("Error: {0:s} could not be found.", filename));
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("Error: {0:s} could not be processed: {1:s}", filename, e.Message));
            }
        }

        /// <summary>
        /// Prints command help to standard out.
        /// </summary>
        private static void Help()
        {
            const String help = "shahash <algorithm:<md5>|sha1|sha256|sha384|sha512> <filename>";

            Console.WriteLine(help);
        }

        /// <summary>
        /// Entry point for shahash application.
        /// 
        /// The default form of the command is:
        ///     shahash <filename>
        /// example:
        ///     shahash somefile.zip
        /// which will print the md5 hash of somefile.zip to standard out:
        /// 
        /// Users can also specify the hash algorithm:
        ///     shahash <algorithm> <filename>
        /// example:
        ///     shahash sha256 somefile.zip
        /// which will print the sha256 hash of the file to standard out.
        /// 
        /// At the end, the hash is also copied to the clipboard for easy access/comparison.
        /// </summary>
        /// <param name="args">filename, or algorithm and filename</param>
        [STAThread] // for clipboard access
        private static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                try
                {
                    // default to md5 and just trust file exists here
                    String hash = ComputeHash(args[0], GetHashAlgorithm(Algorithms.md5));

                    // Write the name of the file to the Console.
                    Console.WriteLine(args[0] + " " + Algorithms.md5.ToString() + ": " + hash);

                    System.Windows.Clipboard.SetData(DataFormats.Text, hash);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Environment.Exit(1);
                }
            }
            else if (args.Length == 2)
            {
                // algorithm specified form of command
                if (!Enum.IsDefined(typeof(Algorithms), args[0]))
                {
                    // invalid algorithm, print help and exit
                    Console.WriteLine("Error: {0:s} is not a valid hash algorithm.", args[0]);
                    Help();
                    Environment.Exit(1);
                }
                else
                {
                    try
                    {
                        // valid algorithm, just trust file exists here
                        Algorithms algorithmChoice = (Algorithms)Enum.Parse(typeof(Algorithms), args[0]);
                        String hash = ComputeHash(args[1], GetHashAlgorithm(algorithmChoice));

                        // Write the name of the file to the Console.
                        Console.WriteLine(args[1] + " " + args[0] + ": " + hash);

                        // Copy the hash to the clipboard for easy verification with download file hash likely shown in browser.
                        System.Windows.Clipboard.SetData(DataFormats.Text, hash);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Environment.Exit(1);
                    }
                }
            }
            else
            {
                // completely invalid form of command
                Help();
                Environment.Exit(1);
            }
            Environment.Exit(0);
        }
    }
}