using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace XNBStudioGUI;

public partial class StackTraceViewer : UserControl
{
    public static readonly DependencyProperty TitleTextProperty = DependencyProperty.Register(
        nameof(TitleText), typeof(string), typeof(StackTraceViewer),
        new UIPropertyMetadata("An exception occured!"));

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }
    
    public static readonly DependencyProperty ExceptionDataProperty = DependencyProperty.Register(
        nameof(ExceptionData), typeof(Exception), typeof(StackTraceViewer));

    public Exception? ExceptionData
    {
        get => (Exception)GetValue(ExceptionDataProperty);
        set => SetValue(ExceptionDataProperty, value);
    }

    public string ExceptionTypeName => ExceptionData?.GetType().FullName ?? "Unknown exception";
    public string ExceptionMessage => !string.IsNullOrEmpty(ExceptionData?.Message) ? ExceptionData.Message : "No message";
    public string ExceptionStackTrace => ExceptionData?.StackTrace ?? "No stack trace";
    
    public StackTraceViewer()
    {
        InitializeComponent();
        DataContext = this;
        
        if (DesignerProperties.GetIsInDesignMode(this))
        {
            ExceptionData = new ApplicationException("Test exception! This should only display in the designer");
        }
    }
}