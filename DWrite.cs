/*
Copyright (C) 2017  Prism Framework Team

This file is part of the Prism Framework.

The Prism Framework is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

The Prism Framework is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/


using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Prism.Windows
{
    [Guid("b859ee5a-d838-4b5b-a2e8-1adc7d93db48"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IDWriteFactory
    {
        void GetSystemFontCollection(out IDWriteFontCollection fontCollection, bool checkForUpdates);
    }

    [Guid("a84cee02-3eea-4eee-a827-87c1a02a0fcc"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IDWriteFontCollection
    {
        uint GetFontFamilyCount();

        int GetFontFamily(uint index, out IDWriteFontFamily fontFamily);
    }

    [Guid("da20d8ef-812a-4c43-9802-62ec4abd7add"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IDWriteFontFamily
    {
        void GetFontCollection(out IDWriteFontCollection fontCollection);

        uint GetFontCount();

        void GetFont(uint index, out object font);

        void GetFamilyNames(out IDWriteLocalizedStrings names);
    }

    [Guid("08256209-099a-4b34-b86d-c22b110e7771"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IDWriteLocalizedStrings
    {
        uint GetCount();

        void FindLocaleName(string localeName, out uint index, out bool exists);

        void GetLocaleNameLength(uint index, out uint length);

        void GetLocaleName(uint index, StringBuilder name, uint size);

        void GetStringLength(uint index, out uint length);

        void GetString(uint index, StringBuilder stringBuffer, uint size);
    }

    static class DWriteFactory
    {
        public static string[] GetFontNames()
        {
            Guid guid = new Guid("b859ee5a-d838-4b5b-a2e8-1adc7d93db48");
            IDWriteFactory factory;
            int result = CreateFactory(0, ref guid, out factory);
            if (result >= 0)
            {
                IDWriteFontCollection fontCollection;
                factory.GetSystemFontCollection(out fontCollection, true);

                var name = new StringBuilder(85);

                result = 0;
                string defaultLocaleName = name.ToString();

                List<string> fonts = new List<string>();
                uint index = 0;
                do
                {
                    IDWriteFontFamily family;
                    try { fontCollection.GetFontFamily(index++, out family); }
                    catch { break; }

                    IDWriteLocalizedStrings names;
                    family.GetFamilyNames(out names);

                    uint nameIndex = 0;
                    bool exists = false;
                    if (result != 0)
                    {
                        names.FindLocaleName(defaultLocaleName, out nameIndex, out exists);
                    }
                    if (!exists)
                    {
                        names.FindLocaleName("en-us", out nameIndex, out exists);
                    }

                    if (!exists)
                    {
                        nameIndex = 0;
                    }

                    uint length;
                    names.GetStringLength(nameIndex, out length);

                    name.Clear();
                    name.Capacity = (int)length;
                    names.GetString(nameIndex, name, length + 1);
                    fonts.Add(name.ToString());

                } while (true);

                return fonts.ToArray();
            }

            return new string[0];
        }

        [DllImport("dwrite.dll", SetLastError = true, EntryPoint = "DWriteCreateFactory")]
        static extern int CreateFactory(int factoryType, ref Guid iid, out IDWriteFactory factory);
    }
}
