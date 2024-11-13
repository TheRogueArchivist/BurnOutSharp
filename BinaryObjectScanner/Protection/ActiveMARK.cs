﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryObjectScanner.Interfaces;
using SabreTools.Matching;
using SabreTools.Matching.Content;
using SabreTools.Serialization.Wrappers;

namespace BinaryObjectScanner.Protection
{
    // TODO: Figure out how to get version numbers
    public class ActiveMARK : IContentCheck, IExecutableCheck<PortableExecutable>
    {
        /// <inheritdoc/>
        public string? CheckContents(string file, byte[] fileContent, bool includeDebug)
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            if (includeDebug)
            {
                var contentMatchSets = new List<ContentMatchSet>
                {
                    // " " + (char)0xC2 + (char)0x16 + (char)0x00 + (char)0xA8 + (char)0xC1 + (char)0x16 + (char)0x00 + (char)0xB8 + (char)0xC1 + (char)0x16 + (char)0x00 + (char)0x86 + (char)0xC8 + (char)0x16 + (char)0x00 + (char)0x9A + (char)0xC1 + (char)0x16 + (char)0x00 + (char)0x10 + (char)0xC2 + (char)0x16 + (char)0x00
                    new(new byte?[]
                    {
                        0x20, 0xC2, 0x16, 0x00, 0xA8, 0xC1, 0x16, 0x00,
                        0xB8, 0xC1, 0x16, 0x00, 0x86, 0xC8, 0x16, 0x00,
                        0x9A, 0xC1, 0x16, 0x00, 0x10, 0xC2, 0x16, 0x00
                    }, "ActiveMARK 5 (Unconfirmed - Please report to us on Github)"),
                };

                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);
            }

