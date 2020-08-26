﻿//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics;
using System.Reflection;

namespace Ntreev.ModernUI.Framework
{
    public class AppInfo
    {
        public static string ProductName
        {
            get
            {
                var assembly = Assembly.GetEntryAssembly();
                var info = FileVersionInfo.GetVersionInfo(assembly.Location);
                return info.ProductName;
            }
        }

        public static string CompanyName
        {
            get
            {
                var assembly = Assembly.GetEntryAssembly();
                var info = FileVersionInfo.GetVersionInfo(assembly.Location);
                return info.CompanyName;
            }
        }

        public static string ProductVersion
        {
            get
            {
                var assembly = Assembly.GetEntryAssembly();
                var info = FileVersionInfo.GetVersionInfo(assembly.Location);
                return info.ProductVersion;
            }
        }

        public static string StartupPath => System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public static string GetUserAppDataPath()
        {
            var assembly = Assembly.GetEntryAssembly();
            var at = typeof(AssemblyCompanyAttribute);
            var r = assembly.GetCustomAttributes(at, false);
            var ct = (AssemblyCompanyAttribute)(r[0]);
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path += @"\" + ct.Company;
            path += @"\" + assembly.GetName().Name.ToString();

            return path;
        }
    }
}
