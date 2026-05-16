using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Valorant_Instalocker.Main.Helpers
{

    public class CenteredWrapPanel : Panel
    {
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(CenteredWrapPanel),
                new FrameworkPropertyMetadata(95.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(CenteredWrapPanel),
                new FrameworkPropertyMetadata(95.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double ItemWidth
        {
            get => (double)GetValue(ItemWidthProperty);
            set => SetValue(ItemWidthProperty, value);
        }

        public double ItemHeight
        {
            get => (double)GetValue(ItemHeightProperty);
            set => SetValue(ItemHeightProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in InternalChildren)
                child.Measure(new Size(ItemWidth, ItemHeight));

            int cols = Math.Max(1, (int)(availableSize.Width / ItemWidth));
            int rows = (int)Math.Ceiling((double)InternalChildren.Count / cols);

            return new Size(cols * ItemWidth, rows * ItemHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;

            var rows = new List<List<UIElement>>();
            var currentRow = new List<UIElement>();
            double currentWidth = 0;

            foreach (UIElement child in InternalChildren)
            {
                if (currentWidth + itemWidth > finalSize.Width + 0.5 && currentRow.Count > 0)
                {
                    rows.Add(currentRow);
                    currentRow = new List<UIElement>();
                    currentWidth = 0;
                }
                currentRow.Add(child);
                currentWidth += itemWidth;
            }
            if (currentRow.Count > 0) rows.Add(currentRow);

            double y = 0;
            foreach (var row in rows)
            {
                double rowWidth = row.Count * itemWidth;
                double x = (finalSize.Width - rowWidth) / 2.0;
                foreach (var child in row)
                {
                    child.Arrange(new Rect(x, y, itemWidth, itemHeight));
                    x += itemWidth;
                }
                y += itemHeight;
            }

            return finalSize;
        }
    }


}
