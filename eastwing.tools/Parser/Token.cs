namespace Eastwing.Tools.Parser
{
    public struct Token
    {
        public string Lexeme;
        public Category Category;

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

        public Token(string Lexeme, Category Category)
        {
            this.Lexeme = Lexeme;
            this.Category = Category;
        }

        /// <summary>
        /// Присваивает существующему экземпляру новые значения без необходимости создавать новый экхемпляр
        /// </summary>
        /// <param name="Lexeme"></param>
        /// <param name="Category"></param>
        /// <returns></returns>
        public Token Reinit(string Lexeme, Category Category)
        {
            this.Lexeme = Lexeme;
            this.Category = Category;

            return this;
        }
    }
}
