using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SimpleSession
{
    /// <summary>
    /// 自己做簡單快取
    /// </summary>
    public class CacheDictionary : Dictionary<string, object>
    {
        /// <summary>
        /// 掌管物件存活時間的集合
        /// </summary>
        private readonly Dictionary<string, CancellationTokenSource> _expireContaner =
            new Dictionary<string, CancellationTokenSource>();

        /// <summary>
        /// 有效時間
        /// </summary>
        private int _defaultExpiration;

        public CacheDictionary() : this(defaultExpiration: 5)
        {
        }

        public CacheDictionary(int defaultExpiration)
        {
            _defaultExpiration = defaultExpiration;
        }

        public T GetOrDefault<T>(string key, Func<T> createDefault)
        {
            return GetOrDefault(key, createDefault, TimeSpan.FromMinutes(_defaultExpiration));
        }

        public T Set<T>(string key, Func<T> create)
        {
            return Set(key, create, TimeSpan.FromMinutes(_defaultExpiration));
        }

        public T GetOrDefault<T>(string key, Func<T> createDefault, TimeSpan expireIn)
        {
            return (T)(ContainsKey(key) ? this[key] : Set(key, createDefault, expireIn));
        }

        /// <summary>
        /// 設置快取對象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="create"></param>
        /// <param name="expireIn"></param>
        /// <returns></returns>
        public T Set<T>(string key, Func<T> create, TimeSpan expireIn)
        {
            //如果此Key被使用 將原本的內容移除
            if (_expireContaner.ContainsKey(key))
            {
                _expireContaner[key].Cancel();
                _expireContaner.Remove(key);
            }

            var expirationTokenSource = new CancellationTokenSource();
            var expirationToken = expirationTokenSource.Token;
            //物件快取
            Task.Delay(expireIn, expirationToken).ContinueWith(_ => Expire(key), expirationToken);

            _expireContaner[key] = expirationTokenSource;

            return (T)(this[key] = create());
        }

        /// <summary>
        /// 時間到移除內容
        /// </summary>
        /// <param name="key"></param>
        private void Expire(string key)
        {
            if (_expireContaner.ContainsKey(key))
                _expireContaner.Remove(key);

            Remove(key);
        }
    }
}