using System.Text;

namespace Eastwing.Tools.Parser
{
    public class Token
    {
        public string Lexeme;
        public TokenCategories Category;

        /// <summary>
        /// Вывод лексемы
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Lexeme;

        /// <summary>
        /// Вывод обоих полей в формате "(Категория) Лексема"
        /// </summary>
        /// <returns></returns>
        public string Full => $"({Category}) {Lexeme}";

        public Token(string Lexeme, TokenCategories Category)
        {
            this.Lexeme = Lexeme;
            this.Category = Category;
        }

        public Token(char Source, TokenCategories Category)
        {
            this.Lexeme = Source.ToString();
            this.Category = Category;
        }

        public Token(StringBuilder Builder, TokenCategories Category)
        {
            this.Lexeme = Builder.ToString();
            this.Category = Category;
        }

        /// <summary>
        /// Присваивает существующему экземпляру новые значения без необходимости создавать новый экхемпляр
        /// </summary>
        /// <param name="Lexeme"></param>
        /// <param name="Category"></param>
        /// <returns></returns>
        public Token Reinit(string Lexeme, TokenCategories Category)
        {
            this.Lexeme = Lexeme;
            this.Category = Category;

            return this;
        }
        public Token Reinit<T>(T Source, TokenCategories Category)
        {
            return Reinit(Source.ToString(), Category);
        }
    }
}
