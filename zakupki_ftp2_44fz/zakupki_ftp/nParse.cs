using System.Linq;
using System.Text;

namespace zakupki_ftp
{
    internal class nParse // 1.5
    {
        private static string ReplaceCharInString(string source, int index, char newSymb)
        {
            StringBuilder sb = new StringBuilder(source) {[index] = newSymb};
            return sb.ToString();
        }


        private static string ClearSentence(string sentence)
        {
            string result = sentence;
            result = result.Replace('-', ' ');
            result = result.Replace('/', ' ');
            result = result.Replace('.', ' ');
            result = result.Replace('(', ' ');
            result = result.Replace(')', ' ');
            result = result.Replace('»', ' ');
            result = result.Replace('«', ' ');
            result = result.Replace(',', ' ');
            result = result.Replace('\n', ' ');
            result = result.Replace('“', ' ');
            result = result.Replace('”', ' ');
            result = result.Replace('[', ' ');
            result = result.Replace(']', ' ');
            return result;
        }

        public string LetChange(string sentence)
        {
            string result = null;
            string cleanSentence = ClearSentence(sentence);
            foreach (string letter in cleanSentence.Split(' '))
            {
                string str = letter;
                switch (GetWordLang(letter))
                {
                    case "rus":
                        if (str.IndexOf('y') > -1) str = str.Replace("y", "☺y☻");
                        if (str.IndexOf('Y') > -1) str = str.Replace("Y", "☺Y☻");
                        if (str.IndexOf('k') > -1) str = str.Replace("k", "☺k☻");
                        if (str.IndexOf('K') > -1) str = str.Replace("K", "☺K☻");
                        if (str.IndexOf('e') > -1) str = str.Replace("e", "☺e☻");
                        if (str.IndexOf('E') > -1) str = str.Replace("E", "☺E☻");
                        if (str.IndexOf('H') > -1) str = str.Replace("H", "☺H☻");
                        if (str.IndexOf('x') > -1) str = str.Replace("x", "☺x☻");
                        if (str.IndexOf('B') > -1) str = str.Replace("B", "☺B☻");
                        if (str.IndexOf('a') > -1) str = str.Replace("a", "☺a☻");
                        if (str.IndexOf('A') > -1) str = str.Replace("A", "☺A☻");
                        if (str.IndexOf('p') > -1) str = str.Replace("p", "☺p☻");
                        if (str.IndexOf('P') > -1) str = str.Replace("P", "☺P☻");
                        if (str.IndexOf('o') > -1) str = str.Replace("o", "☺o☻");
                        if (str.IndexOf('O') > -1) str = str.Replace("O", "☺O☻");
                        if (str.IndexOf('c') > -1) str = str.Replace("c", "☺c☻");
                        if (str.IndexOf('C') > -1) str = str.Replace("C", "☺C☻");
                        if (str.IndexOf('m') > -1) str = str.Replace("m", "☺m☻");
                        if (str.IndexOf('M') > -1) str = str.Replace("M", "☺M☻");
                        if (str.IndexOf('T') > -1) str = str.Replace("T", "☺T☻"); 
                        result += str + " ";
                        break;
                    case "eng":
                        if (str.IndexOf('у') > -1) str = str.Replace("у", "☺у☻");
                        if (str.IndexOf('У') > -1) str = str.Replace("У", "☺У☻");
                        if (str.IndexOf('к') > -1) str = str.Replace("к", "☺к☻");
                        if (str.IndexOf('К') > -1) str = str.Replace("К", "☺К☻");
                        if (str.IndexOf('е') > -1) str = str.Replace("е", "☺е☻");
                        if (str.IndexOf('Е') > -1) str = str.Replace("Е", "☺Е☻");
                        if (str.IndexOf('Н') > -1) str = str.Replace("Н", "☺Н☻");
                        if (str.IndexOf('х') > -1) str = str.Replace("х", "☺х☻");
                        if (str.IndexOf('В') > -1) str = str.Replace("В", "☺В☻");
                        if (str.IndexOf('а') > -1) str = str.Replace("а", "☺а☻");
                        if (str.IndexOf('А') > -1) str = str.Replace("А", "☺А☻");
                        if (str.IndexOf('р') > -1) str = str.Replace("р", "☺р☻");
                        if (str.IndexOf('Р') > -1) str = str.Replace("Р", "☺Р☻");
                        if (str.IndexOf('о') > -1) str = str.Replace("о", "☺о☻");
                        if (str.IndexOf('О') > -1) str = str.Replace("О", "☺О☻");
                        if (str.IndexOf('с') > -1) str = str.Replace("с", "☺с☻");
                        if (str.IndexOf('С') > -1) str = str.Replace("С", "☺С☻");
                        if (str.IndexOf('м') > -1) str = str.Replace("м", "☺м☻");
                        if (str.IndexOf('М') > -1) str = str.Replace("М", "☺М☻");
                        if (str.IndexOf('Т') > -1) str = str.Replace("Т", "☺Т☻"); 
                        result += str + " ";
                        break;
                    default:
                        result += str + " ";
                        break;
                }
            }
            result = result.Remove(result.Length - 1, 1);
            // возвращаем на место замененные символы
            for (int i = 0, k = 0; i < sentence.Length; i++)
            {
                if (result[k] == ' ')
                    result = ReplaceCharInString(result, k, sentence[i]);
                if (result[k] == '☺' || result[k] == '☻')
                {
                    k++;
                    i--;
                }
                else
                    k++;
            }

            return result;
        }

