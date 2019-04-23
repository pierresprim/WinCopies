using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace WinCopies.GUI
{
    public class DragBehavior : Behavior<ListView>
    {
        private Point elementStartPosition;
        private Point mouseStartPosition;
        private TranslateTransform transform = new TranslateTransform();

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(IList<object>), typeof(DragBehavior));

        public IList<object> SelectedItems { get => (IList<object>)GetValue(SelectedItemsProperty); set => SetValue(SelectedItemsProperty, value); }

        protected override void OnAttached()
        {
            try
            {
                Window parent = Application.Current.MainWindow;
                AssociatedObject.RenderTransform = transform;

                AssociatedObject.MouseLeftButtonDown += (sender, e) =>
                {
                    elementStartPosition = AssociatedObject.TranslatePoint(new Point(), parent);
                    mouseStartPosition = e.GetPosition(parent);
                    AssociatedObject.CaptureMouse();
                };

                AssociatedObject.MouseLeftButtonUp += (sender, e) =>
                {
                    AssociatedObject.ReleaseMouseCapture();
                };

                AssociatedObject.MouseMove += (sender, e) =>
                {
                    Vector diff = e.GetPosition(parent) - mouseStartPosition;
                    if (AssociatedObject.IsMouseCaptured)
                    {
                        transform.X = diff.X;
                        transform.Y = diff.Y;
                    }
                };
            }
            catch (Exception) { }
        }
    }
}
