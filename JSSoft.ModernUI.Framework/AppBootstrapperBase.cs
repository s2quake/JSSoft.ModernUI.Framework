﻿// Released under the MIT License.
// 
// Copyright (c) 2018 Ntreev Soft co., Ltd.
// Copyright (c) 2020 Jeesu Choi
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
// Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Forked from https://github.com/NtreevSoft/Ntreev.ModernUI.Framework
// Namespaces and files starting with "Ntreev" have been renamed to "JSSoft".

using Caliburn.Micro;
using JSSoft.ModernUI.Framework.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace JSSoft.ModernUI.Framework
{
    public abstract class AppBootstrapperBase : BootstrapperBase, IServiceProvider, IBuildUp
    {
        private readonly Func<object, object> locateForView = ViewModelLocator.LocateForView;
        private readonly Func<Type, DependencyObject, object, Type> locateTypeForModelType = ViewLocator.LocateTypeForModelType;
        private readonly Func<object, DependencyObject, object, UIElement> locateForModel = ViewLocator.LocateForModel;

        static AppBootstrapperBase()
        {
            ConventionManager.AddElementConvention<ThicknessControl>(ThicknessControl.ValueProperty, nameof(ThicknessControl.Value), nameof(ThicknessControl.ValueChanged));
            ConventionManager.AddElementConvention<ColorPicker>(ColorPicker.ValueProperty, nameof(ColorPicker.Value), nameof(ColorPicker.ValueChanged));
            ConventionManager.AddElementConvention<GuidControl>(GuidControl.ValueProperty, nameof(GuidControl.Value), nameof(GuidControl.ValueChanged));
            ConventionManager.AddElementConvention<ColorButton>(ColorButton.ValueProperty, nameof(ColorButton.Value), nameof(ColorButton.ValueChanged));
        }

        protected AppBootstrapperBase(AppBootstrapperDescriptorBase descriptor)
        {
            this.Descriptor = descriptor;

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()) == false)
            {
                if (Current != null)
                    throw new InvalidOperationException("AppBootstrapper does not allow multi instance");
                Current = this;
            }
            if (this.AutoInitialize == true)
                this.Initialize();
        }

        public static AppBootstrapperBase Current { get; private set; }

        public static IAppConfiguration Configs => AppConfiguration.Current;

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IServiceProvider))
                return this;

            if (typeof(IEnumerable).IsAssignableFrom(serviceType) && serviceType.GenericTypeArguments.Length == 1)
            {
                var itemType = serviceType.GenericTypeArguments.First();
                var items = this.Descriptor.Instances(itemType);
                var listGenericType = typeof(List<>);
                var list = listGenericType.MakeGenericType(itemType);
                var ci = list.GetConstructor(new Type[] { typeof(int) });
                var instance = ci.Invoke(new object[] { items.Count(), }) as IList;
                foreach (var item in items)
                {
                    instance.Add(item);
                }
                return instance;
            }
            else
            {
                return this.Descriptor.Instance(serviceType, null);
            }
        }

        protected override void Configure()
        {
            ViewModelLocator.LocateForView = this.LocateForView;
            ViewLocator.LocateTypeForModelType = this.LocateTypeForModelType;
            ViewLocator.LocateForModel = this.LocateForModel;
            this.Descriptor.Initialize();
        }

        protected override object GetInstance(Type service, string key)
        {
            return this.Descriptor.Instance(service, key);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return this.Descriptor.SelectAssemblies();
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return this.Descriptor.Instances(service);
        }

        protected override void BuildUp(object instance)
        {
            this.Descriptor.BuildUp(instance);
        }

        protected override async void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            base.OnStartup(sender, e);
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
                new FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag)));

            if (this.IgnoreDisplay == false)
                await this.DisplayRootViewForAsync(this.Descriptor.ModelType).ConfigureAwait(false);
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            AppConfiguration.Current.Write();
            base.OnExit(sender, e);
            this.Descriptor.Dispose();
        }

        protected override void PrepareApplication()
        {
            base.PrepareApplication();
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            base.OnUnhandledException(sender, e);
        }

        protected virtual bool IgnoreDisplay => false;

        protected virtual bool AutoInitialize => true;

        protected AppBootstrapperDescriptorBase Descriptor { get; private set; }

        private object LocateForView(object view)
        {
            object viewModel = this.locateForView(view);
            if (viewModel != null)
                return viewModel;

            if (TypeDescriptor.GetAttributes(view)[typeof(ModelAttribute)] is ModelAttribute attr)
            {
                if (attr.ModelType.IsInterface == true)
                    return this.GetInstance(attr.ModelType, null);
                return TypeDescriptor.CreateInstance(null, attr.ModelType, null, null);
            }

            return viewModel;
        }

        private Type LocateTypeForModelType(Type vmType, DependencyObject location, object context)
        {
            var attribute = vmType.GetCustomAttributes(typeof(ViewAttribute), true).OfType<ViewAttribute>().FirstOrDefault();
            if (attribute != null)
            {
                return Type.GetType(attribute.ViewTypeName);
            }

            var viewType = this.locateTypeForModelType(vmType, location, context);

            if (viewType == null)
            {
                Type baseType = vmType.BaseType;
                if (baseType.Name.EndsWith("ViewModel") == true)
                {
                    return ViewLocator.LocateTypeForModelType(baseType, location, context);
                }
            }

            return viewType;
        }

        private UIElement LocateForModel(object model, DependencyObject displayLocation, object context)
        {
            var view = this.locateForModel(model, displayLocation, context);
            //throw new NotImplementedException();
            //if (view != null && view.GetType().GetCustomAttributes<ExportAttribute>(true).Any() == false)
            //{
            //    this.container.SatisfyImportsOnce(view);
            //}
            return view;
        }

        #region IBuildUp

        void IBuildUp.BuildUp(object instance)
        {
            this.BuildUp(instance);
        }

        #endregion
    }
}
