using Godot;

public static class LoggingUtils
{
    public static void Info(string msg, bool isBold=false, bool isUnderlined=false, bool isItalicized=false, int fontSize=10)
    {
        var message = msg;
        if (isBold)
            message = $"[b]{message}[/b]";
        if (isUnderlined)
            message = $"[u]{message}[/u]";
        if (isItalicized)
            message = $"[i]{message}[/i]";
        if (fontSize > 10)
            message = $"[font_size={fontSize}]{message}[/font_size]";
        GD.PrintRich($"[color=green][INFO] {message}[/color]");
    }

    public static void Error(string err)
    {
        GD.PrintErr($"[ERROR] {err}");
    }

    public static void Debug(string msg, bool isBold=false, bool isUnderlined=false, bool isItalicized=false, int fontSize=10)
    {
        var message = msg;
        if (isBold)
            message = $"[b]{message}[/b]";
        if (isUnderlined)
            message = $"[u]{message}[/u]";
        if (isItalicized)
            message = $"[i]{message}[/i]";
        if (fontSize > 10)
            message = $"[font_size={fontSize}]{message}[/font_size]";
        GD.PrintRich($"[color=yellow][DEBUG] {message}[/color]");
    }
}