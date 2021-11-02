using cangulo.nuke.releasecreator.Constants;
using Nuke.Common.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace cangulo.nuke.releasecreator.Repository
{
    public interface IResultBagRepository
    {
        bool Any(string key);
        string GetResult(string key);
        string GetResultOrDefault(string key, string defaultValue);
        T GetResult<T>(string key) where T : class;
        T GetResultOrDefault<T>(string key, T defaultValue) where T : class;
        void AddOrReplaceResult<T>(string key, T value);
    }

    public class ResultBagRepository : IResultBagRepository
    {
        private readonly AbsolutePath ResultBagFilePath;

        public ResultBagRepository(AbsolutePath resultBagFilePath)
        {
            ResultBagFilePath = resultBagFilePath ?? throw new ArgumentNullException(nameof(resultBagFilePath));
        }

        private IDictionary<string, object> GetResultBagDictionary()
        {
            if (File.Exists(ResultBagFilePath))
            {
                var jsonString = File.ReadAllText(ResultBagFilePath);
                return JsonSerializer.Deserialize<IDictionary<string, object>>(jsonString, SerializerContants.DESERIALIZER_OPTIONS);
            }
            else
                return new Dictionary<string, object>();
        }

        public bool Any(string key)
            => GetResultBagDictionary().ContainsKey(key);

        public string GetResult(string key)
        {
            var resultBag = GetResultBagDictionary();
            if (resultBag.Keys.Any(x => x == key))
            {
                return resultBag[key].ToString();
            }
            else
                throw new Exception($"{key} not found in the result bag");
        }

        public string GetResultOrDefault(string key, string defaultValue)
        {
            var resultBag = GetResultBagDictionary();
            if (resultBag.ContainsKey(key))
                return resultBag[key] as string ?? defaultValue;

            return defaultValue;
        }

        public T GetResult<T>(string key) where T : class
        {
            var resultBag = GetResultBagDictionary();
            if (resultBag.Keys.Any(x => x == key))
            {
                var resultString = resultBag[key].ToString();
                return JsonSerializer.Deserialize<T>(resultString, SerializerContants.DESERIALIZER_OPTIONS);
            }
            else
                throw new Exception($"{key} not found in the result bag");
        }

        public T GetResultOrDefault<T>(string key, T defaultValue) where T : class
        {
            var resultBag = GetResultBagDictionary();
            if (resultBag.ContainsKey(key))
            {
                var resultString = resultBag[key].ToString();
                return JsonSerializer.Deserialize<T>(resultString, SerializerContants.DESERIALIZER_OPTIONS) ?? defaultValue;
            }

            return defaultValue;
        }

        public void AddOrReplaceResult<T>(string key, T value)
        {
            var resultBag = GetResultBagDictionary();

            if (resultBag.ContainsKey(key))
                resultBag[key] = value;
            else
                resultBag.Add(key, value);

            var jsonString = JsonSerializer.Serialize(resultBag, SerializerContants.SERIALIZER_OPTIONS);
            File.WriteAllText(ResultBagFilePath, jsonString);
        }

    }
}
