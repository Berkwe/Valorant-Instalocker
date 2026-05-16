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

        private TranslateTransform _translate;
        private Storyboard _marqueeBoard;
        private Storyboard _fadeBoard;
        private bool _isFirstLoad = true;

        public MarqueeTextBlock()
        {
            InitializeComponent();
            _translate = new TranslateTransform(0, 0);
            PART_Text.RenderTransform = _translate;
            Loaded += (s, e) => StartMarquee();
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (MarqueeTextBlock)d;
            if (ctrl._isFirstLoad)
            {
                ctrl.PART_Text.Text = e.NewValue?.ToString() ?? "";
                ctrl._isFirstLoad = false;
                return;
            }
            ctrl.FadeThenChange(e.NewValue?.ToString() ?? "");
        }

        // Yazı değişince: fade out → yazıyı değiştir → fade in → marquee başlar
        private void FadeThenChange(string newText)
        {
            _marqueeBoard?.Stop();

            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(300));
            fadeOut.Completed += (s, e) =>
            {
                PART_Text.Text = newText;
                _translate.X = 0;

                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                fadeIn.Completed += (_, __) => StartMarquee();
                PART_Text.BeginAnimation(OpacityProperty, fadeIn);
            };
            PART_Text.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void StartMarquee()
        {
            _marqueeBoard?.Stop();
            _translate.X = 0;

            // Yazının gerçek genişliğini ölç
            PART_Text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double textWidth = PART_Text.DesiredSize.Width;
            double containerWidth = ActualWidth;

            // Yazı containerdan kısaysa marquee gerekmez
            if (textWidth <= containerWidth)
                return;

            double scrollDistance = textWidth + containerWidth;

            // Hız: piksel başına ~8ms, min 4sn max 12sn
            double duration = Math.Clamp(scrollDistance * 0.008, 4, 12);

            _marqueeBoard = new Storyboard();

            // 3sn bekle (başlangıç konumunda dur)
            var wait = new DoubleAnimation
            {
                From = 0,
                To = 0,
                BeginTime = TimeSpan.Zero,
                Duration = TimeSpan.FromSeconds(3)
            };

            // Sağdan sola kayma
            var scroll = new DoubleAnimation
            {
                From = 0,
                To = -scrollDistance,
                BeginTime = TimeSpan.FromSeconds(3),
                Duration = TimeSpan.FromSeconds(duration),
                EasingFunction = null
            };

            // Bitti → sıfırla ve tekrar (loop)
            scroll.Completed += (s, e) =>
            {
                _translate.X = 0;
                StartMarquee(); // recursive loop
            };

            Storyboard.SetTarget(wait, PART_Text);
            Storyboard.SetTargetProperty(wait, new PropertyPath("RenderTransform.X"));

            Storyboard.SetTarget(scroll, PART_Text);
            Storyboard.SetTargetProperty(scroll, new PropertyPath("RenderTransform.X"));

            _marqueeBoard.Children.Add(wait);
            _marqueeBoard.Children.Add(scroll);
            _marqueeBoard.Begin();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            StartMarquee(); // container boyutu değişirse yeniden hesapla
        }
    }
}