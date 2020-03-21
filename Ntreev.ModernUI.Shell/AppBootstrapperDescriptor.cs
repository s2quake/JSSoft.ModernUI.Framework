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

using Caliburn.Micro;
using Ntreev.Library.Linq;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;

namespace Ntreev.ModernUI.Shell
{
    public class AppBootstrapperDescriptor : AppBootstrapperDescriptorBase
    {
        private CompositionContainer container;

        public override Type ModelType => typeof(IShell);


        //public object GetService(Type serviceType)
        //{
        //    if (serviceType == typeof(IServiceProvider))
        //        return this;

        //    if (typeof(IEnumerable).IsAssignableFrom(serviceType) && serviceType.GenericTypeArguments.Length == 1)
        //    {
        //        var itemType = serviceType.GenericTypeArguments.First();
        //        var contractName = AttributedModelServices.GetContractName(itemType);
        //        var items = this.container.GetExportedValues<object>(contractName);
        //        var listGenericType = typeof(List<>);
        //        var list = listGenericType.MakeGenericType(itemType);
        //        var ci = list.GetConstructor(new Type[] { typeof(int) });
        //        var instance = ci.Invoke(new object[] { items.Count(), }) as IList;
        //        foreach (var item in items)
        //        {
        //            instance.Add(item);
        //        }
        //        return instance;
        //    }
        //    else
        //    {
        //        var contractName = AttributedModelServices.GetContractName(serviceType);
        //        return this.container.GetExportedValue<object>(contractName);
        //    }
        //}

        protected override void OnInitialize(IEnumerable<Assembly> assemblies, IEnumerable<Tuple<Type, object>> parts)
        {
            var catalog = new AggregateCatalog();

            foreach (var item in AssemblySource.Instance)
            {
                catalog.Catalogs.Add(new AssemblyCatalog(item));
            }

            var container = new CompositionContainer(catalog);
            var batch = new CompositionBatch();

            //batch.AddExportedValue<IWindowManager>(AppWindowManager.Current);
            //batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            //batch.AddExportedValue<IAppConfiguration>(AppConfiguration.Current);
            //batch.AddExportedValue<IServiceProvider>(this);
            //batch.AddExportedValue<IBuildUp>(this);
            batch.AddExportedValue<ICompositionService>(container);

            foreach (var item in parts)
            {
                var contractName = AttributedModelServices.GetContractName(item.Item1);
                var typeIdentity = AttributedModelServices.GetTypeIdentity(item.Item1);
                batch.AddExport(new Export(contractName, new Dictionary<string, object>
                {
                    {
                        "ExportTypeIdentity",
                        typeIdentity
                    }
                }, () => item.Item2));
            }
            container.Compose(batch);


            this.container = container;

        }

        public override object GetInstance(Type service, string key)
        {
            var contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(service) : key;
            var exports = this.container.GetExportedValues<object>(contract);

            if (exports.Count() > 0)
                return exports.First();

            throw new InvalidOperationException("Could not locate any instances.");
        }

        protected override IEnumerable<Assembly> GetAssemblies()
        {
            var assemblyList = new List<Assembly>(base.GetAssemblies())
            {
                Assembly.GetExecutingAssembly()
            };

            var assembliesByName = new Dictionary<string, Assembly>();
            foreach (var item in assemblyList)
            {
                assembliesByName.Add(item.FullName, item);
            }

            if (Execute.InDesignMode == false)
            {
                var query = from directory in EnumerableUtility.Friends(AppDomain.CurrentDomain.BaseDirectory, this.SelectPath())
                            let catalog = new DirectoryCatalog(directory)
                            from file in catalog.LoadedFiles
                            select file;

                foreach (var item in query.Distinct())
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(item);
                        if (assembliesByName.ContainsKey(assembly.FullName) == false)
                        {
                            assembliesByName.Add(assembly.FullName, assembly);
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return assembliesByName.Values.ToArray();
        }

        public override IEnumerable<object> GetInstances(Type service)
        {
            return this.container.GetExportedValues<object>(AttributedModelServices.GetContractName(service));
        }

        public override void BuildUp(object instance)
        {
            this.container.SatisfyImportsOnce(instance);
        }

        protected override void OnDispose()
        {
            this.container.Dispose();
        }


        protected virtual IEnumerable<string> SelectPath()
        {
            yield break;
        }

        //protected override IEnumerable<Tuple<Type, object>> GetParts()
        //{
        //    foreach (var item in base.GetParts())
        //    {
        //        yield return item;
        //    }
        //    yield return new Tuple<Type, object>(typeof(ICompositionService), this.container);
        //}

        //protected override void Exit(object sender, EventArgs e)
        //{
        //    AppConfiguration.Current.Write();
        //    base.OnExit(sender, e);
        //    this.container.Dispose();
        //}

        //protected override void PrepareApplication()
        //{
        //    base.PrepareApplication();
        //}

        //protected IEnumerable<(System.Type, object)> GetParts()
        //{
        //    yield break;
        //}
    }
}
