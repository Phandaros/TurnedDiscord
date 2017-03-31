using Discord;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace UnturnedBot.Discord.Utils
{
    class TokenUtils
    {
        internal const char PREFIX = '!';
        internal static Dictionary<string, Token> Tokens = new Dictionary<string, Token> { { "ExampleToken", new Token("YourTokenHere", TokenType.User) } };

        public static void SetDefaults()
        {
            var filePath = "tokens.json";

            if (!File.Exists(filePath))
            {
                Logger.Log("[Tokens] Please set up your tokens in " + Path.Combine(Directory.GetCurrentDirectory(), filePath));
                using (FileStream fs = File.Create(filePath))
                {
                    var json = JsonConvert.SerializeObject(Tokens);
                    byte[] text = new System.Text.UTF8Encoding(true).GetBytes(json);
                    fs.Write(text, 0, text.Length);
                }
            }
            else
            {
                Tokens = JsonConvert.DeserializeObject<Dictionary<string, Token>>(File.ReadAllText(filePath));
                Logger.Log("[Tokens] Loaded tokens: " + string.Join(", ", Tokens.Keys));
            }
        }
    }
    class Token
    {
        public string token;
        public TokenType tokenType;
        public Token(string token, TokenType tokenType)
        {
            this.token = token;
            this.tokenType = tokenType;
        }
    }
}
