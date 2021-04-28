namespace SimpleSession
{
    /// <summary>
    /// Session物件
    /// </summary>
    public class SessionObject
    {
        private readonly CacheDictionary cache = new CacheDictionary();

        public object this[string index]
        {
            get => GetObj(index);
            set => SetCache(index, value);
        }

        private void SetCache(string key, object value)
        {
            cache.Set(key, () => value);
        }

        private object GetObj(string key)
        {
            return cache.GetOrDefault(key, () => default(object));
        }
    }
}