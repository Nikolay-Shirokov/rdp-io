namespace TextSimulator.App;

/// <summary>
/// Простая реализация Service Provider без внешних зависимостей
/// Используется для DI в приложении
/// </summary>
public class SimpleServiceProvider : IServiceProvider
{
    private readonly Dictionary<Type, object> _singletons = new();
    private readonly Dictionary<Type, Func<object>> _factories = new();

    /// <summary>
    /// Регистрирует singleton экземпляр
    /// </summary>
    public void RegisterSingleton<T>(T instance) where T : notnull
    {
        _singletons[typeof(T)] = instance;
    }

    /// <summary>
    /// Регистрирует singleton фабрику
    /// </summary>
    public void RegisterSingleton<T>(Func<T> factory) where T : notnull
    {
        _factories[typeof(T)] = () => factory();
    }

    /// <summary>
    /// Получает зарегистрированный сервис или выбрасывает исключение
    /// </summary>
    public T GetRequiredService<T>() where T : notnull
    {
        var type = typeof(T);

        // Проверяем сначала синглтоны
        if (_singletons.TryGetValue(type, out var singleton))
        {
            return (T)singleton;
        }

        // Проверяем фабрики
        if (_factories.TryGetValue(type, out var factory))
        {
            var instance = (T)factory();
            _singletons[type] = instance; // Кэшируем как singleton
            return instance;
        }

        throw new InvalidOperationException($"Service of type {type.Name} is not registered");
    }

    public object? GetService(Type serviceType)
    {
        if (_singletons.TryGetValue(serviceType, out var singleton))
        {
            return singleton;
        }

        if (_factories.TryGetValue(serviceType, out var factory))
        {
            var instance = factory();
            _singletons[serviceType] = instance;
            return instance;
        }

        return null;
    }
}
