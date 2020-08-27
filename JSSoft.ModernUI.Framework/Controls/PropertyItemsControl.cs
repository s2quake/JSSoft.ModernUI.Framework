// Released under the MIT License.
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

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace JSSoft.ModernUI.Framework.Controls
{
    /// <summary>
    /// PropertyItemsControl.xaml�� ���� ��ȣ �ۿ� ��
    /// </summary>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(HeaderedContentControl))]
    public class PropertyItemsControl : ItemsControl
    {
        private const string Header = nameof(Header);
        private const string Target = nameof(Target);

        public static readonly DependencyProperty HeaderWidthProperty =
            DependencyProperty.Register(nameof(HeaderWidth), typeof(GridLength), typeof(PropertyItemsControl),
                new FrameworkPropertyMetadata(GridLength.Auto, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty HeaderMinWidthProperty =
            DependencyProperty.Register(nameof(HeaderMinWidth), typeof(double), typeof(PropertyItemsControl),
                new FrameworkPropertyMetadata(20.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        private static readonly DependencyPropertyKey HeaderActualWidthPropertyKey =
           DependencyProperty.RegisterReadOnly(nameof(HeaderActualWidth), typeof(double), typeof(PropertyItemsControl),
               new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.RegisterAttached(Header, typeof(object), typeof(PropertyItemsControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, HeaderPropertyChangedCallback));

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.RegisterAttached(Target, typeof(UIElement), typeof(PropertyItemsControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(PropertyItemsControl),
                new PropertyMetadata(Orientation.Horizontal));

        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(nameof(ContentTemplate), typeof(DataTemplate), typeof(PropertyItemsControl));

        public static readonly DependencyProperty ContentTemplateSelectorProperty =
            DependencyProperty.Register(nameof(ContentTemplateSelector), typeof(DataTemplateSelector), typeof(PropertyItemsControl));

        public PropertyItemsControl()
        {

        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }

        public static object GetHeader(DependencyObject obj)
        {
            return (object)obj.GetValue(HeaderProperty);
        }

        public static void SetHeader(DependencyObject obj, object value)
        {
            obj.SetValue(HeaderProperty, value);
        }

        public static UIElement GetTarget(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(TargetProperty);
        }

        public static void SetTarget(DependencyObject obj, UIElement value)
        {
            obj.SetValue(TargetProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public double HeaderActualWidth => (double)this.GetValue(HeaderActualWidthPropertyKey.DependencyProperty);

        [TypeConverter(typeof(GridLengthConverter))]
        public GridLength HeaderWidth
        {
            get => (GridLength)this.GetValue(HeaderWidthProperty);
            set => this.SetValue(HeaderWidthProperty, value);
        }

        [TypeConverter(typeof(LengthConverter))]
        public double HeaderMinWidth
        {
            get => (double)this.GetValue(HeaderMinWidthProperty);
            set => this.SetValue(HeaderMinWidthProperty, value);
        }

        public Orientation Orientation
        {
            get => (Orientation)this.GetValue(OrientationProperty);
            set => this.SetValue(OrientationProperty, value);
        }

        public DataTemplate ContentTemplate
        {
            get => (DataTemplate)this.GetValue(ContentTemplateProperty);
            set => this.SetValue(ContentTemplateProperty, value);
        }

        public DataTemplateSelector ContentTemplateSelector
        {
            get => (DataTemplateSelector)this.GetValue(ContentTemplateSelectorProperty);
            set => this.SetValue(ContentTemplateSelectorProperty, value);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is HeaderedContentControl;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new HeaderedContentControl();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if (item is DependencyObject d)
            {
                var header = GetHeader(d);
                SetHeader(element, header);
                if (element is HeaderedContentControl control)
                {
                    control.Header = header;
                }
                var target = GetTarget(d);
                SetTarget(element, target);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if (this.HeaderWidth.IsAuto == true)
            {
                var width = this.HeaderMinWidth;
                for (var i = 0; i < this.Items.Count; i++)
                {
                    if (this.ItemContainerGenerator.ContainerFromIndex(i) is DependencyObject d)
                    {
                        var headerPresenter = this.FindHeaderPresenter(d);
                        if (headerPresenter != null && headerPresenter.Content != null)
                        {
                            headerPresenter.Measure(constraint);
                            width = Math.Max(width, headerPresenter.DesiredSize.Width);
                        }
                    }
                }
                this.SetValue(HeaderActualWidthPropertyKey, width);
            }
            else if (this.HeaderWidth.IsStar == true)
            {
                var width = Math.Max(this.HeaderMinWidth, constraint.Width * this.HeaderWidth.Value);
                this.SetValue(HeaderActualWidthPropertyKey, Math.Max(this.HeaderMinWidth, width));
            }
            else
            {
                this.SetValue(HeaderActualWidthPropertyKey, Math.Max(this.HeaderMinWidth, this.HeaderWidth.Value));
            }

            return base.MeasureOverride(constraint);
        }

        private static void HeaderPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement fe && fe.Parent is FrameworkElement parent)
            {
                parent.InvalidateMeasure();
            }
        }

        private ContentPresenter FindHeaderPresenter(DependencyObject d)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var child = VisualTreeHelper.GetChild(d, i);
                if (child is ContentPresenter contentPresenter && contentPresenter.ContentSource == nameof(HeaderedContentControl.Header))
                    return contentPresenter;
                var result = this.FindHeaderPresenter(child);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}
