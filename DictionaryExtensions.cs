using System;
using System.Collections.Generic;
using System.Linq;

namespace SpooferHWID
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Compara dois dicionários e retorna as diferenças entre eles
        /// </summary>
        /// <param name="before">Dicionário original</param>
        /// <param name="after">Dicionário após a modificação</param>
        /// <returns>Um dicionário com as informações de alteração</returns>
        public static Dictionary<string, (bool Exists, string BeforeValue, string AfterValue, ChangeType Type)>
            CompareWith(this Dictionary<string, string> before, Dictionary<string, string> after)
        {
            var result = new Dictionary<string, (bool, string, string, ChangeType)>();

            // Verificar todas as chaves em ambos os dicionários
            foreach (var key in before.Keys.Union(after.Keys))
            {
                bool beforeHasKey = before.ContainsKey(key);
                bool afterHasKey = after.ContainsKey(key);

                if (!beforeHasKey && afterHasKey)
                {
                    // Valor novo adicionado
                    result[key] = (true, string.Empty, after[key], ChangeType.Added);
                }
                else if (beforeHasKey && !afterHasKey)
                {
                    // Valor removido
                    result[key] = (false, before[key], string.Empty, ChangeType.Removed);
                }
                else if (beforeHasKey && afterHasKey)
                {
                    if (before[key] != after[key])
                    {
                        // Valor alterado
                        result[key] = (true, before[key], after[key], ChangeType.Modified);
                    }
                    else
                    {
                        // Valor não alterado
                        result[key] = (false, before[key], after[key], ChangeType.Unchanged);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Retorna os valores que foram alterados em um dicionário de comparação
        /// </summary>
        public static Dictionary<string, (bool Success, string BeforeValue, string AfterValue, ChangeType Type)>
            GetChangedValues(this Dictionary<string, (bool Exists, string BeforeValue, string AfterValue, ChangeType Type)> compareResult)
        {
            return compareResult
                .Where(kvp => kvp.Value.Type != ChangeType.Unchanged)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }

    /// <summary>
    /// Tipo de alteração em um valor
    /// </summary>
    public enum ChangeType
    {
        Added,
        Removed,
        Modified,
        Unchanged
    }
}