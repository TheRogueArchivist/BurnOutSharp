﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class ImpulseReactor : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            var matchers = new List<Matcher>
            {
                new Matcher(new List<byte?[]>
                {
                    // CVPInitializeClient
                    new byte?[]
                    {
                        0x43, 0x56, 0x50, 0x49, 0x6E, 0x69, 0x74, 0x69,
                        0x61, 0x6C, 0x69, 0x7A, 0x65, 0x43, 0x6C, 0x69,
                        0x65, 0x6E, 0x74
                    },

                    // A + (char)0x00 + T + (char)0x00 + T + (char)0x00 + L + (char)0x00 + I + (char)0x00 + S + (char)0x00 + T + (char)0x00 + (char)0x00 + (char)0x00 + E + (char)0x00 + L + (char)0x00 + E + (char)0x00 + M + (char)0x00 + E + (char)0x00 + N + (char)0x00 + T + (char)0x00 + (char)0x00 + (char)0x00 + N + (char)0x00 + O + (char)0x00 + T + (char)0x00 + A + (char)0x00 + T + (char)0x00 + I + (char)0x00 + O + (char)0x00 + N + (char)0x00
                    new byte?[]
                    {
                        0x41, 0x00, 0x54, 0x00, 0x54, 0x00, 0x4C, 0x00,
                        0x49, 0x00, 0x53, 0x00, 0x54, 0x00, 0x00, 0x00,
                        0x45, 0x00, 0x4C, 0x00, 0x45, 0x00, 0x4D, 0x00,
                        0x45, 0x00, 0x4E, 0x00, 0x54, 0x00, 0x00, 0x00,
                        0x4E, 0x00, 0x4F, 0x00, 0x54, 0x00, 0x41, 0x00,
                        0x54, 0x00, 0x49, 0x00, 0x4F, 0x00, 0x4E
                    },
                }, Utilities.GetFileVersion, "Impulse Reactor"),

                // CVPInitializeClient
                new Matcher(new byte?[]
                {
                    0x43, 0x56, 0x50, 0x49, 0x6E, 0x69, 0x74, 0x69,
                    0x61, 0x6C, 0x69, 0x7A, 0x65, 0x43, 0x6C, 0x69,
                    0x65, 0x6E, 0x74
                }, "Impulse Reactor"),
            };

            return MatchUtil.GetFirstContentMatch(file, fileContent, matchers, includePosition);
        }

        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (files.Any(f => Path.GetFileName(f).Equals("ImpulseReactor.dll", StringComparison.OrdinalIgnoreCase)))
                return "Impulse Reactor " + Utilities.GetFileVersion(files.First(f => Path.GetFileName(f).Equals("ImpulseReactor.dll", StringComparison.OrdinalIgnoreCase)));
            
            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("ImpulseReactor.dll", StringComparison.OrdinalIgnoreCase))
                    return "Impulse Reactor " + Utilities.GetFileVersion(path);

            return null;
        }
    }
}
