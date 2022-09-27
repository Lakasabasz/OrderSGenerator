using System.Windows;

namespace Generator_rozkazów_S.Extensions;

public class MessageBox
{
    public static void Error(string text, string title)
    {
        System.Windows.MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public static void Warning(string text, string title)
    {
        System.Windows.MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    public static void Critical(string text, string title)
    {
        System.Windows.MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Stop);
    }

    public static bool Question(string question, string title)
    {
        return System.Windows.MessageBox.Show(question, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }
}