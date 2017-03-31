using System;

namespace UnturnedBot.Unturned
{
    static class Logger
    {
        public static void Log(string message, ConsoleColor bracketsColor = ConsoleColor.DarkGreen, ConsoleColor operatorColor = ConsoleColor.Cyan)
        {
            for (int i = 0; i < message.Length; i++)
            {
                char c = message[i];
                if (c == '[')
                {
                    int endIndex = message.IndexOf(']', i);
                    if (endIndex != -1)
                    {
                        var lastColor = Console.ForegroundColor;
                        Console.ForegroundColor = bracketsColor;
                        var subString = message.Substring(i, endIndex - i + 1);
                        Console.Write(subString);
                        Console.ForegroundColor = lastColor;
                        i += subString.Length - 1;
                        continue;
                    }
                }
                else if (c == '<')
                {
                    int endIndex = message.IndexOf('>');
                    if (endIndex != -1)
                    { 
                        var lastColor = Console.ForegroundColor;
                        Console.ForegroundColor = operatorColor;
                        var subString = message.Substring(i, endIndex - i + 1);
                        Console.Write(subString);
                        Console.ForegroundColor = lastColor;
                        i += subString.Length - 1;
                        continue;
                    }
                }
                Console.Write(c);
            }
            Console.Write(Environment.NewLine);
        }
    }
}
