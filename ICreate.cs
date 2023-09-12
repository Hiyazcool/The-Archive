namespace Hiyazcool
{
    public interface ICreate<T> where T : IInitialize, new()
    {
        public static T Create()
        {
            T obj = new();
            obj.Initialize();
            return obj;
        }
    }
}