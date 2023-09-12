namespace Hiyazcool
{
    public interface ISingleton<T> where T : class, IInitialize, ISingleton<T>, new()
    {
        public static T instance { get; private set; }
        public static T Create()
        {
            if (instance == null)
            {
                T obj = new();
                obj.Initialize();
                instance = obj;
                return obj;
            }
            return instance;
        }
        public static bool SingletonCheck(T obj) => instance == obj ? true : false;

    }
}