            return null;
        }

        /// <inheritdoc/>
        public string? CheckExecutable(string file, PortableExecutable pex, bool includeDebug)
        {
            // Get the sections from the executable, if possible
            var sections = pex.Model.SectionTable;
            if (sections == null)
                return null;

            // Get the entry point data, if it exists
            if (pex.EntryPointData != null)
            {
                // Found in "Zuma.exe"
                if (pex.EntryPointData.StartsWith(new byte?[] { 0x89, 0x25, 0x04, 0xF0, 0x86, 0x00, 0x68, 0x30 }))
                    return "ActiveMark v5.3.1078 (Packer Version)";

                // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
                else if (pex.EntryPointData.StartsWith(new byte?[] { 0x89, 0x25, null, null, null, null, 0xEB }))
                    return "ActiveMark -> Trymedia Systems Inc. (Unconfirmed - Please report to us on Github)";

                // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
                else if (pex.EntryPointData.StartsWith(new byte?[] { 0x89, 0x25, null, null, null, null, 0x33, 0xED, 0x55, 0x8B, 0xEC, 0xE8, null, null, null, null, 0x8B, 0xD0, 0x81, 0xE2, 0xFF, 0x00, 0x00, 0x00, 0x89, 0x15, null, null, null, null, 0x8B, 0xD0, 0xC1, 0xEA, 0x08, 0x81, 0xE2, 0xFF, 0x00, 0x00, 0x00, 0xA3, null, null, null, null, 0xD1, 0xE0, 0x0F, 0x93, 0xC3, 0x33, 0xC0, 0x8A, 0xC3, 0xA3, null, null, null, null, 0x68, 0xFF, 0x00, 0x00, 0x00, 0xE8, null, null, null, null, 0x6A, 0x00, 0xE8, null, null, null, null, 0xA3, null, null, null, null, 0xBB, null, null, null, null, 0xC7, 0x03, 0x44, 0x00, 0x00, 0x00 }))
                    return "ActiveMark -> Trymedia Systems Inc. (Unconfirmed - Please report to us on Github)";

                // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
                else if (pex.EntryPointData.StartsWith(new byte?[] { 0x20, 0x2D, 0x2D, 0x4D, 0x50, 0x52, 0x4D, 0x4D, 0x47, 0x56, 0x41, 0x2D, 0x2D, 0x00, 0x75, 0x73, 0x65, 0x72, 0x33, 0x32, 0x2E, 0x64, 0x6C, 0x6C, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x42, 0x6F, 0x78, 0x41, 0x00, 0x54, 0x68, 0x69, 0x73, 0x20, 0x61, 0x70, 0x70, 0x6C, 0x69, 0x63, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x20, 0x63, 0x61, 0x6E, 0x6E, 0x6F, 0x74, 0x20, 0x72, 0x75, 0x6E, 0x20, 0x77, 0x69, 0x74, 0x68, 0x20, 0x61, 0x6E, 0x20, 0x61, 0x63, 0x74, 0x69, 0x76, 0x65, 0x20, 0x64, 0x65, 0x62, 0x75, 0x67 }))
                    return "ActiveMARK 5.x -> Trymedia Systems Inc. (h) (Unconfirmed - Please report to us on Github)";

                // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
                else if (pex.EntryPointData.StartsWith(new byte?[] { 0x20, 0x2D, 0x2D, 0x4D, 0x50, 0x52, 0x4D, 0x4D, 0x47, 0x56, 0x41, 0x2D, 0x2D, 0x00, 0x75, 0x73, 0x65, 0x72, 0x33, 0x32, 0x2E, 0x64, 0x6C, 0x6C, 0x00, 0x4D, 0x65, 0x73, 0x73, 0x61, 0x67, 0x65, 0x42, 0x6F, 0x78, 0x41, 0x00, 0x54, 0x68, 0x69, 0x73, 0x20, 0x61, 0x70, 0x70, 0x6C, 0x69, 0x63, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x20, 0x63, 0x61, 0x6E, 0x6E, 0x6F, 0x74, 0x20, 0x72, 0x75, 0x6E, 0x20, 0x77, 0x69, 0x74, 0x68, 0x20, 0x61, 0x6E, 0x20, 0x61, 0x63, 0x74, 0x69, 0x76, 0x65, 0x20, 0x64, 0x65, 0x62, 0x75, 0x67, 0x67, 0x65, 0x72, 0x20, 0x69, 0x6E, 0x20, 0x6D, 0x65, 0x6D, 0x6F, 0x72, 0x79, 0x2E, 0x0D, 0x0A, 0x50, 0x6C, 0x65, 0x61, 0x73, 0x65, 0x20, 0x75, 0x6E, 0x6C, 0x6F, 0x61, 0x64, 0x20, 0x74, 0x68, 0x65, 0x20, 0x64, 0x65, 0x62, 0x75, 0x67, 0x67, 0x65, 0x72, 0x20, 0x61, 0x6E, 0x64, 0x20, 0x72, 0x65, 0x73, 0x74, 0x61, 0x72, 0x74, 0x20, 0x74, 0x68, 0x65, 0x20, 0x61, 0x70, 0x70, 0x6C, 0x69, 0x63, 0x61, 0x74, 0x69, 0x6F, 0x6E, 0x2E, 0x00, 0x57, 0x61, 0x72, 0x6E, 0x69, 0x6E, 0x67 }))
                    return "ActiveMARK 5.x -> Trymedia Systems,Inc. (Unconfirmed - Please report to us on Github)";

                // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
                else if (pex.EntryPointData.StartsWith(new byte?[] { 0x79, 0x11, 0x7F, 0xAB, 0x9A, 0x4A, 0x83, 0xB5, 0xC9, 0x6B, 0x1A, 0x48, 0xF9, 0x27, 0xB4, 0x25 }))
                    return "ActiveMARK[TM] (Unconfirmed - Please report to us on Github)";

                // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
                else if (pex.EntryPointData.StartsWith(new byte?[] { 0x79, 0x07, 0x0F, 0xB7, 0x07, 0x47, 0x50, 0x47, 0xB9, 0x57, 0x48, 0xF2, 0xAE, 0x55, 0xFF, 0x96, 0x84, null, 0x00, 0x00, 0x09, 0xC0, 0x74, 0x07, 0x89, 0x03, 0x83, 0xC3, 0x04, 0xEB, 0xD8, 0xFF, 0x96, 0x88, null, 0x00, 0x00, 0x61, 0xE9, null, null, null, 0xFF }))
                    return "ActiveMARK[TM] R5.31.1140 -> Trymedia (Unconfirmed - Please report to us on Github)";

                // https://raw.githubusercontent.com/wolfram77web/app-peid/master/userdb.txt
                else if (pex.EntryPointData.StartsWith(new byte?[] { 0xBE, 0x48, 0x01, 0x40, 0x00, 0xAD, 0x8B, 0xF8, 0x95, 0xA5, 0x33, 0xC0, 0x33, 0xC9, 0xAB, 0x48, 0xAB, 0xF7, 0xD8, 0xB1, 0x04, 0xF3, 0xAB, 0xC1, 0xE0, 0x0A, 0xB5, 0x1C, 0xF3, 0xAB, 0xAD, 0x50, 0x97, 0x51, 0xAD, 0x87, 0xF5, 0x58, 0x8D, 0x54, 0x86, 0x5C, 0xFF, 0xD5, 0x72, 0x5A, 0x2C, 0x03, 0x73, 0x02, 0xB0, 0x00, 0x3C, 0x07, 0x72, 0x02, 0x2C, 0x03, 0x50, 0x0F, 0xB6, 0x5F, 0xFF, 0xC1, 0xE3, 0x03, 0xB3, 0x00, 0x8D, 0x1C, 0x5B, 0x8D, 0x9C, 0x9E, 0x0C, 0x10, 0x00, 0x00, 0xB0, 0x01, 0x67, 0xE3, 0x29, 0x8B, 0xD7, 0x2B, 0x56, 0x0C, 0x8A, 0x2A, 0x33, 0xD2, 0x84, 0xE9, 0x0F, 0x95, 0xC6, 0x52, 0xFE, 0xC6, 0x8A, 0xD0, 0x8D, 0x14, 0x93, 0xFF, 0xD5, 0x5A, 0x9F, 0x12, 0xC0, 0xD0, 0xE9, 0x74, 0x0E, 0x9E, 0x1A, 0xF2, 0x74, 0xE4, 0xB4, 0x00, 0x33, 0xC9, 0xB5, 0x01, 0xFF, 0x55, 0xCC, 0x33, 0xC9, 0xE9, 0xDF, 0x00, 0x00, 0x00, 0x8B, 0x5E, 0x0C, 0x83, 0xC2, 0x30, 0xFF, 0xD5, 0x73, 0x50, 0x83, 0xC2, 0x30, 0xFF, 0xD5, 0x72, 0x1B, 0x83, 0xC2, 0x30, 0xFF, 0xD5, 0x72, 0x2B, 0x3C, 0x07, 0xB0, 0x09, 0x72, 0x02, 0xB0, 0x0B, 0x50, 0x8B, 0xC7, 0x2B, 0x46, 0x0C, 0xB1, 0x80, 0x8A, 0x00, 0xEB, 0xCF, 0x83, 0xC2, 0x60, 0xFF, 0xD5, 0x87, 0x5E, 0x10, 0x73, 0x0D, 0x83, 0xC2, 0x30, 0xFF, 0xD5, 0x87, 0x5E, 0x14, 0x73, 0x03, 0x87, 0x5E, 0x18, 0x3C, 0x07, 0xB0, 0x08, 0x72, 0x02, 0xB0, 0x0B, 0x50, 0x53, 0x8D, 0x96, 0x7C, 0x07, 0x00, 0x00, 0xFF, 0x55, 0xD0, 0x5B, 0x91, 0xEB, 0x77, 0x3C, 0x07, 0xB0, 0x07, 0x72, 0x02, 0xB0, 0x0A, 0x50, 0x87, 0x5E, 0x10, 0x87, 0x5E, 0x14, 0x89, 0x5E, 0x18, 0x8D, 0x96, 0xC4, 0x0B, 0x00, 0x00, 0xFF, 0x55, 0xD0, 0x50, 0x48 }))
                    return "ActiveMARK 5.x -> Trymedia Systems,Inc. (h) (Unconfirmed - Please report to us on Github)";
            }

            // Get the .data section strings, if they exist
            var strs = pex.GetLastSectionStrings(".data");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("MPRMMGVA"))
                    && strs.Exists(s => s.Contains("This application cannot run with an active debugger in memory.")))
                {
                    return "ActiveMARK 6.x";
                }
            }

            // Get "REGISTRY, AMINTERNETPROTOCOL" resource items
            var resources = pex.FindResourceByNamedType("REGISTRY, AMINTERNETPROTOCOL");
            if (resources.Any())
            {
                bool match = resources
                    .Select(r => r == null ? string.Empty : Encoding.ASCII.GetString(r))
                    .Any(r => r.Contains("ActiveMARK"));
                if (match)
                    return "ActiveMARK";
            }

            // Get the overlay data, if it exists
            if (pex.OverlayStrings != null)
            {
                if (pex.OverlayStrings.Exists(s => s.Contains("TMSAMVOH")))
                    return "ActiveMARK";
            }

            // Get the last .bss section strings, if they exist
            strs = pex.GetLastSectionStrings(".bss");
            if (strs != null)
            {
                if (strs.Exists(s => s.Contains("TMSAMVOF")))
                    return "ActiveMARK";
            }

            return null;
        }
    }
}
