﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryObjectScanner.Interfaces;

namespace BinaryObjectScanner.Protection
{
    // TODO: Figure out how to use path check framework here
    public class DVDMoviePROTECT : IPathCheck
    {
        /// <inheritdoc/>
        public List<string> CheckDirectoryPath(string path, IEnumerable<string>? files)
        {
            var protections = new List<string>();
            if (files == null)
                return protections;

            if (Directory.Exists(Path.Combine(path, "VIDEO_TS")))
            {
                string[] bupfiles = files.Where(s => s.EndsWith(".bup")).ToArray();
                for (int i = 0; i < bupfiles.Length; i++)
                {
                    var bupfile = new FileInfo(bupfiles[i]);
                    if (bupfile.DirectoryName == null)
                        continue;

                    var ifofile = new FileInfo(Path.Combine(bupfile.DirectoryName, bupfile.Name.Substring(0, bupfile.Name.Length - bupfile.Extension.Length) + ".ifo"));
                    if (bupfile.Length != ifofile.Length)
                    {
                        protections.Add("DVD-Movie-PROTECT (Unconfirmed - Please report to us on Github)");
                        break;
                    }
                }
            }

            return protections;
        }

        /// <inheritdoc/>
        public string? CheckFilePath(string path)
        {
            return null;
        }
    }
}
