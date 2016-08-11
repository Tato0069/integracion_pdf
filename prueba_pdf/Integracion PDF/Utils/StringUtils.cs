using System;
using System.Collections.Generic;
using System.Linq;
//using MODI;

namespace IntegracionPDF.Integracion_PDF.Utils
{
    public static class StringUtils
    {
        private static int _index;

        public static string ConvertStringToHex(this string asciiString)
        {
            return asciiString.Aggregate("", (current, c) => current + $"{Convert.ToUInt32(((int) c).ToString()):x2}").Replace("a0","20").Replace("c2a0", "");
        }

        public static string ExtractTextFromImage(this string filePath)
        {
            //var modiDocument = new Document();
            //modiDocument.Create(Convert.ToString(filePath));
            //modiDocument.OCR(MiLANGUAGES.miLANG_ENGLISH);
            //var modiImage = (modiDocument.Images[0] as MODI.Image);
            //var extractedText = modiImage.Layout.Text;
            //modiDocument.Close();
            //return extractedText;
            return "Libreria no Encontrada";
        }

        public static string ConvertHexToString(this string hexValue)
        {
            var strValue = "";
            if (hexValue.Length < 2) return "";
            while (hexValue.Length > 0)
            {
                strValue += Convert.ToChar(Convert.ToUInt32(hexValue.Substring(0, 2), 16)).ToString();
                hexValue = hexValue.Substring(2, hexValue.Length - 2);
            }
            return strValue;
        }

        public static string DeleteNullHexadecimalValues(this string str)
        {
            return str.ConvertStringToHex().ConvertHexToString();
        }

        public static string NormalizeCentroCostoDavila(this string str)
        {
            return str.DeleteAcent().Replace("-", "").Replace("°", "").Replace("(", "").Replace(")", "").Replace("º","").Trim();
        }

        /// <summary>
        /// Elimina decimales en Precios o cantidades
        /// </summary>
        /// <param name="str">22.00</param>
        /// <returns>22</returns>
        public static string DeleteDecimal(this string str)
        {
            return str.Split('.')[0];
        }

        public static string DeleteDotComa(this string str)
        {
            return str.Replace(",", "").Replace(".","").Replace("$","");
        }

        /// <summary>
        /// Reemplaza una com "," por un punto "."
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceDot(this string str)
        {
            return str.Replace(",",".");
        }

        /// <summary>
        /// Elimina Espacios en Blanco repetidos
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DeleteContoniousWhiteSpace(this string str)
        {
            while (str.Contains("  "))
            {
                str = str.Replace("  ", " ");
            }
            //_index = 0;
            //var st = "";
            //var aux = str.ToCharArray();
            //for (; _index < aux.Length; _index++)
            //{
            //    if (aux[_index].Equals(' '))
            //    {
            //        st += DeleteAux2(aux);
            //    }
            //    else
            //    {
            //        st += aux[_index];
            //    }
            //}
            return str.Trim();
        }

        /// <summary>
        /// Borrar Tilde de String
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DeleteAcent(this string str)
        {
            return str
                .Replace("Á", "A")
                .Replace("É", "E")
                .Replace("Í", "I")
                .Replace("Ó", "O")
                .Replace("Ú", "U")
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ú", "u");
        }

        public static string FormattFolderName(this string str)
        {
            return str
                .Replace("Á", "A")
                .Replace("É", "E")
                .Replace("Í", "I")
                .Replace("Ó", "O")
                .Replace("Ú", "U")
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ú", "u")
                .Replace("/", "")
                .Replace("&", "")
                .Replace(":", "")
                .Replace(";", "")
                .Replace(".", "")
                .DeleteContoniousWhiteSpace();
        }

