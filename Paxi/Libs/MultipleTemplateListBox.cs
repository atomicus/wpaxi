using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Paxi.Libs
{
    public class MultiTemplateListBox : ListBox
    {
        public static readonly DependencyProperty TemplateSelectorProperty =
            DependencyProperty.Register("TemplateSelector",
            typeof(DataTemplateSelector), typeof(MultiTemplateListBox),
            new PropertyMetadata(new PropertyChangedCallback(OnTemplateChanged)));

        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)this.GetValue(TemplateSelectorProperty); }
            set { this.SetValue(TemplateSelectorProperty, value); }
        }

        protected override void PrepareContainerForItemOverride(
            DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            ListBoxItem listBoxItem = element as ListBoxItem;

            if (listBoxItem != null)
            {
                listBoxItem.ContentTemplate = this.ItemTemplateSelector.SelectTemplate(
                    item, this);
            }
        }

        private static void OnTemplateChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
        }
    }

    public class DataTemplateSelector
    {
        public virtual DataTemplate SelectTemplate(object item,
            DependencyObject container)
        {
            return null;
        }
    }
}
