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

using Ntreev.Library;
using System;

namespace Ntreev.ModernUI.Framework
{
    public interface IAppConfiguration
    {
        void Commit(object target);

        bool Contains(string name);

        object this[string name] { get; set; }

        void Update(object target);
    }

    public static class IAppConfigurationExtensions
    {
        public static bool TryGetValue<T>(this IAppConfiguration config, Type section, Type type, string key, out T value)
        {
            var configItem = $"{section.Name}.{type.FullName}.{key}";
            if (config.Contains(configItem) == true)
            {
                try
                {
                    if (ConfigurationBase.CanSupportType(typeof(T)) == true)
                    {
                        value = (T)ConfigurationBase.ConvertFromConfig(config[configItem], typeof(T));
                        return true;
                    }
                    else
                    {
                        throw new NotSupportedException($"{typeof(T).Name} does not supported.");
                    }
                }
                catch
                {

                }
            }

            value = default;
            return false;
        }

        public static void SetValue<T>(this IAppConfiguration config, Type section, Type type, string key, T value)
        {
            var configItem = $"{section.Name}.{type.FullName}.{key}";
            if (ConfigurationBase.CanSupportType(typeof(T)) == true)
            {
                config[configItem] = value;
            }
            else
            {
                throw new NotSupportedException($"{typeof(T).Name} does not supported.");
            }
        }
    }
}
