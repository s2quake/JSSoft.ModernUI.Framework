//Released under the MIT License.
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

using System.Reflection;
using System.Windows.Controls.Primitives;
using Xceed.Wpf.DataGrid.Views;

namespace JSSoft.ModernUI.Framework.DataGrid.Controls
{
    /// <summary>
    /// ���� �ǵ�
    /// ���� Ŭ�������� HorizontalOffset ���� �Ҽ������� ó���ϱ� ������ ���� ��ũ�ѽ� �Ϻ� ���� �ȼ� ���� ������ �߰ߵ�
    /// �Ҽ��� ���� ���������� �ٲپ� ���� �������̽��� ȣ���Ͽ� ���� ������ ����
    /// </summary>
    public class ModernTableViewItemsHost : TableViewItemsHost, IScrollInfo
    {
        private readonly MethodInfo baseSetHorizontalOffset;

        public ModernTableViewItemsHost()
        {
            var methods = typeof(TableViewItemsHost).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var item in methods)
            {
                if (item.IsFinal && item.IsPrivate)
                {
                    if (item.Name.EndsWith("SetHorizontalOffset") == true)
                    {
                        this.baseSetHorizontalOffset = item;
                    }
                }
            }
        }

        #region IScrollInfo

        void IScrollInfo.SetHorizontalOffset(double offset)
        {
            if (double.IsInfinity(offset) == true)
                this.baseSetHorizontalOffset.Invoke(this, new object[] { offset });
            else
                this.baseSetHorizontalOffset.Invoke(this, new object[] { (double)(int)offset });
        }

        #endregion
    }
}