        public static string DeleteSymbol(this string str)
        {
            return str == null? null: str.Replace("+", "").Replace("*", "").Replace("¨", "")
                .Replace("´", "").Replace("{", "").Replace("}", "")
                .Replace("[", "").Replace("]", "").Replace("^", "")
                .Replace("`", "").Replace("-", "").Replace("_", "")
                .Replace(":", "").Replace(".", "").Replace(",", "")
                .Replace(";", "").Replace("(", "").Replace(")", "")
                .Replace("=", "").Replace("?", "").Replace(@"\", "")
                .Replace("¿", "").Replace("¡", "").Replace("'", "")
                .Replace(@"/", "").Replace("&", "").Replace("%", "")
                .Replace("$", "").Replace("#", "").Replace("\"", "")
                .Replace("!", "").Replace("°", "").Replace("|", "")
                .Replace("¬", "").Replace("<", "").Replace(">", "")
                .Replace("~", "").Replace("{", "");
        }

        public static string ReplaceSymbolWhiteSpace(this string str)
        {
            return str.Replace("+", " ").Replace("*", " ").Replace("¨", " ")
                .Replace("´", " ").Replace("{", " ").Replace("}", "")
                .Replace("[", " ").Replace("]", " ").Replace("^", " ")
                .Replace("`", " ").Replace("-", " ").Replace("_", " ")
                .Replace(":", " ").Replace(".", " ").Replace(",", " ")
                .Replace(";", " ").Replace("(", " ").Replace(")", " ")
                .Replace("=", " ").Replace("?", " ").Replace("\\", " ")
                .Replace("¿", " ").Replace("¡", " ").Replace("'", " ")
                .Replace("/", " ").Replace("&", " ").Replace("%", " ")
                .Replace("$", " ").Replace("#", " ").Replace("\"", " ")
                .Replace("!", " ").Replace("°", " ").Replace("|", " ")
                .Replace("¬", " ").Replace("<", " ").Replace(">", " ")
                .Replace("~", " ").Replace("{", " ").DeleteContoniousWhiteSpace();
        }

        public static string DeleteNumber(this string str)
        {
            return str.Replace("0", "").Replace("1", "").Replace("2", "")
                .Replace("3", "").Replace("4", "").Replace("5", "")
                .Replace("6", "").Replace("7", "").Replace("8", "")
                .Replace("9", "");
        }

        private static string DeleteAux2(char[] c)
        {
            if (_index > c.Length) return "";
            if (!c[_index].Equals(' ') 
                || !c[_index + 1].Equals(' '))
                return c[_index++] + DeleteAux2(c);
            _index++;
            return "" + DeleteAux2(c);
        }

        /// <summary>
        /// Retorna las palabras consecutivas que más se repiten
        /// </summary>
        /// <param name="st1"></param>
        /// <param name="st2"></param>
        /// <returns></returns>
        private static string GetModaString(this string st1, string st2)
        {
            var print = "";
            var str1 = st1.Trim().Split(' ');
            var str2 = st2.Trim().Split(' ');
            for (var i = 1; i < str1.Length && i < str2.Length; i++)
            {
                if (str1[str1.Length - i].Equals(str2[str2.Length - i]))
                {
                    print += str1[str1.Length - i] + " ";
                }
            }
            return print.RevertString();
        }

        /// <summary>
        /// Invierte un String
        /// </summary>
        /// <param name="st">ANITA LAVA LA TINA</param>
        /// <returns>ANIT AL AVAL ATINA</returns>
        private static string RevertString(this string st)
        {
            var ret = "";
            var str = st.Split(' ');
            for (var i = str.Length - 1; i >= 0; i--)
            {
                ret += str[i] + " ";
            }
            return ret.Trim();
        }


       
        /// <summary>
        /// Retorna un String desde un Arrego, entre las posiciones dadas
        /// </summary>
        /// <param name="arg">Arreglo</param>
        /// <param name="from">Indice inicio</param>
        /// <param name="to">Indice fin</param>
        /// <returns></returns>
        public static string ArrayToString(this string[] arg, int from, int to)
        {
            var ret = "";
            for (; from <= to; from++)
            {
                ret += " " + arg[from];
            }
            return ret.Trim();
        }

        private static string GetMax(this IEnumerable<Lista> lista)
        {
            int[] max = {0};
            var ret = "";
            foreach (var l in lista.Where(l => l.Repeticiones > max[0] && !l.Cadena.Equals("")))
            {
                max[0] = l.Repeticiones;
                ret = l.Cadena;
            }
            return ret;
        }

        /// <summary>
        /// Obtiene Direccion de Formato Cencosud
        /// </summary>
        /// <param name="it">Lista de Items</param>
        /// <returns>Dirección</returns>
        public static string GetDireccionCencosud(this List<string> it)
        {
            var aux1 = new List<Lista>();
            var aux2 = new List<Lista>();
            for (var i = 0; i < it.Count; i += 2)
            {
                for (var j = i + 1; j < it.Count; j++)
                {
                    var str = it[i].GetModaString(it[j]);
                    aux1.Add(new Lista { Repeticiones = 0, Cadena = str });
                }
            }
            for (var i = 1; i < it.Count; i += 2)
            {
                for (var j = i + 1; j < it.Count; j++)
                {
                    var str = it[i].GetModaString(it[j]);
                    aux2.Add(new Lista { Repeticiones = 0, Cadena = str });
                }
            }
            foreach (var a in from a in aux1 from i in it.Where(i => i.Contains(a.Cadena)) select a)
            {
                a.Repeticiones++;
            }
            foreach (var a in from a in aux2 from i in it.Where(i => i.Contains(a.Cadena)) select a)
            {
                a.Repeticiones++;
            }
            var re1 = aux1.GetMax();
            var re2 = aux2.GetMax();
            if (!re1.Contains(re2)) return re1.Trim() +" "+ re2.Trim();
            return re1;
        }
    }

    public class Lista
    {
        public int Repeticiones { get; set; }
        public string Cadena { get; set; }
    }
}