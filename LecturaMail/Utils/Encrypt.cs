using System;
using System.Security.Cryptography;
using System.Text;

namespace LecturaMail.Utils
{
    public static class Encrypt
    {
        private const string Key = "POIUYTREWDIMERCESLACUMBIAZXCVBNM";

        public static string EncryptKey(string cadena)
        {
            //arreglo de bytes donde guardaremos la llave
            //arreglo de bytes donde guardaremos el texto
            //que vamos a encriptar
            var arregloACifrar = Encoding.UTF8.GetBytes(cadena);

            //se utilizan las clases de encriptación
            //provistas por el Framework
            //Algoritmo MD5
            var hashmd5 = new MD5CryptoServiceProvider();
            //se guarda la llave para que se le realice
            //hashing
            var keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(Key));

            hashmd5.Clear();

            //Algoritmo 3DAS
            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };


            //se empieza con la transformación de la cadena
            var cTransform = tdes.CreateEncryptor();

            //arreglo de bytes donde se guarda la
            //cadena cifrada
            var arrayResultado = cTransform.TransformFinalBlock(arregloACifrar,
                    0, arregloACifrar.Length);

            tdes.Clear();

            //se regresa el resultado en forma de una cadena
            return Convert.ToBase64String(arrayResultado,
                0, arrayResultado.Length);

        }

        public static string DecryptKey(string clave)
        {
            //convierte el texto en una secuencia de bytes
            var arrayADescifrar = Convert.FromBase64String(clave);

            //se llama a las clases que tienen los algoritmos
            //de encriptación se le aplica hashing
            //algoritmo MD5
            var hashmd5 = new MD5CryptoServiceProvider();

            var keyArray = hashmd5.ComputeHash(
                Encoding.UTF8.GetBytes(Key));

            hashmd5.Clear();

            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };


            var cTransform = tdes.CreateDecryptor();

            var resultArray =
                cTransform.TransformFinalBlock(arrayADescifrar,
                    0, arrayADescifrar.Length);

            tdes.Clear();
            //se regresa en forma de cadena
            return Encoding.UTF8.GetString(resultArray);
        }
    }
}