        private static string GetWordLang(string word) // определяем алфавит для слова
        {
            word = word.ToLower();
            // так же считаем кол-во цифр в слове и отнимаем от его длины
            char[] wordArray = word.ToCharArray();
            int countEng = wordArray.Where((n) => n >= 'a' && n <= 'z').Count();
            int countRus = wordArray.Where((n) => n >= 'а' && n <= 'я').Count();
            int countNum = wordArray.Where((n) => n >= 33 && n <= 64).Count();
            // если в слове есть цифра - не считаем его
            if (countNum > 0)
                return "unknown";
            int wordLen = word.Length - countNum;
            // если число, или от слова почти ничего не осталось - вернем ничего 
            if (wordLen <= 3)
                return "unknown";
            if (countEng >= countRus)
            {
                if (countEng >= word.Length / 2)
                    return "eng";
            }
            else
            {
                if (countRus >= word.Length / 2)
                    return "rus";
            }
            return "unknown";
        }

        private static bool HasLetterToChange(string word, string wordLang = "rus") // определяем, есть ли слова, в которых можно заменить букву
        {
            bool foundWord = false;
            if (wordLang == "rus")
            {
                if (word.IndexOf('y') > -1) foundWord = true;
                if (word.IndexOf('Y') > -1) foundWord = true;
                if (word.IndexOf('k') > -1) foundWord = true;
                if (word.IndexOf('K') > -1) foundWord = true;
                if (word.IndexOf('e') > -1) foundWord = true;
                if (word.IndexOf('E') > -1) foundWord = true;
                if (word.IndexOf('H') > -1) foundWord = true;
                if (word.IndexOf('x') > -1) foundWord = true;
                if (word.IndexOf('B') > -1) foundWord = true;
                if (word.IndexOf('a') > -1) foundWord = true;
                if (word.IndexOf('A') > -1) foundWord = true;
                if (word.IndexOf('p') > -1) foundWord = true;
                if (word.IndexOf('P') > -1) foundWord = true;
                if (word.IndexOf('o') > -1) foundWord = true;
                if (word.IndexOf('O') > -1) foundWord = true;
                if (word.IndexOf('c') > -1) foundWord = true;
                if (word.IndexOf('C') > -1) foundWord = true;
                if (word.IndexOf('m') > -1) foundWord = true;
                if (word.IndexOf('M') > -1) foundWord = true;
                if (word.IndexOf('T') > -1) foundWord = true;
            }
            else if (wordLang == "eng")
            {
                if (word.IndexOf('у') > -1) foundWord = true;
                if (word.IndexOf('У') > -1) foundWord = true;
                if (word.IndexOf('к') > -1) foundWord = true;
                if (word.IndexOf('К') > -1) foundWord = true;
                if (word.IndexOf('е') > -1) foundWord = true;
                if (word.IndexOf('Е') > -1) foundWord = true;
                if (word.IndexOf('Н') > -1) foundWord = true;
                if (word.IndexOf('х') > -1) foundWord = true;
                if (word.IndexOf('В') > -1) foundWord = true;
                if (word.IndexOf('а') > -1) foundWord = true;
                if (word.IndexOf('А') > -1) foundWord = true;
                if (word.IndexOf('р') > -1) foundWord = true;
                if (word.IndexOf('Р') > -1) foundWord = true;
                if (word.IndexOf('о') > -1) foundWord = true;
                if (word.IndexOf('О') > -1) foundWord = true;
                if (word.IndexOf('с') > -1) foundWord = true;
                if (word.IndexOf('С') > -1) foundWord = true;
                if (word.IndexOf('м') > -1) foundWord = true;
                if (word.IndexOf('М') > -1) foundWord = true;
                if (word.IndexOf('Т') > -1) foundWord = true;
            }

            return foundWord;
        }

        private static int LetterCount(string word, string typeWord = "rus") // считаем буквы другого алфавита в слове
        {
            int count;
            word = word.ToLower();
            switch (typeWord)
            {
                case "rus":
                    count = word.ToCharArray().Where((n) => n >= 'a' && n <= 'z').Count();
                    break;
                case "eng":
                    count = word.ToCharArray().Where((n) => n >= 'а' && n <= 'я').Count();
                    break;
                default:
                    return 0;
            }
            return count;
        }

        public bool NavalnyParse(string sentence)
        {
            if (sentence != null)
            {
                sentence = ClearSentence(sentence);
                foreach (string word in sentence.Split(' '))
                {
                    if (word.Length > 3)
                    {
                        string wordLang = GetWordLang(word);
                        int letCount = LetterCount(word, wordLang);
                        if (letCount > 0)
                        {
                            if (HasLetterToChange(word, wordLang))
                                return true;
                        }
                    }
                }
                return false;
            }
            return false;
        }
    }
}
