public interface IControlComponentReflection
{
    TReturn ReadProperty<TReturn>(string targetRole, string propertyName);
    void CallMethod(string targetRole, string methodName);
    void CallMethod<TParam>(string targetRole, string methodName, TParam param);
    TReturn CallMethod<TReturn>(string targetRole, string methodName);
    TReturn CallMethod<TParam, TReturn>(string targetRole, string methodName, TParam param);
    void Subscribe<T>(string targetRole, string eventName, T eventHandler);
    void Unsubscribe<T>(string targetRole, string eventName, T eventHandler);
}
