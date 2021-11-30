﻿using System;
using System.Text;
using System.Text.RegularExpressions;

namespace RfcFacil
{
    internal class NaturalPersonTenDigitsCodeCalculator
    {
        private NaturalPerson person;

        private static readonly string VowelPattern = "[AEIOU]+";
        
        private static readonly string[] SpecialParticles = {
            "DE", "LA", "LAS", "MC", "VON", "DEL", "LOS", "Y", "MAC", "VAN", "MI"};

        private static readonly string[] ForbiddenWords = {
            "BUEI", "BUEY", "CACA", "CACO", "CAGA", "KOGE", "KAKA", "MAME", "KOJO", "KULO",
            "CAGO", "COGE", "COJE", "COJO", "FETO", "JOTO", "KACO", "KAGO", "MAMO", "MEAR", "MEON",
            "MION", "MOCO", "MULA", "PEDA", "PEDO", "PENE", "PUTA", "PUTO", "QULO", "RATA", "RUIN"
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="person"></param>
        public NaturalPersonTenDigitsCodeCalculator(NaturalPerson person)
        {
            this.person = person;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Calculate()
        {
            return ObfuscateForbiddenWords(NameCode()) + BirthdayCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameCode"></param>
        /// <returns></returns>
        private string ObfuscateForbiddenWords(string nameCode) 
        {
            foreach(var forbidden in ForbiddenWords)
            {
                if (forbidden.Equals(nameCode))
                {
                    return string.Format("{0}{1}{2}", nameCode.Substring(0, 1), "X", nameCode.Substring(2, 2));
                }
            }

            return nameCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string NameCode()
        {
            if (IsFirstLastNameEmpty())
            {
                return FirstLastNameEmptyForm();
            }
            else 
            {
                if (IsSecondLastNameEmpty())
                {
                    return SecondLastNameEmptyForm();
                }
                else
                {
                    if (IsFirstLastNameIsTooShort())
                    {
                        return FirstLastNameTooShortForm();
                    }
                    else
                    {
                        return NormalForm();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string BirthdayCode()
        {
            DateTime birthday = new DateTime(person.Year, person.Month, person.Day);

            return birthday.ToString("yyMMdd");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsFirstLastNameEmpty()
        {
            return string.IsNullOrEmpty(Normalize(person.FirstLastName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsSecondLastNameEmpty()
        {
            return string.IsNullOrEmpty(Normalize(person.SecondLastName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string FirstLastNameEmptyForm()
        {
            return 
                FirstTwoLettersOf(person.SecondLastName) + 
                FirstTwoLettersOf(FilterName(person.Name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string SecondLastNameEmptyForm()
        {
            return 
                FirstTwoLettersOf(person.FirstLastName) + 
                FirstTwoLettersOf(FilterName(person.Name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsFirstLastNameIsTooShort()
        {
            return Normalize(person.FirstLastName).Length <= 2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string FirstLastNameTooShortForm()
        {
            return 
                FirstLetterOf(person.FirstLastName) + 
                FirstLetterOf(person.SecondLastName) + 
                FirstTwoLettersOf(FilterName(person.Name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string NormalForm()
        {
            return 
                FirstLetterOf(person.FirstLastName) + 
                FirstVowelExcludingFirstCharacterOf(person.FirstLastName) + 
                FirstLetterOf(person.SecondLastName) + 
                FirstLetterOf(FilterName(person.Name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private string FirstVowelExcludingFirstCharacterOf(string word)
        {
            string normalizedWord = Normalize(word).Substring(1);
            
            Match m = Regex.Match(normalizedWord, VowelPattern);
            
            if (m.Length <= 0)
                return "X";

            return normalizedWord[m.Index].ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private string FirstTwoLettersOf(string word)
        {
            string normalizedWord = Normalize(word);
            
            return normalizedWord.Substring(0, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string FilterName(string name)
        {
            bool isPopularGivenName = false;
            string rawName = Normalize(name).Trim();

            if (rawName.Contains(" "))
            {
                isPopularGivenName = rawName.StartsWith("MARIA") || rawName.StartsWith("JOSE");

                if (isPopularGivenName)
                {
                    return rawName.Split(' ')[1];
                }
            }
            return name;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private string FirstLetterOf(string word)
        {
            string normalizedWord = Normalize(word);

            return normalizedWord[0].ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private string Normalize(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return word;
            }
            else
            {
                string normalizedWord = RfcUtils.StripAccents(word).ToUpper();
                return RemoveSpecialParticles(normalizedWord, SpecialParticles);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="normalizedWord"></param>
        /// <param name="SpecialParticles"></param>
        /// <returns></returns>
        private string RemoveSpecialParticles(string normalizedWord, string[] SpecialParticles)
        {
            StringBuilder newWord = new StringBuilder(normalizedWord);

            foreach(var particle in SpecialParticles)
            {
                var particlePositions = $" {particle} ";
                while (newWord.ToString().Contains(particlePositions))
                {
                    int i = newWord.ToString().IndexOf(particlePositions);
                    newWord.Remove(i, i + particlePositions.ToString().Length - 1);
                }
            }

            return newWord.ToString();
        }
    }
}
