namespace Eastwing.Tools.Parser
{
    public enum Category : byte
    {
        /// <summary>
        /// Используется утилитарно. Токен этой категории не может быть получен в ходе разбора строки
        /// </summary>
        Void,
        /// <summary>
        /// Строка - текст, заключённы в кавычки или апострофы
        /// </summary>
        String,
        /// <summary>
        /// Целое число
        /// </summary>
        Integer,
        /// <summary>
        /// Вещественное число
        /// </summary>
        Real,
        /// <summary>
        /// Слово, не указанное ключевым
        /// </summary>
        Word,
        /// <summary>
        /// Слово из списка ключевых слов
        /// </summary>
        Keyword,
        /// <summary>
        /// Кавычка или апостроф
        /// </summary>
        Quote,
        Plus,
        Minus,
        /// <summary>
        /// Т.н. "звёздочка"
        /// </summary>
        Asterisk,
        Equals,
        Slash,
        Backslash,
        /// <summary>
        /// Различные символы, не вошедшие в прочие категории
        /// </summary>
        Separator,
        /// <summary>
        /// Скобка
        /// </summary>
        Bracket,
        /// <summary>
        /// Пробел или табуляция
        /// </summary>
        Space,
        /// <summary>
        /// Неизвестный токен
        /// </summary>
        Unknown
    }
}
