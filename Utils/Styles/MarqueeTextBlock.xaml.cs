using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Valorant_Instalocker.Utils.Styles
{
    public partial class MarqueeTextBlock : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(MarqueeTextBlock),
                new PropertyMetadata(string.Empty, OnTextChanged));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private double _containerWidth;
        private bool _isFirstLoad = true;
        private TranslateTransform _translate;

        public MarqueeTextBlock()
        {
            InitializeComponent();
            _translate = new TranslateTransform(0, 0);
            PART_Stack.RenderTransform = _translate;
            Loaded += (s, e) => StartMarquee();
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (MarqueeTextBlock)d;
            string newText = e.NewValue?.ToString() ?? "";

            if (ctrl._isFirstLoad)
            {
                ctrl.PART_Text.Text = newText;
                ctrl.PART_TextCopy.Text = newText;
                ctrl._isFirstLoad = false;
                return;
            }
            ctrl.FadeThenChange(newText);
        }

        private void FadeThenChange(string newText)
        {
            _translate.BeginAnimation(TranslateTransform.XProperty, null);

            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            fadeOut.Completed += (s, e) =>
            {
                PART_Text.Text = newText;
                PART_TextCopy.Text = newText;
                _translate.X = 0;

                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                fadeIn.Completed += (_, __) => StartMarquee(enterFromRight: false);
                PART_Stack.BeginAnimation(OpacityProperty, fadeIn);
            };
            PART_Stack.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void StartMarquee(bool enterFromRight = false)
        {
            _translate.BeginAnimation(TranslateTransform.XProperty, null);

            PART_Text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double textWidth = PART_Text.DesiredSize.Width;
            double containerWidth = ActualWidth;

            if (textWidth == 0 || containerWidth == 0) return;

            if (textWidth <= containerWidth)
            {
                PART_TextCopy.Visibility = Visibility.Collapsed;
                _translate.X = 0;
                return;
            }

            PART_Stack.Margin = new Thickness(0, 0, -(textWidth * 2), 0);
            PART_TextCopy.Visibility = Visibility.Visible;

            double gap = containerWidth;
            PART_TextCopy.Margin = new Thickness(gap, 0, 0, 0);

            double loopWidth = textWidth + gap;
            double durationSeconds = Math.Max(1, loopWidth * 0.008);

            _translate.X = 0;

            var scroll = new DoubleAnimation
            {
                From = 0,
                To = -loopWidth,
                Duration = TimeSpan.FromSeconds(durationSeconds),
                RepeatBehavior = RepeatBehavior.Forever,
                FillBehavior = FillBehavior.Stop
            };

            _translate.BeginAnimation(TranslateTransform.XProperty, scroll);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (Math.Abs(sizeInfo.NewSize.Width - _containerWidth) > 1)
            {
                _containerWidth = ActualWidth;
                StartMarquee();
            }
        }
    }
}