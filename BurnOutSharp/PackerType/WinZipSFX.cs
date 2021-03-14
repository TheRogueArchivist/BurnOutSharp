using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;

namespace BurnOutSharp.PackerType
{
    public class WinZipSFX : IContentCheck, IScannable
    {
        /// <inheritdoc/>
        public bool ShouldScan(byte[] magic) => true;

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // WinZip Self-Extractor
            byte[] check = new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70, 0x20, 0x53, 0x65, 0x6C, 0x66, 0x2D, 0x45, 0x78, 0x74, 0x72, 0x61, 0x63, 0x74, 0x6F, 0x72 };
            if (fileContent.Contains(check, out int position))
            {
                // "<?xml"
                byte[] check2 = new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C };
                if (fileContent.Contains(check2, out int position2))
                    return $"WinZip SFX {GetV3PlusVersion(fileContent, position2)}" + (includePosition ? $" (Index {position}, {position2})" : string.Empty);
                else
                    return $"WinZip SFX {GetV2Version(fileContent)}" + (includePosition ? $" (Index {position})" : string.Empty);
            }

            // _winzip_
            check = new byte[] { 0x5F, 0x77, 0x69, 0x6E, 0x7A, 0x69, 0x70, 0x5F };
            if (fileContent.Contains(check, out position))
            {
                // "<?xml"
                byte[] check2 = new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C };
                if (fileContent.Contains(check2, out int position2))
                    return $"WinZip SFX {GetV3PlusVersion(fileContent, position2)}" + (includePosition ? $" (Index {position}, {position2})" : string.Empty);
                else
                    return $"WinZip SFX {GetV2Version(fileContent)}" + (includePosition ? $" (Index {position})" : string.Empty);
            }

            return null;
        }

        /// <inheritdoc/>
        public Dictionary<string, List<string>> Scan(Scanner scanner, string file)
        {
            if (!File.Exists(file))
                return null;

            using (var fs = File.OpenRead(file))
            {
                return Scan(scanner, fs, file);
            }
        }

        /// <inheritdoc/>
        public Dictionary<string, List<string>> Scan(Scanner scanner, Stream stream, string file)
        {
            // If the zip file itself fails
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                // Should be using stream instead of file, but stream fails to extract anything. My guess is that the executable portion of the archive is causing stream to fail, but not file.
                using (ZipArchive zipFile = ZipArchive.Open(file))
                {
                    foreach (var entry in zipFile.Entries)
                    {
                        // If an individual entry fails
                        try
                        {
                            // If we have a directory, skip it
                            if (entry.IsDirectory)
                                continue;

                            string tempFile = Path.Combine(tempPath, entry.Key);
                            entry.WriteToFile(tempFile);
                        }
                        catch { }
                    }
                }

                // Collect and format all found protections
                var protections = scanner.GetProtections(tempPath);

                // If temp directory cleanup fails
                try
                {
                    Directory.Delete(tempPath, true);
                }
                catch { }

                // Remove temporary path references
                Utilities.StripFromKeys(protections, tempPath);

                return protections;
            }
            catch { }

            return null;
        }

        private static string GetV3PlusVersion(byte[] fileContent, int xmlStartPosition)
        {
            // </assembly>
            byte[] check = new byte[] { 0x3C, 0x2F, 0x61, 0x73, 0x73, 0x65, 0x6D, 0x62, 0x6C, 0x79, 0x3E };
            if (fileContent.Contains(check, out int position, start: xmlStartPosition))
            {
                int offset = position + 11 - xmlStartPosition;
                string xmlString = Encoding.ASCII.GetString(fileContent, xmlStartPosition, offset);

                try
                {
                    // Load the XML string as a document
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlString);

                    // Get the version attribute, if possible
                    string xmlVersion = xmlDoc["assembly"]["assemblyIdentity"].GetAttributeNode("version").InnerXml;

                    // Some version strings don't exactly match the public version number
                    switch (xmlVersion)
                    {
                        case "3.0.7158.0":
                            return "3.0.7158";
                        case "3.1.7556.0":
                            return "3.1.7556";
                        case "3.1.8421.0":
                            return "4.0.8421";
                        case "3.1.8672.0":
                            return "4.0.8672";
                        case "4.0.1221.0":
                            return "4.0.12218";
                        default:
                            return $"(Unknown Version - {xmlVersion})";
                    }
                }
                catch { }
            }

            return GetV2Version(fileContent);
        }

        private static string GetV2Version(byte[] fileContent)
        {
            #region 16-bit NE Header Checks

            byte[] check = new byte[]
            {
                0x4E, 0x45, 0x11, 0x20, 0x86, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x0A, 0x03, 0x03, 0x00,
                0x00, 0x20, 0x00, 0x40, 0xE6, 0x2B, 0x01, 0x00,
                0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00,
                0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x58, 0x00,
                0x64, 0x00, 0x6C, 0x00, 0xB8, 0x44, 0x00, 0x00,
                0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
            };
            if (fileContent.Contains(check, out _))
                return "Version 2.0 (MS-DOS/16-bit)";

            check = new byte[]
            {
                0x4E, 0x45, 0x11, 0x20, 0x86, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x0A, 0x03, 0x03, 0x00,
                0x00, 0x20, 0x00, 0x40, 0x74, 0x31, 0x01, 0x00,
                0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00,
                0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x58, 0x00,
                0x64, 0x00, 0x6C, 0x00, 0x98, 0x01, 0x00, 0x00,
                0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
            };
            if (fileContent.Contains(check, out _))
                return "Version 2.0 (16-bit)";
                
            check = new byte[]
            {
                0x4E, 0x45, 0x11, 0x20, 0x80, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x0A, 0x03, 0x03, 0x00,
                0x00, 0x20, 0x00, 0x40, 0xA0, 0x24, 0x01, 0x00,
                0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x03, 0x00,
                0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x58, 0x00,
                0x64, 0x00, 0x6A, 0x00, 0x92, 0x01, 0x00, 0x00,
                0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
            };
            if (fileContent.Contains(check, out _))
                return "Compact Version 2.0 (16-bit)";
                
            check = new byte[]
            {
                0x4E, 0x45, 0x11, 0x20, 0xCD, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x02, 0x03, 0x03, 0x00,
                0x00, 0x20, 0x00, 0x40, 0xFA, 0x36, 0x01, 0x00,
                0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x05, 0x00,
                0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x97, 0x00,
                0xA3, 0x00, 0xAD, 0x00, 0xDF, 0x01, 0x00, 0x00,
                0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
            };
            if (fileContent.Contains(check, out _))
                return "Software Installation Version 2.0 (16-bit)";

            check = new byte[]
            {
                0x4E, 0x45, 0x11, 0x20, 0x86, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x0A, 0x03, 0x03, 0x00,
                0x00, 0x20, 0x00, 0x40, 0x86, 0x33, 0x01, 0x00,
                0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00,
                0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x58, 0x00,
                0x64, 0x00, 0x6C, 0x00, 0xC8, 0x43, 0x00, 0x00,
                0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
            };
            if (fileContent.Contains(check, out _))
                return "Version 2.1 RC2 (MS-DOS/16-bit)";

            check = new byte[]
            {
                0x4E, 0x45, 0x11, 0x20, 0xBE, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x02, 0x03, 0x03, 0x00,
                0x00, 0x20, 0x00, 0x40, 0x56, 0x3E, 0x01, 0x00,
                0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00,
                0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x90, 0x00,
                0x9C, 0x00, 0xA4, 0x00, 0xD0, 0x01, 0x00, 0x00,
                0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
            };
            if (fileContent.Contains(check, out _))
                return "Version 2.1 RC2 (16-bit)";

            check = new byte[]
            {
                0x4E, 0x45, 0x11, 0x20, 0x80, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x0A, 0x03, 0x03, 0x00,
                0x00, 0x20, 0x00, 0x40, 0x84, 0x2B, 0x01, 0x00,
                0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x03, 0x00,
                0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x58, 0x00,
                0x64, 0x00, 0x6A, 0x00, 0x92, 0x01, 0x00, 0x00,
                0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
            };
            if (fileContent.Contains(check, out _))
                return "Compact Version 2.1 RC2 (16-bit)";

            check = new byte[]
            {
                0x4E, 0x45, 0x11, 0x20, 0xBE, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x02, 0x03, 0x03, 0x00,
                0x00, 0x20, 0x00, 0x40, 0xAC, 0x43, 0x01, 0x00,
                0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00,
                0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x90, 0x00,
                0x9C, 0x00, 0xA4, 0x00, 0xD0, 0x01, 0x00, 0x00,
                0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
            };
            if (fileContent.Contains(check, out _))
                return "Software Installation Version 2.1 RC2 (16-bit)";

            check = new byte[]
            {
                0x4E, 0x45, 0x11, 0x20, 0x86, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x0A, 0x03, 0x03, 0x00,
                0x00, 0x20, 0x00, 0x3A, 0x96, 0x33, 0x01, 0x00,
                0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00,
                0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x58, 0x00,
                0x64, 0x00, 0x6C, 0x00, 0xC8, 0x43, 0x00, 0x00,
                0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
            };
            if (fileContent.Contains(check, out _))
                return "Version 2.1 (MS-DOS/16-bit)";

            check = new byte[]
            {
                0x4E, 0x45, 0x11, 0x20, 0xBE, 0x00, 0x02, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x02, 0x03, 0x03, 0x00,
                0x00, 0x20, 0x00, 0x3A, 0x7E, 0x3E, 0x01, 0x00,
                0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00,
                0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x90, 0x00,
                0x9C, 0x00, 0xA4, 0x00, 0xD0, 0x01, 0x00, 0x00,
                0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
            };
            if (fileContent.Contains(check, out _))
                return "Version 2.1 (16-bit)";

            check = new byte[]
            {
                0x4E, 0x45, 0x11, 0x20, 0x80, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x0A, 0x03, 0x03, 0x00,
                0x00, 0x20, 0x00, 0x3A, 0x90, 0x2B, 0x01, 0x00,
                0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x03, 0x00,
                0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x58, 0x00,
                0x64, 0x00, 0x6A, 0x00, 0x92, 0x01, 0x00, 0x00,
                0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
            };
            if (fileContent.Contains(check, out _))
                return "Compact Version 2.1 (16-bit)";

            check = new byte[]
            {
                0x4E, 0x45, 0x11, 0x20, 0xBE, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x02, 0x03, 0x03, 0x00,
                0x00, 0x20, 0x00, 0x3A, 0x08, 0x44, 0x01, 0x00,
                0x00, 0x00, 0x03, 0x00, 0x03, 0x00, 0x04, 0x00,
                0x4B, 0x00, 0x40, 0x00, 0x58, 0x00, 0x90, 0x00,
                0x9C, 0x00, 0xA4, 0x00, 0xD0, 0x01, 0x00, 0x00,
                0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03,
            };

            if (fileContent.Contains(check, out _))
                return "Software Installation Version 2.1 (16-bit)";

            #endregion

            #region 32-bit SFX Header Checks

            // .............8�92....�P..............�P..�P..�P..VW95SE.SFX
            check = new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x38, 0x9C, 0x39,
                0x32, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                0x00, 0x88, 0x50, 0x00, 0x00, 0x88, 0x50, 0x00,
                0x00, 0x56, 0x57, 0x39, 0x35, 0x53, 0x45, 0x2E,
                0x53, 0x46, 0x58,
            };
            if (fileContent.Contains(check, out _))
                return "Version 2.0 (32-bit)";

            // .............]�92....�P..............�P..�P..�P..VW95SRE.SFX
            check = new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x5D, 0x9C, 0x39,
                0x32, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                0x00, 0x88, 0x50, 0x00, 0x00, 0x88, 0x50, 0x00,
                0x00, 0x56, 0x57, 0x39, 0x35, 0x53, 0x52, 0x45,
                0x2E, 0x53, 0x46, 0x58,
            };
            if (fileContent.Contains(check, out _))
                return "Software Installation Version 2.0 (32-bit)";

            // .............���3....�P..............�P..�P..�P..VW95SE.SFX
            check = new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x84, 0x82, 0x94,
                0x33, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                0x00, 0x88, 0x50, 0x00, 0x00, 0x88, 0x50, 0x00,
                0x00, 0x56, 0x57, 0x39, 0x35, 0x53, 0x45, 0x2E,
                0x53, 0x46, 0x58,
            };
            if (fileContent.Contains(check, out _))
                return "Version 2.1 RC2 (32-bit)";

            // .............���3....�P..............�P..�P..�P..VW95SRE.SFX
            check = new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0xB0, 0x82, 0x94,
                0x33, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                0x00, 0x88, 0x50, 0x00, 0x00, 0x88, 0x50, 0x00,
                0x00, 0x56, 0x57, 0x39, 0x35, 0x53, 0x52, 0x45,
                0x2E, 0x53, 0x46, 0x58,
            };
            if (fileContent.Contains(check, out _))
                return "Software Installation Version 2.1 RC2 (32-bit)";

            // .............U��3....�P..............�P..�P..�P..VW95SE.SFX
            check = new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x55, 0xCD, 0xCC,
                0x33, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                0x00, 0x88, 0x50, 0x00, 0x00, 0x88, 0x50, 0x00,
                0x00, 0x56, 0x57, 0x39, 0x35, 0x53, 0x45, 0x2E,
                0x53, 0x46, 0x58,
            };
            if (fileContent.Contains(check, out _))
                return "Version 2.1 (32-bit)";

            // .............{��3....�P..............�P..�P..�P..VW95SRE.SFX
            check = new byte[]
            {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x7B, 0xCD, 0xCC,
                0x33, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x88, 0x50, 0x00,
                0x00, 0x88, 0x50, 0x00, 0x00, 0x88, 0x50, 0x00,
                0x00, 0x56, 0x57, 0x39, 0x35, 0x53, 0x52, 0x45,
                0x2E, 0x53, 0x46, 0x58,
            };
            if (fileContent.Contains(check, out _))
                return "Software Installation Version 2.1 (32-bit)";

            #endregion

            #region 32-bit PE Header Checks

            // PE..L...i.[:........�........J...*......�9.......`....@.
            check = new byte[]
            {
                0x50, 0x45, 0x00, 0x00, 0x4C, 0x01, 0x05, 0x00,
                0x69, 0x1B, 0x5B, 0x3A, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0xE0, 0x00, 0x0F, 0x01,
                0x0B, 0x01, 0x05, 0x0A, 0x00, 0x4A, 0x00, 0x00,
                0x00, 0x2A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0xD8, 0x39, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                0x00, 0x60, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00,
            };
            if (fileContent.Contains(check, out _))
                return "Version 2.2.4003";
                
            // PE..L.....[:........�........V...*.......?.......p....@.
            check = new byte[]
            {
                0x50, 0x45, 0x00, 0x00, 0x4C, 0x01, 0x05, 0x00,
                0x81, 0x1B, 0x5B, 0x3A, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0xE0, 0x00, 0x0F, 0x01,
                0x0B, 0x01, 0x05, 0x0A, 0x00, 0x56, 0x00, 0x00,
                0x00, 0x2A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x8F, 0x3F, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                0x00, 0x70, 0x00, 0x00, 0x00, 0x00, 0x40, 0x00, 
            };
            if (fileContent.Contains(check, out _))
                return "Software Installation Version 2.2.4003";

            #endregion

            return "Unknown Version 2.X";
        }
    }
}
